using DSS.Models;
using Microsoft.Azure.Cosmos;

namespace DSS.Interfaces
{
    public interface ICosmosDbService
    {
        Task<ItemResponse<Notification>> GetNotificationDocument(string documentId, string databaseName, string containerName);
        Task<ItemResponse<Notification>> CreateNewNotificationDocument(string databaseName, string containerName);
        Task<ItemResponse<T>> GenericRetrieveDocument<T>(string documentId, string databaseName, string containerName);
    }
}
