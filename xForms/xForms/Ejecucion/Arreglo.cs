using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using xForms.Tabla_Simbolos;


namespace xForms.Ejecucion
{
    class Arreglo:Simbolo
    {
        public ParseTreeNode nodoDimensiones;

         public Arreglo(string nombre, string tipo, string visibilidad, ParseTreeNode nodoExprsion, ParseTreeNode nodoDimensiones)
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.visibilidad = visibilidad;
            this.nodoExpresionValor = nodoExprsion;
            this.nodoDimensiones = nodoDimensiones;
        }


         public atributo clonar()
         {
             return new atributo();
         }

    }
}
