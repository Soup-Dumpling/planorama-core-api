using Microsoft.EntityFrameworkCore;
using Planorama.Notification.Core.UseCases.Notification.DeleteNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planorama.Notification.Infrastructure.Repository.Notification
{
    public class DeleteNotificationsRepository : IDeleteNotificationsRepository
    {
        private readonly NotificationContext context;

        public DeleteNotificationsRepository(NotificationContext context) 
        {
            this.context = context;
        }

        public async Task<IEnumerable<Core.Models.Notification>> DeleteNotificationsAsync(Guid? userId, string deleteId)
        {
            var notifications = await context.Notifications.Where(x => (!userId.HasValue || x.UserId == userId) && x.DeleteId == deleteId).ToListAsync();
            context.Notifications.RemoveRange(notifications);
            await context.SaveChangesAsync();
            return notifications;
        }
    }
}
