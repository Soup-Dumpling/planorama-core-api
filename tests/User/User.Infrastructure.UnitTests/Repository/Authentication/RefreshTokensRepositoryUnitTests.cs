using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Models = Planorama.User.Core.Models;
using Planorama.User.Core.UseCases.Authentication.RefreshTokens;
using Planorama.User.Infrastructure.Repository.Authentication;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.Infrastructure.UnitTests.Repository.Authentication
{
    public class RefreshTokensRepositoryUnitTests
    {
        private readonly UserDBContext context;
        private RefreshTokensRepository refreshTokensRepository;

        public RefreshTokensRepositoryUnitTests()
        {
            context = Helpers.InMemoryContextHelper.GetContext();
            refreshTokensRepository = new RefreshTokensRepository(context);
        }

        [Fact]
        public async Task ValidUpdateRefreshToken()
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
                    x.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(2);
                })
                .Build();
            context.Users.Add(fakeUser);
            context.UserCredentials.Add(fakeUserCredential);
            await context.SaveChangesAsync();

            var updatedRefreshToken = "T4jhzPQ3hI+C0LXLI91OCV5/QGdkNPDuLrJ7L4VXYHzluJUqjY/7uWs1Hk3MoZUM3ObXA6ikyWrpddX7q7frKA==";
            var updatedRefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);
            var expectedEvent = new TokensUpdatedEvent(fakeUser.Id, updatedRefreshToken, updatedRefreshTokenExpiresAtUtc);

            //Act
            var result = await refreshTokensRepository.UpdateRefreshTokenAsync(fakeUser.Id, updatedRefreshToken, updatedRefreshTokenExpiresAtUtc, "user.testing@outlook.com");

            //Assert
            var count = await context.UserCredentials.CountAsync();
            Assert.Equal(expectedEvent, result);
            var user = await context.Users.FindAsync(fakeUser.Id);
            var createdEvent = context.IntegrationEvents.SingleOrDefault(x => x.Username == "user.testing@outlook.com" && x.AggregationId == fakeUser.Id.ToString());
            var createdEventData = JsonSerializer.Deserialize<TokensUpdatedEvent>(createdEvent.Data);
            Assert.Equal(result.Id, user.UserCredential.UserId);
            Assert.Equal(result.RefreshToken, user.UserCredential.RefreshToken);
            Assert.Equal(result.RefreshTokenExpiresAtUtc, user.UserCredential.RefreshTokenExpiresAtUtc);
            Assert.Equal(expectedEvent.Id, createdEventData.Id);
            Assert.Equal(expectedEvent.RefreshToken, createdEventData.RefreshToken);
            Assert.Equal(expectedEvent.RefreshTokenExpiresAtUtc, createdEventData.RefreshTokenExpiresAtUtc);
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task ValidFindUserCredentialByRefreshToken()
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
                    x.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(2);
                })
                .Build();
            context.Users.Add(fakeUser);
            context.UserCredentials.Add(fakeUserCredential);
            await context.SaveChangesAsync();

            //Act
            var result = await refreshTokensRepository.FindUserCredentialByRefreshTokenAsync(fakeUserCredential.RefreshToken);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<Models.UserCredential>(result);
        }

        [Fact]
        public async Task InvalidFindUserCredentialByRefreshToken()
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
                    x.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(2);
                })
                .Build();

            //Act
            var result = await refreshTokensRepository.FindUserCredentialByRefreshTokenAsync(fakeUserCredential.RefreshToken);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ValidGetUserFullNameById()
        {
            //Arrange
            var fakeUser = Builder<Models.User>.CreateNew().Build();
            context.Users.Add(fakeUser);
            await context.SaveChangesAsync();

            //Act
            var result = await refreshTokensRepository.GetUserFullNameByIdAsync(fakeUser.Id);

            //Assert
            Assert.Equal(result, $"{fakeUser.FirstName} {fakeUser.LastName}");
        }

        [Fact]
        public async Task ValidGetUserRolesById()
        {
            //Arrange
            var fakeUser = Builder<Models.User>.CreateNew().Build();
            var roles = Builder<Models.Role>.CreateListOfSize(4)
                .TheFirst(1)
                .Do(x =>
                {
                    x.Id = Guid.NewGuid();
                    x.Name = "User";
                    x.IdentityName = "planorama.user";
                })
                .TheNext(1)
                .Do(x =>
                {
                    x.Id = Guid.NewGuid();
                    x.Name = "Member";
                    x.IdentityName = "planorama.member";
                })
                .TheNext(1)
                .Do(x =>
                {
                    x.Id = Guid.NewGuid();
                    x.Name = "Moderator";
                    x.IdentityName = "planorama.moderator";
                })
                .TheLast(1)
                .Do(x =>
                {
                    x.Id = Guid.NewGuid();
                    x.Name = "Admin";
                    x.IdentityName = "planorama.admin";
                })
                .Build();
            var userRoles = Builder<Models.UserRole>.CreateListOfSize(4)
                .TheFirst(1)
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.RoleId = roles[0].Id;
                })
                .TheNext(1)
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.RoleId = roles[1].Id;
                })
                .TheNext(1)
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.RoleId = roles[2].Id;
                })
                .TheLast(1)
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.RoleId = roles[3].Id;
                })
                .Build();
            context.Users.Add(fakeUser);
            context.Roles.AddRange(roles);
            context.UserRoles.AddRange(userRoles);
            await context.SaveChangesAsync();

            var expectedResult = roles.Select(x => x.IdentityName);

            //Act
            var result = await refreshTokensRepository.GetUserRolesByIdAsync(fakeUser.Id);

            //Assert
            var count = result.Count();
            Assert.Equal(expectedResult, result);
            Assert.Equal(4, count);
        }
    }
}
