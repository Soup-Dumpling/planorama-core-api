using Alba;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Identity;
using Planorama.User.API.IntegrationTests.Extentions;
using Planorama.User.Core.Constants;
using Planorama.User.Core.Models;
using Planorama.User.Core.UseCases.User.GetLoggedInUser;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.API.IntegrationTests.Controllers
{
    [Collection("Integration")]
    public class UserControllerTests(AppFixture fixture)
    {
        private readonly IAlbaHost host = fixture.Host;

        [Fact]
        public async Task ValidLoggedInUserGet()
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
                    x.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(5);
                }).Build();
            fakeUserCredential.HashedPassword = new PasswordHasher<UserCredential>().HashPassword(fakeUserCredential, "Password1!");

            host.WithEmptyDatabase(async context =>
            {
                context.Add(fakeUser);
                context.Add(fakeUserCredential);
                await context.SaveChangesAsync();
            });

            //Act
            var response = await host.Scenario(_ =>
            {
                _.WithClaim(new Claim("email", "testEmail@test.com"));
                _.WithClaim(new Claim("role", Roles.UserRole));
                _.Get.Url($"/api/user/logged-in");
                _.StatusCodeShouldBeOk();
            });

            //Assert
            var result = response.ReadAsJson<GetLoggedInUserViewModel>();
            Assert.NotNull(result);
            Assert.IsType<GetLoggedInUserViewModel>(result);
            Assert.Equal(result.Id, fakeUser.Id);
            Assert.Equal(result.FirstName, fakeUser.FirstName);
            Assert.Equal(result.LastName, fakeUser.LastName);
            Assert.Equal(result.EmailAddress, fakeUserCredential.EmailAddress);
        }
    }
}
