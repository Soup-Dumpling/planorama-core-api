using NSubstitute;
using Planorama.Integration.MessageBroker.Core.Abstraction;
using Planorama.Notification.Core.Enums;
using Planorama.Notification.Core.UseCases.Notification.AddNotification;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.Notification.Core.UnitTests.UseCases.Notification
{
    public class AddNotificationCommandHandlerUnitTests
    {
        private readonly AddNotificationCommandHandler addNotificationCommandHandler;
        private readonly IAddNotificationRepository addNotificationRepositoryMock = Substitute.For<IAddNotificationRepository>();
        private readonly IEventBus eventBusMock = Substitute.For<IEventBus>();

        public AddNotificationCommandHandlerUnitTests()
        {
            addNotificationCommandHandler = new AddNotificationCommandHandler(addNotificationRepositoryMock, eventBusMock);
        }

        [Fact]
        public async Task ValidCommand()
        {
            //Arrange
            var command = new AddNotificationCommand(Guid.NewGuid(), "userEmail", "title", NotificationType.SquadInvitation, "content", Guid.NewGuid(), "deleteId");
            var notificationAddedEvent = new NotificationAddedEvent(Guid.NewGuid(), DateTime.UtcNow, command.UserId, command.UserEmail, command.Title, command.Type, command.Content, command.ReferenceId, command.DeleteId);
            addNotificationRepositoryMock.AddNotificationAsync(Arg.Any<Models.Notification>()).Returns(Task.FromResult(notificationAddedEvent));

            //Act
            await addNotificationCommandHandler.Handle(command, CancellationToken.None);

            //Assert
            await addNotificationRepositoryMock.Received().AddNotificationAsync(Arg.Is<Models.Notification>(x =>
            x.UserId == command.UserId &&
            x.UserEmail == command.UserEmail &&
            x.Title == command.Title &&
            x.Type == command.Type &&
            x.Content == command.Content &&
            x.ReferenceId == command.ReferenceId &&
            x.DeleteId == command.DeleteId));

            await eventBusMock.Received().PublishAsync(Arg.Is<NotificationAddedEvent>(x =>
            x.NotificationId == notificationAddedEvent.Id &&
            x.DateCreatedUtc == notificationAddedEvent.DateCreatedUtc &&
            x.UserId == notificationAddedEvent.UserId &&
            x.UserEmail == notificationAddedEvent.UserEmail &&
            x.Title == notificationAddedEvent.Title &&
            x.Type == notificationAddedEvent.Type &&
            x.Content == notificationAddedEvent.Content &&
            x.ReferenceId == notificationAddedEvent.ReferenceId &&
            x.DeleteId == notificationAddedEvent.DeleteId));
        }
    }
}
