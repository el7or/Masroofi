using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class SonRepository : RepositoryBase<Son>, ISonRepository
    {
        public SonRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface ISonRepository : IRepository<Son>
    {
    }
}
