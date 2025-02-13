﻿using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Configuration
{
    public sealed class Servicios : AuditEntity
    {
        public int IdServicios { get; set; }
        public required string Nombre { get; set; }
        public required string Descripcion { get; set; }
    }
}
