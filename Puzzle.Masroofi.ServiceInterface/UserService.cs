using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.Roles;
using Puzzle.Masroofi.Core.ViewModels.Users;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(Guid id);
        Task<OperationState> AddUser(User user);
        Task<OperationState> EditUser(User user);
        User GetUser(string username, string password);
        Task<User> GetByUsername(string username);
        Task<OperationState> DeleteUser(Guid compoundId);
        Task<PagedOutput<UserList>> FilterUsers(UserFilter filter);
        Task<OperationState> activateUser(Guid userId, bool active);

        UserAuthenticationResponseModel GetUserAccessInformation(Guid userId);
    }

    public class UserService : BaseService, IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IUserRoleService userRoleService;
        private readonly IRoleService roleService;

        public UserService(IUserRepository userRepository,
                IUserRoleService userRoleService,
                IRoleService roleService,
                 IUnitOfWork unitOfWork,
                IMapper mapper)
                : base(unitOfWork, mapper)
        {
            this.userRepository = userRepository;
            this.userRoleService = userRoleService;
            this.roleService = roleService;
        }

        public User GetUser(string username, string password)
        {
            return userRepository.Table
                .Where(u => u.Username == username && u.Password == password)
                .Include(ur => ur.UserRoles)
                    .ThenInclude(r => r.Role)
                        .ThenInclude(a => a.ActionsInRoles)
                            .ThenInclude(pa => pa.SystemPageAction)
                .FirstOrDefault();
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await userRepository.GetAsync(id);
        }

        public async Task<OperationState> AddUser(User user)
        {
            var existsUser = await GetByUsername(user.Username);
            if (existsUser != null)
            {
                return OperationState.Exists;
            }
            user.IsActive = true;
            user.IsDeleted = false;
            user.IsVerified = false;
            user.CreationDate = DateTime.UtcNow;

            userRepository.Add(user);
            int result = await unitOfWork.CommitAsync();
            if (result > 0)
                return OperationState.Created;
            return OperationState.None;
        }

        public async Task<OperationState> EditUser(User user)
        {
            var existsUser = await GetUserByIdAsync(user.UserId);

            if (existsUser == null)
            {
                return OperationState.NotExists;
            }

            existsUser.Username = user.Username;
            existsUser.Password = user.Password;
            existsUser.NameAr = user.NameAr;
            existsUser.NameEn = user.NameEn;
            existsUser.Phone = user.Phone;
            existsUser.Email = user.Email;
            if (user.Image != null)
                existsUser.Image = user.Image;

            userRepository.Update(existsUser);

            int result = await unitOfWork.CommitAsync();

            return result > 0 ? OperationState.Updated : OperationState.None;
        }

        public async Task<OperationState> DeleteUser(Guid userId)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
            {
                return OperationState.NotExists;
            }

            user.IsDeleted = true;
            userRepository.Update(user);
            var result = await unitOfWork.CommitAsync();
            return result > 0 ? OperationState.Deleted : OperationState.None;
        }

        public async Task<User> GetByUsername(string username)
        {
            return await userRepository.Table.FirstOrDefaultAsync(c => c.Username == username);
        }

        public async Task<OperationState> activateUser(Guid userId, bool active)
        {
            var existsUser = await GetUserByIdAsync(userId);
            if (existsUser == null)
            {
                return OperationState.NotExists;
            }
            existsUser.IsActive = active;
            userRepository.Update(existsUser);
            int result = await unitOfWork.CommitAsync();
            return result > 0 ? OperationState.Updated : OperationState.None;
        }

        public async Task<PagedOutput<UserList>> FilterUsers(UserFilter filter)
        {
            var users = userRepository.Table
                .Include(u => u.UserRoles)
                    .ThenInclude(r => r.Role)
                        .ThenInclude(r => r.ActionsInRoles)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.SearchText))
                users = users.Where(z => z.NameAr.Contains(filter.SearchText) ||
                z.NameEn.Contains(filter.SearchText) || z.Email.Contains(filter.SearchText)
                || z.Username.Contains(filter.SearchText) || z.Phone.Contains(filter.SearchText));

            // apply sorting
            var columns = new Dictionary<string, Expression<Func<User, object>>>()
            {
                ["nameAr"] = v => v.NameAr,
                ["nameEn"] = v => v.NameEn,
                ["isActive"] = v => v.IsActive,

            };
            users = users.ApplySorting(filter, columns);

            var filteredUsers = users
                    .Select(z => new UserList()
                    {
                        UserId = z.UserId,
                        NameAr = z.NameAr,
                        NameEn = z.NameEn,
                        IsActive = z.IsActive,
                        Username = z.Username,
                        Email = z.Email,
                        Phone = z.Phone,
                        IsSelected = filter.RoleId.HasValue ? z.UserRoles.Any(r => r.RoleId == filter.RoleId) : false,
                        CurrentRole = z.UserRoles.Any() ? mapper.Map<CurrentRoleViewModel>(z.UserRoles.First().Role) : null
                    });

            var pageUsers = await filteredUsers.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize)
                    .ToListAsync();
            return new PagedOutput<UserList>
            {
                TotalCount = filteredUsers.Count(),
                Result = pageUsers
            };
        }

        public UserAuthenticationResponseModel GetUserAccessInformation(Guid userId)
        {
            var accessInfo = new UserAuthenticationResponseModel();

            var userRoles = userRoleService.GetUserRolesByUserId(userId);

            accessInfo.RolesIds = userRoles.Select(r => r.RoleId).ToList();
            accessInfo.UserActions = userRoleService.GetActionsInRoles(accessInfo.RolesIds);

            return accessInfo;
        }
    }
}
