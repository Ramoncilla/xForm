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

        public Objeto(string nombre, string tipo, string rutaAcceso, bool atri)
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.rutaAcceso = rutaAcceso;
            this.esAtributo = atri;
        }

        public override void asignarValor(Valor v, ParseTreeNode nodo)
        {
            if ((v.tipo.ToLower().Equals(this.tipo.ToLower())) || v.tipo.Equals(Constantes.NULO))
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

            Objeto nueva = new Objeto(this.nombre, this.tipo, this.rutaAcceso, this.esAtributo);
            nueva.valor = this.valor;
            nueva.usada = this.usada;
            nueva.visibilidad = this.visibilidad;
            nueva.nodoExpresionValor = this.nodoExpresionValor;

            return nueva;
        }
    }
}
