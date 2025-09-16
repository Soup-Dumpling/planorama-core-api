using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace Planorama.Notification.Core.Context
{
    public class UserContext : IUserContext
    {
        private readonly ClaimsPrincipal claimsPrincipal;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            this.claimsPrincipal = httpContextAccessor.HttpContext.User;
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
