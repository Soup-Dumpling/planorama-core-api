using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Planorama.Notification.Core.Enums;
using Models = Planorama.Notification.Core.Models;

namespace Planorama.Notification.Infrastructure.Mappings
{
    public class NotificationMapping : IEntityTypeConfiguration<Models.Notification>
    {
        public void Configure(EntityTypeBuilder<Models.Notification> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Type).HasConversion(new EnumToStringConverter<NotificationType>());
        }
    }
}
