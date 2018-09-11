using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xForms.Ejecucion;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;

namespace xForms.Tabla_Simbolos
{
    class Variable:Simbolo
    {
        

        public Variable(string nom,  string tipo, string rutaAcceso, bool atri)
        {
            this.usada = false;
            this.nombre = nom;
            this.tipo = tipo;
            this.rutaAcceso = rutaAcceso;
            this.esAtributo = atri;
        }


        public override void asignarValor(Valor v, ParseTreeNode nodo)
        {
            if ((v.tipo.ToLower().Equals(this.tipo.ToLower()))|| v.tipo.Equals(Constantes.NULO))
            {
                this.valor = v;
                this.usada = true;
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "No es posible asignar la variable " + this.nombre + ", con el tipo " + v.tipo);
            }

        }

        public override Simbolo clonar()
        {
            Variable nueva = new Variable(this.nombre, this.tipo, this.rutaAcceso, this.esAtributo);
            nueva.valor = this.valor;
            nueva.usada = this.usada;
            nueva.visibilidad = this.visibilidad;
            nueva.nodoExpresionValor = this.nodoExpresionValor;

            return nueva;
        }

       
     

 


    }
}
