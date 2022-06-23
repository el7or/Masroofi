using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class GovernorateRepository : RepositoryBase<Governorate>, IGovernorateRepository
    {
        public GovernorateRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IGovernorateRepository : IRepository<Governorate>
    {
    }
}
