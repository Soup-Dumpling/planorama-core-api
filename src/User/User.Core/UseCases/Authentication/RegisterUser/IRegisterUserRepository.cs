using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planorama.User.Core.UseCases.Authentication.RegisterUser
{
    public interface IRegisterUserRepository
    {
        Task<bool> CheckIfUserExistsAsync(string emailAddress);
        Task<UserRegisteredEvent> RegisterUserAsync(Models.User registeredUser, Models.UserCredential userCredential, Models.UserPrivacySetting userPrivacySetting, IEnumerable<string> roles, string username);
    }
}
