namespace DSS.Interfaces
{
    public interface IServiceBusService
    {
        Task<bool> SendQueueMessageAsync<T>(string queueName, string messageId, T messageBody);
    }
}
