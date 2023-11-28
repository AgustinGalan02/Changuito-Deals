 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace CapaDatos
{
    public class CD_Venta
    {

        public bool Registrar(Ventas obj, DataTable DetalleVenta, out string Mensaje) // se obtiene una lista de la tabla DetalleVenta
        {
            bool respuesta = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarVenta", oconexion);
                    cmd.Parameters.AddWithValue("IdCliente", obj.id_cliente);
                    cmd.Parameters.AddWithValue("TotalProducto", obj.total_producto);
                    cmd.Parameters.AddWithValue("PrecioTotal", obj.precio_total);
                    cmd.Parameters.AddWithValue("Direccion", obj.direccion);
                    cmd.Parameters.AddWithValue("IdTransaccion", obj.id_transaccion);
                    cmd.Parameters.AddWithValue("DetalleVenta", DetalleVenta);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                Mensaje = ex.Message;
            }
            return respuesta;
        }




        public List<DetalleVentas> ListarCompras(int idcliente)
        {
            List<DetalleVentas> lista = new List<DetalleVentas>();


            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "select * from fn_ListarCompra(@idcliente)";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@idcliente", idcliente);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open(); // se abre la conexion

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {

                            lista.Add(new DetalleVentas()
                            {
                                oProducto = new Producto()
                                {
                                    nombre = dr["nombre"].ToString(),
                                    precio = Convert.ToDecimal(dr["precio"], new CultureInfo("es-US")),
                                    rutaImagen = dr["rutaImagen"].ToString(),
                                    nombreImagen = dr["nombreImagen"].ToString(),
                                },
                                cantidad = Convert.ToInt32(dr["cantidad"]),
                                precio_total = Convert.ToDecimal(dr["precio_total"], new CultureInfo("es-US")),
                                id_transaccion = dr["id_transaccion"].ToString()
                            }
                                );
                        }
                    }
                }
            }
            catch
            {
                lista = new List<DetalleVentas>();
            }
            return lista;
        }

    }
}
