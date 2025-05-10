namespace SGHR.WebApi.Models.Habitacion
{
    public class RemoveHabitacionModel
    {
        public DateTime? ChangeDate { get; set; }
        public int? ChangeUser { get; set; }
        public int Id { get; set; }
        public bool Deleted { get; set; }

    }
}
