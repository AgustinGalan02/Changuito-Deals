using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Ciudades
    {
        public int codigo_postal { get; set; }
        public string nombre { get; set; }
        public Provincias oProvincias { get; set; }
    }
}
