using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xForms.Ejecucion;
using xForms.Tabla_Simbolos;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using xForms.Fechas;

namespace xForms.Analizar
{
    class Accion
    {

        public Expresion resolvExpresiones;
        public ListaClases claseArchivo;
        public List<Importar> importaciones;

        public Accion()
        {
            this.claseArchivo = new ListaClases();
            resolvExpresiones = new Expresion();
            this.importaciones = new List<Importar>();
        }
        //faltan las importaciones 


        #region generacion de un lisado de clases e importaciones
        public void generarClases(ParseTreeNode nodo)
        {
            switch (nodo.Term.Name)
            {
                case Constantes.ARCHIVO:{
                    foreach (ParseTreeNode item in nodo.ChildNodes)
                        {
                            generarClases(item);
                        }
                        break;
                }

                case Constantes.LISTA_CLASES:
                    {
                        foreach (ParseTreeNode item in nodo.ChildNodes)
                        {
                            generarClases(item);
                        }
                        break;
                    }

                case Constantes.IMPORTACIONES:
                    {
                        foreach (var item in nodo.ChildNodes)
                        {
                            generarClases(item);
                        }
                        break;
                    }

                case Constantes.IMPORTAR:
                    {
                        string nombreArchivo = nodo.ChildNodes[0].Token.ValueString;
                        Importar nuevo = new Importar(nombreArchivo);
                        this.importaciones.Add(nuevo);
                        break;
                    }
                case Constantes.CLASE:
                    {
                        int no = nodo.ChildNodes.Count;
                        String nombre = "";
                        String visibilidad="";
                        ParseTreeNode cuerpoClase;
                        string herencia = "";

                        if (no == 3)
                        {// no posee herencia
                            nombre = nodo.ChildNodes[0].Token.ValueString;
                            visibilidad = nodo.ChildNodes[1].ChildNodes[0].Token.ValueString;
                            cuerpoClase = nodo.ChildNodes[2];
                            Clase c = new Clase(nombre, "", visibilidad, cuerpoClase);
                            this.claseArchivo.insertarClase(c);
                        }
                        else
                        {
                            nombre = nodo.ChildNodes[0].Token.ValueString;
                            visibilidad = nodo.ChildNodes[1].ChildNodes[0].Token.ValueString;
                            cuerpoClase = nodo.ChildNodes[3];
                            herencia = nodo.ChildNodes[2].Token.ValueString;
                            Clase c = new Clase(nombre, herencia, visibilidad, cuerpoClase);
                            this.claseArchivo.insertarClase(c);

                        }
                        break;
                    }
            }

            

        }

        #endregion


        private tablaSimbolos agregarAtributosClase(Clase clase,  Contexto ambiente, String nombreClase, String nombreMetodo, tablaSimbolos actual)
        {
            List<Simbolo> atributosClase = clase.obtenerAtributosClase();
            Simbolo atrTemp;
            bool val;
            for (int i = 0; i < atributosClase.Count; i++)
            {
                atrTemp = atributosClase.ElementAt(i);
                if (atrTemp.nodoExpresionValor != null)
                {
                    Valor v = resolvExpresiones.resolverExpresion(atrTemp.nodoExpresionValor, ambiente, nombreClase, nombreMetodo, actual);
                    atrTemp.asignarValor(v, atrTemp.nodoExpresionValor);
                }

                val = actual.insertarSimbolo(atrTemp);

                
            }
            return actual;
        }


