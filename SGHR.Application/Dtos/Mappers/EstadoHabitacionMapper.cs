using SGHR.Application.Dtos.EstadoHabitacion;

public static class EstadoHabitacionMapper
{
    // Convert EstadoHabitacion entity to EstadoHabitacionDto
    public static EstadoHabitacionDto ToDto(EstadoHabitacion entity)
    {
        return new EstadoHabitacionDto
        {
            IdEstadoHabitacion = entity.Id,
            Descripcion = entity.Descripcion,
            Estado = entity.Estado,
            FechaCreacion = DateTime.UtcNow,
            ChangeDate = entity.ModifyDate,
            ChangeUser = entity.ModifyUser
        };
    }

    // Convert SaveEstadoHabitacionDto to a new EstadoHabitacion entity
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

    // Update existing EstadoHabitacion entity from UpdateEstadoHabitacionDto
    public static void UpdateFromDto(this EstadoHabitacion entity, UpdateEstadoHabitacionDto dto)
    {
        entity.Descripcion = dto.Descripcion;
        entity.Estado = dto.Estado;
        entity.ModifyDate = dto.ChangeDate;
        entity.ModifyUser = dto.ChangeUser;
    }

    // Update existing EstadoHabitacion entity for Remove operation
    public static void RemoveFromDto(this EstadoHabitacion entity, RemoveEstadoHabitacionDto dto)
    {
        entity.Estado = false;
        entity.Deleted = true;
        entity.ModifyDate = dto.ChangeDate;
        entity.ModifyUser = dto.ChangeUser;
    }

    // Update existing EstadoHabitacion entity for Restore operation
    public static void RestoreFromDto(this EstadoHabitacion entity, int userId)
    {
        entity.Estado = true;
        entity.Deleted = false;
        entity.ModifyDate = DateTime.Now;
        entity.ModifyUser = userId;
    }
}
