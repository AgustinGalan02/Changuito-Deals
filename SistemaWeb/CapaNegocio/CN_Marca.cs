using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Marca
    {

        private CD_Marca objCapaDato = new CD_Marca();

        public List<Marca> Listar()
        {
            return objCapaDato.Listar();
        }

        public int Registrar(Marca obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.nombre_marca) || string.IsNullOrWhiteSpace(obj.nombre_marca)) // si el string es vacio o es un espacio vacio
            {
                Mensaje = "El nombre de la marca no puede estar vacio";
                return 0; // Retorna 0 para indicar un error
            }

            if (string.IsNullOrEmpty(Mensaje))
            {
                return objCapaDato.Registrar(obj, out Mensaje);
            }
            else
            {
                return 0;
            }
        }

        public bool Editar(Marca obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.nombre_marca) || string.IsNullOrWhiteSpace(obj.nombre_marca))
            {
                Mensaje = "El nombre de la marca no puede estar vacio";
            }

            if (string.IsNullOrEmpty(Mensaje))
            {
                return objCapaDato.Editar(obj, out Mensaje);
            }
            else
            {
                return false;
            }
        }

        public bool Eliminar(int id, out string Mensaje)
        {
            return objCapaDato.Eliminar(id, out Mensaje);
        }


        public List<Marca> ListarMarcaporCategoria(int idcategoria)
        {
            return objCapaDato.ListarMarcaporCategoria(idcategoria);
        }


    }
}
