using SGHR.Application.Dtos.Servicios;
using SGHR.Domain.Entities.Configuration;

public static class ServiciosMapper
{
    // Convert Servicios entity to ServiciosDto
    public static ServiciosDto ToDto(Servicios entity)
    {
        return new ServiciosDto
        {
            IdServicio = entity.Id,
            Nombre = entity.Nombre,
            Descripcion = entity.Descripcion,
            Estado = entity.Estado,
            ChangeDate = entity.ModifyDate,
            ChangeUser = entity.ModifyUser
        };
    }

    // Convert SaveServiciosDto to a new Servicios entity
    public static Servicios ToEntity(SaveServiciosDto dto)
    {
        return new Servicios
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Estado = dto.Estado,
            FechaCreacion = DateTime.UtcNow,
            CreationUser = dto.ChangeUser,
            Deleted = false
        };
    }

    // Update existing Servicios entity from UpdateServiciosDto
    public static void UpdateFromDto(this Servicios entity, UpdateServiciosDto dto)
    {
        entity.Nombre = dto.Nombre;
        entity.Descripcion = dto.Descripcion;
        entity.Estado = dto.Estado;
        entity.ModifyDate = dto.ChangeDate;
        entity.ModifyUser = dto.ChangeUser;
    }

    // Update existing Servicios entity for Remove operation
    public static void RemoveFromDto(this Servicios entity, RemoveServiciosDto dto)
    {   
        entity.Estado = false;
        entity.Deleted = true;
        entity.ModifyDate = dto.ChangeDate;
        entity.ModifyUser = dto.ChangeUser;
    }

    // Update existing Servicios entity for Restore operation
    public static void RestoreFromDto(this Servicios entity, int userId)
    {   
        entity.Estado = true;
        entity.Deleted = false;
        entity.ModifyDate = DateTime.Now;
        entity.ModifyUser = userId;
    }
}
