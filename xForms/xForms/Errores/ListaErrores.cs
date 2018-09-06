using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;

namespace xForms.Errores
{
    class ListaErrores
    {
        public List<Error> lErrores;


        public ListaErrores()
        {
            this.lErrores= new List<Error>();

        }


        public void errorSemantico(ParseTreeNode nodo, String descripcion)
        {
            int fila = nodo.FindToken().Location.Line;
            int col = nodo.FindToken().Location.Column;
            Error nuevo = new Error(col, fila, Constantes.ERROR_SEMANTICO, descripcion);
            this.lErrores.Add(nuevo);
        }

        public void errorSemantico(String descripcion)
        {
            int fila = 0;
            int col = 0;
            Error nuevo = new Error(col, fila, Constantes.ERROR_SEMANTICO, descripcion);
            this.lErrores.Add(nuevo);
        }
        

    }
}
