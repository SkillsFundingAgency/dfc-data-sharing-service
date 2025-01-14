using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DSS.ActionPlans.HTTP_Functions
{
    public class GetByCustomerId
    {
        private readonly ILogger<GetByCustomerId> _logger;

        public GetByCustomerId(ILogger<GetByCustomerId> logger)
        {
            _logger = logger;
        }

        [Function("GetByCustomerId")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
