using MediatR;
using Microsoft.AspNetCore.Identity;
using Planorama.User.Core.Constants;
using Planorama.User.Core.Exceptions;
using Planorama.User.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planorama.User.Core.UseCases.Authentication.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string>
    {
        private readonly IJwtService jwtService;
        private readonly ILoginUserRepository loginUserRepository;

        public LoginUserCommandHandler(IJwtService jwtService, ILoginUserRepository loginUserRepository)
        {
            this.jwtService = jwtService;
            this.loginUserRepository = loginUserRepository;
        }

        public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var userCredential = await loginUserRepository.FindUserCredentialByEmailAsync(request.EmailAddress);
            if (userCredential == null || new PasswordHasher<Models.UserCredential>().VerifyHashedPassword(userCredential, userCredential.HashedPassword, request.Password) == PasswordVerificationResult.Failed) 
            {
                throw new LoginFailedException();
            }

            var userFullName = await loginUserRepository.GetUserFullNameByIdAsync(userCredential.UserId);

            var userRoles = await loginUserRepository.GetUserRolesByIdAsync(userCredential.UserId);
            var existingRole = userRoles.Where(x => x == Roles.UserRole ||
            x == Roles.MemberRole ||
            x == Roles.ModeratorRole ||
            x == Roles.AdminRole);
            var roles = new List<string>() { Roles.UserRole };
            if (existingRole.Any())
            {
                roles = existingRole.ToList();
            }
            var (jwtToken, tokenExpiresAtUtc) = jwtService.GenerateJwtToken(userCredential, userFullName, roles);

            var refreshToken = jwtService.GenerateRefreshToken();
            var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);

            var result = await loginUserRepository.AddRefreshTokenAsync(userCredential.UserId, refreshToken, refreshTokenExpiresAtUtc, userCredential.EmailAddress);
            jwtService.WriteAccessAndRefreshTokensAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, tokenExpiresAtUtc);
            jwtService.WriteAccessAndRefreshTokensAsHttpOnlyCookie("REFRESH_TOKEN", refreshToken, refreshTokenExpiresAtUtc);
            return $"User with Id: {result.Id} successfully logged in.";
        }
    }
}
