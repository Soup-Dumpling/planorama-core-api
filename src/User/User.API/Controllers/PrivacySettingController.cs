using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Planorama.User.API.Models.PrivacySetting;
using Planorama.User.Core.UseCases.PrivacySetting.GetPrivacySetting;
using Planorama.User.Core.UseCases.PrivacySetting.UpdatePrivacySetting;
using System.Threading.Tasks;

namespace Planorama.User.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrivacySettingController : ControllerBase
    {
        private readonly ILogger<PrivacySettingController> logger;
        private readonly IMediator mediator;

        public PrivacySettingController(ILogger<PrivacySettingController> logger, IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<GetPrivacySettingViewModel> GetPrivacySetting()
        {
            var request = new GetPrivacySettingQuery();
            var response = await mediator.Send(request);
            return response;
        }

        [HttpPut]
        public async Task UpdatePrivacySetting([FromBody] UpdatePrivacySettingRequest model)
        {
            var request = new UpdatePrivacySettingCommand(model.UserId, model.IsPrivate);
            await mediator.Send(request);
        }
    }
}
