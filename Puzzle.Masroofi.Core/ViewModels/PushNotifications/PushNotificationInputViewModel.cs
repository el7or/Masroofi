using Puzzle.Masroofi.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.Core.ViewModels.PushNotifications
{
    public class PushNotificationInputViewModel
    {
        public PushNotificationType NotificationType { get; set; }
        public Guid RecordId { get; set; }
        public decimal? Amount { get; set; }
        public ATMCardStatus? ATMCardStatus { get; set; }
    }
}
