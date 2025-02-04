using DSS.ActionPlans.Interfaces;
using DSS.Interfaces;
using DSS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace DSS.ActionPlans.HTTP_Triggers
{
    public class GetActionPlansByCustomerId
    {
        private readonly ILogger<GetActionPlansByCustomerId> _logger;
        private readonly IHttpRequestService _httpRequestService;
        private readonly IGenericCosmosDbService _genericCosmosDbService;
        private readonly ICosmosDbService _localCosmosDbService;
        private readonly ILogService _logService;

        private readonly string customerDatabaseName = Environment.GetEnvironmentVariable("customerDatabaseName").ToString();
        private readonly string customerContainerName = Environment.GetEnvironmentVariable("customerContainerName").ToString();
        private readonly string actionPlanDatabaseName = Environment.GetEnvironmentVariable("actionPlanDatabaseName").ToString();
        private readonly string actionPlanContainerName = Environment.GetEnvironmentVariable("actionPlanContainerName").ToString();

        public GetActionPlansByCustomerId(
            ILogger<GetActionPlansByCustomerId> logger,
            IHttpRequestService httpRequestService,
            IGenericCosmosDbService genericCosmosDbService,
            ICosmosDbService localCosmosDbService,
            ILogService logService
        ) {
            _logger = logger;
            _httpRequestService = httpRequestService;
            _genericCosmosDbService = genericCosmosDbService;
            _localCosmosDbService= localCosmosDbService;
            _logService = logService;
        }

        [Function("GetActionPlansByCustomerId")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers/{customerId}/ActionPlans")] HttpRequest req, string customerId)
        {
            _logger.LogInformation($"Function '{nameof(GetActionPlansByCustomerId)}' has been invoked");
            Guid correlationId = _httpRequestService.GetCorrelationId(req);

            _logger.LogInformation($"Correlation GUID is '{correlationId}'");
            string touchpointId = _httpRequestService.GetTouchpointId(req);

            if (string.IsNullOrWhiteSpace(touchpointId))
            {
                _logger.LogWarning("Unable to locate 'TouchpointId' in request header");
                _logService.LogFunctionExit(nameof(GetActionPlansByCustomerId));
                return new BadRequestObjectResult("Unable to locate 'TouchpointId' in request header");
            }

            bool customerIdIsValidGuid = Guid.TryParse(customerId, out var customerGuid);
            bool queryParamsValidatedSuccessfully = customerIdIsValidGuid;

            if (!queryParamsValidatedSuccessfully)
            {
                _logger.LogWarning($"Unrecognised or invalid entry identified. Customer ID '{customerId}'");
                _logService.LogFunctionExit(nameof(GetActionPlansByCustomerId));
                return new BadRequestObjectResult("Unrecognised or invalid entry identified (Customer ID)");
            }

            _logger.LogInformation($"HTTP request validation successful. Customer ID '{customerGuid}'");
            _logger.LogInformation("Attempting to check if the customer exists");

            Customer customer = await _genericCosmosDbService.RetrieveDocumentAsync<Customer>(customerGuid.ToString(), customerDatabaseName, customerContainerName);
            if (customer == null)
            {
                _logger.LogWarning($"Customer does not exist with ID '{customerGuid}'");
                _logService.LogFunctionExit(nameof(GetActionPlansByCustomerId));
                return new NoContentResult();
            }

            _logger.LogInformation("Attempting to retrieve action plans belonging to the customer");
            List<ActionPlan> actionPlanList = await _localCosmosDbService.RetrieveActionPlansForCustomerAsync(customerGuid, actionPlanDatabaseName, actionPlanContainerName);
            if (actionPlanList.Count() == 0)
            {
                _logger.LogWarning($"Action Plans do not exist for customer with ID '{customerGuid}'");
                _logService.LogFunctionExit(nameof(GetActionPlansByCustomerId));
                return new NoContentResult();
            } 
            else if (actionPlanList.Count() == 1)
            {
                _logService.LogFunctionExit(nameof(GetActionPlansByCustomerId));
                return new JsonResult(actionPlanList[0], new JsonSerializerOptions()) { StatusCode = (int)HttpStatusCode.OK };
            }

            _logService.LogFunctionExit(nameof(GetActionPlansByCustomerId));
            return new JsonResult(actionPlanList, new JsonSerializerOptions()) { StatusCode = (int)HttpStatusCode.OK };
        }
    }
}
