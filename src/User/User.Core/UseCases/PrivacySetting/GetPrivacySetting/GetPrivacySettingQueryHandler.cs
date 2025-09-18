using MediatR;
using Planorama.User.Core.Context;
using Planorama.User.Core.Exceptions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Planorama.User.Core.UseCases.PrivacySetting.GetPrivacySetting
{
    public class GetPrivacySettingQueryHandler : IRequestHandler<GetPrivacySettingQuery, GetPrivacySettingViewModel>
    {
        private readonly IGetPrivacySettingRepository getPrivacySettingRepository;
        private readonly IUserContext userContext;

        public GetPrivacySettingQueryHandler(IGetPrivacySettingRepository getPrivacySettingRepository, IUserContext userContext)
        {
            this.getPrivacySettingRepository = getPrivacySettingRepository;
            this.userContext = userContext;
        }

        public async Task<GetPrivacySettingViewModel> Handle(GetPrivacySettingQuery request, CancellationToken cancellationToken)
        {
            var errors = new Dictionary<string, string[]>();
            if (!userContext.IsLoggedIn())
            {
                throw new AuthorizationException();
            }

            var userId = await getPrivacySettingRepository.GetUserIdByEmailAsync(userContext.UserName);
            if (userId == null)
            {
                errors.Add("userId", new string[] { "User not found." });
                throw new NotFoundException(errors);
            }

            var result = await getPrivacySettingRepository.GetUserPrivacySettingByIdAsync(userId.Value);
            return result;
        }
    }
}
