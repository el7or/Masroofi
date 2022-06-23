using AutoMapper;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels.ParentMobileRegistrations;
using Puzzle.Masroofi.Core.ViewModels.PushNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.Core.Mapper
{
    public class PushNotificationProfile : Profile
    {
        public PushNotificationProfile()
        {
            CreateMap<NotificationSchedule, NotificationScheduleOutputViewModel>();
            CreateMap<ParentNotification, ParentNotificationOutputViewModel>();
        }
    }
}
