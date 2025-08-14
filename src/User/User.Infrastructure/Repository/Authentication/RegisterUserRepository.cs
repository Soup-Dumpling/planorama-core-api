using Microsoft.EntityFrameworkCore;
using Models = Planorama.User.Core.Models;
using Planorama.User.Core.UseCases.Authentication.RegisterUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planorama.User.Infrastructure.Repository.Authentication
{
    public class RegisterUserRepository : IRegisterUserRepository
    {
        private readonly UserDBContext context;

        public RegisterUserRepository(UserDBContext context) 
        {
            this.context = context;
        }

        public Task<bool> CheckIfUserExistsAsync(string emailAddress)
        {
            return context.UserCredentials.AnyAsync(x => x.EmailAddress == emailAddress);
        }

        public async Task<UserRegisteredEvent> RegisterUserAsync(Models.User user, Models.UserCredential userCredential, Models.UserPrivacySetting userPrivacySetting, IEnumerable<string> roles, string username)
        {
            await context.Users.AddAsync(user);
            await context.UserCredentials.AddAsync(userCredential);
            await context.UserPrivacySettings.AddAsync(userPrivacySetting);
            var userRoles = await context.Roles.Where(x => roles.Contains(x.IdentityName)).ToListAsync();
            await context.UserRoles.AddRangeAsync(userRoles.Select(x => new Models.UserRole { UserId = user.Id, RoleId = x.Id }));
            var userRegisteredEvent = new UserRegisteredEvent(user.Id, user.FirstName, user.LastName);
            await context.IntegrationEvents.AddAsync(new Models.IntegrationEvent<UserRegisteredEvent>(user.Id.ToString(), userRegisteredEvent, username));
            await context.SaveChangesAsync();
            return userRegisteredEvent;
        }
    }
}
