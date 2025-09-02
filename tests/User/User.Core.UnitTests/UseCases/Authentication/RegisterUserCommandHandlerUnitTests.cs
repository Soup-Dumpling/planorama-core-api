using NSubstitute;
using Planorama.User.Core.Constants;
using Planorama.User.Core.Exceptions;
using Planorama.User.Core.Models;
using Planorama.User.Core.UseCases.Authentication.RegisterUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.Core.UnitTests.UseCases.Authentication
{
    public class RegisterUserCommandHandlerUnitTests
    {
        private readonly RegisterUserCommandHandler registerUserCommandHandler;
        private IRegisterUserRepository registerUserRepositoryMock = Substitute.For<IRegisterUserRepository>();

        public RegisterUserCommandHandlerUnitTests() 
        {
            registerUserCommandHandler = new RegisterUserCommandHandler(registerUserRepositoryMock);
        }

        [Fact]
        public async Task ValidCommand()
        {
            //Arrange
            var command = new RegisterUserCommand("firstName", "lastName", "user.testing@outlook.com", "Password1!");
            var userRegisteredEvent = new UserRegisteredEvent(Guid.NewGuid(), command.FirstName, command.LastName);
            registerUserRepositoryMock.CheckIfUserExistsAsync(Arg.Any<string>()).Returns(Task.FromResult(false));
            registerUserRepositoryMock.RegisterUserAsync(Arg.Any<Models.User>(), Arg.Any<UserCredential>(), Arg.Any<UserPrivacySetting>(), Arg.Any<IEnumerable<string>>(), Arg.Any<string>()).Returns(Task.FromResult(userRegisteredEvent));

            //Act
            var result = await registerUserCommandHandler.Handle(command, CancellationToken.None);

            //Assert
            Assert.NotEqual(string.Empty, result);
            await registerUserRepositoryMock.Received().CheckIfUserExistsAsync(command.EmailAddress);
            await registerUserRepositoryMock.Received().RegisterUserAsync(Arg.Is<Models.User>(x => x.FirstName == command.FirstName && x.LastName == command.LastName), Arg.Is<UserCredential>(x => x.EmailAddress == command.EmailAddress), Arg.Is<UserPrivacySetting>(x => x.IsPrivate == false), Arg.Is<IEnumerable<string>>(x => x.Any(r => r == Roles.UserRole)), command.EmailAddress);
        }

        [Fact]
        public async Task UserAlreadyExists()
        {
            //Arrange
            var command = new RegisterUserCommand("firstName", "lastName", "user.testing@outlook.com", "Password1!");
            registerUserRepositoryMock.CheckIfUserExistsAsync(Arg.Any<string>()).Returns(Task.FromResult(true));

            //Act and Assert
            await Assert.ThrowsAsync<ValidationException>(async () => await registerUserCommandHandler.Handle(command, CancellationToken.None));
            await registerUserRepositoryMock.Received().CheckIfUserExistsAsync(command.EmailAddress);
            await registerUserRepositoryMock.DidNotReceive().RegisterUserAsync(Arg.Any<Models.User>(), Arg.Any<UserCredential>(), Arg.Any<UserPrivacySetting>(), Arg.Any<IEnumerable<string>>(), Arg.Any<string>());
        }
    }
}
