using MediatR;
using System;

namespace Planorama.User.Core.UseCases.Authentication.LogoutUser
{
    public class LogoutUserCommand : IRequest<string>
    {
        public Guid UserId { get; set; }

        public LogoutUserCommand(Guid userId) 
        {
            UserId = userId;
        }
    }
}
