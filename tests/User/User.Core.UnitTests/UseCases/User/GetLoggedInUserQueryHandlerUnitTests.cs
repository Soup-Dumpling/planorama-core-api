using NSubstitute;
using Planorama.User.Core.Context;
using Planorama.User.Core.Exceptions;
using Planorama.User.Core.UseCases.User.GetLoggedInUser;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.Core.UnitTests.UseCases.User
{
    public class GetLoggedInUserQueryHandlerUnitTests
    {
        private readonly GetLoggedInUserQueryHandler getLoggedInUserQueryHandler;
        private IGetLoggedInUserRepository getLoggedInUserRepositoryMock = Substitute.For<IGetLoggedInUserRepository>();
        private IUserContext userContextMock = Substitute.For<IUserContext>();

        public GetLoggedInUserQueryHandlerUnitTests()
        {
            getLoggedInUserQueryHandler = new GetLoggedInUserQueryHandler(getLoggedInUserRepositoryMock, userContextMock);
        }

        [Fact]
        public async Task ValidQuery()
        {
            //Arrange
            var query = new GetLoggedInUserQuery();
            userContextMock.IsLoggedIn().Returns(true);
            userContextMock.UserName.Returns("user.testing@outlook.com");
            getLoggedInUserRepositoryMock.GetLoggedInUserByEmail(Arg.Any<string>()).Returns(Task.FromResult(new GetLoggedInUserViewModel() { Id = Guid.NewGuid(), FirstName = "firstName", LastName = "lastName", EmailAddress = "user.testing@outlook.com" }));

            //Act
            var result = await getLoggedInUserQueryHandler.Handle(query, CancellationToken.None);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<GetLoggedInUserViewModel>(result);
            await getLoggedInUserRepositoryMock.Received().GetLoggedInUserByEmail("user.testing@outlook.com");
        }

        [Fact]
        public async Task UserNotLoggedIn()
        {
            //Arrange
            var query = new GetLoggedInUserQuery();
            userContextMock.IsLoggedIn().Returns(false);

            //Act and Assert
            await Assert.ThrowsAsync<AuthorizationException>(async () => await getLoggedInUserQueryHandler.Handle(query, CancellationToken.None));
            await getLoggedInUserRepositoryMock.DidNotReceive().GetLoggedInUserByEmail(Arg.Any<string>());
        }

        [Fact]
        public async Task UserNotFound()
        {
            //Arrange
            var query = new GetLoggedInUserQuery();
            userContextMock.IsLoggedIn().Returns(true);
            userContextMock.UserName.Returns("user.testing@outlook.com");
            getLoggedInUserRepositoryMock.GetLoggedInUserByEmail(Arg.Any<string>()).Returns(Task.FromResult(null as GetLoggedInUserViewModel));

            //Act and Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await getLoggedInUserQueryHandler.Handle(query, CancellationToken.None));
            await getLoggedInUserRepositoryMock.Received().GetLoggedInUserByEmail("user.testing@outlook.com");
        }
    }
}
