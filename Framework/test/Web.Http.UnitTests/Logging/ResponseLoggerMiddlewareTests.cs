using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Sogetrel.Sinapse.Framework.Web.Http.Logging;
using Xunit;

namespace Sogetrel.Sinapse.Framework.Web.Http.UnitTests.Logging
{
    [Trait("Category", "UnitTests")]
    public class ResponseLoggerMiddlewareTests
    {
        private readonly Fixture _fixture;

        public ResponseLoggerMiddlewareTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task InvokeAsync_NominalCase_ShouldLogTheOutgoingResponse()
        {
            // Arrange
            var fakeLogger = new FakeLogger<ResponseLoggerMiddleware>();
            var expectedHttpResponseBody = _fixture.Create<string>();
            var expectedStatusCode = StatusCodes.Status418ImATeapot;
            var expectedTestHeader = _fixture.Create<string>();
            var middleware = ArrangeResponseLoggerMiddleware(fakeLogger, expectedHttpResponseBody, expectedStatusCode, expectedTestHeader, out var httpContext);

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            var expectedLogMessage = "Outgoing response ";
            expectedLogMessage += $"[StatusCode, {expectedStatusCode.ToString()}], ";
            expectedLogMessage += $"[Headers, {new HeaderDictionary().ToDictionary(x => x.Key, y => y.Value).ToString()}], ";
            // Cannot read Headers value because of its type. So we check it is a correct Dictionary.
            expectedLogMessage += $"[Body, {expectedHttpResponseBody}]";
            fakeLogger.LogMessage.Should().Be(expectedLogMessage);
            fakeLogger.LogLevel.Should().Be(LogLevel.Trace);
        }

        [Fact]
        public async Task InvokeAsync_WithUnexpectedError_ShouldProceedWithoutLogging()
        {
            // Arrange
            var fakeLogger = new FakeBadLogger<ResponseLoggerMiddleware>();
            var expectedHttpResponseBody = _fixture.Create<string>();
            var expectedStatusCode = StatusCodes.Status418ImATeapot;
            var expectedTestHeader = _fixture.Create<string>();
            var middleware = ArrangeResponseLoggerMiddleware(fakeLogger, expectedHttpResponseBody, expectedStatusCode, expectedTestHeader, out var httpContext);

            // Act
            httpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
            await Assert.ThrowsAsync<Exception>(async () => await middleware.InvokeAsync(httpContext));
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var output = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();

            // Assert
            httpContext.Response.StatusCode.Should().Be(expectedStatusCode);
            httpContext.Response.Headers.Should().Contain("TestHeader", expectedTestHeader);
            output.Should().Be(expectedHttpResponseBody);
        }

        private ResponseLoggerMiddleware ArrangeResponseLoggerMiddleware(ILogger<ResponseLoggerMiddleware> logger, string expectedHttpResponseBody, int expectedStatusCode, string expectedTestHeader, out DefaultHttpContext httpContext)
        {
            httpContext = new DefaultHttpContext();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<ResponseLoggerMiddleware>))).Returns(logger);
            httpContext.RequestServices = serviceProviderMock.Object;

            RequestDelegate requestDelegate = async (innerHttpContext) =>
            {
                await Task.Delay(200);
                innerHttpContext.Response.Body = ArrangeHttpContextResponseBody(expectedHttpResponseBody);
                innerHttpContext.Response.StatusCode = expectedStatusCode;
                innerHttpContext.Response.Headers.Add("TestHeader", expectedTestHeader);
            };

            var middleware = new ResponseLoggerMiddleware(requestDelegate, logger);
            return middleware;
        }

        private MemoryStream ArrangeHttpContextResponseBody(string body)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(body);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
