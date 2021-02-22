using Assistance.Operational.Dal.MongoImpl.Configurations;
using Assistance.Operational.WebApi.IntegrationTests.Filters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Xunit.Abstractions;

namespace Assistance.Operational.WebApi.IntegrationTests.ApplicationFactory
{
    /// <summary>
    /// Startup class for IT
    /// </summary>
    public class TrineFactory : WebApplicationFactory<Startup>
    {
        /// <summary>
        /// Environment Name used to disable swagger
        /// </summary>
        private const string _IntegrationTestsEnvironment = "IntegrationTests";
        public string ActivitiesCollectionName { get; set; }
        public string UsersCollectionName { get; set; }
        public IConfiguration Configuration { get; private set; }
        public ITestOutputHelper Output { get; set; }

        /// <summary>
        /// Gives a fixture an opportunity to configure the application before it gets built.
        /// </summary>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .UseSolutionRelativeContentRoot("./src/WebApi/") // Needed to make the test to work
                .UseEnvironment(_IntegrationTestsEnvironment) // Needed in order to disable Swagger Doc (fix for Live Unit Test to work)
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    var env = builderContext.HostingEnvironment;
                    Configuration = config
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .Build();
                })
                .ConfigureTestServices(services =>
                {
                    services.AddHttpClient();

                    // Mocking authentication in the Integration Tests
                    services
                        .AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                            "Test", options => { });

                    // We configure a specific collection name for the Integration Tests
                    services.PostConfigure<DatabaseConfiguration>(config =>
                    {
                        config.ActivitiesCollectionName = ActivitiesCollectionName;
                        config.UsersCollectionName = UsersCollectionName;
                    });
                })
                .UseStartup<Startup>()
                .UseEnvironment("IntegrationTests");
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = base.CreateWebHostBuilder();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(Output)
                .CreateLogger()
                .ForContext<TrineFactory>();

            return builder;
        }

    }
}
