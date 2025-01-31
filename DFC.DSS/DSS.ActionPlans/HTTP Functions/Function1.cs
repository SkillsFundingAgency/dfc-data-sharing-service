using DSS.Interfaces;
using DSS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace DSS.ActionPlans.HTTP_Functions
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

            string databaseName = Environment.GetEnvironmentVariable("actionPlanDatabaseName").ToString();
            string containerName = Environment.GetEnvironmentVariable("actionPlanContainerName").ToString();

            ItemResponse<ActionPlan> actionPlanObject = await _cosmos.GenericRetrieveDocument<ActionPlan>(
                req.Headers["ActionPlanId"].ToString(), databaseName, containerName
            );

            return new JsonResult(actionPlanObject.Resource, new JsonSerializerOptions())
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}