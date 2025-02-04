using DSS.Models;

namespace DSS.ActionPlans.Interfaces
{
    public interface ICosmosDbService
    {
        Task<List<ActionPlan>> RetrieveActionPlansForCustomerAsync(Guid customerId, string databaseName, string containerName);
    }
}
