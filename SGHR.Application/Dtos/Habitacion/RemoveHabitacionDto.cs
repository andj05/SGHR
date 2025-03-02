namespace SGHR.Application.Dtos.Habitacion
{
    public class RemoveHabitacionDto : DtoBase
    {
        public int Id { get; set; }
        public bool Removed { get; set; } = false;
    }
}
