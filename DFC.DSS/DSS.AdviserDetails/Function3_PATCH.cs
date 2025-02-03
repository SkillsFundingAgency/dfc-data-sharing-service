using DSS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace DSS.AdviserDetails
{
    public class Function3_PATCH
    {
        private readonly ILogger<Function3_PATCH> _logger;
        private readonly ICosmosDbService _cosmos;

        public Function3_PATCH(ILogger<Function3_PATCH> logger, ICosmosDbService cosmos)
        {
            _logger = logger;
            _cosmos = cosmos;
        }

        [Function("Function3")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "patch")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string databaseName = Environment.GetEnvironmentVariable("adviserDetailsDatabaseName").ToString();
            string containerName = Environment.GetEnvironmentVariable("adviserDetailsContainerName").ToString();

            string existingDocumentId = "<REPLACE>";

            Models.AdviserDetail updatedAdviserDetail = new Models.AdviserDetail()
            {
                AdviserDetailId = new Guid("<REPLACE>"),
                AdviserName = "<REPLACE>",
                AdviserEmailAddress = "<REPLACE>",
                AdviserContactNumber = "<REPLACE>",
                LastModifiedDate = DateTime.Now,
                SubcontractorId = "",
                CreatedBy = "<REPLACE>"
            };
            
            Models.AdviserDetail adviserDetailObject = await _cosmos.GenericReplaceDocument<Models.AdviserDetail>(
                updatedAdviserDetail, existingDocumentId, databaseName, containerName
            );

            return new JsonResult(adviserDetailObject, new JsonSerializerOptions())
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
