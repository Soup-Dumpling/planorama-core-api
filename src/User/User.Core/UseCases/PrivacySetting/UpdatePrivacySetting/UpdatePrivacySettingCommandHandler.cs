using MediatR;
using Planorama.User.Core.Context;
using Planorama.User.Core.Exceptions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Planorama.User.Core.UseCases.PrivacySetting.UpdatePrivacySetting
{
    public class UpdatePrivacySettingCommandHandler : IRequestHandler<UpdatePrivacySettingCommand>
    {
        private readonly IUpdatePrivacySettingRepository updatePrivacySettingRepository;
        private readonly IUserContext userContext;

        public UpdatePrivacySettingCommandHandler(IUpdatePrivacySettingRepository updatePrivacySettingRepository, IUserContext userContext)
        {
            this.updatePrivacySettingRepository = updatePrivacySettingRepository;
            this.userContext = userContext;
        }

        public async Task Handle(UpdatePrivacySettingCommand request, CancellationToken cancellationToken)
        {
            var errors = new Dictionary<string, string[]>();
            var userExists = await updatePrivacySettingRepository.CheckIfUserExists(request.UserId);
            if (!userExists)
            {
                errors.Add("userId", new string[] { "User not found." });
                throw new NotFoundException(errors);
            }

            var userId = await updatePrivacySettingRepository.GetUserIdByEmailAsync(userContext.UserName);
            if (userId != request.UserId)
            {
                throw new AuthorizationException();
            }

            await updatePrivacySettingRepository.UpdatePrivacySettingAsync(request.UserId, request.IsPrivate, userContext.UserName);
            return;
        }
    }
}
