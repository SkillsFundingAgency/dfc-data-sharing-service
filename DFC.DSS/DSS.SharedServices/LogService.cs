using DSS.Interfaces;
using Microsoft.Extensions.Logging;

namespace DSS.SharedServices
{
    public class LogService : ILogService
    {
        private readonly ILogger<LogService> _logger;

        public LogService(ILogger<LogService> logger)
        {
            _logger = logger;
        }

        public void LogMethodExit(string nameOfMethod)
        {
            _logger.LogInformation("Method '{nameOfMethod}' has finished invocation", nameOfMethod);
        }
        public void LogMethodInvocation(string nameOfMethod)
        {
            _logger.LogInformation("Method '{nameOfMethod}' has been invocation", nameOfMethod);
        }

        public void LogFunctionExit(string nameOfFunction, Guid correlationId)
        {
            _logger.LogInformation("Function '{nameOfFunction}' has finished invocation. Correlation GUID: {CorrelationGuid}", nameOfFunction, correlationId);
        }

        public void LogFunctionInvocation(string nameOfFunction)
        {
            _logger.LogInformation("Function '{FunctionName}' has been invoked", nameOfFunction);
        }

        public void LogUnableToLocateInHeader(string nameOfHeader)
        {
            _logger.LogWarning("Unable to locate '{HeaderName}' in request header", nameOfHeader);
        }
    }
}
