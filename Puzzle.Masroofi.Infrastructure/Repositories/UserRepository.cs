using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IUserRepository : IRepository<User>
    {
    }
}
