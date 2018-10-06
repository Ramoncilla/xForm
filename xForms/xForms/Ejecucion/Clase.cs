using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using xForms.Tabla_Simbolos;
using xForms.Formularios;

namespace xForms.Ejecucion
{
    class Clase
    {
        public List<Clase> preguntas;
        public string nombreClase;
        public string herencia;
        public string visibilidad;
        ParseTreeNode cuerpoClase;
        public ListaFunciones funcionesClase;
        public ListaAtributos atributosClase;
        public Funcion principal;
        public bool perteneceArchivoPrincipal;
        public ElementosFormulario objetosForm = new ElementosFormulario();
        public ParseTreeNode nodoParametrosPregunta = null;
        public bool esPregunta;
 

        public Clase(String nombreC, String herencia, string visi, ParseTreeNode nodo)
        {
            this.preguntas = new List<Clase>();
            this.principal = null;
            this.nombreClase = nombreC;
            this.herencia = herencia;
            this.visibilidad = visi;
            this.cuerpoClase = nodo;
            this.funcionesClase = new ListaFunciones();
            this.atributosClase = new ListaAtributos();
            crearElementosClasee();
            perteneceArchivoPrincipal = false;
            this.esPregunta = false;
        }


        public bool tienePrincipal()
        {
            return (this.principal != null);
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
        public Funcion obtenerFuncionNo(String nombreFuncion, int noParametros)
        {
            return this.funcionesClase.obtenerFuncionNo(nombreFuncion, noParametros);
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
                        int n = nodo.ChildNodes.Count;
                     
                        this.funcionesClase.insertarFuncion(nueva, nodo);
                        break;
                    }

                case Constantes.CONSTRUCTOR:
                    {
                        Funcion nueva = new Funcion(nodo, this.nombreClase,2);
                        int n = nodo.ChildNodes.Count;
                        if (n == 4)
                        {
                            string v1 = nodo.ChildNodes[0].Token.ValueString;
                            if (v1.Equals(Constantes.FORMULARIO, StringComparison.CurrentCultureIgnoreCase))
                            {
                                nueva.esFormulario = true;
                            }
                            else
                            {
                                nueva.esConstructor = true;
                            }
                        }
                        else
                        {
                            nueva.esConstructor = true;
                        }
                        
                        this.funcionesClase.insertarFuncion(nueva, nodo);
                        break;
                    }
                case Constantes.PRINCIPAL:
                    {
                        this.principal = new Funcion(nodo, this.nombreClase,3);
                        break;
                    }

                case Constantes.PREGUNTA:
                    {
                        //PREGUNTA.Rule = ToTerm(Constantes.PREGUNTA) + identificador + PARAMETROS + CUERPO_CLASE;
                        string nombre = nodo.ChildNodes[0].Token.ValueString;
                        ParseTreeNode parametros = nodo.ChildNodes[1];
                        ParseTreeNode cuerpo = nodo.ChildNodes[2];
                        Clase c = new Clase(nombre, "", visibilidad, cuerpo);
                        c.esPregunta = true;
                        c.nodoParametrosPregunta = parametros;
                        preguntas.Add(c);
                        break;
                    }

                case Constantes.DECLA_ATRIBUTO:
                    {
                        string tipo = nodo.ChildNodes[0].ChildNodes[0].Token.ValueString;
                        int noHijos = nodo.ChildNodes.Count;
                        bool esObj = this.esObjecto(tipo);
                        bool esLista = this.esLista(tipo);
                        string nombre;
                        if (noHijos == 2)
                        {
                            nombre = nodo.ChildNodes[1].Token.ValueString;
                            // | TIPO_DATOS + identificador + ToTerm(Constantes.PUNTO_COMA)
                            if (!esObj)
                            {
                                Variable nueva = new Variable(nombre, tipo, nombreClase, true);
                                nueva.visibilidad = Constantes.PUBLICO;
                                nueva.nodoExpresionValor = null;
                                this.atributosClase.insertarAtributo(nueva, nodo);
                            }
                            else
                            {
                                if (esLista)
                                {
                                    ListaOpciones nuevo = new ListaOpciones(nombre, tipo, Constantes.PUBLICO, null, nombreClase);
                                    this.atributosClase.insertarAtributo(nuevo, nodo);

                                }
                                else
                                {
                                    Objeto nuevo = new Objeto(nombre, tipo, Constantes.PUBLICO, null);
                                    nuevo.ambito = nombreClase; 
                                    this.atributosClase.insertarAtributo(nuevo, nodo);
                                }
                                

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
                                    Variable nueva = new Variable(nombre, tipo, nombreClase, true);
                                    nueva.nodoExpresionValor = null;
                                    nueva.visibilidad= visibilidad;
                                    this.atributosClase.insertarAtributo(nueva, nodo);



                                }
                                else
                                {

                                    if (esLista)
                                    {
                                        ListaOpciones nuevo = new ListaOpciones(nombre, tipo, visibilidad, null, nombreClase);
                                        this.atributosClase.insertarAtributo(nuevo,nodo);
                                    }
                                    else
                                    {
                                        Objeto nuevo = new Objeto(nombre, tipo, visibilidad, null);
                                        nuevo.ambito = nombreClase; ;
                                        this.atributosClase.insertarAtributo(nuevo, nodo);
                                    }
                                    

                                }
                            }
                            else if (nodo.ChildNodes[2].Term.Name.ToLower().Equals(Constantes.L_CORCHETES_VACIOS.ToLower()))
                            {//TIPO_DATOS + identificador + L_CORCHETES_VACIOS + Constantes.PUNTO_COMA
                                nombre = nodo.ChildNodes[1].Token.ValueString;
                                ParseTreeNode nodoExpresiones = nodo.ChildNodes[2];
                                Arreglo nuevo = new Arreglo(nombre, tipo, Constantes.PUBLICO, null, nodoExpresiones);
                                nuevo.ambito= nombreClase;;
                                this.atributosClase.insertarAtributo(nuevo, nodo);
                            }
                            else
                            {//TIPO_DATOS + identificador + ToTerm(Constantes.IGUAL) + EXPRESION + Constantes.PUNTO_COMA
                                ParseTreeNode expr = nodo.ChildNodes[2];
                                nombre = nodo.ChildNodes[1].Token.ValueString;
                                if (!esObj)
                                {
                                    Variable nuevo = new Variable(nombre, tipo, nombreClase, true);
                                    nuevo.nodoExpresionValor = expr;
                                    nuevo.visibilidad = Constantes.PUBLICO;
                                    this.atributosClase.insertarAtributo(nuevo,nodo);
                                }
                                else
                                {
                                    if (esLista)
                                    {
                                        ListaOpciones nuevo = new ListaOpciones(nombre, tipo, Constantes.PUBLICO, expr, nombreClase);
                                        this.atributosClase.insertarAtributo(nuevo, nodo);

                                    }
                                    else
                                    {
                                        Objeto nuevo = new Objeto(nombre, tipo, Constantes.PUBLICO, expr);
                                        nuevo.ambito = nombreClase; ;
                                        this.atributosClase.insertarAtributo(nuevo, nodo);

                                    }
                                   

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
                                    nuevo.ambito= nombreClase;;
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
                                        Variable nuevo = new Variable(nombre, tipo, nombreClase,true);
                                        nuevo.visibilidad = visibilidad;
                                        nuevo.nodoExpresionValor = nodoExp;
                                        this.atributosClase.insertarAtributo(nuevo, nodo);
                                    }
                                    else
                                    {
                                        if (esLista)
                                        {
                                            ListaOpciones nuevo = new ListaOpciones(nombre, tipo, visibilidad, nodoExp, nombreClase);
                                            this.atributosClase.insertarAtributo(nuevo, nodo);

                                        }
                                        else
                                        {
                                            Objeto nuevo = new Objeto(nombre, tipo, visibilidad, nodoExp);
                                            nuevo.ambito = nombreClase; ;
                                            this.atributosClase.insertarAtributo(nuevo, nodo);

                                        }
                                        
                                    }
                                }

                            }
                            else
                            {//TIPO_DATOS + identificador + L_CORCHETES_VACIOS + ToTerm(Constantes.IGUAL) + EXPRESION + Constantes.PUNTO_COMA
                                nombre = nodo.ChildNodes[1].Token.ValueString;
                                ParseTreeNode nodoDim = nodo.ChildNodes[2];
                                ParseTreeNode nodoExp = nodo.ChildNodes[3];
                                Arreglo arr = new Arreglo(nombre, tipo, Constantes.PUBLICO, nodoExp, nodoDim);
                                arr.ambito= nombreClase;;
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
                            arr.ambito= nombreClase;;
                            this.atributosClase.insertarAtributo(arr, nodo);
                        }

                        break;
                    }
            }

        }



        private bool esLista(String tipo)
        {
            if (tipo.ToLower().Equals(Constantes.OPCIONES.ToLower()))
            {
                return true;
            }
            return false;
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
                tipo.ToLower().Equals(Constantes.HORA.ToLower())||
                tipo.ToLower().Equals(Constantes.OPCIONES)){
                return false;
            }
            return true;
        }





      


    }
}
