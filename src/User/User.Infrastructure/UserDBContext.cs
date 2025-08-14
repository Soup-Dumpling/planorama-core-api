using Microsoft.EntityFrameworkCore;
using Models = Planorama.User.Core.Models;
using Planorama.User.Infrastructure.Mappings;

namespace Planorama.User.Infrastructure
{
    public class UserDBContext : DbContext
    {
        public UserDBContext(DbContextOptions<UserDBContext> contextOptions) : base(contextOptions) { }
        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.UserCredential> UserCredentials { get; set; }
        public DbSet<Models.Role> Roles { get; set; }
        public DbSet<Models.UserRole> UserRoles { get; set; }
        public DbSet<Models.UserPrivacySetting> UserPrivacySettings { get; set; }
        public DbSet<Models.IntegrationEvent> IntegrationEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserMapping());
            modelBuilder.ApplyConfiguration(new UserCredentialMapping());
            modelBuilder.ApplyConfiguration(new RoleMapping());
            modelBuilder.ApplyConfiguration(new UserRoleMapping());
            modelBuilder.ApplyConfiguration(new UserPrivacySettingMapping());
        }
    }
}
