using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using Sogetrel.Sinapse.Framework.Web.Http.Logging;
using Xunit;

namespace Sogetrel.Sinapse.Framework.Web.Http.UnitTests.Logging
{
    [Trait("Category", "UnitTests")]
    public class RequestLoggerMiddlewareTests
    {
        private bool _requestDelegateIsExecuted = false;
        private readonly Fixture _fixture;

        public RequestLoggerMiddlewareTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task InvokeAsync_NominalCase_ShouldLogTheIncomingRequest()
        {
            // Arrange
            var fakeLogger = new FakeLogger<RequestLoggerMiddleware>();
            var httpContext = new DefaultHttpContext();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<RequestLoggerMiddleware>))).Returns(fakeLogger);
            httpContext.RequestServices = serviceProviderMock.Object;

            var requestUri = _fixture.Create<Uri>();
            httpContext.Request.Host = new HostString(requestUri.Host, requestUri.Port);
            httpContext.Request.Scheme = requestUri.Scheme;
            httpContext.Request.Method = _fixture.Create<string>();
            httpContext.Request.Headers.Add("TestHeader", _fixture.Create<string>());

            RequestDelegate requestDelegate = async (innerHttpContext) =>
            {
                await Task.Delay(200);
                _requestDelegateIsExecuted = true;
            };

            var middleware = new RequestLoggerMiddleware(requestDelegate, fakeLogger);
            var requestBody = _fixture.Create<string>();
            httpContext.Request.Body = ArrangeHttpContextRequestBody(requestBody);

            // Act
            _requestDelegateIsExecuted.Should().BeFalse();
            await middleware.InvokeAsync(httpContext);

            // Assert
            var expectedLogMessage = "Incoming request ";
            expectedLogMessage += $"[FullPath, {httpContext.Request.GetDisplayUrl()}], ";
            expectedLogMessage += $"[Method, {httpContext.Request.Method}], ";
            expectedLogMessage += $"[Headers, {new HeaderDictionary().ToDictionary(x => x.Key, y => y.Value).ToString()}], ";
            // Cannot read Headers value because of its type. So we check it is a correct Dictionary.
            expectedLogMessage += $"[Body, {requestBody}]";
            fakeLogger.LogMessage.Should().Be(expectedLogMessage);
            var scope = (Dictionary<string, string>)fakeLogger.LogScope;
            scope["Method"].Should().Be(httpContext.Request.Method);
            scope["FullPath"].Should().Be(httpContext.Request.GetDisplayUrl());
            fakeLogger.LogLevel.Should().Be(LogLevel.Trace);
            _requestDelegateIsExecuted.Should().BeTrue();
        }

        private Stream ArrangeHttpContextRequestBody(string body)
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
