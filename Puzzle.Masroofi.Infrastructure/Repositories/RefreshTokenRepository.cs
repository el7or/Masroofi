using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class RefreshTokenRepository : RepositoryBase<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
    }
}
