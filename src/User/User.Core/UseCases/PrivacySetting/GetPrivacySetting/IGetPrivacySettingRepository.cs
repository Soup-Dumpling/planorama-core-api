using Planorama.User.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planorama.User.Core.UseCases.PrivacySetting.GetPrivacySetting
{
    public interface IGetPrivacySettingRepository
    {
        Task<Guid?> GetUserIdByEmailAsync(string email);
        Task<GetPrivacySettingViewModel> GetUserPrivacySettingByIdAsync(Guid userId);
    }
}
