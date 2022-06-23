using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.Commissions;
using Puzzle.Masroofi.ServiceInterface;
using System;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.WebApiAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommissionsController : ControllerBase
    {
        private readonly ICommissionService _commissionService;
        private readonly UserIdentity _userIdentity;

        public CommissionsController(ICommissionService commissionService, UserIdentity userIdentity)
        {
            _commissionService = commissionService;
            _userIdentity = userIdentity;
        }

        /// <summary>
        /// Get Commissions list
        /// </summary>
        /// <remarks>* You can filter and sort and paging for the list.</remarks>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CommissionFilterViewModel model)
        {
            var result = await _commissionService.GetAllAsync(model);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Get Commission By CommissionId
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _commissionService.GetAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Add new Commission
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CommissionInputViewModel model)
        {
            await _commissionService.AddAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Update Commission
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CommissionInputViewModel model)
        {
            await _commissionService.UpdateAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Delete Commission
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _commissionService.DeleteAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Get Commission value for specific Parent
        /// </summary>
        /// <remarks>
        /// * Must Send one of ParentId or ParentPhone.
        /// * If will send ParentPhone send it without country code e.g: 01234567899
        /// * Amount is: decimal Transaction Amount.
        /// </remarks>
        [HttpGet("for-parent")]
        public async Task<IActionResult> GetParentCommission([FromQuery] ParentCommissionInputViewModel model)
        {
            ChargeParentWalletCommissionType commissionType = 0;

            if (_userIdentity.Channel == ChannelType.Mobile)
                commissionType = ChargeParentWalletCommissionType.ByCreditCard;

            if (_userIdentity.Channel == ChannelType.POS)
                commissionType = ChargeParentWalletCommissionType.ByPOSMachine;

            if(_userIdentity.Channel == ChannelType.Admin)
                return Ok(new PuzzleApiResponse(message: "Can not get Commission value from Admin project!", statusCode: 400));

            var result = await _commissionService.GetParentCommissionAsync(model, commissionType);
            return Ok(new PuzzleApiResponse(result));
        }


    }
}
