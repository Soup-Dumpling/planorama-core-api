using NSubstitute;
using Planorama.Notification.Core.Context;
using Planorama.Notification.Core.UseCases.Notification.GetNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.Notification.Core.UnitTests.UseCases.Notification
{
    public class GetNotificationsQueryHandlerUnitTests
    {
        private readonly GetNotificationsQueryHandler getNotificationsQueryHandler;
        private readonly IGetNotificationsRepository getNotificationsRepositoryMock = Substitute.For<IGetNotificationsRepository>();
        private readonly IUserContext userContextMock = Substitute.For<IUserContext>();

        public GetNotificationsQueryHandlerUnitTests()
        {
            getNotificationsQueryHandler = new GetNotificationsQueryHandler(getNotificationsRepositoryMock, userContextMock);
        }

        [Fact]
        public async Task ValidQuery()
        {
            //Arrange
            var query = new GetNotificationsQuery();
            var userId = Guid.NewGuid();
            userContextMock.UserName.Returns("user.testing@outlook.com");
            var notifications = new List<GetNotificationsViewModel>() { new GetNotificationsViewModel() { Id = Guid.NewGuid(), UserId = userId }, new GetNotificationsViewModel() { Id = Guid.NewGuid(), UserId = userId } };
            getNotificationsRepositoryMock.GetNotificationsByEmailAsync(Arg.Any<string>()).Returns(Task.FromResult(notifications.AsEnumerable()));

            //Act
            var result = await getNotificationsQueryHandler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(2, result.Count());
            await getNotificationsRepositoryMock.Received().GetNotificationsByEmailAsync("user.testing@outlook.com");
        }
    }
}
