using NSubstitute;
using Planorama.User.Core.Context;
using Planorama.User.Core.Exceptions;
using Planorama.User.Core.Models;
using Planorama.User.Core.Services;
using Planorama.User.Core.UseCases.Authentication.RefreshTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.Core.UnitTests.UseCases.Authentication
{
    public class RefreshTokensCommandHandlerUnitTests
    {
        private readonly RefreshTokensCommandHandler refreshTokensCommandHandler;
        private readonly IJwtService jwtServiceMock = Substitute.For<IJwtService>();
        private readonly IUserContext userContextMock = Substitute.For<IUserContext>();
        private readonly IRefreshTokensRepository refreshTokensRepositoryMock = Substitute.For<IRefreshTokensRepository>();

        public RefreshTokensCommandHandlerUnitTests()
        {
            refreshTokensCommandHandler = new RefreshTokensCommandHandler(jwtServiceMock, refreshTokensRepositoryMock, userContextMock);
        }

        [Fact]
        public async Task ValidCommand()
        {
            //Arrange
            var command = new RefreshTokensCommand("WTkh8E7Zgq/l8sqs7yaCUnWXROXyBejV5khykyZlZzoYrGiulKGNqWcwRX5u/WUxWEeXt4M2QeMcImWbw8PlSA==");
            userContextMock.IsLoggedIn().Returns(true);
            userContextMock.UserName.Returns("user.testing@outlook.com");
            var userCredential = new UserCredential() { UserId = Guid.NewGuid(), EmailAddress = "user.testing@outlook.com", HashedPassword = "AQAAAAIAAYagAAAAEON9dR34Gs2mNIsbIL5sClwRZN+NnZtuKc1wVHHipo5H9IgKj04vx22XV49i08wMwg==", RefreshToken = command.RefreshToken, RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7) };
            refreshTokensRepositoryMock.FindUserCredentialByRefreshTokenAsync(Arg.Any<string>()).Returns(Task.FromResult(userCredential));
            refreshTokensRepositoryMock.GetUserFullNameByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult("firstName lastName"));
            var roles = new List<string>() { "planorama.user" };
            refreshTokensRepositoryMock.GetUserRolesByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult(roles.AsEnumerable()));
            var jwtToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJwbGFub3JhbWEtYXBpIiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMSIsImV4cCI6MTc1NjcyNDU2MCwianRpIjoiZGY0Y2JlMGItZmVjYS00NjY2LThjODYtYWIyNzU3YjQ3YzgyIiwic3ViIjoiZmY0Zjk2NzUtNTIwZi00MDgwLTlkYTMtOGMyMzU0Nzg5N2QwIiwibmFtZSI6IkJyaWFuIEdyaWZmaW4iLCJlbWFpbCI6ImJlMTgyYjA4YzNmNDRlMDQ5ZjI2YTg1OEBnbWFpbC5jb20iLCJzY29wZSI6InBsYW5vcmFtYS1hcGkiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJwbGFub3JhbWEudXNlciIsImlhdCI6MTc1NjcyMDk2MCwibmJmIjoxNzU2NzIwOTYwfQ.YELSFUzpc1N6BTg6wGgg3v1FeH3NlIGvjuzns6lZ11kuOGPujyM2IzA0Ryoxxqc7FhOyuYqrca8qv4T45n3cGA";
            jwtServiceMock.GenerateJwtToken(Arg.Any<UserCredential>(), Arg.Any<string>(), Arg.Any<IEnumerable<string>>()).Returns((jwtToken, DateTime.UtcNow.AddMinutes(60)));
            var refreshToken = "nvBhkx2Pfm633TYf56by974P0al2JLhjZ1ch324auT6Jn9dIAerWQNJCthZ05KguFymxZ0QBfeeaMP3IlYt1HQ==";
            jwtServiceMock.GenerateRefreshToken().Returns(refreshToken);
            var tokensUpdatedEvent = new TokensUpdatedEvent(userCredential.UserId, refreshToken, DateTime.UtcNow.AddDays(7));
            refreshTokensRepositoryMock.UpdateRefreshTokenAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<string>()).Returns(Task.FromResult(tokensUpdatedEvent));
            jwtServiceMock.WriteAccessAndRefreshTokensAsHttpOnlyCookie(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
            jwtServiceMock.WriteAccessAndRefreshTokensAsHttpOnlyCookie(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());

            //Act
            await refreshTokensCommandHandler.Handle(command, CancellationToken.None);

            //Assert
            await refreshTokensRepositoryMock.Received().FindUserCredentialByRefreshTokenAsync(command.RefreshToken);
            await refreshTokensRepositoryMock.Received().GetUserFullNameByIdAsync(userCredential.UserId);
            await refreshTokensRepositoryMock.Received().GetUserRolesByIdAsync(userCredential.UserId);
            jwtServiceMock.Received().GenerateJwtToken(userCredential, "firstName lastName", Arg.Is<IEnumerable<string>>(x => x.Count() == 1 && x.First() == Constants.Roles.UserRole));
            jwtServiceMock.Received().GenerateRefreshToken();
            await refreshTokensRepositoryMock.Received().UpdateRefreshTokenAsync(userCredential.UserId, refreshToken, Arg.Is<DateTime>(x => x <= DateTime.UtcNow.AddDays(7)), "user.testing@outlook.com");
            jwtServiceMock.Received().WriteAccessAndRefreshTokensAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, Arg.Is<DateTime>(x => x <= DateTime.UtcNow.AddMinutes(60)));
            jwtServiceMock.Received().WriteAccessAndRefreshTokensAsHttpOnlyCookie("REFRESH_TOKEN", refreshToken, Arg.Is<DateTime>(x => x <= DateTime.UtcNow.AddDays(7)));
        }

        [Fact]
        public async Task UserNotLoggedIn()
        {
            //Arrange
            var command = new RefreshTokensCommand("WTkh8E7Zgq/l8sqs7yaCUnWXROXyBejV5khykyZlZzoYrGiulKGNqWcwRX5u/WUxWEeXt4M2QeMcImWbw8PlSA==");
            userContextMock.IsLoggedIn().Returns(false);

            //Act and Assert
            await Assert.ThrowsAsync<AuthorizationException>(async () => await refreshTokensCommandHandler.Handle(command, CancellationToken.None));
            await refreshTokensRepositoryMock.DidNotReceive().FindUserCredentialByRefreshTokenAsync(Arg.Any<string>());
            await refreshTokensRepositoryMock.DidNotReceive().GetUserFullNameByIdAsync(Arg.Any<Guid>());
            await refreshTokensRepositoryMock.DidNotReceive().GetUserRolesByIdAsync(Arg.Any<Guid>());
            jwtServiceMock.DidNotReceive().GenerateJwtToken(Arg.Any<UserCredential>(), Arg.Any<string>(), Arg.Any<IEnumerable<string>>());
            jwtServiceMock.DidNotReceive().GenerateRefreshToken();
            await refreshTokensRepositoryMock.DidNotReceive().UpdateRefreshTokenAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<string>());
            jwtServiceMock.DidNotReceive().WriteAccessAndRefreshTokensAsHttpOnlyCookie("ACCESS_TOKEN", Arg.Any<string>(), Arg.Any<DateTime>());
            jwtServiceMock.DidNotReceive().WriteAccessAndRefreshTokensAsHttpOnlyCookie("REFRESH_TOKEN", Arg.Any<string>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task UserCredentialDoesNotExist()
        {
            //Arrange
            var command = new RefreshTokensCommand("WTkh8E7Zgq/l8sqs7yaCUnWXROXyBejV5khykyZlZzoYrGiulKGNqWcwRX5u/WUxWEeXt4M2QeMcImWbw8PlSA==");
            userContextMock.IsLoggedIn().Returns(true);
            refreshTokensRepositoryMock.FindUserCredentialByRefreshTokenAsync(Arg.Any<string>()).Returns(Task.FromResult(null as UserCredential));

            //Act and Assert
            await Assert.ThrowsAsync<RefreshTokenException>(async () => await refreshTokensCommandHandler.Handle(command, CancellationToken.None));
            await refreshTokensRepositoryMock.Received().FindUserCredentialByRefreshTokenAsync(command.RefreshToken);
            await refreshTokensRepositoryMock.DidNotReceive().GetUserFullNameByIdAsync(Arg.Any<Guid>());
            await refreshTokensRepositoryMock.DidNotReceive().GetUserRolesByIdAsync(Arg.Any<Guid>());
            jwtServiceMock.DidNotReceive().GenerateJwtToken(Arg.Any<UserCredential>(), Arg.Any<string>(), Arg.Any<IEnumerable<string>>());
            jwtServiceMock.DidNotReceive().GenerateRefreshToken();
            await refreshTokensRepositoryMock.DidNotReceive().UpdateRefreshTokenAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<string>());
            jwtServiceMock.DidNotReceive().WriteAccessAndRefreshTokensAsHttpOnlyCookie("ACCESS_TOKEN", Arg.Any<string>(), Arg.Any<DateTime>());
            jwtServiceMock.DidNotReceive().WriteAccessAndRefreshTokensAsHttpOnlyCookie("REFRESH_TOKEN", Arg.Any<string>(), Arg.Any<DateTime>());
        }

        [Fact]
        public async Task RefreshTokenExpired()
        {
            //Arrange
            var command = new RefreshTokensCommand("WTkh8E7Zgq/l8sqs7yaCUnWXROXyBejV5khykyZlZzoYrGiulKGNqWcwRX5u/WUxWEeXt4M2QeMcImWbw8PlSA==");
            userContextMock.IsLoggedIn().Returns(true);
            var userCredential = new UserCredential() { UserId = Guid.NewGuid(), EmailAddress = "user.testing@outlook.com", HashedPassword = "AQAAAAIAAYagAAAAEON9dR34Gs2mNIsbIL5sClwRZN+NnZtuKc1wVHHipo5H9IgKj04vx22XV49i08wMwg==", RefreshToken = command.RefreshToken, RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(-1) };
            refreshTokensRepositoryMock.FindUserCredentialByRefreshTokenAsync(Arg.Any<string>()).Returns(Task.FromResult(userCredential));

            //Act and Assert
            await Assert.ThrowsAsync<RefreshTokenException>(async () => await refreshTokensCommandHandler.Handle(command, CancellationToken.None));
            await refreshTokensRepositoryMock.Received().FindUserCredentialByRefreshTokenAsync(command.RefreshToken);
            await refreshTokensRepositoryMock.DidNotReceive().GetUserFullNameByIdAsync(Arg.Any<Guid>());
            await refreshTokensRepositoryMock.DidNotReceive().GetUserRolesByIdAsync(Arg.Any<Guid>());
            jwtServiceMock.DidNotReceive().GenerateJwtToken(Arg.Any<UserCredential>(), Arg.Any<string>(), Arg.Any<IEnumerable<string>>());
            jwtServiceMock.DidNotReceive().GenerateRefreshToken();
            await refreshTokensRepositoryMock.DidNotReceive().UpdateRefreshTokenAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<string>());
            jwtServiceMock.DidNotReceive().WriteAccessAndRefreshTokensAsHttpOnlyCookie("ACCESS_TOKEN", Arg.Any<string>(), Arg.Any<DateTime>());
            jwtServiceMock.DidNotReceive().WriteAccessAndRefreshTokensAsHttpOnlyCookie("REFRESH_TOKEN", Arg.Any<string>(), Arg.Any<DateTime>());
        }
    }
}
