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
    class Funcion
    {
        public string nombreFuncion;
        public string visibilidad;
        public listaParametros parametrosFuncion;
        ParseTreeNode nodoFuncion;
        public string tipo;
        public ParseTreeNode cuerpoFuncion;
        public string contexto;
        public bool esConstructor;
        public bool esPrincipal;



        public int obtenerTamanioParametros()
        {
            return this.parametrosFuncion.lParametros.Count;
        }

        public string obtenerCadenaParametros()
        {
            return this.parametrosFuncion.obtenerCadenaTipos();
        }

        /*
         * v=1 funcion
         * 2= constructor
         * 3= principal
         */

        public Funcion(ParseTreeNode nodo, string contexto, int v)
        {
            this.contexto = contexto;
            nodoFuncion = nodo;
            esConstructor = false;
            this.esPrincipal = false;
            iniciarValores(v);
        }

        private void iniciarValores(int v)
        {/*
           FUNCION.Rule= VISIBILIDAD +TIPO_DATOS+ identificador+ PARAMETROS+ CUERPO_FUNCION
	            |VISIBILIDAD +ToTerm(Constantes.VACIO) +identificador+ PARAMETROS+ CUERPO_FUNCION 
	            |TIPO_DATOS +identificador+ PARAMETROS +CUERPO_FUNCION
	            |ToTerm(Constantes.VACIO) +identificador +PARAMETROS +CUERPO_FUNCION;
          * 
                PRINCIPAL.Rule= ToTerm(Constantes.PRINCIPAL)+ ToTerm(Constantes.ABRE_PAR)+ ToTerm(Constantes.CIERRA_PAR)+ CUERPO_FUNCION;
          */

            if (v == 1)
            {
                #region cuando es una funcion
                int cont = this.nodoFuncion.ChildNodes.Count;
                if (cont == 5)
                {
                    this.visibilidad = this.nodoFuncion.ChildNodes[0].ChildNodes[0].Token.ValueString;
                    if (this.nodoFuncion.ChildNodes[1].Term.Name.Equals(Constantes.TIPO_DATOS))
                    {
                        this.tipo = nodoFuncion.ChildNodes[1].ChildNodes[0].Token.ValueString;
                    }
                    else
                    {
                        this.tipo = Constantes.VACIO;
                    }
                    this.nombreFuncion = nodoFuncion.ChildNodes[2].Token.ValueString;
                    this.parametrosFuncion = new listaParametros(nodoFuncion.ChildNodes[3]);
                    this.cuerpoFuncion = nodoFuncion.ChildNodes[4];
                }
                else
                {
                    this.visibilidad = Constantes.PUBLICO;
                    if (this.nodoFuncion.ChildNodes[0].Term.Name.Equals(Constantes.TIPO_DATOS))
                    {
                        this.tipo = nodoFuncion.ChildNodes[0].ChildNodes[0].Token.ValueString;
                    }
                    else
                    {
                        this.tipo = Constantes.VACIO;
                    }
                    this.nombreFuncion = nodoFuncion.ChildNodes[1].Token.ValueString;
                    this.parametrosFuncion = new listaParametros(nodoFuncion.ChildNodes[2]);
                    this.cuerpoFuncion = nodoFuncion.ChildNodes[3];
                }


                #endregion
            }

            if (v == 2)
            {
                #region cuando es un constructor
                /*
                 *  CONSTRUCTOR.Rule= identificador+ PARAMETROS +CUERPO_FUNCION
	                |ToTerm(Constantes.PUBLICO)+  identificador+ PARAMETROS +CUERPO_FUNCION;
                 */
               this.esConstructor = true;
               int cont = this.nodoFuncion.ChildNodes.Count;
               if (cont == 3)
               {
                   this.visibilidad = Constantes.PUBLICO;
                   this.tipo = Constantes.VACIO;
                   this.nombreFuncion = nodoFuncion.ChildNodes[0].Token.ValueString;
                   this.parametrosFuncion = new listaParametros(nodoFuncion.ChildNodes[1]);
                   this.cuerpoFuncion = nodoFuncion.ChildNodes[2];

               }
               else
               {
                   this.visibilidad = Constantes.PUBLICO;
                   this.tipo = Constantes.VACIO;
                   this.nombreFuncion = nodoFuncion.ChildNodes[1].Token.ValueString;
                   this.parametrosFuncion = new listaParametros(nodoFuncion.ChildNodes[2]);
                   this.cuerpoFuncion = nodoFuncion.ChildNodes[3];

               }

                #endregion
            }


            if (v == 3)
            {
                #region cunado esPrincipal 
                this.visibilidad = Constantes.PUBLICO;
                this.tipo = Constantes.VACIO;
                this.nombreFuncion = Constantes.PRINCIPAL;
                this.parametrosFuncion = new listaParametros(null);
                this.cuerpoFuncion = nodoFuncion.ChildNodes[0];

                #endregion
            }
           
        }

    }
}
