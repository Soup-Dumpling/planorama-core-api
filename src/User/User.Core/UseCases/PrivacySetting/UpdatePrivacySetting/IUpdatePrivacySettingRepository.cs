using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planorama.User.Core.UseCases.PrivacySetting.UpdatePrivacySetting
{
    public interface IUpdatePrivacySettingRepository
    {
        Task<bool> CheckIfUserExists(Guid userId);
        Task<Guid?> GetUserIdByEmailAsync(string email);
        Task<PrivacySettingUpdatedEvent> UpdatePrivacySettingAsync(Guid userId, bool isPrivate, string username);
    }
}
