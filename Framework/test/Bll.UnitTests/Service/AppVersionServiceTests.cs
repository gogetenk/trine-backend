using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Sogetrel.Sinapse.Framework.Bll.Services;
using Xunit;

namespace Sogetrel.Sinapse.Framework.Bll.UnitTests.Service
{
    public class AppVersionServiceTests
    {
        private readonly Mock<IAppVersionAccessor> _mockVersionAccessor;
        private readonly AppVersionService _appVersionService;

        public AppVersionServiceTests()
        {
            _mockVersionAccessor = new Mock<IAppVersionAccessor>();
            var mockLogger = new Mock<ILogger<AppVersionService>>();
            _appVersionService = new AppVersionService(mockLogger.Object, _mockVersionAccessor.Object);
        }

        [Fact]
        public void GetAppVersionDto_NominalCase()
        {
            // Arrange
            var stringFixture = new Fixture().Create<string>();
            _mockVersionAccessor.SetupGet(x => x.Version).Returns(stringFixture);

            // Act
            var model = _appVersionService.GetAppVersion();
            // Assert
            model.Should().NotBeNull();
            model.Version.Should().Be(stringFixture);
        }
    }
}
