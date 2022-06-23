using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.ATMCardTransactions;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.ServiceInterface;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.WebApiAtmTransactions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ATMCardTransactionsController : ControllerBase
    {
        private readonly IATMCardTransactionService _transactionService;
        private readonly UserIdentity _userIdentity;

        public ATMCardTransactionsController(IATMCardTransactionService transactionService,
            UserIdentity userIdentity)
        {
            _transactionService = transactionService;
            _userIdentity = userIdentity;
        }

        /// <summary>
        /// Charge ATMCard By Parent Wallet
        /// </summary>
        /// <param name="model">
        /// * Send Amount value for Charge
        /// </param>
        [HttpPost("charge-by-parent")]
        public async Task<ActionResult> ChargeByParent([FromBody] ChargeByParentViewModel model)
        {
            if (_userIdentity.Channel != ChannelType.Mobile)
                throw new BusinessException("Wrong Channel Type");

            await _transactionService.ChargeByParentAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Refund From ATMCard To Parent Wallet
        /// </summary>
        /// <param name="model">
        /// * Send Amount value for Refund
        /// </param>
        [HttpPost("refund-to-parent")]
        public async Task<ActionResult> RefundToParent([FromBody] RefundToParentViewModel model)
        {
            if (_userIdentity.Channel != ChannelType.Mobile)
                throw new BusinessException("Wrong Channel Type");

            await _transactionService.RefundToParentAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Pay By ATM Card To Vendor POS
        /// </summary>
        /// <param name="model">
        /// * Send Amount value for Pay
        /// * Must send ATMCardPassword encrypted.
        /// </param>
        [HttpPost("pay-to-vendor")]
        public async Task<ActionResult> PayToVendor([FromBody] PayToVendorViewModel model)
        {
            if (_userIdentity.Channel != ChannelType.POS)
                throw new BusinessException("Wrong Channel Type");

            await _transactionService.PayToVendorAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Refund From Vendor POS To ATMCard
        /// </summary>
        /// <param name="model">
        /// * Send Amount value for Refund
        /// * ReferenceTransactionId is POSMachineTransactionId for old transaction
        /// * Must send POSMachinePinCode encrypted.
        /// </param>
        [HttpPost("refund-from-vendor")]
        public async Task<ActionResult> RefundFromVendor([FromBody] RefundFromVendorViewModel model)
        {
            if (_userIdentity.Channel != ChannelType.POS)
                throw new BusinessException("Wrong Channel Type");

            await _transactionService.RefundFromVendorAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Get ATMCardTransactions list
        /// </summary>
        /// <remarks>* You can filter and sort and paging for the list.</remarks>
        /// <response code="200">paymentType is enum: Visa = 1, Cash = 2, Transfer = 3, Gift = 4, Withdraw = 5</response>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ATMCardTransactionFilterViewModel model)
        {
            var result = await _transactionService.GetAllAsync(model);
            return Ok(new PuzzleApiResponse(result));
        }
    }
}
