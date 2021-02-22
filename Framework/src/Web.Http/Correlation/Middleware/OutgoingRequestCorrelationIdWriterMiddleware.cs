using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Sogetrel.Sinapse.Framework.Web.Http.Correlation.Middleware
{
    /// <summary>
    /// Adds a correlation id and request id to the outgoing http messages.
    /// </summary>
    public class OutgoingRequestCorrelationIdWriterMiddleware : DelegatingHandler
    {
        private readonly ICorrelationIdAccessor _correlationIdAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OutgoingRequestCorrelationIdWriterMiddleware(ICorrelationIdAccessor correlationIdAccessor, IHttpContextAccessor httpContextAccessor)
        {
            _correlationIdAccessor = correlationIdAccessor;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add(Headers.RequestIdKey, _httpContextAccessor.HttpContext.TraceIdentifier);
            request.Headers.Add(Headers.CorrelationIdKey, _correlationIdAccessor.GetCorrelationId());

            return base.SendAsync(request, cancellationToken);
        }
    }
}
