using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using DSS.ActionPlans.HTTP_Triggers;
using DSS.Interfaces;
using DSS.Models;
using System.Net;

namespace DSS.ActionPlans.Tests.FunctionTests
{
    [TestFixture]
    public class GetActionPlanByActionPlanIdHttpTriggerTests
    {
        private const string validCustomerId = "7E467BDB-213F-407A-B86A-1954053D3C24";
        private const string validInteractionId = "1e1a555c-9633-4e12-ab28-09ed60d51cb3";
        private const string validActionPlanId = "d5369b9a-6959-4bd3-92fc-1583e72b7e51";
        private Guid validDssCorrelationId = new Guid("452d8e8c-2516-4a6b-9fc1-c85e578ac066");
        private const string invalidId = "1111111-2222-3333-4444-555555555555";
        private Customer validCustomer;
        private Interaction validInteraction;
        private Models.ActionPlan validActionPlan;

        private Mock<ILogService> _log;
        private HttpRequest _request;
        private Mock<IHttpRequestService> _httpRequestService;
        private Mock<ILogger<GetActionPlanByActionPlanId>> _logger;
        private Mock<IGenericCosmosDbService> _genericCosmosDbService;
        private GetActionPlanByActionPlanId _function;

        [SetUp]
        public void Setup()
        {
            validCustomer = new Customer { CustomerId = new Guid(validCustomerId) };
            validInteraction = new Interaction { InteractionId = new Guid(validInteractionId), CustomerId = new Guid(validCustomerId) };
            validActionPlan = new Models.ActionPlan { ActionPlanId = new Guid(validActionPlanId), CustomerId = new Guid(validCustomerId) };
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
            _function = new GetActionPlanByActionPlanId(_logger.Object, _httpRequestService.Object, _genericCosmosDbService.Object, _log.Object);
        }

        [Test]
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeBadRequest_WhenTouchpointIdIsNotProvided()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns((string)null);
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validDssCorrelationId);


            // Act
            var result = await RunFunction(invalidId, validInteractionId, validActionPlanId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeBadRequest_WhenCustomerIdIsInvalid()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validDssCorrelationId);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns("0000000001");

            // Act
            var result = await RunFunction(invalidId, validInteractionId, validActionPlanId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeBadRequest_WhenInteractionIdIsInvalid()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validDssCorrelationId);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns("0000000001");

            // Act
            var result = await RunFunction(validCustomerId, invalidId, validActionPlanId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeBadRequest_WhenActionPlanIdIsInvalid()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validDssCorrelationId);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns("0000000001");

            // Act
            var result = await RunFunction(validCustomerId, validInteractionId, invalidId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validDssCorrelationId);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns("0000000001");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult((Customer)null));

            // Act
            var result = await RunFunction(validCustomerId, validInteractionId, validActionPlanId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeNoContent_WhenInteractionDoesNotExist()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validDssCorrelationId);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns("0000000001");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult((Interaction)null));

            // Act
            var result = await RunFunction(validCustomerId, validInteractionId, validActionPlanId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeNoContent_WhenActionPlanDoesNotExist()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validDssCorrelationId);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns("0000000001");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validInteraction));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Models.ActionPlan>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult((Models.ActionPlan)null));

            // Act
            var result = await RunFunction(validCustomerId, validInteractionId, validActionPlanId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeOk_WhenActionPlanExists()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validDssCorrelationId);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns("0000000001");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Interaction>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validInteraction));
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Models.ActionPlan>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validActionPlan));

            // Act
            var result = await RunFunction(validCustomerId, validInteractionId, validActionPlanId);
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