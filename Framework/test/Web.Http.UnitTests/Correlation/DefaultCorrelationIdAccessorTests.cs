using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Sogetrel.Sinapse.Framework.Web.Http.Correlation;
using Xunit;

namespace Sogetrel.Sinapse.Framework.Web.Http.UnitTests.Correlation
{
    [Trait("Category", "UnitTests")]
    public class DefaultCorrelationIdAccessorTests
    {
        [Fact]
        public void GetCorrelationId_NominalCase()
        {
            // Arrange
            var testId = new Fixture().Create<string>();

            var httpContext = new DefaultHttpContext();
            httpContext.Items.Add(Headers.CorrelationIdKey, testId);

            var loggerMock = new Mock<ILogger<DefaultCorrelationIdAccessor>>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(accessor => accessor.HttpContext).Returns(httpContext);
            var correlationIdAccessor = new DefaultCorrelationIdAccessor(loggerMock.Object, httpContextAccessorMock.Object);

            // Act
            var recoveredId = correlationIdAccessor.GetCorrelationId();

            // Assert
            recoveredId.Should().Be(testId);
        }

        //[Fact]
        //public void GetCorrelationId_WithRandomError_ReturnsStringEmptyAndLogWarning()
        //{
        //    // Arrange
        //    var exception = new Exception("Hello there");
        //    var loggerMock = new Mock<ILogger<DefaultCorrelationIdAccessor>>();
        //    var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        //    httpContextAccessorMock.SetupGet(accessor => accessor.HttpContext).Throws(exception);
        //    var correlationIdAccessor = new DefaultCorrelationIdAccessor(loggerMock.Object, httpContextAccessorMock.Object);

        //    // Act
        //    var recoveredId = correlationIdAccessor.GetCorrelationId();

        //    // Assert
        //    recoveredId.Should().BeEmpty();
        //    loggerMock.Verify(logger => logger.Log(
        //            LogLevel.Warning,
        //            It.IsAny<EventId>(),
        //            It.IsAny<Func<object, Exception>>(),
        //            It.Is<Exception>(ex => ex.Message == exception.Message),
        //            It.IsAny<Func<object, Exception, string>>()
        //        ), Times.Once);
        //    loggerMock.VerifyNoOtherCalls();
        //}
    }
}
