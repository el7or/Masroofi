using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.POSMachineTransactions;
using Puzzle.Masroofi.ServiceInterface;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.WebApiVendors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class POSMachineTransactionsController : ControllerBase
    {
        private readonly IPOSMachineTransactionService _transactionService;
        private readonly UserIdentity _userIdentity;

        public POSMachineTransactionsController(IPOSMachineTransactionService transactionService,
            UserIdentity userIdentity)
        {
            _transactionService = transactionService;
            _userIdentity = userIdentity;
        }

        /// <summary>
        /// Charge POS Machine By Admin
        /// </summary>
        /// <param name="model">
        /// * Send Amount value for Charge
        /// </param>
        [HttpPost("charge-by-admin")]
        public async Task<ActionResult> ChargeByAdmin([FromBody] ChargePOSByAdminViewModel model)
        {
            if (_userIdentity.Channel != ChannelType.Admin)
                throw new BusinessException("Wrong Channel Type");

            await _transactionService.ChargeByAdminAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Pay Dues From POS Machine By Admin
        /// </summary>
        /// <param name="model">
        /// * Send Amount value for Pay Dues
        /// </param>
        [HttpPost("pay-by-admin")]
        public async Task<ActionResult> PayDuesByAdmin([FromBody] PayDuesByAdminViewModel model)
        {
            if (_userIdentity.Channel != ChannelType.Admin)
                throw new BusinessException("Wrong Channel Type");

            await _transactionService.PayDuesByAdminAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Get POSMachineTransactions list
        /// </summary>
        /// <remarks>* You can filter and sort and paging for the list.</remarks>
        /// <response code="200">paymentType is enum: Visa = 1, Cash = 2, Transfer = 3, Gift = 4, Withdraw = 5</response>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] POSMachineTransactionFilterViewModel model)
        {
            var result = await _transactionService.GetAllAsync(model);
            return Ok(new PuzzleApiResponse(result));
        }
    }
}
