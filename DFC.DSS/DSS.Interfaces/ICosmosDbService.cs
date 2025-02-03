namespace DSS.Interfaces
{
    public interface ICosmosDbService
    {
        Task<T> GenericRetrieveDocument<T>(string documentId, string databaseName, string containerName);
        Task<T> GenericCreateDocument<T>(T newDocumentObject, string databaseName, string containerName);
        Task<T> GenericReplaceDocument<T>(T updatedDocumentObject, string existingDocumentId, string databaseName, string containerName);
    }
}
