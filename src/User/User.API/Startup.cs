using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Planorama.Integration.MessageBroker.RabbitMq;
using Planorama.User.API.Filters;
using Planorama.User.API.Middleware;
using Planorama.User.Core.Constants;
using Planorama.User.Core.Context;
using Planorama.User.Core.Services;
using Planorama.User.Core.UseCases.Authentication.RegisterUser;
using Planorama.User.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Planorama.User.API
{
    public class Startup
    {
        readonly string AllowSpecificOrigins = "_allowSpecificOrigins";
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this.env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowSpecificOrigins, builder =>
                {
                    builder.WithOrigins(Configuration.GetValue<string>("Cors:AllowedOrigins")?.Split(','));
                });
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorPipelineBehavior<,>));

            AssemblyScanner.FindValidatorsInAssembly(typeof(RegisterUserCommandValidator).Assembly)
                .ForEach(item => services.AddScoped(item.InterfaceType, item.ValidatorType));

            services.AddControllers()
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Planorama Api", Version = "v1" });
                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);
            });

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration.GetValue<string>("Jwt:Issuer"),
                    ValidAudience = Configuration.GetValue<string>("Jwt:Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("Jwt:Secret")))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["ACCESS_TOKEN"];
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy(Constants.Policies.UserPolicy, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(c => c.User.Claims.Any(x => x.Type == "scope" && x.Value.Contains("planorama-api")));
                    policy.RequireRole(Roles.UserRole, Roles.MemberRole, Roles.ModeratorRole, Roles.AdminRole);
                });
                opt.AddPolicy(Constants.Policies.MemberPolicy, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(c => c.User.Claims.Any(x => x.Type == "scope" && x.Value.Contains("planorama-api")));
                    policy.RequireRole(Roles.MemberRole, Roles.ModeratorRole, Roles.AdminRole);
                });
                opt.AddPolicy(Constants.Policies.ModeratorPolicy, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(c => c.User.Claims.Any(x => x.Type == "scope" && x.Value.Contains("planorama-api")));
                    policy.RequireRole(Roles.ModeratorRole, Roles.AdminRole);
                });
                opt.AddPolicy(Constants.Policies.AdminPolicy, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(c => c.User.Claims.Any(x => x.Type == "scope" && x.Value.Contains("planorama-api")));
                    policy.RequireRole(Roles.AdminRole);
                });
            });

            services.AddHealthChecks();
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddInfrastructureBindings(Configuration);
            services.AddRabbitMq(Configuration);
            services.AddHttpClient<IHttpService, HttpService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            bool disableMigration = Configuration.GetValue<bool>("DisableMigration");
            if (!disableMigration)
            {
                MigrateDatabase(app);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1");
                });
            }

            app.AddExceptionHandling();

            app.UseRouting();

            app.UseCors(AllowSpecificOrigins);
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/api/health");
                endpoints.MapControllers().RequireAuthorization(Constants.Policies.UserPolicy);
            });
        }

        private void MigrateDatabase(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UserDBContext>();
            context.Database.Migrate();
        }
    }
}
