using DSS.ActionPlan.Models;
using DSS.ActionPlans.Interfaces;
using DSS.Interfaces;
using DSS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using DSS.Swagger.Annotations;

namespace DSS.ActionPlans.HTTP_Triggers
{
    public class GetActionPlansByCustomerId
    {
        private readonly ILogger<GetActionPlansByCustomerId> _logger;
        private readonly IHttpRequestService _httpRequestService;
        private readonly IGenericCosmosDbService _genericCosmosDbService;
        private readonly ICosmosDbService _localCosmosDbService;
        private readonly ILogService _logService;
        private readonly IDynamicConverterService _dynamicConverterService;

        #pragma warning disable 8602
        private readonly string customerDatabaseName = Environment.GetEnvironmentVariable("customerDatabaseName").ToString();
        private readonly string customerContainerName = Environment.GetEnvironmentVariable("customerContainerName").ToString();
        private readonly string actionPlanDatabaseName = Environment.GetEnvironmentVariable("actionPlanDatabaseName").ToString();
        private readonly string actionPlanContainerName = Environment.GetEnvironmentVariable("actionPlanContainerName").ToString();
        #pragma warning restore 8602

        public GetActionPlansByCustomerId(
            ILogger<GetActionPlansByCustomerId> logger,
            IHttpRequestService httpRequestService,
            IGenericCosmosDbService genericCosmosDbService,
            ICosmosDbService localCosmosDbService,
            ILogService logService,
            IDynamicConverterService dynamicConverterService
        ) {
            _logger = logger;
            _httpRequestService = httpRequestService;
            _genericCosmosDbService = genericCosmosDbService;
            _localCosmosDbService= localCosmosDbService;
            _logService = logService;
            _dynamicConverterService = dynamicConverterService;
        }

        [Function("GetActionPlansByCustomerId")]
        [ProducesResponseType(typeof(Models.ActionPlan), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Action Plans found", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Action Plans do not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Get", Description = "Ability to return all action plans for the given customer.")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers/{customerId}/ActionPlans")] HttpRequest req, string customerId)
        {
            _logService.LogFunctionInvocation(nameof(GetActionPlansByCustomerId));
            Guid correlationId = _httpRequestService.GetCorrelationId(req);

            _logger.LogInformation("Correlation GUID is '{correlationId}'", correlationId);
            string touchpointId = _httpRequestService.GetTouchpointId(req);

            if (string.IsNullOrWhiteSpace(touchpointId))
            {
                _logService.LogUnableToLocateInHeader("TouchpointId");
                _logService.LogFunctionExit(nameof(GetActionPlansByCustomerId), correlationId);
                return new BadRequestObjectResult("Unable to locate 'TouchpointId' in request header");
            }

            bool customerIdIsValidGuid = Guid.TryParse(customerId, out var customerGuid);
            bool queryParamsValidatedSuccessfully = customerIdIsValidGuid;

            if (!queryParamsValidatedSuccessfully)
            {
                _logger.LogWarning("Unrecognised or invalid entry identified. Customer ID '{customerId}'", customerId);
                _logService.LogFunctionExit(nameof(GetActionPlansByCustomerId), correlationId);
                return new BadRequestObjectResult("Unrecognised or invalid entry identified (Customer ID)");
            }

            _logger.LogInformation("HTTP request validation successful. Customer ID '{customerGuid}'", customerGuid);
            _logger.LogInformation("Attempting to check if the customer exists");

            Customer? customer = await _genericCosmosDbService.RetrieveDocumentAsync<Customer>(customerGuid.ToString(), customerDatabaseName, customerContainerName);
            if (customer == null)
            {
                _logger.LogWarning("Customer does not exist with ID '{customerGuid}'", customerGuid);
                _logService.LogFunctionExit(nameof(GetActionPlansByCustomerId), correlationId);
                return new NoContentResult();
            }

            _logger.LogInformation("Attempting to retrieve action plans belonging to the customer");
            List<Models.ActionPlan> actionPlanList = await _localCosmosDbService.RetrieveActionPlansForCustomerAsync(customerGuid, actionPlanDatabaseName, actionPlanContainerName);
            if (actionPlanList.Count() == 0)
            {
                _logger.LogWarning("Action Plans do not exist for customer with ID '{customerGuid}'", customerGuid);
                _logService.LogFunctionExit(nameof(GetActionPlansByCustomerId), correlationId);
                return new NoContentResult();
            } 
            else if (actionPlanList.Count() == 1)
            {
                _logService.LogFunctionExit(nameof(GetActionPlansByCustomerId), correlationId);
                return new JsonResult(_dynamicConverterService.RenameProperty(actionPlanList[0], "id", "ActionPlanId"),
                    new JsonSerializerOptions())
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
            }

            _logService.LogFunctionExit(nameof(GetActionPlansByCustomerId), correlationId);
            return new JsonResult(_dynamicConverterService.RenameProperty<Models.ActionPlan>(actionPlanList, "id", "ActionPlanId"),
                new JsonSerializerOptions())
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
