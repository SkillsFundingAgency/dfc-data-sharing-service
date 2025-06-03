using DSS.Models;

namespace DSS.Interfaces
{
    public interface IGenericCosmosDbService
    {
        Task<T?> RetrieveDocumentAsync<T>(string? documentId, string databaseName, string containerName);
        Task<T?> CreateDocumentAsync<T>(T newDocumentObject, string databaseName, string containerName);
        Task<T?> ReplaceDocumentAsync<T>(T updatedDocumentObject, string? existingDocumentId, string databaseName, string containerName);
        public bool IsCustomerReadOnly(Customer customer);
    }
}
