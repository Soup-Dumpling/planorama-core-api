using System;
using System.Collections.Generic;

namespace Planorama.User.Core.Models
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string IdentityName { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
