using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;

namespace xForms.Tabla_Simbolos
{
    class tablaSimbolos
    {
        private Stack<nodoTablaSimbolos> listaSimbolos;


        public tablaSimbolos()
        {
            this.listaSimbolos = new Stack<nodoTablaSimbolos>();
        }


        public void crearNuevoAmbito(String nombreFuncion)
        {
            nodoTablaSimbolos nuevo = new nodoTablaSimbolos(nombreFuncion);
            this.listaSimbolos.Push(nuevo);
        }

        public void insertarSimbolo(Simbolo simb,ParseTreeNode nodo )
        {
            nodoTablaSimbolos temp = this.listaSimbolos.Peek();
            bool a = temp.insertarSimbolo(simb);
            if (!a)
            {
                Constantes.erroresEjecucion.errorSemantico(nodo,"La variable, ya existe en el ambito actual");
            }
        }

        public bool insertarSimbolo(Simbolo simb)
        {
            nodoTablaSimbolos temp = this.listaSimbolos.Peek();
            return temp.insertarSimbolo(simb);
            
        }

        public void salirAmbiente()
        {
            this.listaSimbolos.Pop();
        }


        public Simbolo buscarSimbolo(string nombre, string rutaAcceso, string nombreClase)
        {
            /*1. Se busca en un ambito local y luego en los atributos*/

            int no = this.listaSimbolos.Count;
            if (no > 1)
            {
                nodoTablaSimbolos actual = this.listaSimbolos.Peek();
                Simbolo busc = actual.obtenerSimbolo(nombre, rutaAcceso);
                if (busc != null)
                {
                    return busc;
                }
                else
                {
                    nodoTablaSimbolos global = this.listaSimbolos.ElementAt((listaSimbolos.Count-1));
                    busc = global.obtenerSimbolo(nombre, nombreClase);
                    return busc;
                }
            }
            else if(no==1)
            {
                nodoTablaSimbolos actual = this.listaSimbolos.Peek();
                Simbolo busc = actual.obtenerSimbolo(nombre, rutaAcceso);
                if (busc != null)
                {
                    return busc;
                }

            }
            return null;
        }










    }
}
