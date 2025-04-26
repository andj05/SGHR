
namespace SGHR.Domain.Base
{
    public class OperationResult
    {
        public OperationResult()
        {
            this.Success = true;
            Data = new object(); 
        }
        public string? Message { get; set; }
        public bool Success { get; set; }
        public dynamic Data { get; set; }

    }
}

