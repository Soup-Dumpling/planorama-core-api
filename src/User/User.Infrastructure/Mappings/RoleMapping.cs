using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planorama.User.Core.Models;
using System;

namespace Planorama.User.Infrastructure.Mappings
{
    public class RoleMapping : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder) 
        {
            builder.HasKey(x => x.Id);
            builder.HasData(
                new Role { Id = Guid.Parse("bb1b340c-4437-492e-8703-01815179e510"), Name = "User", IdentityName = "planorama.user" },
                new Role { Id = Guid.Parse("17f6719f-4af1-4d2e-a7db-0a6e0750c149"), Name = "Member", IdentityName = "planorama.member" },
                new Role { Id = Guid.Parse("3054c7dd-b9b0-406d-86bc-345155f6b51f"), Name = "Moderator", IdentityName = "planorama.moderator" },
                new Role { Id = Guid.Parse("5f0e7491-7a95-418a-9545-5adc7a079037"), Name = "Admin", IdentityName = "planorama.admin" }
            );
        }
    }
}
