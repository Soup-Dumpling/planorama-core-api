using Planorama.Notification.Core.Enums;
using System;

namespace Planorama.Notification.Core.UseCases.Notification.GetNotifications
{
    public class GetNotificationsViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public NotificationType Type { get; set; }
        public string Content { get; set; }
        public Guid ReferenceId { get; set; }
    }
}
