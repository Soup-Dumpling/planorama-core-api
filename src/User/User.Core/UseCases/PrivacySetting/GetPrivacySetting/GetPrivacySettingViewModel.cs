using System;

namespace Planorama.User.Core.UseCases.PrivacySetting.GetPrivacySetting
{
    public class GetPrivacySettingViewModel
    {
        public Guid UserId { get; set; }
        public bool IsPrivate { get; set; }
    }
}
