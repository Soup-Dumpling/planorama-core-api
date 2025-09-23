using MediatR;
using Planorama.Integration.MessageBroker.Core.Abstraction;
using System.Threading;
using System.Threading.Tasks;

namespace Planorama.Notification.Core.UseCases.Notification.DeleteNotifications
{
    public class DeleteNotificationsCommandHandler : IRequestHandler<DeleteNotificationsCommand>
    {
        private readonly IDeleteNotificationsRepository deleteNotificationsRepository;
        private readonly IEventBus eventBus;

        public DeleteNotificationsCommandHandler(IDeleteNotificationsRepository deleteNotificationsRepository, IEventBus eventBus)
        {
            this.deleteNotificationsRepository = deleteNotificationsRepository;
            this.eventBus = eventBus;
        }

        public async Task Handle(DeleteNotificationsCommand request, CancellationToken cancellationToken)
        {
            var deletedNotifications = await deleteNotificationsRepository.DeleteNotificationsAsync(request.UserId, request.DeleteId);
            foreach (var deletedNotification in deletedNotifications) 
            {
                await eventBus.PublishAsync(new NotificationDeletedEvent(deletedNotification.Id, deletedNotification.UserId, deletedNotification.UserEmail, deletedNotification.ReferenceId, deletedNotification.DeleteId));
            }
            return;
        }
    }
}