        public void ejecutarArchivo()
        {
            Clase temp;
            Funcion principal;
            tablaSimbolos tabla = new tablaSimbolos();
            Contexto contexto = new Contexto();

            for (int i = 0; i < this.claseArchivo.size(); i++)
            {
                temp = this.claseArchivo.get(i);
                principal = temp.obtenerPrincipal();
                if (principal != null)
                {
                    ParseTreeNode sentencia;
                    //1. Agregamos el ambito de la clase y los atributos
                    contexto.addAmbito(temp.nombreClase);
                    tabla.crearNuevoAmbito(temp.nombreClase);
                    tabla = agregarAtributosClase(temp, contexto, temp.nombreClase, "", tabla);
                    
                    //2. Agregamos el ambito del metodo principal
                    contexto.addAmbito(Constantes.PRINCIPAL);
                    tabla.crearNuevoAmbito(Constantes.PRINCIPAL);
                  
                    for (int j = 0; j < principal.cuerpoFuncion.ChildNodes[0].ChildNodes.Count; j++)
                    {
                        sentencia = principal.cuerpoFuncion.ChildNodes[0].ChildNodes[j];
                        evaluarArbol(sentencia, contexto, temp.nombreClase, Constantes.PRINCIPAL, tabla);
                    }
                    contexto.Ambitos.Pop();
                    tabla.salirAmbiente();
                    contexto.Ambitos.Pop();
                    tabla.salirAmbiente();
                    break;

                }  
            }
        }





