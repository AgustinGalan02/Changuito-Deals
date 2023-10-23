using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class DetalleVentas
    {
        public int id_detalle_venta { get; set; }
        public int cantidad { get; set; }
        public decimal precio_producto { get; set; }
        public decimal precio_total { get; set; }
        public int id_venta { get; set; }
        public Producto oProducto { get; set; }
        public string id_transaccion { get; set; }
    }
}
