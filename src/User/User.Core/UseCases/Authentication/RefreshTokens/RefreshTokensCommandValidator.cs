using FluentValidation;

namespace Planorama.User.Core.UseCases.Authentication.RefreshTokens
{
    public class RefreshTokensCommandValidator : AbstractValidator<RefreshTokensCommand>
    {
        public RefreshTokensCommandValidator() 
        {
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }
}
