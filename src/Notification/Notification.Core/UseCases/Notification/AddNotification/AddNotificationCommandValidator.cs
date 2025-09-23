using FluentValidation;

namespace Planorama.Notification.Core.UseCases.Notification.AddNotification
{
    public class AddNotificationCommandValidator : AbstractValidator<AddNotificationCommand>
    {
        public AddNotificationCommandValidator() 
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.UserEmail).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Type).NotEmpty();
            RuleFor(x => x.Content).NotEmpty();
            RuleFor(x => x.ReferenceId).NotEmpty();
            RuleFor(x => x.DeleteId).NotEmpty();
        }
    }
}
