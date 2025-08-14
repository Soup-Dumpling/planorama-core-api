using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planorama.User.Core.UseCases.Authentication.RefreshTokens
{
    public interface IRefreshTokensRepository
    {
        Task<Models.UserCredential> FindUserCredentialByRefreshTokenAsync(string refreshToken);
        Task<string> GetUserFullNameByIdAsync(Guid userId);
        Task<IEnumerable<string>> GetUserRolesByIdAsync(Guid userId);
        Task<TokensUpdatedEvent> UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime refreshTokenExpiresAtUtc, string username);
    }
}
