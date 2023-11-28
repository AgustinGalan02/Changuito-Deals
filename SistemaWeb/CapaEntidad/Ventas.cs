using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Ventas
    {
        public int id_venta { get; set; }
        public int id_cliente { get; set; }
        public decimal precio_total { get; set; }
        public string id_transaccion { get; set; }
        public string direccion { get; set; }
        public int total_producto { get; set; }
        public List<DetalleVentas> oDetalleVentas { get; set; }

    }
}
