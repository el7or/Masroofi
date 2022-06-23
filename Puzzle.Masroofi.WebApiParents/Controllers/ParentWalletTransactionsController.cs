using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.ParentWalletTransactions;
using Puzzle.Masroofi.ServiceInterface;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.WebApiParents.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParentWalletTransactionsController : ControllerBase
    {
        private readonly IParentWalletTransactionService _transactionService;
        private readonly UserIdentity _userIdentity;

        public ParentWalletTransactionsController(IParentWalletTransactionService transactionService,
            UserIdentity userIdentity)
        {
            _transactionService = transactionService;
            _userIdentity = userIdentity;
        }

        /// <summary>
        /// Charge Parent Wallet By Credit Card
        /// </summary>
        /// <param name="model">
        /// * Send Amount value and Commission Values for Charge
        /// </param>
        [HttpPost("charge-by-credit-card")]
        public async Task<ActionResult> ChargeByCreditCard([FromBody] ChargeByCreditCardViewModel model)
        {
            if (_userIdentity.Channel != ChannelType.Mobile)
                throw new BusinessException("Wrong Channel Type");

            await _transactionService.ChargeByCreditCardAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Update Charge Parent Wallet By Credit Card
        /// </summary>
        /// <param name="model">
        /// * Must send parentWalletTransactionId encrypted.
        /// </param>
        [HttpPut("charge-by-credit-card")]
        public async Task<ActionResult> UpdateChargeByCreditCard([FromBody] UpdateChargeByCreditCardViewModel model)
        {
            if (_userIdentity.Channel != ChannelType.Mobile)
                throw new BusinessException("Wrong Channel Type");

            await _transactionService.UpdateChargeByCreditCardAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Charge Parent Wallet By Vendor POS
        /// </summary>
        /// <param name="model">
        /// * Must Send one of ParentId or ParentPhone.
        /// * If will send ParentPhone send it without country code e.g: 01234567899
        /// * Send Amount value for Charge and Commission Values.
        /// * Must send POSMachinePinCode encrypted.
        /// </param>
        [HttpPost("charge-by-vendor")]
        public async Task<ActionResult> ChargeByVendor([FromBody] ChargeByVendorViewModel model)
        {
            if (_userIdentity.Channel != ChannelType.POS)
                throw new BusinessException("Wrong Channel Type");

            await _transactionService.ChargeByVendorAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Charge Parent Wallet By Admin
        /// </summary>
        /// <param name="model">
        /// * Send Amount value for Charge
        /// </param>
        [HttpPost("charge-by-admin")]
        public async Task<ActionResult> ChargeByAdmin([FromBody] ChargeByAdminViewModel model)
        {
            if (_userIdentity.Channel != ChannelType.Admin)
                throw new BusinessException("Wrong Channel Type");

            await _transactionService.ChargeByAdminAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Withdraw From Parent Wallet By Admin
        /// </summary>
        /// <param name="model">
        /// * Send Amount value for Withdraw
        /// </param>
        [HttpPost("withdraw-by-admin")]
        public async Task<ActionResult> WithdrawByAdmin([FromBody] WithdrawByAdminViewModel model)
        {
            if (_userIdentity.Channel != ChannelType.Admin)
                throw new BusinessException("Wrong Channel Type");

            await _transactionService.WithdrawByAdminAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Get ParentWalletTransactions list
        /// </summary>
        /// <remarks>* You can filter and sort and paging for the list.</remarks>
        /// <response code="200">paymentType is enum: Visa = 1, Cash = 2, Transfer = 3, Gift = 4, Withdraw = 5</response>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ParentWalletTransactionFilterViewModel model)
        {
            var result = await _transactionService.GetAllAsync(model);
            return Ok(new PuzzleApiResponse(result));
        }
    }
}
