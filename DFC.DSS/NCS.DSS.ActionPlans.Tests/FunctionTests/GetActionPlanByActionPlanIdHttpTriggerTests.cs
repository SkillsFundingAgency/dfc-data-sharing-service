using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using DSS.ActionPlans.HTTP_Triggers;
using DSS.Interfaces;
using DSS.Models;
using System.Net;
using Castle.Core.Resource;
using Microsoft.VisualBasic;
using DSS.SharedServices;

namespace DSS.ActionPlans.Tests.FunctionTests
{
    [TestFixture]
    public class GetActionPlanByActionPlanIdHttpTriggerTests
    {
        private const string validId = "7e467bdb-213f-407a-b86a-1954053d3c24";
        private Guid validGuid = new Guid("452d8e8c-2516-4a6b-9fc1-c85e578ac066");
        private const string invalidId = "1111111-2222-3333-4444-555555555555";
        private Customer validCustomer;
        private Models.Interaction validInteraction;
        private Models.ActionPlan validActionPlan;
        private const string validTouchpointId = "0000000001";

        private Mock<ILogService> _log;
        private HttpRequest _request;
        private Mock<IHttpRequestService> _httpRequestService;
        private Mock<ILogger<GetActionPlanByActionPlanId>> _logger;
        private Mock<IGenericCosmosDbService> _genericCosmosDbService;
        private GetActionPlanByActionPlanId _function;
        private DynamicConverterService _dynamicConverterService;

        [SetUp]
        public void Setup()
        {
            validCustomer = new Customer { CustomerId = new Guid(validId) };
            validInteraction = new Models.Interaction { InteractionId = new Guid(validId), CustomerId = new Guid(validId) };
            validActionPlan = new Models.ActionPlan { ActionPlanId = new Guid(validId), CustomerId = new Guid(validId) };
            Environment.SetEnvironmentVariable("customerDatabaseName", "customers");
            Environment.SetEnvironmentVariable("customerContainerName", "customers");
            Environment.SetEnvironmentVariable("interactionDatabaseName", "interactions");
            Environment.SetEnvironmentVariable("interactionContainerName", "interactions");
            Environment.SetEnvironmentVariable("actionPlanDatabaseName", "actionplans");
            Environment.SetEnvironmentVariable("actionPlanContainerName", "actionplans");
            _request = (new DefaultHttpContext()).Request;
            _log = new Mock<ILogService>();
            _httpRequestService = new Mock<IHttpRequestService>();
            _logger = new Mock<ILogger<GetActionPlanByActionPlanId>>();
            _genericCosmosDbService = new Mock<IGenericCosmosDbService>();
            _dynamicConverterService = new DynamicConverterService();
            _function = new GetActionPlanByActionPlanId(_logger.Object, _httpRequestService.Object, _genericCosmosDbService.Object, _log.Object, _dynamicConverterService);
        }

        [Test]
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeBadRequest_WhenTouchpointIdIsNotProvided()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns((string)null);
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validGuid);


            // Act
            var result = await RunFunction(invalidId, validId, validId);

            // Assert
            _logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == "Unable to locate 'TouchpointId' in request header"),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeBadRequest_WhenCustomerIdIsInvalid()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validGuid);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
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
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeBadRequest_WhenInteractionIdIsInvalid()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validGuid);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
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
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeBadRequest_WhenActionPlanIdIsInvalid()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validGuid);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
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
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validGuid);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult((Customer)null));
            string warning = $"Customer does not exist with ID '{validId}'";
            //Customer does not exist with ID '7e467bdb-213f-407a-b86a-1954053d3c24'

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
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeNoContent_WhenInteractionDoesNotExist()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validGuid);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Models.Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult((Models.Interaction)null));
            string warning = $"Interaction with ID '{validId}' does not exist";

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
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeNoContent_WhenActionPlanDoesNotExist()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validGuid);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Models.Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validInteraction));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Models.ActionPlan>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult((Models.ActionPlan)null));
            string warning = $"Action Plan does not exist with ID '{validId}'";

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
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeOk_WhenActionPlanExists()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validGuid);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns(validTouchpointId);
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Models.Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validInteraction));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Models.ActionPlan>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validActionPlan));

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