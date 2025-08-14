using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models = Planorama.User.Core.Models;

namespace Planorama.User.Infrastructure.Mappings
{
    public class UserCredentialMapping : IEntityTypeConfiguration<Models.UserCredential>
    {
        public void Configure(EntityTypeBuilder<Models.UserCredential> builder) 
        {
            builder.HasKey(x => x.UserId);
            builder.HasOne(x => x.User).WithOne(x => x.UserCredential).HasForeignKey<Models.UserCredential>(x => x.UserId).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(x => new { x.UserId, x.EmailAddress }).IsUnique();
        }
    }
}
