using System;
using System.Collections.Generic;

namespace Planorama.User.Core.Services
{
    public interface IJwtService
    {
        (string jwtToken, DateTime tokenExpiresAtUtc) GenerateJwtToken(Models.UserCredential userCredential, string userFullName, IEnumerable<string> roles);
        string GenerateRefreshToken();
        void WriteAccessAndRefreshTokensAsHttpOnlyCookie(string cookieName, string token, DateTime tokenExpiresAtUtc);
        void ExpireTokensFromHttpOnlyCookie();
    }
}
