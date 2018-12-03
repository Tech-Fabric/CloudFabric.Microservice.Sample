using Serilog;
using System.IO;
using System.Fabric;
using System.Net.Http;
using CloudFabric.Logging;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using CloudFabric.SampleService.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

namespace CloudFabric.SampleService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class SampleService : StatelessService
    {
        ConfigSettings _settings;
        public SampleService(StatelessServiceContext context)
            : base(context)
        {
            _settings = new ConfigSettings(context);
            Log.Logger = new ConfigLogging().CreateLogger(_settings.Environment, _settings.ApplicationName, _settings.LogglyToken);
        }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<StatelessServiceContext>(serviceContext)
                                            .AddSingleton<ConfigSettings>(new ConfigSettings(serviceContext))
                                            .AddSingleton<HttpClient>(new HttpClient()))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseEnvironment(_settings.Environment)
                                    .UseStartup<Startup>()
                                    .UseSerilog()
                                    .UseApplicationInsights(_settings.InstrumentationKey)
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }
    }
}
