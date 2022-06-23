using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.SendNotification.ViewModels
{
    public class DataForSend
    {
        public string RegisterId { get; set; }
        public string Device { get; set; }
        public string MType { get; set; }
        public string TitleAr { get; set; }
        public string Title { get; set; }
        public string NotificationText { get; set; }
        public Guid RecordId { get; set; }
        public string Global { get; set; }
        public string NotificationData { get; set; }
    }
}
