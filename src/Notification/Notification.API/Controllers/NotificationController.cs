using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Planorama.Notification.Core.UseCases.Notification.GetNotifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planorama.Notification.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> logger;
        private readonly IMediator mediator;

        public NotificationController(ILogger<NotificationController> logger, IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<IEnumerable<GetNotificationsViewModel>> GetNotifications()
        {
            var query = new GetNotificationsQuery();
            var result = await mediator.Send(query);
            return result;
        }
    }
}
