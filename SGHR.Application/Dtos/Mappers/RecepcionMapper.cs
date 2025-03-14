using SGHR.Application.Dtos.Recepcion;
using SGHR.Domain.Entities.Reservation;

public static class RecepcionMapper
{
    //Convertir Recepcion a RecepcionDto
    public static RecepcionDto ToDto(Recepcion entity)
    {
        return new RecepcionDto
        {
            Id = entity.Id, 
            IdCliente = entity.IdCliente,
            IdHabitacion = entity.IdHabitacion,
            IdEstadoReserva = entity.IdEstadoReserva,
            FechaEntrada = entity.FechaEntrada,
            FechaSalida = entity.FechaSalida,
            FechaSalidaConfirmacion = entity.FechaSalidaConfirmacion,
            PrecioInicial = entity.PrecioInicial,
            Adelanto = entity.Adelanto,
            PrecioRestante = entity.PrecioRestante,
            TotalPagado = entity.TotalPagado,
            CostoPenalidad = entity.CostoPenalidad,
            Observacion = entity.Observacion,
            Estado = entity.Estado,
            ChangeDate = entity.FechaCreacion,
            ChangeUser = entity.CreationUser
        };
    }

    //convertir saverecepciondto a una nueva recepcion
    public static Recepcion ToEntity(SaveRecepcionDto dto)
    {
        return new Recepcion
        {
            IdCliente = dto.IdCliente,
            IdHabitacion = dto.IdHabitacion,
            IdEstadoReserva = dto.IdEstadoReserva,
            FechaEntrada = dto.FechaEntrada,
            FechaSalida = dto.FechaSalida,
            FechaSalidaConfirmacion = dto.FechaSalidaConfirmacion,
            PrecioInicial = dto.PrecioInicial,
            Adelanto = dto.Adelanto,
            PrecioRestante = dto.PrecioRestante,
            TotalPagado = dto.TotalPagado,
            CostoPenalidad = dto.CostoPenalidad,
            Observacion = dto.Observacion,
            FechaCreacion = DateTime.Now,
            CreationUser = dto.ChangeUser,
        };
    }

    //actualizar recepcion existente desde updaterecepciondto
    public static void UpdateFromDto(this Recepcion entity, UpdateRecepcionDto dto)
    {
        entity.IdCliente = dto.IdCliente;
        entity.IdHabitacion = dto.IdHabitacion;
        entity.IdEstadoReserva = dto.IdEstadoReserva;
        entity.FechaEntrada = dto.FechaEntrada;
        entity.FechaSalida = dto.FechaSalida;
        entity.FechaSalidaConfirmacion = dto.FechaSalidaConfirmacion;
        entity.PrecioInicial = dto.PrecioInicial;
        entity.Adelanto = dto.Adelanto;
        entity.PrecioRestante = dto.PrecioRestante;
        entity.TotalPagado = dto.TotalPagado;
        entity.CostoPenalidad = dto.CostoPenalidad;
        entity.Observacion = dto.Observacion;
        entity.ModifyDate = dto.ChangeDate ?? DateTime.Now;
        entity.ModifyUser = dto.ChangeUser;
    }

    //marcando como borrada recepcion existente desde removerecepciondto
    public static void RemoveFromDto(this Recepcion entity, RemoveRecepcionDto dto)
    {
        entity.Estado = false;
        entity.Deleted = true;
        entity.DeletedUser = dto.ChangeUser ?? 1;
    }

    //restaurando recepcion existente desde restorerecepciondto
    public static void RestoreFromDto(this Recepcion entity, int userId)
    {
        entity.Estado = true;
        entity.Deleted = false;
        entity.ModifyDate = DateTime.Now;
        entity.ModifyUser = userId;
        entity.DeletedUser = null;
    }
}
