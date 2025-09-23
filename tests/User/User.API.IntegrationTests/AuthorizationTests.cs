using Alba;
using Planorama.User.Core.Constants;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.API.IntegrationTests
{
    [Collection("Integration")]
    public class AuthorizationTests(AppFixture fixture)
    {
        private readonly IAlbaHost host = fixture.Host;

        [Theory]
        [InlineData("/api/user/logged-in")]
        [InlineData("/api/privacysetting")]
        public async Task GetWithNoRoleReturnsForbidden(string url)
        {
            var response = await host.Scenario(_ =>
            {
                _.Get.Url(url);
                _.StatusCodeShouldBe(403);
            });
        }

        [Theory]
        [InlineData("/api/authentication/refresh")]
        [InlineData("/api/authentication/logout")]
        public async Task PostWithNoRoleReturnsForbidden(string url)
        {
            var response = await host.Scenario(_ =>
            {
                _.Post.Url(url);
                _.StatusCodeShouldBe(403);
            });
        }

        [Theory]
        [InlineData("/api/privacysetting")]
        public async Task PutWithNoRoleReturnsForbidden(string url)
        {
            var response = await host.Scenario(_ =>
            {
                _.Put.Url(url);
                _.StatusCodeShouldBe(403);
            });
        }
    }
}
