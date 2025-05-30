using DSS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using DSS.ActionPlan.Models;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using DSS.Models;
using DSS.ActionPlans.Interfaces;
using DSS.Swagger.Annotations;

namespace DSS.ActionPlans.HTTP_Triggers
{
    public class PatchActionPlan
    {
        private readonly IDynamicConverterService _dynamicConverterService;
        private readonly ILogger<PatchActionPlan> _logger;
        private readonly IHttpRequestService _httpRequestService;
        private readonly IGenericCosmosDbService _genericCosmosDbService;
        private readonly ILogService _logService;
        private readonly ICosmosDbService _localCosmosDbService;
        private readonly IServiceBusService _serviceBusService;
        private readonly IValidate _validate;

        private readonly string customerDatabaseName = Environment.GetEnvironmentVariable("customerDatabaseName").ToString();
        private readonly string customerContainerName = Environment.GetEnvironmentVariable("customerContainerName").ToString();
        private readonly string interactionDatabaseName = Environment.GetEnvironmentVariable("interactionDatabaseName").ToString();
        private readonly string interactionContainerName = Environment.GetEnvironmentVariable("interactionContainerName").ToString();
        private readonly string actionPlanDatabaseName = Environment.GetEnvironmentVariable("actionPlanDatabaseName").ToString();
        private readonly string actionPlanContainerName = Environment.GetEnvironmentVariable("actionPlanContainerName").ToString();
        private readonly string sessionDatabaseName = Environment.GetEnvironmentVariable("sessionDatabaseName").ToString();
        private readonly string sessionContainerName = Environment.GetEnvironmentVariable("sessionContainerName").ToString();
        private readonly string serviceBusQueueName = Environment.GetEnvironmentVariable("serviceBusQueueName").ToString();

