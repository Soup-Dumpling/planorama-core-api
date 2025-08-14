using Alba;
using Alba.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Planorama.Integration.MessageBroker.Core.Abstraction;
using Planorama.User.API.Constants;
using Planorama.User.API.IntegrationTests.Fakes;
using Planorama.User.Core.Constants;
using Planorama.User.Core.Services;
using Planorama.User.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.API.IntegrationTests
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
                    var descriptors = services.Where(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<UserDBContext>)
                    || d.ServiceType == typeof(IHttpService)
                    || d.ServiceType == typeof(IEventBus)).ToList();

                    foreach (var descriptor in descriptors)
                    {
                        services.Remove(descriptor);
                    }
                    services.AddScoped<IHttpService, FakeHttpService>();
                    services.AddSingleton<IEventBus, FakeEventBus>();
                    services.AddDbContext<UserDBContext>(options =>
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
