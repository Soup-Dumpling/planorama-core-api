using Planorama.Notification.Core.Enums;
using System;

namespace Planorama.Notification.Core.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public DateTime? DateCreatedUtc { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; }
        public string Title { get; set; }
        public NotificationType Type { get; set; }
        public string Content { get; set; }
        public Guid ReferenceId { get; set; }
        public string DeleteId { get; set; }
    }
}
