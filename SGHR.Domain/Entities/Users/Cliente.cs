using SGHR.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Domain.Entities.Users
{
    public sealed class Cliente : AuditEntity
    {
        public int IdCliente { get; set; }
        public string? TipoDocumento { get; set; }
        public string? Documento { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string? Nacionalidad { get; set; }
        public string? Telefono { get; set; }
    }
}
