using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xForms.Tabla_Simbolos;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;

namespace xForms.Ejecucion
{
    class ListaFunciones
    {
        public List<Funcion> lFunciones;

        public ListaFunciones()
        {
            this.lFunciones = new List<Funcion>();
        }

          public Funcion obtenerConstructor()
        {
              Funcion temp;
            for (int i = 0; i < this.lFunciones.Count; i++)
            {
                temp = this.lFunciones.ElementAt(i);
                if (temp.esConstructor)
                {
                    return temp;
                }
            }
            return null;
        }
        


        public void insertarFuncion(Funcion f, ParseTreeNode nodo)
        {
            if(!existeFuncion(f))
            {
                this.lFunciones.Add(f);
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "La funcion " + f.nombreFuncion + ", ya existe en el ambito acutal ");
            }
        }



        private bool existeFuncion(Funcion fNueva)
        {
            Funcion temp;
            for (int i = 0; i < this.lFunciones.Count; i++)
            {
                temp = this.lFunciones.ElementAt(i);
                if(temp.obtenerTamanioParametros() == fNueva.obtenerTamanioParametros() &&
                    temp.obtenerCadenaParametros().ToLower().Equals(fNueva.obtenerCadenaParametros().ToLower()))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
