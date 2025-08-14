using MediatR;

namespace Planorama.User.Core.UseCases.User.GetLoggedInUser
{
    public class GetLoggedInUserQuery : IRequest<GetLoggedInUserViewModel>
    {
    }
}
