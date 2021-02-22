using System;
using System.IO;
using Assistance.Event.Dal.Repositories;
using Assistance.Operational.Bll.Impl.Services;
using Assistance.Operational.Bll.Services;
using Assistance.Operational.Dal.AzureNotificationImpl.Repositories;
using Assistance.Operational.Dal.AzureStorageImpl.Repositories;
using Assistance.Operational.Dal.MongoImpl.Configurations;
using Assistance.Operational.Dal.MongoImpl.Repositories;
using Assistance.Operational.Dal.Repositories;
using Assistance.Operational.Dal.SendGridImpl.Repositories;
using Assistance.Operational.WebApi.Builders;
using Assistance.Operational.WebApi.Configurations;
using Assistance.Operational.WebApi.Extensions;
using Assistance.Operational.WebApi.Handlers;
using Assistance.Operational.WebApi.OperationFilters;
using Dal.DiscordImpl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SendGrid;
using Sentry;
using Serilog;
using Sogetrel.Sinapse.Framework.Dal.MongoDb;
using Sogetrel.Sinapse.Framework.Web.Http.Extensions;
using Swashbuckle.AspNetCore.Filters;

namespace Assistance.Operational.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;

            var logger = new LoggerConfiguration()
              .ReadFrom.Configuration(Configuration)
              .WriteTo.Sentry(o =>
              {
                  o.Dsn = new Dsn("https://e6c2a6a4e94c47c2a322128e573357d6@o388208.ingest.sentry.io/5224724");
                  o.Environment = env.EnvironmentName;
              })
              .CreateLogger();

            Log.Logger = logger;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions()
                .ConfigureByConvention<AuthenticationSettings>(Configuration)
                .ConfigureByConvention<DatabaseConfiguration>(Configuration)
                .ConfigureByConvention<AppSettings>(Configuration);

            services.AddVersionedApiExplorer(o => o.GroupNameFormat = "F");
            services.AddApiVersioning(o =>
            {
                using var provider = services.BuildServiceProvider();
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ReportApiVersions = true;
            });
            AddSwaggerGen(services);

            services.AddControllers(opts =>
            {
                opts.Filters.Add<ExceptionHandler>();
                opts.EnableEndpointRouting = true;
            })
            .AddJsonOptions(options => { options.JsonSerializerOptions.IgnoreNullValues = true; })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddResponseCaching();
            services.AddHealthChecks();

            // Http client
            services.AddHttpClient();

            // Bll
            services.AddTransient<IDashboardService, DashboardService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IMissionService, MissionService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IActivityService, ActivityService>();
            services.AddTransient<IOrganizationService, OrganizationService>();
            services.AddTransient<IInviteService, InviteService>();
            services.AddTransient<ILeadService, LeadService>();
            services.AddTransient<IEventService, EventService>();

            // Dal
            services.AddTransient<IMissionRepository, MissionRepository>();
            services.AddTransient<IOrganizationRepository, OrganizationRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IActivityRepository, ActivityRepository>();
            services.AddTransient<IInviteRepository, InviteRepository>();
            services.AddTransient<IEventRepository, EventRepository>();
            services.AddSingleton<IAzureStorageRepository, AzureStorageRepository>();
            services.AddSingleton<IInternalNotificationRepository, DiscordRepository>();
            services.AddTransient<IMailRepository, MailRepository>();
            services.AddTransient<INotificationRepository, NotificationRepository>();
            services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddTransient<ISendGridClient, SendGridClient>(x => new SendGridClient(Configuration["Mail:ApiKey"]));

            services.TryAddSingleton<IMongoDbContext>(provider => new DefaultMongoDbContext(provider.GetRequiredService<IOptionsMonitor<DatabaseConfiguration>>().CurrentValue));
            services.AddScoped<IMongoDbConfiguration>(provider => provider.GetRequiredService<IOptionsSnapshot<DatabaseConfiguration>>().Value);

            // add AutoMapper
            var mappingBuilder = new MappingBuilder();
            services.AddSingleton(x => mappingBuilder.BuildAutoMapper());

            // Add Cors
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder => builder.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader());
            });

            services.AddHttpLoggingMiddleware();

            if (Env.EnvironmentName != "IntegrationTests")
            {
                services
                    .AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        using var provider = services.BuildServiceProvider();
                        var config = provider.GetRequiredService<IOptionsSnapshot<AuthenticationSettings>>().Value;
                        options.Authority = config.Authority;
                        options.Audience = config.Audience;
                        if (!Env.IsProduction())
                            options.RequireHttpsMetadata = false;
                    });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IApiVersionDescriptionProvider provider)
        {
            loggerFactory.AddSerilog();

            if (Env.EnvironmentName != "Production")
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var versionDescription in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"./{versionDescription.GroupName}/swagger.json",
                        versionDescription.GroupName.ToUpperInvariant());
                }
            });
            app.UseRouting();
            app.UseCors();
            app.UseResponseCaching();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// Adds Swagger documentation
        /// </summary>
        /// <param name="services"></param>
        private void AddSwaggerGen(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                using var provider = services.BuildServiceProvider();
                foreach (var versionDescription in provider.GetRequiredService<IApiVersionDescriptionProvider>().ApiVersionDescriptions)
                {
                    options.SwaggerDoc(versionDescription.GroupName, new OpenApiInfo
                    {
                        Title = $"Trine Gateway ({Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "None"}) {versionDescription.ApiVersion}",
                        Version = versionDescription.ApiVersion.ToString()
                    });
                }
                var openApiSecurityScheme = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                };
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, openApiSecurityScheme);
                options.OperationFilter<SwaggerDefaultValuesOperationFilter>();
                foreach (var path in Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.AllDirectories))
                {
                    options.IncludeXmlComments(path);
                }
                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                options.OperationFilter<SecurityRequirementsOperationFilter>(true, JwtBearerDefaults.AuthenticationScheme);
                options.CustomSchemaIds(i => i.FullName);
                options.CustomOperationIds(api => api.ActionDescriptor.Id);
            });
        }
    }
}
