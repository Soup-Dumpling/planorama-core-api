using NSubstitute;
using Planorama.User.Core.Exceptions;
using Planorama.User.Core.Models;
using Planorama.User.Core.Services;
using Planorama.User.Core.UseCases.Authentication.LoginUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.Core.UnitTests.UseCases.Authentication
{
    public class LoginUserCommandHandlerUnitTests
    {
        private readonly LoginUserCommandHandler loginUserCommandHandler;
        private IJwtService jwtServiceMock = Substitute.For<IJwtService>();
        private ILoginUserRepository loginUserRepositoryMock = Substitute.For<ILoginUserRepository>();

        public LoginUserCommandHandlerUnitTests()
        {
            loginUserCommandHandler = new LoginUserCommandHandler(jwtServiceMock, loginUserRepositoryMock);
        }

        [Fact]
        public async Task ValidCommand()
        {
            //Arrange
            var command = new LoginUserCommand("user.testing@outlook.com", "Password1!");
            var userLoggedInEvent = new UserLoggedInEvent(Guid.NewGuid(), "nvBhkx2Pfm633TYf56by974P0al2JLhjZ1ch324auT6Jn9dIAerWQNJCthZ05KguFymxZ0QBfeeaMP3IlYt1HQ==", DateTime.UtcNow.AddDays(7));
            var userCredential = new UserCredential() { UserId = userLoggedInEvent.Id, EmailAddress = command.EmailAddress, HashedPassword = "AQAAAAIAAYagAAAAEON9dR34Gs2mNIsbIL5sClwRZN+NnZtuKc1wVHHipo5H9IgKj04vx22XV49i08wMwg==", RefreshToken = userLoggedInEvent.RefreshToken, RefreshTokenExpiresAtUtc = userLoggedInEvent.RefreshTokenExpiresAtUtc };
            loginUserRepositoryMock.FindUserCredentialByEmailAsync(Arg.Any<string>()).Returns(Task.FromResult(userCredential));
            loginUserRepositoryMock.GetUserFullNameByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult("firstName lastName"));
            var roles = new List<string>() { "planorama.user" };
            loginUserRepositoryMock.GetUserRolesByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult(roles.AsEnumerable()));
            var jwtToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJwbGFub3JhbWEtYXBpIiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMSIsImV4cCI6MTc1NDExMzQwMywianRpIjoiMGZkYWJjNjEtMjBhYi00OGEyLTgyOTQtYzlkMGVhMzIwNmQzIiwic3ViIjoiZmY0Zjk2NzUtNTIwZi00MDgwLTlkYTMtOGMyMzU0Nzg5N2QwIiwibmFtZSI6IkJyaWFuIEdyaWZmaW4iLCJlbWFpbCI6ImJlMTgyYjA4YzNmNDRlMDQ5ZjI2YTg1OEBnbWFpbC5jb20iLCJzY29wZSI6InBsYW5vcmFtYS1hcGkiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJwbGFub3JhbWEudXNlciIsImlhdCI6MTc1NDEwOTgwMywibmJmIjoxNzU0MTA5ODAzfQ.iytSGvyslmq8r4qTEa6ZtxEoTJ0qy5p0rWCfSLV_NJuJ_0-sjlCfAT_XedzSP8rVZndBX9iAbtzt-o28sYOlgw";
            jwtServiceMock.GenerateJwtToken(Arg.Any<UserCredential>(), Arg.Any<string>(), Arg.Any<IEnumerable<string>>()).Returns((jwtToken, DateTime.UtcNow.AddMinutes(60)));
            jwtServiceMock.GenerateRefreshToken().Returns(userLoggedInEvent.RefreshToken);
            loginUserRepositoryMock.AddRefreshTokenAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<string>()).Returns(userLoggedInEvent);
            jwtServiceMock.WriteAccessAndRefreshTokensAsHttpOnlyCookie(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
            jwtServiceMock.WriteAccessAndRefreshTokensAsHttpOnlyCookie(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());

            //Act
            var result = await loginUserCommandHandler.Handle(command, CancellationToken.None);

            //Assert
            Assert.NotEqual(string.Empty, result);
            await loginUserRepositoryMock.Received().FindUserCredentialByEmailAsync(command.EmailAddress);
            await loginUserRepositoryMock.Received().GetUserFullNameByIdAsync(userCredential.UserId);
            await loginUserRepositoryMock.Received().GetUserRolesByIdAsync(userCredential.UserId);
            jwtServiceMock.Received().GenerateJwtToken(userCredential, "firstName lastName", Arg.Is<IEnumerable<string>>(x => x.Count() == 1 && x.First() == Constants.Roles.UserRole));
            jwtServiceMock.Received().GenerateRefreshToken();
            await loginUserRepositoryMock.Received().AddRefreshTokenAsync(userCredential.UserId, userLoggedInEvent.RefreshToken, Arg.Is<DateTime>(x => x >= userLoggedInEvent.RefreshTokenExpiresAtUtc), command.EmailAddress);
            jwtServiceMock.Received().WriteAccessAndRefreshTokensAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, Arg.Is<DateTime>(x => x <= DateTime.UtcNow.AddMinutes(60)));
            jwtServiceMock.Received().WriteAccessAndRefreshTokensAsHttpOnlyCookie("REFRESH_TOKEN", userLoggedInEvent.RefreshToken, Arg.Is<DateTime>(x => x >= userLoggedInEvent.RefreshTokenExpiresAtUtc));
        }

        [Fact]
        public async Task UserNotFound()
        {
            //Arrange
            var command = new LoginUserCommand("user.testing@outlook.com", "Password1!");
            loginUserRepositoryMock.FindUserCredentialByEmailAsync(Arg.Any<string>()).Returns(Task.FromResult(null as UserCredential));

            //Act and Assert
            await Assert.ThrowsAsync<LoginFailedException>(async () => await loginUserCommandHandler.Handle(command, CancellationToken.None));
            await loginUserRepositoryMock.Received().FindUserCredentialByEmailAsync(command.EmailAddress);
            await loginUserRepositoryMock.DidNotReceive().GetUserFullNameByIdAsync(Arg.Any<Guid>());
            await loginUserRepositoryMock.DidNotReceive().GetUserRolesByIdAsync(Arg.Any<Guid>());
            jwtServiceMock.DidNotReceive().GenerateJwtToken(Arg.Any<UserCredential>(), Arg.Any<string>(), Arg.Any<IEnumerable<string>>());
            jwtServiceMock.DidNotReceive().GenerateRefreshToken();
            await loginUserRepositoryMock.DidNotReceive().AddRefreshTokenAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<string>());
            jwtServiceMock.DidNotReceive().WriteAccessAndRefreshTokensAsHttpOnlyCookie("ACCESS_TOKEN", Arg.Any<string>(), Arg.Any<DateTime>());
            jwtServiceMock.DidNotReceive().WriteAccessAndRefreshTokensAsHttpOnlyCookie("REFRESH_TOKEN", Arg.Any<string>(), Arg.Any<DateTime>());
        }
    }
}
