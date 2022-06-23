using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.Core.ViewModels.PushNotifications
{
    public class NotificationScheduleOutputViewModel
    {
        public Guid NotificationScheduleId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string NotificationText { get; set; }
        public string NotificationTextEn { get; set; }
        public string NotificationData { get; set; }
        public Guid RecordId { get; set; }
        public DateTime PostDatetime { get; set; }
        public DateTime SendDateTime { get; set; }
        public DateTime ScheduleDateTime { get; set; }
        public int NotificationType { get; set; }
        public bool HasSend { get; set; }
        public bool HasError { get; set; }
        public string GlobalType { get; set; }
        public int? PrioritySend { get; set; }

        public List<ParentNotificationOutputViewModel> ParentNotifications { get; set; }
    }
}
