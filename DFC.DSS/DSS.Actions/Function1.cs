using DSS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace DSS.Actions
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

            string databaseName = Environment.GetEnvironmentVariable("actionDatabaseName").ToString();
            string containerName = Environment.GetEnvironmentVariable("actionContainerName").ToString();

            ItemResponse<Models.Action> actionObject = await _cosmos.GenericRetrieveDocument<Models.Action>(
                req.Headers["ActionId"].ToString(), databaseName, containerName
            );

            return new JsonResult(actionObject.Resource, new JsonSerializerOptions())
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}