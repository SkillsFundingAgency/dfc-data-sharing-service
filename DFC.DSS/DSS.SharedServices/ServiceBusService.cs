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

        public ServiceBusService(ServiceBusClient serviceBusClient, ILogger<ServiceBusService> logger)
        {
            _serviceBusClient = serviceBusClient;
            _logger = logger;
        }

        public async Task<bool> SendQueueMessageAsync<T>(string queueName, string messageId, T messageBody)
        {
            _logger.LogInformation($"Method '{nameof(SendQueueMessageAsync)}' has been invoked");
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
                LogMethodExit(nameof(SendQueueMessageAsync));
                return false;
            }

            _logger.LogInformation($"Message was sent successfully");
            LogMethodExit(nameof(SendQueueMessageAsync));

            return true;
        }

        // Private helper methods
        private void LogMethodExit(string nameOfMethod)
        {
            _logger.LogInformation($"Method '{nameOfMethod}' has finished invocation");
        }
    }
}
