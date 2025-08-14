using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models = Planorama.User.Core.Models;

namespace Planorama.User.Infrastructure.Mappings
{
    public class UserPrivacySettingMapping : IEntityTypeConfiguration<Models.UserPrivacySetting>
    {
        public void Configure(EntityTypeBuilder<Models.UserPrivacySetting> builder)
        {
            builder.HasKey(x => x.UserId);
            builder.HasOne(x => x.User).WithOne(x => x.UserPrivacySetting).HasForeignKey<Models.UserPrivacySetting>(x => x.UserId).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(x => new { x.UserId, x.IsPrivate }).IsUnique();
            builder.Property(x => x.IsPrivate).HasDefaultValue(false);
        }
    }
}
