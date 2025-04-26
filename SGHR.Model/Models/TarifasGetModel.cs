using System.Data.SqlTypes;

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
