using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Planorama.User.Core.UseCases.User.GetLoggedInUser;
using System;
using System.Threading.Tasks;

namespace Planorama.User.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly IMediator mediator;

        public UserController(ILogger<UserController> logger, IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("logged-in")]
        public async Task<GetLoggedInUserViewModel> GetLoggedInUser()
        {
            var request = new GetLoggedInUserQuery();
            var response = await mediator.Send(request);
            return response;
        }
    }
}
