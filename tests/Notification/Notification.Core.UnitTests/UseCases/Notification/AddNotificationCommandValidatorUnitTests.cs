using FluentValidation.TestHelper;
using Planorama.Notification.Core.UseCases.Notification.AddNotification;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.Notification.Core.UnitTests.UseCases.Notification
{
    public class AddNotificationCommandValidatorUnitTests
    {
        private readonly AddNotificationCommandValidator validator;

        public AddNotificationCommandValidatorUnitTests()
        {
            validator = new AddNotificationCommandValidator();
        }

        [Fact]
        public async Task ValidCommand()
        {
            var command = new AddNotificationCommand(Guid.NewGuid(), "userEmail", "title", Enums.NotificationType.SquadInvitation, "content", Guid.NewGuid(), "deleteId");
            var result = await validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task InvalidCommand()
        {
            var command = new AddNotificationCommand(Guid.Empty, string.Empty, string.Empty, 0, string.Empty, Guid.Empty, string.Empty);
            var result = await validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.UserId);
            result.ShouldHaveValidationErrorFor(x => x.UserEmail);
            result.ShouldHaveValidationErrorFor(x => x.Title);
            result.ShouldHaveValidationErrorFor(x => x.Type);
            result.ShouldHaveValidationErrorFor(x => x.Content);
            result.ShouldHaveValidationErrorFor(x => x.ReferenceId);
            result.ShouldHaveValidationErrorFor(x => x.DeleteId);
        }
    }
}
