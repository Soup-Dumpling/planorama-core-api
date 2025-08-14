using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Planorama.User.Core.UseCases.Authentication.RegisterUser;
using Planorama.User.Core.UseCases.Authentication.LoginUser;
using Planorama.User.Core.UseCases.Authentication.RefreshTokens;
using Planorama.User.Core.UseCases.Authentication.LogoutUser;
using Planorama.User.Core.UseCases.User.GetLoggedInUser;
using Planorama.User.Core.UseCases.PrivacySetting.GetPrivacySetting;
using Planorama.User.Core.UseCases.PrivacySetting.UpdatePrivacySetting;
using Planorama.User.Infrastructure.Repository.Authentication;
using Planorama.User.Infrastructure.Repository.User;
using Planorama.User.Infrastructure.Repository.PrivacySetting;
using System.Reflection;

namespace Planorama.User.Infrastructure
{
    public static class Startup
    {
        public static void AddInfrastructureBindings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UserDBContext>(opt =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                opt.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                });
            });

            services.AddScoped<IRegisterUserRepository, RegisterUserRepository>();
            services.AddScoped<ILoginUserRepository, LoginUserRepository>();
            services.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();
            services.AddScoped<ILogoutUserRepository, LogoutUserRepository>();
            services.AddScoped<IGetLoggedInUserRepository, GetLoggedInUserRepository>();
            services.AddScoped<IGetPrivacySettingRepository, GetPrivacySettingRepository>();
            services.AddScoped<IUpdatePrivacySettingRepository, UpdatePrivacySettingRepository>();
        }
    }
}
