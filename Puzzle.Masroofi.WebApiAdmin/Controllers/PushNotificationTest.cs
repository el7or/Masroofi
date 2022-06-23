using Microsoft.AspNetCore.Mvc;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.PushNotifications;
using Puzzle.Masroofi.ServiceInterface;
using System;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.WebApiAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PushNotificationTest : ControllerBase
    {
        private readonly IPushNotificationService _pushNotificationService;

        public PushNotificationTest(IPushNotificationService pushNotificationService)
        {
            _pushNotificationService = pushNotificationService;
        }

        [HttpPost("AddSchedual")]
        public async Task<IActionResult> AddSchedual(PushNotificationInputViewModel notificationDto)
        {
            var notification = await _pushNotificationService.CreatePushNotification(notificationDto);
            return Ok(new PuzzleApiResponse(notification));
        }

        [HttpGet("GetScheduals")]
        public async Task<IActionResult> GetScheduals()
        {
            var list = await _pushNotificationService.GetAllNotifications();
            return Ok(new PuzzleApiResponse(list));
        }

        [HttpPost("ResetScheduals")]
        public async Task<IActionResult> ResetScheduals(Guid notificationId)
        {
            var data = await _pushNotificationService.ResetPushNotification(notificationId);
            return Ok(new PuzzleApiResponse(data));
        }
    }
}
