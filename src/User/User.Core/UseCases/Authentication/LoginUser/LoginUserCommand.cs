using MediatR;

namespace Planorama.User.Core.UseCases.Authentication.LoginUser
{
    public class LoginUserCommand : IRequest<string>
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }

        public LoginUserCommand(string emailAddress, string password)
        {
            EmailAddress = emailAddress;
            Password = password;
        }
    }
}
