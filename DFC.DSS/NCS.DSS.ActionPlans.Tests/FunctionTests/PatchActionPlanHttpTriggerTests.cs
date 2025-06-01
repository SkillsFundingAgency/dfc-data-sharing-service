using Azure.Core;
using Castle.Core.Resource;
using DSS.ActionPlan.Models;
using DSS.ActionPlan.Validation;
using DSS.ActionPlans.HTTP_Triggers;
using DSS.ActionPlans.Interfaces;
using DSS.Interfaces;
using DSS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace DSS.ActionPlans.Tests.FunctionTests
{
    [TestFixture]
    public class PatchActionPlanHttpTriggerTests
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
        private Mock<ILogger<PatchActionPlan>> _logger;
        private Mock<IGenericCosmosDbService> _genericCosmosDbService;
        private Mock<ICosmosDbService> _actionPlansCosmosDbService;
        private PatchActionPlan _function;
        private Mock<IDynamicConverterService> _dynamicConverterService;
        private IValidate _validate;
        private Mock<IServiceBusService> _serviceBusService;
        private ActionPlanPatch _actionPlanPatch;
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


            _logger = new Mock<ILogger<PatchActionPlan>>();
            _actionPlanPatch = new ActionPlanPatch();
            _validate = new Validate();
            _request = (new DefaultHttpContext()).Request;
            _httpRequestService = new Mock<IHttpRequestService>();
            _logger = new Mock<ILogger<PatchActionPlan>>();
            _genericCosmosDbService = new Mock<IGenericCosmosDbService>();
            _log = new Mock<ILogService>();
            _actionPlansCosmosDbService = new Mock<ICosmosDbService>();
            _dynamicConverterService = new Mock<IDynamicConverterService>();
            _serviceBusService = new Mock<IServiceBusService>();
            _function = new PatchActionPlan(_dynamicConverterService.Object, _httpRequestService.Object, _genericCosmosDbService.Object, _log.Object, _logger.Object, _actionPlansCosmosDbService.Object, _serviceBusService.Object, _validate);
        }
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenTouchpointIdIsNotProvided()
        {
            // arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns((string)null);
            string warning = "Unable to locate 'TouchpointId' in request header";

            // Act
            var result = await RunFunction(invalidId, validId, validId);

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
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenApiurlIsNotProvided()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns((string)null);
            string warning = "Unable to locate 'apimURL' in request header";

            // Act
            var result = await RunFunction(invalidId, validId, validId);

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
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenCustomerIdIsInvalid()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            string warning = $"Unrecognised or invalid entry identified. Customer ID '{invalidId}' , Interaction ID '{validId}' , Action Plan ID '{validId}'";


            // Act
            var result = await RunFunction(invalidId, validId, validId);

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
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenInteractionIdIsInvalid()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            string warning = $"Unrecognised or invalid entry identified. Customer ID '{validId}' , Interaction ID '{invalidId}' , Action Plan ID '{validId}'";

            // Act
            var result = await RunFunction(validId, invalidId, validId);

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
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenActionPlanIdIsInvalid()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            string warning = $"Unrecognised or invalid entry identified. Customer ID '{validId}' , Interaction ID '{validId}' , Action Plan ID '{invalidId}'";

            // Act
            var result = await RunFunction(validId, validId, invalidId);

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
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenActionPlanRequestIsInvalid()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            _httpRequestService.Setup(x => x.GetResourceFromRequest<ActionPlanPatch>(_request)).Throws(new JsonException());
            string error = "Unable to read request body.";


            // Act
            var result = await RunFunction(validId, validId, validId);

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
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult((Customer)null));
            _httpRequestService.Setup(x => x.GetResourceFromRequest<ActionPlanPatch>(_request)).Returns(Task.FromResult(_actionPlanPatch));
            string warning = $"Customer does not exist with ID '{validId}'";

            // Act
            var result = await RunFunction(validId, validId, validId);

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
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenInteractionDoesNotExist()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _httpRequestService.Setup(x => x.GetResourceFromRequest<ActionPlanPatch>(_request)).Returns(Task.FromResult(_actionPlanPatch));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult((Interaction)null));
            string warning = $"Interaction does not exist. Customer GUID: { validId }. Interaction GUID: { validId }";

            // Act
            var result = await RunFunction(validId, validId, validId);

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

        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenInteractionDoesNotBelongToCustomer()
        {
            // Arrange
            validInteraction.CustomerId = validGuid;
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _httpRequestService.Setup(x => x.GetResourceFromRequest<ActionPlanPatch>(_request)).Returns(Task.FromResult(_actionPlanPatch));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validInteraction));
            string warning = $"Interaction does not belong to the provided customer. Customer GUID: {validId}. Interaction GUID: {validId}";

            // Act
            var result = await RunFunction(validId, validId, validId);

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
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenSessionDoesNotExist()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer)); _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validInteraction));
            _httpRequestService.Setup(x => x.GetResourceFromRequest<ActionPlanPatch>(_request)).Returns(Task.FromResult(_actionPlanPatch));
            var listOfActionPlans = new List<DSS.Models.ActionPlan>() { validActionPlan, validActionPlan };
            _actionPlansCosmosDbService.Setup(x => x.RetrieveActionPlansForCustomerAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(listOfActionPlans));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Session>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult((Session)null));
            string warning = $"Session does not exist. Customer GUID: {validId}. Session GUID: {validId}";

            // Act
            var result = await RunFunction(validId, validId, validId);

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

        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenSessionDoesNotBelongToCustomer()
        {
            // Arrange
            validSession.CustomerId = validGuid;
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validInteraction));
            _httpRequestService.Setup(x => x.GetResourceFromRequest<ActionPlanPatch>(_request)).Returns(Task.FromResult(_actionPlanPatch));
            var listOfActionPlans = new List<DSS.Models.ActionPlan>() { validActionPlan, validActionPlan };
            _actionPlansCosmosDbService.Setup(x => x.RetrieveActionPlansForCustomerAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(listOfActionPlans));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Session>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validSession));
            string warning = $"Session does not belong to the provided customer. Customer GUID: {validId}. Session GUID: {validId}";

            // Act
            var result = await RunFunction(validId, validId, validId);

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
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenActionPlanDoesNotExist()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validInteraction));
            _httpRequestService.Setup(x => x.GetResourceFromRequest<ActionPlanPatch>(_request)).Returns(Task.FromResult(_actionPlanPatch));
            var listOfActionPlans = new List<Models.ActionPlan>() { };
            _actionPlansCosmosDbService.Setup(x => x.RetrieveActionPlansForCustomerAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(listOfActionPlans));
            string warning = $"Action Plan does not exist. Customer GUID: {validId}. Action Plan GUID: {validId}";

            // Act
            var result = await RunFunction(validId, validId, validId);

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
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenActionPlanHasFailedValidation()
        {
            // Arrange
            var actionPlanRequest = new ActionPlanPatch() { DateActionPlanCreated = DateTime.Now.AddDays(-1), SessionId = new Guid(validId) };
            var actionPlan = new DSS.Models.ActionPlan() { ActionPlanId = new Guid(validId), DateActionPlanCreated = DateTime.Now.AddDays(-1), SessionId = new Guid(validId) };
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validInteraction));
            _httpRequestService.Setup(x => x.GetResourceFromRequest<ActionPlanPatch>(_request)).Returns(Task.FromResult(actionPlanRequest));
            var listOfActionPlans = new List<DSS.Models.ActionPlan>() { actionPlan };
            _actionPlansCosmosDbService.Setup(x => x.RetrieveActionPlansForCustomerAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(listOfActionPlans));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Session>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validSession));
            string warning = $"Failed to validate {nameof(ActionPlan)}";

            // Action
            var result = await RunFunction(validId, validId, validId);

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
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenUnableToUpdateActionPlanRecord()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validInteraction));
            _httpRequestService.Setup(x => x.GetResourceFromRequest<ActionPlanPatch>(_request)).Returns(Task.FromResult(validActionPlanRequest));
            var listOfActionPlans = new List<DSS.Models.ActionPlan>() { validActionPlan };
            _actionPlansCosmosDbService.Setup(x => x.RetrieveActionPlansForCustomerAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(listOfActionPlans));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Session>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validSession));
            _genericCosmosDbService.Setup(x => x.ReplaceDocumentAsync(It.IsAny<Models.ActionPlan>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<DSS.Models.ActionPlan>(null));
            string warning = $"PATCH request unsuccessful. Action Plan GUID: {validId}";
                
            // Act
            var result = await RunFunction(validId, validId, validId);

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
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeOK_WhenRequestIsValid()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _httpRequestService.Setup(x => x.GetApimUrl(_request)).Returns("http://someurl.com");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validInteraction));
            _httpRequestService.Setup(x => x.GetResourceFromRequest<ActionPlanPatch>(_request)).Returns(Task.FromResult(validActionPlanRequest));
            var listOfActionPlans = new List<DSS.Models.ActionPlan>() { validActionPlan };
            _actionPlansCosmosDbService.Setup(x => x.RetrieveActionPlansForCustomerAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(listOfActionPlans));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Session>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validSession));
            _genericCosmosDbService.Setup(x => x.ReplaceDocumentAsync(It.IsAny<DSS.Models.ActionPlan>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validActionPlan));

            // Act
            var result = await RunFunction(validId, validId, validId);
            var jsonResult = result as JsonResult;

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            Assert.That(jsonResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        private async Task<IActionResult> RunFunction(string customerId, string interactionId, string actionPlanId)
        {
            return await _function.Run(
                _request,
                customerId,
                interactionId,
                actionPlanId).ConfigureAwait(false);
        }
    }
}