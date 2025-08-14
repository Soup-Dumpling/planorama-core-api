using MediatR;

namespace Planorama.User.Core.UseCases.Authentication.RegisterUser
{
    public class RegisterUserCommand : IRequest<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }

        public RegisterUserCommand(string firstName, string lastName, string emailAddress, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            Password = password;
        }
    }
}
