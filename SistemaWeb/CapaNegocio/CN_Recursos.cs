using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using System.Net.Mail;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;

namespace CapaNegocio
{
    public class CN_Recursos
    {

        public static string GenerarClave()
        {
            string clave = Guid.NewGuid().ToString("N").Substring(0, 6); // devuelve un texto autogenerado de 6 digitos. Cada texto es unico
            return clave;
        }

        //encriptacion texto en SHA256

        public static string ConvertirSha256(string texto)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }

        public static bool EnviarCorreo(string correo, string asunto, string mensaje)
        {
            bool resultado = false;

            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(correo); // a quien le mando el correo
                mail.From = new MailAddress("agustingalan4@gmail.com"); // desde donde se van a mandar los mails
                mail.Subject = asunto; // asunto del correo
                mail.Body = mensaje; // mensaje del correo
                mail.IsBodyHtml = true; // se especifica que el body es html

                var smtp = new SmtpClient()  // CONFIGURACION DEL SERVIDOR
                {
                    Credentials = new NetworkCredential("agustingalan4@gmail.com", "rquhhhcnvybxynvt"),
                    Host = "smtp.gmail.com", // servidor de google
                    Port = 587,
                    EnableSsl = true
                };

                smtp.Send(mail);
                resultado = true;
            }
            catch (Exception ex)
            {
                resultado = false;

            }
            return resultado;
        }

        public static string ConvertirBase64(string ruta, out bool conversion)
        {
            string textoBase64 = string.Empty;
            conversion = true;

            try
            {
                byte[] bytes = File.ReadAllBytes(ruta);
                textoBase64 = Convert.ToBase64String(bytes);
            }
            catch
            {
                conversion = false;
            }

            return textoBase64;
        }

    }
}
