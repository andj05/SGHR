namespace SGHR.Domain.Base
{
    public class OperationResult
    {
        public OperationResult()
        {
            this.Success = true;
            this.Data = new object();
        }
        public bool? Success { get; set; }
        public string? Message { get; set; }
        public dynamic Data { get; set; }
    }
}
