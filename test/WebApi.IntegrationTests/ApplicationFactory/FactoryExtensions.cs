using System.Net.Http;
using System.Net.Http.Headers;
using Assistance.Operational.WebApi.IntegrationTests.Filters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Assistance.Operational.WebApi.IntegrationTests.ApplicationFactory
{
    public static class AuthenticationExtensions
    {
        public static HttpClient CreateClientWithTestAuth<T>(this WebApplicationFactory<T> factory, TestClaimsProvider claimsProvider) where T : class
        {
            var client = factory.
                WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddAuthentication("IntegrationTests")
                                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("IntegrationTests", op => { });

                        services.AddScoped<TestClaimsProvider>(_ => claimsProvider);
                    });
                })
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("IntegrationTests");

            return client;
        }
    }
}
