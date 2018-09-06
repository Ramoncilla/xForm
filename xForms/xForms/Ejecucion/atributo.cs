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
    class atributo
    {
        public string tipoAtributo;
        public string nombreAtributo;
        public string visibilidadAtributo;
        public ParseTreeNode nodoExpresionAtributo;

        public atributo clonar()
        {
            return new atributo();
        }

        public Simbolo convertirSimb()
        {
            return new Simbolo();
        }


    }
}
