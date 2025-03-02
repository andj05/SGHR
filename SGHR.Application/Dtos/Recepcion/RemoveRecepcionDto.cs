namespace SGHR.Application.Dtos.Recepcion
{
    public class RemoveRecepcionDto : DtoBase
    {
        public int Id { get; set; }
        public bool Removed { get; set; } = false;
    }
}
