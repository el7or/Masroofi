using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.Constants;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.ATMCards;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Puzzle.Masroofi.Core.Resources;
using Puzzle.Masroofi.Core.ViewModels.Sons;
using Puzzle.Masroofi.Core.ViewModels.PushNotifications;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface IATMCardService
    {
        Task<PagedOutput<ATMCardSonOutput>> GetAllAsync(ATMCardFilterViewModel model);
        Task<ATMCardOutputViewModel> GetAsync(Guid id);
        Task<SonOutputViewModel> GetByCardIdAsync(long cardid);
        Task AddAsync(ATMCardInputViewModel model, bool updateCurrentATMCard = true);
        Task UpdateAsync(ATMCardInputViewModel model);
        Task<OperationState> DeleteAsync(Guid id);
        Task<OperationState> UpdatePasswordAsync(ATMCardPasswordViewModel model);
        Task<OperationState> UpdateStatusAsync(ATMCardStatusViewModel model);
        Task LostATMCardAsync(LostATMCardViewModel model);
        Task ReplacedATMCardAsync(ReplacedATMCardViewModel model);
        Task<decimal> GetBalanceAsync(Guid id);
        List<EnumValue> GetAtmCardStatus();
    }

    public class ATMCardService : BaseService, IATMCardService
    {
        private readonly IATMCardRepository _atmCardRepository;
        private readonly IATMCardTypeRepository _atmCradTypeRepository;
        private readonly ISonRepository _sonRepository;
        private readonly IParentRepository _parentRepository;
        private readonly UserIdentity _userIdentity;
        private readonly IConfiguration _configuration;
        private readonly string encryptionKey;
        private readonly IPushNotificationService _pushNotificationService;

        public ATMCardService(IATMCardRepository atmCardRepository,
            IATMCardTypeRepository atmCradTypeRepository,
            ISonRepository sonRepository,
            IParentRepository parentRepository,

            UserIdentity userIdentity,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IMapper mapper, IPushNotificationService pushNotificationService) : base(unitOfWork, mapper)
        {
            _atmCardRepository = atmCardRepository;
            _atmCradTypeRepository = atmCradTypeRepository;
            _sonRepository = sonRepository;
            _parentRepository = parentRepository;
            _userIdentity = userIdentity;
            _configuration = configuration;

            encryptionKey = _configuration.GetSection("Security:EncryptionKey").Value;
            _pushNotificationService = pushNotificationService;
        }

        public async Task<PagedOutput<ATMCardSonOutput>> GetAllAsync(ATMCardFilterViewModel model)
        {
            var result = new PagedOutput<ATMCardSonOutput>();

            var query = _atmCardRepository.Table
                .Include(a => a.ATMCardType)
                .Include(s => s.Son)
                .ThenInclude(s=> s.Parent) 
                .AsQueryable();
          
            // filtering
            query = FilterATMCards(query, model);
           
            // sorting
            var columnsMap = new Dictionary<string, Expression<Func<ATMCard, object>>>()
            {
                ["status"] = v => v.Status,
                ["firstName"] = v => v.FirstName,
                ["middleName"] = v => v.MiddleName,
                ["lastName"] = v => v.LastName,
                ["creationDate"] = v => v.CreationDate
            };
            query = query.ApplySorting(model, columnsMap);

            result.TotalCount = await query.CountAsync();

            // paging
            query = query.ApplyPaging(model);

            result.Result = mapper.Map<List<ATMCardSonOutput>>(query);

            return result;
        }

        public async Task<ATMCardOutputViewModel> GetAsync(Guid id)
        {
            var entityToGet = await _atmCardRepository.GetWhereIncludeAsync(a => a.ATMCardId == id, a => a.ATMCardType);

            if (entityToGet == null)
            {
                throw new BusinessException("ATMCard Not found!");
            }

            if (_userIdentity.Channel.Value == ChannelType.Admin)
            {
                if (string.IsNullOrEmpty(entityToGet.CardNumber) && entityToGet.Status == ATMCardStatus.Pending)
                {

                    if (_atmCardRepository.GetAny(aTMCard => aTMCard.CardNumber != null && aTMCard.CardNumber != ""))
                    {
                        var aTMCard = _atmCardRepository.Take(1, aTMCard => aTMCard.CardNumber).FirstOrDefault();
                        long card = long.Parse(aTMCard.CardNumber) + 1;
                        entityToGet.CardNumber = (card).ToString();
                        entityToGet.SecurityCode = entityToGet.CardNumber.Substring(entityToGet.CardNumber.Length - 3);
                        entityToGet.ShortNumber = entityToGet.CardNumber.Substring(entityToGet.CardNumber.Length - 6);
                        entityToGet.ExpiryDate = entityToGet.CreationDate.AddYears(3);

                    }
                    else
                    {
                        entityToGet.CardNumber = _configuration.GetSection("AtmCardNumber").Value;
                        entityToGet.SecurityCode = entityToGet.CardNumber.Substring(entityToGet.CardNumber.Length - 3);
                        entityToGet.ShortNumber = entityToGet.CardNumber.Substring(entityToGet.CardNumber.Length - 6);
                        entityToGet.ExpiryDate = entityToGet.CreationDate.AddYears(3);

                    }
                }
            }
            var result = mapper.Map<ATMCardOutputViewModel>(entityToGet);
       

            return result;
        }

        public async Task<SonOutputViewModel> GetByCardIdAsync(long cardid)
        {

            var entityToGet = await _atmCardRepository.GetWhereIncludeAsync(a => a.CardId == cardid, a => a.Son, a => a.Son.Parent);

            if (entityToGet == null)
            {
                throw new BusinessException(ValidatorsMessages.ATMCardNotfound);
            }
            else if (!entityToGet.Son.IsActive || !entityToGet.Son.Parent.IsActive || entityToGet.Status != ATMCardStatus.Active || entityToGet.Status != ATMCardStatus.Replaced)
            {
                throw new BusinessException(ValidatorsMessages.ATMCardDeactivated);
            }
            else
            {
                return mapper.Map<SonOutputViewModel>(entityToGet.Son);
            }
        }

        public async Task AddAsync(ATMCardInputViewModel model, bool updateCurrentATMCard = true)
        {
            model.ATMCardId = Guid.NewGuid();

            ValidateATMCard(model);

            var son = _sonRepository.GetWhereInclude(s => s.SonId == model.SonId, s => s.CurrentATMCard);

            if (son.CurrentATMCard != null &&
                (son.CurrentATMCard.Status == ATMCardStatus.Pending
                || son.CurrentATMCard.Status == ATMCardStatus.InProgress
                || son.CurrentATMCard.Status == ATMCardStatus.Active
                || son.CurrentATMCard.Status == ATMCardStatus.Received
                || son.CurrentATMCard.Status == ATMCardStatus.Shipping))
            {
                throw new BusinessException("Son already have another ATMCard!");
            }

            var entityToAdd = mapper.Map<ATMCard>(model);

            entityToAdd.Status = ATMCardStatus.Pending;
            entityToAdd.IsDeleted = false;
            entityToAdd.CreationDate = DateTime.UtcNow;
            entityToAdd.CreationUser = _userIdentity.Id.Value;

            _atmCardRepository.Add(entityToAdd);

            if (updateCurrentATMCard)
            {
                son.CurrentATMCardId = entityToAdd.ATMCardId;
                son.ModificationDate = DateTime.UtcNow;
                son.ModificationUser = _userIdentity.Id.Value;
            }

            // withdraw ATMCard cost from parent wallet
            var atmCardType = await _atmCradTypeRepository.GetAsync(model.ATMCardTypeId);
            var parent = await _parentRepository.Table
            .Include(p => p.ParentWalletTransactions)
                .ThenInclude(t => t.TransactionCommissions)
            .FirstOrDefaultAsync(p => p.ParentId == son.ParentId);
            if (atmCardType.Cost > 0)
            {
                // check if parent balance allow to withdraw
                if (parent != null && parent.CurrentBalance < atmCardType.Cost)
                    throw new BusinessException("Not enough balance in Parent Wallet for withdraw ATMCard cost! Current Balance is: " + parent.CurrentBalance);

                var transaction = new ParentWalletTransaction
                {
                    ATMCardId = model.ATMCardId,
                    SonId = model.SonId,
                    Amount = -atmCardType.Cost,
                    PaymentType = PaymentType.Withdraw,
                    TitleAr = TransactionTypesConstants.ParentWalletTransactionTypes.WithdrawFromParentWalletBySystem.TitleAr,
                    TitleEn = TransactionTypesConstants.ParentWalletTransactionTypes.WithdrawFromParentWalletBySystem.TitleEn,
                    DetailsAr = TransactionTypesConstants.ParentWalletTransactionTypes.WithdrawFromParentWalletBySystem.DetailsAr,
                    DetailsEn = TransactionTypesConstants.ParentWalletTransactionTypes.WithdrawFromParentWalletBySystem.DetailsEn,
                    CreationDate = DateTime.UtcNow,
                    IsSuccess = true,
                    IsActive = true
                };
                parent.ParentWalletTransactions.Add(transaction);
                parent.CurrentBalance += transaction.Amount;
            }

            if (await unitOfWork.CommitAsync() > 0)
            {
                await _pushNotificationService.CreatePushNotification(new PushNotificationInputViewModel
                {
                    NotificationType = PushNotificationType.WithdrawBySystem,
                    RecordId = parent.ParentId,
                    Amount = atmCardType.Cost
                });
                await _pushNotificationService.CreatePushNotification(new PushNotificationInputViewModel
                {
                    NotificationType = PushNotificationType.RequestNewATMCard,
                    RecordId = parent.ParentId
                });
                mapper.Map(entityToAdd, model);
            }
            else
                throw new BusinessException("Unable to add ATMCard!");
        }

        public async Task UpdateAsync(ATMCardInputViewModel model)
        {
            ValidateATMCard(model);

            var entityToUpdate = await _atmCardRepository.GetAsync(model.ATMCardId.Value);

            if (entityToUpdate == null)
            {
                throw new BusinessException("ATMCard Not found!");
            }

            mapper.Map(model, entityToUpdate);

            entityToUpdate.ModificationDate = DateTime.UtcNow;
            entityToUpdate.ModificationUser = _userIdentity.Id.Value;

            if (await unitOfWork.CommitAsync() > 0)
            {
                mapper.Map(entityToUpdate, model);
                if ( _userIdentity.Channel.Value == ChannelType.Admin)
                {
                    var enumDisplayStatus = (ATMCardStatus)model.Status;
                    ATMCardStatusViewModel statusmodel = new ATMCardStatusViewModel { ATMCardId = model.ATMCardId.Value, Status = enumDisplayStatus, SonId = model.SonId };
                    var statusresult = await UpdateStatusAsync(statusmodel);
                    if (statusresult == OperationState.None)
                        throw new BusinessException("Unable to update ATMCard Status!");
                }
            }
            else
                throw new BusinessException("Unable to update ATMCard!");
        }
        
        public async Task<OperationState> DeleteAsync(Guid id)
        {
            var entityToDelete = await _atmCardRepository.GetAsync(id);
            if (entityToDelete == null)
            {
                throw new BusinessException("ATMCard not found");
            }
            entityToDelete.IsDeleted = true;
            entityToDelete.Status = ATMCardStatus.Deactivated;
            entityToDelete.ModificationDate = DateTime.UtcNow;
            entityToDelete.ModificationUser = _userIdentity.Id.Value;
            return await unitOfWork.CommitAsync() > 0 ? OperationState.Deleted : OperationState.None;
        }

        public async Task<OperationState> UpdatePasswordAsync(ATMCardPasswordViewModel model)
        {
            var errors = new List<Exception>();

            if (model.ATMCardId == Guid.Empty)
                errors.Add(new Exception("ATMCardId is required!"));

            if (model.SonId == Guid.Empty)
                errors.Add(new Exception("SonId is required!"));

            if (string.IsNullOrEmpty(model.Password))
                errors.Add(new Exception("Password is required!"));
            else
            {
                var password = Encryption.DecryptData(model.Password.Replace(" ", ""), encryptionKey);
                if (!password.IsValidThreeDigits())
                    errors.Add(new Exception("Password must be 3 digits only!"));
            }
            if (errors.Count > 0)
                throw new AggregateException(errors);

            var entityToUpdate = await _atmCardRepository.GetAsync(model.ATMCardId);

            if (entityToUpdate == null)
                throw new BusinessException("ATMCard not found!");

            entityToUpdate.ATMCardHistories.Add(new ATMCardHistory
            {
                Password = Encryption.DecryptData(entityToUpdate.Password, encryptionKey),
                HistoryType = ATMCardHistoryType.ChangePassword,
                UpdatedBy = _userIdentity.Id.Value,
                UpdatedOn = DateTime.UtcNow
            });
            entityToUpdate.Password = model.Password;
            entityToUpdate.ModificationDate = DateTime.UtcNow;
            entityToUpdate.ModificationUser = _userIdentity.Id;

            return await unitOfWork.CommitAsync() > 0 ? OperationState.Updated : OperationState.None;
        }

        public async Task<OperationState> UpdateStatusAsync(ATMCardStatusViewModel model)
        {
            var errors = new List<Exception>();

            if (model.ATMCardId == Guid.Empty)
                errors.Add(new Exception("ATMCardId is required!"));

            if (model.SonId == Guid.Empty)
                errors.Add(new Exception("SonId is required!"));

            if (!_sonRepository.GetAny(p => p.SonId == model.SonId))
                errors.Add(new Exception("Son Not found!"));

            if (!model.Status.IsEnumValid())
                errors.Add(new Exception("Status is required!"));

            if (model.Status == ATMCardStatus.Lost)
                errors.Add(new Exception("Can not update to this status from here, please use LostATMCard api!"));

            if (model.Status == ATMCardStatus.Replaced)
                errors.Add(new Exception("Can not update to this status from here, please use ReplacedATMCard api!"));

            if (model.Status == ATMCardStatus.Rejected && string.IsNullOrEmpty(model.RejectedReason))
                errors.Add(new Exception("RejectedReason is required when Status is Rejected!"));

            if (model.Status == ATMCardStatus.Active && string.IsNullOrEmpty(model.Password))
                errors.Add(new Exception("Password is required when Status is Active!"));

            if (!string.IsNullOrEmpty(model.Password))
            {
                var decryptedPassword = Encryption.DecryptData(model.Password.Replace(" ", ""), encryptionKey);
                if (!decryptedPassword.IsValidThreeDigits())
                    errors.Add(new Exception("Password must be 3 digits only!"));
            }

            var entityToUpdate = await _atmCardRepository.GetWhereIncludeAsync(a => a.ATMCardId == model.ATMCardId, a => a.Son);
            if (entityToUpdate == null)
                errors.Add(new Exception("ATMCard not found!"));

            if (errors.Count > 0)
                throw new AggregateException(errors);

            entityToUpdate.ATMCardHistories.Add(new ATMCardHistory
            {
                Status = model.Status,
                HistoryType = ATMCardHistoryType.ChangeStatus,
                UpdatedBy = _userIdentity.Id.Value,
                UpdatedOn = DateTime.UtcNow
            });
            entityToUpdate.Status = model.Status;
            entityToUpdate.Password = model.Password;
            entityToUpdate.ModificationDate = DateTime.UtcNow;
            entityToUpdate.ModificationUser = _userIdentity.Id;

            // if activate atm card for son who have another replaced atm card 
            var son = await _sonRepository.GetWhereIncludeAsync(s => s.SonId == model.SonId, s => s.CurrentATMCard);
            if (model.Status == ATMCardStatus.Active && son.CurrentATMCard.Status == ATMCardStatus.Replaced)
            {
                son.CurrentATMCard.Status = ATMCardStatus.Deactivated;
                son.CurrentATMCard.ATMCardHistories.Add(new ATMCardHistory
                {
                    Status = ATMCardStatus.Deactivated,
                    HistoryType = ATMCardHistoryType.ChangeStatus,
                    UpdatedBy = _userIdentity.Id.Value,
                    UpdatedOn = DateTime.UtcNow
                });
                son.CurrentATMCardId = model.ATMCardId;
                son.ModificationDate = DateTime.UtcNow;
                son.ModificationUser = _userIdentity.Id.Value;
            }

            await _pushNotificationService.CreatePushNotification(new PushNotificationInputViewModel
            {
                NotificationType = PushNotificationType.ATMCardStatusChanged,
                RecordId = entityToUpdate.Son.ParentId,
                ATMCardStatus = model.Status
            });

            return await unitOfWork.CommitAsync() > 0 ? OperationState.Updated : OperationState.None;
        }

        public async Task LostATMCardAsync(LostATMCardViewModel model)
        {
            var errors = new List<Exception>();

            if (model.CurrentATMCardId == Guid.Empty)
                errors.Add(new Exception("ATMCardId is required!"));

            if (model.NewATMCard.SonId == Guid.Empty)
                errors.Add(new Exception("SonId is required!"));

            if (!_sonRepository.GetAny(p => p.SonId == model.NewATMCard.SonId))
                errors.Add(new Exception("Son Not found!"));

            var entityToUpdate = await _atmCardRepository.GetWhereIncludeAsync(a => a.ATMCardId == model.CurrentATMCardId, a => a.Son);

            if (entityToUpdate == null)
            {
                errors.Add(new Exception("Current ATMCard Not found!"));
            }

            if (errors.Count > 0)
                throw new AggregateException(errors);

            entityToUpdate.ATMCardHistories.Add(new ATMCardHistory
            {
                Status = entityToUpdate.Status,
                HistoryType = ATMCardHistoryType.ChangeStatus,
                UpdatedBy = _userIdentity.Id.Value,
                UpdatedOn = DateTime.UtcNow
            });
            entityToUpdate.Status = ATMCardStatus.Lost;
            entityToUpdate.ModificationDate = DateTime.UtcNow;
            entityToUpdate.ModificationUser = _userIdentity.Id.Value;

            await _pushNotificationService.CreatePushNotification(new PushNotificationInputViewModel
            {
                NotificationType = PushNotificationType.ATMCardStatusChanged,
                RecordId = entityToUpdate.Son.ParentId,
                ATMCardStatus = ATMCardStatus.Lost
            });

            await AddAsync(model.NewATMCard);
        }

        public async Task ReplacedATMCardAsync(ReplacedATMCardViewModel model)
        {
            var errors = new List<Exception>();

            if (model.CurrentATMCardId == Guid.Empty)
                errors.Add(new Exception("ATMCardId is required!"));

            if (model.NewATMCard.SonId == Guid.Empty)
                errors.Add(new Exception("SonId is required!"));

            if (!_sonRepository.GetAny(p => p.SonId == model.NewATMCard.SonId))
                errors.Add(new Exception("Son Not found!"));

            var entityToUpdate = await _atmCardRepository.GetWhereIncludeAsync(a => a.ATMCardId == model.CurrentATMCardId, a => a.Son);

            if (entityToUpdate == null)
            {
                errors.Add(new Exception("Current ATMCard Not found!"));
            }

            if (errors.Count > 0)
                throw new AggregateException(errors);

            entityToUpdate.ATMCardHistories.Add(new ATMCardHistory
            {
                Status = entityToUpdate.Status,
                HistoryType = ATMCardHistoryType.ChangeStatus,
                UpdatedBy = _userIdentity.Id.Value,
                UpdatedOn = DateTime.UtcNow
            });
            entityToUpdate.Status = ATMCardStatus.Replaced;
            entityToUpdate.ModificationDate = DateTime.UtcNow;
            entityToUpdate.ModificationUser = _userIdentity.Id.Value;

            await _pushNotificationService.CreatePushNotification(new PushNotificationInputViewModel
            {
                NotificationType = PushNotificationType.ATMCardStatusChanged,
                RecordId = entityToUpdate.Son.ParentId,
                ATMCardStatus = ATMCardStatus.Replaced
            });

            await AddAsync(model.NewATMCard, false);
        }

        public async Task<decimal> GetBalanceAsync(Guid id)
        {
            var atmCard = await _atmCardRepository.GetWhereIncludeAsync(a => a.ATMCardId == id, a => a.ATMCardTransactions);
            if (atmCard == null)
            {
                throw new BusinessException("ATMCard Not found!");
            }
            return atmCard.ATMCardTransactions.Sum(t => t.Amount);
        }

        public List<EnumValue> GetAtmCardStatus()
        {
            List<EnumValue> values = new List<EnumValue>();
            foreach (ATMCardStatus itemType in Enum.GetValues(typeof(ATMCardStatus)))
            {
                //For each value of this enumeration, add a new EnumValue instance
                values.Add(new EnumValue
                {
                    Name = EnumExtensions.GetDescription(itemType),
                    Value = (int)itemType
                }); ;
            }
            return values;
        }

        private IQueryable<ATMCard> FilterATMCards(IQueryable<ATMCard> query, ATMCardFilterViewModel model)
        {
            if (model.SonId.HasValue)
            {
                query = query.Where(c => c.SonId == model.SonId);
            }
            if (model.ATMCardTypeId.HasValue)
            {
                query = query.Where(c => c.ATMCardTypeId == model.ATMCardTypeId);
            }
            if (model.ATMCardTypeId.HasValue)
            {
                query = query.Where(c => c.ATMCardTypeId == model.ATMCardTypeId);
            }
            if (model.Status.HasValue)
            {
                query = query.Where(c => c.Status == model.Status);
            }
            if (!string.IsNullOrEmpty(model.Name))
            {
                query = query.Where(c => c.FirstName.Contains(model.Name) || c.MiddleName.Contains(model.Name) || c.LastName.Contains(model.Name));
            }
            if (!string.IsNullOrEmpty(model.SonName))
            {
                query = query.Where(c => c.Son.SonNameEn.Contains(model.SonName) || c.Son.SonNameAr.Contains(model.SonName));
            }
            if (!string.IsNullOrEmpty(model.ParentName))
            {
                query = query.Where(c => c.Son.Parent.FullNameEn.Contains(model.ParentName) || c.Son.Parent.FullNameAr.Contains(model.ParentName));
            }

            return query;
        }

        private void ValidateATMCard(ATMCardInputViewModel model)
        {
            var errors = new List<Exception>();

            if (model.ATMCardId == null || model.ATMCardId == Guid.Empty)
                errors.Add(new Exception("ATMCardId is required!"));

            if (model.ATMCardTypeId == Guid.Empty)
                errors.Add(new Exception("ATMCardTypeId is required!"));

            if (string.IsNullOrEmpty(model.FirstName))
                errors.Add(new Exception("FirstName is required!"));
            else
            {
                if (!model.FirstName.IsValidEnglishLetters())
                    errors.Add(new Exception("FirstName must be English letters only!"));
            }

            if (string.IsNullOrEmpty(model.MiddleName))
                errors.Add(new Exception("MiddleName is required!"));
            else
            {
                if (!model.MiddleName.IsValidEnglishLetters())
                    errors.Add(new Exception("MiddleName must be English letters only!"));
            }

            if (string.IsNullOrEmpty(model.LastName))
                errors.Add(new Exception("LastName is required!"));
            else
            {
                if (!model.LastName.IsValidEnglishLetters())
                    errors.Add(new Exception("LastName must be English letters only!"));
            }

            if (!_sonRepository.GetAny(p => p.SonId == model.SonId))
                errors.Add(new Exception("Son Not found!"));

            if (!_atmCradTypeRepository.GetAny(p => p.ATMCardTypeId == model.ATMCardTypeId))
                errors.Add(new Exception("ATMCardType Not found!"));

            if (!string.IsNullOrEmpty(model.Password))
            {
                var decryptedPassword = Encryption.DecryptData(model.Password.Replace(" ", ""), encryptionKey);
                if (!decryptedPassword.IsValidThreeDigits())
                    errors.Add(new Exception("Password must be 3 digits only!"));
            }

            if (!string.IsNullOrEmpty(model.ShortNumber) && !model.ShortNumber.IsValidSixDigits())
                errors.Add(new Exception("ShortNumber must be 6 digits only!"));

            if (!string.IsNullOrEmpty(model.CardNumber) && !model.CardNumber.IsValidSixteenDigits())
                errors.Add(new Exception("CardNumber must be 16 digits only!"));

            if (!string.IsNullOrEmpty(model.SecurityCode) && !model.SecurityCode.IsValidThreeDigits())
                errors.Add(new Exception("SecurityCode must be 3 digits only!"));

            if (errors.Count > 0)
                throw new AggregateException(errors);
        }
    }
}
