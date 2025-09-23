using Planorama.Integration.MessageBroker.Core.Events;
using System;

namespace Planorama.Notification.Core.UseCases.Notification.DeleteNotifications
{
    public record NotificationDeletedEvent(Guid NotificationId, Guid UserId, string UserEmail, Guid ReferenceId, string DeleteId) : ServiceBusEvent(NotificationId);
}
