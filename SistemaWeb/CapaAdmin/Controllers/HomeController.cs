using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapaEntidad;
using CapaNegocio;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

namespace CapaAdmin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Usuarios(string correo)
        {
            correo = User.Identity.Name;
            var usuario = new CN_Usuarios().Listar().FirstOrDefault(u => u.correo == correo);

            if (usuario != null && usuario.roles == true)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
            

            [HttpGet] // es una URL para mostrar datos. GET MOSTRAR| POST GUARDAR | PUT EDITAR | DELETE BORRAR
        public JsonResult ListarUsuarios()          // para devolver los datos de la lista
        {
            List<Usuario> oLista = new List<Usuario>();

            oLista = new CN_Usuarios().Listar();

            return Json(new { data = oLista } , JsonRequestBehavior.AllowGet);
        }

        [HttpPost] 
        public JsonResult GuardarUsuario(Usuario objeto)
        {
            object resultado; // para guardar el resultado
            string mensaje = string.Empty;

            if (objeto.id_usuario == 0) // en caso que sea un usuario nuevo
            {
                resultado = new CN_Usuarios().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Usuarios().Editar(objeto, out mensaje);
            }

            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarUsuario(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new CN_Usuarios().Eliminar(id, out mensaje); // se toma el ID del usuario a eliminar y el mensaje 

            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult ListaReporte(string fechainicio, string fechafin, string idtransaccion)
        {

            List<Reporte> oLista = new List<Reporte>();

            oLista = new CN_Reporte().Ventas(fechainicio, fechafin, idtransaccion);

            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult VistaDashBoard()
        {
            DashBoard objeto = new CN_Reporte().VerDashBoard();

            return Json(new {resultado = objeto}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost] // exportar a excel
        public FileResult ExportarVenta(string fechainicio, string fechafin, string idtransaccion)
        {
            List<Reporte> oLista = new List<Reporte>();
            oLista = new CN_Reporte().Ventas(fechainicio, fechafin, idtransaccion);

            DataTable dt = new DataTable();

            dt.Locale = new System.Globalization.CultureInfo("es-US"); // configuracion regional
            dt.Columns.Add("Fecha Venta", typeof(string));
            dt.Columns.Add("Cliente", typeof(string));
            dt.Columns.Add("Producto", typeof(string));
            dt.Columns.Add("Precio", typeof(decimal));
            dt.Columns.Add("Cantidad", typeof(int));
            dt.Columns.Add("Total", typeof(decimal));
            dt.Columns.Add("IdTransaccion", typeof(string));

            foreach (Reporte rp in oLista)
            {
                dt.Rows.Add(new object[]           // Agregar columnas al DataTable para almacenar diferentes atributos de ventas.
                {
                    rp.FechaVenta,
                    rp.Cliente,
                    rp.Producto,
                    rp.Precio,
                    rp.Cantidad,
                    rp.Total,
                    rp.IdTransaccion
                });
            }

            dt.TableName = "Datos";

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteVenta" + DateTime.Now.ToString() + ".xlsx"); // se especifica que se va a generar un archivo excel
                }
            }


        }


    }
}