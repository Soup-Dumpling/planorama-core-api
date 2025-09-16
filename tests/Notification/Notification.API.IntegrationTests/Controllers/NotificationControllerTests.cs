using Alba;
using FizzWare.NBuilder;
using FluentAssertions;
using Planorama.Notification.API.IntegrationTests.Extentions;
using Planorama.Notification.Core.Constants;
using Planorama.Notification.Core.UseCases.Notification.GetNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.Notification.API.IntegrationTests.Controllers
{
    [Collection("Integration")]
    public class NotificationControllerTests
    {
        private readonly IAlbaHost host;

        public NotificationControllerTests(AppFixture fixture) 
        {
            host = fixture.Host;
        }

        [Fact]
        public async Task GetNotifications()
        {
            //Arrange
            var fakeUserId = Guid.NewGuid();
            var fakeAdminId = Guid.NewGuid();
            var fakeNotifications = Builder<Core.Models.Notification>.CreateListOfSize(6)
                .TheFirst(3)
                .Do(x =>
                {
                    x.UserId = fakeUserId;
                    x.UserEmail = "testEmail@test.com";
                })
                .TheLast(3)
                .Do(x => 
                {
                    x.UserId = fakeAdminId;
                    x.UserEmail = "adminEmail@test.com";
                }).Build();

            host.WithEmptyDatabase(async context =>
            {
                context.Notifications.AddRange(fakeNotifications);
                await context.SaveChangesAsync();
            });

            //Act
            var response = await host.Scenario(_ =>
            {
                _.WithClaim(new Claim("email", "testEmail@test.com"));
                _.WithClaim(new Claim("role", Roles.UserRole));
                _.Get.Url($"/api/notification");
                _.StatusCodeShouldBeOk();
            });

            //Assert
            var result = response.ReadAsJson<IEnumerable<GetNotificationsViewModel>>();
            Assert.Equal(3, result.Count());

            var expectedList = fakeNotifications
              .Where(x => x.UserEmail == "testEmail@test.com")
              .Select(x => new GetNotificationsViewModel
              {
                  Id = x.Id,
                  UserId = x.UserId,
                  Title = x.Title,
                  Type = x.Type,
                  Content = x.Content,
                  ReferenceId = x.ReferenceId,
              }).ToList();
            result.Should().BeEquivalentTo(expectedList);
        }
    }
}
