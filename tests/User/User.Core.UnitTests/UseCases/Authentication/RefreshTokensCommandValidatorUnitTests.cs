using FluentValidation.TestHelper;
using Planorama.User.Core.UseCases.Authentication.RefreshTokens;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.Core.UnitTests.UseCases.Authentication
{
    public class RefreshTokensCommandValidatorUnitTests
    {
        private readonly RefreshTokensCommandValidator validator;

        public RefreshTokensCommandValidatorUnitTests() 
        {
            validator = new RefreshTokensCommandValidator();
        }

        [Fact]
        public async Task ValidCommand()
        {
            var command = new RefreshTokensCommand();
            var result = await validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
