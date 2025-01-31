using DSS.Models;
using Microsoft.Azure.Cosmos;

namespace DSS.Interfaces
{
    public interface ICosmosDbService
    {
        Task<ItemResponse<Notification>> GetNotificationDocument(string documentId);
        Task<ItemResponse<Notification>> CreateNewNotificationDocument();
    }
}
