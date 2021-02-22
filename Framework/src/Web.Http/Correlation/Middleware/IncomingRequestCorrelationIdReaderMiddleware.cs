using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sogetrel.Sinapse.Framework.Web.Http.Correlation.Middleware
{
    /// <summary>
    /// Verifies if the <see cref="HttpContext"/> has a correlation id. If not, creates a correlation id.
    /// This class also initiates a log scope with that ID so the logs from a same session will expose a same correlation id.
    /// Security note : do not use this class on front-end apps. You should use <see cref="OutgoingRequestCorrelationIdWriterMiddleware"/> instead.
    /// </summary>
    public class IncomingRequestCorrelationIdReaderMiddleware
    {
        private readonly RequestDelegate _next;
        private const string _sessionIdLabel = "@SessionId";

        public IncomingRequestCorrelationIdReaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var header = context.Request.Headers[Headers.CorrelationIdKey];
            var correlationId = header.Count > 0 ? header[0] : Guid.NewGuid().ToString();
            context.Items.Add(Headers.CorrelationIdKey, correlationId);
            var logger = context.RequestServices.GetRequiredService<ILogger<IncomingRequestCorrelationIdReaderMiddleware>>();

            using (logger.BeginScope(new Dictionary<string, string> { { _sessionIdLabel, correlationId } }))
            {
                await _next(context);
            }
        }
    }
}
