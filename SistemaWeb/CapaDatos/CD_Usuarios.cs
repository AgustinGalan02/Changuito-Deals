using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CapaEntidad;
using System.Data.SqlClient;
using System.Data;

namespace CapaDatos
{
    public class CD_Usuarios
    {

        public List<Usuario> Listar() { // OBTENER TODOS LOS USUARIOS
            List<Usuario> lista = new List<Usuario>();


            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn)) {

                    string query = "select id_usuario, nombre, apellido, correo, contrasenia, reestablecer, activo, roles from usuarios"; // se realiza la query de SQL

                    SqlCommand cmd = new SqlCommand(query, oconexion); // se establece la relacion con la BD
                    cmd.CommandType = CommandType.Text; // se especifica que tipo de dato es

                    oconexion.Open(); // se abre la conexion

                    using (SqlDataReader dr = cmd.ExecuteReader()) {
                        while (dr.Read()) {             // cuando el datareader busque la tabla, que esta se almacene en la lista Usuario

                            lista.Add(
                                new Usuario() {
                                    id_usuario = Convert.ToInt32(dr["id_usuario"]),         // se convierte el id
                                    nombre = dr["nombre"].ToString(),                // se convierte a string 
                                    apellido = dr["apellido"].ToString(),
                                    correo = dr["correo"].ToString(),
                                    contrasenia = dr["contrasenia"].ToString(),
                                    reestablecer = Convert.ToBoolean(dr["reestablecer"]),           // se convierte a booleano
                                    activo = Convert.ToBoolean(dr["activo"]),
                                    roles = Convert.ToBoolean(dr["roles"])
                                }
                                );

                        }
                    }
                }

                
            }
            catch {
                lista = new List<Usuario>();
            }

            return lista;
        }

        public int Registrar(Usuario obj, out string Mensaje)
        {
            int idautogenerado = 0;
            Mensaje = string.Empty;
            try
            {

                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_registrarusuario", oconexion); // se llama al sp
                    cmd.Parameters.AddWithValue("Nombres", obj.nombre); // se llama a los parametros del sp
                    cmd.Parameters.AddWithValue("Apellidos", obj.apellido);
                    cmd.Parameters.AddWithValue("Correo", obj.correo);
                    cmd.Parameters.AddWithValue("Clave", obj.contrasenia);
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

        public bool Editar(Usuario obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_editarusuario", oconexion);
                    cmd.Parameters.AddWithValue("IdUsuario", obj.id_usuario);
                    cmd.Parameters.AddWithValue("Nombres", obj.nombre);
                    cmd.Parameters.AddWithValue("Apellidos", obj.apellido);
                    cmd.Parameters.AddWithValue("Correo", obj.correo);
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

        public bool Eliminar(int id, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("delete top (1) from usuarios where id_usuario = @id", oconexion);
                    cmd.Parameters.AddWithValue("@id", id);
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

        // cambiar contraseña default
        public bool CambiarClave(int idusuario, string nuevaclave, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("update usuarios set contrasenia = @nuevaclave, reestablecer = 0 where id_usuario = @id", oconexion);
                    cmd.Parameters.AddWithValue("@id", idusuario);
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

        // REESTABLECER CONTRASEÑA
        public bool ReestablecerClave(int idusuario, string clave, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("update usuarios set contrasenia = @clave, reestablecer = 1 where id_usuario = @id", oconexion);
                    cmd.Parameters.AddWithValue("@id", idusuario);
                    cmd.Parameters.AddWithValue("@clave", clave);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    resultado = cmd.ExecuteNonQuery() > 0 ? true : false;
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
