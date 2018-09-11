using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using xForms.Ejecucion;

namespace xForms.Tabla_Simbolos
{
    class Simbolo
    {
        public String nombre;
        public String tipo;
        public Valor valor=new Valor(Constantes.NULO, Constantes.NULO);
        public Boolean usada=false;
        public string rutaAcceso;
        public string visibilidad = "noTiene";
        public bool esAtributo = false;

        public ParseTreeNode nodoExpresionValor;



        public virtual Simbolo clonar()
        {
            return new Simbolo();
        }

        public virtual void asignarValor(Valor v, ParseTreeNode nodo)
        {
           

        }


    }
}
