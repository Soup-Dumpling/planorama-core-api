using Alba;
using Alba.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Planorama.Integration.MessageBroker.Core.Abstraction;
using Planorama.Notification.API.IntegrationTests.Fakes;
using Planorama.Notification.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.Notification.API.IntegrationTests
{
    public class AppFixture : IDisposable, IAsyncLifetime
    {
        public IAlbaHost Host { get; private set; }
        private readonly InMemoryDatabaseRoot _dbRoot = new InMemoryDatabaseRoot();
        public void Dispose()
        {
            Host?.Dispose();
        }

        public async Task InitializeAsync()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            Environment.SetEnvironmentVariable("DisableMigration", "true");

            var jwtSecurityStub = new JwtSecurityStub()
                .With(JwtRegisteredClaimNames.Aud, "planorama-api")
                .With("scope", "planorama-api");

            Host = await Program
                .CreateHostBuilder(Array.Empty<string>())
                .ConfigureServices((config, services) =>
                {
                    var descriptors = services.Where(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<NotificationContext>)
                    || d.ServiceType == typeof(IEventBus)).ToList();

                    foreach (var descriptor in descriptors)
                    {
                        services.Remove(descriptor);
                    }
                    services.AddSingleton<IEventBus, FakeEventBus>();
                    services.AddDbContext<NotificationContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting", _dbRoot);
                    });
                })
                .StartAlbaAsync(jwtSecurityStub);
        }

        public async Task DisposeAsync()
        {
            await Host.StopAsync();
        }
    }

    [CollectionDefinition("Integration")]
    public class AppFixtureCollection : ICollectionFixture<AppFixture>
    {

    }
}
