using System.Threading.Tasks;

namespace Planorama.User.Core.UseCases.User.GetLoggedInUser
{
    public interface IGetLoggedInUserRepository
    {
        Task<GetLoggedInUserViewModel> GetLoggedInUserByEmail(string email);
    }
}
