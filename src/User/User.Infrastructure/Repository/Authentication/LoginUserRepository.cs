using Microsoft.EntityFrameworkCore;
using Models = Planorama.User.Core.Models;
using Planorama.User.Core.UseCases.Authentication.LoginUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planorama.User.Infrastructure.Repository.Authentication
{
    public class LoginUserRepository : ILoginUserRepository
    {
        private readonly UserDBContext context;

        public LoginUserRepository(UserDBContext context) 
        {
            this.context = context;
        }

        public Task<Models.UserCredential> FindUserCredentialByEmailAsync(string email) 
        {
            return context.UserCredentials.AsNoTracking().FirstOrDefaultAsync(x => x.EmailAddress == email);
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

        public async Task<UserLoggedInEvent> AddRefreshTokenAsync(Guid userId, string refreshToken, DateTime refreshTokenExpiresAtUtc, string username)
        {
            var userCredential = await context.UserCredentials.FindAsync(userId);
            userCredential.RefreshToken = refreshToken;
            userCredential.RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc;
            context.UserCredentials.Update(userCredential);
            var userLoggedInEvent = new UserLoggedInEvent(userCredential.UserId, refreshToken, refreshTokenExpiresAtUtc);
            await context.IntegrationEvents.AddAsync(new Models.IntegrationEvent<UserLoggedInEvent>(userId.ToString(), userLoggedInEvent, username));
            await context.SaveChangesAsync();
            return userLoggedInEvent;
        }
    }
}
