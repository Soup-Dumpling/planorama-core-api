using FizzWare.NBuilder;
using Models = Planorama.User.Core.Models;
using Planorama.User.Infrastructure.Repository.User;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace Planorama.User.Infrastructure.UnitTests.Repository.User
{
    public class GetLoggedInUserRepositoryUnitTests
    {
        private readonly UserDBContext context;
        private readonly GetLoggedInUserRepository getLoggedInUserRepository;

        public GetLoggedInUserRepositoryUnitTests() 
        {
            context = Helpers.InMemoryContextHelper.GetContext();
            getLoggedInUserRepository = new GetLoggedInUserRepository(context);
        }

        [Fact]
        public async Task ValidGetLoggedInUserByEmail()
        {
            //Arrange
            var fakeUser = Builder<Models.User>.CreateNew().Build();
            var fakeUserCredential = Builder<Models.UserCredential>.CreateNew()
                .Do(x =>
                {
                    x.UserId = fakeUser.Id;
                    x.EmailAddress = "user.testing@outlook.com";
                    x.HashedPassword = "Gt9Yc4AiIvmsC1QQbe2RZsCIqvoYlst2xbz0Fs8aHnw=";
                    x.RefreshToken = "nvBhkx2Pfm633TYf56by974P0al2JLhjZ1ch324auT6Jn9dIAerWQNJCthZ05KguFymxZ0QBfeeaMP3IlYt1HQ==";
                    x.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);
                })
                .Build();
            context.Users.Add(fakeUser);
            context.UserCredentials.Add(fakeUserCredential);
            await context.SaveChangesAsync();

            //Act
            var result = await getLoggedInUserRepository.GetLoggedInUserByEmail(fakeUserCredential.EmailAddress);

            //Assert
            Assert.NotNull(result);
            var count = await context.Users.CountAsync();
            Assert.Equal(fakeUser.Id, result.Id);
            Assert.Equal(fakeUser.FirstName, result.FirstName);
            Assert.Equal(fakeUser.LastName, result.LastName);
            Assert.Equal(fakeUserCredential.EmailAddress, result.EmailAddress);
            Assert.Equal(1, count);
        }
    }
}
