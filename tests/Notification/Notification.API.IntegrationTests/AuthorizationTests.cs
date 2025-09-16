using Alba;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.Notification.API.IntegrationTests
{
    [Collection("Integration")]
    public class AuthorizationTests
    {
        private readonly IAlbaHost host;

        public AuthorizationTests(AppFixture fixture)
        {
            host = fixture.Host;
        }

        [Theory]
        [InlineData("/api/notification")]
        public async Task GetWithNoRoleReturnsForbidden(string url)
        {
            var response = await host.Scenario(_ =>
            {
                _.Get.Url(url);
                _.StatusCodeShouldBe(403);
            });
        }
    }
}
