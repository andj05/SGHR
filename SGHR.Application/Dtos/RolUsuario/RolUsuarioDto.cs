using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.Dtos.RolUsuario
{
    public class RolUsuarioDto : DtoBase
    {
        public string? Descripcion { get; set; }
        public bool? Estado { get; set; }
        public DateTime FechaCreacion { get; set; }

    }
}
