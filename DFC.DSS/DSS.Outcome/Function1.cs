using DSS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace DSS.Outcome
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

            string databaseName = Environment.GetEnvironmentVariable("outcomesDatabaseName").ToString();
            string containerName = Environment.GetEnvironmentVariable("outcomesContainerName").ToString();

            Models.Outcome outcomeObject = await _cosmos.RetrieveDocumentAsync<Models.Outcome>(
                req.Headers["OutcomeId"].ToString(), databaseName, containerName
            );

            return new JsonResult(outcomeObject, new JsonSerializerOptions())
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
