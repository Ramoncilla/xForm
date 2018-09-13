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
    class ListaAtributos
    {
        public List<Simbolo> lAtributos;

        public ListaAtributos()
        {
            this.lAtributos = new List<Simbolo>();
        }



        private bool existe(Simbolo at)
        {
            for (int i = 0; i < this.lAtributos.Count; i++)
            {
                if (lAtributos.ElementAt(i).nombre.ToLower().Equals(at.nombre.ToLower()) &&
                    lAtributos.ElementAt(i).ambito.ToLower().Equals(at.ambito.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        public void insertarAtributo(Simbolo val, ParseTreeNode nodo)
        {
            if (!existe(val))
            {
                this.lAtributos.Add(val);
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "El atributo " + val.nombre + ", ya existe en el ambito actual ");
            }

        }
        public void insertarAtributo(Simbolo val)
        {
            if (!existe(val))
            {
                this.lAtributos.Add(val);
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico( "El atributo " + val.nombre + ", ya existe en el ambito actual ");
            }

        }

        public List<Simbolo> clonarLista()
        {
            List<Simbolo> nueva = new List<Simbolo>();
            Simbolo temp;
            Simbolo temp2;
            for (int i = 0; i < this.lAtributos.Count; i++)
            {
                temp = this.lAtributos.ElementAt(i);
                temp2 = temp.clonar();
                nueva.Add(temp2);
                
            }
            return nueva;
        }


    }
}
