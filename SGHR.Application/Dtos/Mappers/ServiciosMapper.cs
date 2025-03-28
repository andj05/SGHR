using SGHR.Application.Dtos.Servicios;
using SGHR.Domain.Entities.Configuration;

public static class ServiciosMapper
{
    // Convertir Servicios a ServiciosDto
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

    // Convertir SaveServiciosDto a una nueva Servicios
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

    // Actualizar Servicios existente desde UpdateServiciosDto
    public static void UpdateFromDto(this Servicios entity, UpdateServiciosDto dto)
    {
        entity.Nombre = dto.Nombre;
        entity.Descripcion = dto.Descripcion;
        entity.Estado = dto.Estado;
        entity.ModifyDate = dto.ChangeDate ?? DateTime.Now;
        entity.ModifyUser = dto.ChangeUser;
    }

    // Marcar como borrada Servicios existente desde RemoveServiciosDto
    public static void RemoveFromDto(this Servicios entity, RemoveServiciosDto dto)
    {
        entity.Estado = false;
        entity.Deleted = true;
        entity.ModifyDate = dto.ChangeDate ?? DateTime.Now;
        entity.ModifyUser = dto.ChangeUser ?? 1;
    }

    // Restaurar Servicios existente desde RestoreServiciosDto
    public static void RestoreFromDto(this Servicios entity, int userId)
    {
        entity.Estado = true;
        entity.Deleted = false;
        entity.ModifyDate = DateTime.Now;
        entity.ModifyUser = userId;
    }
}
