using Castle.Core.Resource;
using DSS.ActionPlans.HTTP_Triggers;
using DSS.ActionPlans.Interfaces;
using DSS.Interfaces;
using DSS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace DSS.ActionPlans.Tests.FunctionTests
{
    [TestFixture]
    public class GetActionPlansByCustomerIdHttpTriggerTest
    {
        private const string validIdString = "11111111-1111-1111-1111-111111111111"; 
        private const string invalidIdString = "1111118811-1111-1111-1111-111111111111";
        private Guid validGuid = new Guid();
        private Customer validCustomer;
        private Models.ActionPlan validActionPlan;

        private Mock<ILogService> _log;
        private HttpRequest _request;
        private Mock<IHttpRequestService> _httpRequestService;
        private Mock<ILogger<GetActionPlansByCustomerId>> _logger;
        private Mock<IGenericCosmosDbService> _genericCosmosDbService;
        private GetActionPlansByCustomerId _function;
        private Mock<ICosmosDbService> _actionPlansCosmosDbService;
        private Mock<IDynamicConverterService> _dynamicConverterService;

        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("customerDatabaseName", "customers");
            Environment.SetEnvironmentVariable("customerContainerName", "customers");
            Environment.SetEnvironmentVariable("actionPlanDatabaseName", "actionplans");
            Environment.SetEnvironmentVariable("actionPlanContainerName", "actionplans");

            validCustomer = new Customer { CustomerId = validGuid };
            validActionPlan = new Models.ActionPlan { ActionPlanId = validGuid, CustomerId = validGuid };

            _request = (new DefaultHttpContext()).Request;
            _httpRequestService = new Mock<IHttpRequestService>();
            _logger = new Mock<ILogger<GetActionPlansByCustomerId>>();
            _genericCosmosDbService = new Mock<IGenericCosmosDbService>();
            _log = new Mock<ILogService>();
            _actionPlansCosmosDbService = new Mock<ICosmosDbService>();
            _dynamicConverterService = new Mock<IDynamicConverterService>();
            _function = new GetActionPlansByCustomerId(_logger.Object, _httpRequestService.Object, _genericCosmosDbService.Object, _actionPlansCosmosDbService.Object, _log.Object, _dynamicConverterService.Object);
        }

        [Test]
        public async Task GetActionPlanHttpTrigger_ReturnsStatusCodeBadResult_WhenCustomerIdIsInvalid()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validGuid);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns("0000000001");
            string warning = $"Unrecognised or invalid entry identified. Customer ID '{invalidIdString}'";

            // Act
            var result = await RunFunction(invalidIdString);

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
        public async Task GetActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validGuid);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns("0000000001");
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult((Customer)null));
            string warning = $"Customer does not exist with ID '{validIdString}'";

            // Act
            var result = await RunFunction(validIdString);

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
        public async Task GetActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenActionPlanDoesNotExist()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validGuid);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns("0000000001");
            var listOfActionPlans = new List<Models.ActionPlan>() { };
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _actionPlansCosmosDbService.Setup(x => x.RetrieveActionPlansForCustomerAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(listOfActionPlans));
            string warning = $"Action Plans do not exist for customer with ID '{validIdString}'";

            // Act
            var result = await RunFunction(validIdString);

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
        public async Task GetActionPlanHttpTrigger_ReturnsStatusCodeOk_When1ActionPlanExists()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validGuid);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns("0000000001");
            var listOfActionPlans = new List<Models.ActionPlan>() { validActionPlan };
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _actionPlansCosmosDbService.Setup(x => x.RetrieveActionPlansForCustomerAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(listOfActionPlans));

            // Act
            var result = await RunFunction(validIdString);
            var jsonResult = result as JsonResult;

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            Assert.That(jsonResult.Value, Is.TypeOf<Models.ActionPlan>());
            Assert.That(jsonResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task GetActionPlanHttpTrigger_ReturnsStatusCodeOk_WhenMultipleActionPlansExist()
        {
            // Arrange
            _httpRequestService.Setup(x => x.GetCorrelationId(_request)).Returns(validGuid);
            _httpRequestService.Setup(x => x.GetTouchpointId(_request)).Returns("0000000001");
            var listOfActionPlans = new List<Models.ActionPlan>() { validActionPlan, validActionPlan };
            _genericCosmosDbService.Setup(x => x.RetrieveDocumentAsync<Customer>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(validCustomer));
            _actionPlansCosmosDbService.Setup(x => x.RetrieveActionPlansForCustomerAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(listOfActionPlans));

            // Act
            var result = await RunFunction(validIdString);
            var jsonResult = result as JsonResult;

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            Assert.That(jsonResult.Value, Is.TypeOf<List<Models.ActionPlan>>());
            Assert.That(jsonResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }



        private async Task<IActionResult> RunFunction(string customerId)
        {
            return await _function.Run(
                _request,
                customerId).ConfigureAwait(false);
        }
    }
}