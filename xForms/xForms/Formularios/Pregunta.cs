using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xForms.Ejecucion;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;


namespace xForms.Formularios
{
    class Pregunta
    {
        public Clase preguntaClase;
        public ParseTreeNode nodoParametros;

        public Pregunta(Clase clas, ParseTreeNode nodo)
        {
            this.preguntaClase = clas;
            this.nodoParametros = nodo;
        }



    }
}
