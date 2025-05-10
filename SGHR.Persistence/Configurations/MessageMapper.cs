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
                    ["NullEntity"] = "Objeto de entidad nulo.",
                    ["InvalidDeleteState"] = "El estado 'borrado' no puede asignarse a una entidad.",
                    ["CreationDateRequired"] = "La fecha de creación es obligatoria."
                },
                // Errores de operaciones CRUD
                ["Operations"] = new Dictionary<string, string>
                {
                    ["SaveFailed"] = "Error al guardar el registro.",
                    ["UpdateFailed"] = "Error al actualizar el registro.",
                    ["DeleteFailed"] = "Error al eliminar el registro.",
                    ["RestoreFailed"] = "Error al restaurar el registro.",
                    ["DbException"] = "Error interno al restaurar la entidad.",
                    ["DeleteInProgress"] = "No se puede eliminar una entidad en curso."
                },
                // Validaciones específicas de reservas/recepción
                ["Reservation"] = new Dictionary<string, string>
                {
                    ["MissingEntryDate"] = "La fecha de entrada es obligatoria.",
                    ["InvalidClientID"] = "El ID del cliente debe ser mayor que cero o nulo.",
                    ["InvalidRoomID"] = "El ID de la habitación debe ser mayor que cero o nulo.",
                    ["InvalidStatusID"] = "El ID del estado de reserva debe ser mayor que cero o nulo.",
                    ["ObservationTooLong"] = "La observación debe tener un máximo de 500 caracteres.",
                    ["AlreadyActive"] = "La entidad ya está activa.",
                    ["InvalidDateRange"] = "Rango de fechas inválido.",
                    ["RoomNotAvailable"] = "Habitación no disponible en el periodo seleccionado.",
                    ["PaymentInsufficient"] = "Pago insuficiente.",
                    ["InvalidExitDate"] = "La fecha de salida no puede ser anterior a la fecha de entrada.",
                    ["InvalidExitConfirmationDate"] = "La fecha de confirmación de salida no puede ser anterior a la fecha de entrada.",
                    ["InvalidPrice"] = "El precio no puede ser menor que 0.",
                    ["InvalidAdvance"] = "El adelanto no puede ser menor que 0.",
                    ["InvalidRemainingPrice"] = "El precio restante no puede ser menor que 0.",
                    ["InvalidTotalPaid"] = "El total pagado no puede ser menor que 0.",
                    ["InvalidPenaltyCost"] = "El costo de penalidad no puede ser menor que 0."

                },
                // Validaciones específicas de habitaciones
                ["Room"] = new Dictionary<string, string>
                {
                    ["MissingNumber"] = "El número no puede estar vacío.",
                    ["NumberTooLong"] = "El número no puede exceder 50 caracteres.",
                    ["InvalidStatusID"] = "El ID del estado debe ser mayor que cero.",
                    ["InvalidFloorID"] = "El ID del piso debe ser mayor que cero.",
                    ["InvalidCategoryID"] = "El ID de la categoría debe ser mayor que cero.",
                    ["DetailTooLong"] = "El detalle debe tener un máximo de 100 caracteres.",
                    ["DuplicateNumber"] = "Una habitación con este número ya existe."
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
                ["Rate"] = new Dictionary<string, string>
                {
                    ["InvalidDateRange"] = "Rango FechaInicio-FechaFin inválido.",
                    ["InvalidPrice"] = "Campo PrecioPorNoche debe ser > 0.",
                    ["InvalidInitialPrice"] = "El precio inicial debe ser mayor o igual a cero.",
                    ["DateOverlap"] = "Conflicto con tarifa existente."
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
