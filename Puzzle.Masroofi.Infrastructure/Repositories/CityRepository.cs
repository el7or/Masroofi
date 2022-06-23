using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class CityRepository : RepositoryBase<City>, ICityRepository
    {
        public CityRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface ICityRepository : IRepository<City>
    {
    }
}
