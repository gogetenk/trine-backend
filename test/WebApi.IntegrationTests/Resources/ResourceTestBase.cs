using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Assistance.Operational.WebApi.IntegrationTests.Resources
{
    public abstract class ResourceTestBase<TFactory, TStartup> : IClassFixture<TFactory>, IAsyncLifetime
        where TFactory : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        protected TFactory Factory { get; }

        protected HttpClient Client { get; set; }

        protected ResourceTestBase(WebApplicationFactory<TStartup> factory, Action<TFactory> initializationAction = null)
        {
            Factory = (TFactory)factory;
            initializationAction?.Invoke(Factory);
            Client = Factory.CreateClient();
        }

        public abstract Task InitializeAsync();

        public abstract Task DisposeAsync();
    }
}
