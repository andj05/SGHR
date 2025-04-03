namespace SGHR.WebApi.ServicesApi.Interface
{
    public interface ILoggerManager<T>
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(Exception ex, string message);
    }
}
