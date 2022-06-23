using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class VendorRepository : RepositoryBase<Vendor>, IVendorRepository
    {
        public VendorRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IVendorRepository : IRepository<Vendor>
    {
    }
}
