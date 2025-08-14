using FluentValidation.TestHelper;
using Planorama.User.Core.UseCases.Authentication.LoginUser;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.Core.UnitTests.UseCases.Authentication
{
    public class LoginUserCommandValidatorUnitTests
    {
        private readonly LoginUserCommandValidator validator;

        public LoginUserCommandValidatorUnitTests() 
        {
            validator = new LoginUserCommandValidator();
        }

        [Fact]
        public async Task ValidCommand()
        {
            var command = new LoginUserCommand("user.testing@outlook.com", "Password1!");
            var result = await validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task InvalidCommand()
        {
            var command = new LoginUserCommand(string.Empty, string.Empty);
            var result = await validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.EmailAddress);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
    }
}
