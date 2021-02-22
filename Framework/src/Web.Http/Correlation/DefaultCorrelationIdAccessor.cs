using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Sogetrel.Sinapse.Framework.Web.Http.Correlation
{
    /// <summary>
    /// This class provides a method to get the current http context correlation id identified by the http header "X-Correlation-ID".
    /// </summary>
    public class DefaultCorrelationIdAccessor : ICorrelationIdAccessor
    {
        private readonly ILogger<DefaultCorrelationIdAccessor> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DefaultCorrelationIdAccessor(ILogger<DefaultCorrelationIdAccessor> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCorrelationId()
        {
            try
            {
                var context = _httpContextAccessor.HttpContext;
                var result = (string)context?.Items[Headers.CorrelationIdKey];
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Unable to get original session id header.");
            }

            return string.Empty;
        }
    }
}
