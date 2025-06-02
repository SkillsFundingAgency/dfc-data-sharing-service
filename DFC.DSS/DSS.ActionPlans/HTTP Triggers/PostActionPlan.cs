using DSS.Swagger.Annotations;
using DSS.ActionPlans.Interfaces;
using DSS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using DSS.Models;

namespace DSS.ActionPlans.HTTP_Triggers
{
    public class PostActionPlan
    {
        private readonly ILogger<PostActionPlan> _logger;
        private readonly IDynamicConverterService _dynamicConverterService;
        private readonly IHttpRequestService _httpRequestService;
        private readonly IGenericCosmosDbService _genericCosmosDbService;
        private readonly ILogService _logService;
        private readonly ICosmosDbService _localCosmosDbService;
        private readonly IServiceBusService _serviceBusService;
        private readonly IValidate _validate;

        #pragma warning disable 8602
        private readonly string customerDatabaseName = Environment.GetEnvironmentVariable("customerDatabaseName").ToString();
        private readonly string customerContainerName = Environment.GetEnvironmentVariable("customerContainerName").ToString();
        private readonly string interactionDatabaseName = Environment.GetEnvironmentVariable("interactionDatabaseName").ToString();
        private readonly string interactionContainerName = Environment.GetEnvironmentVariable("interactionContainerName").ToString();
        private readonly string actionPlanDatabaseName = Environment.GetEnvironmentVariable("actionPlanDatabaseName").ToString();
        private readonly string actionPlanContainerName = Environment.GetEnvironmentVariable("actionPlanContainerName").ToString();
        private readonly string sessionDatabaseName = Environment.GetEnvironmentVariable("sessionDatabaseName").ToString();
        private readonly string sessionContainerName = Environment.GetEnvironmentVariable("sessionContainerName").ToString();
        private readonly string serviceBusQueueName = Environment.GetEnvironmentVariable("serviceBusQueueName").ToString();
        #pragma warning restore 8602

        public PostActionPlan(
            ILogger<PostActionPlan> logger,
            IDynamicConverterService dynamicConverterService,
            IHttpRequestService httpRequestService,
            IGenericCosmosDbService genericCosmosDbService,
            ILogService logService,
            ICosmosDbService cosmosDbService,
            IServiceBusService serviceBusService,
            IValidate validate)
        {
            _dynamicConverterService = dynamicConverterService;
            _logger = logger;
            _httpRequestService = httpRequestService;
            _genericCosmosDbService = genericCosmosDbService;
            _logService = logService;
            _localCosmosDbService = cosmosDbService;
            _serviceBusService = serviceBusService;
            _validate = validate;
        }


