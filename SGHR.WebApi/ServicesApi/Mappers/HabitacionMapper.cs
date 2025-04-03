using SGHR.WebApi.Models.Habitacion;

namespace SGHR.WebApi.ServicesApi.Mappers
{
    public static class HabitacionMapper
    {
        public static HabitacionModel ToHabitacionModel(SaveHabitacionModel dto)
        {
            return new HabitacionModel
            {
                Numero = dto.Numero,
                Detalle = dto.Detalle,
                IdEstadoHabitacion = dto.IdEstadoHabitacion,
                IdPiso = dto.IdPiso,
                IdCategoria = dto.IdCategoria,
                ChangeDate = DateTime.Now,
                ChangeUser = dto.ChangeUser,
                Estado = true 
            };
        }

        public static UpdateHabitacionModel ToUpdateModel(HabitacionModel habitacion)
        {
            return new UpdateHabitacionModel
            {
                Id = habitacion.Id,
                Numero = habitacion.Numero,
                Detalle = habitacion.Detalle,
                IdEstadoHabitacion = habitacion.IdEstadoHabitacion,
                IdPiso = habitacion.IdPiso,
                IdCategoria = habitacion.IdCategoria,
                ChangeDate = habitacion.ChangeDate,
                ChangeUser = habitacion.ChangeUser
            };
        }

        public static RemoveHabitacionModel ToRemoveModel(HabitacionModel habitacion)
        {
            return new RemoveHabitacionModel
            {
                Id = habitacion.Id,
                ChangeDate = habitacion.ChangeDate,
                ChangeUser = habitacion.ChangeUser,
                Deleted = habitacion.Estado
            };
        }

        public static void MapFromUpdateModel(this HabitacionModel habitacion, UpdateHabitacionModel update)
        {
            habitacion.Numero = update.Numero;
            habitacion.Detalle = update.Detalle;
            habitacion.IdEstadoHabitacion = update.IdEstadoHabitacion;
            habitacion.IdPiso = update.IdPiso;
            habitacion.IdCategoria = update.IdCategoria;
            habitacion.ChangeDate = update.ChangeDate;
            habitacion.ChangeUser = update.ChangeUser;
        }
    }
}