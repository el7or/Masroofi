using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.Constants;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.POSMachineTransactions;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface IPOSMachineTransactionService
    {
        Task ChargeByAdminAsync(ChargePOSByAdminViewModel model);
        Task PayDuesByAdminAsync(PayDuesByAdminViewModel model);
        Task<PagedOutput<POSMachineTransactionOutputViewModel>> GetAllAsync(POSMachineTransactionFilterViewModel model);
    }
    public class POSMachineTransactionService : BaseService, IPOSMachineTransactionService
    {
        private readonly IPOSMachineTransactionRepository _posMachineTransactionRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IPOSMachineRepository _posMachineRepository;
        private readonly UserIdentity _userIdentity;
        public POSMachineTransactionService(IPOSMachineTransactionRepository posMachineTransactionRepository,
            IVendorRepository vendorRepository,
            IPOSMachineRepository posMachineRepository,
            UserIdentity userIdentity,
            IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork, mapper)
        {
            _posMachineTransactionRepository = posMachineTransactionRepository;
            _vendorRepository = vendorRepository;
            _posMachineRepository = posMachineRepository;
            _userIdentity = userIdentity;
        }

        public async Task ChargeByAdminAsync(ChargePOSByAdminViewModel model)
        {
            // validate model
            var errors = new List<Exception>();

            if (model.VendorId == Guid.Empty)
                errors.Add(new Exception("VendorId is required!"));

            if (model.POSMachineId == Guid.Empty)
                errors.Add(new Exception("POSMachineId is required!"));

            var posMachine = await _posMachineRepository.GetAsync(model.POSMachineId);
            if (posMachine == null)
                errors.Add(new Exception("POSMachine Not found!"));

            var vendor = await _vendorRepository.GetAsync(model.VendorId);
            if (vendor == null)
                errors.Add(new Exception("Vendor Not found!"));

            if (posMachine != null && posMachine.VendorId != model.VendorId)
                errors.Add(new Exception("Wrong VendorId for POSMachine!"));

            if (!(model.Amount > 0))
                errors.Add(new Exception("Amount is required!"));

            if (errors.Count > 0)
                throw new AggregateException(errors);

            var transaction = new POSMachineTransaction
            {
                POSMachineId = model.POSMachineId,
                VendorId = model.VendorId,
                AdminUserId = _userIdentity.Id,
                Amount = model.Amount,
                PaymentType = PaymentType.Cash,
                TitleAr = TransactionTypesConstants.POSMachineTransactionTypes.ChargePOSMachineByAdmin.TitleAr,
                TitleEn = TransactionTypesConstants.POSMachineTransactionTypes.ChargePOSMachineByAdmin.TitleEn,
                DetailsAr = TransactionTypesConstants.POSMachineTransactionTypes.ChargePOSMachineByAdmin.DetailsAr,
                DetailsEn = TransactionTypesConstants.POSMachineTransactionTypes.ChargePOSMachineByAdmin.DetailsEn,
                IsSuccess = true,
                CreationDate = DateTime.UtcNow
            };
            _posMachineTransactionRepository.Add(transaction);
            //vendor.CurrentBalance = posMachine.POSMachineTransactions.Where(t => t.IsSuccess).Sum(t => t.Amount + t.TransactionCommissions.Sum(c => c.FixedCommissionValue + c.PercentageCommissionValue));
            vendor.CurrentBalance += transaction.Amount;

            if (await unitOfWork.CommitAsync() == 0)
            {
                transaction.IsSuccess = false;
                _posMachineTransactionRepository.Add(transaction);
                await unitOfWork.CommitAsync();
                throw new BusinessException("Unable to add Transaction!");
            }
        }

        public async Task PayDuesByAdminAsync(PayDuesByAdminViewModel model)
        {
            // validate model
            var errors = new List<Exception>();

            if (model.VendorId == Guid.Empty)
                errors.Add(new Exception("VendorId is required!"));

            if (model.POSMachineId == Guid.Empty)
                errors.Add(new Exception("POSMachineId is required!"));

            var posMachine = await _posMachineRepository.GetAsync(model.POSMachineId);
            if (posMachine == null)
                errors.Add(new Exception("POSMachine Not found!"));

            var vendor = await _vendorRepository.GetAsync(model.VendorId);
            if (vendor == null)
                errors.Add(new Exception("Vendor Not found!"));

            if (posMachine != null && posMachine.VendorId != model.VendorId)
                errors.Add(new Exception("Wrong VendorId for POSMachine!"));

            if (!(model.Amount > 0))
                errors.Add(new Exception("Amount is required!"));

            if (string.IsNullOrEmpty(model.Note))
                errors.Add(new Exception("Note is required!"));

            // check if vendor balance allow to pay
            if (vendor != null && vendor.CurrentBalance < model.Amount)
                errors.Add(new Exception("Not enough balance in POSMachine for pay! Current Balance is: " + vendor.CurrentBalance));

            if (errors.Count > 0)
                throw new AggregateException(errors);

            var transaction = new POSMachineTransaction
            {
                POSMachineId = model.POSMachineId,
                VendorId = model.VendorId,
                AdminUserId = _userIdentity.Id,
                Amount = -model.Amount,
                Note = model.Note,
                PaymentType = PaymentType.Cash,
                TitleAr = TransactionTypesConstants.POSMachineTransactionTypes.PayDuesFromPOSMachineByAdmin.TitleAr,
                TitleEn = TransactionTypesConstants.POSMachineTransactionTypes.PayDuesFromPOSMachineByAdmin.TitleEn,
                DetailsAr = TransactionTypesConstants.POSMachineTransactionTypes.PayDuesFromPOSMachineByAdmin.DetailsAr,
                DetailsEn = TransactionTypesConstants.POSMachineTransactionTypes.PayDuesFromPOSMachineByAdmin.DetailsEn,
                IsSuccess = true,
                CreationDate = DateTime.UtcNow
            };
            _posMachineTransactionRepository.Add(transaction);
            //vendor.CurrentBalance = posMachine.POSMachineTransactions.Where(t => t.IsSuccess).Sum(t => t.Amount + t.TransactionCommissions.Sum(c => c.FixedCommissionValue + c.PercentageCommissionValue));
            vendor.CurrentBalance += transaction.Amount;

            if (await unitOfWork.CommitAsync() == 0)
            {
                transaction.IsSuccess = false;
                _posMachineTransactionRepository.Add(transaction);
                await unitOfWork.CommitAsync();
                throw new BusinessException("Unable to add Transaction!");
            }
        }

        public async Task<PagedOutput<POSMachineTransactionOutputViewModel>> GetAllAsync(POSMachineTransactionFilterViewModel model)
        {
            var result = new PagedOutput<POSMachineTransactionOutputViewModel>();

            var query = _posMachineTransactionRepository.Table
                .Include(s => s.Son)
                .Include(r => r.RefundTransactions)
                .AsQueryable();

            // filtering
            if (model.POSMachineId.HasValue)
                query = query.Where(c => c.POSMachineId == model.POSMachineId);

            if (model.VendorId.HasValue)
                query = query.Where(c => c.VendorId == model.VendorId);

            if (model.ATMCardId.HasValue)
                query = query.Where(c => c.ATMCardId == model.ATMCardId);

            if (model.FromCreationDate.HasValue)
                query = query.Where(p => p.CreationDate.Date >= model.FromCreationDate.Value.Date);

            if (model.ToCreationDate.HasValue)
                query = query.Where(p => p.CreationDate.Date <= model.ToCreationDate.Value.Date);

            if (model.IsSuccess.HasValue)
                query = query.Where(p => p.IsSuccess == model.IsSuccess);

            if (!string.IsNullOrEmpty(model.SonName))
            {
                query = query.Where(c => c.Son.SonNameAr.Contains(model.SonName) || c.Son.SonNameEn.Contains(model.SonName));
            }

            // sorting
            var columnsMap = new Dictionary<string, Expression<Func<POSMachineTransaction, object>>>()
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

            result.Result = mapper.Map<List<POSMachineTransactionOutputViewModel>>(query);

            return result;
        }
    }
}
