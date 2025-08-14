using MediatR;
using Microsoft.AspNetCore.Identity;
using Planorama.User.Core.Constants;
using Planorama.User.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planorama.User.Core.UseCases.Authentication.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, string>
    {
        private readonly IRegisterUserRepository registerUserRepository;

        public RegisterUserCommandHandler(IRegisterUserRepository registerUserRepository)
        {
            this.registerUserRepository = registerUserRepository;
        }

        public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var errors = new Dictionary<string, string[]>();
            var isDuplicate = await registerUserRepository.CheckIfUserExistsAsync(request.EmailAddress);
            if (isDuplicate)
            {
                errors.Add("emailAddress", ["User with the same email address already exist."]);
            }

            if (errors.Any())
            {
                throw new ValidationException(errors);
            }

            var user = new Models.User()
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
            };

            var userCredential = new Models.UserCredential()
            {
                UserId = user.Id,
                EmailAddress = request.EmailAddress,
            };

            var hashedPassword = new PasswordHasher<Models.UserCredential>().HashPassword(userCredential, request.Password);
            userCredential.HashedPassword = hashedPassword;

            var userPrivacySetting = new Models.UserPrivacySetting()
            {
                UserId = user.Id
            };

            var userRoles = new List<string>() { Roles.UserRole };

            var result = await registerUserRepository.RegisterUserAsync(user, userCredential, userPrivacySetting, userRoles, request.EmailAddress);
            return $"User with Id: {result.Id} successfully registered.";
        }
    }
}
