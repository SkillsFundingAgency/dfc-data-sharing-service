using DSS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace DSS.AdviserDetails
{
    public class Function2
    {
        private readonly ILogger<Function2> _logger;
        private readonly ICosmosDbService _cosmos;

        public Function2(ILogger<Function2> logger, ICosmosDbService cosmos)
        {
            _logger = logger;
            _cosmos = cosmos;
        }

        [Function("Function2")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string databaseName = Environment.GetEnvironmentVariable("adviserDetailsDatabaseName").ToString();
            string containerName = Environment.GetEnvironmentVariable("adviserDetailsContainerName").ToString();

            Models.AdviserDetail newAdviserDetail = new Models.AdviserDetail()
            {
                AdviserName = "<NAME>",
                AdviserEmailAddress = "<EMAIL>",
                AdviserContactNumber = "<TEL>",
                LastModifiedDate = DateTime.Now,
                SubcontractorId = "",
                CreatedBy = "<TOUCHPOINT>"
            };

            Models.AdviserDetail adviserDetailObject = await _cosmos.GenericCreateDocument<Models.AdviserDetail>(
                newAdviserDetail, databaseName, containerName
            );

            _logger.LogInformation($"Document with ID '{adviserDetailObject.AdviserDetailId}' was created within Cosmos DB successfully");

            return new JsonResult(adviserDetailObject, new JsonSerializerOptions())
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
