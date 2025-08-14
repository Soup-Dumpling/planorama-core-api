using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planorama.User.Core.UseCases.Authentication.LogoutUser
{
    public interface ILogoutUserRepository
    {
        Task<bool> CheckIfUserCredentialExistsAsync(Guid userId);
        Task<UserLoggedOutEvent> ReplaceUserCredentialAsync(Guid userId, string username);
    }
}
