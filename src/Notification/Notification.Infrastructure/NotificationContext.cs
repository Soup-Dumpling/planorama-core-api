using Microsoft.EntityFrameworkCore;
using Models = Planorama.Notification.Core.Models;
using Planorama.Notification.Infrastructure.Mappings;

namespace Planorama.Notification.Infrastructure
{
    public class NotificationContext : DbContext
    {
        public NotificationContext(DbContextOptions<NotificationContext> contextOptions) : base(contextOptions) { }
        public DbSet<Models.Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new NotificationMapping());
        }
    }
}
