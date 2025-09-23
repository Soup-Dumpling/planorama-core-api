using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Planorama.User.API.Models.Authentication;
using Planorama.User.Core.UseCases.Authentication.LoginUser;
using Planorama.User.Core.UseCases.Authentication.LogoutUser;
using Planorama.User.Core.UseCases.Authentication.RefreshTokens;
using Planorama.User.Core.UseCases.Authentication.RegisterUser;
using System;
using System.Threading.Tasks;

namespace Planorama.User.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> logger;
        private readonly IMediator mediator;

        public AuthenticationController(ILogger<AuthenticationController> logger, IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<string> RegisterUser([FromBody] RegisterUserRequest model)
        {
            var request = new RegisterUserCommand(model.FirstName, model.LastName, model.EmailAddress, model.Password);
            var response = await mediator.Send(request);
            return response;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<string> LoginUser([FromBody] LoginUserRequest model)
        {
            var request = new LoginUserCommand(model.EmailAddress, model.Password);
            var response = await mediator.Send(request);
            return response;
        }

        [HttpPost("refresh")]
        public async Task RefreshTokens([FromBody] RefreshTokensRequest model)
        {
            var request = new RefreshTokensCommand(model.RefreshToken);
            await mediator.Send(request);
        }

        [HttpPost("logout")]
        public async Task<string> LogoutUser([FromBody] LogoutUserRequest model)
        {
            var request = new LogoutUserCommand(model.UserId);
            var response = await mediator.Send(request);
            return response;
        }
    }
}