        #region evualuar sentencias
        public void evaluarArbol(ParseTreeNode nodo, Contexto ambiente, String nombreClase, String nombreMetodo, tablaSimbolos tabla)
        {
            switch (nodo.Term.Name)
            {
                case Constantes.IMPRIMIR:
                    {
                        
                        this.imprimir(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        break;
                    }

                case Constantes.CUERPO_FUNCION:
                    {
                        foreach (ParseTreeNode item in nodo.ChildNodes)
                        {
                            evaluarArbol(item, ambiente, nombreClase, nombreMetodo, tabla);
                        }
                        break;
                    }


                case Constantes.SENTENCIAS:
                    {
                        foreach (ParseTreeNode item in nodo.ChildNodes)
                        {
                            evaluarArbol(item, ambiente, nombreClase, nombreMetodo, tabla);
                        }
                        break;
                    }

                case Constantes.SI:
                    {

                       #region resolver un if 
                        int cont = nodo.ChildNodes.Count;
                        if (cont == 2)
                        {
                            if (nodo.ChildNodes[1].Term.Name.Equals(Constantes.L_SINO_SI))
                            {//S_SI +L_SINO_SI
                                bool primero = this.resolverSI(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
                                ParseTreeNode listaSinos = nodo.ChildNodes[1];
                                ParseTreeNode siTemporal;
                                int i = 0;
                                while ((!primero) && i < listaSinos.ChildNodes.Count)
                                {
                                    siTemporal = listaSinos.ChildNodes[i];
                                    primero = this.resolverSI(siTemporal, ambiente, nombreClase, nombreMetodo, tabla);
                                    i++;
                                }
                               

                            }
                            else
                            {//S_SI +SINO
                                bool primero = this.resolverSI(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
                                
                                if (!primero)
                                {
                                    //resolver el sino
                                    this.resolverSino(nodo.ChildNodes[1].ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
                                }

                            }
                        }
                        else if (cont == 3)
                        {//S_SI +L_SINO_SI+ SINO
                            bool primero = this.resolverSI(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
                            ParseTreeNode listaSinos = nodo.ChildNodes[1];
                            ParseTreeNode siTemporal, exp, cuerpo;
                            int i = 0;
                            while ((!primero) && i < listaSinos.ChildNodes.Count)
                            {
                                siTemporal = listaSinos.ChildNodes[i];
                                primero = this.resolverSI(siTemporal, ambiente, nombreClase, nombreMetodo, tabla);
                                i++;
                            }
                            if (!primero)
                            {
                                //resolver el sino
                                this.resolverSino(nodo.ChildNodes[2].ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
                            }
                        }
                        else
                        {//S_SI;
                            this.resolverSI(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
                        }

#endregion
                       break;
                    }

                case Constantes.DECLA_VARIABLE:
                    {
                        /* DECLA_VARIABLE.Rule = TIPO_DATOS + identificador + ToTerm(Constantes.IGUAL) + EXPRESION + ToTerm(Constantes.PUNTO_COMA)
                | TIPO_DATOS + identificador + ToTerm(Constantes.PUNTO_COMA);*/
                        #region declaracion de vairables 
                        string rutaAcceso = ambiente.getAmbito();
                        string tipo = nodo.ChildNodes[0].ChildNodes[0].Token.ValueString;
                        string nombre = nodo.ChildNodes[1].Token.ValueString;
                        bool esObj = this.esObjecto(tipo);
                        int no = nodo.ChildNodes.Count;
                        Valor v;
                        if (no == 3)
                        {

                            if(!esObj){
                                v = resolvExpresiones.resolverExpresion(nodo.ChildNodes[2], ambiente, nombreClase, nombreMetodo, tabla);
                                Variable varNueva = new Variable(nombre, tipo, rutaAcceso);
                                varNueva.asignarValor(v, nodo);
                                varNueva.usada = true;
                                tabla.insertarSimbolo(varNueva, nodo);
                            }
                            else
                            {

                               

                            }

                            

                        }
                        else
                        { //no ==2

                            if (!esObj)
                            {
                                Variable varNueva = new Variable(nombre, tipo, rutaAcceso);
                                tabla.insertarSimbolo(varNueva, nodo);
                            }
                            else
                            {
                                Objeto nuevo = new Objeto(nombre, tipo, rutaAcceso);
                                tabla.insertarSimbolo(nuevo, nodo);
                            }


                        }
#endregion
                        break;
                    }

                   case Constantes.MIENTRAS:{

                       this.resolverMientras(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        break;
                    }
                   case Constantes.ROMPER:
                       {
                           elementoRetorno nuevo = new elementoRetorno();
                           nuevo.parar = true;
                           break;
                          // return nuevo;
                       }
                   case Constantes.CONTINUAR:
                       {
                           elementoRetorno nuevo = new elementoRetorno();
                           nuevo.continuar = true;
                           break;
                          // return nuevo;

                         
                       }

                   case Constantes.ASIGNACION:
                       {
                           this.resolverAsignar(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                           break;
                       }


            }

        }



        #endregion

        #region Asignaciones
        private void resolverAsignar(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            ParseTreeNode acceso = nodo.ChildNodes[0];
            ParseTreeNode expresion = nodo.ChildNodes[1];
            int cont = acceso.ChildNodes.Count;
            ParseTreeNode temp;
            string nombreElemento;
            for (int i = 0; i < cont; i++)
            {
                temp = acceso.ChildNodes[i];
                if (temp.Term.Name.ToLower().Equals(Constantes.ID.ToLower()))
                {
                    nombreElemento = temp.ChildNodes[0].Token.ValueString;
                    Simbolo simb = tabla.buscarSimbolo(nombreElemento, ambiente.getAmbito(), nombreClase);
                    Valor v = resolvExpresiones.resolverExpresion(expresion, ambiente, nombreClase, nombreMetodo, tabla);
                    if (simb != null)
                    {
                        simb.asignarValor(v, expresion);
                    }
                    else
                    {
                        Constantes.erroresEjecucion.errorSemantico(nodo, "No existe la variable " + nombreElemento + ", en el ambito actual");
                    }

                }
                else if (temp.Term.Name.ToLower().Equals(Constantes.LLAMADA.ToLower()))
                {

                }
                else if (temp.Term.Name.ToLower().Equals(Constantes.POS_ARREGLO.ToLower()))
                {

                }
            }
        }


        #endregion


        #region Ciclos

        #region Mientras
        private void resolverMientras(ParseTreeNode nodo, Contexto ambiente, string nombreClase, String nombreMetodo, tablaSimbolos tabla)
        {
            //MIENTRAS.Rule = ToTerm(Constantes.MIENTRAS) + ToTerm(Constantes.ABRE_PAR) + EXPRESION + ToTerm(Constantes.CIERRA_PAR) + CUERPO_FUNCION;
            ParseTreeNode nodoExpresion = nodo.ChildNodes[0];
            ParseTreeNode nodoCuerpo = nodo.ChildNodes[1];
            Valor resExpr = resolvExpresiones.resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla);
            if (esBooleano(resExpr))
            {
                while (esVerdadero(resExpr))
                {
                    foreach (ParseTreeNode item in nodoCuerpo.ChildNodes)
                    {
                        evaluarArbol(item, ambiente, nombreClase, nombreMetodo, tabla);
                    }

                    resExpr = resolvExpresiones.resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla);
                }
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "La expresion para el ciclo mientrass no es valida");
            }


        }


        #endregion


        #endregion



        private bool resolverSino(ParseTreeNode nodo, Contexto ambiente, String nombreClase, String nombreMetodo, tablaSimbolos tabla)
        {
            ambiente.addElse();
            foreach (ParseTreeNode item in nodo.ChildNodes)
            {
                evaluarArbol(item, ambiente, nombreClase, nombreMetodo, tabla);
            }
            ambiente.Ambitos.Pop();
            return true;
        }

        private bool resolverSI(ParseTreeNode nodo, Contexto ambiente, String nombreClase, String nombreMetodo, tablaSimbolos tabla)
        {
            #region S_SI
            ParseTreeNode expresionIf = nodo.ChildNodes[0];
            ParseTreeNode sentenciasCuerpo = nodo.ChildNodes[1];
            Valor v = resolvExpresiones.resolverExpresion(expresionIf, ambiente, nombreClase, nombreMetodo, tabla);
            if (!esNulo(v))
            {
                if (esBooleano(v))
                {
                    if (esVerdadero(v))
                    {
                        ambiente.addIf();
                        foreach (ParseTreeNode item in sentenciasCuerpo.ChildNodes)
                        {
                            evaluarArbol(item, ambiente, nombreClase, nombreMetodo, tabla);
                        }
                        ambiente.Ambitos.Pop();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico(nodo, "La expresion de la sentencia SI, no retorna un valor valido");
                }
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "La expresion de la sentencia SI, no retorna un valor valido");
            }
            #endregion
            return false;
        }

        private void imprimir(ParseTreeNode nodo, Contexto ambiente, String nombreClase, String nombreMetodo, tablaSimbolos tabla)
        {
           
            Valor v = resolvExpresiones.resolverExpresion(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
            if (v != null)
            {
                if (v.valor is Fecha)
                {
                    Fecha f = (Fecha)v.valor;
                    Console.WriteLine(f.valFechaCadena);
                }
                else if (v.valor is Hora)
                {
                    Hora f = (Hora)v.valor;
                    Console.WriteLine(f.valHoraCadena);
                }
                else if (v.valor is FechaHora)
                {
                    FechaHora f = (FechaHora)v.valor;
                    Console.WriteLine(f.cadenaRealFechaHora);
                }
                else
                {
                    Console.WriteLine(v.valor);

                }
                
            }

        }




        private bool esObjecto(String tipo)
        {
            tipo = tipo.ToLower();
            if (tipo.ToLower().Equals(Constantes.CADENA.ToLower()) ||
               tipo.ToLower().Equals(Constantes.BOOLEANO.ToLower()) ||
                tipo.ToLower().Equals(Constantes.CARACTER.ToLower()) ||
                tipo.ToLower().Equals(Constantes.ENTERO.ToLower()) ||
                tipo.ToLower().Equals(Constantes.DECIMAL.ToLower()) ||
                tipo.ToLower().Equals(Constantes.FECHA.ToLower()) ||
                tipo.ToLower().Equals(Constantes.FECHA_HORA.ToLower()) ||
                tipo.ToLower().Equals(Constantes.HORA.ToLower()))
            {
                return false;
            }
            return true;
        }

        private bool esNulo(Valor v)
        {
            return v.tipo.ToLower().Equals(Constantes.NULO.ToLower());
        }


        private bool esBooleano(Valor v)
        {
            return v.tipo.ToLower().Equals(Constantes.BOOLEANO.ToLower());
        }

        private bool esVerdadero(Valor v)
        {
            return v.valor.ToString().ToLower().Equals(Constantes.VERDADERO.ToLower());
        }

    }
}
