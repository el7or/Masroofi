using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.Constants;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.Sons;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Puzzle.Masroofi.Core.ViewModels.PushNotifications;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface ISonService
    {
        Task<PagedOutput<SonOutputViewModel>> GetAllAsync(SonFilterViewModel model);
        Task<SonOutputViewModel> GetAsync(Guid id);
        Task AddAsync(SonInputViewModel model);
        Task UpdateAsync(SonInputViewModel model);
        Task<OperationState> DeleteAsync(Guid id);
        Task<OperationState> ChangeActivationAsync(SonActivationViewModel model);
        Task<decimal> GetBalanceAsync(Guid id);
    }
    public class SonService : BaseService, ISonService
    {
        private readonly ISonRepository _sonRepository;
        private readonly IParentRepository _parentRepository;
        private readonly UserIdentity _userIdentity;
        private readonly IS3Service _s3Service;
        private readonly IPushNotificationService _pushNotificationService;

        public SonService(ISonRepository sonRepository,
            IParentRepository parentRepository,
            UserIdentity userIdentity,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IS3Service s3Service, IPushNotificationService pushNotificationService) : base(unitOfWork, mapper)
        {
            _sonRepository = sonRepository;
            _parentRepository = parentRepository;
            _userIdentity = userIdentity;
            _s3Service = s3Service;
            _pushNotificationService = pushNotificationService;
        }

        public async Task<PagedOutput<SonOutputViewModel>> GetAllAsync(SonFilterViewModel model)
        {
            var result = new PagedOutput<SonOutputViewModel>();

            var query = _sonRepository.Table
                .Include(a => a.ATMCards.OrderBy(c => c.CreationDate))
                    .ThenInclude(c => c.ATMCardType)
                .AsQueryable();

            // filtering
            query = FilterSons(query, model);

            // sorting
            var columnsMap = new Dictionary<string, Expression<Func<Son, object>>>()
            {
                ["sonName"] = v => _userIdentity.Language == Language.en ? v.SonNameEn : v.SonNameAr,
                ["gender"] = v => v.Gender,
                ["birthdate"] = v => v.Birthdate,
                ["dailyLimit"] = v => v.DailyLimit,
                ["currentBalance"] = v => v.CurrentBalance,
                ["creationDate"] = v => v.CreationDate
            };
            query = query.ApplySorting(model, columnsMap);

            result.TotalCount = await query.CountAsync();

            // paging
            query = query.ApplyPaging(model);

            result.Result = mapper.Map<List<SonOutputViewModel>>(query);

            return result;
        }

        public async Task<SonOutputViewModel> GetAsync(Guid id)
        {
            var entityToGet = await _sonRepository.Table
                .Include(a => a.ATMCards.OrderBy(c => c.CreationDate).Where(a => a.Status == ATMCardStatus.Active || a.Status == ATMCardStatus.InProgress || a.Status == ATMCardStatus.Pending || a.Status == ATMCardStatus.Received || a.Status == ATMCardStatus.Replaced || a.Status == ATMCardStatus.Shipping))
                    .ThenInclude(c => c.ATMCardType)
                .FirstOrDefaultAsync(s => s.SonId == id);

            if (entityToGet == null)
            {
                throw new BusinessException("Son Not found!");
            }
            return mapper.Map<SonOutputViewModel>(entityToGet);
        }

        public async Task AddAsync(SonInputViewModel model)
        {
            model.SonId = Guid.NewGuid();

            ValidateSon(model);

            if (model.NewImage == null || string.IsNullOrEmpty(model.NewImage.FileBase64))
                throw new BusinessException("NewImage is required!");

            var entityToAdd = mapper.Map<Son>(model);

            if (model.NewImage != null)
            {
                var fileBytes = Convert.FromBase64String(model.NewImage.FileBase64);
                var imagePath = _s3Service.UploadFile("Sons", model.NewImage.FileName, fileBytes);
                entityToAdd.ImageUrl = imagePath;
            }

            // add 20 amount to parent wallet if adding first son 
            var parent = await _parentRepository.Table
                .Include(s => s.Sons)
                .IgnoreQueryFilters()
                .Include(p => p.ParentWalletTransactions)
                    .ThenInclude(t => t.TransactionCommissions)
                .FirstOrDefaultAsync(p => p.ParentId == model.ParentId);
            if (parent.Sons.Count() == 0)
            {
                var transaction = new ParentWalletTransaction
                {
                    Amount = 20,
                    PaymentType = PaymentType.Gift,
                    TitleAr = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletBySystem.TitleAr,
                    TitleEn = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletBySystem.TitleEn,
                    DetailsAr = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletBySystem.DetailsAr,
                    DetailsEn = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletBySystem.DetailsEn,
                    CreationDate = DateTime.UtcNow,
                    IsSuccess = true,
                    IsActive = true
                };
                parent.ParentWalletTransactions.Add(transaction);
                parent.CurrentBalance += transaction.Amount;
            }

            entityToAdd.IsActive = true;
            entityToAdd.IsDeleted = false;
            entityToAdd.CreationDate = DateTime.UtcNow;
            entityToAdd.CreationUser = _userIdentity.Id.Value;
            _sonRepository.Add(entityToAdd);

            if (await unitOfWork.CommitAsync() > 0)
            {
                await _pushNotificationService.CreatePushNotification(new PushNotificationInputViewModel
                {
                    NotificationType = PushNotificationType.ChargeBySystem,
                    RecordId = entityToAdd.ParentId,
                    Amount = 20
                });
                mapper.Map(entityToAdd, model);
            }
            else
                throw new BusinessException("Unable to add Son!");
        }

        public async Task UpdateAsync(SonInputViewModel model)
        {
            ValidateSon(model);

            if ((model.NewImage == null || string.IsNullOrEmpty(model.NewImage.FileBase64)) && string.IsNullOrEmpty(model.ImageUrl))
                throw new BusinessException("NewImage or old ImageUrl is required!");

            var entityToUpdate = await _sonRepository.GetAsync(model.SonId.Value);

            if (entityToUpdate == null)
            {
                throw new BusinessException("Son Not found!");
            }

            mapper.Map(model, entityToUpdate);

            if (model.NewImage != null)
            {
                var fileBytes = Convert.FromBase64String(model.NewImage.FileBase64);
                entityToUpdate.ImageUrl = _s3Service.UploadFile("Sons", model.NewImage.FileName, fileBytes);
            }

            entityToUpdate.ModificationDate = DateTime.UtcNow;
            entityToUpdate.ModificationUser = _userIdentity.Id.Value;

            if (await unitOfWork.CommitAsync() > 0)
            {
                mapper.Map(entityToUpdate, model);
            }
            else
                throw new BusinessException("Unable to update Son!");
        }

        public async Task<OperationState> DeleteAsync(Guid id)
        {
            var entityToDelete = await _sonRepository.GetAsync(id);
            if (entityToDelete == null)
            {
                throw new BusinessException("Son Not found!");
            }
            entityToDelete.IsDeleted = true;
            entityToDelete.IsActive = false;
            entityToDelete.ModificationDate = DateTime.UtcNow;
            entityToDelete.ModificationUser = _userIdentity.Id.Value;
            return await unitOfWork.CommitAsync() > 0 ? OperationState.Deleted : OperationState.None;
        }

        public async Task<OperationState> ChangeActivationAsync(SonActivationViewModel model)
        {
            var entityToUpdate = await _sonRepository.GetAsync(model.SonId);

            if (entityToUpdate == null)
            {
                throw new BusinessException("Son Not found!");
            }

            entityToUpdate.IsActive = model.IsActive;
            entityToUpdate.ModificationDate = DateTime.UtcNow;
            entityToUpdate.ModificationUser = _userIdentity.Id.Value;

            return await unitOfWork.CommitAsync() > 0 ? OperationState.Updated : OperationState.None;
        }

        public async Task<decimal> GetBalanceAsync(Guid id)
        {
            var son = await _sonRepository.GetAsync(id);
            if (son == null)
            {
                throw new BusinessException("Son Not found!");
            }
            return son.CurrentBalance;
        }

        private IQueryable<Son> FilterSons(IQueryable<Son> query, SonFilterViewModel model)
        {
            if (model.ParentId.HasValue)
            {
                query = query.Where(c => c.ParentId == model.ParentId);
            }
            if (model.Gender.HasValue)
            {
                query = query.Where(c => c.Gender == model.Gender);
            }
            if (model.Birthdate.HasValue)
            {
                query = query.Where(c => c.Birthdate == model.Birthdate);
            }
            if (!string.IsNullOrEmpty(model.SonName))
            {
                query = query.Where(c => _userIdentity.Language == Language.en ? c.SonNameEn.Contains(model.SonName) :
                _userIdentity.Language == Language.ar ? c.SonNameAr.Contains(model.SonName) :
                c.SonNameAr.Contains(model.SonName) || c.SonNameEn.Contains(model.SonName));
            }

            return query;
        }

        private void ValidateSon(SonInputViewModel model)
        {
            var errors = new List<Exception>();

            if (!_parentRepository.GetAny(p => p.ParentId == model.ParentId))
                errors.Add(new Exception("Parent Not found!"));

            if (model.SonId == null || model.SonId == Guid.Empty)
                errors.Add(new Exception("SonId is required!"));

            if (model.ParentId == Guid.Empty)
                errors.Add(new Exception("ParentId is required!"));

            if (string.IsNullOrEmpty(model.SonNameAr))
                errors.Add(new Exception("SonNameAr is required!"));

            if (string.IsNullOrEmpty(model.SonNameEn))
                errors.Add(new Exception("SonNameEn is required!"));

            if (!((Gender)model.Gender).IsEnumValid())
                errors.Add(new Exception("Gender is required!"));

            if(!string.IsNullOrEmpty(model.Mobile) && !model.Mobile.IsValidPhoneNumber())
                errors.Add(new Exception("Invalid Mobile number!"));

            if (!(model.DailyLimit >= 0))
                errors.Add(new Exception("DailyLimit is required!"));

            if (errors.Count > 0)
                throw new AggregateException(errors);
        }
    }
}
