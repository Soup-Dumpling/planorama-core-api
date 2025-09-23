using FluentValidation.TestHelper;
using Planorama.Notification.Core.UseCases.Notification.DeleteNotifications;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.Notification.Core.UnitTests.UseCases.Notification
{
    public class DeleteNotificationsCommandValidatorUnitTests
    {
        private readonly DeleteNotificationsCommandValidator validator;

        public DeleteNotificationsCommandValidatorUnitTests() 
        {
            validator = new DeleteNotificationsCommandValidator();
        }

        [Fact]
        public async Task ValidDeleteIndividualNotificationCommand()
        {
            var command = new DeleteNotificationsCommand(Guid.NewGuid(), "deleteId");
            var result = await validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task InvalidDeleteIndividualNotificationCommand()
        {
            var command = new DeleteNotificationsCommand(Guid.NewGuid(), string.Empty);
            var result = await validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.DeleteId);
        }

        [Fact]
        public async Task ValidDeleteMultipleNotificationsCommand()
        {
            var command = new DeleteNotificationsCommand("deleteId");
            var result = await validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task InvalidDeleteMultipleNotificationsCommand()
        {
            var command = new DeleteNotificationsCommand(string.Empty);
            var result = await validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.DeleteId);
        }
    }
}
