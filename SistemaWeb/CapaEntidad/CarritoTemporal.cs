using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class CarritoTemporal
    {
        public int id_carrito { get; set; }
        public int cantidad { get; set; }
        public Cliente oCliente { get; set; }
        public Producto oProducto { get; set; }
    }
}
