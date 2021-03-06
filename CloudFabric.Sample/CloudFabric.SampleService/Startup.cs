﻿using System;
using System.IO;
using Newtonsoft.Json;
using CloudFabric.Logging;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using CloudFabric.SampleService.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using CloudFabric.Library.Common.Filters;
using CloudFabric.Library.Common.Middlewares;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace CloudFabric.SampleService
{
    public class Startup
    {
        ConfigSettings _settings;
        public IConfiguration _configuration { get; }
        public Startup(IConfiguration configuration, ConfigSettings settings)
        {
            _settings = settings;
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(_configuration);

            services.AddCors(options =>
            {
                options.AddPolicy("default", policy => {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddAuthentication("Bearer")
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = _settings.IdentityServerHost;
                        options.RequireHttpsMetadata = true;
                        options.ApiName = "SampleAPI";
                    });

            services.AddMvc(config =>
                    {
                        config.Filters.Add<GlobalExceptionFilter>();
                        config.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())); // By default, all controllers are protected
                    })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>())
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    });
                       
            services.AddOptions();
            services.AddHttpClient();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Sample Service API",
                    Description = "Sample Service API",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "Preetham Reddy", Email = "preetham@techfabric.io" }
                });

                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "CloudFabric.SampleService.xml");
                c.IncludeXmlComments(xmlPath);
            });

            services.AddSingleton<TelemetryClient>(
                new Func<IServiceProvider, TelemetryClient>(
                    (IServiceProvider provider) =>
                        new TelemetryClient()
                        {
                            InstrumentationKey = _settings.InstrumentationKey
                        }
                )
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var appInsightsLogLevel = _configuration.GetValue<LogLevel>("Logging:Application Insights:LogLevel:Default");
            loggerFactory.AddApplicationInsights(app.ApplicationServices, appInsightsLogLevel);

            app.UseCorrelationIdHeader();
            app.UseMiddleware<RequestLoggingMiddlewareExtended>();

            app.UseAuthentication();

            app.UseCors("default");
            app.UseMvc();

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    if (_settings.Environment != "Development")
                    {
                        swaggerDoc.BasePath = "/sample"; //This should be the path to the micro-service on Service Fabric Cluster configured on Traefik
                    }
                });
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "Cloud Fabric Sample API V1");
            });

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }
    }
}