        [Function("Post")]
        [ProducesResponseType(typeof(Models.ActionPlan), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Created, Description = "Action Plan Created", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Action Plan does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Response(HttpStatusCode = 422, Description = "Action Plan validation error(s)", ShowSchema = false)]
        [Display(Name = "Post", Description = "Ability to create a new action plan for a customer. <br>" +
                                              "<br><b>Validation Rules:</b> <br>" +
                                              "<br><b>DateActionPlanCreated:</b> DateActionPlanCreated >= Session.DateAndTimeOfSession <br>" +
                                              "<br><b>DateActionPlanSentToCustomer:</b> DateActionPlanSentToCustomer >= DateActionPlanCreated <br>" +
                                              "<br><b>DateActionPlanAcknowledged:</b> DateActionPlanAcknowledged >= DateActionPlanCreated")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customers/{customerId}/Interactions/{interactionId}/ActionPlans")] HttpRequest req, string customerId, string interactionId)
        {
            _logService.LogFunctionInvocation(nameof(PostActionPlan));

            Guid correlationId = _httpRequestService.GetCorrelationId(req);

            _logger.LogInformation("Correlation GUID is '{correlationId}'", correlationId);
            string touchpointId = _httpRequestService.GetTouchpointId(req);

            if (string.IsNullOrWhiteSpace(touchpointId))
            {
                _logger.LogWarning("Unable to locate 'TouchpointId' in request header");
                _logService.LogFunctionExit(nameof(PatchActionPlan), correlationId);
                return new BadRequestObjectResult("Unable to locate 'TouchpointId' in request header");
            }

            var apimUrl = _httpRequestService.GetApimUrl(req);
            if (string.IsNullOrEmpty(apimUrl))
            {
                _logger.LogWarning("Unable to locate 'apimURL' in request header");
                _logService.LogFunctionExit(nameof(PostActionPlan), correlationId);
                return new BadRequestObjectResult("Unable to locate 'apimURL' in request header");
            }

            var subcontractorId = _httpRequestService.GetSubcontractorId(req);
            if (string.IsNullOrEmpty(subcontractorId))
            {
                _logger.LogInformation("Unable to locate 'SubcontractorId' in request header");
            }

            bool customerIdIsValidGuid = Guid.TryParse(customerId, out var customerGuid);
            bool InteractionIdIsValidGuid = Guid.TryParse(interactionId, out var interactionGuid);
            bool queryParamsValidatedSuccessfully = customerIdIsValidGuid && InteractionIdIsValidGuid;

            if (!queryParamsValidatedSuccessfully)
            {
                _logger.LogWarning("Unrecognised or invalid entry identified. Customer ID '{customerId}' , Interaction ID '{interactionId}'", customerId, interactionId);
                _logService.LogFunctionExit(nameof(PostActionPlan), correlationId);
                return new BadRequestObjectResult("Unrecognised or invalid entry identified (Customer ID and/or Interaction ID)");
            }

            _logger.LogInformation("HTTP request validation successful. Customer ID '{customerGuid}' , Interaction ID '{interactionGuid}'", customerId, interactionId);

            Models.ActionPlan actionPlanRequest;
            try
            {                
                _logger.LogInformation("Attempting to get resource from body of the request");
                actionPlanRequest = await _httpRequestService.GetResourceFromRequest<Models.ActionPlan>(req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to read request body. Exception: {ExceptionMessage}", ex.Message);
                _logService.LogFunctionExit(nameof(PostActionPlan), correlationId);
                return new UnprocessableEntityObjectResult(_dynamicConverterService.ExcludeProperty(ex, ["TargetSite"]));               
            }

            if (actionPlanRequest == null)
            {
                _logger.LogWarning("{actionPlanRequest} object is NULL", nameof(actionPlanRequest));
                return new UnprocessableEntityObjectResult(req);
            }

            _logger.LogInformation("Retrieved resource from request body");

            _logger.LogInformation("Attempting to set IDs for Action Plan");
            actionPlanRequest.SetIds(customerGuid, interactionGuid, touchpointId, subcontractorId);
            _logger.LogInformation("IDs successfully set for Action Plan");

            _logger.LogInformation("Attempting to check if customer exists. Customer GUID: {CustomerId}", customerGuid);
            Customer? customer = await _genericCosmosDbService.RetrieveDocumentAsync<Customer>(customerGuid.ToString(), customerDatabaseName, customerContainerName);
            if (customer == null)
            {
                _logger.LogWarning("Customer does not exist with ID '{customerGuid}'", customerGuid);
                _logService.LogFunctionExit(nameof(GetActionPlansByCustomerId), correlationId);
                return new NoContentResult();
            }
            _logger.LogInformation("Customer exists. Customer GUID: {CustomerGuid}", customerGuid);

            _logger.LogInformation("Attempting to check if customer is read-only. Customer GUID: {CustomerId}", customerGuid);
            var isCustomerReadOnly = _genericCosmosDbService.IsCustomerReadOnly(customer);

            if (isCustomerReadOnly)
            {
                string warning = $"Customer is read-only. Customer GUID: {customerGuid}";
                var response = new ObjectResult(warning)
                {
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
                _logger.LogWarning(warning);
                _logService.LogFunctionExit(nameof(PostActionPlan), correlationId);
                return response;
            }

            _logger.LogInformation("Attempting to get Interaction for Customer. Customer GUID: {CustomerId}. Interaction GUID: {InteractionGuid}", customerGuid, interactionGuid);
            Interaction? interaction = await _genericCosmosDbService.RetrieveDocumentAsync<Interaction>(interactionGuid.ToString(), interactionDatabaseName, interactionContainerName);
            if (interaction == null)
            {
                _logger.LogWarning("Interaction does not exist. Customer GUID: {CustomerId}. Interaction GUID: {InteractionGuid}", customerGuid, interactionGuid);
                _logService.LogFunctionExit(nameof(PostActionPlan), correlationId);
                return new NoContentResult();
            }
            else if (interaction.CustomerId != customerGuid)
            {
                _logger.LogWarning("Interaction does not belong to the provided customer. Customer GUID: {CustomerId}. Interaction GUID: {InteractionGuid}", customerGuid, interactionGuid);
                _logService.LogFunctionExit(nameof(PostActionPlan), correlationId);
                return new NoContentResult();
            }
            _logger.LogInformation("Interaction exists and belongs to the provided customer. Customer GUID: {CustomerId}. Interaction GUID: {InteractionGuid}", customerGuid, interactionGuid);

            Session? session = await _genericCosmosDbService.RetrieveDocumentAsync<Session>(actionPlanRequest.SessionId.ToString(), sessionDatabaseName, sessionContainerName);
            if (session == null)
            {
                _logger.LogWarning("Session does not exist. Customer GUID: {CustomerId}. Session GUID: {SessionGuid}", customerGuid, actionPlanRequest.SessionId);
                _logService.LogFunctionExit(nameof(PatchActionPlan), correlationId);
                return new NoContentResult();
            }
            else if (session.CustomerId != customerGuid)
            {
                _logger.LogWarning("Session does not belong to the provided customer. Customer GUID: {CustomerId}. Session GUID: {SessionGuid}", customerGuid, actionPlanRequest.SessionId);
                _logService.LogFunctionExit(nameof(PatchActionPlan), correlationId);
                return new NoContentResult();
            }
            _logger.LogInformation("Session exists and belongs to the provided customer. Customer GUID: {CustomerId}. Session GUID: {SessionGuid}", customerGuid, actionPlanRequest.SessionId);


            _logger.LogInformation("Attempting to validate {ActionPlanRequest} object", nameof(ActionPlan));
            var errors = _validate.ValidateResource(actionPlanRequest, session.DateandTimeOfSession);
            if (errors != null && errors.Any())
            {
                var er = errors.Select(e => e.ErrorMessage).ToList();
                var response = new UnprocessableEntityObjectResult(errors);
                _logger.LogWarning("Failed to validate {ActionPlanRequest}", nameof(ActionPlan));
                _logService.LogFunctionExit(nameof(PostActionPlan), correlationId);
                return response;
            }
            _logger.LogInformation("Successfully validated {ActionPlanRequest}", nameof(ActionPlan));
                        
            _logger.LogInformation("Attempting to POST Action Plan in Cosmos DB. Action Plan GUID: {ActionPlanGuid}", actionPlanRequest.ActionPlanId);
            var actionPlan = await _genericCosmosDbService.CreateDocumentAsync(actionPlanRequest, actionPlanDatabaseName, actionPlanContainerName);

            if (actionPlan != null)
            {
                _logger.LogInformation("Successfully POSTed Action Plan in Cosmos DB. Action Plan GUID: {ActionPlanGuid}", actionPlan.ActionPlanId);

                var message = new
                {
                    TitleMessage = "New Action Plan record {" + actionPlan.ActionPlanId + "} added at " + DateTime.UtcNow,
                    CustomerGuid = actionPlan.CustomerId,
                    actionPlan.LastModifiedDate,
                    URL = apimUrl + "/" + actionPlan.ActionPlanId,
                    IsNewCustomer = false,
                    TouchpointId = actionPlan.LastModifiedTouchpointId
                };

                _logger.LogInformation("Attempting to send message to Service Bus Namespace. Action Plan GUID: {ActionPlanGuid}", actionPlan.ActionPlanId);
                await _serviceBusService.SendQueueMessageAsync(serviceBusQueueName, actionPlan.CustomerId + " " + DateTime.UtcNow, message);
                _logger.LogInformation("Successfully sent message to Service Bus. Action Plan GUID: {ActionPlanGuid}", actionPlan.ActionPlanId);
            }

            if (actionPlan == null)
            {
                _logger.LogWarning("POST request unsuccessful. Action Plan GUID: {ActionPlanGuid}", actionPlanRequest.ActionPlanId);
                _logService.LogFunctionExit(nameof(PostActionPlan), correlationId);
                return new BadRequestObjectResult(customerGuid);
            }

            _logService.LogFunctionExit(nameof(PostActionPlan), correlationId);
            return new JsonResult(_dynamicConverterService.RenameAndExcludeProperty(actionPlan, "id", "ActionPlanId", "CreatedBy"), new JsonSerializerOptions() { }) 
            { 
                StatusCode = (int)HttpStatusCode.Created 
            };
        }
    }
}