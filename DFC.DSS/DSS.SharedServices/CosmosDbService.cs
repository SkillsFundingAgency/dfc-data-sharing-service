using DSS.Interfaces;
using DSS.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace DSS.SharedServices
{
    public class CosmosDbService : ICosmosDbService
    {
        private readonly CosmosClient _cosmosDbClient;
        private readonly ILogger<CosmosDbService> _logger;

        public CosmosDbService(CosmosClient cosmosClient, ILogger<CosmosDbService> logger)
        {
            _cosmosDbClient = cosmosClient;
            _logger = logger;
        }

        // As an example
        public async Task<ItemResponse<Notification>> GetNotificationDocument(string documentId)
        {
            _logger.LogInformation($"{nameof(GetNotificationDocument)} function has been invoked");
            Container cosmosDbNotificationContainer = _cosmosDbClient.GetContainer("<REPLACE>", "<REPLACE>");

            _logger.LogInformation("Attempting to create new document in Cosmos DB");

            ItemResponse<Notification> createRequestResponse = await cosmosDbNotificationContainer.ReadItemAsync<Notification>(documentId, PartitionKey.None);

            _logger.LogInformation($"{nameof(GetNotificationDocument)} function has finished invocation");

            return createRequestResponse;
        }

        public async Task<ItemResponse<Notification>> CreateNewNotificationDocument()
        {
            Notification newDoc = new Notification()
            {
                id = Guid.NewGuid().ToString(),
                CollectionId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                CustomerId = Guid.Parse("<REPLACE>"),
                LastModifiedDate = DateTime.Now,
                ResourceURL = new Uri("<REPLACE>"),
                TouchpointId = "<REPLACE>"
            };

            _logger.LogInformation($"{nameof(CreateNewNotificationDocument)} function has been invoked");
            Container cosmosDbNotificationContainer = _cosmosDbClient.GetContainer("<REPLACE>", "<REPLACE>");

            _logger.LogInformation("Attempting to create new document in Cosmos DB");

            ItemResponse<Notification> createRequestResponse = await cosmosDbNotificationContainer.CreateItemAsync(newDoc, PartitionKey.None);

            _logger.LogInformation($"{nameof(CreateNewNotificationDocument)} function has finished invocation");

            return createRequestResponse;
        }
    }
}
