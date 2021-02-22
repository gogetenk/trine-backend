using Microsoft.Extensions.DependencyInjection;
using Sogetrel.Sinapse.Framework.Web.Http.Correlation;
using Sogetrel.Sinapse.Framework.Web.Http.Correlation.Middleware;
using Sogetrel.Sinapse.Framework.Web.Http.Logging;

namespace Sogetrel.Sinapse.Framework.Web.Http.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds middleware configuration to the Service Collection for Http Correlation.
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <param name="services"></param>
        /// <param name="httpClientConfiguration"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpCorrelationMiddleware<TConfiguration>(this IServiceCollection services, TConfiguration httpClientConfiguration)
            where TConfiguration : class, IHttpClientConfiguration
        {
            services.AddHttpClient(httpClientConfiguration.HttpClientName)
                .AddHttpMessageHandler<OutgoingRequestCorrelationIdWriterMiddleware>();
            services.AddHttpContextAccessor();
            services.AddScoped<IHttpClientConfiguration, TConfiguration>();
            services.AddScoped<ICorrelationIdAccessor, DefaultCorrelationIdAccessor>();
            services.AddTransient<OutgoingRequestCorrelationIdWriterMiddleware>();
            return services;
        }

        /// <summary>
        /// Adds middleware configuration to the Service Collection for Http Logging.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpLoggingMiddleware(this IServiceCollection services)
        {
            services.AddTransient<RequestLoggerMiddleware>();
            services.AddTransient<ResponseLoggerMiddleware>();
            return services;
        }
    }
}
