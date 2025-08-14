using MediatR;
using Planorama.User.Core.Context;
using Planorama.User.Core.Exceptions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Planorama.User.Core.UseCases.User.GetLoggedInUser
{
    public class GetLoggedInUserQueryHandler : IRequestHandler<GetLoggedInUserQuery, GetLoggedInUserViewModel>
    {
        private readonly IGetLoggedInUserRepository getLoggedInUserRepository;
        private readonly IUserContext userContext;

        public GetLoggedInUserQueryHandler(IGetLoggedInUserRepository getLoggedInUserRepository, IUserContext userContext)
        {
            this.getLoggedInUserRepository = getLoggedInUserRepository;
            this.userContext = userContext;
        }

        public async Task<GetLoggedInUserViewModel> Handle(GetLoggedInUserQuery request, CancellationToken cancellationToken)
        {
            var errors = new Dictionary<string, string[]>();
            if (!userContext.IsLoggedIn())
            {
                throw new AuthorizationException();
            }

            var result = await getLoggedInUserRepository.GetLoggedInUserByEmail(userContext.UserName);
            if (result == null) 
            {
                errors.Add("emailAddress", new string[] { "User not found." });
                throw new NotFoundException(errors);
            }
            return result;
        }
    }
}
