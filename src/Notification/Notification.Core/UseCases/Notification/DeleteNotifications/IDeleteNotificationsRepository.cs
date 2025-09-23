using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planorama.Notification.Core.UseCases.Notification.DeleteNotifications
{
    public interface IDeleteNotificationsRepository
    {
        Task<IEnumerable<Models.Notification>> DeleteNotificationsAsync(Guid? userId, string deleteId);
    }
}
