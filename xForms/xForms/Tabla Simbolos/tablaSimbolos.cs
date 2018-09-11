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
        public Stack<nodoTablaSimbolos> listaSimbolos;


        public tablaSimbolos()
        {
            this.listaSimbolos = new Stack<nodoTablaSimbolos>();
        }


        public void mostrarSimbolos()
        {
            /*
            nodoTablaSimbolos temp;
            for (int i = 0; i < listaSimbolos.Count; i++)
            {
                temp = listaSimbolos.ElementAt(i);
                Console.WriteLine("***************** Nodo tabla dde simbolos **********************");
                temp.imprimirNodoTabla();
                Console.WriteLine("***************** Fin Nodo tabla dde simbolos **********************");

            }*/
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
            else
            {
                Console.WriteLine("Se ha insertado la variable " + simb.nombre + ", de tipo " + simb.tipo + " y ruta " + simb.rutaAcceso);
            }
        }

        public bool insertarSimbolo(Simbolo simb)
        {
            nodoTablaSimbolos temp = this.listaSimbolos.Peek();
            return temp.insertarSimbolo(simb);
            
        }

        public bool insertarSimboloAmbienteGlobal(Simbolo simb)
        {
            nodoTablaSimbolos temp = this.listaSimbolos.ElementAt(listaSimbolos.Count - 1);
            return temp.insertarSimbolo(simb);

        }

        public void salirAmbiente()
        {
           // this.listaSimbolos.Pop();
        }



        public int esAtributoSimbolo(string nombre, Contexto ambiente)
        { 

            //1 es atributo
            //2 es local
            //0 no existe el simbolo 

            int no = this.listaSimbolos.Count;
            if (no > 1)
            {
                nodoTablaSimbolos actual = this.listaSimbolos.Peek();
                Simbolo busc = actual.obtenerSimbolo(nombre, ambiente);
                if (busc != null)
                {
                    return 2;
                }
                else
                {
                    nodoTablaSimbolos global = this.listaSimbolos.ElementAt((listaSimbolos.Count - 1));
                    busc = global.obtenerSimbolo(nombre, ambiente);
                    return 1;
                }
            }
            else if (no == 1)
            {
                nodoTablaSimbolos actual = this.listaSimbolos.Peek();
                Simbolo busc = actual.obtenerSimbolo(nombre, ambiente);
                if (busc != null)
                {
                    return 1;
                }

            }
            return 0;

        }


        public Simbolo buscarSimbolo(string nombre, Contexto ambiente)
        {

            nodoTablaSimbolos temp;
            Simbolo simb;
            for (int i = 0; i < this.listaSimbolos.Count; i++)
            {
                temp = listaSimbolos.ElementAt(i);
                simb = temp.obtenerSimbolo(nombre, ambiente);
                if (simb != null)
                {
                    return simb;
                }
                
            }
            return null;

            /*
            int no = this.listaSimbolos.Count;
            if (no > 1)
            {
                nodoTablaSimbolos actual = this.listaSimbolos.Peek();
                Simbolo busc = actual.obtenerSimbolo(nombre, ambiente);
                if (busc != null)
                {
                    return busc;
                }
                else
                {
                    nodoTablaSimbolos global = this.listaSimbolos.ElementAt((listaSimbolos.Count-1));
                    busc = global.obtenerSimbolo(nombre, ambiente);
                    return busc;
                }
            }
            else if(no==1)
            {
                nodoTablaSimbolos actual = this.listaSimbolos.Peek();
                Simbolo busc = actual.obtenerSimbolo(nombre, ambiente);
                if (busc != null)
                {
                    return busc;
                }

            }
            return null;*/
        }










    }
}
