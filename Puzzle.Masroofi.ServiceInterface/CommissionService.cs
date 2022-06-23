using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.Commissions;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface ICommissionService
    {
        Task<PagedOutput<CommissionOutputViewModel>> GetAllAsync(CommissionFilterViewModel model);
        Task<CommissionOutputViewModel> GetAsync(Guid id);
        Task AddAsync(CommissionInputViewModel model);
        Task UpdateAsync(CommissionInputViewModel model);
        Task<OperationState> DeleteAsync(Guid id);
        Task<CommissionValueOutputViewModel> GetParentCommissionAsync(ParentCommissionInputViewModel model, ChargeParentWalletCommissionType commissionType);
        
    }

    public class CommissionService : BaseService, ICommissionService
    {
        private readonly ICommissionRepository _commissionRepository;
        private readonly IParentWalletTransactionRepository _parentWalletTransactionRepository;
        private readonly IParentRepository _parentRepository;
        private readonly UserIdentity _userIdentity;
        private readonly IConfiguration _configuration;
        public CommissionService(ICommissionRepository commissionRepository,
            IParentWalletTransactionRepository parentWalletTransactionRepository,
            IParentRepository parentRepository,
            UserIdentity userIdentity,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork, mapper)
        {
            _commissionRepository = commissionRepository;
            _parentWalletTransactionRepository = parentWalletTransactionRepository;
            _parentRepository = parentRepository;
            _userIdentity = userIdentity;
            _configuration = configuration;
        }

        public async Task<PagedOutput<CommissionOutputViewModel>> GetAllAsync(CommissionFilterViewModel model)
        {
            var result = new PagedOutput<CommissionOutputViewModel>();

            var query = _commissionRepository.Table;

            // filtering
            query = FilterCommissions(query, model);

            // sorting
            var columnsMap = new Dictionary<string, Expression<Func<CommissionSetting, object>>>()
            {
                ["fromValue"] = v => v.FromValue,
                ["toValue"] = v => v.ToValue,
                ["fixedCommission"] = v => v.FixedCommission,
                ["percentageCommission"] = v => v.PercentageCommission,
                ["vendorFixedCommission"] = v => v.VendorFixedCommission,
                ["vendorPercentageCommission"] = v => v.VendorPercentageCommission,
                ["creationDate"] = v => v.CreationDate
            };
            query = query.ApplySorting(model, columnsMap);

            result.TotalCount = await query.CountAsync();

            // paging
            query = query.ApplyPaging(model);

            result.Result = mapper.Map<List<CommissionOutputViewModel>>(query);

            return result;
        }

        public async Task<CommissionOutputViewModel> GetAsync(Guid id)
        {
            var entityToGet = await _commissionRepository.GetAsync(id);

            if (entityToGet == null)
            {
                throw new BusinessException("Commission Not found!");
            }
            return mapper.Map<CommissionOutputViewModel>(entityToGet);
        }

        public async Task AddAsync(CommissionInputViewModel model)
        {
            model.CommissionSettingId = Guid.NewGuid();

            ValidateCommission(model);

            var entityToAdd = mapper.Map<CommissionSetting>(model);

            entityToAdd.IsActive = true;
            entityToAdd.IsDeleted = false;
            entityToAdd.CreationDate = DateTime.UtcNow;
            entityToAdd.CreationUser = _userIdentity.Id.Value;
            _commissionRepository.Add(entityToAdd);

            if (await unitOfWork.CommitAsync() > 0)
            {
                mapper.Map(entityToAdd, model);
            }
            else
                throw new BusinessException("Unable to add Commission Setting!");
        }

        public async Task UpdateAsync(CommissionInputViewModel model)
        {
            ValidateCommission(model);

            var entityToUpdate = await _commissionRepository.GetAsync(model.CommissionSettingId.Value);

            if (entityToUpdate == null)
            {
                throw new BusinessException("Commission Not found!");
            }

            mapper.Map(model, entityToUpdate);

            entityToUpdate.ModificationDate = DateTime.UtcNow;
            entityToUpdate.ModificationUser = _userIdentity.Id.Value;

            if (await unitOfWork.CommitAsync() > 0)
            {
                mapper.Map(entityToUpdate, model);
            }
            else
                throw new BusinessException("Unable to update Commission Setting!");
        }

        public async Task<OperationState> DeleteAsync(Guid id)
        {
            var entityToDelete = await _commissionRepository.GetAsync(id);
            if (entityToDelete == null)
            {
                throw new BusinessException("Commission Not found!");
            }
            entityToDelete.IsDeleted = true;
            entityToDelete.IsActive = false;
            entityToDelete.ModificationDate = DateTime.UtcNow;
            entityToDelete.ModificationUser = _userIdentity.Id.Value;
            return await unitOfWork.CommitAsync() > 0 ? OperationState.Deleted : OperationState.None;
        }

        public async Task<CommissionValueOutputViewModel> GetParentCommissionAsync(ParentCommissionInputViewModel model, ChargeParentWalletCommissionType commissionType)
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

            if (!(model.Amount > 0))
                errors.Add(new Exception("Amount is required!"));

            if (errors.Count > 0)
                throw new AggregateException(errors);

            int chargeTimesWithoutCommission = int.Parse(_configuration.GetSection("ChargeTimesWithoutCommission").Value);
            int parentWalletTransactionsCount = await _parentWalletTransactionRepository.GetAllWhereCountAsync(p => p.ParentId == parent.ParentId && p.PaymentType == PaymentType.Visa && p.IsSuccess);

            // check if deserve Charge Without Commission
            if (parentWalletTransactionsCount < chargeTimesWithoutCommission)
            {
                return new CommissionValueOutputViewModel
                {
                    Amount = model.Amount,
                    FixedCommissionValue = 0,
                    PercentageCommissionValue = 0,
                    TotalCommissionValues = 0,
                    AmountPlusCommissions = model.Amount
                };
            }
            else
            {
                return await GetByAmountAsync(model.Amount, commissionType);
            }
        }

        private async Task<CommissionValueOutputViewModel> GetByAmountAsync(decimal amount, ChargeParentWalletCommissionType commissionType)
        {
            var errors = new List<Exception>();

            if (!(amount > 0))
                errors.Add(new Exception("Amount is required!"));

            var commissionSetting = await _commissionRepository.GetWhereAsync(c => c.FromValue <= amount && c.ToValue >= amount && c.CommissionType == commissionType);
            if (commissionSetting == null)
                errors.Add(new Exception("Not found Commission for this value in Commission Setting! Please contact with Admin to add it."));

            if (errors.Count > 0)
                throw new AggregateException(errors);

            int decimalPlacesNumber = int.Parse(_configuration.GetSection("decimalPlacesNumber").Value);
            decimal percentageCommissionValue = Math.Round(amount * commissionSetting.PercentageCommission / 100, decimalPlacesNumber);

            return new CommissionValueOutputViewModel
            {
                CommissionSettingId = commissionSetting.CommissionSettingId,
                Amount = amount,
                FixedCommissionValue = commissionSetting.FixedCommission,
                PercentageCommissionValue = percentageCommissionValue,
                TotalCommissionValues = commissionSetting.FixedCommission + percentageCommissionValue,
                AmountPlusCommissions = amount + commissionSetting.FixedCommission + percentageCommissionValue,
            };
        }
      
        private IQueryable<CommissionSetting> FilterCommissions(IQueryable<CommissionSetting> query, CommissionFilterViewModel model)
        {
            if (model.CommissionType.HasValue)
            {
                query = query.Where(c => c.CommissionType == model.CommissionType);
            }
            if (model.FixedCommission.HasValue)
            {
                query = query.Where(c => c.FixedCommission == model.FixedCommission);
            }
            if (model.PercentageCommission.HasValue)
            {
                query = query.Where(c => c.PercentageCommission == model.PercentageCommission);
            }
            if (model.VendorFixedCommission.HasValue)
            {
                query = query.Where(c => c.VendorFixedCommission == model.VendorFixedCommission);
            }
            if (model.VendorPercentageCommission.HasValue)
            {
                query = query.Where(c => c.VendorPercentageCommission == model.VendorPercentageCommission);
            }

            return query;
        }

        private void ValidateCommission(CommissionInputViewModel model)
        {
            var errors = new List<Exception>();

            if (model.CommissionSettingId == null || model.CommissionSettingId == Guid.Empty)
                errors.Add(new Exception("CommissionSettingId is required!"));

            if (!(model.FromValue > 0))
                errors.Add(new Exception("FromValue is required!"));

            if (!(model.ToValue > 0))
                errors.Add(new Exception("ToValue is required!"));

            if (!model.CommissionType.IsEnumValid())
                errors.Add(new Exception("CommissionType is required!"));

            if (!(model.PercentageCommission >= 0))
                errors.Add(new Exception("PercentageCommission is required!"));

            if (!(model.FixedCommission >= 0))
                errors.Add(new Exception("FixedCommission is required!"));

            if (!(model.VendorPercentageCommission >= 0))
                errors.Add(new Exception("VendorPercentageCommission is required!"));

            if (!(model.VendorFixedCommission >= 0))
                errors.Add(new Exception("VendorFixedCommission is required!"));

            if (model.FixedCommission < model.VendorFixedCommission)
                errors.Add(new Exception("FixedCommission must be greater than VendorFixedCommission!"));

            if (model.PercentageCommission < model.VendorPercentageCommission)
                errors.Add(new Exception("PercentageCommission must be greater than VendorPercentageCommission!"));

            if (errors.Count > 0)
                throw new AggregateException(errors);
        }
    }
}
