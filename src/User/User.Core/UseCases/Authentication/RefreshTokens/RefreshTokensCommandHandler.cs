using MediatR;
using Planorama.User.Core.Constants;
using Planorama.User.Core.Context;
using Planorama.User.Core.Exceptions;
using Planorama.User.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planorama.User.Core.UseCases.Authentication.RefreshTokens
{
    public class RefreshTokensCommandHandler : IRequestHandler<RefreshTokensCommand>
    {
        private readonly IJwtService jwtService;
        private readonly IRefreshTokensRepository refreshTokensRepository;
        private readonly IUserContext userContext;

        public RefreshTokensCommandHandler(IJwtService jwtService, IRefreshTokensRepository refreshTokensRepository, IUserContext userContext)
        {
            this.jwtService = jwtService;
            this.refreshTokensRepository = refreshTokensRepository;
            this.userContext = userContext;
        }

        public async Task Handle(RefreshTokensCommand command, CancellationToken cancellationToken)
        {
            var errors = new Dictionary<string, string[]>();
            if (!userContext.IsLoggedIn())
            {
                throw new AuthorizationException();
            }
            var userCredential = await refreshTokensRepository.FindUserCredentialByRefreshTokenAsync(command.RefreshToken);
            if (userCredential == null)
            {
                errors.Add("refreshToken", new string[] { "Unable to retrieve user for refresh token." });
                throw new RefreshTokenException(errors);
            }

            if (userCredential.RefreshTokenExpiresAtUtc < DateTime.UtcNow)
            {
                errors.Add("refreshToken", new string[] { "Refresh token is expired." });
                throw new RefreshTokenException(errors);
            }

            var userFullName = await refreshTokensRepository.GetUserFullNameByIdAsync(userCredential.UserId);

            var userRoles = await refreshTokensRepository.GetUserRolesByIdAsync(userCredential.UserId);
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

            await refreshTokensRepository.UpdateRefreshTokenAsync(userCredential.UserId, refreshToken, refreshTokenExpiresAtUtc, userContext.UserName);
            jwtService.WriteAccessAndRefreshTokensAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, tokenExpiresAtUtc);
            jwtService.WriteAccessAndRefreshTokensAsHttpOnlyCookie("REFRESH_TOKEN", refreshToken, refreshTokenExpiresAtUtc);
            return;
        }
    }
}
