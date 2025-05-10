public class ErrorMessageService : IErrorMessageService
{
    private readonly IConfiguration _configuration;

    public ErrorMessageService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetErrorMessage(string category, string key)
    {
        return _configuration[$"ErrorMessages:{category}:{key}"];
    }
}