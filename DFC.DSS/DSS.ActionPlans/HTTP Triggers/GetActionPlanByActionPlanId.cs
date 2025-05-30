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
    public class GetActionPlanByActionPlanId
    {
        private readonly ILogger<GetActionPlanByActionPlanId> _logger;
        private readonly IHttpRequestService _httpRequestService;
        private readonly IGenericCosmosDbService _cosmosDbService;
        private readonly ILogService _logService;
        private readonly IDynamicConverterService _dynamicConverterService;

        private readonly string customerDatabaseName = Environment.GetEnvironmentVariable("customerDatabaseName").ToString();
        private readonly string customerContainerName = Environment.GetEnvironmentVariable("customerContainerName").ToString();
        private readonly string interactionDatabaseName = Environment.GetEnvironmentVariable("interactionDatabaseName").ToString();
        private readonly string interactionContainerName = Environment.GetEnvironmentVariable("interactionContainerName").ToString();
        private readonly string actionPlanDatabaseName = Environment.GetEnvironmentVariable("actionPlanDatabaseName").ToString();
        private readonly string actionPlanContainerName = Environment.GetEnvironmentVariable("actionPlanContainerName").ToString();

        public GetActionPlanByActionPlanId(
            ILogger<GetActionPlanByActionPlanId> logger, 
            IHttpRequestService httpRequestService, 
            IGenericCosmosDbService cosmosDbService,
            ILogService logService,
            IDynamicConverterService dynamicConverterService
        ) {
            _logger = logger;
            _httpRequestService = httpRequestService;
            _cosmosDbService = cosmosDbService;
            _logService = logService;
            _dynamicConverterService = dynamicConverterService;
        }

        [Function("GetActionPlanByActionPlanId")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers/{customerId}/Interactions/{interactionId}/ActionPlans/{actionPlanId}")] HttpRequest req, string customerId, string interactionId, string actionPlanId)
        {
            _logService.LogFunctionInvocation(nameof(GetActionPlanByActionPlanId));
            Guid correlationId = _httpRequestService.GetCorrelationId(req);

            _logger.LogInformation("Correlation GUID is '{correlationId}'", correlationId);
            string touchpointId = _httpRequestService.GetTouchpointId(req);

            if (string.IsNullOrWhiteSpace(touchpointId))
            {
                _logger.LogWarning("Unable to locate 'TouchpointId' in request header");
                _logService.LogFunctionExit(nameof(GetActionPlanByActionPlanId), correlationId);
                return new BadRequestObjectResult("Unable to locate 'TouchpointId' in request header");
            }

            bool customerIdIsValidGuid = Guid.TryParse(customerId, out var customerGuid);
            bool InteractionIdIsValidGuid = Guid.TryParse(interactionId, out var interactionGuid);
            bool ActionPlanIdIsValidGuid = Guid.TryParse(actionPlanId, out var actionPlanGuid);
            bool queryParamsValidatedSuccessfully = customerIdIsValidGuid && InteractionIdIsValidGuid && ActionPlanIdIsValidGuid;
            
            if (!queryParamsValidatedSuccessfully)
            {
                _logger.LogWarning("Unrecognised or invalid entry identified. Customer ID '{customerId}' , Interaction ID '{interactionId}' , Action Plan ID '{actionPlanId}'", customerId, interactionId, actionPlanId);
                _logService.LogFunctionExit(nameof(GetActionPlanByActionPlanId), correlationId);
                return new BadRequestObjectResult("Unrecognised or invalid entry identified (Customer ID, Interaction ID and/or Action Plan ID)");
            }

            _logger.LogInformation("HTTP request validation successful. Customer ID '{customerGuid}' , Interaction ID '{interactionGuid}' , Action Plan ID '{actionPlanGuid}'", customerId, interactionId, actionPlanId);
            _logger.LogInformation("Attempting to check if the customer exists");

            Customer customer = await _cosmosDbService.RetrieveDocumentAsync<Customer>(customerGuid.ToString(), customerDatabaseName, customerContainerName);
            if (customer == null)
            {
                _logger.LogWarning("Customer does not exist with ID '{customerGuid}'", customerGuid);
                _logService.LogFunctionExit(nameof(GetActionPlanByActionPlanId), correlationId);
                return new NoContentResult();
            }

            _logger.LogInformation("Attempting to check if the interaction exists and whether it belongs to the customer");
            
            Interaction interaction = await _cosmosDbService.RetrieveDocumentAsync<Interaction>(interactionGuid.ToString(), interactionDatabaseName, interactionContainerName);
            if (interaction == null) 
            {
                _logger.LogWarning("Interaction with ID '{interactionGuid}' does not exist", interactionGuid);
                _logService.LogFunctionExit(nameof(GetActionPlanByActionPlanId), correlationId);
                return new NoContentResult();
            } 
            else if (interaction.CustomerId != customerGuid)
            {
                _logger.LogWarning("Interaction with ID '{interactionGuid}' does not belong to customer with ID '{customerGuid}'", interactionGuid, customer);
                _logService.LogFunctionExit(nameof(GetActionPlanByActionPlanId), correlationId);
                return new NoContentResult();
            }

            _logger.LogInformation("Attempting to retrieve action plan and confirm whether it belongs to the customer");
            Models.ActionPlan actionPlan = await _cosmosDbService.RetrieveDocumentAsync<Models.ActionPlan>(actionPlanGuid.ToString(), actionPlanDatabaseName, actionPlanContainerName);
            if (actionPlan == null)
            {
                _logger.LogWarning("Action Plan does not exist with ID '{actionPlanGuid}'", actionPlanGuid);
                _logService.LogFunctionExit(nameof(GetActionPlanByActionPlanId), correlationId);
                return new NoContentResult();
            }
            else if (actionPlan.CustomerId != customerGuid)
            {
                _logger.LogWarning("Action Plan with ID '{actionPlanGuid}' does not belong to customer with ID '{customerGuid}'", actionPlanGuid, customerGuid);
                _logService.LogFunctionExit(nameof(GetActionPlanByActionPlanId), correlationId);
                return new NoContentResult();
            }

            _logService.LogFunctionExit(nameof(GetActionPlanByActionPlanId), correlationId);
            return new JsonResult(_dynamicConverterService.RenameProperty(actionPlan, "id", "ActionPlanId"),
                new JsonSerializerOptions())
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
