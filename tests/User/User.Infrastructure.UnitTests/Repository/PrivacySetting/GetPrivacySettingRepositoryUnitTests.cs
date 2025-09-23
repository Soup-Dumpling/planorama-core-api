using FizzWare.NBuilder;
using Models = Planorama.User.Core.Models;
using Planorama.User.Infrastructure.Repository.PrivacySetting;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.Infrastructure.UnitTests.Repository.PrivacySetting
{
    public class GetPrivacySettingRepositoryUnitTests
    {
        private readonly UserDBContext context;
        private readonly GetPrivacySettingRepository getPrivacySettingRepository;

        public GetPrivacySettingRepositoryUnitTests() 
        {
            context = Helpers.InMemoryContextHelper.GetContext();
            getPrivacySettingRepository = new GetPrivacySettingRepository(context);
        }

        [Fact]
        public async Task ValidGetUserPrivacySettingById()
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

            //Act
            var result = await getPrivacySettingRepository.GetUserPrivacySettingByIdAsync(fakeUser.Id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(fakeUser.Id, result.UserId);
            Assert.False(result.IsPrivate);
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
            var result = await getPrivacySettingRepository.GetUserIdByEmailAsync(fakeUserCredential.EmailAddress);

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
            var result = await getPrivacySettingRepository.GetUserIdByEmailAsync(fakeUserCredential.EmailAddress);

            //Assert
            Assert.Null(result);
        }
    }
}
