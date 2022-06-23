using AutoMapper;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.UserRoles;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface IUserRoleService
    {
        OperationState AddUserRole(AddEditUserRoleViewModel userRole);
        OperationState EditUserRole(AddEditUserRoleViewModel userRole);
        OperationState DeleteUserRole(Guid userRoleId);
        UserRoleInfoViewModel GetUserRoleInfoById(Guid userRoleId);
        UserRole GetUserRoleById(Guid userRoleId);
        PagedOutput<UserRoleInfoViewModel> GetUserRoles(UserRoleFilterByTextInputViewModel model);
        PagedOutput<UserRoleInfoViewModel> GetUserRolesByRoleId(UserRoleFilterByTextInputViewModel model);
        OperationState UpdateUsersRoles(UpdateUserRoles model);
        List<string> GetActionsInRoles(List<Guid> rolesIds);
        IEnumerable<UserRoleInfoViewModel> GetUserRolesByUserId(Guid userId);
    }

    public class UserRoleService : BaseService, IUserRoleService
    {
        private readonly IUserRoleRepository userRoleRepository;

        public UserRoleService(IUserRoleRepository userRoleRepository, IUnitOfWork unitOfWork,
            IMapper mapper)
            : base(unitOfWork, mapper)
        {
            this.userRoleRepository = userRoleRepository;
        }

        public OperationState AddUserRole(AddEditUserRoleViewModel userRole)
        {
            var existingRole = userRoleRepository.GetWhere(r => r.UserId == userRole.UserId && r.RoleId == userRole.RoleId);
            if (existingRole != null)
            {
                return OperationState.Exists;
            }
            userRole.AssignedDate = DateTime.UtcNow;

            var mappedUserRole = mapper.Map<AddEditUserRoleViewModel, UserRole>(userRole);

            mappedUserRole.CreationDate = DateTime.UtcNow;
            userRoleRepository.Add(mappedUserRole);
            int result = unitOfWork.Commit();

            if (result > 0)
            {
                userRole.UserRoleId = mappedUserRole.UserRoleId;
                return OperationState.Created;
            }
            else
            {
                return OperationState.None;
            }
        }

        public PagedOutput<UserRoleInfoViewModel> GetUserRoles(UserRoleFilterByTextInputViewModel model)
        {
            var roles = userRoleRepository.GetAllWhere(r => r.UserId == model.UserId
                                                        || r.RoleId == model.RoleId);

            return GetPaged(model, roles);
        }

        public IEnumerable<UserRoleInfoViewModel> GetUserRolesByUserId(Guid userId)
        {
            var roles = userRoleRepository.GetAllWhere(r => r.UserId == userId);

            return mapper.Map<List<UserRoleInfoViewModel>>(roles);
        }

        public UserRole GetUserRoleById(Guid id)
        {
            return userRoleRepository.GetWhere(g => g.UserRoleId == id);
        }

        public UserRoleInfoViewModel GetUserRoleInfoById(Guid id)
        {
            var role = userRoleRepository.GetWhere(g => g.UserRoleId == id);
            return mapper.Map<UserRole, UserRoleInfoViewModel>(role);
        }

        public OperationState EditUserRole(AddEditUserRoleViewModel updatedUserRole)
        {
            var existingUserRole = userRoleRepository.GetWhere(r => r.UserId == updatedUserRole.UserId && r.RoleId == updatedUserRole.RoleId);

            if (existingUserRole != null && existingUserRole.RoleId == updatedUserRole.RoleId)
            {
                return OperationState.Exists;
            }

            existingUserRole.ModificationDate = DateTime.UtcNow;

            userRoleRepository.Update(existingUserRole);
            int result = unitOfWork.Commit();

            return (result > 0) ? OperationState.Updated : OperationState.None;
        }

        public OperationState DeleteUserRole(Guid userRoleId)
        {
            var role = userRoleRepository.GetWhere(g => g.RoleId == userRoleId);
            if (role != null)
            {
                userRoleRepository.Delete(role);
                var result = unitOfWork.Commit();

                return result > 0 ? OperationState.Deleted : OperationState.None;
            }
            return OperationState.NotExists;
        }

        public PagedOutput<UserRoleInfoViewModel> GetUserRolesByRoleId(UserRoleFilterByTextInputViewModel model)
        {
            var roles = userRoleRepository.GetAllWhere(c => c.RoleId == model.RoleId);

            return GetPaged(model, roles);
        }

        private PagedOutput<UserRoleInfoViewModel> GetPaged(UserRoleFilterByTextInputViewModel model, IEnumerable<UserRole> roles)
        {
            var output = new PagedOutput<UserRoleInfoViewModel>
            {
                TotalCount = roles.Count()
            };
            var result = userRoleRepository.Table.ApplyPaging(model);
            output.Result = mapper.Map<List<UserRoleInfoViewModel>>(result.ToList());

            return output;
        }

        public OperationState UpdateUsersRoles(UpdateUserRoles model)
        {
            var oldUserRoles = userRoleRepository.GetAllWhere(r => r.RoleId == model.UsersRoles.First().RoleId || model.UsersRoles.Select(u => u.UserId).Contains(r.UserId));
            foreach (var item in oldUserRoles)
            {
                userRoleRepository.Delete(item);
            }
            foreach (var item in model.UsersRoles)
            {
                userRoleRepository.Add(new UserRole
                {
                    UserId = item.UserId,
                    RoleId = item.RoleId,
                    CreationDate = DateTime.UtcNow
                });
            }
            var result = unitOfWork.Commit();
            return (result > 0) ? OperationState.Updated : OperationState.None;
        }

        public List<string> GetActionsInRoles(List<Guid> rolesIds)
        {
            return userRoleRepository.GetActionsInRoles(rolesIds);
        }
    }
}
