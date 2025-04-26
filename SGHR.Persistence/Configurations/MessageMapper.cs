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
                    ["InvalidDiscount"] = "Descuento inválido.",
                    ["InvalidStartDate"] = "La fecha de inicio debe ser anterior a la fecha de fin.",
                    ["PastStartDate"] = "No se pueden crear tarifas con fechas pasadas",
                    ["ShortDuration"] = "La duración de la tarifa debe ser de al menos 7 días",
                    ["LongDuration"] = "La duración de la tarifa no puede exceder 1 año",
                    ["HighDiscount"] = "No se puede aplicar un descuento mayor al 20% para estancias menores a 30 días",
                    ["HighSeasonDiscount"] = "En temporada alta el descuento máximo permitido es del 15%",
                    ["OverlappingTarifa"] = "La tarifa se solapa con otra tarifa existente para la misma habitación",
                    ["ExcessivePriceChange"] = "El cambio de precio no puede exceder el 30% del precio actual"
                },

                // Validaciones específicas de servicios
                ["Servicios"] = new Dictionary<string, string>
                {
                    ["EmptyService"] = "El servicio no puede estar vacío.",
                    ["EmptyName"] = "El nombre del servicio no puede estar vacío.",
                    ["InvalidDescriptionLength"] = "La descripción del servicio no puede estar vacía y debe tener menos de 255 caracteres.",
                    ["ShortName"] = "El nombre del servicio debe tener al menos 10 caracteres.",
                    ["InvalidNameCharacters"] = "El nombre del servicio contiene caracteres no permitidos.",
                    ["ShortDescription"] = "La descripción del servicio debe tener al menos 25 caracteres.",
                    ["DuplicateName"] = "Ya existe un servicio con este nombre.",
                    ["DuplicateNameUpdate"] = "Ya existe otro servicio con este nombre."
                },

                // Validaciones específicas de roles de usuario
                ["RolUsuario"] = new Dictionary<string, string>
                {
                    ["NullRole"] = "El rol de usuario no puede ser nulo.",
                    ["EmptyDescription"] = "La descripción del rol no puede estar vacía.",
                    ["DescriptionTooLong"] = "La descripción del rol no puede exceder los 50 caracteres.",
                    ["DuplicateDescription"] = "Ya existe un rol con la misma descripción.",
                    ["DuplicateDescriptionUpdate"] = "Ya existe otro rol con la misma descripción.",
                    ["DuplicateDescriptionRestore"] = "Ya existe un rol activo con la misma descripción."
                },

                // Validaciones específicas de pisos
                ["Pisos"] = new Dictionary<string, string>
                {
                    ["NullPiso"] = "El piso no puede ser nulo.",
                    ["EmptyDescription"] = "La descripción del piso no puede estar vacía.",
                    ["DescriptionTooLong"] = "La descripción del piso no puede exceder los 50 caracteres.",
                    ["DuplicateDescription"] = "Ya existe un piso con la misma descripción.",
                    ["DuplicateDescriptionUpdate"] = "Ya existe otro piso con la misma descripción.",
                    ["DuplicateDescriptionRestore"] = "Ya existe un piso activo con la misma descripción.",
                    ["MaxPisosExceeded"] = "Se ha excedido el número máximo de pisos permitidos.",
                    ["SystemPisoModification"] = "No se permite modificar pisos del sistema.",
                    ["SystemPisoDeletion"] = "No se permite eliminar pisos del sistema.",
                    ["PisoInUse"] = "No se puede eliminar el piso porque está en uso."
                },

                // Validaciones específicas de estado de habitación
                ["EstadoHabitacion"] = new Dictionary<string, string>
                {
                    ["NullEstado"] = "El estado de habitación no puede ser nulo.",
                    ["EmptyDescription"] = "La descripción del estado no puede estar vacía.",
                    ["DescriptionTooLong"] = "La descripción del estado no debe exceder 50 caracteres.",
                    ["InvalidDescriptionCharacters"] = "La descripción contiene caracteres no permitidos.",
                    ["InvalidColorHex"] = "El formato de color hexadecimal no es válido.",
                    ["InvalidOrder"] = "El orden debe ser un número positivo.",
                    ["DuplicateDescription"] = "Ya existe un estado de habitación con esta descripción."
                },

                // Validaciones específicas de categorías
                ["Categorias"] = new Dictionary<string, string>
                {
                    ["NullCategoria"] = "Categoría inválida.",
                    ["EmptyDescription"] = "La descripción de la categoría es obligatoria.",
                    ["DescriptionTooLong"] = "La descripción no puede exceder los 50 caracteres.",
                    ["InvalidDescriptionCharacters"] = "La descripción contiene caracteres no permitidos.",
                    ["InvalidUser"] = "El usuario responsable es obligatorio."
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
