using Microsoft.EntityFrameworkCore;
using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class UserRoleRepository : RepositoryBase<UserRole>, IUserRoleRepository
    {
        private readonly MasroofiDbContext _dbContext;
        public UserRoleRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public List<string> GetActionsInRoles(List<Guid> rolesIds)
        {
            return _dbContext.ActionsInRoles.Where(a => rolesIds.Contains(a.RoleId))
                 .Include(a => a.SystemPageAction).Select(a => a.SystemPageAction.ActionUniqueName).ToList();
        }
    }

    public interface IUserRoleRepository : IRepository<UserRole>
    {
        List<string> GetActionsInRoles(List<Guid> rolesIds);
    }
}
