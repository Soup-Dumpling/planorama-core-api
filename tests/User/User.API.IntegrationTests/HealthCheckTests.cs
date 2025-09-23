using Alba;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.API.IntegrationTests
{
    [Collection("Integration")]
    public class HealthCheckTests(AppFixture fixture)
    {
        private readonly IAlbaHost host = fixture.Host;

        [Fact]
        public async Task Healthy()
        {
            await host.Scenario(_ =>
            {
                _.Get.Url("/api/health");
                _.ContentShouldBe("Healthy");
            });
        }
    }
}
