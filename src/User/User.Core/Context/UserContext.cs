using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Planorama.User.Core.Context
{
    public class UserContext : IUserContext
    {
        private readonly ClaimsPrincipal claimsPrincipal;
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            this.claimsPrincipal = httpContextAccessor.HttpContext.User;
            AccessToken = httpContextAccessor.HttpContext.Request.Cookies["ACCESS_TOKEN"];
            RefreshToken = httpContextAccessor.HttpContext.Request.Cookies["REFRESH_TOKEN"];
        }

        public string UserName
        {
            get
            {
                return claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            }
        }

        public bool IsLoggedIn()
        {
            return claimsPrincipal != null;
        }

        public bool IsInRole(string roleName)
        {
            return claimsPrincipal.IsInRole(roleName);
        }
    }
}
