using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Planorama.User.Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;

        public JwtService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }

        public (string jwtToken, DateTime tokenExpiresAtUtc) GenerateJwtToken(Models.UserCredential userCredential, string userFullName, IEnumerable<string> roles) 
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Secret")));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512);

            List<Claim> claims = [
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, userCredential.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, userFullName),
                new Claim(JwtRegisteredClaimNames.Email, userCredential.EmailAddress),
                new Claim("scope", "planorama-api")
            ];
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var tokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationTimeInMinutes"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = configuration.GetValue<string>("Jwt:Issuer"),
                Audience = configuration.GetValue<string>("Jwt:Audience"),
                Subject = new ClaimsIdentity(claims),
                Expires = tokenExpiresAtUtc,
                SigningCredentials = credentials,
            };

            var jwtToken = new JsonWebTokenHandler().CreateToken(tokenDescriptor);

            return (jwtToken, tokenExpiresAtUtc);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public void WriteAccessAndRefreshTokensAsHttpOnlyCookie(string cookieName, string token, DateTime tokenExpiresAtUtc)
        {
            httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, token, new CookieOptions 
            {
                HttpOnly = true,
                Expires = tokenExpiresAtUtc,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
        }

        public void ExpireTokensFromHttpOnlyCookie()
        {
            foreach(var cookie in httpContextAccessor.HttpContext.Request.Cookies.Keys)
            {
                httpContextAccessor.HttpContext.Response.Cookies.Append(cookie, string.Empty, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddDays(-1),
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
            }
        }
    }
}
