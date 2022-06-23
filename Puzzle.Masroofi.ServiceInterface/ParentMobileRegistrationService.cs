using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.ParentMobileRegistrations;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface IParentMobileRegistrationService
    {
        Task<ParentMobileRegistrationOutputViewModel> AddRegistration(ParentMobileRegistrationInputViewModel model);
        Task DeleteByUserId(Guid userId);
        Task RemoveRegistrationId(string registerId);

        //Task<PagedOutput<ParentMobileRegistrationOutputViewModel>> GetAllAsync(ParentMobileRegistrationFilterViewModel model);
        //Task<ParentMobileRegistrationOutputViewModel> GetAsync(Guid id);
        //Task AddAsync(ParentMobileRegistrationInputViewModel model);
        //Task UpdateAsync(ParentMobileRegistrationInputViewModel model);
        //Task<OperationState> DeleteAsync(Guid id);
    }
    public class ParentMobileRegistrationService : BaseService, IParentMobileRegistrationService
    {
        private readonly IParentMobileRegistrationRepository _parentMobileRegistrationRepository;
        private readonly IParentRepository _parentRepository;
        private readonly UserIdentity _userIdentity;
        private readonly IS3Service _s3Service;
        public ParentMobileRegistrationService(IParentMobileRegistrationRepository parentMobileRegistrationRepository,
            IParentRepository parentRepository,
            UserIdentity userIdentity,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IS3Service s3Service) : base(unitOfWork, mapper)
        {
            _parentMobileRegistrationRepository = parentMobileRegistrationRepository;
            _parentRepository = parentRepository;
            _userIdentity = userIdentity;
            _s3Service = s3Service;
        }

        public async Task<ParentMobileRegistrationOutputViewModel> AddRegistration(ParentMobileRegistrationInputViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.RegisterId))
                throw new BusinessException("RegisterId is Invalid");
            if (model.ParentId == Guid.Empty)
                throw new BusinessException("ParentId is Invalid");
            if (model.RegisterType != "android" && model.RegisterType != "ios")
                throw new BusinessException("RegisterType is Invalid");
            if (_parentMobileRegistrationRepository.GetAny(x => x.RegisterId == model.RegisterId && x.ParentId == model.ParentId))
                throw new BusinessException("RegisterId exist before for this Parent");

            var prevRegister = await _parentMobileRegistrationRepository.GetWhereAsync(p => p.RegisterId == model.RegisterId);
            if (prevRegister == null)
            {

                var register = new ParentMobileRegistration()
                {
                    ParentMobileRegistrationId = Guid.NewGuid(),
                    RegisterId = model.RegisterId,
                    IsActive = true,
                    CreationUser = _userIdentity.Id ?? Guid.Empty,
                    CreationDate = DateTime.UtcNow,
                    ParentId = model.ParentId,
                    RegisterType = model.RegisterType
                };
                _parentMobileRegistrationRepository.Add(register);
                await unitOfWork.CommitAsync();

                return mapper.Map<ParentMobileRegistrationOutputViewModel>(register);
            }
            else
            {
                return mapper.Map<ParentMobileRegistrationOutputViewModel>(prevRegister);
            }
        }

        public async Task DeleteByUserId(Guid parentId)
        {
            if (parentId == Guid.Empty)
                throw new BusinessException("ParentId is Invalid");

            var registrationForDelete = _parentMobileRegistrationRepository.GetAllWhere(x => x.ParentId == parentId);
            if (!registrationForDelete.Any())
                throw new BusinessException("No registrations found");

            _parentMobileRegistrationRepository.Delete(x => x.ParentId == parentId);
            await unitOfWork.CommitAsync();
        }

        public async Task RemoveRegistrationId(string registerId)
        {
            var registration = _parentMobileRegistrationRepository.GetWhere(x => x.RegisterId == registerId);
            if (registration == null)
                throw new BusinessException("no registrations found");

            _parentMobileRegistrationRepository.Delete(x => x.RegisterId == registerId);

            await unitOfWork.CommitAsync();

        }
    }
}
