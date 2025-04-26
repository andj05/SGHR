using SGHR.Application.Dtos.Pisos;
using SGHR.Domain.Entities.Configuration;

public static class PisoMapper
{
    // Convertir Piso a PisosDto
    public static PisosDto ToDto(Piso entity)
    {
        return new PisosDto
        {
            IdPiso = entity.Id,
            Descripcion = entity.Descripcion,
            Estado = entity.Estado,
            ChangeDate = entity.ModifyDate,
            FechaCreacion = entity.FechaCreacion,
            ChangeUser = entity.ModifyUser
        };
    }

    // Convertir SavePisosDto a una nueva Piso
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

    // Actualizar Piso existente desde UpdatePisosDto
    public static void UpdateFromDto(this Piso entity, UpdatePisosDto dto)
    {
        entity.Descripcion = dto.Descripcion;
        entity.Estado = dto.Estado;
        entity.ModifyDate = dto.ChangeDate ?? DateTime.Now;
        entity.ModifyUser = dto.ChangeUser;
    }

    // Marcar como borrada Piso existente desde RemovePisosDto
    public static void RemoveFromDto(this Piso entity, RemovePisosDto dto)
    {
        entity.Estado = false;
        entity.Deleted = true;
        entity.ModifyDate = dto.ChangeDate ?? DateTime.Now;
        entity.ModifyUser = dto.ChangeUser ?? 1;
    }

    // Restaurar Piso existente desde RestorePisosDto
    public static void RestoreFromDto(this Piso entity, int userId)
    {
        entity.Estado = true;
        entity.Deleted = false;
        entity.ModifyDate = DateTime.Now;
        entity.ModifyUser = userId;
    }
}


