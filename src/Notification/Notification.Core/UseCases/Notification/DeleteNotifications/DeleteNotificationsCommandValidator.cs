using FluentValidation;

namespace Planorama.Notification.Core.UseCases.Notification.DeleteNotifications
{
    public class DeleteNotificationsCommandValidator : AbstractValidator<DeleteNotificationsCommand>
    {
        public DeleteNotificationsCommandValidator() 
        {
            RuleFor(x => x.DeleteId).NotEmpty();
        }
    }
}
