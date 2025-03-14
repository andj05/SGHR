using SGHR.Application.Dtos.Habitacion;
using SGHR.Domain.Entities.Reservation;

public static class HabitacionMapper
{
    //Convertir habitacion a habitacion dto
    public static HabitacionDto ToDto(Habitacion entity)
    {
        return new HabitacionDto
        {
            Id = entity.Id, 
            IdPiso = entity.IdPiso,
            IdCategoria = entity.IdCategoria,
            Numero = entity.Numero,
            Detalle = entity.Detalle,
            IdEstadoHabitacion = entity.IdEstadoHabitacion,
            ChangeDate = entity.FechaCreacion, 
            ChangeUser = entity.CreationUser,  
            Estado = entity.Estado,
        };
    }
    //Convertir savehabitaciondto a una nueva habitacion
    public static Habitacion ToEntity(SaveHabitacionDto dto)
    {
        return new Habitacion
        {
            IdPiso = dto.IdPiso,
            IdCategoria = dto.IdCategoria,
            Numero = dto.Numero,
            Detalle = dto.Detalle,
            IdEstadoHabitacion = dto.IdEstadoHabitacion,
            FechaCreacion = DateTime.Now,
            CreationUser = dto.ChangeUser,
        };
    }

    //actualizar habitacion existente desde updatehabitaciondto
    public static void UpdateFromDto(this Habitacion entity, UpdateHabitacionDto dto)
    {
        entity.IdPiso = dto.IdPiso;
        entity.IdCategoria = dto.IdCategoria;
        entity.Numero = dto.Numero;
        entity.Detalle = dto.Detalle;
        entity.IdEstadoHabitacion = dto.IdEstadoHabitacion;
        entity.ModifyDate = dto.ChangeDate ?? DateTime.Now;
        entity.ModifyUser = dto.ChangeUser;
    }

    //marcando como borrada habitacion existente desde removehabitaciondto
    public static void RemoveFromDto(this Habitacion entity, RemoveHabitacionDto dto)
    {
        entity.Estado = false;
        entity.Deleted = true;
        entity.DeletedUser = dto.ChangeUser ?? 1;
    }

    //restaurando habitacion existente desde restorehabitaciondto
    public static void RestoreFromDto(this Habitacion entity, int userId)
    {
        entity.Estado = true;
        entity.Deleted = false;
        entity.ModifyDate = DateTime.Now;
        entity.ModifyUser = userId;
        entity.DeletedUser = null;
    }
}