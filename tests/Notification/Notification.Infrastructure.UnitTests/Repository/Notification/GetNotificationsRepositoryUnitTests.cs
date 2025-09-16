using FizzWare.NBuilder;
using Planorama.Notification.Core.Enums;
using Planorama.Notification.Infrastructure.Repository.Notification;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.Notification.Infrastructure.UnitTests.Repository.Notification
{
    public class GetNotificationsRepositoryUnitTests
    {
        private readonly NotificationContext context;
        private GetNotificationsRepository getNotificationsRepository;

        public GetNotificationsRepositoryUnitTests() 
        {
            context = Helpers.InMemoryContextHelper.GetContext();
            getNotificationsRepository = new GetNotificationsRepository(context);
        }

        [Fact]
        public async Task ValidGetNotificationsByEmail()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var fakeNotifications = Builder<Core.Models.Notification>.CreateListOfSize(3).All().Do(x =>
            {
                x.UserEmail = "user.testing@outlook.com";
            })
                .TheFirst(1)
                .Do(x =>
                {
                    x.Id = Guid.NewGuid();
                    x.DateCreatedUtc = DateTime.UtcNow.AddHours(-2);
                    x.UserId = userId;
                    x.UserEmail = "user.testing@outlook.com";
                    x.Title = "Event Invitation Notification";
                    x.Type = NotificationType.EventInvitation;
                    x.Content = "You have been invited to a event. Click to see invite.";
                    x.ReferenceId = Guid.NewGuid();
                })
                .TheNext(1)
                .Do(x =>
                {
                    x.Id = Guid.NewGuid();
                    x.DateCreatedUtc = DateTime.UtcNow.AddDays(-1);
                    x.UserId = userId;
                    x.UserEmail = "user.testing@outlook.com";
                    x.Title = "Squad Invitation Notification";
                    x.Type = NotificationType.SquadInvitation;
                    x.Content = "You have been invited to a squad. Click to see invite.";
                    x.ReferenceId = Guid.NewGuid();
                })
                .TheLast(1)
                .Do(x =>
                {
                    x.Id = Guid.NewGuid();
                    x.DateCreatedUtc = DateTime.UtcNow.AddDays(-2);
                    x.UserId = userId;
                    x.UserEmail = "user.testing@outlook.com";
                    x.Title = "Event Invitation Notification";
                    x.Type = NotificationType.EventInvitation;
                    x.Content = "You have been invited to a event. Click to see invite.";
                    x.ReferenceId = Guid.NewGuid();
                })
                .Build();
            context.Notifications.AddRange(fakeNotifications);
            await context.SaveChangesAsync();

            //Act
            var result = await getNotificationsRepository.GetNotificationsByEmailAsync("user.testing@outlook.com");

            //Assert
            Assert.Equal(3, result.Count());
            var firstNotification = result.First();
            Assert.Equal(firstNotification.Id, fakeNotifications[0].Id);
            Assert.Equal(firstNotification.UserId, fakeNotifications[0].UserId);
            Assert.Equal(firstNotification.Title, fakeNotifications[0].Title);
            Assert.Equal(firstNotification.Type, fakeNotifications[0].Type);
            Assert.Equal(firstNotification.Content, fakeNotifications[0].Content);
            Assert.Equal(firstNotification.ReferenceId, fakeNotifications[0].ReferenceId);
            var secondNotification = result.ElementAt(1);
            Assert.Equal(secondNotification.Id, fakeNotifications[1].Id);
            Assert.Equal(secondNotification.UserId, fakeNotifications[1].UserId);
            Assert.Equal(secondNotification.Title, fakeNotifications[1].Title);
            Assert.Equal(secondNotification.Type, fakeNotifications[1].Type);
            Assert.Equal(secondNotification.Content, fakeNotifications[1].Content);
            Assert.Equal(secondNotification.ReferenceId, fakeNotifications[1].ReferenceId);
            var thirdNotification = result.Last();
            Assert.Equal(thirdNotification.Id, fakeNotifications[2].Id);
            Assert.Equal(thirdNotification.UserId, fakeNotifications[2].UserId);
            Assert.Equal(thirdNotification.Title, fakeNotifications[2].Title);
            Assert.Equal(thirdNotification.Type, fakeNotifications[2].Type);
            Assert.Equal(thirdNotification.Content, fakeNotifications[2].Content);
            Assert.Equal(thirdNotification.ReferenceId, fakeNotifications[2].ReferenceId);
        }
    }
}
