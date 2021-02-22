using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace Assistance.Operational.WebApi
{
    /// <summary>
    /// Web application program class to define the entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Entry point call.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Create web host builder.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseUrls($"http://*:{Environment.GetEnvironmentVariable("PORT")}")
                .UseSerilog()
                .UseStartup<Startup>();
        }
    }
}
