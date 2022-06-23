using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.ParentMobileRegistrations;
using Puzzle.Masroofi.Core.ViewModels.PushNotifications;
using Puzzle.Masroofi.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.WebApiParents.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParentMobileRegistrationsController : ControllerBase
    {
        private readonly IParentMobileRegistrationService _parentMobileRegistrationService;
        private readonly IPushNotificationService _notificationService;

        public ParentMobileRegistrationsController(IParentMobileRegistrationService parentMobileRegistrationService, IPushNotificationService notificationService)
        {
            _parentMobileRegistrationService = parentMobileRegistrationService;
            _notificationService = notificationService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ParentMobileRegistrationOutputViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddMobile([FromBody] ParentMobileRegistrationInputViewModel model)
        {
            await _notificationService.CreatePushNotification(new PushNotificationInputViewModel
            {
                NotificationType = PushNotificationType.ParentRegistered,
                RecordId = model.ParentId
            });
            var output = await _parentMobileRegistrationService.AddRegistration(model);
            return Ok(new PuzzleApiResponse(output));
        }

        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteByUserId(Guid userId)
        {
            await _parentMobileRegistrationService.DeleteByUserId(userId);
            return Ok(new PuzzleApiResponse(new { }));
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveRegistrationId(string registerId)
        {
            await _parentMobileRegistrationService.RemoveRegistrationId(registerId);
            return Ok(new PuzzleApiResponse(new { }));
        }
    }
}
