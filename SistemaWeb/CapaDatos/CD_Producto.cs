using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace CapaDatos
{
    public class CD_Producto
    {

        public List<Producto> Listar()
        {
            List<Producto> lista = new List<Producto>();


            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {

                    StringBuilder sb = new StringBuilder(); // para poner una query que tiene mas de una linea de codigo

                    sb.AppendLine("select p.id_producto, p.nombre, p.descripcion, m.id_marca, m.nombre_marca[DesMarca], c.id_categoria, c.nombre_categoria[DesCategoria], p.precio, p.stock, p.rutaImagen, p.nombreImagen, p.activo from productos p");
                    sb.AppendLine("inner join marca m on m.id_marca = p.id_marca");
                    sb.AppendLine("inner join categorias c on c.id_categoria = p.id_categoria\r\n");

                    SqlCommand cmd = new SqlCommand(sb.ToString(), oconexion);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open(); // se abre la conexion

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {          

                            lista.Add(new Producto()
                            {
                                id_producto = Convert.ToInt32(dr["id_producto"]),         // se convierte el id
                                nombre = dr["nombre"].ToString(),                // se convierte a string 
                                descripcion = dr["descripcion"].ToString(),
                                oMarca = new Marca() { id_marca = Convert.ToInt32(dr["id_marca"]), nombre_marca = dr["DesMarca"].ToString() },
                                oCategoria = new Categoria() { id_categoria = Convert.ToInt32(dr["id_categoria"]), nombre_categoria = dr["DesCategoria"].ToString() },
                                precio = Convert.ToDecimal(dr["precio"], new CultureInfo("es-AR")),
                                stock = Convert.ToInt32(dr["stock"]),
                                rutaImagen = dr["rutaImagen"].ToString(),
                                nombreImagen = dr["nombreImagen"].ToString(),
                                activo = Convert.ToBoolean(dr["activo"]),

                            }
                                );
                        }
                    }
                }
            }
            catch
            {
                lista = new List<Producto>();
            }
            return lista;
        }

        public int Registrar(Producto obj, out string Mensaje)
        {
            int idautogenerado = 0;
            Mensaje = string.Empty;
            try
            {

                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarProducto", oconexion);
                    cmd.Parameters.AddWithValue("Nombre", obj.nombre);
                    cmd.Parameters.AddWithValue("Descripcion", obj.descripcion);
                    cmd.Parameters.AddWithValue("IdMarca", obj.oMarca.id_marca);
                    cmd.Parameters.AddWithValue("IdCategoria", obj.oCategoria.id_categoria);
                    cmd.Parameters.AddWithValue("Precio", obj.precio);
                    cmd.Parameters.AddWithValue("Stock", obj.stock);
                    cmd.Parameters.AddWithValue("Activo", obj.activo);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    idautogenerado = Convert.ToInt32(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                idautogenerado = 0;
                Mensaje = ex.Message;
            }
            return idautogenerado;
        }

        public bool Editar(Producto obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_EditarProducto", oconexion);
                    cmd.Parameters.AddWithValue("IdProducto", obj.id_producto);
                    cmd.Parameters.AddWithValue("Nombre", obj.nombre);
                    cmd.Parameters.AddWithValue("Descripcion", obj.descripcion);
                    cmd.Parameters.AddWithValue("IdMarca", obj.oMarca.id_marca);
                    cmd.Parameters.AddWithValue("IdCategoria", obj.oCategoria.id_categoria);
                    cmd.Parameters.AddWithValue("Precio", obj.precio);
                    cmd.Parameters.AddWithValue("Stock", obj.stock);
                    cmd.Parameters.AddWithValue("Activo", obj.activo);
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

        public bool GuardarDatosImagen(Producto obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {

                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {

                    string query = "update productos set rutaImagen = @rutaimagen, nombreImagen = @nombreimagen where id_producto = @idproducto";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@rutaimagen", obj.rutaImagen);
                    cmd.Parameters.AddWithValue("@nombreimagen", obj.nombreImagen);
                    cmd.Parameters.AddWithValue("@idproducto", obj.id_producto);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        resultado = true;

                    }
                    else
                    {
                        Mensaje = "No se pudo actualizar la imagen";
                    }

                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }
            return resultado;

        }

        public bool Eliminar(int id, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_EliminarProducto", oconexion);
                    cmd.Parameters.AddWithValue("IdProducto", id);
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

    }
}
