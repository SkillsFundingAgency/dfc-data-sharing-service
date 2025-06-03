using Azure.Messaging.ServiceBus;
using DSS.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DSS.SharedServices
{
    public class ServiceBusService : IServiceBusService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ILogger<ServiceBusService> _logger;
        private readonly ILogService _logService;

        public ServiceBusService(ServiceBusClient serviceBusClient, ILogger<ServiceBusService> logger, ILogService logService)
        {
            _serviceBusClient = serviceBusClient;
            _logger = logger;
            _logService = logService;
        }

        public async Task<bool> SendQueueMessageAsync<T>(string queueName, string messageId, T messageBody)
        {
            _logService.LogMethodInvocation(nameof(SendQueueMessageAsync));
            _logger.LogInformation($"Attempting to send message onto queue '{queueName}' with ID '{messageId}'");

            ServiceBusSender sender = _serviceBusClient.CreateSender(queueName);
            string jsonSerialized = JsonConvert.SerializeObject(messageBody);
            byte[] jsonAsByteArray = System.Text.Encoding.UTF8.GetBytes(jsonSerialized);

            ServiceBusMessage message = new ServiceBusMessage(jsonAsByteArray)
            {
                ContentType = "application/json",
                MessageId = messageId
            };

            try
            {
                await sender.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Unable to send message to queue. Exception: {ex.Message}");
                _logService.LogMethodExit(nameof(SendQueueMessageAsync));
                return false;
            }

            _logger.LogInformation($"Message was sent successfully");
            _logService.LogMethodExit(nameof(SendQueueMessageAsync));

            return true;
        }
    }
}
