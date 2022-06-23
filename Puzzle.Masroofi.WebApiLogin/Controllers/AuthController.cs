using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.ServiceInterface;
using System;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.WebApiLogin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _UserService;
        private readonly IConfiguration _configuration;

        public AuthController(ITokenService tokenService,
            IUserService UserService, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _UserService = UserService;
            _configuration = configuration;
        }

        /// <response code="200">userType is enum: AdminUser = 1, Parent = 2, Vendor = 3, Son = 4</response>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateRequest model)
        {
            var response = _tokenService.Authenticate(model.Username, model.Password, ipAddress());

            if (response == null)
            {
                return Ok(new PuzzleApiResponse(message: "Username or password is incorrect!", 401));
            }

            var accessDetails = _UserService.GetUserAccessInformation(response.Id);
            response.AccessDetails = accessDetails;

            setTokenCookie(response.RefreshToken);

            return Ok(new PuzzleApiResponse(result: response));
        }

        /// <param name="model">Send RefreshToken not AccessToken.</param>
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequest model)
        {
            var refreshToken = model.RefreshToken; //Request.Cookies["refreshToken"];
            var response = await _tokenService.RefreshTokenAsync(refreshToken, ipAddress());

            if (response == null)
                return Ok(new PuzzleApiResponse(message: "Invalid refreshToken!", 401));

            setTokenCookie(response.RefreshToken);

            return Ok(new PuzzleApiResponse(result: response));
        }

        /// <param name="model">Send RefreshToken not AccessToken.</param>
        [HttpPost("revoke-token")]
        public IActionResult RevokeToken([FromBody] RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.RefreshToken ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return Ok(new PuzzleApiResponse(message: "refreshToken is required!", 400));

            var response = _tokenService.RevokeToken(token, ipAddress());

            if (!response)
                return Ok(new PuzzleApiResponse(message: "Invalid refreshToken!", 401));

            return Ok(new PuzzleApiResponse(message: "Token revoked"));
        }

        [AllowAnonymous]
        [HttpGet("encrypt-data")]
        public IActionResult EncryptData(string dataToEncrypt)
        {
            var encryptionKey = _configuration.GetSection("Security:EncryptionKey").Value;
            var result = Encryption.EncryptData(dataToEncrypt, encryptionKey);

            return Ok(new PuzzleApiResponse(result:result));
        }

        [AllowAnonymous]
        [HttpGet("decrypt-data")]
        public IActionResult DecryptData(string dataToDecrypt)
        {
            var encryptionKey = _configuration.GetSection("Security:EncryptionKey").Value;
            var result = Encryption.DecryptData(dataToDecrypt, encryptionKey);

            return Ok(new PuzzleApiResponse(result: result));
        }

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
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
