using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Infrastructure;
using Puzzle.Masroofi.Infrastructure.Repositories;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Puzzle.Masroofi.Core.Helpers;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface ITokenService
    {
        AuthenticateResponse Authenticate(string username, string password, string ipAddress);
        AuthenticateResponse Authenticate(Guid userId, AuthUserType userType, string ipAddress);
        Task<AuthenticateResponse> RefreshTokenAsync(string token, string ipAddress);
        bool RevokeToken(string token, string ipAddress);
    }

    public class TokenService : BaseService, ITokenService
    {
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private readonly IUserService userService;
        private readonly IConfiguration configuration;

        public TokenService(IUnitOfWork unitOfWork, IMapper mapper,
                IRefreshTokenRepository refreshTokenRepository,
                IUserService userService,
                IConfiguration configuration) : base(unitOfWork, mapper)
        {
            this.refreshTokenRepository = refreshTokenRepository;
            this.userService = userService;
            this.configuration = configuration;
        }

        public AuthenticateResponse Authenticate(string username, string password, string ipAddress)
        {
            var user = userService.GetUser(username, password);

            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt and refresh tokens
            return GenerateAccess(user.UserId, AuthUserType.AdminUser, ipAddress);

        }

        public AuthenticateResponse Authenticate(Guid userId, AuthUserType userType, string ipAddress)
        {
            return GenerateAccess(userId, userType, ipAddress);
        }

        private AuthenticateResponse GenerateAccess(Guid userId, AuthUserType userType, string ipAddress)
        {
            long tokenExpiresIn = 0;
            var jwtToken = GenerateJwtToken(userId, out tokenExpiresIn);
            var refreshToken = GenerateRefreshToken(ipAddress);

            // save refresh token
            refreshToken.UserId = userId;
            refreshToken.UserType = (int)userType;
            refreshTokenRepository.Add(refreshToken);
            unitOfWork.Commit();

            long refreshTokenExpiresIn = refreshToken.Expires.ToUnixTime();
            long serverTimeMs = DateTime.UtcNow.ToUnixTime();
            return new AuthenticateResponse(userId, userType, jwtToken, tokenExpiresIn, refreshToken.Token, refreshTokenExpiresIn, serverTimeMs);
        }

        public async Task<AuthenticateResponse> RefreshTokenAsync(string token, string ipAddress)
        {
            var refreshToken = refreshTokenRepository.GetWhere(x => x.Token == token);

            if (refreshToken == null)
                return null;

            // return null if token is no longer active
            if (refreshToken.IsActive.HasValue && !refreshToken.IsActive.Value) return null;

            // replace old refresh token with a new one and save
            var newRefreshToken = GenerateRefreshToken(ipAddress);

            //refreshToken.Revoked = DateTime.UtcNow;
            //refreshToken.RevokedByIp = ipAddress;
            //refreshToken.ReplacedByToken = newRefreshToken.Token;
            refreshToken.Expires = newRefreshToken.Expires;
            refreshToken.Created = newRefreshToken.Created;
            refreshToken.CreatedByIp = newRefreshToken.CreatedByIp;
            refreshToken.Token = newRefreshToken.Token;
            refreshTokenRepository.Update(refreshToken);
            unitOfWork.Commit();

            var user = await userService.GetUserByIdAsync(refreshToken.UserId);

            // generate new jwt
            var jwtToken = GenerateJwtToken(refreshToken.UserId, out long tokenExpiresIn);

            long refreshTokenExpiresIn = newRefreshToken.Expires.ToUnixTime();
            long serverTimeMs = DateTime.UtcNow.ToUnixTime();

            return new AuthenticateResponse(refreshToken.UserId, (AuthUserType)refreshToken.UserType, jwtToken, tokenExpiresIn, newRefreshToken.Token, refreshTokenExpiresIn, serverTimeMs);
        }

        public bool RevokeToken(string token, string ipAddress)
        {
            //var user = refreshTokenRepository.Get(t => t.Token == token);

            //// return false if no user found with token
            //if (user == null) return false;

            var refreshToken = refreshTokenRepository.GetWhere(x => x.Token == token);

            if (refreshToken == null)
                return false;

            // return false if token is not active
            if (refreshToken.IsActive.HasValue && !refreshToken.IsActive.Value) return false;

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.IsActive = false;
            refreshTokenRepository.Update(refreshToken);
            unitOfWork.Commit();

            return true;
        }

        private string GenerateJwtToken(Guid userId, out long tokenExpiresIn)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = configuration.GetSection("Security:JWTKey").Value;
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var tokenExpiresAfterMinutes = configuration.GetSection("Security:tokenExpiresAfterMinutes").Value;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                                        new Claim(ClaimTypes.Name, userId.ToString()),
                    }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(tokenExpiresAfterMinutes)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            tokenExpiresIn = tokenDescriptor.Expires.Value.ToUnixTime();

            return tokenHandler.WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            var refreshTokenExpiresAfterDays = configuration.GetSection("Security:refreshTokenExpiresAfterDays").Value;
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(int.Parse(refreshTokenExpiresAfterDays)),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress,
                    IsActive = true,
                    IsExpired = false
                };
            }
        }
    }
}
