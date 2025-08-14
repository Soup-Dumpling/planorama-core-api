using System;
using System.Collections.Generic;

namespace Planorama.User.Core.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserCredential UserCredential { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public UserPrivacySetting UserPrivacySetting { get; set; }
    }
}
