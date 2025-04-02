namespace SGHR.WebApi.PersistenApi.Interface
{
    public interface ILoggerManger<T>
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(Exception ex, string message);
    }
}

