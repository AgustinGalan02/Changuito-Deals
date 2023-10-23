using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

using CapaEntidad;
using CapaNegocio;
using Newtonsoft.Json.Linq;
using System.Web.Services.Description;
using System.Runtime.InteropServices;

namespace CapaTienda.Controllers
{
    public class TiendaController : Controller
    {
        // GET: Tienda
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Carrito()
        {
            return View();
        }

        public ActionResult DetalleProducto(int idproducto = 0)
        {
            Producto oProducto = new Producto();
            bool conversion;

            oProducto = new CN_Producto().Listar().Where(p => p.id_producto == idproducto).FirstOrDefault();

            if (oProducto != null)
            {
                oProducto.Base64 = CN_Recursos.ConvertirBase64(Path.Combine(oProducto.rutaImagen, oProducto.nombreImagen), out conversion);
                oProducto.Extension = Path.GetExtension(oProducto.nombreImagen);
            }

            return View(oProducto);
        }



        [HttpGet]
        public JsonResult ListaCategorias() // se obtiene la lista de categorias
        {
            List<Categoria> lista = new List<Categoria>();

            lista = new CN_Categoria().Listar();

            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost] // filtar marcas por categoria
        public JsonResult ListarMarcaporCategoria(int idcategoria)
        {
            List<Marca> lista = new List<Marca>();

            lista = new CN_Marca().ListarMarcaporCategoria(idcategoria);

            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost] // filtrar productos por marca y/o categoria
        public JsonResult ListarProducto(int idcategoria, int idmarca)
        {
            List<Producto> lista = new List<Producto>();

            bool conversion;

            lista = new CN_Producto().Listar().Select(p => new Producto()
            {
                id_producto = p.id_producto,
                nombre = p.nombre,
                descripcion = p.descripcion,
                oMarca = p.oMarca,
                oCategoria = p.oCategoria,
                precio = p.precio,
                stock = p.stock,
                rutaImagen = p.rutaImagen,
                Base64 = CN_Recursos.ConvertirBase64(Path.Combine(p.rutaImagen, p.nombreImagen), out conversion),
                Extension = Path.GetExtension(p.nombreImagen),
                activo = p.activo
            }).Where(p =>
                p.oCategoria.id_categoria == (idcategoria == 0 ? p.oCategoria.id_categoria : idcategoria) && // si es igual a 0 busca por todas las categorias, sino por la seleccionada
                p.oMarca.id_marca == (idmarca == 0 ? p.oMarca.id_marca : idmarca) && // lo mismo que arriba
                p.stock > 0 && p.activo == true).ToList();

            var jsonresult = Json(new { data = lista }, JsonRequestBehavior.AllowGet);
            jsonresult.MaxJsonLength = int.MaxValue; // no tiene limite

            return jsonresult;
        }


        // METODOS PARA AGREGAR/QUITAR PRODUCTOS AL CARRITO

        [HttpPost]
        public JsonResult AgregarCarrito(int idproducto)
        {
            int idcliente = ((Cliente)Session["Cliente"]).id_cliente; // forma de obtener el id del cliente

            bool existe = new CN_Carrito().ExisteCarrito(idcliente, idproducto); // valida si existe el producto dentro del carrito

            bool respuesta = false;

            string mensaje = string.Empty;

            if (existe)
            {
                mensaje = "El producto ya esta agregado al carrito";
            }
            else
            {
                respuesta = new CN_Carrito().OperacionCarrito(idcliente, idproducto, true, out mensaje); // si el producto no esta agregado, lo suma. True seria el booleano Sumar
            }

            return Json(new { respuesta = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet] // metodo para devolver la cantidad de productos segun el id del cliente
        public JsonResult CantidadEnCarrito()
        {
            int idcliente = ((Cliente)Session["Cliente"]).id_cliente;
            int cantidad = new CN_Carrito().CantidadEnCarrito(idcliente);

            return Json(new { cantidad = cantidad }, JsonRequestBehavior.AllowGet);


        }


        [HttpPost] //
        public JsonResult ListarProductosCarrito()
        {
            int idcliente = ((Cliente)Session["Cliente"]).id_cliente;
            List<CarritoTemporal> oLista = new List<CarritoTemporal>();

            bool conversion;

            oLista = new CN_Carrito().ListarProducto(idcliente).Select(oc => new CarritoTemporal() // obtiene la lista de productos en el carrito del cliente
            {
                oProducto = new Producto() // crea un nuevo objeto producto
                {
                    id_producto = oc.oProducto.id_producto,
                    nombre = oc.oProducto.nombre,
                    oMarca = oc.oProducto.oMarca,
                    precio = oc.oProducto.precio,
                    rutaImagen = oc.oProducto.rutaImagen,
                    nombreImagen = oc.oProducto.nombreImagen,
                    Base64 = CN_Recursos.ConvertirBase64(Path.Combine(oc.oProducto.rutaImagen, oc.oProducto.nombreImagen), out conversion), // unir ruta y nombre imagen
                    Extension = Path.GetExtension(oc.oProducto.nombreImagen)
                },
                cantidad = oc.cantidad
            }).ToList();

            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);


        }

        [HttpPost] // metodo para agregar/restar cantidad de un producto ya agregado
        public JsonResult OperacionCarrito(int idproducto, bool sumar) 
        {
            int idcliente = ((Cliente)Session["Cliente"]).id_cliente; 


            bool respuesta = false;

            string mensaje = string.Empty;

            respuesta = new CN_Carrito().OperacionCarrito(idcliente, idproducto, sumar, out mensaje);

            return Json(new { respuesta = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost] // metodo para eliminar producto del carrito
        public JsonResult EliminarCarrito(int idproducto)
        {
            int idcliente = ((Cliente)Session["Cliente"]).id_cliente;

            bool respuesta = false;

            string mensaje = string.Empty;

            respuesta = new CN_Carrito().EliminarCarrito(idcliente, idproducto);

            return Json(new { respuesta = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);


        }
    }
}