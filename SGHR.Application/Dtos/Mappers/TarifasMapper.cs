using SGHR.Application.Dtos.Tarifas;
using SGHR.Domain.Entities.Configuration;

public static class TarifasMapper
{
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

        };
    }

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

    public static void UpdateFromDto(this Tarifas entity, UpdateTarifasDto dto)
    {
        entity.IdHabitacion = dto.IdHabitacion;
        entity.FechaInicio = dto.FechaInicio;
        entity.FechaFin = dto.FechaFin;
        entity.PrecioPorNoche = dto.PrecioPorNoche;
        entity.Descuento = dto.Descuento;
        entity.Descripcion = dto.Descripcion;
        entity.Estado = dto.Estado;
        entity.ModifyDate = dto.ChangeDate;
        entity.ModifyUser = dto.ChangeUser;
    }

    public static void RemoveFromDto(this Tarifas entity, RemoveTarifasDto dto)
    {
        entity.Estado = false;
        entity.Deleted = true;
        entity.ModifyDate = dto.ChangeDate;
        entity.ModifyUser = dto.ChangeUser;
    }

    public static void RestoreFromDto(this Tarifas entity, int userId)
    {
        entity.Estado = true;
        entity.Deleted = false;
        entity.ModifyDate = DateTime.Now;
        entity.ModifyUser = userId;
    }
}
