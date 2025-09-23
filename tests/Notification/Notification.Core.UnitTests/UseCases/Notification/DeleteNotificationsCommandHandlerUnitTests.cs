using NSubstitute;
using Planorama.Integration.MessageBroker.Core.Abstraction;
using Planorama.Notification.Core.UseCases.Notification.DeleteNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.Notification.Core.UnitTests.UseCases.Notification
{
    public class DeleteNotificationsCommandHandlerUnitTests
    {
        private readonly DeleteNotificationsCommandHandler deleteNotificationsCommandHandler;
        private readonly IDeleteNotificationsRepository deleteNotificationsRepositoryMock = Substitute.For<IDeleteNotificationsRepository>();
        private readonly IEventBus eventBusMock = Substitute.For<IEventBus>();

        public DeleteNotificationsCommandHandlerUnitTests() 
        {
            deleteNotificationsCommandHandler = new DeleteNotificationsCommandHandler(deleteNotificationsRepositoryMock, eventBusMock);
        }

        [Fact]
        public async Task ValidCommand()
        {
            //Arrange
            var command = new DeleteNotificationsCommand(Guid.NewGuid(), "deleteId");
            var deletedNotifications = new List<Models.Notification>() { new Models.Notification() { Id = Guid.NewGuid(), DateCreatedUtc = DateTime.UtcNow.AddDays(-1), UserId = (Guid)command.UserId, UserEmail = "userEmail", Title = "title", Type = Enums.NotificationType.SquadInvitation, Content = "content", ReferenceId = Guid.NewGuid(), DeleteId = command.DeleteId } };
            deleteNotificationsRepositoryMock.DeleteNotificationsAsync(Arg.Any<Guid?>(), Arg.Any<string>()).Returns(Task.FromResult(deletedNotifications.AsEnumerable()));

            //Act
            await deleteNotificationsCommandHandler.Handle(command, CancellationToken.None);

            //Assert
            await deleteNotificationsRepositoryMock.Received().DeleteNotificationsAsync(command.UserId, command.DeleteId);
            await eventBusMock.Received(1).PublishAsync(Arg.Is<NotificationDeletedEvent>(x => 
            x.Id == deletedNotifications[0].Id &&
            x.NotificationId == deletedNotifications[0].Id &&
            x.UserId == deletedNotifications[0].UserId &&
            x.UserEmail == deletedNotifications[0].UserEmail &&
            x.ReferenceId == deletedNotifications[0].ReferenceId &&
            x.DeleteId == deletedNotifications[0].DeleteId));
        }
    }
}
