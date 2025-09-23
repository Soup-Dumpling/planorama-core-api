using Planorama.Notification.Core.UseCases.Notification.AddNotification;
using System.Threading.Tasks;

namespace Planorama.Notification.Infrastructure.Repository.Notification
{
    public class AddNotificationRepository : IAddNotificationRepository
    {
        private readonly NotificationContext context;

        public AddNotificationRepository(NotificationContext context) 
        {
            this.context = context;
        }

        public async Task<NotificationAddedEvent> AddNotificationAsync(Core.Models.Notification notification)
        {
            await context.Notifications.AddAsync(notification);
            var notificationAddedEvent = new NotificationAddedEvent(notification.Id, notification.DateCreatedUtc, notification.UserId, notification.UserEmail, notification.Title, notification.Type, notification.Content, notification.ReferenceId, notification.DeleteId);
            await context.SaveChangesAsync();
            return notificationAddedEvent;
        }
    }
}
