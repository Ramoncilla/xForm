using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xForms.Ejecucion
{
    public class Opcion
    {

        public string nombre { get; set; }
        public string etiqueta{ get; set; }
        public string ruta{ get; set; }


        public Opcion(string nombre, string eti, string ru)
        {
            this.nombre = nombre;
            this.etiqueta = eti;
            this.ruta = ru;
        }


    }
}
