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
            var command = new RefreshTokensCommand("WTkh8E7Zgq/l8sqs7yaCUnWXROXyBejV5khykyZlZzoYrGiulKGNqWcwRX5u/WUxWEeXt4M2QeMcImWbw8PlSA==");
            var result = await validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task InvalidCommand()
        {
            var command = new RefreshTokensCommand(string.Empty);
            var result = await validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
        }
    }
}
