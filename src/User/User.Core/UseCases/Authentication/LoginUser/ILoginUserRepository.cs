using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planorama.User.Core.UseCases.Authentication.LoginUser
{
    public interface ILoginUserRepository
    {
        Task<Models.UserCredential> FindUserCredentialByEmailAsync(string email);
        Task<string> GetUserFullNameByIdAsync(Guid userId);
        Task<IEnumerable<string>> GetUserRolesByIdAsync(Guid userId);
        Task<UserLoggedInEvent> AddRefreshTokenAsync(Guid userId, string refreshToken, DateTime refreshTokenExpiresAtUtc, string username);
    }
}
