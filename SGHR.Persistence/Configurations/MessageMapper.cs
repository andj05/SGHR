namespace SGHR.Persistence.Configurations
{
    public class MessageMapper
    {
        /// <summary>
        /// Diccionario de mensajes de error agrupados por categoría.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> ErrorMessages { get; }

        /// <summary>
        /// Diccionario de mensajes de éxito.
        /// </summary>
        public Dictionary<string, string> SuccessMessages { get; }

        public MessageMapper()
        {
            ErrorMessages = new Dictionary<string, Dictionary<string, string>>
            {
                // Mensaje de error genérico
                ["Generic"] = new Dictionary<string, string>
                {
                    ["GenericError"] = "Ha ocurrido un error inesperado."
                },
                // Errores generales de entidad
                ["EntityBase"] = new Dictionary<string, string>
                {
                    ["InvalidID"] = "ID no válido.",
                    ["NotFound"] = "Registro no encontrado.",
                    ["DuplicateEntry"] = "Registro duplicado detectado.",
                    ["ProtectedDelete"] = "El registro no puede ser eliminado porque está siendo referenciado.",
                    ["NullEntity"] = "Objeto de entidad nulo."
                },
                // Errores de operaciones CRUD
                ["Operations"] = new Dictionary<string, string>
                {
                    ["SaveFailed"] = "Error al guardar el registro.",
                    ["UpdateFailed"] = "Error al actualizar el registro.",
                    ["DeleteFailed"] = "Error al eliminar el registro.",
                    ["RestoreFailed"] = "Error al restaurar el registro.",
                    ["DbException"] = "Excepción en la base de datos."
                },
                // Validaciones específicas de reservas/recepción
                ["Reservation"] = new Dictionary<string, string>
                {
                    ["MissingEntryDate"] = "Campo FechaEntrada requerido.",
                    ["MissingClientID"] = "Campo IdCliente requerido.",
                    ["MissingRoomID"] = "Campo IdHabitacion requerido.",
                    ["MissingStatusID"] = "Campo IdEstadoReserva requerido.",
                    ["InvalidDateRange"] = "Rango de fechas inválido.",
                    ["RoomNotAvailable"] = "Habitación no disponible en el periodo seleccionado.",
                    ["PaymentInsufficient"] = "Pago insuficiente."
                },
                // Validaciones específicas de habitaciones
                ["Room"] = new Dictionary<string, string>
                {
                    ["MissingNumber"] = "Campo Numero requerido (máx. 50 caracteres).",
                    ["DetailTooLong"] = "Campo Detalle excede límite (100 caracteres).",
                    ["InvalidStatusID"] = "Campo IdEstadoHabitacion inválido.",
                    ["InvalidFloorID"] = "Campo IdPiso inválido.",
                    ["InvalidCategoryID"] = "Campo IdCategoria inválido."
                },
                // Validaciones específicas de clientes
                ["Client"] = new Dictionary<string, string>
                {
                    ["MissingName"] = "Campo NombreCompleto requerido.",
                    ["InvalidDocFormat"] = "Formato de documento inválido.",
                    ["InvalidEmail"] = "Formato de correo inválido.",
                    ["InvalidPhone"] = "Formato de teléfono inválido.",
                    ["MissingPassword"] = "Campo Clave requerido."
                },
                // Validaciones específicas de usuarios del sistema
                ["User"] = new Dictionary<string, string>
                {
                    ["InvalidRoleID"] = "Campo IdRolUsuario inválido.",
                    ["MissingEmail"] = "Campo Correo requerido.",
                    ["EmailInUse"] = "Correo ya registrado."
                },
                // Validaciones específicas de tarifas
                ["Tarifas"] = new Dictionary<string, string>
                {
                    ["InvalidDateRange"] = "Rango FechaInicio-FechaFin inválido.",
                    ["InvalidPrice"] = "Campo PrecioPorNoche debe ser > 0.",
                    ["DateOverlap"] = "Conflicto con tarifa existente.",
                    ["NullTarifa"] = "Tarifa es nula.",
                    ["InvalidDescription"] = "Descripción inválida.",
                    ["InvalidHabitacionID"] = "ID de habitación inválido.",
                    ["MissingDates"] = "Fechas faltantes.",
                    ["InvalidDiscount"] = "Descuento inválido."
                },
                // Validaciones específicas de reportes
                ["Report"] = new Dictionary<string, string>
                {
                    ["MissingTypeID"] = "Campo IdTipoReporte requerido.",
                    ["MissingDescription"] = "Campo Descripcion requerido.",
                    ["DescriptionTooLong"] = "Campo Descripcion excede límite (500 caracteres).",
                    ["InvalidStatusID"] = "Campo IdEstado inválido."
                },
                // Errores de autenticación
                ["Auth"] = new Dictionary<string, string>
                {
                    ["InvalidCredentials"] = "Correo/contraseña inválidos.",
                    ["SessionExpired"] = "Sesión expirada.",
                    ["InsufficientPermissions"] = "Permisos insuficientes."
                }
            };

            SuccessMessages = new Dictionary<string, string>
            {
                // Mensaje de éxito genérico
                ["GenericSuccess"] = "Operación completada exitosamente.",
                ["SaveSuccess"] = "Registro guardado exitosamente.",
                ["UpdateSuccess"] = "Registro actualizado exitosamente.",
                ["DeleteSuccess"] = "Registro eliminado exitosamente.",
                ["RestoreSuccess"] = "Registro restaurado exitosamente.",
                ["ReservationConfirmed"] = "Reserva confirmada (ID: {0}).",
                ["CheckInCompleted"] = "Check-in completado (ID: {0}).",
                ["CheckOutCompleted"] = "Check-out completado (ID: {0}).",
                ["ReportResolved"] = "Reporte resuelto (ID: {0}).",
                ["PasswordChanged"] = "Contraseña actualizada."
            };
        }
    }
}
