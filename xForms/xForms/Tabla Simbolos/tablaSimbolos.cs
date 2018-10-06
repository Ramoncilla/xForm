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
                Console.WriteLine("Se ha insertado la variable " + simb.nombre + ", de tipo " + simb.tipo + " y Ambiente " + simb.ambito+ "  Ruta "+ simb.rutaAcc +" jjojoj");
            }
        }

        public bool insertarSimbolo(Simbolo simb)
        {
            nodoTablaSimbolos temp = this.listaSimbolos.Peek();
            if (simb != null)
            {
                bool a = temp.insertarSimbolo(simb);
                if (!a)
                {
                    Constantes.erroresEjecucion.errorSemantico("La variable, ya existe en el ambito actual");
                }
                else
                {
                    Console.WriteLine("Se ha insertado la variable " + simb.nombre + ", de tipo " + simb.tipo + " y Ambiente " + simb.ambito + "  Ruta " + simb.rutaAcc+" uuuuuu");

                }
                return a;

            }
            return false;
           
            
        }


        public void  sacarSimbolo()
        {
            nodoTablaSimbolos temp = listaSimbolos.Peek();
            temp.variablesAmbito.Pop();
        }

        public bool insertarSimboloAmbienteGlobal(Simbolo simb)
        {
            nodoTablaSimbolos temp = this.listaSimbolos.ElementAt(listaSimbolos.Count - 1);
            return temp.insertarSimbolo(simb);

        }

        public void salirAmbiente()
        {
            this.listaSimbolos.Pop();
        }


        public VairablesObjeto obtenerObjetoConAtributos(string nombreObjeto, string ambitoObjeto, string rutaA)
        {
            nodoTablaSimbolos temp;
            VairablesObjeto simb;
            for (int i = 0; i < this.listaSimbolos.Count; i++)
            {
                temp = listaSimbolos.ElementAt(i);
                simb = temp.obtenerObjetoConAtributos(nombreObjeto, ambitoObjeto, rutaA);
                if (simb != null)
                {
                    return simb;
                }

            }
            return null;

            /*
            nodoTablaSimbolos actual = this.listaSimbolos.Peek();
            return actual.obtenerObjetoConAtributos(nombreObjeto, ambitoObjeto, rutaA);*/

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

            
        }


        public Simbolo buscarSimboloRes(string nombre)
        {
            nodoTablaSimbolos temp;
            Simbolo simb;
            for (int i = 0; i < this.listaSimbolos.Count; i++)
            {
                temp = listaSimbolos.ElementAt(i);
                simb = temp.obtenerSimboloRes(nombre);
                if (simb != null)
                {
                    return simb;
                }

            }
            return null;

        }



        public Simbolo buscarSimboloRutaAcceso(string nombre, Contexto ambiente)
        {

            nodoTablaSimbolos temp;
            Simbolo simb;
            for (int i = 0; i < this.listaSimbolos.Count; i++)
            {
                temp = listaSimbolos.ElementAt(i);
                simb = temp.obtenerSimboloRuta(nombre, ambiente);
                if (simb != null)
                {
                    return simb;
                }

            }
            return null;


        }




        public tablaSimbolos clonarTabla()
        {
            tablaSimbolos nueva = new tablaSimbolos();
            nodoTablaSimbolos temp;
            nodoTablaSimbolos nuevoN;
            for (int i = 0; i < this.listaSimbolos.Count; i++)
            {
                temp = this.listaSimbolos.ElementAt(i);
                nuevoN = temp.clonarNodo();
                nueva.listaSimbolos.Push(nuevoN);
            }
            return nueva;
        }




        public void cambiarAmbito(String nuevoAmbito)
        {
            nodoTablaSimbolos temp;
            for (int i = 0; i < listaSimbolos.Count; i++)
            {
                temp = listaSimbolos.ElementAt(i);
                temp.cambiarAmbito(nuevoAmbito);
            }
        }

        public Simbolo obtenerPregunta(string nombre, string tipo)
        {
            nodoTablaSimbolos temp;
            Simbolo simb;
            for (int i = 0; i < this.listaSimbolos.Count; i++)
            {
                temp = listaSimbolos.ElementAt(i);
                simb = temp.obtenerNodoPregunta(nombre, tipo);
                if (simb != null)
                {
                    return simb;
                }

            }
            return null;
            
        }


    }
}
