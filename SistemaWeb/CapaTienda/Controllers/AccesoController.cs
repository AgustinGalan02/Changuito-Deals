using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CapaTienda.Controllers
{
    public class AccesoController : Controller
    {
        // GET: Acceso
        public ActionResult Index() // login
        {
            return View();
        }

        public ActionResult Registrar() //registro
        {
            return View();
        }

        public ActionResult Reestablecer() // reestablecer contraseña
        {
            return View();
        }

        public ActionResult CambiarClave() // cambiar contraseña
        {
            return View();
        }

        //////////////////////////////////////////////////////////////

        [HttpPost]
        public ActionResult Registrar(Cliente objeto)
        {

            int resultado;
            string mensaje = string.Empty;

            ViewData["Nombres"] = string.IsNullOrEmpty(objeto.nombre) ? "" : objeto.nombre; // almacenar la info del cliente temporalmente
            ViewData["Apellidos"] = string.IsNullOrEmpty(objeto.apellido) ? "" : objeto.apellido;
            ViewData["Correo"] = string.IsNullOrEmpty(objeto.correo) ? "" : objeto.correo;
            ViewData["CodigoPostal"] = objeto.oCiudades.codigo_postal;
            ViewData["Telefono"] = string.IsNullOrEmpty(objeto.telefono) ? "" : objeto.telefono;

            if (objeto.contrasenia != objeto.confirmarcontrasenia)
            {
                ViewBag.Error = "Las contraseñas no coinciden";
                return View();
            }

            resultado = new CN_Cliente().Registrar(objeto, out mensaje);

            if (resultado > 0)
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

        [HttpPost]
        public ActionResult Index(string correo, string clave)
        {
            Cliente oCliente = null;

            oCliente = new CN_Cliente().Listar().Where(item => item.correo == correo && item.contrasenia == CN_Recursos.ConvertirSha256(clave)).FirstOrDefault();

            if (oCliente == null) // no se encontro el cliente
            {
                ViewBag.Error = "Correo y/o contraseña incorrectas";
                return View();
            }
            else // se encontro el cliente
            {
                if (oCliente.reestablecer) // tiene que reestablecer la contraseña
                {
                    TempData["IdCliente"] = oCliente.id_cliente;
                    return RedirectToAction("CambiarClave", "Acceso");
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(oCliente.correo, false);

                    Session["Cliente"] = oCliente;      // se obtiene la info del cliente
                    ViewBag.Error = null;

                    return RedirectToAction("Index", "Tienda");

                }
            }
        }

        [HttpPost]
        public ActionResult Reestablecer(string correo)
        {
            Cliente cliente = new Cliente();

            cliente = new CN_Cliente().Listar().Where(item => item.correo == correo).FirstOrDefault();

            if (cliente == null)
            {
                ViewBag.Error = "No se encontro ningun cliente con este correo";
                return View();
            }

            string mensaje = string.Empty;
            bool respuesta = new CN_Cliente().ReestablecerClave(cliente.id_cliente, correo, out mensaje);

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



        [HttpPost]
        public ActionResult CambiarClave(string idcliente, string claveactual, string nuevaclave, string confirmarclave)
        {
            Cliente oCliente = new Cliente();

            oCliente = new CN_Cliente().Listar().Where(u => u.id_cliente == int.Parse(idcliente)).FirstOrDefault(); // se obtiene el id del usuario a cambiar la contraseña

            if (oCliente.contrasenia != CN_Recursos.ConvertirSha256(claveactual)) // si la contraseña es incorrecta
            {
                TempData["IdCliente"] = idcliente;
                ViewData["vclave"] = "";
                ViewBag.Error = "La contraseña actual es incorrecta";
                return View();


            }
            else if (nuevaclave != confirmarclave)
            {
                TempData["IdCliente"] = idcliente;
                ViewData["vclave"] = claveactual;
                ViewBag.Error = "Las contraseñas no coinciden";
                return View();

            }
            ViewData["vclave"] = "";

            nuevaclave = CN_Recursos.ConvertirSha256(nuevaclave);

            string mensaje = string.Empty;

            bool respuesta = new CN_Cliente().CambiarClave(int.Parse(idcliente), nuevaclave, out mensaje);

            if (respuesta) // validar respuesta
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["IdCliente"] = idcliente;
                ViewBag.Error = mensaje;

                return View();
            }
        }



        public ActionResult CerrarSesion()
        {
            Session["Cliente"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Acceso");

        }
    }
}