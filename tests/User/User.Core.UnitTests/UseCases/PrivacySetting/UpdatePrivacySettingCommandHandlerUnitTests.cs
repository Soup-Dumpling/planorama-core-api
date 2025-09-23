using NSubstitute;
using Planorama.User.Core.Context;
using Planorama.User.Core.Exceptions;
using Planorama.User.Core.UseCases.PrivacySetting.UpdatePrivacySetting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.Core.UnitTests.UseCases.PrivacySetting
{
    public class UpdatePrivacySettingCommandHandlerUnitTests
    {
        private readonly UpdatePrivacySettingCommandHandler updatePrivacySettingCommandHandler;
        private readonly IUpdatePrivacySettingRepository updatePrivacySettingRepositoryMock = Substitute.For<IUpdatePrivacySettingRepository>();
        private readonly IUserContext userContextMock = Substitute.For<IUserContext>();

        public UpdatePrivacySettingCommandHandlerUnitTests()
        {
            updatePrivacySettingCommandHandler = new UpdatePrivacySettingCommandHandler(updatePrivacySettingRepositoryMock, userContextMock);
        }

        [Fact]
        public async Task ValidCommand()
        {
            //Arrange
            var command = new UpdatePrivacySettingCommand(Guid.NewGuid(), true);
            var privacySettingUpdatedEvent = new PrivacySettingUpdatedEvent(command.UserId, command.IsPrivate);
            userContextMock.UserName.Returns("user.testing@outlook.com");
            updatePrivacySettingRepositoryMock.CheckIfUserExists(Arg.Any<Guid>()).Returns(Task.FromResult(true));
            updatePrivacySettingRepositoryMock.GetUserIdByEmailAsync(Arg.Any<string>()).Returns(command.UserId);
            updatePrivacySettingRepositoryMock.UpdatePrivacySettingAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<string>()).Returns(Task.FromResult(privacySettingUpdatedEvent));

            //Act
            await updatePrivacySettingCommandHandler.Handle(command, CancellationToken.None);

            //Assert
            await updatePrivacySettingRepositoryMock.Received().CheckIfUserExists(command.UserId);
            await updatePrivacySettingRepositoryMock.Received().GetUserIdByEmailAsync("user.testing@outlook.com");
            await updatePrivacySettingRepositoryMock.Received().UpdatePrivacySettingAsync(command.UserId, command.IsPrivate, "user.testing@outlook.com");
        }

        [Fact]
        public async Task UserNotFound()
        {
            //Arrange
            var command = new UpdatePrivacySettingCommand(Guid.NewGuid(), true);
            updatePrivacySettingRepositoryMock.CheckIfUserExists(Arg.Any<Guid>()).Returns(Task.FromResult(false));

            //Act and Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await updatePrivacySettingCommandHandler.Handle(command, CancellationToken.None));
            await updatePrivacySettingRepositoryMock.Received().CheckIfUserExists(command.UserId);
            await updatePrivacySettingRepositoryMock.DidNotReceive().GetUserIdByEmailAsync(Arg.Any<string>());
            await updatePrivacySettingRepositoryMock.DidNotReceive().UpdatePrivacySettingAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<string>());
        }

        [Fact]
        public async Task UserDoesNotMatch()
        {
            //Arrange
            var command = new UpdatePrivacySettingCommand(Guid.NewGuid(), true);
            var privacySettingUpdatedEvent = new PrivacySettingUpdatedEvent(command.UserId, command.IsPrivate);
            userContextMock.UserName.Returns("user.testing@outlook.com");
            updatePrivacySettingRepositoryMock.CheckIfUserExists(Arg.Any<Guid>()).Returns(Task.FromResult(true));
            updatePrivacySettingRepositoryMock.GetUserIdByEmailAsync(Arg.Any<string>()).Returns(Guid.NewGuid());

            //Act and Assert
            await Assert.ThrowsAsync<AuthorizationException>(async () => await updatePrivacySettingCommandHandler.Handle(command, CancellationToken.None));
            await updatePrivacySettingRepositoryMock.Received().CheckIfUserExists(command.UserId);
            await updatePrivacySettingRepositoryMock.Received().GetUserIdByEmailAsync("user.testing@outlook.com");
            await updatePrivacySettingRepositoryMock.DidNotReceive().UpdatePrivacySettingAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<string>());
        }
    }
}
