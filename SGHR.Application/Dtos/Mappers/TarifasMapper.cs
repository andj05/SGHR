using SGHR.Application.Dtos.Tarifas;
using SGHR.Domain.Entities.Configuration;

public static class TarifasMapper
{
    // Convertir Tarifas a TarifasDto
    public static TarifasDto ToDto(Tarifas entity)
    {
        return new TarifasDto
        {
            IdTarifa = entity.Id,
            IdHabitacion = entity.IdHabitacion,
            FechaInicio = entity.FechaInicio,
            FechaFin = entity.FechaFin,
            PrecioPorNoche = entity.PrecioPorNoche,
            Descuento = entity.Descuento,
            Descripcion = entity.Descripcion,
            Estado = entity.Estado,
            ChangeDate = entity.FechaCreacion,
            ChangeUser = entity.CreationUser
        };
    }

    // Convertir SaveTarifasDto a una nueva Tarifas
    public static Tarifas ToEntity(SaveTarifasDto dto)
    {
        return new Tarifas
        {
            IdHabitacion = dto.IdHabitacion,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin,
            PrecioPorNoche = dto.PrecioPorNoche,
            Descuento = dto.Descuento,
            Descripcion = dto.Descripcion,
            Estado = dto.Estado,
            FechaCreacion = DateTime.UtcNow,
            CreationUser = dto.ChangeUser,
            Deleted = false
        };
    }

    // Actualizar Tarifas existente desde UpdateTarifasDto
    public static void UpdateFromDto(this Tarifas entity, UpdateTarifasDto dto)
    {
        entity.IdHabitacion = dto.IdHabitacion;
        entity.FechaInicio = dto.FechaInicio;
        entity.FechaFin = dto.FechaFin;
        entity.PrecioPorNoche = dto.PrecioPorNoche;
        entity.Descuento = dto.Descuento;
        entity.Descripcion = dto.Descripcion;
        entity.Estado = dto.Estado;
        entity.ModifyDate = dto.ChangeDate ?? DateTime.Now;
        entity.ModifyUser = dto.ChangeUser;
    }

    // Marcar como borrada Tarifas existente desde RemoveTarifasDto
    public static void RemoveFromDto(this Tarifas entity, RemoveTarifasDto dto)
    {
        entity.Estado = false;
        entity.Deleted = true;
        entity.ModifyDate = dto.ChangeDate ?? DateTime.Now;
        entity.ModifyUser = dto.ChangeUser ?? 1;
    }

    // Restaurar Tarifas existente desde RestoreTarifasDto
    public static void RestoreFromDto(this Tarifas entity, int userId)
    {
        entity.Estado = true;
        entity.Deleted = false;
        entity.ModifyDate = DateTime.Now;
        entity.ModifyUser = userId;
    }
}
