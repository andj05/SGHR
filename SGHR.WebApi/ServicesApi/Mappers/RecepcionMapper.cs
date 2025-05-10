using SGHR.WebApi.Models.Recepcion;

namespace SGHR.WebApi.ServicesApi.Mappers
{
    public static class RecepcionMapper
    {
        public static UpdateRecepcionModel ToUpdateModel(RecepcionModel recepcion)
        {
            return new UpdateRecepcionModel
            {
                Id = recepcion.Id,
                IdCliente = recepcion.IdCliente,
                IdHabitacion = recepcion.IdHabitacion,
                IdEstadoReserva = recepcion.IdEstadoReserva,
                FechaEntrada = recepcion.FechaEntrada,
                FechaSalida = recepcion.FechaSalida,
                FechaSalidaConfirmacion = recepcion.FechaSalidaConfirmacion,
                PrecioInicial = recepcion.PrecioInicial,
                Adelanto = recepcion.Adelanto,
                PrecioRestante = recepcion.PrecioRestante,
                TotalPagado = recepcion.TotalPagado,
                CostoPenalidad = recepcion.CostoPenalidad,
                Observacion = recepcion.Observacion,
                Estado = recepcion.Estado,
                ChangeDate = recepcion.ChangeDate,
                ChangeUser = recepcion.ChangeUser
            };
        }

        public static RemoveRecepcionModel ToRemoveModel(RecepcionModel recepcion)
        {
            return new RemoveRecepcionModel
            {
                Id = recepcion.Id,
                ChangeDate = recepcion.ChangeDate,
                ChangeUser = recepcion.ChangeUser,
                Deleted = recepcion.Estado
            };
        }

        public static void MapFromUpdateModel(this RecepcionModel recepcion, UpdateRecepcionModel update)
        {
            recepcion.IdCliente = update.IdCliente;
            recepcion.IdHabitacion = update.IdHabitacion;
            recepcion.IdEstadoReserva = update.IdEstadoReserva;
            recepcion.FechaEntrada = update.FechaEntrada;
            recepcion.FechaSalida = update.FechaSalida;
            recepcion.FechaSalidaConfirmacion = update.FechaSalidaConfirmacion;
            recepcion.PrecioInicial = update.PrecioInicial;
            recepcion.Adelanto = update.Adelanto;
            recepcion.PrecioRestante = update.PrecioRestante;
            recepcion.TotalPagado = update.TotalPagado;
            recepcion.CostoPenalidad = update.CostoPenalidad;
            recepcion.Observacion = update.Observacion;
            recepcion.Estado = update.Estado;
            recepcion.ChangeDate = update.ChangeDate;
            recepcion.ChangeUser = update.ChangeUser;
        }

        public static RecepcionModel ToRecepcionModel(SaveRecepcionModel saveRecepcion)
        {
            return new RecepcionModel
            {
                IdCliente = saveRecepcion.IdCliente,
                IdHabitacion = saveRecepcion.IdHabitacion,
                IdEstadoReserva = saveRecepcion.IdEstadoReserva,
                FechaEntrada = saveRecepcion.FechaEntrada,
                FechaSalida = saveRecepcion.FechaSalida,
                FechaSalidaConfirmacion = saveRecepcion.FechaSalidaConfirmacion,
                PrecioInicial = saveRecepcion.PrecioInicial,
                Adelanto = saveRecepcion.Adelanto,
                PrecioRestante = saveRecepcion.PrecioRestante,
                TotalPagado = saveRecepcion.TotalPagado,
                CostoPenalidad = saveRecepcion.CostoPenalidad,
                Observacion = saveRecepcion.Observacion,
                Estado = saveRecepcion.Estado,
                ChangeDate = saveRecepcion.ChangeDate,
                ChangeUser = saveRecepcion.ChangeUser
            };
        }
    }
}