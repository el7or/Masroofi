using AutoMapper;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Roles;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface IRoleService
    {
        OperationState AddRole(AddEditRoleViewModel role);
        OperationState EditRole(AddEditRoleViewModel role);
        OperationState DeleteRole(Guid roleId);
        RoleInfoViewModel GetRoleById(Guid roleId);
        PagedOutput<RoleInfoViewModel> GetRolesByName(RoleFilterByTextInputViewModel model);
    }

    public class RoleService : BaseService, IRoleService
    {
        private readonly IRoleRepository RoleRepository;
        private readonly IUserRoleRepository UserRoleRepository;
        private readonly IRoleActionsRepository RoleActionsRepository;

        public RoleService(IRoleRepository RoleRepository, IUnitOfWork unitOfWork,
            IMapper mapper, IUserRoleRepository UserRoleRepository, IRoleActionsRepository RoleActionsRepository)
            : base(unitOfWork, mapper)
        {
            this.RoleRepository = RoleRepository;
            this.UserRoleRepository = UserRoleRepository;
            this.RoleActionsRepository = RoleActionsRepository;
        }

        public PagedOutput<RoleInfoViewModel> GetRolesByName(RoleFilterByTextInputViewModel model)
        {
            var roles = RoleRepository.GetAllWhere(r => r.RoleArabicName.ToLower().Contains(model.Text.ToLower())
                                                        || r.RoleEnglishName.ToLower().Contains(model.Text.ToLower()));


            return GetPaged(model, roles);
        }

        public RoleInfoViewModel GetRoleById(Guid id)
        {
            var role = RoleRepository.GetWhere(g => g.RoleId == id);
            return mapper.Map<Role, RoleInfoViewModel>(role);
        }

        public Role GetRoleByName(string arabicName, string englishName)
        {
            return RoleRepository.GetWhere(c => c.RoleArabicName.ToLower().Contains(arabicName.ToLower()) || c.RoleEnglishName.ToLower().Contains(englishName.ToLower()));
        }

        public OperationState AddRole(AddEditRoleViewModel role)
        {
            var existingRole = GetRoleByName(role.RoleArabicName, role.RoleEnglishName);
            if (existingRole != null)
            {
                return OperationState.Exists;
            }

            var mappedRole = mapper.Map<AddEditRoleViewModel, Role>(role);

            mappedRole.CreationDate = DateTime.UtcNow;
            RoleRepository.Add(mappedRole);
            int result = unitOfWork.Commit();

            if (result > 0)
            {
                role.RoleId = mappedRole.RoleId;
                return OperationState.Created;
            }
            else
            {
                return OperationState.None;
            }
        }

        public OperationState EditRole(AddEditRoleViewModel updatedRole)
        {
            var role = RoleRepository.GetWhere(g => g.RoleId == updatedRole.RoleId);

            var existingRole = GetRoleByName(updatedRole.RoleArabicName, updatedRole.RoleEnglishName);
            if (existingRole != null && existingRole.RoleId != updatedRole.RoleId)
            {
                return OperationState.Exists;
            }

            role.RoleArabicName = updatedRole.RoleArabicName;
            role.RoleEnglishName = updatedRole.RoleEnglishName;
            role.ModificationDate = DateTime.UtcNow;

            RoleRepository.Update(role);
            int result = unitOfWork.Commit();

            return (result > 0) ? OperationState.Updated : OperationState.None;
        }

        public OperationState DeleteRole(Guid roleId)
        {
            var role = RoleRepository.GetWhere(g => g.RoleId == roleId);
            if (role != null)
            {
                UserRoleRepository.Delete(u => u.RoleId == roleId);
                RoleActionsRepository.Delete(u => u.RoleId == roleId);
                RoleRepository.Delete(role);
                var result = unitOfWork.Commit();

                return result > 0 ? OperationState.Deleted : OperationState.None;
            }
            return OperationState.NotExists;
        }

        private PagedOutput<RoleInfoViewModel> GetPaged(RoleFilterByTextInputViewModel model, IEnumerable<Role> roles)
        {
            var output = new PagedOutput<RoleInfoViewModel>
            {
                TotalCount = roles.Count()
            };
            var result = RoleRepository.Table.ApplyPaging(model);
            output.Result = mapper.Map<List<RoleInfoViewModel>>(result.ToList());

            return output;
        }
    }
}
