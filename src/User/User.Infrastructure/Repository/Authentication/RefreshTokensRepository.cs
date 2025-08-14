using Microsoft.EntityFrameworkCore;
using Models = Planorama.User.Core.Models;
using Planorama.User.Core.UseCases.Authentication.RefreshTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planorama.User.Infrastructure.Repository.Authentication
{
    public class RefreshTokensRepository : IRefreshTokensRepository
    {
        private readonly UserDBContext context;

        public RefreshTokensRepository(UserDBContext context) 
        {
            this.context = context;
        }

        public Task<Models.UserCredential> FindUserCredentialByRefreshTokenAsync(string refreshToken)
        {
            return context.UserCredentials.AsNoTracking().FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
        }

        public async Task<string> GetUserFullNameByIdAsync(Guid userId)
        {
            var user = await context.Users.FindAsync(userId);
            var result = $"{user.FirstName} {user.LastName}";
            return result;
        }

        public async Task<IEnumerable<string>> GetUserRolesByIdAsync(Guid userId)
        {
            var result = await context.UserRoles.Where(x => x.UserId == userId).Select(x => x.Role.IdentityName).ToListAsync();
            return result;
        }

        public async Task<TokensUpdatedEvent> UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime refreshTokenExpiresAtUtc, string username)
        {
            var userCredential = await context.UserCredentials.FindAsync(userId);
            userCredential.RefreshToken = refreshToken;
            userCredential.RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc;
            context.UserCredentials.Update(userCredential);
            var tokensUpdatedEvent = new TokensUpdatedEvent(userCredential.UserId, refreshToken, refreshTokenExpiresAtUtc);
            await context.IntegrationEvents.AddAsync(new Models.IntegrationEvent<TokensUpdatedEvent>(userId.ToString(), tokensUpdatedEvent, username));
            await context.SaveChangesAsync();
            return tokensUpdatedEvent;
        }
    }
}
