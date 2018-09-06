using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;
using Irony.Interpreter;
using xForms.Tabla_Simbolos;

namespace xForms.Ejecucion
{
    class Objeto:Simbolo
    {
         public Object valorObjeto;

        public Objeto(string nombre, string tipo, string visibilidad, ParseTreeNode nodoExprsion)
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.visibilidad = visibilidad;
            this.nodoExpresionValor = nodoExprsion;
        }

        public Objeto(string nombre, string tipo, string rutaAcceso)
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.rutaAcceso = rutaAcceso;
        }

        public Simbolo clonar()
        {
            return new Simbolo();
        }
    }
}
