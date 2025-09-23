using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Planorama.Notification.Core.Enums;
using Planorama.Notification.Infrastructure.Repository.Notification;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.Notification.Infrastructure.UnitTests.Repository.Notification
{
    public class DeleteNotificationsRepositoryUnitTests
    {
        private readonly NotificationContext context;
        private readonly DeleteNotificationsRepository deleteNotificationsRepository;

        public DeleteNotificationsRepositoryUnitTests() 
        {
            context = Helpers.InMemoryContextHelper.GetContext();
            deleteNotificationsRepository = new DeleteNotificationsRepository(context);
        }

        [Fact]
        public async Task DeleteIndividualNotificationByUserIdAndDeleteId()
        {
            //Arrange
            var fakeNotification = Builder<Core.Models.Notification>.CreateNew()
                .Do(x =>
                {
                    x.Id = Guid.NewGuid();
                    x.DateCreatedUtc = DateTime.UtcNow.AddDays(-1);
                    x.UserId = Guid.NewGuid();
                    x.UserEmail = "user.testing@outlook.com";
                    x.Title = "Squad Invite";
                    x.Type = NotificationType.SquadInvitation;
                    x.Content = "content";
                    x.ReferenceId = Guid.NewGuid();
                    x.DeleteId = "deleteId";
                })
                .Build();
            await context.Notifications.AddAsync(fakeNotification);
            await context.SaveChangesAsync();

            //Act
            var result = await deleteNotificationsRepository.DeleteNotificationsAsync(fakeNotification.UserId, fakeNotification.DeleteId);

            //Assert
            var notificationExists = await context.Notifications.AnyAsync(x => x.ReferenceId == fakeNotification.ReferenceId);
            Assert.False(notificationExists);
            Assert.Equal(fakeNotification.Id, result.First().Id);
            Assert.Equal(fakeNotification.DateCreatedUtc, result.First().DateCreatedUtc);
            Assert.Equal(fakeNotification.UserId, result.First().UserId);
            Assert.Equal(fakeNotification.UserEmail, result.First().UserEmail);
            Assert.Equal(fakeNotification.Title, result.First().Title);
            Assert.Equal(fakeNotification.Type, result.First().Type);
            Assert.Equal(fakeNotification.Content, result.First().Content);
            Assert.Equal(fakeNotification.ReferenceId, result.First().ReferenceId);
            Assert.Equal(fakeNotification.DeleteId, result.First().DeleteId);
        }

        [Fact]
        public async Task DeleteMultipleNotificationsByDeleteId()
        {
            //Arrange
            var notificationsReferenceId = Guid.NewGuid();
            var fakeNotifications = Builder<Core.Models.Notification>.CreateListOfSize(3)
                .All().Do(x => x.DeleteId = $"requesteeUserId{notificationsReferenceId}")
                .TheFirst(1)
                .Do(x =>
                {
                    x.Id = Guid.NewGuid();
                    x.DateCreatedUtc = DateTime.UtcNow.AddDays(-2);
                    x.UserId = Guid.NewGuid();
                    x.UserEmail = "moderator.testing@outlook.com";
                    x.Title = "Join Squad Request";
                    x.Type = NotificationType.SquadRequestApproval;
                    x.Content = "content";
                    x.ReferenceId = notificationsReferenceId;
                })
                .TheNext(1)
                .Do(x =>
                {
                    x.Id = Guid.NewGuid();
                    x.DateCreatedUtc = DateTime.UtcNow.AddDays(-2);
                    x.UserId = Guid.NewGuid();
                    x.UserEmail = "moderator2.testing@outlook.com";
                    x.Title = "Join Squad Request";
                    x.Type = NotificationType.SquadRequestApproval;
                    x.Content = "content";
                    x.ReferenceId = notificationsReferenceId;
                })
                .TheLast(1)
                .Do(x =>
                {
                    x.Id = Guid.NewGuid();
                    x.DateCreatedUtc = DateTime.UtcNow.AddDays(-2);
                    x.UserId = Guid.NewGuid();
                    x.UserEmail = "admin.testing@outlook.com";
                    x.Title = "Join Squad Request";
                    x.Type = NotificationType.SquadRequestApproval;
                    x.Content = "content";
                    x.ReferenceId = notificationsReferenceId;
                })
                .Build();
            await context.Notifications.AddRangeAsync(fakeNotifications);
            await context.SaveChangesAsync();

            //Act
            var result = await deleteNotificationsRepository.DeleteNotificationsAsync(null, $"requesteeUserId{notificationsReferenceId}");

            //Assert
            var notificationsCount = await context.Notifications.CountAsync(x => x.DeleteId == $"requesteeUserId{notificationsReferenceId}");
            Assert.Equal(0, notificationsCount);
            Assert.Equal(fakeNotifications[0].Id, result.First().Id);
            Assert.Equal(fakeNotifications[0].DateCreatedUtc, result.First().DateCreatedUtc);
            Assert.Equal(fakeNotifications[0].UserId, result.First().UserId);
            Assert.Equal(fakeNotifications[0].UserEmail, result.First().UserEmail);
            Assert.Equal(fakeNotifications[0].Title, result.First().Title);
            Assert.Equal(fakeNotifications[0].Type, result.First().Type);
            Assert.Equal(fakeNotifications[0].Content, result.First().Content);
            Assert.Equal(fakeNotifications[0].ReferenceId, result.First().ReferenceId);
            Assert.Equal(fakeNotifications[0].DeleteId, result.First().DeleteId);
            Assert.Equal(fakeNotifications[1].Id, result.ElementAt(1).Id);
            Assert.Equal(fakeNotifications[1].DateCreatedUtc, result.ElementAt(1).DateCreatedUtc);
            Assert.Equal(fakeNotifications[1].UserId, result.ElementAt(1).UserId);
            Assert.Equal(fakeNotifications[1].UserEmail, result.ElementAt(1).UserEmail);
            Assert.Equal(fakeNotifications[1].Title, result.ElementAt(1).Title);
            Assert.Equal(fakeNotifications[1].Type, result.ElementAt(1).Type);
            Assert.Equal(fakeNotifications[1].Content, result.ElementAt(1).Content);
            Assert.Equal(fakeNotifications[1].ReferenceId, result.ElementAt(1).ReferenceId);
            Assert.Equal(fakeNotifications[1].DeleteId, result.ElementAt(1).DeleteId);
            Assert.Equal(fakeNotifications[2].Id, result.Last().Id);
            Assert.Equal(fakeNotifications[2].DateCreatedUtc, result.Last().DateCreatedUtc);
            Assert.Equal(fakeNotifications[2].UserId, result.Last().UserId);
            Assert.Equal(fakeNotifications[2].UserEmail, result.Last().UserEmail);
            Assert.Equal(fakeNotifications[2].Title, result.Last().Title);
            Assert.Equal(fakeNotifications[2].Type, result.Last().Type);
            Assert.Equal(fakeNotifications[2].Content, result.Last().Content);
            Assert.Equal(fakeNotifications[2].ReferenceId, result.Last().ReferenceId);
            Assert.Equal(fakeNotifications[2].DeleteId, result.Last().DeleteId);
        }
    }
}
