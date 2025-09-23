using MediatR;
using Planorama.Integration.MessageBroker.Core.Abstraction;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planorama.Notification.Core.UseCases.Notification.AddNotification
{
    public class AddNotificationCommandHandler : IRequestHandler<AddNotificationCommand>
    {
        private readonly IAddNotificationRepository addNotificationRepository;
        private readonly IEventBus eventBus;

        public AddNotificationCommandHandler(IAddNotificationRepository addNotificationRepository, IEventBus eventBus)
        {
            this.addNotificationRepository = addNotificationRepository;
            this.eventBus = eventBus;
        }

        public async Task Handle(AddNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = new Models.Notification()
            {
                Id = Guid.NewGuid(),
                DateCreatedUtc = DateTime.UtcNow,
                UserId = request.UserId,
                UserEmail = request.UserEmail,
                Title = request.Title,
                Type = request.Type,
                Content = request.Content,
                ReferenceId = request.ReferenceId,
                DeleteId = request.DeleteId,
            };

            var result = await addNotificationRepository.AddNotificationAsync(notification);
            await eventBus.PublishAsync(result);
            return;
        }
    }
}
