using System;

namespace Planorama.User.Core.UseCases.Authentication.RefreshTokens
{
    public record TokensUpdatedEvent(Guid Id, string RefreshToken, DateTime RefreshTokenExpiresAtUtc);
}
