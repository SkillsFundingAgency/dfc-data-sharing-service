using DSS.ActionPlan.Models;
using DSS.ActionPlan.Validation;
using DSS.ActionPlans.HTTP_Triggers;
using DSS.ActionPlans.Interfaces;
using DSS.Interfaces;
using DSS.Models;
using DSS.Models.Interfaces;
using DSS.SharedServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
namespace DSS.ActionPlans.Tests.FunctionTests
{
    [TestFixture]
    public class PostActionPlanHttpTriggerTests
    {
        private const string validId = "7e467bdb-213f-407a-b86a-1954053d3c24";
        private Guid validGuid = new Guid("452d8e8c-2516-4a6b-9fc1-c85e578ac066");
        private const string invalidId = "1111111-2222-3333-4444-555555555555";
        private Customer validCustomer;
        private Session validSession;
        private Interaction validInteraction;
        private DSS.Models.ActionPlan validActionPlan;
        private ActionPlanPatch validActionPlanRequest;
        private const string validTouchpointId = "0000000001";

        private Mock<ILogService> _log;
        private HttpRequest _request;
        private Mock<IHttpRequestService> _httpRequestService;
        private Mock<ILogger<PostActionPlan>> _logger;
        private Mock<IGenericCosmosDbService> _genericCosmosDbService;
        private Mock<ICosmosDbService> _actionPlansCosmosDbService;
        private PostActionPlan _function;
        private DynamicConverterService _dynamicConverterService;
        private IValidate _validate;
        private Mock<IServiceBusService> _serviceBusService;
        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("customerDatabaseName", "customers");
            Environment.SetEnvironmentVariable("customerContainerName", "customers");
            Environment.SetEnvironmentVariable("interactionDatabaseName", "interactions");
            Environment.SetEnvironmentVariable("interactionContainerName", "interactions");
            Environment.SetEnvironmentVariable("actionPlanDatabaseName", "actionplans");
            Environment.SetEnvironmentVariable("actionPlanContainerName", "actionplans");
            Environment.SetEnvironmentVariable("sessionDatabaseName", "sessions");
            Environment.SetEnvironmentVariable("sessionContainerName", "sessions");
            Environment.SetEnvironmentVariable("serviceBusQueueName", "dss.contentqueue");

            validCustomer = new Customer { CustomerId = new Guid(validId) };
            validInteraction = new Models.Interaction { InteractionId = new Guid(validId), CustomerId = new Guid(validId) };
            validActionPlan = new Models.ActionPlan { ActionPlanId = new Guid(validId), CustomerId = new Guid(validId), DateActionPlanCreated = DateTime.Now.AddDays(-1), SessionId = new Guid(validId), CustomerCharterShownToCustomer = true };
            validSession = new Models.Session { SessionId = new Guid(validId), CustomerId = new Guid(validId) };
            validActionPlanRequest = new ActionPlanPatch() { DateActionPlanCreated = DateTime.Now.AddDays(-1), SessionId = new Guid(validId) };


            _logger = new Mock<ILogger<PostActionPlan>>();
            _validate = new Validate();
            _request = (new DefaultHttpContext()).Request;
            _httpRequestService = new Mock<IHttpRequestService>();
            _genericCosmosDbService = new Mock<IGenericCosmosDbService>();
            _log = new Mock<ILogService>();
            _actionPlansCosmosDbService = new Mock<ICosmosDbService>();
            _dynamicConverterService = new DynamicConverterService();
            _serviceBusService = new Mock<IServiceBusService>();
            _function = new PostActionPlan(_logger.Object, _dynamicConverterService, _httpRequestService.Object, _genericCosmosDbService.Object, _log.Object, _actionPlansCosmosDbService.Object, _serviceBusService.Object, _validate);
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenTouchpointIdIsNotProvided()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns((string)null);
            string warning = "Unable to locate 'TouchpointId' in request header";

            // Act
            var result = await RunFunction(invalidId, validId);

            // Assert
            _logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == warning),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenApiurlIsNotProvided()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns((string)null);
            string warning = "Unable to locate 'apimURL' in request header";

            // Act
            var result = await RunFunction(invalidId, validId);

