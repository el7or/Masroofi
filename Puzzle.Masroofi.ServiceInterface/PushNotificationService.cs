using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Puzzle.Masroofi.Core.Constants;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.PushNotifications;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface IPushNotificationService
    {
        Task<NotificationSchedule> CreatePushNotification(PushNotificationInputViewModel pushNotification);
        Task<List<NotificationScheduleOutputViewModel>> GetAllNotifications();
        Task<bool> ResetPushNotification(Guid id);
    }

    public class PushNotificationService : BaseService, IPushNotificationService
    {
        private readonly INotificationScheduleRepository _notificationScheduleRepository;
        private readonly IParentRepository _parentRepository;
        private readonly UserIdentity _userIdentity;
        private readonly IOptionsSnapshot<RouteAndroid> _routeAndroid;
        private readonly IConfiguration _configuration;

        public PushNotificationService(INotificationScheduleRepository notificationScheduleRepository,
            IParentRepository parentRepository,
            UserIdentity userIdentity,
            IUnitOfWork unitOfWork,
            IMapper mapper, IOptionsSnapshot<RouteAndroid> routeAndroid, IConfiguration configuration)
            : base(unitOfWork, mapper)
        {
            _userIdentity = userIdentity;
            _notificationScheduleRepository = notificationScheduleRepository;
            _parentRepository = parentRepository;
            _routeAndroid = routeAndroid;
            _configuration = configuration;
        }

        public async Task<NotificationSchedule> CreatePushNotification(PushNotificationInputViewModel pushNotification)
        {
            return await SaveNotification(pushNotification);
        }

        public async Task<List<NotificationScheduleOutputViewModel>> GetAllNotifications()
        {
            var notificationData = await _notificationScheduleRepository.TableNoTracking
                .Include(q => q.ParentNotifications)
                .ToListAsync();

            return mapper.Map<List<NotificationScheduleOutputViewModel>>(notificationData);
        }

        public async Task<bool> ResetPushNotification(Guid id)
        {
            var ExistRecord = _notificationScheduleRepository.Get(id)
                ?? throw new BusinessException("Notification schedual is not exist.");

            ExistRecord.IsActive = true;
            ExistRecord.HasSend = false;
            ExistRecord.HasError = false;
            ExistRecord.IsDeleted = false;

            _notificationScheduleRepository.Update(ExistRecord);
            var excuted = await unitOfWork.CommitAsync();

            return excuted > 0;
        }

        private async Task<NotificationSchedule> SaveNotification(PushNotificationInputViewModel pushNotification)
        {
            if (pushNotification.RecordId == Guid.Empty) return null;

            var checkRecordExist = _notificationScheduleRepository.GetWhere(q =>
            q.RecordId == pushNotification.RecordId &&
            q.NotificationType == (int)pushNotification.NotificationType);

            if (checkRecordExist != null) return null;

            var notificationSchedule = new NotificationSchedule()
            {
                NotificationType = (int)pushNotification.NotificationType,
                IsActive = true,
                RecordId = pushNotification.RecordId,
                CreatedDate = DateTime.Now,
                ScheduleDateTime = DateTime.Now,
                PostDatetime = DateTime.Now,
                CreatedBy = _userIdentity?.Id.ToString(),
                PrioritySend = (int)SendPriority.Low
            };

            switch (pushNotification.NotificationType)
            {
                case PushNotificationType.ParentRegistered:
                    ParentRegistered(notificationSchedule, NotificationMessagesConstants.ParentRegisteredAr, NotificationMessagesConstants.ParentRegisteredEn);
                    break;
                case PushNotificationType.ChargeBySystem:
                    ParentWalletTransaction(notificationSchedule, NotificationMessagesConstants.ChargeBySystemAr, NotificationMessagesConstants.ChargeBySystemEn, pushNotification.Amount);
                    break;
                case PushNotificationType.ChargeByCreditCard:
                    ParentWalletTransaction(notificationSchedule, NotificationMessagesConstants.ChargeByCreditCardAr, NotificationMessagesConstants.ChargeByCreditCardEn, pushNotification.Amount);
                    break;
                case PushNotificationType.ChargeByVendor:
                    ParentWalletTransaction(notificationSchedule, NotificationMessagesConstants.ChargeByVendorAr, NotificationMessagesConstants.ChargeByVendorEn, pushNotification.Amount);
                    break;
                case PushNotificationType.ChargeByAdmin:
                    ParentWalletTransaction(notificationSchedule, NotificationMessagesConstants.ChargeByAdminAr, NotificationMessagesConstants.ChargeByAdminEn, pushNotification.Amount);
                    break;
                case PushNotificationType.WithdrawBySystem:
                    ParentWalletTransaction(notificationSchedule, NotificationMessagesConstants.WithdrawBySystemAr, NotificationMessagesConstants.WithdrawBySystemEn, pushNotification.Amount);
                    break;
                case PushNotificationType.WithdrawByAdmin:
                    ParentWalletTransaction(notificationSchedule, NotificationMessagesConstants.WithdrawByAdminAr, NotificationMessagesConstants.WithdrawByAdminEn, pushNotification.Amount);
                    break;
                case PushNotificationType.ChargeATMCard:
                    ParentWalletTransaction(notificationSchedule, NotificationMessagesConstants.ChargeATMCardAr, NotificationMessagesConstants.ChargeATMCardEn, pushNotification.Amount);
                    break;
                case PushNotificationType.RefundFromATMCard:
                    ParentWalletTransaction(notificationSchedule, NotificationMessagesConstants.RefundFromATMCardAr, NotificationMessagesConstants.RefundFromATMCardEn, pushNotification.Amount);
                    break;
                case PushNotificationType.PayByATMCardToVendor:
                    ParentWalletTransaction(notificationSchedule, NotificationMessagesConstants.PayByATMCardToVendorAr, NotificationMessagesConstants.PayByATMCardToVendorEn, pushNotification.Amount);
                    break;
                case PushNotificationType.RefundFromVendorToATMCard:
                    ParentWalletTransaction(notificationSchedule, NotificationMessagesConstants.RefundFromVendorToATMCardAr, NotificationMessagesConstants.RefundFromVendorToATMCardEn, pushNotification.Amount);
                    break;
                case PushNotificationType.RequestNewATMCard:
                    RequestNewATMCard(notificationSchedule, NotificationMessagesConstants.RequestNewATMCardAr, NotificationMessagesConstants.RequestNewATMCardEn);
                    break;
                case PushNotificationType.ATMCardStatusChanged:
                    ATMCardStatusChanged(notificationSchedule, NotificationMessagesConstants.ATMCardStatusChangedAr, NotificationMessagesConstants.ATMCardStatusChangedEn, (ATMCardStatus) pushNotification.ATMCardStatus);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(pushNotification.NotificationType), pushNotification.NotificationType, null);
            }

            _notificationScheduleRepository.Add(notificationSchedule);

            await unitOfWork.CommitAsync();

            return notificationSchedule;
        }

        private string ContentAr, ContentEn, route_android;
        private void ParentRegistered(NotificationSchedule notification, string messageAr, string messageEn)
        {
            var recordData = _parentRepository.Get(notification.RecordId)
                            ?? throw new BusinessException("Parent is not exist");

            ContentAr = string.Format(messageAr);
            ContentEn = string.Format(messageEn);
            var Data = new Dictionary<string, string>()
            {
                { nameof(route_android), _routeAndroid.Value.GetActivity(notification.NotificationType)},
                { nameof(NotificationMessagesConstants.TitleAr),NotificationMessagesConstants.TitleAr},
                { nameof(NotificationMessagesConstants.TitleEn),NotificationMessagesConstants.TitleEn},
                { nameof(ContentAr),ContentAr},
                { nameof(ContentEn),ContentEn},
                //
                { nameof(notification.RecordId),notification.RecordId.ToString() },
                { nameof(notification.NotificationType),notification.NotificationType.ToString() }
            };

            notification.NotificationData = JsonConvert.SerializeObject(Data);
            notification.NotificationText = ContentAr;
            notification.NotificationTextEn = ContentEn;
            notification.PrioritySend = (int)SendPriority.High;
            notification.ParentNotifications = new List<ParentNotification>() { new ParentNotification { ParentId = recordData.ParentId } };
        }
        private void ParentWalletTransaction(NotificationSchedule notification, string messageAr, string messageEn, decimal? amount)
        {
            var recordData = _parentRepository.Get(notification.RecordId)
                            ?? throw new BusinessException("Parent is not exist");

            ContentAr = string.Format(messageAr, amount + " " + _configuration.GetSection("CurrencySymbol:ar").Value);
            ContentEn = string.Format(messageEn, amount + " " + _configuration.GetSection("CurrencySymbol:en").Value);
            var Data = new Dictionary<string, string>()
            {
                { nameof(route_android), _routeAndroid.Value.GetActivity(notification.NotificationType)},
                { nameof(NotificationMessagesConstants.TitleAr),NotificationMessagesConstants.TitleAr},
                { nameof(NotificationMessagesConstants.TitleEn),NotificationMessagesConstants.TitleEn},
                { nameof(ContentAr),ContentAr},
                { nameof(ContentEn),ContentEn},
                //
                { nameof(notification.RecordId),notification.RecordId.ToString() },
                { nameof(notification.NotificationType),notification.NotificationType.ToString() }
            };

            notification.NotificationData = JsonConvert.SerializeObject(Data);
            notification.NotificationText = ContentAr;
            notification.NotificationTextEn = ContentEn;
            notification.PrioritySend = (int)SendPriority.High;
            notification.ParentNotifications = new List<ParentNotification>() { new ParentNotification { ParentId = recordData.ParentId } };
        }
        private void RequestNewATMCard(NotificationSchedule notification, string messageAr, string messageEn)
        {
            var recordData = _parentRepository.Get(notification.RecordId)
                            ?? throw new BusinessException("Parent is not exist");

            ContentAr = string.Format(messageAr);
            ContentEn = string.Format(messageEn);
            var Data = new Dictionary<string, string>()
            {
                { nameof(route_android), _routeAndroid.Value.GetActivity(notification.NotificationType)},
                { nameof(NotificationMessagesConstants.TitleAr),NotificationMessagesConstants.TitleAr},
                { nameof(NotificationMessagesConstants.TitleEn),NotificationMessagesConstants.TitleEn},
                { nameof(ContentAr),ContentAr},
                { nameof(ContentEn),ContentEn},
                //
                { nameof(notification.RecordId),notification.RecordId.ToString() },
                { nameof(notification.NotificationType),notification.NotificationType.ToString() }
            };

            notification.NotificationData = JsonConvert.SerializeObject(Data);
            notification.NotificationText = ContentAr;
            notification.NotificationTextEn = ContentEn;
            notification.PrioritySend = (int)SendPriority.High;
            notification.ParentNotifications = new List<ParentNotification>() { new ParentNotification { ParentId = recordData.ParentId } };
        }
        private void ATMCardStatusChanged(NotificationSchedule notification, string messageAr, string messageEn, ATMCardStatus atmCardStatus)
        {
            var recordData = _parentRepository.Get(notification.RecordId)
                            ?? throw new BusinessException("Parent is not exist");

            CultureInfo cultureInfo = new CultureInfo("ar");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            ContentAr = string.Format(messageAr, atmCardStatus.GetDescription());
            ContentEn = string.Format(messageEn, atmCardStatus.ToString());
            var Data = new Dictionary<string, string>()
            {
                { nameof(route_android), _routeAndroid.Value.GetActivity(notification.NotificationType)},
                { nameof(NotificationMessagesConstants.TitleAr),NotificationMessagesConstants.TitleAr},
                { nameof(NotificationMessagesConstants.TitleEn),NotificationMessagesConstants.TitleEn},
                { nameof(ContentAr),ContentAr},
                { nameof(ContentEn),ContentEn},
                //
                { nameof(notification.RecordId),notification.RecordId.ToString() },
                { nameof(notification.NotificationType),notification.NotificationType.ToString() }
            };

            notification.NotificationData = JsonConvert.SerializeObject(Data);
            notification.NotificationText = ContentAr;
            notification.NotificationTextEn = ContentEn;
            notification.PrioritySend = (int)SendPriority.High;
            notification.ParentNotifications = new List<ParentNotification>() { new ParentNotification { ParentId = recordData.ParentId } };
        }
    }
}
