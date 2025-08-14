using MediatR;
using System;

namespace Planorama.User.Core.UseCases.PrivacySetting.UpdatePrivacySetting
{
    public class UpdatePrivacySettingCommand : IRequest
    {
        public Guid UserId { get; set; }
        public bool IsPrivate { get; set; }

        public UpdatePrivacySettingCommand(Guid userId, bool isPrivate)
        {
            UserId = userId;
            IsPrivate = isPrivate;
        }
    }
}
