using AutoMapper;
using Microsoft.Extensions.Configuration;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.Parents;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Puzzle.Masroofi.Core.ViewModels.PushNotifications;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface IParentService
    {
        Task<ParentOutputViewModel> RegisterAsync(ParentInputViewModel model);
        Task<ParentOutputViewModel> LoginAsync(ParentLoginViewModel model);
        Task UpdateAsync(ParentInputViewModel model);

        Task<ParentOutputViewModel> GetAsync(Guid id);
        Task<OperationState> DeleteAsync(Guid id);
        Task<OperationState> ChangeActivationAsync(ParentActivationViewModel model);
        bool CheckPinCodeExists(CheckPinCodeViewModel model);
        Task<string> ForgetPinCode(Guid id);
        Task<OperationState> UpdatePinCode(UpdatePinCodeViewModel model);
        Task<OperationState> UpdatePhone(UpdatePhoneViewModel model);
        Task<decimal> GetBalanceAsync(Guid id);
    }
    public class ParentService : BaseService, IParentService
    {
        private readonly IParentRepository _parentRepository;
        private readonly IParentLoginHistoryRepository _parentLoginHistoryRepository;
        private readonly UserIdentity _userIdentity;
        private readonly IConfiguration _configuration;
        private readonly IS3Service _s3Service;
        private readonly ISmsService _smsService;
        private readonly string encryptionKey;
        private readonly IPushNotificationService _pushNotificationService;

        public ParentService(IParentRepository parentRepository,
            IParentLoginHistoryRepository parentLoginHistoryRepository,
            IS3Service s3Service,
            ISmsService smsService,
            UserIdentity userIdentity,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IMapper mapper, IPushNotificationService pushNotificationService) : base(unitOfWork, mapper)
        {
            _parentRepository = parentRepository;
            _parentLoginHistoryRepository = parentLoginHistoryRepository;
            _userIdentity = userIdentity;
            _configuration = configuration;
            _s3Service = s3Service;
            _smsService = smsService;
            encryptionKey = _configuration.GetSection("Security:EncryptionKey").Value;
            _pushNotificationService = pushNotificationService;
        }
        public async Task<ParentOutputViewModel> GetAsync(Guid id)
        {
            var entityToGet = await _parentRepository.GetAsync(id);

            if (entityToGet == null)
            {
                throw new BusinessException("Parent Not found!");
            }
            return mapper.Map<ParentOutputViewModel>(entityToGet);
        }
        public async Task<ParentOutputViewModel> RegisterAsync(ParentInputViewModel model)
        {
            model.ParentId = Guid.NewGuid();

            model = ValidateParent(model);

            var entityToAdd = mapper.Map<ParentInputViewModel, Parent>(model);

            if (model.NewImage != null)
            {
                var fileBytes = Convert.FromBase64String(model.NewImage.FileBase64);
                var imagePath = _s3Service.UploadFile("Parents", model.NewImage.FileName, fileBytes);
                entityToAdd.ImageUrl = imagePath;
            }

            entityToAdd.CreationDate = DateTime.UtcNow;
            entityToAdd.CreationUser = model.ParentId.Value;
            entityToAdd.IsActive = true;
            entityToAdd.IsDeleted = false;

            _parentRepository.Add(entityToAdd);

            if (await unitOfWork.CommitAsync() > 0)
            {
                await _pushNotificationService.CreatePushNotification(new PushNotificationInputViewModel
                {
                    NotificationType = PushNotificationType.ParentRegistered,
                    RecordId = entityToAdd.ParentId
                });
                var result = mapper.Map<ParentOutputViewModel>(GetByPhone(entityToAdd.Phone));
                return result;
            }
            else
            {
                throw new BusinessException("Unable to add the Parent!");
            }
        }

        public async Task<ParentOutputViewModel> LoginAsync(ParentLoginViewModel model)
        {
            var decryptedPhone = Encryption.DecryptData(model.Phone.Replace(" ", ""), encryptionKey);
            if (decryptedPhone is null)
            {
                throw new BusinessException("Phone is required!");
            }
            var entityToGet = GetByPhone(decryptedPhone);
            if (entityToGet == null)
            {
                throw new BusinessException("Phone not found!");
            }
            if (!entityToGet.IsActive)
            {
                throw new BusinessException("Your account is unactive!");
            }
            _parentLoginHistoryRepository.Add(new ParentLoginHistory
            {
                ParentId = entityToGet.ParentId,
                LoginDate = DateTime.UtcNow
            });
            await unitOfWork.CommitAsync();

            var result = mapper.Map<ParentOutputViewModel>(entityToGet);

            return result;
        }

        public async Task UpdateAsync(ParentInputViewModel model)
        {
            var entityToUpdate = await _parentRepository.GetAsync(model.ParentId.Value);

            if (entityToUpdate == null)
                throw new BusinessException("Parent not found!");

            model = ValidateParent(model);

            mapper.Map(model, entityToUpdate);
            if (model.NewImage != null)
            {
                var fileBytes = Convert.FromBase64String(model.NewImage.FileBase64);
                entityToUpdate.ImageUrl = _s3Service.UploadFile("Parents", model.NewImage.FileName, fileBytes);
            }
            entityToUpdate.ModificationDate = DateTime.UtcNow;
            entityToUpdate.ModificationUser = _userIdentity.Id;

            if (await unitOfWork.CommitAsync() > 0)
            {
                mapper.Map(entityToUpdate, model);
            }
            else
                throw new BusinessException("Unable to update Parent!");
        }

        public async Task<OperationState> DeleteAsync(Guid id)
        {
            var entityToDelete = await _parentRepository.GetAsync(id);
            if (entityToDelete == null)
            {
                throw new BusinessException("Parent not found");
            }
            entityToDelete.IsDeleted = true;
            entityToDelete.IsActive = false;
            entityToDelete.ModificationDate = DateTime.UtcNow;
            entityToDelete.ModificationUser = _userIdentity.Id.Value;
            return await unitOfWork.CommitAsync() > 0 ? OperationState.Deleted : OperationState.None;
        }

        public async Task<OperationState> ChangeActivationAsync(ParentActivationViewModel model)
        {
            var entityToUpdate = await _parentRepository.GetAsync(model.ParentId);

            if (entityToUpdate == null)
            {
                throw new BusinessException("Parent Not found!");
            }

            entityToUpdate.IsActive = model.IsActive;
            entityToUpdate.ModificationDate = DateTime.UtcNow;
            entityToUpdate.ModificationUser = _userIdentity.Id.Value;

            return await unitOfWork.CommitAsync() > 0 ? OperationState.Updated : OperationState.None;
        }

        public bool CheckPinCodeExists(CheckPinCodeViewModel model)
        {
            if (model.ParentId == Guid.Empty)
                throw new BusinessException("ParentId is required");

            if (string.IsNullOrEmpty(model.PinCode))
                throw new BusinessException("Password is required");

            return _parentRepository.GetAny(p => p.ParentId == model.ParentId && p.PinCode.ToLower() == model.PinCode.ToLower());
        }

        public async Task<string> ForgetPinCode(Guid id)
        {
            if (id == Guid.Empty)
                throw new BusinessException("ParentId is required");

            var entityToUpdate = await _parentRepository.GetAsync(id);

            if (entityToUpdate == null)
            {
                throw new BusinessException("Parent not found");
            }

            entityToUpdate.ParentPinCodeHistories.Add(new ParentPinCodeHistory
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
                var sendSmsResult = await _smsService.SendAsync(entityToUpdate.Phone, smsMessage);

                if (sendSmsResult.Code == "1901")
                    return "The new PinCode has been generated and sent it to Parent phone successfully!";
                else
                    throw new BusinessException("Unable to send SMS with new PinCode to Parent Phone!");
            }
            else
            {
                throw new BusinessException("Unable to change PinCode!");
            }
        }

        public async Task<OperationState> UpdatePinCode(UpdatePinCodeViewModel model)
        {
            var errors = new List<Exception>();

            if (model.ParentId == Guid.Empty)
                errors.Add(new Exception("ParentId is required!"));

            if (string.IsNullOrEmpty(model.OldPinCode))
                errors.Add(new Exception("OldPinCode is required!"));

            if (string.IsNullOrEmpty(model.PinCode))
                errors.Add(new Exception("PinCode is required!"));
            else
            {
                var pinCode = Encryption.DecryptData(model.PinCode.Replace(" ", ""), encryptionKey);
                if (!pinCode.IsValidSixDigits())
                    errors.Add(new Exception("PinCode must be 6 digits only!"));
            }

            if (errors.Count > 0)
                throw new AggregateException(errors);

            var entityToUpdate = await _parentRepository.GetAsync(model.ParentId);

            if (entityToUpdate == null)
                throw new BusinessException("Parent not found!");

            if (model.OldPinCode.ToLower() != entityToUpdate.PinCode.ToLower())
                throw new BusinessException("Wrong OldPinCode!");

            entityToUpdate.ParentPinCodeHistories.Add(new ParentPinCodeHistory
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
                string smsMessage = (_userIdentity.Language == Language.en ? "New PinCode for your Masroofi account: " : "الكود الجديد لحسابكم في تطبيق مصروفي هو: ") + Encryption.DecryptData(entityToUpdate.PinCode, encryptionKey);
                var sendSmsResult = await _smsService.SendAsync(entityToUpdate.Phone, smsMessage);

                if (sendSmsResult.Code == "1901")
                    return OperationState.Updated;
                else
                    throw new BusinessException("Unable to send SMS with new PinCode to Parent Phone!");
            }
            else
            {
                throw new BusinessException("Unable to change PinCode!");
            }
        }

        public async Task<OperationState> UpdatePhone(UpdatePhoneViewModel model)
        {
            if (model.ParentId == Guid.Empty)
                throw new BusinessException("ParentId is required!");

            if (string.IsNullOrEmpty(model.Phone))
                throw new BusinessException("Phone is required!");

            var entityToUpdate = await _parentRepository.GetAsync(model.ParentId);

            if (entityToUpdate == null)
            {
                throw new BusinessException("Parent not found!");
            }

            model.Phone = model.Phone.Replace(" ", "");
            var phone = Encryption.DecryptData(model.Phone, encryptionKey);

            bool isPhoneExists = _parentRepository.GetAny(p => p.ParentId != entityToUpdate.ParentId && p.Phone == phone);
            if (isPhoneExists)
                throw new BusinessException($"Parent with phone: {model.Phone} already exists for another Parent");

            if (!phone.IsValidPhoneNumber())
            {
                throw new BusinessException("Invalid Phone number!");
            }

            entityToUpdate.Phone = phone;
            entityToUpdate.ModificationDate = DateTime.UtcNow;
            entityToUpdate.ModificationUser = _userIdentity.Id;

            return await unitOfWork.CommitAsync() > 0 ? OperationState.Updated : OperationState.None;
        }

        public async Task<decimal> GetBalanceAsync(Guid id)
        {
            var parent = await _parentRepository.GetAsync(id);
            if (parent == null)
            {
                throw new BusinessException("Parent Not found!");
            }
            return parent.CurrentBalance;
        }

        private Parent GetByPhone(string phone)
        {
            return _parentRepository.GetWhereInclude(c => c.Phone == phone && c.IsActive, c => c.City, c => c.City.Governorate);
        }

        private ParentInputViewModel ValidateParent(ParentInputViewModel model)
        {
            var errors = new List<Exception>();

            if (model.ParentId == null || model.ParentId == Guid.Empty)
                errors.Add(new Exception("ParentId is required!"));

            if (string.IsNullOrEmpty(model.Phone))
            {
                errors.Add(new Exception("Phone is required!"));
            }
            else
            {
                model.Phone = Encryption.DecryptData(model.Phone.Replace(" ", ""), encryptionKey);

                if (!model.Phone.IsValidPhoneNumber())
                    errors.Add(new Exception("Invalid Phone number!"));

                if (_parentRepository.GetAny(p => p.ParentId != model.ParentId && p.Phone == model.Phone))
                    errors.Add(new Exception($"Parent with phone: {model.Phone} already exists!"));
            }

            if (string.IsNullOrEmpty(model.PinCode))
            {
                errors.Add(new Exception("PinCode is required!"));
            }
            else
            {
                var decryptedPinCode = Encryption.DecryptData(model.PinCode.Replace(" ", ""), encryptionKey);
                if (!decryptedPinCode.IsValidSixDigits())
                    errors.Add(new Exception("PinCode must be 6 digits only!"));
            }

            if (string.IsNullOrEmpty(model.FullNameAr))
                errors.Add(new Exception("FullNameAr is required!"));

            if (string.IsNullOrEmpty(model.FullNameEn))
                errors.Add(new Exception("FullNameEn is required!"));

            if (!string.IsNullOrEmpty(model.Email) && !model.Email.IsValidEmailAddress())
                errors.Add(new Exception("Invalid Email Adress!"));

            if (!((Gender) model.Gender).IsEnumValid())
                errors.Add(new Exception("Gender is required!"));

            if (errors.Count > 0)
                throw new AggregateException(errors);
            else
                return model;
        }
    }
}
