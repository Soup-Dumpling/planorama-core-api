using Microsoft.EntityFrameworkCore;
using Planorama.Notification.Core.UseCases.Notification.GetNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planorama.Notification.Infrastructure.Repository.Notification
{
    public class GetNotificationsRepository : IGetNotificationsRepository
    {
        private readonly NotificationContext context;

        public GetNotificationsRepository(NotificationContext context) 
        {
            this.context = context;
        }

        public async Task<IEnumerable<GetNotificationsViewModel>> GetNotificationsByEmailAsync(string email)
        {
            var result = await context.Notifications.Where(x => x.UserEmail == email)
                .OrderByDescending(x => x.DateCreatedUtc)
                .Select(x => new GetNotificationsViewModel() { Id = x.Id, UserId = x.UserId, Title = x.Title, Type = x.Type, Content = x.Content, ReferenceId = x.ReferenceId })
                .ToListAsync();
            return result;
        }
    }
}
