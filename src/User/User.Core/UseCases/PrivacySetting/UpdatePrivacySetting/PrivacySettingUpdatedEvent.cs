using System;

namespace Planorama.User.Core.UseCases.PrivacySetting.UpdatePrivacySetting
{
    public record PrivacySettingUpdatedEvent(Guid Id, bool IsPrivate);
}
