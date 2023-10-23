using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Cliente
    {
        public int id_cliente { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string correo { get; set; }
        public string contrasenia { get; set; }
        public string confirmarcontrasenia { get; set; }
        public Ciudades oCiudades { get; set; }
        public string telefono { get; set; }
        public bool reestablecer { get; set; }

    }
}
