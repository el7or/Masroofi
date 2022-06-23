using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.Core.ViewModels.PushNotifications
{
    public class ParentNotificationOutputViewModel
    {
        public Guid NotificationParentId { get; set; }
        public Guid ParentId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Guid NotificationScheduleId { get; set; }
        public bool IsRead { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
