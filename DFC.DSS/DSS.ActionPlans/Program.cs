using Azure.Messaging.ServiceBus;
using DSS.ActionPlans.Interfaces;
using DSS.ActionPlans.Services;
using DSS.Interfaces;
using DSS.SharedServices;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DSS.ActionPlans
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var host = new HostBuilder()
               .ConfigureFunctionsWebApplication()
               .ConfigureServices(services =>
               {
                   services.AddApplicationInsightsTelemetryWorkerService();
                   services.ConfigureFunctionsApplicationInsights();
                   services.AddSingleton<IGenericCosmosDbService, GenericCosmosDbService>();
                   services.AddSingleton<ICosmosDbService, CosmosDbService>();
                   services.AddSingleton<IServiceBusService, ServiceBusService>();
                   services.AddSingleton<IHttpRequestService, HttpRequestService>();
                   services.AddSingleton<ILogService, LogService>();
                   services.AddSingleton<IDynamicConverterService, DynamicConverterService>();
                   services.AddSingleton(sp =>
                   {
                       var options = new CosmosClientOptions()
                       {
                           ConnectionMode = ConnectionMode.Gateway
                       };

                       return new CosmosClient(
                           Environment.GetEnvironmentVariable("cosmosDbUri"),
                           Environment.GetEnvironmentVariable("cosmosDbAccessKey"),
                           options
                       );
                   });
                   services.AddSingleton(sp =>
                   {
                       ServiceBusClient client = new ServiceBusClient(Environment.GetEnvironmentVariable("serviceBusConnectionString"), new ServiceBusClientOptions
                       {
                           TransportType = ServiceBusTransportType.AmqpWebSockets
                       });

                       return client;
                   });
               })
               .Build();
            host.Run();
        }
    }
}