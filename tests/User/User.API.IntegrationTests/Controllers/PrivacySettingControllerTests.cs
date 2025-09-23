using Alba;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Identity;
using Planorama.User.API.IntegrationTests.Extentions;
using Planorama.User.API.Models.PrivacySetting;
using Planorama.User.Core.Constants;
using Planorama.User.Core.Models;
using Planorama.User.Core.UseCases.PrivacySetting.GetPrivacySetting;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.API.IntegrationTests.Controllers
{
    [Collection("Integration")]
    public class PrivacySettingControllerTests(AppFixture fixture)
    {
        private readonly IAlbaHost host = fixture.Host;

        [Fact]
        public async Task ValidPrivacySettingGet()
        {
            //Arrange
            var fakeUser = Builder<Core.Models.User>.CreateNew().Build();
            var fakeUserCredential = Builder<UserCredential>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.EmailAddress = "testEmail@test.com";
                    x.RefreshToken = "Hw4y06bskqzxz4YIYoYFp8xcqw+p6hyonncU7hyamWkL/seWkibsPMnMH1nMVo4rDaAyHD9a+mUGYkRvNwPlOA==";
                    x.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(5);
                }).Build();
            fakeUserCredential.HashedPassword = new PasswordHasher<UserCredential>().HashPassword(fakeUserCredential, "Password1!");
            var fakeUserPrivacySetting = Builder<UserPrivacySetting>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.IsPrivate = false;
                }).Build();

            host.WithEmptyDatabase(async context =>
            {
                context.Add(fakeUser);
                context.Add(fakeUserCredential);
                context.Add(fakeUserPrivacySetting);
                await context.SaveChangesAsync();
            });

            //Act
            var response = await host.Scenario(_ =>
            {
                _.WithClaim(new Claim("email", "testEmail@test.com"));
                _.WithClaim(new Claim("role", Roles.UserRole));
                _.Get.Url($"/api/privacysetting");
                _.StatusCodeShouldBeOk();
            });

            //Assert
            var result = response.ReadAsJson<GetPrivacySettingViewModel>();
            Assert.NotNull(result);
            Assert.IsType<GetPrivacySettingViewModel>(result);
            Assert.Equal(result.UserId, fakeUser.Id);
            Assert.False(result.IsPrivate);
        }

        [Fact]
        public async Task ValidPrivacySettingPut()
        {
            //Arrange
            var fakeUser = Builder<Core.Models.User>.CreateNew().Build();
            var fakeUserCredential = Builder<UserCredential>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.EmailAddress = "testEmail@test.com";
                    x.RefreshToken = "Hw4y06bskqzxz4YIYoYFp8xcqw+p6hyonncU7hyamWkL/seWkibsPMnMH1nMVo4rDaAyHD9a+mUGYkRvNwPlOA==";
                    x.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(5);
                }).Build();
            fakeUserCredential.HashedPassword = new PasswordHasher<UserCredential>().HashPassword(fakeUserCredential, "Password1!");
            var fakeUserPrivacySetting = Builder<UserPrivacySetting>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.IsPrivate = false;
                }).Build();

            host.WithEmptyDatabase(async context =>
            {
                context.Add(fakeUser);
                context.Add(fakeUserCredential);
                context.Add(fakeUserPrivacySetting);
                await context.SaveChangesAsync();
            });

            var request = new UpdatePrivacySettingRequest() { UserId = fakeUser.Id, IsPrivate = true };

            //Act
            await host.Scenario(_ =>
            {
                _.WithClaim(new Claim("email", "testEmail@test.com"));
                _.WithClaim(new Claim("role", Roles.UserRole));
                _.Put.Json(request).ToUrl($"/api/privacysetting");
                _.StatusCodeShouldBeOk();
            });

            //Assert
            var updatedPrivacySettingResponse = await host.Scenario(_ =>
            {
                _.WithClaim(new Claim("email", "testEmail@test.com"));
                _.WithClaim(new Claim("role", Roles.UserRole));
                _.Get.Url($"/api/privacysetting");
                _.StatusCodeShouldBeOk();
            });
            var result = updatedPrivacySettingResponse.ReadAsJson<GetPrivacySettingViewModel>();
            Assert.Equal(result.UserId, fakeUser.Id);
            Assert.True(result.IsPrivate);
        }
    }
}
