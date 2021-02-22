using System;
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
    public class IncomingRequestCorrelationIdReaderMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_WithNoCorrelationGuid_SetsNewCorrelationGuid()
        {
            // Arrange
            var middleware = ArrangeLogHeaderMiddleware(out var httpContext);

            // Act
            httpContext.Items.Count.Should().Be(0);
            await middleware.InvokeAsync(httpContext);
            object correlationId;
            httpContext.Items.TryGetValue(Headers.CorrelationIdKey, out correlationId);

            // Assert
            httpContext.Items.Count.Should().Be(1);
            ((string)correlationId).Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task InvokeAsync_WithExistingCorrelationGuid_ReadsExistingCorrelationGuidFromHeader()
        {
            // Arrange
            var middleware = ArrangeLogHeaderMiddleware(out var httpContext);
            var existingCorrelationGuid = new Fixture().Create<string>();
            httpContext.Request.Headers.Add(Headers.CorrelationIdKey, existingCorrelationGuid);

            // Act
            httpContext.Items.Count.Should().Be(0);
            await middleware.InvokeAsync(httpContext);
            object correlationId;
            httpContext.Items.TryGetValue(Headers.CorrelationIdKey, out correlationId);

            // Assert
            httpContext.Items.Count.Should().Be(1);
            ((string)correlationId).Should().NotBeNullOrWhiteSpace();
            ((string)correlationId).Should().BeEquivalentTo(existingCorrelationGuid);
        }

        private IncomingRequestCorrelationIdReaderMiddleware ArrangeLogHeaderMiddleware(out DefaultHttpContext httpContext)
        {
            httpContext = new DefaultHttpContext();
            var loggerMock = new Mock<ILogger<IncomingRequestCorrelationIdReaderMiddleware>>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<IncomingRequestCorrelationIdReaderMiddleware>))).Returns(loggerMock.Object);
            httpContext.RequestServices = serviceProviderMock.Object;
            var middleware = new IncomingRequestCorrelationIdReaderMiddleware(async (innerHttpContext) =>
            {
                await innerHttpContext.Response.WriteAsync("doing some tests");
            });
            return middleware;
        }
    }
}
