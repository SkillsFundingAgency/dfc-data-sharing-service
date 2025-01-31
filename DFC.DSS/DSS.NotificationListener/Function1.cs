using DSS.Interfaces;
using DSS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DSS.NotificationListener
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly ICosmosDbService _cosmos;

        public Function1(ILogger<Function1> logger, ICosmosDbService cosmos)
        {
            _logger = logger;
            _cosmos = cosmos;
        }

        [Function("Function1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string databaseName = Environment.GetEnvironmentVariable("notificationDatabaseName").ToString();
            string containerName = Environment.GetEnvironmentVariable("notificationContainerName").ToString();

            //var hello = await _cosmos.GetNotificationDocument("<REPLACE ME>", databaseName, containerName);
            //var hello = await _cosmos.CreateNewNotificationDocument();

            var hello = await _cosmos.GenericRetrieveDocument<Notification>("<REPLACE ME>", databaseName, containerName);

            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
