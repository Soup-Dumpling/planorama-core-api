using MediatR;

namespace Planorama.User.Core.UseCases.Authentication.RefreshTokens
{
    public class RefreshTokensCommand : IRequest
    {
        public string RefreshToken { get; set; }

        public RefreshTokensCommand(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}
