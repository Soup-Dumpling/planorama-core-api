using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Models = Planorama.User.Core.Models;
using Planorama.User.Core.UseCases.Authentication.RegisterUser;
using Planorama.User.Infrastructure.Repository.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.Infrastructure.UnitTests.Repository.Authentication
{
    public class RegisterUserRepositoryUnitTests
    {
        private readonly UserDBContext context;
        private readonly RegisterUserRepository registerUserRepository;

        public RegisterUserRepositoryUnitTests() 
        {
            context = Helpers.InMemoryContextHelper.GetContext();
            registerUserRepository = new RegisterUserRepository(context);
        }

        [Fact]
        public async Task ValidRegisterUser()
        {
            //Arrange
            var fakeUser = Builder<Models.User>.CreateNew().Build();
            var fakeUserCredential = Builder<Models.UserCredential>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.EmailAddress = "user.testing@outlook.com";
                    x.HashedPassword = "Gt9Yc4AiIvmsC1QQbe2RZsCIqvoYlst2xbz0Fs8aHnw=";
                }).Build();
            var fakeUserPrivacySetting = Builder<Models.UserPrivacySetting>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                }).Build();
            var fakeRoles = Builder<Models.Role>.CreateNew().Build();
            context.Roles.Add(fakeRoles);
            await context.SaveChangesAsync();

            var expectedEvent = new UserRegisteredEvent(fakeUser.Id, fakeUser.FirstName, fakeUser.LastName);

            //Act
            var result = await registerUserRepository.RegisterUserAsync(fakeUser, fakeUserCredential, fakeUserPrivacySetting, new List<string> { fakeRoles.IdentityName }, fakeUserCredential.EmailAddress);

            //Assert
            var userCount = await context.Users.CountAsync();
            var userCredentialCount = await context.UserCredentials.CountAsync();
            var userPrivacySettingCount = await context.UserPrivacySettings.CountAsync();
            var userRoleCount = await context.UserRoles.CountAsync();
            Assert.Equal(1, userCount);
            Assert.Equal(1, userCredentialCount);
            Assert.Equal(1, userPrivacySettingCount);
            Assert.Equal(1, userRoleCount);
            Assert.Equal(expectedEvent, result);
            var user = await context.Users.FindAsync(fakeUser.Id);
            var createdEvent = context.IntegrationEvents.SingleOrDefault(x => x.Username == "user.testing@outlook.com" && x.AggregationId == fakeUser.Id.ToString());
            var createdEventData = JsonSerializer.Deserialize<UserRegisteredEvent>(createdEvent.Data);
            Assert.Equal(result.Id, user.Id);
            Assert.Equal(result.FirstName, user.FirstName);
            Assert.Equal(result.LastName, user.LastName);
            Assert.Equal(fakeUserCredential.UserId, user.Id);
            Assert.Equal(fakeUserCredential.EmailAddress, user.UserCredential.EmailAddress);
            Assert.Equal(fakeUserCredential.HashedPassword, user.UserCredential.HashedPassword);
            Assert.Equal(fakeUserPrivacySetting.UserId, user.Id);
            Assert.False(user.UserPrivacySetting.IsPrivate);
            Assert.Equal(fakeRoles.IdentityName, user.UserRoles.First().Role.IdentityName);
            Assert.Equal(expectedEvent.Id, createdEventData.Id);
            Assert.Equal(expectedEvent.FirstName, createdEventData.FirstName);
            Assert.Equal(expectedEvent.LastName, createdEventData.LastName);
        }

        [Fact]
        public async Task ValidCheckIfUserExists()
        {
            //Arrange
            var fakeUser = Builder<Models.User>.CreateNew().Build();
            var fakeUserCredential = Builder<Models.UserCredential>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.EmailAddress = "user.testing@outlook.com";
                    x.HashedPassword = "Gt9Yc4AiIvmsC1QQbe2RZsCIqvoYlst2xbz0Fs8aHnw=";
                    x.RefreshToken = "WTkh8E7Zgq/l8sqs7yaCUnWXROXyBejV5khykyZlZzoYrGiulKGNqWcwRX5u/WUxWEeXt4M2QeMcImWbw8PlSA==";
                    x.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(-1);
                }).Build();
            context.Users.Add(fakeUser);
            context.UserCredentials.Add(fakeUserCredential);
            await context.SaveChangesAsync();

            //Act
            var result = await registerUserRepository.CheckIfUserExistsAsync(fakeUserCredential.EmailAddress);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task InvalidCheckIfUserExists()
        {
            //Arrange
            var fakeUser = Builder<Models.User>.CreateNew().Build();
            var fakeUserCredential = Builder<Models.UserCredential>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.EmailAddress = "user.testing@outlook.com";
                    x.HashedPassword = "Gt9Yc4AiIvmsC1QQbe2RZsCIqvoYlst2xbz0Fs8aHnw=";
                    x.RefreshToken = "WTkh8E7Zgq/l8sqs7yaCUnWXROXyBejV5khykyZlZzoYrGiulKGNqWcwRX5u/WUxWEeXt4M2QeMcImWbw8PlSA==";
                    x.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(-1);
                }).Build();

            //Act
            var result = await registerUserRepository.CheckIfUserExistsAsync(fakeUserCredential.EmailAddress);

            //Assert
            Assert.False(result);
        }
    }
}
