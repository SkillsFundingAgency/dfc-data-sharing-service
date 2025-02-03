using DSS.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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

        public async Task<T> GenericRetrieveDocument<T>(string documentId, string databaseName, string containerName)
        {
            _logger.LogInformation($"Method '{nameof(GenericRetrieveDocument)}' has been invoked");
            _logger.LogInformation($"Attempting to retrieve container '{containerName}' from database '{databaseName}'");

            Container cosmosDbContainer = _cosmosDbClient.GetContainer(databaseName, containerName);

            _logger.LogInformation($"Attempting to retrieve document with ID '{documentId}' from Cosmos DB");

            using (ResponseMessage response = await cosmosDbContainer.ReadItemStreamAsync(documentId, PartitionKey.None))
            {
                _logger.LogInformation($"Status code returned was '{((int)response.StatusCode)} - {response.StatusCode}'");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"No document with ID '{documentId}' could be found within Cosmos DB");
                    LogMethodExit(nameof(GenericRetrieveDocument).ToString());
                    return default;
                }

                string content;
                using (StreamReader streamReader = new StreamReader(response.Content))
                {
                    content = await streamReader.ReadToEndAsync();
                }

                _logger.LogInformation($"A document with ID '{documentId}' was found within Cosmos DB");
                LogMethodExit(nameof(GenericRetrieveDocument).ToString());
                return JsonConvert.DeserializeObject<T>(content);
            }
        }

        public async Task<T> GenericCreateDocument<T>(T newDocumentObject, string databaseName, string containerName)
        {
            _logger.LogInformation($"Method '{nameof(GenericCreateDocument)}' has been invoked");
            _logger.LogInformation($"Attempting to retrieve container '{containerName}' from database '{databaseName}'");

            Container cosmosDbContainer = _cosmosDbClient.GetContainer(databaseName, containerName);

            _logger.LogInformation("Attempting to create a new document within Cosmos DB");

            // Convert the document object into a Stream
            string jsonSerialized = JsonConvert.SerializeObject(newDocumentObject);
            byte[] jsonAsByteArray = System.Text.Encoding.UTF8.GetBytes(jsonSerialized);
            MemoryStream documentAsStream = new MemoryStream(jsonAsByteArray);

            using (ResponseMessage response = await cosmosDbContainer.CreateItemStreamAsync(documentAsStream, PartitionKey.None))
            {
                _logger.LogInformation($"Status code returned was '{((int)response.StatusCode)} - {response.StatusCode}'");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Unable to create document within Cosmos DB");
                    LogMethodExit(nameof(GenericCreateDocument).ToString());
                    return default;
                }

                string content;
                using (StreamReader streamReader = new StreamReader(response.Content))
                {
                    content = await streamReader.ReadToEndAsync();
                }

                LogMethodExit(nameof(GenericCreateDocument).ToString());
                return JsonConvert.DeserializeObject<T>(content);
            }
        }

        // Private helper methods
        private void LogMethodExit(string nameOfMethod)
        {
            _logger.LogInformation($"Method '{nameOfMethod}' has finished invocation");
        }
    }
}
