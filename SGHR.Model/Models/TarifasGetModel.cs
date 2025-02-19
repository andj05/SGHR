using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Model.Models
{
    public class TarifasGetModel
    {
        public SqlMoney PrecioPorNoche { get; set; }
        public int Descuento { get; set; }

        public string Descripcion { get; set; }

        public bool Estado { get; set; }

    }
}
