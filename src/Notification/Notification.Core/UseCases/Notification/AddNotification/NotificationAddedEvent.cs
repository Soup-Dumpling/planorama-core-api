using Planorama.Integration.MessageBroker.Core.Events;
using Planorama.Notification.Core.Enums;
using System;

namespace Planorama.Notification.Core.UseCases.Notification.AddNotification
{
    public record NotificationAddedEvent(Guid NotificationId, DateTime? DateCreatedUtc, Guid UserId, string UserEmail, string Title, NotificationType Type, string Content, Guid ReferenceId, string DeleteId) : ServiceBusEvent(NotificationId);
}
