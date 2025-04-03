using SGHR.WebApi.ServicesApi.Interface;

namespace SGHR.WebApi.ServicesApi.Service
{
    public class LoggerManager<T> : ILoggerManager<T>
    {
        private readonly ILogger<T> _logger;

        public LoggerManager(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        public void LogError(Exception ex, string message)
        {
            _logger.LogError(ex, message);
        }
    }
}
