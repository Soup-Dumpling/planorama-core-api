using FizzWare.NBuilder;
using Models = Planorama.User.Core.Models;
using Planorama.User.Core.UseCases.Authentication.LogoutUser;
using Planorama.User.Infrastructure.Repository.Authentication;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.Infrastructure.UnitTests.Repository.Authentication
{
    public class LogoutUserRepositoryUnitTests
    {
        private readonly UserDBContext context;
        private readonly LogoutUserRepository logoutUserRepository;

        public LogoutUserRepositoryUnitTests()
        {
            context = Helpers.InMemoryContextHelper.GetContext();
            logoutUserRepository = new LogoutUserRepository(context);
        }

        [Fact]
        public async Task ValidReplaceUserCredential()
        {
            //Arrange
            var fakeUser = Builder<Models.User>.CreateNew().Build();
            var fakeUserCredential = Builder<Models.UserCredential>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.EmailAddress = "user.testing@outlook.com";
                    x.HashedPassword = "Gt9Yc4AiIvmsC1QQbe2RZsCIqvoYlst2xbz0Fs8aHnw=";
                    x.RefreshToken = "nvBhkx2Pfm633TYf56by974P0al2JLhjZ1ch324auT6Jn9dIAerWQNJCthZ05KguFymxZ0QBfeeaMP3IlYt1HQ==";
                    x.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);
                })
                .Build();
            context.Users.Add(fakeUser);
            context.UserCredentials.Add(fakeUserCredential);
            await context.SaveChangesAsync();

            var expectedEvent = new UserLoggedOutEvent(fakeUser.Id);

            //Act
            var result = await logoutUserRepository.ReplaceUserCredentialAsync(fakeUser.Id, "user.testing@outlook.com");

            //Assert
            Assert.Equal(expectedEvent, result);
            var userCredential = await context.UserCredentials.FindAsync(fakeUser.Id);
            var createdEvent = context.IntegrationEvents.SingleOrDefault(x => x.Username == "user.testing@outlook.com" && x.AggregationId == fakeUser.Id.ToString());
            var createdEventData = JsonSerializer.Deserialize<UserLoggedOutEvent>(createdEvent.Data);
            Assert.Equal(result.Id, userCredential.UserId);
            Assert.Equal(fakeUserCredential.EmailAddress, userCredential.EmailAddress);
            Assert.Equal(fakeUserCredential.HashedPassword, userCredential.HashedPassword);
            Assert.Equal(expectedEvent.Id, createdEventData.Id);
            Assert.Null(userCredential.RefreshToken);
            Assert.Null(userCredential.RefreshTokenExpiresAtUtc);
        }

        [Fact]
        public async Task ValidCheckIfUserCredentialExists()
        {
            //Arrange
            var fakeUser = Builder<Models.User>.CreateNew().Build();
            var fakeUserCredential = Builder<Models.UserCredential>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.EmailAddress = "user.testing@outlook.com";
                    x.HashedPassword = "Gt9Yc4AiIvmsC1QQbe2RZsCIqvoYlst2xbz0Fs8aHnw=";
                    x.RefreshToken = "nvBhkx2Pfm633TYf56by974P0al2JLhjZ1ch324auT6Jn9dIAerWQNJCthZ05KguFymxZ0QBfeeaMP3IlYt1HQ==";
                    x.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);
                })
                .Build();
            context.Users.Add(fakeUser);
            context.UserCredentials.Add(fakeUserCredential);
            await context.SaveChangesAsync();

            //Act
            var result = await logoutUserRepository.CheckIfUserCredentialExistsAsync(fakeUser.Id);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task InvalidCheckIfUserCredentialExists()
        {
            //Arrange
            var fakeUser = Builder<Models.User>.CreateNew().Build();
            var fakeUserCredential = Builder<Models.UserCredential>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.EmailAddress = "user.testing@outlook.com";
                    x.HashedPassword = "Gt9Yc4AiIvmsC1QQbe2RZsCIqvoYlst2xbz0Fs8aHnw=";
                    x.RefreshToken = "nvBhkx2Pfm633TYf56by974P0al2JLhjZ1ch324auT6Jn9dIAerWQNJCthZ05KguFymxZ0QBfeeaMP3IlYt1HQ==";
                    x.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);
                })
                .Build();

            //Act
            var result = await logoutUserRepository.CheckIfUserCredentialExistsAsync(fakeUser.Id);

            //Assert
            Assert.False(result);
        }
    }
}
