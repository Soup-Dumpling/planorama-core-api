using Alba;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.Notification.API.IntegrationTests
{
    [Collection("Integration")]
    public class HealthCheckTests
    {
        private readonly IAlbaHost host;

        public HealthCheckTests(AppFixture fixture)
        {
            host = fixture.Host;
        }

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
