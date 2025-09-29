using MediatR;
using Planorama.User.Core.Context;
using Planorama.User.Core.Exceptions;
using Planorama.User.Core.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Planorama.User.Core.UseCases.Authentication.LogoutUser
{
    public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, string>
    {
        private readonly IJwtService jwtService;
        private readonly ILogoutUserRepository logoutUserRepository;
        private readonly IUserContext userContext;

        public LogoutUserCommandHandler(IJwtService jwtService, ILogoutUserRepository logoutUserRepository, IUserContext userContext)
        {
            this.jwtService = jwtService;
            this.logoutUserRepository = logoutUserRepository;
            this.userContext = userContext;
        }

        public async Task<string> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            var errors = new Dictionary<string, string[]>();
            var userCredentialExists = await logoutUserRepository.CheckIfUserCredentialExistsAsync(request.UserId);
            if (!userCredentialExists)
            {
                errors.Add("userId", new string[] { "User not found." });
                throw new NotFoundException(errors);
            }

            var result = await logoutUserRepository.ReplaceUserCredentialAsync(request.UserId, userContext.UserName);
            if (result.Id != request.UserId) 
            {
                throw new AuthorizationException();
            }
            jwtService.RevokeTokens();
            return $"User with Id: {result.Id} successfully logged out.";
        }
    }
}
