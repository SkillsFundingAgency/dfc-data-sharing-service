namespace DSS.Interfaces
{
    public interface ICosmosDbService
    {
        Task<T> GenericRetrieveDocumentAsync<T>(string documentId, string databaseName, string containerName);
        Task<T> GenericCreateDocumentAsync<T>(T newDocumentObject, string databaseName, string containerName);
        Task<T> GenericReplaceDocumentAsync<T>(T updatedDocumentObject, string existingDocumentId, string databaseName, string containerName);
    }
}
