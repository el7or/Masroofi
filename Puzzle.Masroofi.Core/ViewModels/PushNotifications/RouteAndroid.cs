using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.Core.ViewModels.PushNotifications
{
    public class RouteAndroid
    {
        public string BaseUrl { get; set; }
        public List<RouteAndroidActivity> Activities { get; set; }

        public string GetActivity(int notificationType)
        {
            return BaseUrl + Activities.FirstOrDefault(q => q.Id == notificationType).Activity;
        }
    }
}
