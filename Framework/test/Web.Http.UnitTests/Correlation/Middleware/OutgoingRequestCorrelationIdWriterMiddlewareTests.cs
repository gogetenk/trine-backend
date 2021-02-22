using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Sogetrel.Sinapse.Framework.Web.Http.Correlation;
using Sogetrel.Sinapse.Framework.Web.Http.Correlation.Middleware;
using Xunit;

namespace Sogetrel.Sinapse.Framework.Web.Http.UnitTests.Correlation.Middleware
{
    [Trait("Category", "UnitTests")]
    public class OutgoingRequestCorrelationIdWriterMiddlewareTests
    {
        [Fact]
        public async Task SendAsync_WithNoCorrelationId_SetsEmptyCorrelationId()
        {
            // Arrange
            var middlewareHandler = ArrangeMiddleware();
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://test.com");
            var client = new HttpClient(middlewareHandler);

            // Act
            var result = await client.SendAsync(httpRequestMessage);

            // Assert
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task SendAsync_WithCorrelationId_SetsValidCorrelationId()
        {
            // Arrange
            var correlationId = new Fixture().Create<string>();
            var middlewareHandler = ArrangeMiddleware(correlationId);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://test.com");
            var client = new HttpClient(middlewareHandler);

            // Act
            var result = await client.SendAsync(httpRequestMessage);

            // Assert
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        private OutgoingRequestCorrelationIdWriterMiddleware ArrangeMiddleware(string correlationId = null)
        {
            var httpContext = new DefaultHttpContext();
            if (!string.IsNullOrWhiteSpace(correlationId))
                httpContext.Items.Add(Headers.CorrelationIdKey, correlationId);
            var loggerMock = new Mock<ILogger<DefaultCorrelationIdAccessor>>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(accessor => accessor.HttpContext).Returns(httpContext);
            var correlationIdAccessor = new DefaultCorrelationIdAccessor(loggerMock.Object, httpContextAccessorMock.Object);
            var middlewareHandler = new OutgoingRequestCorrelationIdWriterMiddleware(correlationIdAccessor, httpContextAccessorMock.Object)
            {
                InnerHandler = new TestHandler((request, cancellationToken) =>
                {
                    // Assert
                    request.Headers.Contains(Headers.CorrelationIdKey).Should().BeTrue();
                    request.Headers.TryGetValues(Headers.CorrelationIdKey, out var correlationValues);
                    correlationValues.Should().NotBeNullOrEmpty();
                    correlationValues.ToList().Count.Should().Be(1);
                    if (!string.IsNullOrWhiteSpace(correlationId))
                        correlationValues.ToList()[0].Should().BeEquivalentTo(correlationId);
                    else
                        correlationValues.ToList()[0].Should().BeNullOrEmpty();
                    return TestHandler.Return200();
                })
            };
            return middlewareHandler;
        }
    }
}
