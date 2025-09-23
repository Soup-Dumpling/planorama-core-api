using MediatR;
using Planorama.Notification.Core.Context;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Planorama.Notification.Core.UseCases.Notification.GetNotifications
{
    public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, IEnumerable<GetNotificationsViewModel>>
    {
        private readonly IGetNotificationsRepository getNotificationsRepository;
        private readonly IUserContext userContext;

        public GetNotificationsQueryHandler(IGetNotificationsRepository getNotificationsRepository, IUserContext userContext)
        {
            this.getNotificationsRepository = getNotificationsRepository;
            this.userContext = userContext;
        }

        public async Task<IEnumerable<GetNotificationsViewModel>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var result = await getNotificationsRepository.GetNotificationsByEmailAsync(userContext.UserName);
            return result;
        }
    }
}
