using System;

namespace Planorama.User.Core.UseCases.Authentication.LoginUser
{
    public record UserLoggedInEvent(Guid Id, string RefreshToken, DateTime RefreshTokenExpiresAtUtc);
}
