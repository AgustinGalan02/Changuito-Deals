using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CapaEntidad;
using CapaNegocio;

using System.Web.Security;

namespace CapaAdmin.Controllers
{
    public class AccesoController : Controller
    {
        // GET: Acceso
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CambiarClave()
        {
            return View();
        }

        public ActionResult Reestablecer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string correo, string clave)
        {
            Usuario oUsuario = new Usuario(); // establecemos relacion con la tabla de usuarios

            oUsuario = new CN_Usuarios().Listar().Where(u => u.correo == correo && u.contrasenia == CN_Recursos.ConvertirSha256(clave)).FirstOrDefault(); // busca un usuario cuyo correo y contraseña encriptada coincidan

            if (oUsuario == null)   // validar si se encontro un usuario

            {
                ViewBag.Error = "Correo o contraseña incorrectos";
                return View();
            }
            else if (oUsuario.activo == false)
            {
                ViewBag.Error = "Tu cuenta está desactivada. Contacta al administrador para más información.";
                return View();
            }
            else
            {

                if (oUsuario.reestablecer) // el usuario debe reestablecer la contraseña
                {

                    TempData["IdUsuario"] = oUsuario.id_usuario; // SIRVE PARA PASAR DATOS ENTRE ACCIONES/CONTROLADORES, TIENE VIDA UTIL LIMITADA A LA SOLICITUD ACTUAL Y A LA SIGUIENTE

                    return RedirectToAction("CambiarClave");
                }

                FormsAuthentication.SetAuthCookie(oUsuario.correo, false); // se crea autenticacion del usuario a traves de su correo. Al poner false la cookie elimina la sesion al cerrar la pagina

                ViewBag.Error = null;
                return RedirectToAction("Index", "Home");

            }
        }

        [HttpPost]
        public ActionResult CambiarClave(string idusuario, string claveactual, string nuevaclave, string confirmarclave)
        {
            Usuario oUsuario = new Usuario();

            oUsuario = new CN_Usuarios().Listar().Where(u => u.id_usuario == int.Parse(idusuario)).FirstOrDefault(); // se obtiene el id del usuario a cambiar la contraseña

            if (oUsuario.contrasenia != CN_Recursos.ConvertirSha256(claveactual)) // si la contraseña es incorrecta
            {
                TempData["IdUsuario"] = idusuario;
                ViewData["vclave"] = ""; // se utiliza para pasar datos entre un controlador y una vista
                ViewBag.Error = "La contraseña actual es incorrecta";
                return View();


            }
            else if (nuevaclave != confirmarclave)
            {
                TempData["IdUsuario"] = idusuario;
                ViewData["vclave"] = claveactual; // se almacena la contraseña en caso de que la nueva contraseña se haya puesto mal
                ViewBag.Error = "Las contraseñas no coinciden";
                return View();

            }
            ViewData["vclave"] = "";

            nuevaclave = CN_Recursos.ConvertirSha256(nuevaclave);

            string mensaje = string.Empty;

            bool respuesta = new CN_Usuarios().CambiarClave(int.Parse(idusuario), nuevaclave, out mensaje);

            if (respuesta) // validar respuesta
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["IdUsuario"] = idusuario;
                ViewBag.Error = mensaje;

                return View();
            }
        }

        [HttpPost]
        public ActionResult Reestablecer(string correo)
        {
            Usuario oUsuario = new Usuario();

            oUsuario = new CN_Usuarios().Listar().Where(item => item.correo == correo).FirstOrDefault();

            if (oUsuario == null)
            {
                ViewBag.Error = "No se encontro un usuario con este correo";
                return View();
            }

            string mensaje = string.Empty;
            bool respuesta = new CN_Usuarios().ReestablecerClave(oUsuario.id_usuario, correo, out mensaje);

            if (respuesta)
            {
                ViewBag.Error = null;
                return RedirectToAction("Index", "Acceso");
            }
            else
            {
                ViewBag.Error = mensaje;
                return View();
            }
        }

        public ActionResult CerrarSesion()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Acceso");

        }

    }
}