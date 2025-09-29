using Alba;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Identity;
using Planorama.User.API.IntegrationTests.Extentions;
using Planorama.User.API.Models.Authentication;
using Planorama.User.Core.Constants;
using Planorama.User.Core.Models;
using Planorama.User.Core.UseCases.User.GetLoggedInUser;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.API.IntegrationTests.Controllers
{
    [Collection("Integration")]
    public class AuthenticationControllerTests(AppFixture fixture)
    {
        private readonly IAlbaHost host = fixture.Host;

        [Fact]
        public async Task ValidRegisterUserPost()
        {
            //Arrange
            var request = Builder<RegisterUserRequest>.CreateNew()
                .Do(x =>
                {
                    x.FirstName = "firstName";
                    x.LastName = "lastName";
                    x.EmailAddress = "testEmail@test.com";
                    x.Password = "Password1!";
                }).Build();

            host.WithEmptyDatabase(async context =>
            {
                await context.SaveChangesAsync();
            });

            //Act
            var response = await host.Scenario(_ =>
            {
                _.Post.Json(request).ToUrl("/api/authentication/register");
                _.StatusCodeShouldBeOk();
            });

            //Assert
            var result = response.ReadAsJson<string>();
            Assert.NotEqual(result, string.Empty);

            var loggedInUserResponse = await host.Scenario(_ =>
            {
                _.WithClaim(new Claim("email", "testEmail@test.com"));
                _.WithClaim(new Claim("role", Roles.UserRole));
                _.Get.Url($"/api/user/logged-in");
                _.StatusCodeShouldBeOk();
            });

            var loggedInUserResult = loggedInUserResponse.ReadAsJson<GetLoggedInUserViewModel>();

            Assert.Equal(loggedInUserResult.FirstName, request.FirstName);
            Assert.Equal(loggedInUserResult.LastName, request.LastName);
            Assert.Equal(loggedInUserResult.EmailAddress, request.EmailAddress);
        }

        [Fact]
        public async Task ValidLoginUserPost()
        {
            //Arrange
            var fakeRoles = Builder<Role>.CreateNew()
                .Do(x =>
                {
                    x.Name = "User";
                    x.IdentityName = Roles.UserRole;
                }).Build();
            var fakeUser = Builder<Core.Models.User>.CreateNew()
                .Do(x =>
                {
                    x.Id = Guid.NewGuid();
                    x.FirstName = "firstName";
                    x.LastName = "lastName";
                }).Build();
            var fakeUserCredential = Builder<UserCredential>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.EmailAddress = "testEmail@test.com";
                }).Build();
            fakeUserCredential.HashedPassword = new PasswordHasher<UserCredential>().HashPassword(fakeUserCredential, "Password1!");
            var fakeUserRoles = Builder<UserRole>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.RoleId = fakeRoles.Id;
                }).Build();

            host.WithEmptyDatabase(async context =>
            {
                context.Add(fakeRoles);
                context.Add(fakeUser);
                context.Add(fakeUserCredential);
                context.Add(fakeUserRoles);
                await context.SaveChangesAsync();
            });

            var request = Builder<LoginUserRequest>.CreateNew()
                .Do(x =>
                {
                    x.EmailAddress = "testEmail@test.com";
                    x.Password = "Password1!";
                }).Build();

            //Act
            var response = await host.Scenario(_ =>
            {
                _.Post.Json(request).ToUrl("/api/authentication/login");
                _.StatusCodeShouldBeOk();
            });

            //Assert
            var result = response.ReadAsJson<string>();
            Assert.NotEqual(result, string.Empty);
            Assert.True(response.Context.Response.Headers.ContainsKey("Set-Cookie"));
            var accessToken = response.Context.Response.Headers.SetCookie.FirstOrDefault(x => x.StartsWith("ACCESS_TOKEN"));
            Assert.NotNull(accessToken);
            var refreshToken = response.Context.Response.Headers.SetCookie.FirstOrDefault(x => x.StartsWith("REFRESH_TOKEN"));
            Assert.NotNull(refreshToken);

            var loggedInUserResponse = await host.Scenario(_ =>
            {
                _.WithClaim(new Claim("email", "testEmail@test.com"));
                _.WithClaim(new Claim("role", Roles.UserRole));
                _.Get.Url($"/api/user/logged-in");
                _.StatusCodeShouldBeOk();
            });

            var loggedInUserResult = loggedInUserResponse.ReadAsJson<GetLoggedInUserViewModel>();

            Assert.Equal(loggedInUserResult.Id, fakeUser.Id);
            Assert.Equal(loggedInUserResult.FirstName, fakeUser.FirstName);
            Assert.Equal(loggedInUserResult.LastName, fakeUser.LastName);
            Assert.Equal(loggedInUserResult.EmailAddress, request.EmailAddress);
        }

        [Fact]
        public async Task ValidRefreshTokensPost()
        {
            //Arrange
            var fakeUser = Builder<Core.Models.User>.CreateNew()
                .Do(x =>
                {
                    x.Id = Guid.NewGuid();
                    x.FirstName = "firstName";
                    x.LastName = "lastName";
                }).Build();
            var fakeRoles = Builder<Role>.CreateNew()
                .Do(x =>
                {
                    x.Name = "User";
                    x.IdentityName = Roles.UserRole;
                }).Build();
            var fakeUserCredential = Builder<UserCredential>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.EmailAddress = "testEmail@test.com";
                    x.RefreshToken = "Hw4y06bskqzxz4YIYoYFp8xcqw+p6hyonncU7hyamWkL/seWkibsPMnMH1nMVo4rDaAyHD9a+mUGYkRvNwPlOA==";
                    x.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(1);
                }).Build();
            fakeUserCredential.HashedPassword = new PasswordHasher<UserCredential>().HashPassword(fakeUserCredential, "Password1!");
            var fakeUserRoles = Builder<UserRole>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.RoleId = fakeRoles.Id;
                }).Build();

            host.WithEmptyDatabase(async context =>
            {
                context.Add(fakeRoles);
                context.Add(fakeUser);
                context.Add(fakeUserCredential);
                context.Add(fakeUserRoles);
                await context.SaveChangesAsync();
            });

            //Act
            var response = await host.Scenario(_ =>
            {
                _.WithRequestHeader("Cookie", $"REFRESH_TOKEN={fakeUserCredential.RefreshToken}");
                _.Post.Url("/api/authentication/refresh");
                _.StatusCodeShouldBeOk();
            });

            //Assert
            var result = response.Context.Response.Headers.SetCookie.LastOrDefault();
            Assert.NotNull(result);
            Assert.DoesNotContain(fakeUserCredential.RefreshToken, result);
        }

        [Fact]
        public async Task ValidLogoutUserPost()
        {
            //Arrange
            var fakeUser = Builder<Core.Models.User>.CreateNew()
                .Do(x =>
                {
                    x.Id = Guid.NewGuid();
                    x.FirstName = "firstName";
                    x.LastName = "lastName";
                }).Build();
            var fakeUserCredential = Builder<UserCredential>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.EmailAddress = "testEmail@test.com";
                    x.RefreshToken = "Hw4y06bskqzxz4YIYoYFp8xcqw+p6hyonncU7hyamWkL/seWkibsPMnMH1nMVo4rDaAyHD9a+mUGYkRvNwPlOA==";
                    x.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(1);
                }).Build();
            fakeUserCredential.HashedPassword = new PasswordHasher<UserCredential>().HashPassword(fakeUserCredential, "Password1!");

            host.WithEmptyDatabase(async context =>
            {
                context.Add(fakeUser);
                context.Add(fakeUserCredential);
                await context.SaveChangesAsync();
            });

            var request = Builder<LogoutUserRequest>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                }).Build();

            //Act
            var response = await host.Scenario(_ =>
            {
                _.WithClaim(new Claim("email", "testEmail@test.com"));
                _.WithClaim(new Claim("role", Roles.UserRole));
                _.Post.Json(request).ToUrl("/api/authentication/logout");
                _.StatusCodeShouldBeOk();
            });

            //Assert
            var result = response.ReadAsJson<string>();
            Assert.NotEqual(result, string.Empty);
            Assert.False(response.Context.Response.Headers.ContainsKey("Set-Cookie"));
        }
    }
}
