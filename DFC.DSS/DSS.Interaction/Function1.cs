using DSS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace DSS.Interaction
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly IGenericCosmosDbService _cosmos;

        public Function1(ILogger<Function1> logger, IGenericCosmosDbService cosmos)
        {
            _logger = logger;
            _cosmos = cosmos;
        }

        [Function("Function1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string databaseName = Environment.GetEnvironmentVariable("interactionDatabaseName").ToString();
            string containerName = Environment.GetEnvironmentVariable("interactionContainerName").ToString();

            Models.Interaction interactionObject = await _cosmos.RetrieveDocumentAsync<Models.Interaction>(
                req.Headers["InteractionId"].ToString(), databaseName, containerName
            );

            return new JsonResult(interactionObject, new JsonSerializerOptions())
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
