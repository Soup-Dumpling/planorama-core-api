using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Planorama.Notification.Core.Enums;
using Planorama.Notification.Core.UseCases.Notification.AddNotification;
using Planorama.Notification.Infrastructure.Repository.Notification;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.Notification.Infrastructure.UnitTests.Repository.Notification
{
    public class AddNotificationRepositoryUnitTests
    {
        private readonly NotificationContext context;
        private readonly AddNotificationRepository addNotificationRepository;

        public AddNotificationRepositoryUnitTests() 
        {
            context = Helpers.InMemoryContextHelper.GetContext();
            addNotificationRepository = new AddNotificationRepository(context);
        }

        [Fact]
        public async Task AddNotification()
        {
            //Arrange
            var fakeNotification = Builder<Core.Models.Notification>.CreateNew()
                .Do(x =>
                {
                    x.Id = Guid.NewGuid();
                    x.DateCreatedUtc = DateTime.UtcNow;
                    x.UserId = Guid.NewGuid();
                    x.UserEmail = "user.testing@outlook.com";
                    x.Title = "title";
                    x.Type = NotificationType.SquadInvitation;
                    x.Content = "content";
                    x.ReferenceId = Guid.NewGuid();
                    x.DeleteId = "deleteId";
                })
                .Build();
            var expectedEvent = new NotificationAddedEvent(fakeNotification.Id, fakeNotification.DateCreatedUtc, fakeNotification.UserId, fakeNotification.UserEmail, fakeNotification.Title, fakeNotification.Type, fakeNotification.Content, fakeNotification.ReferenceId, fakeNotification.DeleteId);
            
            //Act
            var result = await addNotificationRepository.AddNotificationAsync(fakeNotification);

            //Assert
            var count = await context.Notifications.CountAsync();
            Assert.Equal(1, count);
            Assert.Equal(expectedEvent, result);
            var notification = await context.Notifications.FindAsync(fakeNotification.Id);
            notification.Should().BeEquivalentTo(fakeNotification);
        }
    }
}
