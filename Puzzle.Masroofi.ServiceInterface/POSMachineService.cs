using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.POSMachines;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Puzzle.Masroofi.Core.Resources;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface IPOSMachineService
    {
        Task<PagedOutput<POSMachineOutputViewModel>> GetAllAsync(POSMachineFilterViewModel model);
        Task<POSMachineOutputViewModel> GetAsync(Guid id);
        Task AddAsync(POSMachineInputViewModel model);
        Task UpdateAsync(POSMachineInputViewModel model);
        Task<OperationState> DeleteAsync(Guid id);
        Task<POSMachineOutputViewModel> LoginAsync(POSMachineLoginViewModel model);
        Task<string> ForgetPinCodeAsync(string username);
        Task<OperationState> UpdatePinCodeAsync(POSMachinePinCodeViewModel model);
        Task<decimal> GetBalanceAsync(Guid id);
    }
    public class POSMachineService : BaseService, IPOSMachineService
    {
        private readonly IPOSMachineRepository _posMachineRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IPOSMachineLoginHistoryRepository _posMachineLoginHistoryRepository;
        private readonly UserIdentity _userIdentity;
        private readonly IConfiguration _configuration;
        private readonly ISmsService _smsService;
        private readonly string encryptionKey;

        public POSMachineService(IPOSMachineRepository posMachineRepository,
            IVendorRepository vendorRepository,
            IPOSMachineLoginHistoryRepository posMachineLoginHistoryRepository,
            UserIdentity userIdentity,
            IConfiguration configuration,
            ISmsService smsService,
            IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork, mapper)
        {
            _posMachineRepository = posMachineRepository;
            _vendorRepository = vendorRepository;
            _posMachineLoginHistoryRepository = posMachineLoginHistoryRepository;
            _userIdentity = userIdentity;
            _configuration = configuration;
            _smsService = smsService;
            encryptionKey = _configuration.GetSection("Security:EncryptionKey").Value;
        }

        public async Task<PagedOutput<POSMachineOutputViewModel>> GetAllAsync(POSMachineFilterViewModel model)
        {
            var result = new PagedOutput<POSMachineOutputViewModel>();

            var query = _posMachineRepository.Table
                .Include(v => v.Vendor)
                .AsQueryable();

            // filtering
            query = FilterPOSMachines(query, model);

            // sorting
            var columnsMap = new Dictionary<string, Expression<Func<POSMachine, object>>>()
            {
                ["username"] = v => v.Username,
                ["posModel"] = v => v.POSModel,
                ["posNumber"] = v => v.POSNumber,
                ["creationDate"] = v => v.CreationDate,
            };
            query = query.ApplySorting(model, columnsMap);

            result.TotalCount = await query.CountAsync();

            // paging
            query = query.ApplyPaging(model);

            result.Result = mapper.Map<List<POSMachineOutputViewModel>>(query);

            return result;
        }

        public async Task<POSMachineOutputViewModel> GetAsync(Guid id)
        {
            var entityToGet = await _posMachineRepository.GetWhereIncludeAsync(p => p.POSMachineId == id, v => v.Vendor);
            entityToGet.PinCode = Encryption.DecryptData(entityToGet.PinCode.Replace(" ", ""), encryptionKey);
            if (entityToGet == null)
            {
                throw new BusinessException("POSMachine Not found!");
            }
            return mapper.Map<POSMachineOutputViewModel>(entityToGet);
        }

        public async Task AddAsync(POSMachineInputViewModel model)
        {
            model.POSMachineId = Guid.NewGuid();

            ValidatePOSMachine(model);

            //if (_posMachineRepository.GetAny(p => p.VendorId == model.VendorId && p.IsActive))
            //    throw new BusinessException("Vendor already have another active POSMachine!");

            if (_posMachineRepository.GetAny(p => p.Username.ToLower().Trim() == model.Username.ToLower().Trim()))
                throw new BusinessException("Username already taken , Please choose another one!");

            var entityToAdd = mapper.Map<POSMachine>(model);

            entityToAdd.PinCode = Encryption.EncryptData(model.PinCode.Replace(" ", ""), encryptionKey);
            entityToAdd.CreationDate = DateTime.UtcNow;
            entityToAdd.CreationUser = _userIdentity.Id.Value;
            entityToAdd.IsActive = true;
            entityToAdd.IsDeleted = false;

            _posMachineRepository.Add(entityToAdd);

            if (await unitOfWork.CommitAsync() > 0)
            {
                mapper.Map(entityToAdd, model);
            }
            else
                throw new BusinessException("Unable to add POSMachine!");
        }

        public async Task UpdateAsync(POSMachineInputViewModel model)
        {
            ValidatePOSMachine(model);

            var entityToUpdate = await _posMachineRepository.GetAsync(model.POSMachineId.Value);

            if (entityToUpdate == null)
            {
                throw new BusinessException("POSMachine Not found!");
            }

            mapper.Map(model, entityToUpdate);
            entityToUpdate.PinCode = Encryption.EncryptData(model.PinCode.Replace(" ", ""), encryptionKey);
            entityToUpdate.ModificationDate = DateTime.UtcNow;
            entityToUpdate.ModificationUser = _userIdentity.Id.Value;

            if (await unitOfWork.CommitAsync() > 0)
            {
                mapper.Map(entityToUpdate, model);
            }
            else
                throw new BusinessException("Unable to update POSMachine!");
        }

        public async Task<OperationState> DeleteAsync(Guid id)
        {
            var entityToDelete = await _posMachineRepository.GetAsync(id);
            if (entityToDelete == null)
            {
                throw new BusinessException("POSMachine not found");
            }
            entityToDelete.IsDeleted = true;
            entityToDelete.IsActive = false;
            entityToDelete.ModificationDate = DateTime.UtcNow;
            entityToDelete.ModificationUser = _userIdentity.Id.Value;
            return await unitOfWork.CommitAsync() > 0 ? OperationState.Deleted : OperationState.None;
        }

        public async Task<POSMachineOutputViewModel> LoginAsync(POSMachineLoginViewModel model)
        {
            var errors = new List<Exception>();

            if (string.IsNullOrEmpty(model.Username))
                errors.Add(new Exception("Username is required!"));

            if (string.IsNullOrEmpty(model.PinCode))
            {
                errors.Add(new Exception("PinCode is required!"));
            }
            if (errors.Count > 0)
                throw new AggregateException(errors);

            var entityToGet = await _posMachineRepository.GetWhereIncludeAsync(p => p.Username == model.Username && p.PinCode.ToLower() == model.PinCode.ToLower(), v => v.Vendor);
            if (entityToGet == null)
            {
                throw new BusinessException("Wrong Username or PinCode!");
            }
            if (!entityToGet.IsActive)
            {
                throw new BusinessException("Your account is unactive!");
            }

            _posMachineLoginHistoryRepository.Add(new POSMachineLoginHistory
            {
                POSMachineId = entityToGet.POSMachineId,
                LoginDate = DateTime.UtcNow
            });
            await unitOfWork.CommitAsync();

            var result = mapper.Map<POSMachineOutputViewModel>(entityToGet);

            return result;
        }

        public async Task<string> ForgetPinCodeAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new BusinessException("POSMachine username is required");

            var entityToUpdate = await _posMachineRepository.GetWhereIncludeAsync(p => p.Username == username, v => v.Vendor);

            if (entityToUpdate == null)
            {
                throw new BusinessException("POSMachine not found");
            }

            entityToUpdate.POSMachinePinCodeHistories.Add(new POSMachinePinCodeHistory
            {
                PinCode = Encryption.DecryptData(entityToUpdate.PinCode, encryptionKey),
                HistoryType = PinCodeHistoryType.ForgetPinCode,
                UpdatedBy = _userIdentity.Id.Value,
                UpdatedOn = DateTime.UtcNow
            });
            string newPinCode = new Random().Next(0, 1000000).ToString("D6");
            entityToUpdate.PinCode = Encryption.EncryptData(newPinCode, encryptionKey);
            entityToUpdate.ModificationDate = DateTime.UtcNow;
            entityToUpdate.ModificationUser = _userIdentity.Id;

            if (await unitOfWork.CommitAsync() > 0)
            {
                string smsMessage = (_userIdentity.Language == Language.en ? "New PinCode for your Masroofi account: " : "الكود الجديد لحسابكم في تطبيق مصروفي هو: ") + newPinCode;
                var sendSmsResult = await _smsService.SendAsync(entityToUpdate.Vendor.Phone1, smsMessage);

                if (sendSmsResult.Code == "1901")
                    return "The new PinCode has been generated and sent it to Vendor phone successfully!";
                else
                    throw new BusinessException("Unable to send SMS with new PinCode to Vendor Phone!");
            }
            else
            {
                throw new BusinessException("Unable to change PinCode!");
            }
        }

        public async Task<OperationState> UpdatePinCodeAsync(POSMachinePinCodeViewModel model)
        {
            var errors = new List<Exception>();

            if (model.POSMachineId == Guid.Empty)
                errors.Add(new Exception("POSMachineId is required!"));

            if (string.IsNullOrEmpty(model.OldPinCode))
                errors.Add(new Exception("OldPinCode is required!"));

            if (string.IsNullOrEmpty(model.PinCode))
                errors.Add(new Exception("PinCode is required!"));
            else
            {
                var decryptedPinCode = Encryption.DecryptData(model.PinCode.Replace(" ", ""), encryptionKey);
                if (!decryptedPinCode.IsValidFourDigits())
                    errors.Add(new Exception("PinCode must be 4 digits only!"));
            }
            if (errors.Count > 0)
                throw new AggregateException(errors);

            var entityToUpdate = await _posMachineRepository.GetAsync(model.POSMachineId);

            if (entityToUpdate == null)
                throw new BusinessException("POSMachine not found!");

            if (model.OldPinCode.ToLower() != entityToUpdate.PinCode.ToLower())
                throw new BusinessException("Wrong OldPinCode!");

            entityToUpdate.POSMachinePinCodeHistories.Add(new POSMachinePinCodeHistory
            {
                PinCode = Encryption.DecryptData(entityToUpdate.PinCode, encryptionKey),
                HistoryType = PinCodeHistoryType.ChangePinCode,
                UpdatedBy = _userIdentity.Id.Value,
                UpdatedOn = DateTime.UtcNow
            });
            entityToUpdate.PinCode = model.PinCode;
            entityToUpdate.ModificationDate = DateTime.UtcNow;
            entityToUpdate.ModificationUser = _userIdentity.Id;

            if (await unitOfWork.CommitAsync() > 0)
            {
                string smsMessage = (_userIdentity.Language == Language.en ? "New Password for your Masroofi account: " : "الكود الجديد لحسابكم في تطبيق مصروفي هو: ") + Encryption.DecryptData(entityToUpdate.PinCode, encryptionKey);
                var sendSmsResult = await _smsService.SendAsync(entityToUpdate.Vendor.Phone1, smsMessage);

                if (sendSmsResult.Code == "1901")
                    return OperationState.Updated;
                else
                    throw new BusinessException("Unable to send SMS with new PinCode to Vendor Phone!");
            }
            else
            {
                throw new BusinessException("Unable to change PinCode!");
            }
        }

        public async Task<decimal> GetBalanceAsync(Guid id)
        {
            var posMachine = await _posMachineRepository.GetWhereIncludeAsync(a => a.POSMachineId == id, a => a.POSMachineTransactions);
            if (posMachine == null)
            {
                throw new BusinessException("POSMachine Not found!");
            }
            return posMachine.POSMachineTransactions.Sum(t => t.Amount);
        }

        private IQueryable<POSMachine> FilterPOSMachines(IQueryable<POSMachine> query, POSMachineFilterViewModel model)
        {
            if (model.VendorId.HasValue)
            {
                query = query.Where(c => c.VendorId == model.VendorId);
            }
            if (!string.IsNullOrEmpty(model.Username))
            {
                query = query.Where(c => c.Username.Contains(model.Username));
            }
            if (!string.IsNullOrEmpty(model.POSModel))
            {
                query = query.Where(c => c.POSModel.Contains(model.POSModel));
            }
            if (!string.IsNullOrEmpty(model.POSNumber))
            {
                query = query.Where(c => c.POSNumber.Contains(model.POSNumber));
            }

            return query;
        }

        private void ValidatePOSMachine(POSMachineInputViewModel model)
        {
            var errors = new List<Exception>();

            if (model.POSMachineId == null || model.POSMachineId == Guid.Empty)
                errors.Add(new Exception("POSMachineId is required!"));

            if (!_vendorRepository.GetAny(p => p.VendorId == model.VendorId))
                errors.Add(new Exception("Vendor Not found!"));

            if (string.IsNullOrEmpty(model.Username))
                errors.Add(new Exception("Username is required!"));

            if (string.IsNullOrEmpty(model.PinCode))
            {
                errors.Add(new Exception("PinCode is required!"));
            }
            else
            {
                if (!model.PinCode.IsValidFourDigits())
                    throw new BusinessException("PinCode must be 4 digits only!");

               


                //var decryptedPinCode = Encryption.DecryptData(model.PinCode.Replace(" ", ""), encryptionKey);
                //if (!decryptedPinCode.IsValidFourDigits())
                //    errors.Add(new Exception("PinCode must be 4 digits only!"));
            }

            if (errors.Count > 0)
                throw new AggregateException(errors);
        }
    }
}
