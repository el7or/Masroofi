using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.Constants;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.ParentWalletTransactions;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Puzzle.Masroofi.Core.ViewModels.Commissions;
using Puzzle.Masroofi.Core.ViewModels.PushNotifications;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface IParentWalletTransactionService
    {
        Task ChargeByCreditCardAsync(ChargeByCreditCardViewModel model);
        Task UpdateChargeByCreditCardAsync(UpdateChargeByCreditCardViewModel model);
        Task ChargeByVendorAsync(ChargeByVendorViewModel model);
        Task ChargeByAdminAsync(ChargeByAdminViewModel model);
        Task WithdrawByAdminAsync(WithdrawByAdminViewModel model);
        Task<PagedOutput<ParentWalletTransactionOutputViewModel>> GetAllAsync(ParentWalletTransactionFilterViewModel model);
    }

    public class ParentWalletTransactionService : BaseService, IParentWalletTransactionService
    {
        private readonly IParentWalletTransactionRepository _parentWalletTransactionRepository;
        private readonly IParentRepository _parentRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IPOSMachineRepository _posMachineRepository;
        private readonly IPOSMachineTransactionRepository _posMachineTransactionRepository;
        private readonly ICommissionService _commissionService;
        private readonly UserIdentity _userIdentity;
        private readonly IConfiguration _configuration;
        private readonly string encryptionKey;
        private readonly IPushNotificationService _pushNotificationService;

        public ParentWalletTransactionService(IParentRepository parentRepository,
            ICommissionService commissionService,
            IVendorRepository vendorRepository,
            IPOSMachineRepository posMachineRepository,
            IParentWalletTransactionRepository parentWalletTransactionRepository,
            IPOSMachineTransactionRepository posMachineTransactionRepository,
            UserIdentity userIdentity,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IMapper mapper, IPushNotificationService pushNotificationService) : base(unitOfWork, mapper)
        {
            _parentRepository = parentRepository;
            _commissionService = commissionService;
            _vendorRepository = vendorRepository;
            _posMachineRepository = posMachineRepository;
            _parentWalletTransactionRepository = parentWalletTransactionRepository;
            _posMachineTransactionRepository = posMachineTransactionRepository;
            _userIdentity = userIdentity;
            _configuration = configuration;
            encryptionKey = _configuration.GetSection("Security:EncryptionKey").Value;
            _pushNotificationService = pushNotificationService;
        }

        public async Task ChargeByCreditCardAsync(ChargeByCreditCardViewModel model)
        {
            // validate model
            var errors = new List<Exception>();

            if (model.ParentId == Guid.Empty)
                errors.Add(new Exception("ParentId is required!"));

            var parent = await _parentRepository.GetAsync(model.ParentId);
            if (parent == null)
                errors.Add(new Exception("Parent Not found!"));

            if (!(model.Amount > 0))
                errors.Add(new Exception("Amount is required!"));

            if (!(model.FixedCommissionValue >= 0))
                errors.Add(new Exception("FixedCommissionValue is required!"));

            if (!(model.PercentageCommissionValue >= 0))
                errors.Add(new Exception("PercentageCommissionValue is required!"));

            if (errors.Count > 0)
                throw new AggregateException(errors);

            // check if commission is correct
            var commissionValues = await _commissionService.GetParentCommissionAsync(new ParentCommissionInputViewModel
            {
                ParentId = model.ParentId,
                Amount = model.Amount
            }, ChargeParentWalletCommissionType.ByCreditCard);
            if (!(commissionValues.FixedCommissionValue == model.FixedCommissionValue && commissionValues.PercentageCommissionValue == model.PercentageCommissionValue))
                throw new BusinessException("Wrong Commission Values!");

            var transaction = new ParentWalletTransaction
            {
                ParentId = model.ParentId,
                Amount = model.Amount,
                PaymentType = PaymentType.Visa,
                TitleAr = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletByCreditCard.TitleAr,
                TitleEn = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletByCreditCard.TitleEn,
                DetailsAr = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletByCreditCard.DetailsAr,
                DetailsEn = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletByCreditCard.DetailsEn,
                IsSuccess = true,
                IsActive = false,
                CreationDate = DateTime.UtcNow,
                TransactionCommissions = new List<TransactionCommission>
                {
                    new TransactionCommission
                    {
                        TransactionValue = model.Amount,
                        TransactionType = ChargeParentWalletCommissionType.ByCreditCard,
                        FixedCommissionValue = model.FixedCommissionValue,
                        PercentageCommissionValue = model.PercentageCommissionValue,
                        CreationDate = DateTime.UtcNow
                    }
                }
            };
            _parentWalletTransactionRepository.Add(transaction);

            if (await unitOfWork.CommitAsync() == 0)
            {
                transaction.IsSuccess = false;
                _parentWalletTransactionRepository.Add(transaction);
                await unitOfWork.CommitAsync();
                throw new BusinessException("Unable to add Transaction!");
            }
            else
            {
                model.ParentWalletTransactionId = transaction.ParentWalletTransactionId;
                await _pushNotificationService.CreatePushNotification(new PushNotificationInputViewModel
                {
                    NotificationType = PushNotificationType.ChargeByCreditCard,
                    RecordId = transaction.ParentId,
                    Amount = model.Amount
                });
            }
        }

        public async Task UpdateChargeByCreditCardAsync(UpdateChargeByCreditCardViewModel model)
        {
            // validate model
            var errors = new List<Exception>();

            if (string.IsNullOrEmpty(model.ParentWalletTransactionId))
                errors.Add(new Exception("ParentWalletTransactionId is required!"));

            if (string.IsNullOrEmpty(model.TransactionReference))
                errors.Add(new Exception("TransactionReference is required!"));

            if (string.IsNullOrEmpty(model.TransactionDataJson))
                errors.Add(new Exception("TransactionDataJson is required!"));

            if (errors.Count > 0)
                throw new AggregateException(errors);

            var decryptedId = Encryption.DecryptData(model.ParentWalletTransactionId.Replace(" ", ""), encryptionKey);
            var transaction = await _parentWalletTransactionRepository.GetWhereIncludeAsync(t => t.ParentWalletTransactionId == Guid.Parse(decryptedId), t => t.TransactionCommissions);
            if (transaction == null)
                throw new BusinessException("Parent Wallet Transaction Not found!");
            if (transaction.PaymentType != PaymentType.Visa)
                throw new BusinessException("Update only for Charge By Credit Card type!");
            if (transaction.IsActive)
                throw new BusinessException("Transaction is already updated!");

            transaction.IsActive = true;
            transaction.TransactionReference = model.TransactionReference;
            transaction.TransactionDataJson = model.TransactionDataJson;
            transaction.ModificationDate = DateTime.UtcNow;

            var parent = await _parentRepository.GetAsync(transaction.ParentId);
            parent.CurrentBalance += transaction.Amount;

            if (await unitOfWork.CommitAsync() == 0)
            {
                transaction.IsSuccess = false;
                _parentWalletTransactionRepository.Add(transaction);
                await unitOfWork.CommitAsync();
                throw new DbUpdateConcurrencyException("Concurrency conflict! Please retry.");
            }
        }

        public async Task ChargeByVendorAsync(ChargeByVendorViewModel model)
        {
            // validate model
            var errors = new List<Exception>();

            if ((model.ParentId == null || model.ParentId == Guid.Empty) && string.IsNullOrEmpty(model.ParentPhone))
                errors.Add(new Exception("Must Send one of ParentId or ParentPhone!"));

            if (!string.IsNullOrEmpty(model.ParentPhone) && !model.ParentPhone.IsValidShortPhoneNumber())
                errors.Add(new Exception("Send ParentPhone without country code e.g: 01234567899"));

            var parent = (model.ParentId == null || model.ParentId == Guid.Empty) && string.IsNullOrEmpty(model.ParentPhone)
                ? null
                : await _parentRepository.GetWhereAsync(p => string.IsNullOrEmpty(model.ParentPhone) ? p.ParentId == model.ParentId.Value : p.Phone.Contains(model.ParentPhone));
            if (parent == null)
                errors.Add(new Exception("Parent Not found!"));

            if (model.VendorId == Guid.Empty)
                errors.Add(new Exception("VendorId is required!"));

            var vendor = await _vendorRepository.GetAsync(model.VendorId);
            if (vendor == null)
                errors.Add(new Exception("Vendor Not found!"));

            if (model.POSMachineId == Guid.Empty)
                errors.Add(new Exception("POSMachineId is required!"));

            var posMachine = await _posMachineRepository.GetAsync(model.POSMachineId);
            if (posMachine == null)
                errors.Add(new Exception("POSMachine Not found!"));

            if (posMachine != null && posMachine.VendorId != model.VendorId)
                errors.Add(new Exception("Wrong VendorId for POSMachine!"));

            if (posMachine.PinCode.ToLower() != model.POSMachinePinCode.ToLower())
                errors.Add(new Exception("Wrong POSMachine PinCode!"));

            if (!(model.Amount > 0))
                errors.Add(new Exception("Amount is required!"));

            if (!(model.FixedCommissionValue >= 0))
                errors.Add(new Exception("FixedCommissionValue is required!"));

            if (!(model.PercentageCommissionValue >= 0))
                errors.Add(new Exception("PercentageCommissionValue is required!"));            

            if (errors.Count > 0)
                throw new AggregateException(errors);

            // check if commission is correct
            var commissionValues = await _commissionService.GetParentCommissionAsync(new ParentCommissionInputViewModel
            {
                ParentId = parent.ParentId,
                Amount = model.Amount
            }, ChargeParentWalletCommissionType.ByPOSMachine);
            if (!(commissionValues.FixedCommissionValue == model.FixedCommissionValue
                && commissionValues.PercentageCommissionValue == model.PercentageCommissionValue))
                throw new BusinessException("Wrong Commission Values!");

            // 1) discount amount and commission from pos:
            var posTransaction = new POSMachineTransaction
            {
                POSMachineTransactionId = Guid.NewGuid(),
                POSMachineId = model.POSMachineId,
                VendorId = model.VendorId,
                ParentId = model.ParentId,
                Amount = -(model.Amount + model.FixedCommissionValue + model.PercentageCommissionValue),
                PaymentType = PaymentType.Transfer,
                TitleAr = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletByVendor.TitleAr,
                TitleEn = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletByVendor.TitleEn,
                DetailsAr = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletByVendor.DetailsAr,
                DetailsEn = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletByVendor.DetailsEn,
                IsSuccess = true,
                CreationDate = DateTime.UtcNow
            };
            _posMachineTransactionRepository.Add(posTransaction);
            vendor.CurrentBalance += posTransaction.Amount;

            // 2) add amount to parent wallet include transaction commission:
            var commissionSetting = commissionValues.CommissionSettingId.HasValue ? await _commissionService.GetAsync(commissionValues.CommissionSettingId.Value) : null;
            int decimalPlacesNumber = int.Parse(_configuration.GetSection("decimalPlacesNumber").Value);

            var parentTransaction = new ParentWalletTransaction
            {
                ParentId = parent.ParentId,
                Amount = model.Amount,
                PaymentType = PaymentType.Transfer,
                TitleAr = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletByVendor.TitleAr,
                TitleEn = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletByVendor.TitleEn,
                DetailsAr = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletByVendor.DetailsAr,
                DetailsEn = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletByVendor.DetailsEn,
                IsSuccess = true,
                IsActive = true,
                CreationDate = DateTime.UtcNow,
                TransactionCommissions = new List<TransactionCommission>
                {
                    new TransactionCommission
                    {
                        POSMachineTransactionId = posTransaction.POSMachineTransactionId,
                        POSMachineId = model.POSMachineId,
                        VendorId = model.VendorId,
                        TransactionValue = model.Amount,
                        TransactionType = ChargeParentWalletCommissionType.ByPOSMachine,
                        FixedCommissionValue = model.FixedCommissionValue,
                        PercentageCommissionValue = model.PercentageCommissionValue,
                        VendorFixedCommissionValue = commissionSetting == null
                                                   ? 0
                                                   : commissionSetting.VendorFixedCommission,
                        VendorPercentageCommissionValue = commissionSetting == null
                                                        ? 0
                                                        : Math.Round(model.Amount * commissionSetting.VendorPercentageCommission / 100, decimalPlacesNumber),
                        CreationDate = DateTime.UtcNow
                    }
                }
            };
            _parentWalletTransactionRepository.Add(parentTransaction);
            parent.CurrentBalance += parentTransaction.Amount;

            // 3) add vendor commission to pos:
            var vendorCommissionTransaction = new POSMachineTransaction
            {
                POSMachineId = model.POSMachineId,
                VendorId = model.VendorId,
                ParentId = model.ParentId,
                Amount = commissionSetting == null ? 0 : commissionSetting.VendorFixedCommission + Math.Round(model.Amount * commissionSetting.VendorPercentageCommission / 100, decimalPlacesNumber),
                PaymentType = PaymentType.Transfer,
                TitleAr = TransactionTypesConstants.POSMachineTransactionTypes.AddVendorCommissionBySystem.TitleAr,
                TitleEn = TransactionTypesConstants.POSMachineTransactionTypes.AddVendorCommissionBySystem.TitleEn,
                DetailsAr = TransactionTypesConstants.POSMachineTransactionTypes.AddVendorCommissionBySystem.DetailsAr,
                DetailsEn = TransactionTypesConstants.POSMachineTransactionTypes.AddVendorCommissionBySystem.DetailsEn,
                IsSuccess = true,
                CreationDate = DateTime.UtcNow
            };
            _posMachineTransactionRepository.Add(vendorCommissionTransaction);
            vendor.CurrentBalance += vendorCommissionTransaction.Amount;

            if (await unitOfWork.CommitAsync() == 0)
            {
                parentTransaction.IsSuccess = posTransaction.IsSuccess = false;
                _parentWalletTransactionRepository.Add(parentTransaction);
                _posMachineTransactionRepository.Add(posTransaction);
                await unitOfWork.CommitAsync();
                throw new BusinessException("Unable to add Transactions!");
            }
            else
            {
                model.ParentWalletTransactionId = parentTransaction.ParentWalletTransactionId;
                model.POSMachineTransactionId = posTransaction.POSMachineTransactionId;
                await _pushNotificationService.CreatePushNotification(new PushNotificationInputViewModel
                {
                    NotificationType = PushNotificationType.ChargeByVendor,
                    RecordId = parent.ParentId,
                    Amount = model.Amount
                });
            }
        }

        public async Task ChargeByAdminAsync(ChargeByAdminViewModel model)
        {
            // validate model
            var errors = new List<Exception>();

            if (model.ParentId == Guid.Empty)
                errors.Add(new Exception("ParentId is required!"));

            var parent = await _parentRepository.GetAsync(model.ParentId);
            if (parent == null)
                errors.Add(new Exception("Parent Not found!"));

            if (!(model.Amount > 0))
                errors.Add(new Exception("Amount is required!"));

            if (errors.Count > 0)
                throw new AggregateException(errors);

            var transaction = new ParentWalletTransaction
            {
                ParentId = model.ParentId,
                AdminUserId = _userIdentity.Id,
                Amount = model.Amount,
                PaymentType = PaymentType.Cash,
                TitleAr = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletByAdmin.TitleAr,
                TitleEn = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletByAdmin.TitleEn,
                DetailsAr = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletByAdmin.DetailsAr,
                DetailsEn = TransactionTypesConstants.ParentWalletTransactionTypes.ChargeParentWalletByAdmin.DetailsEn,
                IsSuccess = true,
                IsActive = true,
                CreationDate = DateTime.UtcNow
            };
            _parentWalletTransactionRepository.Add(transaction);
            parent.CurrentBalance += transaction.Amount;

            if (await unitOfWork.CommitAsync() == 0)
            {
                transaction.IsSuccess = false;
                _parentWalletTransactionRepository.Add(transaction);
                await unitOfWork.CommitAsync();
                throw new BusinessException("Unable to add Transaction!");
            }
            else
            {
                model.ParentWalletTransactionId = transaction.ParentWalletTransactionId;
                await _pushNotificationService.CreatePushNotification(new PushNotificationInputViewModel
                {
                    NotificationType = PushNotificationType.ChargeByAdmin,
                    RecordId = parent.ParentId,
                    Amount = model.Amount
                });
            }
        }

        public async Task WithdrawByAdminAsync(WithdrawByAdminViewModel model)
        {
            // validate model
            var errors = new List<Exception>();

            if (model.ParentId == Guid.Empty)
                errors.Add(new Exception("ParentId is required!"));

            var parent = await _parentRepository.GetAsync(model.ParentId);
            if (parent == null)
                errors.Add(new Exception("Parent Not found!"));

            if (!(model.Amount > 0))
                errors.Add(new Exception("Amount is required!"));

            // check if parent balance allow to withdraw
            if (parent != null && parent.CurrentBalance < model.Amount)
                errors.Add(new Exception("Not enough balance in Parent Wallet for withdraw! Current Balance is: " + parent.CurrentBalance));

            if (errors.Count > 0)
                throw new AggregateException(errors);

            var transaction = new ParentWalletTransaction
            {
                ParentId = model.ParentId,
                AdminUserId = _userIdentity.Id,
                Amount = -model.Amount,
                PaymentType = PaymentType.Cash,
                TitleAr = TransactionTypesConstants.ParentWalletTransactionTypes.WithdrawFromParentWalletByAdmin.TitleAr,
                TitleEn = TransactionTypesConstants.ParentWalletTransactionTypes.WithdrawFromParentWalletByAdmin.TitleEn,
                DetailsAr = TransactionTypesConstants.ParentWalletTransactionTypes.WithdrawFromParentWalletByAdmin.DetailsAr,
                DetailsEn = TransactionTypesConstants.ParentWalletTransactionTypes.WithdrawFromParentWalletByAdmin.DetailsEn,
                IsSuccess = true,
                IsActive = true,
                CreationDate = DateTime.UtcNow
            };
            _parentWalletTransactionRepository.Add(transaction);
            parent.CurrentBalance += transaction.Amount;

            if (await unitOfWork.CommitAsync() == 0)
            {
                transaction.IsSuccess = false;
                _parentWalletTransactionRepository.Add(transaction);
                await unitOfWork.CommitAsync();
                throw new BusinessException("Unable to add Transaction!");
            }
            else
            {
                model.ParentWalletTransactionId = transaction.ParentWalletTransactionId;
                await _pushNotificationService.CreatePushNotification(new PushNotificationInputViewModel
                {
                    NotificationType = PushNotificationType.WithdrawByAdmin,
                    RecordId = parent.ParentId,
                    Amount = model.Amount
                });
            }
        }

        public async Task<PagedOutput<ParentWalletTransactionOutputViewModel>> GetAllAsync(ParentWalletTransactionFilterViewModel model)
        {
            var result = new PagedOutput<ParentWalletTransactionOutputViewModel>();

            var query = _parentWalletTransactionRepository.Table;

            // filtering
            if (model.ParentId.HasValue)
                query = query.Where(c => c.ParentId == model.ParentId);

            if (model.FromCreationDate.HasValue)
                query = query.Where(p => p.CreationDate.Date >= model.FromCreationDate.Value.Date);

            if (model.ToCreationDate.HasValue)
                query = query.Where(p => p.CreationDate.Date <= model.ToCreationDate.Value.Date);

            if (model.IsSuccess.HasValue)
                query = query.Where(p => p.IsSuccess == model.IsSuccess);

            if (model.IsActive.HasValue)
                query = query.Where(p => p.IsActive == model.IsActive);

            // sorting
            var columnsMap = new Dictionary<string, Expression<Func<ParentWalletTransaction, object>>>()
            {
                ["transactionNumber"] = v => v.TransactionNumber,
                ["amount"] = v => v.Amount,
                ["paymentType"] = v => v.PaymentType,
                ["title"] = v => _userIdentity.Language == Language.en ? v.TitleEn : v.TitleAr,
                ["details"] = v => _userIdentity.Language == Language.en ? v.DetailsEn : v.DetailsAr,
                ["creationDate"] = v => v.CreationDate,
                ["isSuccess"] = v => v.IsSuccess,
                ["isActive"] = v => v.IsActive
            };
            query = query.ApplySorting(model, columnsMap);

            result.TotalCount = await query.CountAsync();

            // paging
            query = query.ApplyPaging(model);

            result.Result = mapper.Map<List<ParentWalletTransactionOutputViewModel>>(query);

            return result;
        }
    }
}
