using Microsoft.EntityFrameworkCore;
using Models = Planorama.User.Core.Models;
using Planorama.User.Core.UseCases.PrivacySetting.GetPrivacySetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Planorama.User.Core.Models;

namespace Planorama.User.Infrastructure.Repository.PrivacySetting
{
    public class GetPrivacySettingRepository : IGetPrivacySettingRepository
    {
        private readonly UserDBContext context;

        public GetPrivacySettingRepository(UserDBContext context) 
        {
            this.context = context;
        }

        public Task<Guid?> GetUserIdByEmailAsync(string email)
        {
            return context.UserCredentials.Where(x => x.EmailAddress == email).Select(x => (Guid?)x.UserId).FirstOrDefaultAsync();
        }

        public async Task<GetPrivacySettingViewModel> GetUserPrivacySettingByIdAsync(Guid userId)
        {
            var result = await context.UserPrivacySettings.Where(x => x.UserId == userId).Select(x => new GetPrivacySettingViewModel()
            {
                UserId = x.UserId,
                IsPrivate = x.IsPrivate
            }).FirstOrDefaultAsync();
            return result;
        }
    }
}
