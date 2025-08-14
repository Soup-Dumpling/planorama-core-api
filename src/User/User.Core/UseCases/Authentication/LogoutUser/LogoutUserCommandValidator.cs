using FluentValidation;

namespace Planorama.User.Core.UseCases.Authentication.LogoutUser
{
    public class LogoutUserCommandValidator : AbstractValidator<LogoutUserCommand>
    {
        public LogoutUserCommandValidator() 
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
