using FluentValidation.TestHelper;
using Planorama.User.Core.UseCases.Authentication.LogoutUser;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.Core.UnitTests.UseCases.Authentication
{
    public class LogoutUserCommandValidatorUnitTests
    {
        private readonly LogoutUserCommandValidator validator;

        public LogoutUserCommandValidatorUnitTests() 
        {
            validator = new LogoutUserCommandValidator();
        }

        [Fact]
        public async Task ValidCommand()
        {
            var command = new LogoutUserCommand(Guid.NewGuid());
            var result = await validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task InvalidCommand()
        {
            var command = new LogoutUserCommand(Guid.Empty);
            var result = await validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.UserId);
        }
    }
}
