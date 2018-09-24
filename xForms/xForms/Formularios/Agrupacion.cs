using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;

namespace xForms.Formularios
{
    class Agrupacion
    {
        public string nombre;
        public ParseTreeNode cuerpo;


        public Agrupacion(string nom, ParseTreeNode n)
        {
            this.nombre = nom;
            this.cuerpo = n;
        }


    }
}
