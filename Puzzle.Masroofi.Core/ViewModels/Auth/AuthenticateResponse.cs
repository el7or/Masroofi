using Newtonsoft.Json;
using Puzzle.Masroofi.Core.Enums;
using System;

namespace Puzzle.Masroofi.Core.ViewModels.Auth
{
    public class AuthenticateResponse
    {
        public Guid Id { get; set; }
        public AuthUserType UserType { get; set; }
        public string AccessToken { get; set; }
        public long AccessTokenExpiresIn { get; set; }

        //[JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }
        public long RefreshTokenExpiresIn { get; set; }
        public long ServerTimeMs { get; set; }

        public UserAuthenticationResponseModel AccessDetails { get; set; }

        public AuthenticateResponse(Guid userId,
            AuthUserType userType,
            string accessToken,
            long accessTokenExpiresIn,
            string refreshToken,
            long refreshTokenExpiresIn,
            long serverTimeMs)
        {
            Id = userId;
            UserType = userType;
            AccessToken = accessToken;
            AccessTokenExpiresIn = accessTokenExpiresIn;
            RefreshToken = refreshToken;
            RefreshTokenExpiresIn = refreshTokenExpiresIn;
            ServerTimeMs = serverTimeMs;
        }
    }
}
