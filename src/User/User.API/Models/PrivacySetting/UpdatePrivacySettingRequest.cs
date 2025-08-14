using System;

namespace Planorama.User.API.Models.PrivacySetting
{
    public class UpdatePrivacySettingRequest
    {
        public Guid UserId { get; set; }
        public bool IsPrivate { get; set; }
    }
}
