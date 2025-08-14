using Microsoft.EntityFrameworkCore;
using Models = Planorama.User.Core.Models;
using Planorama.User.Core.UseCases.PrivacySetting.UpdatePrivacySetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planorama.User.Infrastructure.Repository.PrivacySetting
{
    public class UpdatePrivacySettingRepository : IUpdatePrivacySettingRepository
    {
        private readonly UserDBContext context;

        public UpdatePrivacySettingRepository(UserDBContext context) 
        {
            this.context = context;
        }

        public Task<bool> CheckIfUserExists(Guid userId)
        {
            return context.Users.AnyAsync(x => x.Id == userId);
        }

        public Task<Guid?> GetUserIdByEmailAsync(string email)
        {
            return context.UserCredentials.Where(x => x.EmailAddress == email).Select(x => (Guid?)x.UserId).FirstOrDefaultAsync();
        }

        public async Task<PrivacySettingUpdatedEvent> UpdatePrivacySettingAsync(Guid userId, bool isPrivate, string username)
        {
            var userPrivacySetting = await context.UserPrivacySettings.FindAsync(userId);
            userPrivacySetting.IsPrivate = isPrivate;
            context.UserPrivacySettings.Update(userPrivacySetting);
            var privacySettingUpdatedEvent = new PrivacySettingUpdatedEvent(userPrivacySetting.UserId, isPrivate);
            await context.IntegrationEvents.AddAsync(new Models.IntegrationEvent<PrivacySettingUpdatedEvent>(userPrivacySetting.UserId.ToString(), privacySettingUpdatedEvent, username));
            await context.SaveChangesAsync();
            return privacySettingUpdatedEvent;
        }
    }
}
