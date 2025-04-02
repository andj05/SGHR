using SGHR.WebApi.ServicesApi.Interface;
using Microsoft.Extensions.Configuration;

namespace SGHR.WebApi.ServicesApi.Service
{
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
}

