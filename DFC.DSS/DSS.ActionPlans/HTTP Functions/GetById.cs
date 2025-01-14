using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DSS.ActionPlans.HTTP_Functions
{
    public class GetById
    {
        private readonly ILogger<GetById> _logger;

        public GetById(ILogger<GetById> logger)
        {
            _logger = logger;
        }

        [Function("GetById")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
