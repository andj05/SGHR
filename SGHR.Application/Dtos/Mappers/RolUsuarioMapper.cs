using SGHR.Application.Dtos.RolUsuario;
using SGHR.Domain.Entities.Configuration;

public static class RolUsuarioMapper
{
    // Convertir RolUsuario a RolUsuarioDto
    public static RolUsuarioDto ToDto(RolUsuario entity)
    {
        return new RolUsuarioDto
        {
            IdRolUsuario = entity.Id,
            Descripcion = entity.Descripcion,
            Estado = entity.Estado,
            FechaCreacion = entity.FechaCreacion,
            ChangeDate = entity.ModifyDate,
            ChangeUser = entity.ModifyUser
        };
    }

    // Convertir SaveRolUsuarioDto a una nueva RolUsuario
    public static RolUsuario ToEntity(SaveRolUsuarioDto dto)
    {
        return new RolUsuario
        {
            Descripcion = dto.Descripcion,
            Estado = dto.Estado,
            FechaCreacion = DateTime.UtcNow,
            CreationUser = dto.ChangeUser,
            Deleted = false
        };
    }

    // Actualizar RolUsuario existente desde UpdateRolUsuarioDto
    public static void UpdateFromDto(this RolUsuario entity, UpdateRolUsuarioDto dto)
    {
        entity.Descripcion = dto.Descripcion;
        entity.Estado = dto.Estado;
        entity.ModifyDate = dto.ChangeDate ?? DateTime.Now;
        entity.ModifyUser = dto.ChangeUser;
    }

    // Marcar como borrada RolUsuario existente desde RemoveRolUsuarioDto
    public static void RemoveFromDto(this RolUsuario entity, RemoveRolUsuarioDto dto)
    {
        entity.Estado = false;
        entity.Deleted = true;
        entity.ModifyDate = dto.ChangeDate ?? DateTime.Now;
        entity.ModifyUser = dto.ChangeUser ?? 1;
    }

    // Restaurar RolUsuario existente desde RestoreRolUsuarioDto
    public static void RestoreFromDto(this RolUsuario entity, int userId)
    {
        entity.Estado = true;
        entity.Deleted = false;
        entity.ModifyDate = DateTime.Now;
        entity.ModifyUser = userId;
    }
}

