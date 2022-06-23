using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        public RoleRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IRoleRepository : IRepository<Role>
    {
    }
}
