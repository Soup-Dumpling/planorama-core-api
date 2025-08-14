using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models = Planorama.User.Core.Models;

namespace Planorama.User.Infrastructure.Mappings
{
    public class UserMapping : IEntityTypeConfiguration<Models.User>
    {
        public void Configure(EntityTypeBuilder<Models.User> builder) 
        {
            builder.HasKey(x => x.Id);
        }
    }
}
