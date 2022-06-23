using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.Parents;
using Puzzle.Masroofi.ServiceInterface;
using System;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.WebApiParents.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParentsController : ControllerBase
    {
        private readonly IParentService _parentService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly UserIdentity _userIdentity;

        public ParentsController(IParentService parentService,
            ITokenService tokenService,
            IConfiguration configuration,
            UserIdentity userIdentity)
        {
            _parentService = parentService;
            _tokenService = tokenService;
            _configuration = configuration;
            _userIdentity = userIdentity;
        }

        /// <summary>
        /// Register parent by mobile
        /// </summary>
        /// <param name="model">
        /// * Must send Phone encrypted
        /// * Must send PinCode encrypted
        /// * For adding parent image send fileBase64 and fileName in newImage property
        /// * Gender is enum: Male = 1, Female = 2
        /// </param>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] ParentInputViewModel model)
        {
            var parent = await _parentService.RegisterAsync(model);
            // Generate token
            var token = _tokenService.Authenticate(parent.ParentId, AuthUserType.Parent, ipAddress());
            var result = new ParentAuthenticateViewModel
            {
                ParentId = parent.ParentId,
                ParentInfo = parent,
                Token = token,
                IsValidateSonFace = _configuration.GetSection("IsValidateSonFace").Value.ToLower(),
                CurrencySymbol = _userIdentity.Language == Language.ar ? _configuration.GetSection("CurrencySymbol:ar").Value : _configuration.GetSection("CurrencySymbol:en").Value
            };
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Login parent by mobile
        /// </summary>
        /// <param name="model">
        /// * Must send Phone encrypted
        /// </param>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] ParentLoginViewModel model)
        {
            var parent = await _parentService.LoginAsync(model);

            // Generate token
            var token = _tokenService.Authenticate(parent.ParentId, AuthUserType.Parent, ipAddress());
            var result = new ParentAuthenticateViewModel
            {
                ParentId = parent.ParentId,
                ParentInfo = parent,
                Token = token,
                IsValidateSonFace = _configuration.GetSection("IsValidateSonFace").Value.ToLower(),
                CurrencySymbol = _userIdentity.Language == Language.ar ? _configuration.GetSection("CurrencySymbol:ar").Value : _configuration.GetSection("CurrencySymbol:en").Value
            };

            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// To add more information for registered Parent
        /// </summary>
        /// <param name="model">
        /// * Must send Phone encrypted
        /// * Must send PinCode encrypted
        /// * If will update image send fileBase64 and fileName in newImage property, if will keep old image send imageUrl property
        /// * Gender is enum: Male = 1, Female = 2
        /// </param>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ParentInputViewModel model)
        {
            await _parentService.UpdateAsync(model);
            return Ok(new PuzzleApiResponse(model));
        }
        /// <summary>
        /// Get Parent By ParentId
        /// </summary>
          [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _parentService.GetAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Delete Parent
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _parentService.DeleteAsync(id);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Activate or Deactivate Parent
        /// </summary>
        [HttpPut("change-activation")]
        public async Task<IActionResult> ChangeActivation([FromBody] ParentActivationViewModel model)
        {
            var result = await _parentService.ChangeActivationAsync(model);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Check if PinCode is exists for the Parent
        /// </summary>
        /// <param name="model">
        /// * Must send PinCode encrypted
        /// </param>
        /// <response code="200">result will be true if PinCode is exists</response>
        [HttpPost("check-pincode")]
        public ActionResult CheckPinCodeExists([FromBody] CheckPinCodeViewModel model)
        {
            var result = _parentService.CheckPinCodeExists(model);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Generate new PinCode and send it as SMS to Parent Phone
        /// </summary>
        /// <param name="id">
        /// * Send ParentId to generate new PinCode
        /// </param>
        [HttpGet("forget-pincode/{id}")]
        public async Task<ActionResult> ForgetPinCodeAsync(Guid id)
        {
            var result = await _parentService.ForgetPinCode(id);
            return Ok(new PuzzleApiResponse(message:result));
        }

        /// <summary>
        /// Update Parent PinCode
        /// </summary>
        /// <param name="model">
        /// * Must send PinCode and OldPinCode encrypted.
        /// </param>
        [HttpPut("update-pincode")]
        public async Task<ActionResult> UpdatePinCodeAsync([FromBody] UpdatePinCodeViewModel model)
        {
            var result = await _parentService.UpdatePinCode(model);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Update Parent Phone
        /// </summary>
        /// <param name="model">
        /// * Must send Phone encrypted
        /// </param>
        [HttpPut("update-phone")]
        public async Task<IActionResult> UpdatePhone([FromBody] UpdatePhoneViewModel model)
        {
            var result = await _parentService.UpdatePhone(model);
            return Ok(new PuzzleApiResponse(result));
        }

        /// <summary>
        /// Get Parent balance by ParentId 
        /// </summary>
        [HttpGet("balance/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _parentService.GetBalanceAsync(id);
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
