using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Resources;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.POSMachines;
using Puzzle.Masroofi.ServiceInterface;
using System;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.WebApiVendors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class POSMachinesController : ControllerBase
    {
        private readonly IPOSMachineService _posMachineService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly UserIdentity _userIdentity;

        public POSMachinesController(IPOSMachineService posMachineService,
            ITokenService tokenService,
            UserIdentity userIdentity,
            IConfiguration configuration)
        {
            _posMachineService = posMachineService;
            _tokenService = tokenService;
            _userIdentity = userIdentity;
            _configuration = configuration;
        }

        /// <summary>
        /// Get POSMachines list
        /// </summary>
        /// <remarks>* You can filter and sort and paging for the list.</remarks>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] POSMachineFilterViewModel model)
        {
            var result = await _posMachineService.GetAllAsync(model);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Get POSMachine By POSMachineId
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _posMachineService.GetAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Add new POSMachine for Vendor
        /// </summary>
        /// <param name="model">
        /// * Must send PinCode encrypted for 4 digits.
        /// </param>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] POSMachineInputViewModel model)
        {
            await _posMachineService.AddAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Update POSMachine
        /// </summary>
        /// <param name="model">
        /// * Must send PinCode encrypted for 4 digits.
        /// </param>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] POSMachineInputViewModel model)
        {
            await _posMachineService.UpdateAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }

        /// <summary>
        /// Delete POSMachine
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _posMachineService.DeleteAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Login posMachine
        /// </summary>
        /// <param name="model">
        /// * Must send PinCode encrypted for 4 digits.
        /// </param>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] POSMachineLoginViewModel model)
        {
            var posMachine = await _posMachineService.LoginAsync(model);

            // Generate token
            var token = _tokenService.Authenticate(posMachine.POSMachineId, AuthUserType.POSMachine, ipAddress());
            var result = new POSMachineAuthenticateViewModel
            {
                POSMachineId = posMachine.POSMachineId,
                POSMachineInfo = posMachine,
                Token = token,
                CurrencySymbol = _userIdentity.Language == Language.ar ? _configuration.GetSection("CurrencySymbol:ar").Value : _configuration.GetSection("CurrencySymbol:en").Value,
                MinorCurrencySymbol = _userIdentity.Language == Language.ar ? _configuration.GetSection("MinorCurrencySymbol:ar").Value : _configuration.GetSection("MinorCurrencySymbol:en").Value
            };

            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Generate new PinCode and send it as SMS to Vendor Phone
        /// </summary>
        /// <param name="username">
        /// * Send username to generate new PinCode
        /// </param>
        [AllowAnonymous]
        [HttpGet("forget-pinCode/{username}")]
        public async Task<ActionResult> ForgetPinCode(string username)
        {
            var result = await _posMachineService.ForgetPinCodeAsync(username);
            return Ok(new PuzzleApiResponse(message: result));
        }

        /// <summary>
        /// Update POSMachine PinCode
        /// </summary>
        /// <param name="model">
        /// * Must send PinCode and OldPinCode encrypted.
        /// </param>
        [HttpPut("update-pinCode")]
        public async Task<IActionResult> UpdatePinCode([FromBody] POSMachinePinCodeViewModel model)
        {
            var result = await _posMachineService.UpdatePinCodeAsync(model);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Get Vendor balance by POSMachineId 
        /// </summary>
        [HttpGet("balance/{id}")]
        public async Task<IActionResult> GetBalance(Guid id)
        {
            var result = await _posMachineService.GetBalanceAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
