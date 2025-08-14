using System;

namespace Planorama.User.Core.Models
{
    public class UserCredential
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string EmailAddress { get; set; }
        public string HashedPassword { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAtUtc { get; set; }
    }
}
