using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.Infrastructure.Repositories
{
    public class NotificationScheduleRepository : RepositoryBase<NotificationSchedule>, INotificationScheduleRepository
    {
        public NotificationScheduleRepository(MasroofiDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface INotificationScheduleRepository : IRepository<NotificationSchedule>
    {
    }
}
