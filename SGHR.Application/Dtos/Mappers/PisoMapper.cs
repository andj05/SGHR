using SGHR.Application.Dtos.Pisos;
using SGHR.Domain.Entities.Configuration;

public static class PisoMapper
{
    // Convert Piso entity to PisoDto
    public static PisosDto ToDto(Piso entity)
    {
        return new PisosDto
        {
            IdPiso = entity.Id,
            Descripcion = entity.Descripcion,
            Estado = entity.Estado,
            ChangeDate = entity.ModifyDate,
            FechaCreacion = DateTime.UtcNow,
            ChangeUser = entity.ModifyUser
        };
    }

    // Convert SavePisoDto to a new Piso entity
    public static Piso ToEntity(SavePisosDto dto)
    {
        return new Piso
        {
            Descripcion = dto.Descripcion,
            Estado = dto.Estado,
            FechaCreacion = DateTime.UtcNow,
            CreationUser = dto.ChangeUser,
            Deleted = false
        };
    }

    // Update existing Piso entity from UpdatePisoDto
    public static void UpdateFromDto(this Piso entity, UpdatePisosDto dto)
    {
        entity.Descripcion = dto.Descripcion;
        entity.Estado = dto.Estado;
        entity.ModifyDate = dto.ChangeDate;
        entity.ModifyUser = dto.ChangeUser;
    }

    // Update existing Piso entity for Remove operation
    public static void RemoveFromDto(this Piso entity, RemovePisosDto dto)
    {
        entity.Estado = false;
        entity.Deleted = true;
        entity.ModifyDate = dto.ChangeDate;
        entity.ModifyUser = dto.ChangeUser;
    }

    // Update existing Piso entity for Restore operation
    public static void RestoreFromDto(this Piso entity, int userId)
    {
        entity.Estado = true;
        entity.Deleted = false;
        entity.ModifyDate = DateTime.Now;
        entity.ModifyUser = userId;
    }
}
