using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Venta
    {

        private CD_Venta objCapaDato = new CD_Venta();

        public bool Registrar(Ventas obj, DataTable DetalleVenta, out string Mensaje)
        {
            return objCapaDato.Registrar(obj,DetalleVenta, out Mensaje); // pasa los parametros obj, DetalleVenta y Mensaje
        }

        public List<DetalleVentas> ListarCompras(int idcliente)
        {
            return objCapaDato.ListarCompras(idcliente);
        }


    }
}
