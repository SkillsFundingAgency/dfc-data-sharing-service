using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DSS.ActionPlans.HTTP_Functions
{
    public class Patch
    {
        private readonly ILogger<Patch> _logger;

        public Patch(ILogger<Patch> logger)
        {
            _logger = logger;
        }

        [Function("Patch")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "patch")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