        public PatchActionPlan(
             IDynamicConverterService dynamicConverterService,
             IHttpRequestService httpRequestService,
             IGenericCosmosDbService genericCosmosDbService,
             ILogService logService,
             ILogger<PatchActionPlan> logger,
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

        [Function("PatchActionPlan")]
        [ProducesResponseType(typeof(Models.ActionPlan), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Action Plan Updated", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Action Plan does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Response(HttpStatusCode = 422, Description = "Action Plan validation error(s)", ShowSchema = false)]
        [Display(Name = "Patch", Description = "Ability to modify/update a customers action plan record. <br>" +
                                               "<br><b>Validation Rules:</b> <br>" +
                                               "<br><b>DateActionPlanCreated:</b> DateActionPlanCreated >= Session.DateAndTimeOfSession <br>" +
                                               "<br><b>DateActionPlanSentToCustomer:</b> DateActionPlanSentToCustomer >= DateActionPlanCreated <br>" +
                                               "<br><b>DateActionPlanAcknowledged:</b> DateActionPlanAcknowledged >= DateActionPlanCreated")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "Customers/{customerId}/Interactions/{interactionId}/ActionPlans/{actionPlanId}")] HttpRequest req, string customerId, string interactionId, string actionPlanId)
        {
            _logService.LogFunctionInvocation(nameof(PatchActionPlan));

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
                _logService.LogFunctionExit(nameof(PatchActionPlan), correlationId);
                return new BadRequestObjectResult("nable to locate 'apimurl' in request header");
            }

            var subcontractorId = _httpRequestService.GetSubcontractorId(req);
            if (string.IsNullOrEmpty(subcontractorId))
            {
                _logger.LogInformation("Unable to locate 'SubcontractorId' in request header");
            }

            bool customerIdIsValidGuid = Guid.TryParse(customerId, out var customerGuid);
            bool InteractionIdIsValidGuid = Guid.TryParse(interactionId, out var interactionGuid);
            bool ActionPlanIdIsValidGuid = Guid.TryParse(actionPlanId, out var actionPlanGuid);
            bool queryParamsValidatedSuccessfully = customerIdIsValidGuid && InteractionIdIsValidGuid && ActionPlanIdIsValidGuid;

            if (!queryParamsValidatedSuccessfully)
            {
                _logger.LogWarning("Unrecognised or invalid entry identified. Customer ID '{customerId}' , Interaction ID '{interactionId}' , Action Plan ID '{actionPlanId}'", customerId, interactionId, actionPlanId);
                _logService.LogFunctionExit(nameof(PatchActionPlan), correlationId);
                return new BadRequestObjectResult("Unrecognised or invalid entry identified (Customer ID, Interaction ID and/or Action Plan ID)");
            }

            _logger.LogInformation("HTTP request validation successful. Customer ID '{customerGuid}' , Interaction ID '{interactionGuid}' , Action Plan ID '{actionPlanGuid}'", customerId, interactionId, actionPlanId);

            ActionPlanPatch actionPlanPatchRequest;
            try
            {
                _logger.LogInformation("Attempting to get resource from body of the request");
                actionPlanPatchRequest = await _httpRequestService.GetResourceFromRequest<ActionPlanPatch>(req);
            }
            catch (Exception ex)
            {           
                _logger.LogError(ex, "Unable to read request body. Exception: {ExceptionMessage}", ex.Message);
                _logService.LogFunctionExit(nameof(PatchActionPlan), correlationId);
                return new UnprocessableEntityObjectResult(_dynamicConverterService.ExcludeProperty(ex, ["TargetSite"]));
            }

            if (actionPlanPatchRequest == null)
            {
                _logger.LogWarning("{actionPlanPatchRequest} object is NULL", nameof(actionPlanPatchRequest));
                _logService.LogFunctionExit(nameof(PatchActionPlan), correlationId);
                return new UnprocessableEntityObjectResult(req);
            }

            _logger.LogInformation("Retrieved resource from request body");
                        
            _logger.LogInformation("Attempting to set IDs for Action Plan PATCH");
            actionPlanPatchRequest.SetIds(touchpointId, subcontractorId);
            _logger.LogInformation("IDs successfully set for Action Plan PATCH");

            _logger.LogInformation("Attempting to check if customer exists. Customer GUID: {CustomerId}", customerGuid);
            Customer customer = await _genericCosmosDbService.RetrieveDocumentAsync<Customer>(customerGuid.ToString(), customerDatabaseName, customerContainerName);
            if (customer == null)
            {
                _logger.LogWarning("Customer does not exist with ID '{customerGuid}'", customerGuid);
                _logService.LogFunctionExit(nameof(PatchActionPlan), correlationId);
                return new NoContentResult();
            }
            _logger.LogInformation("Customer exists. Customer GUID: {CustomerGuid}", customerGuid);

            _logger.LogInformation("Attempting to check if customer is read-only. Customer GUID: {CustomerId}", customerGuid);
            var isCustomerReadOnly = _genericCosmosDbService.IsCustomerReadOnly(customer);

            if (isCustomerReadOnly)
            {
                var response = new ObjectResult(customerGuid)
                {
                    StatusCode = (int)HttpStatusCode.Forbidden
                };                
                _logger.LogWarning("Customer is read-only. Customer GUID: {CustomerId}", customerGuid);
                _logService.LogFunctionExit(nameof(PatchActionPlan), correlationId);
                return response;
            }
                        
            _logger.LogInformation("Attempting to get Interaction for Customer. Customer GUID: {CustomerId}. Interaction GUID: {InteractionGuid}", customerGuid, interactionGuid);
            Interaction interaction = await _genericCosmosDbService.RetrieveDocumentAsync<Interaction>(interactionGuid.ToString(), interactionDatabaseName, interactionContainerName);
            if (interaction == null)
            {
                _logger.LogWarning("Interaction does not exist. Customer GUID: {CustomerId}. Interaction GUID: {InteractionGuid}", customerGuid, interactionGuid);
                _logService.LogFunctionExit(nameof(PatchActionPlan), correlationId); 
                return new NoContentResult();
            }
            else if (interaction.CustomerId != customerGuid)
            {
                _logger.LogWarning("Interaction does not belong to the provided customer. Customer GUID: {CustomerId}. Interaction GUID: {InteractionGuid}", customerGuid, interactionGuid);
                _logService.LogFunctionExit(nameof(PatchActionPlan), correlationId); 
                return new NoContentResult();
            }
            _logger.LogInformation("Interaction exists and belongs to the provided customer. Customer GUID: {CustomerId}. Interaction GUID: {InteractionGuid}", customerGuid, interactionGuid);


            _logger.LogInformation("Attempting to get Action Plan for Customer. Customer GUID: {CustomerId}. Action Plan GUID: {ActionPlanGuid}", customerGuid, actionPlanGuid);
            List<Models.ActionPlan> actionPlanList = await _localCosmosDbService.RetrieveActionPlansForCustomerAsync(customerGuid, actionPlanDatabaseName, actionPlanContainerName);
            var actionPlanForCustomer = actionPlanList.FirstOrDefault(a => a.ActionPlanId == actionPlanGuid);
            if (actionPlanForCustomer == null)
            {                                
                _logger.LogWarning("Action Plan does not exist. Customer GUID: {CustomerId}. Action Plan GUID: {ActionPlanGuid}", customerGuid, actionPlanGuid);
                _logService.LogFunctionExit(nameof(PatchActionPlan), correlationId); 
                return new NoContentResult();
            }

            _logger.LogInformation("Attempting to PATCH Action Plan resource.");

            Models.ActionPlan patchedActionPlan = new Models.ActionPlan()
            {
                ActionPlanId = actionPlanForCustomer.ActionPlanId,
                CustomerId = actionPlanForCustomer.CustomerId,
                InteractionId = actionPlanForCustomer.InteractionId,
                SessionId = actionPlanPatchRequest.SessionId ?? actionPlanForCustomer.SessionId,
                CreatedBy = actionPlanForCustomer.CreatedBy,
                DateActionPlanCreated = actionPlanPatchRequest.DateActionPlanCreated ?? actionPlanForCustomer.DateActionPlanCreated,
                CustomerCharterShownToCustomer = actionPlanPatchRequest.CustomerCharterShownToCustomer ?? actionPlanForCustomer.CustomerCharterShownToCustomer,
                DateAndTimeCharterShown = actionPlanPatchRequest.DateAndTimeCharterShown ?? actionPlanForCustomer.DateAndTimeCharterShown,
                DateActionPlanSentToCustomer = actionPlanPatchRequest.DateActionPlanSentToCustomer ?? actionPlanForCustomer.DateActionPlanSentToCustomer,
                ActionPlanDeliveryMethod = actionPlanPatchRequest.ActionPlanDeliveryMethod ?? actionPlanForCustomer.ActionPlanDeliveryMethod,
                DateActionPlanAcknowledged = actionPlanPatchRequest.DateActionPlanAcknowledged ?? actionPlanForCustomer.DateActionPlanAcknowledged,
                CurrentSituation = actionPlanPatchRequest.CurrentSituation ?? actionPlanForCustomer.CurrentSituation,
                LastModifiedDate = actionPlanPatchRequest.LastModifiedDate ?? actionPlanForCustomer.LastModifiedDate,
                LastModifiedTouchpointId = actionPlanPatchRequest.LastModifiedTouchpointId ?? actionPlanForCustomer.LastModifiedTouchpointId,
                SubcontractorId = actionPlanPatchRequest.SubcontractorId ?? actionPlanForCustomer.SubcontractorId,
                CustomerSatisfaction = actionPlanPatchRequest.CustomerSatisfaction ?? actionPlanForCustomer.CustomerSatisfaction
            };

            Session session = await _genericCosmosDbService.RetrieveDocumentAsync<Session>(patchedActionPlan.SessionId.GetValueOrDefault().ToString(), sessionDatabaseName, sessionContainerName);

            _logger.LogInformation("Attempting to validate {ActionPlanValidationObject} object", nameof(patchedActionPlan));
            var errors = _validate.ValidateResource(patchedActionPlan, session.DateandTimeOfSession);
            if (errors != null && errors.Any())
            {
                var er = errors.Select(e => e.ErrorMessage).ToList();
                var response = new UnprocessableEntityObjectResult(errors);
                _logger.LogWarning("Falied to validate {ActionPlanValidationObject}", nameof(patchedActionPlan));
                _logService.LogFunctionExit(nameof(PatchActionPlan), correlationId); 
                return response;
            }
            _logger.LogInformation("Successfully validated {ActionPlanValidationObject}", nameof(patchedActionPlan));
                        
            _logger.LogInformation("Attempting to PATCH Action Plan in Cosmos DB. Action Plan GUID: {ActionPlanGuid}", actionPlanGuid);
            var updatedActionPlan = await _genericCosmosDbService.ReplaceDocumentAsync(patchedActionPlan, patchedActionPlan.ActionPlanId.ToString(), actionPlanDatabaseName, actionPlanContainerName);

            if (updatedActionPlan != null)
            {
                _logger.LogInformation("Successfully PATCHed Action Plan in Cosmos DB. Action Plan GUID: {ActionPlanGuid}", actionPlanGuid);

                var message = new
                {
                    TitleMessage = "Action Plan record modification for {" + updatedActionPlan.CustomerId + "} at " + DateTime.UtcNow,
                    CustomerGuid = updatedActionPlan.CustomerId,
                    updatedActionPlan.LastModifiedDate,
                    URL = apimUrl,
                    IsNewCustomer = false,
                    TouchpointId = updatedActionPlan.LastModifiedTouchpointId
                };

                _logger.LogInformation("Attempting to send message to Service Bus Namespace. Action Plan GUID: {ActionPlanGuid}", actionPlanGuid);
                await _serviceBusService.SendQueueMessageAsync(serviceBusQueueName, patchedActionPlan.CustomerId + " " + DateTime.UtcNow, message);
                _logger.LogInformation("Successfully sent message to Service Bus. Action Plan GUID: {ActionPlanGuid}", actionPlanGuid);
            }

            if (updatedActionPlan == null)
            {
                _logger.LogWarning("PATCH request unsuccessful. Action Plan GUID: {ActionPlanGuid}", actionPlanGuid);
                _logService.LogFunctionExit(nameof(PatchActionPlan), correlationId); 
                return new BadRequestObjectResult(actionPlanGuid);
            }

            _logService.LogFunctionExit(nameof(PatchActionPlan), correlationId);
            return new JsonResult(_dynamicConverterService.ExcludeProperty(updatedActionPlan, "CreatedBy"), new JsonSerializerOptions()) 
            { 
                StatusCode = (int)HttpStatusCode.OK 
            };
        }
    }
}