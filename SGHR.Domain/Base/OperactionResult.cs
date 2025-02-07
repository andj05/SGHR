
namespace SGHR.Domain.Base
{
    public class OperactionResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public dynamic? Data { get; set; }
    }
}
