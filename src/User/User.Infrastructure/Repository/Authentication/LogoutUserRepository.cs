using Microsoft.EntityFrameworkCore;
using Models = Planorama.User.Core.Models;
using Planorama.User.Core.UseCases.Authentication.LogoutUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planorama.User.Infrastructure.Repository.Authentication
{
    public class LogoutUserRepository : ILogoutUserRepository
    {
        private readonly UserDBContext context;

        public LogoutUserRepository(UserDBContext context) 
        {
            this.context = context;
        }

        public Task<bool> CheckIfUserCredentialExistsAsync(Guid userId) 
        {
            return context.UserCredentials.AnyAsync(x => x.UserId == userId);
        }

        public async Task<UserLoggedOutEvent> ReplaceUserCredentialAsync(Guid userId, string username)
        {
            var userCredential = await context.UserCredentials.FindAsync(userId);
            context.UserCredentials.Remove(userCredential);
            await context.SaveChangesAsync();
            await context.UserCredentials.AddAsync(new Models.UserCredential() { UserId = userCredential.UserId, EmailAddress = userCredential.EmailAddress, HashedPassword = userCredential.HashedPassword });
            var userLoggedOutEvent = new UserLoggedOutEvent(userId);
            await context.IntegrationEvents.AddAsync(new Models.IntegrationEvent<UserLoggedOutEvent>(userId.ToString(), userLoggedOutEvent, username));
            await context.SaveChangesAsync();
            return userLoggedOutEvent;
        }
    }
}
