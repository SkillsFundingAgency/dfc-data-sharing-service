using DSS.Interfaces;
using DSS.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DSS.SharedServices
{
    public class GenericCosmosDbService : IGenericCosmosDbService
    {
        private readonly CosmosClient _cosmosDbClient;
        private readonly ILogger<GenericCosmosDbService> _logger;
        private readonly ILogService _logService;

        public GenericCosmosDbService(CosmosClient cosmosClient, ILogger<GenericCosmosDbService> logger, ILogService logService)
        {
            _cosmosDbClient = cosmosClient;
            _logger = logger;
            _logService = logService;
        }

        public async Task<T> RetrieveDocumentAsync<T>(string documentId, string databaseName, string containerName)
        {
            _logService.LogMethodInvocation(nameof(RetrieveDocumentAsync).ToString());
            _logger.LogInformation($"Attempting to retrieve container '{containerName}' from database '{databaseName}'");

            Container cosmosDbContainer = _cosmosDbClient.GetContainer(databaseName, containerName);

            _logger.LogInformation($"Attempting to retrieve document with ID '{documentId}' from Cosmos DB");

            using (ResponseMessage response = await cosmosDbContainer.ReadItemStreamAsync(documentId, PartitionKey.None))
            {
                _logger.LogInformation($"Status code returned was '{((int)response.StatusCode)} - {response.StatusCode}'");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"No document with ID '{documentId}' could be found within Cosmos DB");
                    _logService.LogMethodExit(nameof(RetrieveDocumentAsync).ToString());
                    return default;
                }

                string content;
                using (StreamReader streamReader = new StreamReader(response.Content))
                {
                    content = await streamReader.ReadToEndAsync();
                }

                _logger.LogInformation($"A document with ID '{documentId}' was found within Cosmos DB");
                _logService.LogMethodExit(nameof(RetrieveDocumentAsync).ToString());
                return JsonConvert.DeserializeObject<T>(content);
            }
        }

        public async Task<T> CreateDocumentAsync<T>(T newDocumentObject, string databaseName, string containerName)
        {
            _logService.LogMethodInvocation(nameof(CreateDocumentAsync).ToString());
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
                    _logService.LogMethodExit(nameof(CreateDocumentAsync).ToString());
                    return default;
                }

                string content;
                using (StreamReader streamReader = new StreamReader(response.Content))
                {
                    content = await streamReader.ReadToEndAsync();
                }

                _logService.LogMethodExit(nameof(CreateDocumentAsync).ToString());
                return JsonConvert.DeserializeObject<T>(content);
            }
        }

        public async Task<T> ReplaceDocumentAsync<T>(T updatedDocumentObject, string existingDocumentId, string databaseName, string containerName)
        {
            _logService.LogMethodInvocation(nameof(ReplaceDocumentAsync).ToString());
            _logger.LogInformation($"Attempting to retrieve container '{containerName}' from database '{databaseName}'");

            Container cosmosDbContainer = _cosmosDbClient.GetContainer(databaseName, containerName);

            _logger.LogInformation($"Attempting to replace existing document with ID '{existingDocumentId}' within Cosmos DB");

            // Convert the document object into a Stream
            string jsonSerialized = JsonConvert.SerializeObject(updatedDocumentObject);
            byte[] jsonAsByteArray = System.Text.Encoding.UTF8.GetBytes(jsonSerialized);
            MemoryStream updatedDocumentAsStream = new MemoryStream(jsonAsByteArray);

            using (ResponseMessage response = await cosmosDbContainer.ReplaceItemStreamAsync(updatedDocumentAsStream, existingDocumentId, PartitionKey.None))
            {
                _logger.LogInformation($"Status code returned was '{((int)response.StatusCode)} - {response.StatusCode}'");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Unable to replace document within Cosmos DB");
                    _logService.LogMethodExit(nameof(ReplaceDocumentAsync).ToString());
                    return default;
                }

                string content;
                using (StreamReader streamReader = new StreamReader(response.Content))
                {
                    content = await streamReader.ReadToEndAsync();
                }

                _logger.LogInformation($"Document with ID '{existingDocumentId}' was replaced successfully");
                _logService.LogMethodExit(nameof(ReplaceDocumentAsync).ToString());
                return JsonConvert.DeserializeObject<T>(content);
            }
        }

        public bool IsCustomerReadOnly(Customer customer){
            return !(customer.DateOfTermination == null);
        }
    }
}
