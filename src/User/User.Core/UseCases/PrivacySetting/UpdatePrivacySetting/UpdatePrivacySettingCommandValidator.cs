using FluentValidation;

namespace Planorama.User.Core.UseCases.PrivacySetting.UpdatePrivacySetting
{
    public class UpdatePrivacySettingCommandValidator : AbstractValidator<UpdatePrivacySettingCommand>
    {
        public UpdatePrivacySettingCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
