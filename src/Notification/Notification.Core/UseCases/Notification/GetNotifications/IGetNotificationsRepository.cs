using System.Threading.Tasks;
using System.Collections.Generic;

namespace Planorama.Notification.Core.UseCases.Notification.GetNotifications
{
    public interface IGetNotificationsRepository
    {
        Task<IEnumerable<GetNotificationsViewModel>> GetNotificationsByEmailAsync(string email);
    }
}
