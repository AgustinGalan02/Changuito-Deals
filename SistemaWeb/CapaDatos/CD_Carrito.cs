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
    public class CD_Carrito
    {

        public bool ExisteCarrito(int idcliente, int idproducto)
        {
            bool resultado = true;

            try
            {

                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_ExisteCarrito", oconexion); 
                    cmd.Parameters.AddWithValue("IdCliente", idcliente); 
                    cmd.Parameters.AddWithValue("IdProducto", idproducto);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                }
            }
            catch (Exception ex)
            {
                resultado = false;
            }
            return resultado;
        }



        public bool OperacionCarrito(int idcliente, int idproducto, bool sumar, out string Mensaje)
        {
            bool resultado = true;

            Mensaje = string.Empty;

            try
            {

                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_OperacionCarrito", oconexion); 
                    cmd.Parameters.AddWithValue("IdCliente", idcliente); 
                    cmd.Parameters.AddWithValue("IdProducto", idproducto);
                    cmd.Parameters.AddWithValue("Sumar", sumar);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }
            return resultado;
        }



        public int CantidadEnCarrito(int idcliente)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("select count(*) from carrito_temporal where id_cliente = @idcliente", oconexion);
                    cmd.Parameters.AddWithValue("@idcliente", idcliente);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    resultado = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                resultado = 0;
            }
            return resultado;
        }

        // LISTAR
        public List<CarritoTemporal> ListarProducto(int idcliente)
        {
            List<CarritoTemporal> lista = new List<CarritoTemporal>();


            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "select * from fn_obtenerCarritoCliente(@idcliente)";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@idcliente", idcliente);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open(); // se abre la conexion

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {

                            lista.Add(new CarritoTemporal()
                            {
                                oProducto = new Producto()
                                {
                                    id_producto = Convert.ToInt32(dr["id_producto"]),
                                    nombre = dr["nombre"].ToString(),
                                    precio = Convert.ToDecimal(dr["precio"], new CultureInfo("es-US")),
                                    rutaImagen = dr["rutaImagen"].ToString(),
                                    nombreImagen = dr["nombreImagen"].ToString(),
                                    oMarca = new Marca() { nombre_marca = dr["DesMarca"].ToString() }
                                },
                                cantidad = Convert.ToInt32(dr["cantidad"])
                            }
                                );
                        }
                    }
                }
            }
            catch
            {
                lista = new List<CarritoTemporal>();
            }
            return lista;
        }


        // DELETE

        public bool EliminarCarrito(int idcliente, int idproducto)
        {
            bool resultado = true;

            try
            {

                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_EliminarCarrito", oconexion);
                    cmd.Parameters.AddWithValue("IdCliente", idcliente);
                    cmd.Parameters.AddWithValue("IdProducto", idproducto);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                }
            }
            catch (Exception ex)
            {
                resultado = false;
            }
            return resultado;
        }


    }
}
