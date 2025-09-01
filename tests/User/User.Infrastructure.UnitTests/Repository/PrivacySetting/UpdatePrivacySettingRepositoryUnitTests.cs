using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Planorama.User.Core.UseCases.PrivacySetting.UpdatePrivacySetting;
using Planorama.User.Infrastructure.Repository.PrivacySetting;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Models = Planorama.User.Core.Models;

namespace Planorama.User.Infrastructure.UnitTests.Repository.PrivacySetting
{
    public class UpdatePrivacySettingRepositoryUnitTests
    {
        private readonly UserDBContext context;
        private UpdatePrivacySettingRepository updatePrivacySettingRepository;

        public UpdatePrivacySettingRepositoryUnitTests()
        {
            context = Helpers.InMemoryContextHelper.GetContext();
            updatePrivacySettingRepository = new UpdatePrivacySettingRepository(context);
        }

        [Fact]
        public async Task ValidUpdatePrivacySetting()
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
            var fakeUserPrivacySetting = Builder<Models.UserPrivacySetting>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.IsPrivate = false;
                }).Build();
            context.Users.Add(fakeUser);
            context.UserCredentials.Add(fakeUserCredential);
            context.UserPrivacySettings.Add(fakeUserPrivacySetting);
            await context.SaveChangesAsync();

            var expectedEvent = new PrivacySettingUpdatedEvent(fakeUser.Id, true);

            //Act
            var result = await updatePrivacySettingRepository.UpdatePrivacySettingAsync(fakeUser.Id, true, "user.testing@outlook.com");

            //Assert
            var count = await context.UserPrivacySettings.CountAsync();
            Assert.Equal(expectedEvent, result);
            var userPrivacySetting = await context.UserPrivacySettings.FindAsync(fakeUser.Id);
            var createdEvent = context.IntegrationEvents.SingleOrDefault(x => x.Username == "user.testing@outlook.com" && x.AggregationId == fakeUser.Id.ToString());
            var createdEventData = JsonSerializer.Deserialize<PrivacySettingUpdatedEvent>(createdEvent.Data);
            Assert.Equal(result.Id, userPrivacySetting.UserId);
            Assert.True(result.IsPrivate);
            Assert.True(userPrivacySetting.IsPrivate);
            Assert.Equal(expectedEvent.Id, createdEventData.Id);
            Assert.Equal(expectedEvent.IsPrivate, createdEventData.IsPrivate);
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task ValidCheckIfUserExists()
        {
            //Arrange
            var fakeUser = Builder<Models.User>.CreateNew().Build();
            context.Users.Add(fakeUser);
            await context.SaveChangesAsync();

            //Act
            var result = await updatePrivacySettingRepository.CheckIfUserExists(fakeUser.Id);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task InvalidCheckIfUserExists()
        {
            //Arrange
            var fakeUser = Builder<Models.User>.CreateNew().Build();

            //Act
            var result = await updatePrivacySettingRepository.CheckIfUserExists(fakeUser.Id);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidGetUserIdByEmail()
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
            var result = await updatePrivacySettingRepository.GetUserIdByEmailAsync(fakeUserCredential.EmailAddress);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(fakeUser.Id, result);
        }

        [Fact]
        public async Task InvalidGetUserIdByEmail()
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
            var result = await updatePrivacySettingRepository.GetUserIdByEmailAsync(fakeUserCredential.EmailAddress);

            //Assert
            Assert.Null(result);
        }
    }
}
