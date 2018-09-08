using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using xForms.Tabla_Simbolos;

namespace xForms.Ejecucion
{
    class Clase
    {

        public string nombreClase;
        public string herencia;
        public string visibilidad;
        ParseTreeNode cuerpoClase;
        public ListaFunciones funcionesClase;
        public ListaAtributos atributosClase;
        public Funcion principal;
 

        public Clase(String nombreC, String herencia, string visi, ParseTreeNode nodo)
        {
            this.principal = null;
            this.nombreClase = nombreC;
            this.herencia = herencia;
            this.visibilidad = visi;
            this.cuerpoClase = nodo;
            this.funcionesClase = new ListaFunciones();
            this.atributosClase = new ListaAtributos();
            crearElementosClasee();
        }


        public Funcion obtenerPrincipal()
        {
            return this.principal;
        }


        public void crearElementosClasee()
        {
            this.recorrerArbol(this.cuerpoClase);
        }


        public Funcion obtenerFuncion(String nombreFuncion, String cadenaParametros)
        {
            return this.funcionesClase.obtenerFuncion(nombreFuncion, cadenaParametros);
        }


        public List<Simbolo> obtenerAtributosClase()
        {
            return this.atributosClase.clonarLista();
        }

        private void recorrerArbol(ParseTreeNode nodo)
        {
            switch (nodo.Term.Name)
            {
                case Constantes.CUERPO_CLASE:
                    {
                        foreach (ParseTreeNode item in nodo.ChildNodes)
                        {
                            recorrerArbol(item);
                        }
                        break;
                    }
                case Constantes.LISTA_ELEMENTOS_CLASEE:
                    {
                        foreach (ParseTreeNode item in nodo.ChildNodes)
                        {
                            recorrerArbol(item);
                        }
                        break;
                    }
                case Constantes.FUNCION:
                    {
                        Funcion nueva = new Funcion(nodo, this.nombreClase,1);
                        this.funcionesClase.insertarFuncion(nueva, nodo);
                        break;
                    }

                case Constantes.CONSTRUCTOR:
                    {
                        Funcion nueva = new Funcion(nodo, this.nombreClase,2);
                        nueva.esConstructor = true;
                        this.funcionesClase.insertarFuncion(nueva, nodo);
                        break;
                    }
                case Constantes.PRINCIPAL:
                    {
                        this.principal = new Funcion(nodo, this.nombreClase,3);
                        break;
                    }

                case Constantes.DECLA_ATRIBUTO:
                    {
                        string tipo = nodo.ChildNodes[0].ChildNodes[0].Token.ValueString;
                        int noHijos = nodo.ChildNodes.Count;
                        bool esObj = this.esObjecto(tipo);
                        string nombre;
                        if (noHijos == 2)
                        {
                            nombre = nodo.ChildNodes[1].Token.ValueString;
                            // | TIPO_DATOS + identificador + ToTerm(Constantes.PUNTO_COMA)
                            if (!esObj)
                            {
                                Variable nueva = new Variable(nombre, tipo, nombreClase);
                                nueva.visibilidad = Constantes.PUBLICO;
                                nueva.nodoExpresionValor = null;
                                this.atributosClase.insertarAtributo(nueva, nodo);
                            }
                            else
                            {
                                Objeto nuevo = new Objeto(nombre, tipo, Constantes.PUBLICO, null);
                                nuevo.rutaAcceso= nombreClase;;
                                this.atributosClase.insertarAtributo(nuevo, nodo);

                            }
                            

                        }
                        else if (noHijos == 3)
                        {
                           
                            if (nodo.ChildNodes[1].Term.Name.ToLower().Equals(Constantes.VISIBILIDAD.ToLower()))
                            {// TIPO_DATOS + VISIBILIDAD + identificador + ToTerm(Constantes.PUNTO_COMA)
                                visibilidad = nodo.ChildNodes[1].ChildNodes[0].Token.ValueString;
                                nombre = nodo.ChildNodes[2].Token.ValueString;
                                if (!esObj)
                                {
                                    Variable nueva = new Variable(nombre, tipo, nombreClase);
                                    nueva.nodoExpresionValor = null;
                                    nueva.visibilidad= visibilidad;
                                    this.atributosClase.insertarAtributo(nueva, nodo);



                                }
                                else
                                {
                                    Objeto nuevo = new Objeto(nombre, tipo, visibilidad, null);
                                    nuevo.rutaAcceso= nombreClase;;
                                    this.atributosClase.insertarAtributo(nuevo, nodo);

                                }
                            }
                            else if (nodo.ChildNodes[2].Term.Name.ToLower().Equals(Constantes.L_CORCHETES_VACIOS.ToLower()))
                            {//TIPO_DATOS + identificador + L_CORCHETES_VACIOS + Constantes.PUNTO_COMA
                                nombre = nodo.ChildNodes[1].Token.ValueString;
                                ParseTreeNode nodoExpresiones = nodo.ChildNodes[2];
                                Arreglo nuevo = new Arreglo(nombre, tipo, Constantes.PUBLICO, null, nodoExpresiones);
                                nuevo.rutaAcceso= nombreClase;;
                                this.atributosClase.insertarAtributo(nuevo, nodo);
                            }
                            else
                            {//TIPO_DATOS + identificador + ToTerm(Constantes.IGUAL) + EXPRESION + Constantes.PUNTO_COMA
                                ParseTreeNode expr = nodo.ChildNodes[2];
                                nombre = nodo.ChildNodes[1].Token.ValueString;
                                if (!esObj)
                                {
                                    Variable nuevo = new Variable(nombre, tipo, nombreClase);
                                    nuevo.nodoExpresionValor = expr;
                                    nuevo.visibilidad = Constantes.PUBLICO;
                                    this.atributosClase.insertarAtributo(nuevo,nodo);
                                }
                                else
                                {
                                    Objeto nuevo = new Objeto(nombre, tipo, Constantes.PUBLICO, expr);
                                    nuevo.rutaAcceso= nombreClase;;
                                    this.atributosClase.insertarAtributo(nuevo, nodo);

                                }
                            }

                        }
                        else if (noHijos == 4)
                        {
                           
                            if (nodo.ChildNodes[1].Term.Name.ToLower().Equals(Constantes.VISIBILIDAD.ToLower()))
                            {

                                if (nodo.ChildNodes[3].Term.Name.ToLower().Equals(Constantes.L_CORCHETES_VACIOS.ToLower()))
                                {//TIPO_DATOS + VISIBILIDAD + identificador + L_CORCHETES_VACIOS + ToTerm(Constantes.PUNTO_COMA)
                                    visibilidad = nodo.ChildNodes[1].ChildNodes[0].Token.ValueString;
                                    nombre = nodo.ChildNodes[2].Token.ValueString;
                                    ParseTreeNode nodoDim = nodo.ChildNodes[3];
                                    Arreglo nuevo = new Arreglo(nombre, tipo, visibilidad, null, nodoDim);
                                    nuevo.rutaAcceso= nombreClase;;
                                    this.atributosClase.insertarAtributo(nuevo, nodo);

                                }
                                else
                                {
                                    //TIPO_DATOS + VISIBILIDAD + identificador + ToTerm(Constantes.IGUAL) + EXPRESION + Constantes.PUNTO_COMA
                                    visibilidad = nodo.ChildNodes[1].ChildNodes[0].Token.ValueString;
                                    nombre = nodo.ChildNodes[2].Token.ValueString;
                                    ParseTreeNode nodoExp = nodo.ChildNodes[3];
                                    if (!esObj)
                                    {
                                        Variable nuevo = new Variable(nombre, tipo, nombreClase);
                                        nuevo.visibilidad = visibilidad;
                                        nuevo.nodoExpresionValor = nodoExp;
                                        this.atributosClase.insertarAtributo(nuevo, nodo);
                                    }
                                    else
                                    {
                                        Objeto nuevo = new Objeto(nombre, tipo, visibilidad, nodoExp);
                                        nuevo.rutaAcceso= nombreClase;;
                                        this.atributosClase.insertarAtributo(nuevo, nodo);
                                    }
                                }

                            }
                            else
                            {//TIPO_DATOS + identificador + L_CORCHETES_VACIOS + ToTerm(Constantes.IGUAL) + EXPRESION + Constantes.PUNTO_COMA
                                nombre = nodo.ChildNodes[1].Token.ValueString;
                                ParseTreeNode nodoDim = nodo.ChildNodes[2];
                                ParseTreeNode nodoExp = nodo.ChildNodes[3];
                                Arreglo arr = new Arreglo(nombre, tipo, Constantes.PUBLICO, nodoExp, nodoDim);
                                arr.rutaAcceso= nombreClase;;
                                this.atributosClase.insertarAtributo(arr, nodo);
                            }


                        }
                        else if (noHijos == 5)
                        {
                            /*
                             * TIPO_DATOS + VISIBILIDAD + identificador + L_CORCHETES_VACIOS + ToTerm(Constantes.IGUAL) + EXPRESION + Constantes.PUNTO_COMA
                             */
                            visibilidad = nodo.ChildNodes[1].ChildNodes[0].Token.ValueString;
                            nombre = nodo.ChildNodes[2].Token.ValueString;
                            ParseTreeNode nodoDim = nodo.ChildNodes[3];
                            ParseTreeNode nodoExp = nodo.ChildNodes[4];
                            Arreglo arr = new Arreglo(nombre, tipo, visibilidad, nodoExp, nodoDim);
                            arr.rutaAcceso= nombreClase;;
                            this.atributosClase.insertarAtributo(arr, nodo);
                        }

                        break;
                    }
            }

        }



        private bool esObjecto(String tipo)
        {
            tipo = tipo.ToLower();
            if(tipo.ToLower().Equals(Constantes.CADENA.ToLower()) ||
               tipo.ToLower().Equals(Constantes.BOOLEANO.ToLower()) ||
                tipo.ToLower().Equals(Constantes.CARACTER.ToLower()) ||
                tipo.ToLower().Equals(Constantes.ENTERO.ToLower()) ||
                tipo.ToLower().Equals(Constantes.DECIMAL.ToLower()) ||
                tipo.ToLower().Equals(Constantes.FECHA.ToLower())||
                tipo.ToLower().Equals(Constantes.FECHA_HORA.ToLower())||
                tipo.ToLower().Equals(Constantes.HORA.ToLower())){
                return false;
            }
            return true;
        }





      


    }
}
