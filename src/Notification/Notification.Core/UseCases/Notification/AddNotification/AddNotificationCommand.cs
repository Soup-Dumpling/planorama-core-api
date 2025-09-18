using MediatR;
using Planorama.Notification.Core.Enums;
using System;

namespace Planorama.Notification.Core.UseCases.Notification.AddNotification
{
    public class AddNotificationCommand : IRequest
    {
        public Guid UserId { get; set; }
        public string UserEmail { get; set; }
        public string Title { get; set; }
        public NotificationType Type { get; set; }
        public string Content { get; set; }
        public Guid ReferenceId { get; set; }
        public string DeleteId { get; set; }

        public AddNotificationCommand(Guid userId, string userEmail, string title, NotificationType type, string content, Guid referenceId, string deleteId)
        {
            UserId = userId;
            UserEmail = userEmail;
            Title = title;
            Type = type;
            Content = content;
            ReferenceId = referenceId;
            DeleteId = deleteId;
        }
    }
}