            // Assert
            _logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == warning),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenCustomerIdIsInvalid()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://localhost:");
            string warning = $"Unrecognised or invalid entry identified. Customer ID '{invalidId}' , Interaction ID '{validId}'";

            // Act
            var result = await RunFunction(invalidId, validId);

            // Assert
            _logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == warning),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenInteractionIdIsInvalid()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://localhost:");
            string warning = $"Unrecognised or invalid entry identified. Customer ID '{validId}' , Interaction ID '{invalidId}'";

            // Act
            var result = await RunFunction(validId, invalidId);

            // Assert
            _logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == warning),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenActionPlanRequestIsInvalid()
        {
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            _httpRequestService.Setup(x => x.GetResourceFromRequest<Models.ActionPlan>(_request)).Throws(new JsonException());
            string error = "Unable to read request body.";


            // Act
            var result = await RunFunction(validId, validId);

            // Assert
            _logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(error)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.That(result, Is.InstanceOf<UnprocessableEntityObjectResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult((Customer)null));
            _httpRequestService.Setup(x => x.GetResourceFromRequest<Models.ActionPlan>(_request)).Returns(Task.FromResult(validActionPlan));
            string warning = $"Customer does not exist with ID '{validId}'";

            // Act
            var result = await RunFunction(validId, validId);

            // Assert
            _logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == warning),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenInteractionDoesNotExist()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _httpRequestService.Setup(x => x.GetResourceFromRequest<Models.ActionPlan>(_request)).Returns(Task.FromResult(validActionPlan));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult((Interaction)null));
            string warning = $"Interaction does not exist. Customer GUID: {validId}. Interaction GUID: {validId}";

            // Act
            var result = await RunFunction(validId, validId);

            // Assert
            _logger.Verify(
               x => x.Log(
                   It.Is<LogLevel>(l => l == LogLevel.Warning),
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((v, t) => v.ToString() == warning),
                   It.IsAny<Exception>(),
                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenInteractionDoesNotBelongToCustomer()
        {
            // Arrange
            validInteraction.CustomerId = validGuid;
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _httpRequestService.Setup(x => x.GetResourceFromRequest<Models.ActionPlan>(_request)).Returns(Task.FromResult(validActionPlan));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validInteraction));
            string warning = $"Interaction does not belong to the provided customer. Customer GUID: {validId}. Interaction GUID: {validId}";

            // Act
            var result = await RunFunction(validId, validId);

            // Assert
            _logger.Verify(
               x => x.Log(
                   It.Is<LogLevel>(l => l == LogLevel.Warning),
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((v, t) => v.ToString() == warning),
                   It.IsAny<Exception>(),
                   It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenSessionDoesNotExist()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer)); _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validInteraction));
            _httpRequestService.Setup(x => x.GetResourceFromRequest<Models.ActionPlan>(_request)).Returns(Task.FromResult(validActionPlan));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Session>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult((Session)null));
            string warning = $"Session does not exist. Customer GUID: {validId}. Session GUID: {validId}";

            // Act
            var result = await RunFunction(validId, validId);

            // Assert
            _logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == warning),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenSessionDoesNotBelongToCustomer()
        {
            // Arrange
            validSession.CustomerId = validGuid;
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validInteraction));
            _httpRequestService.Setup(x => x.GetResourceFromRequest<Models.ActionPlan>(_request)).Returns(Task.FromResult(validActionPlan));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Session>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validSession));
            string warning = $"Session does not belong to the provided customer. Customer GUID: {validId}. Session GUID: {validId}";

            // Act
            var result = await RunFunction(validId, validId);

            // Assert
            _logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == warning),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenActionPlanHasFailedValidation()
        {
            // Arrange
            Models.ActionPlan _invalidActionPlan = new Models.ActionPlan() { DateActionPlanCreated = DateTime.Now.AddDays(-1), SessionId = new Guid(validId) };
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://localhost:");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validInteraction));
            _httpRequestService.Setup(x => x.GetResourceFromRequest<Models.ActionPlan>(_request)).Returns(Task.FromResult(_invalidActionPlan));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Session>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validSession));
            string warning = $"Failed to validate {nameof(ActionPlan)}";

            // Act
            var result = await RunFunction(validId, validId);

            // Assert
            _logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == warning),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.That(result, Is.InstanceOf<UnprocessableEntityObjectResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenUnableToCreateActionPlanRecord()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validInteraction));
            _httpRequestService.Setup(x => x.GetResourceFromRequest<Models.ActionPlan>(_request)).Returns(Task.FromResult(validActionPlan));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Session>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validSession));
            _genericCosmosDbService.Setup(x => x.CreateDocumentAsync(It.IsAny<Models.ActionPlan>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<DSS.Models.ActionPlan>(null));
            string warning = "POST request unsuccessful";
            // Act
            var result = await RunFunction(validId, validId);

            // Assert
            _logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(warning)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeCreated_WhenRequestIsValid()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validInteraction));
            _httpRequestService.Setup(x => x.GetResourceFromRequest<Models.ActionPlan>(_request)).Returns(Task.FromResult(validActionPlan));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Session>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validSession));
            _genericCosmosDbService.Setup(x => x.CreateDocumentAsync(It.IsAny<Models.ActionPlan>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validActionPlan));

            // Act
            var result = await RunFunction(validId, validId);
            var jsonResult = result as JsonResult;

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            Assert.That(jsonResult.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
        }
        private async Task<IActionResult> RunFunction(string customerId, string interactionId)
        {
            return await _function.Run(
                _request,
                customerId,
                interactionId).ConfigureAwait(false);
        }
    }
}