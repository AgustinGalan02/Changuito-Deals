using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Cliente
    {

        public List<Cliente> Listar()
        {
            List<Cliente> lista = new List<Cliente>();


            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {

                    string query = "select id_cliente, nombre, apellido, correo, contrasenia, codigo_postal, telefono, reestablecer from clientes";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {

                            lista.Add(
                                new Cliente()
                                {
                                    id_cliente = Convert.ToInt32(dr["id_cliente"]),
                                    nombre = dr["nombre"].ToString(),
                                    apellido = dr["apellido"].ToString(),
                                    correo = dr["correo"].ToString(),
                                    contrasenia = dr["contrasenia"].ToString(),
                                    oCiudades = new Ciudades
                                    {
                                        codigo_postal = Convert.ToInt32(dr["codigo_postal"])
                                    },
                                    telefono = dr["telefono"].ToString(),
                                    reestablecer = Convert.ToBoolean(dr["reestablecer"]),
                                }
                                );

                        }
                    }
                }


            }
            catch
            {
                lista = new List<Cliente>();
            }

            return lista;
        }

        public int Registrar(Cliente obj, out string Mensaje)
        {
            int idautogenerado = 0;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarCliente", oconexion);
                    cmd.Parameters.AddWithValue("Nombres", obj.nombre); 
                    cmd.Parameters.AddWithValue("Apellidos", obj.apellido); 
                    cmd.Parameters.AddWithValue("Correo", obj.correo); 
                    cmd.Parameters.AddWithValue("Clave", obj.contrasenia); 
                    cmd.Parameters.AddWithValue("CodigoPostal", obj.oCiudades.codigo_postal); 
                    cmd.Parameters.AddWithValue("Telefono", obj.telefono); 
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    cmd.ExecuteNonQuery();

                    idautogenerado = Convert.ToInt32(cmd.Parameters["Resultado"].Value); // Cambiar "Resultado" a "@Resultado"
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString(); // Cambiar "Mensaje" a "@Mensaje"
                }
            }
            catch (Exception ex)
            {
                idautogenerado = 0;
                Mensaje = ex.Message;
            }
            return idautogenerado;
        }


     

        public bool CambiarClave(int idcliente, string nuevaclave, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("update clientes set contrasenia = @nuevaclave, reestablecer = 0 where id_cliente = @id", oconexion);
                    cmd.Parameters.AddWithValue("@id", idcliente);
                    cmd.Parameters.AddWithValue("@nuevaclave", nuevaclave);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    resultado = cmd.ExecuteNonQuery() > 0 ? true : false; // si la cantidad de filas afectadas es mayor a 0 salio todo bien, sino hubo un error
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }
            return resultado;
        }

        public bool ReestablecerClave(int idcliente, string clave, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("update clientes set contrasenia = @clave, reestablecer = 1 where id_cliente = @id", oconexion);
                    cmd.Parameters.AddWithValue("@id", idcliente);
                    cmd.Parameters.AddWithValue("@clave", clave);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    resultado = cmd.ExecuteNonQuery() > 0 ? true : false; // si la cantidad de filas afectadas es mayor a 0 salio todo bien, sino hubo un error al eliminar el usuario
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
