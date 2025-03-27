namespace SGHR.WebApi.Models.Recepcion
{
    public class RemoveRecepcionModel
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime? ChangeDate { get; set; }
        public int? ChangeUser { get; set; }
    }
}
