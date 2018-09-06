using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;

namespace xForms.Ejecucion
{
    class listaParametros
    {
        public List<parametro> lParametros;
        ParseTreeNode nodoParametros;

        public listaParametros(ParseTreeNode parametros)
        {
            this.nodoParametros = parametros;
            this.lParametros = new List<parametro>();
            iniciarParametros();
        }

        private void iniciarParametros()
        {

            if (this.nodoParametros != null)
            {
                ParseTreeNode temp;
                parametro nuevo;
                for (int i = 0; i < nodoParametros.ChildNodes.Count; i++)
                {
                    temp = nodoParametros.ChildNodes[i];
                    nuevo = new parametro(temp.ChildNodes[0].ChildNodes[0].Token.ValueString, temp.ChildNodes[1].Token.ValueString);
                    insertarParametro(nuevo, temp);
                }
            }
           
        }

        public String obtenerCadenaTipos()
        {
            string cad = "";
            for (int i = 0; i <this.lParametros.Count; i++)
            {
                cad += this.lParametros.ElementAt(i).tipo;
            }
            return cad;
        }

        private void insertarParametro(parametro par, ParseTreeNode nodo ){

            if (!existeParametro(par.nombre))
            {
                this.lParametros.Add(par);
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "El parametro " + par.nombre + ", ya existe en el contexto actual");
            }
        }

        private bool existeParametro(string nombre)
        {
            parametro temp;
            for (int i = 0; i < this.lParametros.Count; i++)
            {
                temp = this.lParametros.ElementAt(i);
                if (temp.nombre.ToLower().Equals(nombre.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }


    }
}
