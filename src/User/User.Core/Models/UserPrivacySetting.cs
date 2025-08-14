using System;

namespace Planorama.User.Core.Models
{
    public class UserPrivacySetting
    {
        public Guid UserId { get; set; }
        public bool IsPrivate { get; set; }
        public virtual User User { get; set; }
    }
}
