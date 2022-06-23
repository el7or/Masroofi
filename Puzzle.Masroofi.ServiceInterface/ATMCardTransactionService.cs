using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.Constants;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.ATMCardTransactions;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Puzzle.Masroofi.Core.ViewModels.PushNotifications;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface IATMCardTransactionService
    {
        Task ChargeByParentAsync(ChargeByParentViewModel model);
        Task RefundToParentAsync(RefundToParentViewModel model);
        Task PayToVendorAsync(PayToVendorViewModel model);
        Task RefundFromVendorAsync(RefundFromVendorViewModel model);

        Task<PagedOutput<ATMCardTransactionOutputViewModel>> GetAllAsync(ATMCardTransactionFilterViewModel model);
    }
    public class ATMCardTransactionService : BaseService, IATMCardTransactionService
    {
        private readonly IATMCardTransactionRepository _atmCardTransactionRepository;
        private readonly IATMCardRepository _atmCardRepository;
        private readonly IParentRepository _parentRepository;
        private readonly ISonRepository _sonRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IPOSMachineRepository _posMachineRepository;
        private readonly IParentWalletTransactionRepository _parentWalletTransactionRepository;
        private readonly IPOSMachineTransactionRepository _posMachineTransactionRepository;
        private readonly UserIdentity _userIdentity;
        private readonly IPushNotificationService _pushNotificationService;

        public ATMCardTransactionService(IATMCardTransactionRepository atmCardTransactionRepository,
            IATMCardRepository atmCardRepository,
            IParentRepository parentRepository,
            ISonRepository sonRepository,
            IVendorRepository vendorRepository,
            IPOSMachineRepository posMachineRepository,
            IParentWalletTransactionRepository parentWalletTransactionRepository,
            IPOSMachineTransactionRepository posMachineTransactionRepository,
            UserIdentity userIdentity,
            IUnitOfWork unitOfWork,
            IMapper mapper, IPushNotificationService pushNotificationService) : base(unitOfWork, mapper)
        {
            _atmCardTransactionRepository = atmCardTransactionRepository;
            _atmCardRepository = atmCardRepository;
            _parentRepository = parentRepository;
            _sonRepository = sonRepository;
            _vendorRepository = vendorRepository;
            _posMachineRepository = posMachineRepository;
            _parentWalletTransactionRepository = parentWalletTransactionRepository;
            _posMachineTransactionRepository = posMachineTransactionRepository;
            _userIdentity = userIdentity;
            _pushNotificationService = pushNotificationService;
        }

        public async Task ChargeByParentAsync(ChargeByParentViewModel model)
        {
            // validate model
            var errors = new List<Exception>();

            if (model.ATMCardId == Guid.Empty)
                errors.Add(new Exception("ATMCardId is required!"));

            if (model.SonId == Guid.Empty)
                errors.Add(new Exception("SonId is required!"));

            if (model.ParentId == Guid.Empty)
                errors.Add(new Exception("ParentId is required!"));

            var atmCard = await _atmCardRepository.GetAsync(model.ATMCardId);
            if (atmCard == null)
                errors.Add(new Exception("ATMCard Not found!"));

            var son = await _sonRepository.GetAsync(model.SonId);
            if (son == null)
                errors.Add(new Exception("Son Not found!"));

            if (son != null && son.CurrentATMCardId != model.ATMCardId)
                errors.Add(new Exception("Wrong current ATMCard for Son!"));

            if (son != null && son.ParentId != model.ParentId)
                errors.Add(new Exception("Wrong ParentId for Son!"));

            var parent = await _parentRepository.GetAsync(model.ParentId);
            if (parent == null)
                errors.Add(new Exception("Parent Not found!"));

            if (!(model.Amount > 0))
                errors.Add(new Exception("Amount is required!"));

            // check if parent balance allow to charge
            if (parent != null && parent.CurrentBalance < model.Amount)
                errors.Add(new Exception("Not enough balance in Parent Wallet for charge! Current Balance is: " + parent.CurrentBalance));

            if (errors.Count > 0)
                throw new AggregateException(errors);

            var atmCardTransaction = new ATMCardTransaction
            {
                ATMCardId = model.ATMCardId,
                SonId = model.SonId,
                ParentId = model.ParentId,
                Amount = model.Amount,
                PaymentType = PaymentType.Transfer,
                TitleAr = TransactionTypesConstants.ATMCardTransactionTypes.ChargeATMCardByParent.TitleAr,
                TitleEn = TransactionTypesConstants.ATMCardTransactionTypes.ChargeATMCardByParent.TitleEn,
                DetailsAr = TransactionTypesConstants.ATMCardTransactionTypes.ChargeATMCardByParent.DetailsAr,
                DetailsEn = TransactionTypesConstants.ATMCardTransactionTypes.ChargeATMCardByParent.DetailsEn,
                IsSuccess = true,
                CreationDate = DateTime.UtcNow
            };
            _atmCardTransactionRepository.Add(atmCardTransaction);
            //son.CurrentBalance = atmCard.ATMCardTransactions.Where(t => t.IsSuccess).Sum(t => t.Amount);
            son.CurrentBalance += atmCardTransaction.Amount;
            
            var parentTransaction = new ParentWalletTransaction
            {
                ParentId = model.ParentId,
                ATMCardId = model.ATMCardId,
                SonId = model.SonId,
                Amount = -model.Amount,
                PaymentType = PaymentType.Transfer,
                TitleAr = TransactionTypesConstants.ATMCardTransactionTypes.ChargeATMCardByParent.TitleAr,
                TitleEn = TransactionTypesConstants.ATMCardTransactionTypes.ChargeATMCardByParent.TitleEn,
                DetailsAr = TransactionTypesConstants.ATMCardTransactionTypes.ChargeATMCardByParent.DetailsAr,
                DetailsEn = TransactionTypesConstants.ATMCardTransactionTypes.ChargeATMCardByParent.DetailsEn,
                IsSuccess = true,
                IsActive = true,
                CreationDate = DateTime.UtcNow
            };
            _parentWalletTransactionRepository.Add(parentTransaction);
            parent.CurrentBalance += parentTransaction.Amount;

            if (await unitOfWork.CommitAsync() == 0)
            {
                atmCardTransaction.IsSuccess = parentTransaction.IsSuccess = false;
                _atmCardTransactionRepository.Add(atmCardTransaction);
                _parentWalletTransactionRepository.Add(parentTransaction);
                await unitOfWork.CommitAsync();
                throw new BusinessException("Unable to add Transactions!");
            }
            else
            {
                await _pushNotificationService.CreatePushNotification(new PushNotificationInputViewModel
                {
                    NotificationType = PushNotificationType.ChargeATMCard,
                    RecordId = parent.ParentId,
                    Amount = model.Amount
                });
            }
        }

        public async Task RefundToParentAsync(RefundToParentViewModel model)
        {
            // validate model
            var errors = new List<Exception>();

            if (model.ATMCardId == Guid.Empty)
                errors.Add(new Exception("ATMCardId is required!"));

            if (model.SonId == Guid.Empty)
                errors.Add(new Exception("SonId is required!"));

            if (model.ParentId == Guid.Empty)
                errors.Add(new Exception("ParentId is required!"));

            var atmCard = await _atmCardRepository.GetAsync(model.ATMCardId);
            if (atmCard == null)
                errors.Add(new Exception("ATMCard Not found!"));

            var son = await _sonRepository.GetAsync(model.SonId);
            if (son == null)
                errors.Add(new Exception("Son Not found!"));

            if (son != null && son.CurrentATMCardId != model.ATMCardId)
                errors.Add(new Exception("Wrong current ATMCard for Son!"));

            if (son != null && son.ParentId != model.ParentId)
                errors.Add(new Exception("Wrong ParentId for Son!"));

            var parent = await _parentRepository.GetAsync(model.ParentId);
            if (parent == null)
                errors.Add(new Exception("Parent Not found!"));

            if (!(model.Amount > 0))
                errors.Add(new Exception("Amount is required!"));

            // check if son balance allow to refund
            if (son != null && son.CurrentBalance < model.Amount)
                errors.Add(new Exception("Not enough balance in ATMCard for refund! Current Balance is: " + son.CurrentBalance));

            if (errors.Count > 0)
                throw new AggregateException(errors);

            var atmCardTransaction = new ATMCardTransaction
            {
                ATMCardId = model.ATMCardId,
                SonId = model.SonId,
                ParentId = model.ParentId,
                Amount = -model.Amount,
                PaymentType = PaymentType.Transfer,
                TitleAr = TransactionTypesConstants.ATMCardTransactionTypes.RefundFromATMCardToParent.TitleAr,
                TitleEn = TransactionTypesConstants.ATMCardTransactionTypes.RefundFromATMCardToParent.TitleEn,
                DetailsAr = TransactionTypesConstants.ATMCardTransactionTypes.RefundFromATMCardToParent.DetailsAr,
                DetailsEn = TransactionTypesConstants.ATMCardTransactionTypes.RefundFromATMCardToParent.DetailsEn,
                IsSuccess = true,
                CreationDate = DateTime.UtcNow
            };
            _atmCardTransactionRepository.Add(atmCardTransaction);
            //son.CurrentBalance = atmCard.ATMCardTransactions.Where(t => t.IsSuccess).Sum(t => t.Amount);
            son.CurrentBalance += atmCardTransaction.Amount;

            var parentTransaction = new ParentWalletTransaction
            {
                ParentId = model.ParentId,
                ATMCardId = model.ATMCardId,
                SonId = model.SonId,
                Amount = model.Amount,
                PaymentType = PaymentType.Transfer,
                TitleAr = TransactionTypesConstants.ATMCardTransactionTypes.RefundFromATMCardToParent.TitleAr,
                TitleEn = TransactionTypesConstants.ATMCardTransactionTypes.RefundFromATMCardToParent.TitleEn,
                DetailsAr = TransactionTypesConstants.ATMCardTransactionTypes.RefundFromATMCardToParent.DetailsAr,
                DetailsEn = TransactionTypesConstants.ATMCardTransactionTypes.RefundFromATMCardToParent.DetailsEn,
                IsSuccess = true,
                IsActive = true,
                CreationDate = DateTime.UtcNow
            };
            _parentWalletTransactionRepository.Add(parentTransaction);
            parent.CurrentBalance += parentTransaction.Amount;

            if (await unitOfWork.CommitAsync() == 0)
            {
                atmCardTransaction.IsSuccess = parentTransaction.IsSuccess = false;
                _atmCardTransactionRepository.Add(atmCardTransaction);
                _parentWalletTransactionRepository.Add(parentTransaction);
                await unitOfWork.CommitAsync();
                throw new BusinessException("Unable to add Transactions!");
            }
            else
            {
                await _pushNotificationService.CreatePushNotification(new PushNotificationInputViewModel
                {
                    NotificationType = PushNotificationType.RefundFromATMCard,
                    RecordId = parent.ParentId,
                    Amount = model.Amount
                });
            }
        }

        public async Task PayToVendorAsync(PayToVendorViewModel model)
        {
            // validate model
            var errors = new List<Exception>();

            if (model.ATMCardId == Guid.Empty)
                errors.Add(new Exception("ATMCardId is required!"));

            if (string.IsNullOrEmpty(model.ATMCardPassword))
                errors.Add(new Exception("Password is required!"));            

            if (model.SonId == Guid.Empty)
                errors.Add(new Exception("SonId is required!"));

            if (model.VendorId == Guid.Empty)
                errors.Add(new Exception("VendorId is required!"));

            var atmCard = await _atmCardRepository.GetAsync(model.ATMCardId);
            if (atmCard == null)
                errors.Add(new Exception("ATMCard Not found!"));
                        
            if (atmCard.Password.ToLower() != model.ATMCardPassword.ToLower())
                errors.Add(new Exception("Wrong  ATMCard Password!"));

            var son = await _sonRepository.GetAsync(model.SonId);
            if (son == null)
                errors.Add(new Exception("Son Not found!"));

            if (son != null && son.CurrentATMCardId != model.ATMCardId)
                errors.Add(new Exception("Wrong current ATMCard for Son!"));

            var vendor = await _vendorRepository.GetAsync(model.VendorId);
            if (vendor == null)
                errors.Add(new Exception("Vendor Not found!"));

            var posMachine = await _posMachineRepository.GetAsync(model.POSMachineId);

            if (posMachine == null)
                errors.Add(new Exception("POSMachine Not found!"));

            if (posMachine != null && posMachine.VendorId != model.VendorId)
                errors.Add(new Exception("Wrong VendorId for POSMachine!"));

            if (!(model.Amount > 0))
                errors.Add(new Exception("Amount is required!"));

            // check if son balance allow to pay
            if (son != null && son.CurrentBalance < model.Amount)
                errors.Add(new Exception("Not enough balance in ATMCard for pay! Current Balance is: " + son.CurrentBalance));

            if (errors.Count > 0)
                throw new AggregateException(errors);

            var atmCardTransaction = new ATMCardTransaction
            {
                ATMCardId = model.ATMCardId,
                SonId = model.SonId,
                VendorId = model.VendorId,
                POSMachineId = model.POSMachineId,
                Amount = -model.Amount,
                PaymentType = PaymentType.Transfer,
                TitleAr = TransactionTypesConstants.ATMCardTransactionTypes.PayByATMCardToVendor.TitleAr,
                TitleEn = TransactionTypesConstants.ATMCardTransactionTypes.PayByATMCardToVendor.TitleEn,
                DetailsAr = TransactionTypesConstants.ATMCardTransactionTypes.PayByATMCardToVendor.DetailsAr,
                DetailsEn = TransactionTypesConstants.ATMCardTransactionTypes.PayByATMCardToVendor.DetailsEn,
                IsSuccess = true,
                CreationDate = DateTime.UtcNow
            };
            _atmCardTransactionRepository.Add(atmCardTransaction);
            //son.CurrentBalance = atmCard.ATMCardTransactions.Where(t => t.IsSuccess).Sum(t => t.Amount);
            son.CurrentBalance += atmCardTransaction.Amount;

            var posTransaction = new POSMachineTransaction
            {
                POSMachineId = model.POSMachineId,
                VendorId = model.VendorId,
                ATMCardId = model.ATMCardId,
                SonId = model.SonId,
                Amount = model.Amount,
                PaymentType = PaymentType.Transfer,
                TitleAr = TransactionTypesConstants.ATMCardTransactionTypes.PayByATMCardToVendor.TitleAr,
                TitleEn = TransactionTypesConstants.ATMCardTransactionTypes.PayByATMCardToVendor.TitleEn,
                DetailsAr = TransactionTypesConstants.ATMCardTransactionTypes.PayByATMCardToVendor.DetailsAr,
                DetailsEn = TransactionTypesConstants.ATMCardTransactionTypes.PayByATMCardToVendor.DetailsEn,
                IsSuccess = true,
                CreationDate = DateTime.UtcNow
            };
            _posMachineTransactionRepository.Add(posTransaction);
            //vendor.CurrentBalance = posMachine.POSMachineTransactions.Where(t => t.IsSuccess).Sum(t => t.Amount + t.TransactionCommissions.Sum(c => c.FixedCommissionValue + c.PercentageCommissionValue));
            vendor.CurrentBalance += posTransaction.Amount;

            if (await unitOfWork.CommitAsync() == 0)
            {
                atmCardTransaction.IsSuccess = posTransaction.IsSuccess = false;
                _atmCardTransactionRepository.Add(atmCardTransaction);
                _posMachineTransactionRepository.Add(posTransaction);
                await unitOfWork.CommitAsync();
                throw new BusinessException("Unable to add Transactions!");
            }
            else
            {
                await _pushNotificationService.CreatePushNotification(new PushNotificationInputViewModel
                {
                    NotificationType = PushNotificationType.PayByATMCardToVendor,
                    RecordId = son.ParentId,
                    Amount = model.Amount
                });
            }
        }

        public async Task RefundFromVendorAsync(RefundFromVendorViewModel model)
        {
            // validate model
            var errors = new List<Exception>();

            if (model.ATMCardId == Guid.Empty)
                errors.Add(new Exception("ATMCardId is required!"));

            if (model.SonId == Guid.Empty)
                errors.Add(new Exception("SonId is required!"));

            if (model.VendorId == Guid.Empty)
                errors.Add(new Exception("VendorId is required!"));

            if (model.ReferenceTransactionId == Guid.Empty)
                errors.Add(new Exception("ReferenceTransactionId is required!"));

            if (string.IsNullOrEmpty(model.POSMachinePinCode))
                errors.Add(new Exception("POSMachine PinCode is required!"));

            var atmCard = await _atmCardRepository.GetAsync(model.ATMCardId);
            if (atmCard == null)
                errors.Add(new Exception("ATMCard Not found!"));

            var son = await _sonRepository.GetAsync(model.SonId);
            if (son == null)
                errors.Add(new Exception("Son Not found!"));

            if (son != null && son.CurrentATMCardId != model.ATMCardId)
                errors.Add(new Exception("Wrong current ATMCard for Son!"));

            var vendor = await _vendorRepository.GetAsync(model.VendorId);
            if (vendor == null)
                errors.Add(new Exception("Vendor Not found!"));

            var posMachine = await _posMachineRepository.GetAsync(model.POSMachineId);

            if (posMachine == null)
                errors.Add(new Exception("POSMachine Not found!"));

            if (posMachine != null && posMachine.VendorId != model.VendorId)
                errors.Add(new Exception("Wrong VendorId for POSMachine!"));

            if (posMachine.PinCode.ToLower() != model.POSMachinePinCode.ToLower())
                errors.Add(new Exception("Wrong POSMachine PinCode!"));

            var referenceTransaction = await _posMachineTransactionRepository.GetWhereIncludeAsync(t => t.POSMachineTransactionId == model.ReferenceTransactionId, t => t.RefundTransactions);
            if (referenceTransaction == null)
                errors.Add(new Exception("Wrong ReferenceTransactionId!"));

            if (!(model.Amount > 0))
                errors.Add(new Exception("Amount is required!"));

            // check if reference transaction allow to refund
            var referenceTransactionAmount = referenceTransaction.Amount + referenceTransaction.RefundTransactions.Sum(r => r.Amount);
            if (referenceTransactionAmount < model.Amount)
                errors.Add(new Exception("Reference Transaction Amount is less than amount to refund! Reference Transaction Amount is: " + referenceTransactionAmount));

            if (errors.Count > 0)
                throw new AggregateException(errors);

            var atmCardTransaction = new ATMCardTransaction
            {
                ATMCardId = model.ATMCardId,
                SonId = model.SonId,
                VendorId = model.VendorId,
                POSMachineId = model.POSMachineId,
                Amount = model.Amount,
                PaymentType = PaymentType.Transfer,
                TitleAr = TransactionTypesConstants.ATMCardTransactionTypes.RefundFromVendorToATMCard.TitleAr,
                TitleEn = TransactionTypesConstants.ATMCardTransactionTypes.RefundFromVendorToATMCard.TitleEn,
                DetailsAr = TransactionTypesConstants.ATMCardTransactionTypes.RefundFromVendorToATMCard.DetailsAr,
                DetailsEn = TransactionTypesConstants.ATMCardTransactionTypes.RefundFromVendorToATMCard.DetailsEn,
                IsSuccess = true,
                CreationDate = DateTime.UtcNow
            };
            _atmCardTransactionRepository.Add(atmCardTransaction);
            //son.CurrentBalance = atmCard.ATMCardTransactions.Where(t => t.IsSuccess).Sum(t => t.Amount);
            son.CurrentBalance += atmCardTransaction.Amount;

            var posTransaction = new POSMachineTransaction
            {
                POSMachineId = model.POSMachineId,
                VendorId = model.VendorId,
                ATMCardId = model.ATMCardId,
                SonId = model.SonId,
                ReferenceTransactionId = model.ReferenceTransactionId,
                Amount = -model.Amount,
                PaymentType = PaymentType.Transfer,
                TitleAr = TransactionTypesConstants.ATMCardTransactionTypes.RefundFromVendorToATMCard.TitleAr,
                TitleEn = TransactionTypesConstants.ATMCardTransactionTypes.RefundFromVendorToATMCard.TitleEn,
                DetailsAr = TransactionTypesConstants.ATMCardTransactionTypes.RefundFromVendorToATMCard.DetailsAr,
                DetailsEn = TransactionTypesConstants.ATMCardTransactionTypes.RefundFromVendorToATMCard.DetailsEn,
                IsSuccess = true,
                CreationDate = DateTime.UtcNow
            };
            _posMachineTransactionRepository.Add(posTransaction);
            //vendor.CurrentBalance = posMachine.POSMachineTransactions.Where(t => t.IsSuccess).Sum(t => t.Amount + t.TransactionCommissions.Sum(c => c.FixedCommissionValue + c.PercentageCommissionValue));
            vendor.CurrentBalance += posTransaction.Amount;

            if (await unitOfWork.CommitAsync() == 0)
            {
                atmCardTransaction.IsSuccess = posTransaction.IsSuccess = false;
                _atmCardTransactionRepository.Add(atmCardTransaction);
                _posMachineTransactionRepository.Add(posTransaction);
                await unitOfWork.CommitAsync();
                throw new BusinessException("Unable to add Transactions!");
            }
            else
            {
                await _pushNotificationService.CreatePushNotification(new PushNotificationInputViewModel
                {
                    NotificationType = PushNotificationType.RefundFromVendorToATMCard,
                    RecordId = son.ParentId,
                    Amount = model.Amount
                });
            }
        }

        public async Task<PagedOutput<ATMCardTransactionOutputViewModel>> GetAllAsync(ATMCardTransactionFilterViewModel model)
        {
            var result = new PagedOutput<ATMCardTransactionOutputViewModel>();

            var query = _atmCardTransactionRepository.Table;

            // filtering
            if (model.SonId.HasValue)
                query = query.Where(c => c.SonId == model.SonId);

            if (model.FromCreationDate.HasValue)
                query = query.Where(p => p.CreationDate.Date >= model.FromCreationDate.Value.Date);

            if (model.ToCreationDate.HasValue)
                query = query.Where(p => p.CreationDate.Date <= model.ToCreationDate.Value.Date);

            if (model.IsSuccess.HasValue)
                query = query.Where(p => p.IsSuccess == model.IsSuccess);

            // sorting
            var columnsMap = new Dictionary<string, Expression<Func<ATMCardTransaction, object>>>()
            {
                ["transactionNumber"] = v => v.TransactionNumber,
                ["amount"] = v => v.Amount,
                ["paymentType"] = v => v.PaymentType,
                ["title"] = v => _userIdentity.Language == Language.en ? v.TitleEn : v.TitleAr,
                ["details"] = v => _userIdentity.Language == Language.en ? v.DetailsEn : v.DetailsAr,
                ["creationDate"] = v => v.CreationDate,
                ["isSuccess"] = v => v.IsSuccess
            };
            query = query.ApplySorting(model, columnsMap);

            result.TotalCount = await query.CountAsync();

            // paging
            query = query.ApplyPaging(model);

            result.Result = mapper.Map<List<ATMCardTransactionOutputViewModel>>(query);

            return result;
        }
    }
}
