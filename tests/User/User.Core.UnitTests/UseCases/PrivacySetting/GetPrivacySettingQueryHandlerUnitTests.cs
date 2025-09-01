using NSubstitute;
using Planorama.User.Core.Context;
using Planorama.User.Core.Exceptions;
using Planorama.User.Core.UseCases.PrivacySetting.GetPrivacySetting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Planorama.User.Core.UnitTests.UseCases.PrivacySetting
{
    public class GetPrivacySettingQueryHandlerUnitTests
    {
        private readonly GetPrivacySettingQueryHandler getPrivacySettingQueryHandler;
        private IGetPrivacySettingRepository getPrivacySettingRepositoryMock = Substitute.For<IGetPrivacySettingRepository>();
        private IUserContext userContextMock = Substitute.For<IUserContext>();

        public GetPrivacySettingQueryHandlerUnitTests()
        {
            getPrivacySettingQueryHandler = new GetPrivacySettingQueryHandler(getPrivacySettingRepositoryMock, userContextMock);
        }

        [Fact]
        public async Task ValidQuery()
        {
            //Arrange
            var query = new GetPrivacySettingQuery();
            userContextMock.IsLoggedIn().Returns(true);
            userContextMock.UserName.Returns("user.testing@outlook.com");
            var userId = Guid.NewGuid();
            getPrivacySettingRepositoryMock.GetUserIdByEmailAsync(Arg.Any<string>()).Returns(userId);
            getPrivacySettingRepositoryMock.GetUserPrivacySettingByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult(new GetPrivacySettingViewModel() { UserId = userId, IsPrivate = false }));

            //Act
            var result = await getPrivacySettingQueryHandler.Handle(query, CancellationToken.None);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<GetPrivacySettingViewModel>(result);
            await getPrivacySettingRepositoryMock.Received().GetUserIdByEmailAsync("user.testing@outlook.com");
            await getPrivacySettingRepositoryMock.Received().GetUserPrivacySettingByIdAsync(userId);
        }
    }
}
