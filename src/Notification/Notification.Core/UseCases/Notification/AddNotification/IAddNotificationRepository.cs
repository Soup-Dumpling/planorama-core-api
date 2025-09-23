using System.Threading.Tasks;

namespace Planorama.Notification.Core.UseCases.Notification.AddNotification
{
    public interface IAddNotificationRepository
    {
        Task<NotificationAddedEvent> AddNotificationAsync(Models.Notification notification);
    }
}
