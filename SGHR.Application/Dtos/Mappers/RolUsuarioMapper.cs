using SGHR.Application.Dtos.RolUsuario;
using SGHR.Domain.Entities.Configuration;

public static class RolUsuarioMapper
{
    // Convert RolUsuario entity to RolUsuarioDto
    public static RolUsuarioDto ToDto(RolUsuario entity)
    {
        return new RolUsuarioDto
        {
            IdRolUsuario = entity.Id,
            Descripcion = entity.Descripcion,
            Estado = entity.Estado,
            ChangeData = entity.ModifyDate,
            ChangeUser = entity.ModifyUser
        };
    }

    // Convert SaveRolUsuarioDto to a new RolUsuario entity
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

    // Update existing RolUsuario entity from UpdateRolUsuarioDto
    public static void UpdateFromDto(this RolUsuario entity, UpdateRolUsuarioDto dto)
    {
        entity.Descripcion = dto.Descripcion;
        entity.Estado = dto.Estado;
        entity.ModifyDate = dto.ChangeData;
        entity.ModifyUser = dto.ChangeUser;
    }

    // Update existing RolUsuario entity for Remove operation
    public static void RemoveFromDto(this RolUsuario entity, RemoveRolUsuarioDto dto)
    {
        entity.Estado = false;
        entity.Deleted = true;
        entity.ModifyDate = dto.ChangeData;
        entity.ModifyUser = dto.ChangeUser;
    }

    // Update existing RolUsuario entity for Restore operation
    public static void RestoreFromDto(this RolUsuario entity, int userId)
    {
        entity.Estado = true;
        entity.Deleted = false;
        entity.ModifyDate = DateTime.Now;
        entity.ModifyUser = userId;
    }
}
