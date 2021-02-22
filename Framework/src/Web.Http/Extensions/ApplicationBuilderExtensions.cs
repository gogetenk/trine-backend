using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Sogetrel.Sinapse.Framework.Web.Http.Correlation.Middleware;
using Sogetrel.Sinapse.Framework.Web.Http.Logging;

namespace Sogetrel.Sinapse.Framework.Web.Http.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Exposes <see cref="IncomingRequestCorrelationIdReaderMiddleware"/> to the Application Builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>Application Builder</returns>
        public static IApplicationBuilder UseCorrelationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IncomingRequestCorrelationIdReaderMiddleware>();
        }

        /// <summary>
        /// Exposes <see cref="RequestLoggerMiddleware"/> and <see cref="ResponseLoggerMiddleware"/> to the Application Builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>Application Builder</returns>
        public static IApplicationBuilder UseHttpLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            })
            .UseMiddleware<RequestLoggerMiddleware>()
            .UseMiddleware<ResponseLoggerMiddleware>();
        }
    }
}
