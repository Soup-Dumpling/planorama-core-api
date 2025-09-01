using FluentValidation.TestHelper;
using Planorama.User.Core.UseCases.Authentication.RegisterUser;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.Core.UnitTests.UseCases.Authentication
{
    public class RegisterUserCommandValidatorUnitTests
    {
        private readonly RegisterUserCommandValidator validator;

        public RegisterUserCommandValidatorUnitTests()
        {
            validator = new RegisterUserCommandValidator();
        }

        [Fact]
        public async Task ValidCommand()
        {
            var command = new RegisterUserCommand("firstName", "lastName", "user.testing@outlook.com", "Password1!");
            var result = await validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task InvalidCommand()
        {
            var command = new RegisterUserCommand(string.Empty, string.Empty, string.Empty, string.Empty);
            var result = await validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
            result.ShouldHaveValidationErrorFor(x => x.LastName);
            result.ShouldHaveValidationErrorFor(x => x.EmailAddress);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public async Task PasswordWithoutUppercase()
        {
            var command = new RegisterUserCommand("firstName", "lastName", "user.testing@outlook.com", "password1!");
            var result = await validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public async Task PasswordWithoutLowercase()
        {
            var command = new RegisterUserCommand("firstName", "lastName", "user.testing@outlook.com", "PASSWORD1!");
            var result = await validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public async Task PasswordWithoutNumber()
        {
            var command = new RegisterUserCommand("firstName", "lastName", "user.testing@outlook.com", "Password!");
            var result = await validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public async Task PasswordWithoutSymbol()
        {
            var command = new RegisterUserCommand("firstName", "lastName", "user.testing@outlook.com", "Password");
            var result = await validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
    }
}
