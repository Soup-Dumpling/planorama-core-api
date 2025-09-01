using NSubstitute;
using Planorama.User.Core.Context;
using Planorama.User.Core.Exceptions;
using Planorama.User.Core.Services;
using Planorama.User.Core.UseCases.Authentication.LogoutUser;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.Core.UnitTests.UseCases.Authentication
{
    public class LogoutUserCommandHandlerUnitTests
    {
        private readonly LogoutUserCommandHandler logoutUserCommandHandler;
        private IJwtService jwtServiceMock = Substitute.For<IJwtService>();
        private ILogoutUserRepository logoutUserRepositoryMock = Substitute.For<ILogoutUserRepository>();
        private IUserContext userContextMock = Substitute.For<IUserContext>();

        public LogoutUserCommandHandlerUnitTests() 
        {
            logoutUserCommandHandler = new LogoutUserCommandHandler(jwtServiceMock, logoutUserRepositoryMock, userContextMock);
        }

        [Fact]
        public async Task ValidCommand()
        {
            //Arrange
            var command = new LogoutUserCommand(Guid.NewGuid());
            var userLoggedOutEvent = new UserLoggedOutEvent(command.UserId);
            logoutUserRepositoryMock.CheckIfUserCredentialExistsAsync(Arg.Any<Guid>()).Returns(Task.FromResult(true));
            userContextMock.UserName.Returns("user.testing@outlook.com");
            logoutUserRepositoryMock.ReplaceUserCredentialAsync(Arg.Any<Guid>(), Arg.Any<string>()).Returns(Task.FromResult(userLoggedOutEvent));

            //Act
            var result = await logoutUserCommandHandler.Handle(command, CancellationToken.None);

            //Assert
            Assert.NotEqual(string.Empty, result);
            await logoutUserRepositoryMock.Received().CheckIfUserCredentialExistsAsync(command.UserId);
            await logoutUserRepositoryMock.Received().ReplaceUserCredentialAsync(command.UserId, "user.testing@outlook.com");
            jwtServiceMock.Received().ExpireTokensFromHttpOnlyCookie();
        }

        [Fact]
        public async Task UserCredentialNotFound()
        {
            //Arrange
            var command = new LogoutUserCommand(Guid.NewGuid());
            logoutUserRepositoryMock.CheckIfUserCredentialExistsAsync(Arg.Any<Guid>()).Returns(Task.FromResult(false));

            //Act and Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await logoutUserCommandHandler.Handle(command, CancellationToken.None));
            await logoutUserRepositoryMock.Received().CheckIfUserCredentialExistsAsync(command.UserId);
            await logoutUserRepositoryMock.DidNotReceive().ReplaceUserCredentialAsync(Arg.Any<Guid>(), Arg.Any<string>());
            jwtServiceMock.DidNotReceive().ExpireTokensFromHttpOnlyCookie();
        }

        [Fact]
        public async Task UserDoesNotMatch()
        {
            //Arrange
            var command = new LogoutUserCommand(Guid.NewGuid());
            var userLoggedOutEvent = new UserLoggedOutEvent(command.UserId);
            logoutUserRepositoryMock.CheckIfUserCredentialExistsAsync(Arg.Any<Guid>()).Returns(Task.FromResult(true));
            userContextMock.UserName.Returns("user.testing@outlook.com");
            logoutUserRepositoryMock.ReplaceUserCredentialAsync(Arg.Any<Guid>(), Arg.Any<string>()).Returns(Task.FromResult(new UserLoggedOutEvent(Guid.NewGuid())));

            //Act and Assert
            await Assert.ThrowsAsync<AuthorizationException>(async () => await logoutUserCommandHandler.Handle(command, CancellationToken.None));
            await logoutUserRepositoryMock.Received().CheckIfUserCredentialExistsAsync(command.UserId);
            await logoutUserRepositoryMock.Received().ReplaceUserCredentialAsync(command.UserId, "user.testing@outlook.com");
            jwtServiceMock.DidNotReceive().ExpireTokensFromHttpOnlyCookie();
        }
    }
}
