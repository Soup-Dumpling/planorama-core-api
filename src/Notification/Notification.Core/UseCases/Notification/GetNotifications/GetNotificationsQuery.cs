using MediatR;
using System.Collections.Generic;

namespace Planorama.Notification.Core.UseCases.Notification.GetNotifications
{
    public class GetNotificationsQuery : IRequest<IEnumerable<GetNotificationsViewModel>>
    {
    }
}
