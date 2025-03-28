using SGHR.Application.Dtos.EstadoHabitacion;

public static class EstadoHabitacionMapper
{
    // Convertir EstadoHabitacion a EstadoHabitacionDto
    public static EstadoHabitacionDto ToDto(EstadoHabitacion entity)
    {
        return new EstadoHabitacionDto
        {
            IdEstadoHabitacion = entity.Id,
            Descripcion = entity.Descripcion,
            Estado = entity.Estado,
            FechaCreacion = entity.FechaCreacion,
            ChangeDate = entity.ModifyDate,
            ChangeUser = entity.ModifyUser
        };
    }

    // Convertir SaveEstadoHabitacionDto a una nueva EstadoHabitacion
    public static EstadoHabitacion ToEntity(SaveEstadoHabitacionDto dto)
    {
        return new EstadoHabitacion
        {
            Descripcion = dto.Descripcion,
            Estado = dto.Estado,
            FechaCreacion = DateTime.UtcNow,
            CreationUser = dto.ChangeUser,
            Deleted = false
        };
    }

    // Actualizar EstadoHabitacion existente desde UpdateEstadoHabitacionDto
    public static void UpdateFromDto(this EstadoHabitacion entity, UpdateEstadoHabitacionDto dto)
    {
        entity.Descripcion = dto.Descripcion;
        entity.Estado = dto.Estado;
        entity.ModifyDate = dto.ChangeDate ?? DateTime.Now;
        entity.ModifyUser = dto.ChangeUser;
    }

    // Marcar como borrada EstadoHabitacion existente desde RemoveEstadoHabitacionDto
    public static void RemoveFromDto(this EstadoHabitacion entity, RemoveEstadoHabitacionDto dto)
    {
        entity.Estado = false;
        entity.Deleted = true;
        entity.ModifyDate = dto.ChangeDate ?? DateTime.Now;
        entity.ModifyUser = dto.ChangeUser ?? 1;
    }

    // Restaurar EstadoHabitacion existente desde RestoreEstadoHabitacionDto
    public static void RestoreFromDto(this EstadoHabitacion entity, int userId)
    {
        entity.Estado = true;
        entity.Deleted = false;
        entity.ModifyDate = DateTime.Now;
        entity.ModifyUser = userId;
    }
}


