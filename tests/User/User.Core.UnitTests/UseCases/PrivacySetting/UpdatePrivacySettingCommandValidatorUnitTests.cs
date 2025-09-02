using FluentValidation.TestHelper;
using Planorama.User.Core.UseCases.PrivacySetting.UpdatePrivacySetting;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.Core.UnitTests.UseCases.PrivacySetting
{
    public class UpdatePrivacySettingCommandValidatorUnitTests
    {
        private readonly UpdatePrivacySettingCommandValidator validator;

        public UpdatePrivacySettingCommandValidatorUnitTests() 
        {
            validator = new UpdatePrivacySettingCommandValidator();
        }

        [Fact]
        public async Task ValidCommand()
        {
            var command = new UpdatePrivacySettingCommand(Guid.NewGuid(), true);
            var result = await validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task InvalidCommand()
        {
            var command = new UpdatePrivacySettingCommand(Guid.Empty, true);
            var result = await validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.UserId);
        }
    }
}
