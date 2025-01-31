using DSS.Interfaces;
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

        /*public async Task<ItemResponse<T>> CreateNewDocument(string databaseName, string containerName, Notification newDocument)
        {
            _logger.LogInformation($"{nameof(CreateNewNotificationDocument)} function has been invoked");
            Container cosmosDbContainer = _cosmosDbClient.GetContainer(databaseName, containerName);

            _logger.LogInformation("Attempting to create new document in Cosmos DB");

            ItemResponse<Notification> createRequestResponse = await cosmosDbContainer.CreateItemAsync(newDocument, PartitionKey.None);

            _logger.LogInformation($"{nameof(CreateNewNotificationDocument)} function has finished invocation");

            return createRequestResponse;
        }*/
    }
}
