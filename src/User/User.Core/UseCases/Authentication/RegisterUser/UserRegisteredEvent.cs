using System;

namespace Planorama.User.Core.UseCases.Authentication.RegisterUser
{
    public record UserRegisteredEvent(Guid Id, string FirstName, string LastName);
}
