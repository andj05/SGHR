namespace SGHR.WebApi.ServicesApi.Interface
{
    public interface IErrorMessageService
    {
        string GetErrorMessage(string category, string key);
    }
}