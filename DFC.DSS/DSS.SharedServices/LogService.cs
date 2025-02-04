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
            _logger.LogInformation($"Method '{nameOfMethod}' has finished invocation");
        }

        public void LogFunctionExit(string nameOfFunction)
        {
            _logger.LogInformation($"Function '{nameOfFunction}' has finished invocation");
        }
    }
}
