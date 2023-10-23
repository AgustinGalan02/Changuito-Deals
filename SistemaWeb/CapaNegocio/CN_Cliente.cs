using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Cliente
    {



        private CD_Cliente objCapaDato = new CD_Cliente();

        public List<Cliente> Listar()
        {
            return objCapaDato.Listar();
        }





        private bool ExisteCodigoPostalEnBaseDeDatos(int codigoPostal)
        {
            using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
            {
                string query = "SELECT COUNT(*) FROM ciudades WHERE codigo_postal = @CodigoPostal";
                SqlCommand cmd = new SqlCommand(query, oconexion);
                cmd.Parameters.AddWithValue("@CodigoPostal", codigoPostal);

                oconexion.Open();

                int count = (int)cmd.ExecuteScalar();

                return count > 0;
            }
        }


        public int Registrar(Cliente obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.nombre) || string.IsNullOrWhiteSpace(obj.nombre))
            {
                Mensaje = "El nombre del cliente no puede estar vacío";
                return 0; // Retorna 0 para indicar un error
            }
            else if (string.IsNullOrEmpty(obj.apellido) || string.IsNullOrWhiteSpace(obj.apellido))
            {
                Mensaje = "El apellido del cliente no puede estar vacío";
                return 0; // Retorna 0 para indicar un error
            }
            else if (string.IsNullOrEmpty(obj.correo) || string.IsNullOrWhiteSpace(obj.correo))
            {
                Mensaje = "El correo del cliente no puede estar vacío";
                return 0; // Retorna 0 para indicar un error
            }
            else if (obj.oCiudades == null || !ExisteCodigoPostalEnBaseDeDatos(obj.oCiudades.codigo_postal))
            {
                Mensaje = "El código postal es incorrecto";
                return 0; // Retorna 0 para indicar un error
            }
            else if (string.IsNullOrEmpty(obj.telefono) || string.IsNullOrWhiteSpace(obj.telefono))
            {
                Mensaje = "El teléfono del cliente no puede estar vacío";
                return 0; // Retorna 0 para indicar un error
            }

            if (string.IsNullOrEmpty(Mensaje))
            {
                obj.contrasenia = CN_Recursos.ConvertirSha256(obj.contrasenia);
                return objCapaDato.Registrar(obj, out Mensaje);
            }
            else
            {
                return 0;
            }
        }

        // CAMBIAR CONTRASEÑA
        public bool CambiarClave(int idcliente, string nuevaclave, out string Mensaje)
        {
            return objCapaDato.CambiarClave(idcliente, nuevaclave, out Mensaje);
        }


        // REESTABLECER CONTRASEÑA
        public bool ReestablecerClave(int idcliente, string correo, out string Mensaje)
        {
            Mensaje = string.Empty;
            string nuevaclave = CN_Recursos.GenerarClave();
            bool resultado = objCapaDato.ReestablecerClave(idcliente, CN_Recursos.ConvertirSha256(nuevaclave), out Mensaje);

            if (resultado) // mensaje que se muestra al usuario
            {
                string asunto = "Contraseña Reestablecida";
                string mensaje_correo = "<h3>Su cuenta fue reestablecida correctamente</h3></br><p>Su contraseña para acceder ahora es: !clave!</p>";
                mensaje_correo = mensaje_correo.Replace("!clave!", nuevaclave);

                bool respuesta = CN_Recursos.EnviarCorreo(correo, asunto, mensaje_correo);

                if (respuesta)
                {
                    return true;
                }
                else
                {
                    Mensaje = "No se pudo enviar el correo";
                    return false;
                }
            }
            else
            {
                Mensaje = "No se pudo reestablecer la contraseña";
                return false;
            }
        }
    }
}
