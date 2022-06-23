using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class RoleActionsRepository : RepositoryBase<ActionsInRoles>, IRoleActionsRepository
    {
        public RoleActionsRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IRoleActionsRepository : IRepository<ActionsInRoles>
    {
    }
}
