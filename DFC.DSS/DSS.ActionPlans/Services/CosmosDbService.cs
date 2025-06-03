using DSS.ActionPlans.Interfaces;
using DSS.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;

namespace DSS.ActionPlans.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private readonly CosmosClient _cosmosDbClient;
        private readonly ILogger<CosmosDbService> _logger;
        private readonly ILogService _logService;

        public CosmosDbService(CosmosClient cosmosClient, ILogger<CosmosDbService> logger, ILogService logService)
        {
            _cosmosDbClient = cosmosClient;
            _logger = logger;
            _logService = logService;
        }

        public async Task<List<Models.ActionPlan>> RetrieveActionPlansForCustomerAsync(Guid customerId, string databaseName, string containerName)
        {
            _logger.LogInformation($"Method '{nameof(RetrieveActionPlansForCustomerAsync)}' has been invoked");
            _logger.LogInformation($"Attempting to retrieve container '{containerName}' from database '{databaseName}'");

            Container cosmosDbContainer = _cosmosDbClient.GetContainer(databaseName, containerName);
            List<Models.ActionPlan> actionPlanList = new List<Models.ActionPlan>();

            _logger.LogInformation($"Attempting to retrieve all Action Plan documents with CustomerId '{customerId}' from Cosmos DB");

            using (FeedIterator<Models.ActionPlan> setIterator = cosmosDbContainer
                .GetItemLinqQueryable<Models.ActionPlan>()
                .Where(actionPlan => actionPlan.CustomerId == customerId)
                .ToFeedIterator()
            )
            {
                while (setIterator.HasMoreResults)
                {
                    foreach (Models.ActionPlan actionPlan in await setIterator.ReadNextAsync())
                    {
                        actionPlanList.Add(actionPlan);
                    }
                }

                _logger.LogInformation($"Processing complete. '{actionPlanList.Count().ToString()}' document(s) matching the criteria have been retrieved");
                _logService.LogMethodExit(nameof(RetrieveActionPlansForCustomerAsync));

                return actionPlanList;
            }
        }
    }
}
