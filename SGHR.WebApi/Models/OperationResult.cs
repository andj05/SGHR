namespace SGHR.WebApi.Models
{
    public class OperationResult
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public object Data { get; set; }
    }
}
