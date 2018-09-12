﻿using System;
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

       
        public ListaClases claseArchivo;
        public List<Importar> importaciones;
        string variableInstancia = "";
        private bool esAtriAsigna = false;
        private bool esAtriRes = false;

        public Accion()
        {
            this.claseArchivo = new ListaClases();
            this.importaciones = new List<Importar>();
        }
        //faltan las importaciones 

        public void instanciarAtributosClase(){
            this.claseArchivo.instanciarAtributosClase();
        }


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
                    elementoRetorno v;
                    v = resolverExpresion(atrTemp.nodoExpresionValor, ambiente, nombreClase, nombreMetodo, actual);
                    atrTemp.asignarValor(v.val, atrTemp.nodoExpresionValor);
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
                    //contexto.addAmbito(temp.nombreClase);
                    //tabla.crearNuevoAmbito(temp.nombreClase);
                    //tabla = agregarAtributosClase(temp, contexto, temp.nombreClase, "", tabla);
                    
                    //2. Agregamos el ambito del metodo principal
                    contexto.addAmbito(Constantes.PRINCIPAL);
                    tabla.crearNuevoAmbito(Constantes.PRINCIPAL);
                  
                    for (int j = 0; j < principal.cuerpoFuncion.ChildNodes[0].ChildNodes.Count; j++)
                    {
                        sentencia = principal.cuerpoFuncion.ChildNodes[0].ChildNodes[j];
                        evaluarArbol(sentencia, contexto, temp.nombreClase, Constantes.PRINCIPAL, tabla, new elementoRetorno());
                    }
                    contexto.Ambitos.Pop();
                    tabla.salirAmbiente();
                    //contexto.Ambitos.Pop();
                    //tabla.salirAmbiente();
                    break;

                }  
            }
        }



        #region evualuar sentencias
        public elementoRetorno evaluarArbol(ParseTreeNode nodo, Contexto ambiente, String nombreClase, String nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            switch (nodo.Term.Name)
            {
                #region imprimir
                case Constantes.IMPRIMIR:
                    {
                        ret = this.imprimir(nodo, ambiente, nombreClase, nombreMetodo, tabla, ret);
                        return ret;
                    }
#endregion
                #region Evaluar Cuerpo
                case Constantes.CUERPO_FUNCION:
                    {
                        foreach (ParseTreeNode item in nodo.ChildNodes)
                        {
                            ret = evaluarArbol(item, ambiente, nombreClase, nombreMetodo, tabla, ret);
                        }
                        return ret;
                    }

                case Constantes.SENTENCIAS:
                    {
                        foreach (ParseTreeNode item in nodo.ChildNodes)
                        {
                            if (ret.banderaRetorno)
                            {
                                ret.banderaRetorno = false;
                                break;

                            }
                            else
                            {
                                ret = evaluarArbol(item, ambiente, nombreClase, nombreMetodo, tabla, ret);

                            }
                        }
                        return ret;
                    }

                #endregion
                #region Si
                case Constantes.SI:
                    {
                        #region resolver un if
                        int cont = nodo.ChildNodes.Count;
                        if (cont == 2)
                        {
                            if (nodo.ChildNodes[1].Term.Name.Equals(Constantes.L_SINO_SI))
                            {//S_SI +L_SINO_SI
                                ret = this.resolverSI(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla, ret);
                                ParseTreeNode listaSinos = nodo.ChildNodes[1];
                                ParseTreeNode siTemporal;
                                int i = 0;
                                while ((!ret.banderaSi) && i < listaSinos.ChildNodes.Count)
                                {
                                    siTemporal = listaSinos.ChildNodes[i];
                                    ret = this.resolverSI(siTemporal, ambiente, nombreClase, nombreMetodo, tabla, ret);
                                    i++;
                                }


                            }
                            else
                            {//S_SI +SINO
                                ret = this.resolverSI(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla, ret);

                                if (!ret.banderaSi)
                                {
                                    //resolver el sino
                                    this.resolverSino(nodo.ChildNodes[1].ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla, ret);
                                }

                            }
                        }
                        else if (cont == 3)
                        {//S_SI +L_SINO_SI+ SINO
                            ret = this.resolverSI(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla, ret);
                            ParseTreeNode listaSinos = nodo.ChildNodes[1];
                            ParseTreeNode siTemporal, exp, cuerpo;
                            int i = 0;
                            while ((!ret.banderaSi) && i < listaSinos.ChildNodes.Count)
                            {
                                siTemporal = listaSinos.ChildNodes[i];
                                ret = this.resolverSI(siTemporal, ambiente, nombreClase, nombreMetodo, tabla, ret);
                                i++;
                            }
                            if (!ret.banderaSi)
                            {
                                //resolver el sino
                                this.resolverSino(nodo.ChildNodes[2].ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla, ret);
                            }
                        }
                        else
                        {//S_SI;
                            this.resolverSI(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla, ret);
                        }

                        #endregion
                        break;
                    }
                #endregion
                #region Declaracion
                case Constantes.DECLA_VARIABLE:
                    {
                        ret = this.resolverDeclaracion(nodo, ambiente, nombreClase, nombreMetodo, tabla, ret);
                        return ret;
                    }
                #endregion
                #region Ciclos
                case Constantes.MIENTRAS:
                    {
                        ret = this.resolverMientras(nodo, ambiente, nombreClase, nombreMetodo, tabla, ret);
                        return ret;
                    }
                case Constantes.HACER:
                    {
                        ret = this.resolverHacerMientras(nodo, ambiente, nombreClase, nombreMetodo, tabla, ret);
                        return ret;
                    }

                case Constantes.REPETIR_HASTA:
                    {
                        ret = this.resolverRepetirHasta(nodo, ambiente, nombreClase, nombreMetodo, tabla, ret);
                        return ret;
                    }

                case Constantes.PARA:
                    {
                        ret = this.resolverPara(nodo, ambiente, nombreClase, nombreMetodo, tabla, ret);
                        return ret;
                    }
                case Constantes.ROMPER:
                    {
                        ret.parar = true;
                        return ret;
                    }
                case Constantes.CONTINUAR:
                    {
                        ret.continuar = true;
                        return ret;
                    }
#endregion
                #region asignacion
                case Constantes.ASIGNACION:
                    {
                        ret = this.resolverAsignar(nodo, ambiente, nombreClase, nombreMetodo, tabla, ret);
                        return ret;
                    }

                #endregion
                #region Acceso

                case Constantes.ACCESO:
                    {
                        #region resolver un acceso
                        int no = nodo.ChildNodes.Count;
                        ret = resolverAccesoVar(nodo, ambiente, nombreClase, nombreMetodo,tabla,ret);
                        return ret;
                        /*if (no == 1)
                        {
                            ret = evaluarArbol(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla, ret);
                            return ret;
                        }*/

                        /*foreach (ParseTreeNode item in nodo.ChildNodes)
                        {
                            evaluarArbol(item, ambiente, nombreClase, nombreMetodo, tabla);
                        }
                        break;*/

                        break;
                        #endregion
                    }

                case Constantes.ID:
                    {
                        /*
                        string nombreVar = nodo.ChildNodes[0].Token.ValueString;
                        string ruta = ambiente.getAmbito();
                        Simbolo simb = tabla.buscarSimbolo(nombreVar, ambiente);
                        if (simb != null)
                        {
                            return simb.valor;
                        }
                        else
                        {
                            Constantes.erroresEjecucion.errorSemantico(nodo, "La variable " + nombreVar + ", no existe en el ambito actual " + ambiente.getAmbito());
                            return new Valor(Constantes.NULO, Constantes.NULO);
                        }*/
                        break;
                    }

                case Constantes.LLAMADA:
                    {
                        ret = this.llamadaFuncion(nodo, ambiente, nombreClase, nombreMetodo, tabla, ret);
                        return ret;
                        //break;
                    }

                case Constantes.POS_ARREGLO:
                    {
                        break;
                    }
                #endregion
                #region retorno
                case Constantes.RETORNO:
                    {
                        
                        ret.banderaRetorno = true;
                        int no = nodo.ChildNodes.Count;
                        if (no == 1)
                        {
                            elementoRetorno var = resolverExpresion(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
                            Simbolo simb = tabla.buscarSimbolo("retorno", ambiente);
                            if (simb != null)
                            {
                                simb.asignarValor(var.val, nodo.ChildNodes[0]);
                            }
                        }
                        
                        return ret;
                    }
                #endregion
                #region asignaUnario
                case Constantes.ASIGNA_UNARIO:
                    {
                        ret = asignacionUnario(nodo, ambiente, nombreClase, nombreMetodo, tabla, ret);

                        return ret;

                    }
                #endregion
                #region Caso
                case Constantes.CASO:
                    {
                        ret = resolverCaso(nodo, ambiente, nombreClase, nombreMetodo, tabla, ret);
                        return ret;
                    }
                #endregion
            }
            return ret;

        }

       
        #endregion


        /*--------------------- DEclaraciones locales y ASignaciones ----------------------------------------------*/

        #region Declaraciones Locales

        private elementoRetorno resolverDeclaracion(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            #region declaraion de vairables
            string rutaAcceso = ambiente.getAmbito();
            string tipo = nodo.ChildNodes[0].ChildNodes[0].Token.ValueString;
            string nombre = nodo.ChildNodes[1].Token.ValueString;
            bool esObj = this.esObjecto(tipo);
            int no = nodo.ChildNodes.Count;
            elementoRetorno v;
            if (no == 3)
            {
                if (!esObj)
                {
                    this.esAtriAsigna = false;
                    v = resolverExpresion(nodo.ChildNodes[2], ambiente, nombreClase, nombreMetodo, tabla);
                    Variable varNueva = new Variable(nombre, tipo, rutaAcceso, false);
                    varNueva.asignarValor(v.val, nodo);
                    varNueva.usada = true;
                    tabla.insertarSimbolo(varNueva, nodo);
                }
                else
                {
                    this.esAtriAsigna = false;
                    Clase claseBuscada = this.claseArchivo.obtenerClase(tipo);
                    if (claseBuscada != null)
                    {
                        Objeto nuevoObj = new Objeto(nombre, tipo, rutaAcceso, false);
                       // tabla.insertarSimbolo(nuevo, nodo);
                        Simbolo atriTemp;
                        ListaAtributos lTemporal = new ListaAtributos();

                        //cambiando de valor la ruta de acceso de los atribtos de la declaracion
                        string[] valoresRuta;
                       
                        lTemporal.lAtributos = claseBuscada.atributosClase.clonarLista();
                        for (int j = 0; j < lTemporal.lAtributos.Count; j++)
                        {
                            string rutaTemp = "";
                            atriTemp = lTemporal.lAtributos.ElementAt(j);
                            valoresRuta = atriTemp.rutaAcceso.Split('/');
                            valoresRuta[0] = ambiente.getAmbito() + "/" + nombre;
                            for (int i = 0; i < valoresRuta.Length; i++)
                            {
                                if (i == (valoresRuta.Length - 1))
                                {
                                    rutaTemp += valoresRuta[i];
                                }
                                else
                                {
                                    rutaTemp += valoresRuta[i] + "/";
                                }
                            }

                            atriTemp.rutaAcceso = rutaTemp;
                            if (atriTemp.nodoExpresionValor != null)
                            {
                                elementoRetorno r = new elementoRetorno();
                                r = resolverExpresion(atriTemp.nodoExpresionValor, ambiente, nombreClase, nombreMetodo, tabla);
                                atriTemp.asignarValor(r.val, atriTemp.nodoExpresionValor);
                            }
                            tabla.insertarSimbolo(atriTemp);
                        }
                        if (nodo.ChildNodes[2].Term.Name.Equals(Constantes.INSTANCIA, StringComparison.CurrentCultureIgnoreCase))
                        {
                            ambiente.addAmbito(nombre);
                            v = resolverExpresion(nodo.ChildNodes[2], ambiente, nombreClase, nombreMetodo, tabla);
                            ambiente.salirAmbito();
                        }
                        else
                        {
                            v = resolverExpresion(nodo.ChildNodes[2], ambiente, nombreClase, nombreMetodo, tabla);
                        }
                        nuevoObj.asignarValor(v.val, nodo.ChildNodes[2]);
                        nuevoObj.usada = false;
                        tabla.insertarSimbolo(nuevoObj, nodo);



                    }
                    else
                    {
                        Constantes.erroresEjecucion.errorSemantico("No se pudo declarar la variable de nombre " + nombre + ", de tipo " + tipo + ", no existe esa clase");
                    }





                }
            }
            else
            { //no ==2
                #region declaracion sin asignacion
                this.esAtriAsigna = false;
                if (!esObj)
                {
                    Variable varNueva = new Variable(nombre, tipo, rutaAcceso, false);
                    tabla.insertarSimbolo(varNueva, nodo);
                }
                else
                {
                    Clase claseBuscada = this.claseArchivo.obtenerClase(tipo);
                    if(claseBuscada!= null){
                        Objeto nuevo = new Objeto(nombre, tipo, rutaAcceso, false);
                        tabla.insertarSimbolo(nuevo, nodo);
                        Simbolo atriTemp;
                        ListaAtributos lTemporal = new ListaAtributos();

                        //cambiando de valor la ruta de acceso de los atribtos de la declaracion
                        string[] valoresRuta;
                        
                        lTemporal.lAtributos = claseBuscada.atributosClase.clonarLista();
                        for (int j = 0; j < lTemporal.lAtributos.Count; j++)
                        {
                            string rutaTemp = "";
                            atriTemp = lTemporal.lAtributos.ElementAt(j);
                            valoresRuta = atriTemp.rutaAcceso.Split('/');
                            valoresRuta[0] = ambiente.getAmbito() + "/" + nombre;
                            for (int i = 0; i < valoresRuta.Length; i++)
                            {
                                if (i == (valoresRuta.Length - 1))
                                {
                                    rutaTemp += valoresRuta[i];
                                }
                                else
                                {
                                    rutaTemp += valoresRuta[i]+"/";
                                }
                            }

                            atriTemp.rutaAcceso = rutaTemp;
                            if (atriTemp.nodoExpresionValor != null)
                            {
                                elementoRetorno r = new elementoRetorno();
                                r = resolverExpresion(atriTemp.nodoExpresionValor, ambiente, nombreClase, nombreMetodo, tabla);
                                atriTemp.asignarValor(r.val, atriTemp.nodoExpresionValor);
                            }
                            tabla.insertarSimbolo(atriTemp);
                        }


                    }else{
                        Constantes.erroresEjecucion.errorSemantico("No se pudo declarar la variable de nombre " + nombre + ", de tipo " + tipo + ", no existe esa clase");
                    }
                    




                   



                }
                #endregion
            }
            #endregion
            this.esAtriAsigna = false;
            return ret;
        }

        #endregion

        #region Asignaciones

        private elementoRetorno asignacionUnario(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            string nombreVar = nodo.ChildNodes[0].Token.ValueString;
            string operador = nodo.ChildNodes[1].Token.ValueString;
            Simbolo simb = tabla.buscarSimbolo(nombreVar, ambiente);
            if (simb != null)
            {
                if (esEntero(simb.valor) || esDecimal(simb.valor))
                {
                    if (operador.Equals(Constantes.MAS_MAS))
                    {
                        double d = getDecimal(simb.valor) + 1;
                        simb.valor.valor = d;
                    }
                    else
                    {
                        double d = getDecimal(simb.valor) - 1;
                        simb.valor.valor = d;

                    }
                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico(nodo, "Tipo de variable " + simb.tipo + ", es incompatible para un unario");
                }
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "No existe la variable " + nombreVar + ", en el ambito actual " + ambiente.getAmbito());
                tabla.mostrarSimbolos();
                //
            }
            return ret;
        }


        private elementoRetorno resolverAcceso2(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {

            return ret;
        }


        private elementoRetorno resolverAccesoVar(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {

            int numeroValores = nodo.ChildNodes.Count;
            if (numeroValores > 1)
            {
                bool banderaObjeto = false;
                bool banderaAtributo = false;
                ParseTreeNode elementoTemporal;
                int i = 0;
                string rutaElemento="";
                Valor valorAcceso;
                int contAmbitos = 0;
                do
                {
                    elementoTemporal = nodo.ChildNodes[i];
                    if (elementoTemporal.Term.Name.Equals(Constantes.ID, StringComparison.CurrentCultureIgnoreCase))
                    {
                        string nombreVar = elementoTemporal.ChildNodes[0].Token.ValueString;
                        if (nombreVar.Equals(Constantes.ESTE, StringComparison.CurrentCultureIgnoreCase))
                        {
                            banderaAtributo = true;
                        }
                        else
                        {
                            Simbolo simb = tabla.buscarSimbolo(nombreVar, ambiente);
                            if (simb != null)
                            {
                                string tipo = simb.tipo;
                                if (esObjecto(tipo))
                                {
                                    banderaObjeto = true;
                                }

                            }
                            else
                            {
                                Constantes.erroresEjecucion.errorSemantico(nodo, "La variable " + nombreVar + ", no existe en el ambito actual. Imposible resolver acceso");
                                tabla.mostrarSimbolos();
                                //
                            }
                        }

                    }
                    #region llamada
                    else if (elementoTemporal.Term.Name.Equals(Constantes.LLAMADA, StringComparison.CurrentCultureIgnoreCase))
                    {

                    }
                    #endregion
                    #region posArreglo
                    else
                    {
                        // es una posicion arreglo
                    }
                    #endregion


                    i++;
                } while (i < numeroValores && banderaObjeto);

            }
            else
            {// solo trae un elemento id, llamada o pos arreglo

            }

            return ret;
        }

        private elementoRetorno resolverAccesoRes(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            elementoRetorno ret = new elementoRetorno();

            return ret;
        }

        private elementoRetorno resolverAsignar(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            ParseTreeNode acceso = nodo.ChildNodes[0];
            ParseTreeNode expresion = nodo.ChildNodes[1];
            int cont = acceso.ChildNodes.Count;
            ParseTreeNode temp;
            string nombreElemento;
            for (int i = 0; i < acceso.ChildNodes.Count; i++)
            {
                temp = acceso.ChildNodes[i];
                if (temp.Term.Name.ToLower().Equals(Constantes.ID.ToLower()))
                {
                    nombreElemento = temp.ChildNodes[0].Token.ValueString;
                    if (expresion.Term.Name.Equals(Constantes.INSTANCIA, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ambiente.addAmbito(nombreElemento);
                    }
                    Simbolo simb = tabla.buscarSimbolo(nombreElemento, ambiente);
                    elementoRetorno r = new elementoRetorno();
                    if (simb != null)
                    {
                       
                        if (expresion.Term.Name.Equals(Constantes.INSTANCIA, StringComparison.InvariantCultureIgnoreCase))
                        {
                            //ambiente.addAmbito(simb.nombre);
                            r = resolverExpresion(expresion, ambiente, nombreClase, nombreMetodo, tabla);
                            ambiente.salirAmbito();
                        }
                        else
                        {
                            r = resolverExpresion(expresion, ambiente, nombreClase, nombreMetodo, tabla);
                        }

                    }
                    else
                    {
                        if (expresion.Term.Name.Equals(Constantes.INSTANCIA, StringComparison.InvariantCultureIgnoreCase))
                        {
                            ambiente.salirAmbito();
                        }
                        Constantes.erroresEjecucion.errorSemantico(expresion, "no existe la vairable " + nombreElemento + "en el ambiente " + ambiente.getAmbito() + "  aquii");
                        tabla.mostrarSimbolos();
                        //

                    }
                   
                    
                    Valor v = r.val;
                    if (simb != null)
                    {
                        simb.asignarValor(v, expresion);
                    }
                    else
                    {
                        Constantes.erroresEjecucion.errorSemantico(nodo, "No existe la variable " + nombreElemento + ", en el ambito actual " + ambiente.getAmbito());
                        tabla.mostrarSimbolos();
                        //
                    }

                }
                else if (temp.Term.Name.ToLower().Equals(Constantes.LLAMADA.ToLower()))
                {

                }
                else if (temp.Term.Name.ToLower().Equals(Constantes.POS_ARREGLO.ToLower()))
                {

                }
            }
            return ret;
        }


        #endregion



        /*-------------------------------- Fin declaraciones y Asignaciones ---------------------------------------*/


        /*----------------------------------- Estructuras de Control -----------------------------------------*/

        #region caso (Switch)

        private elementoRetorno resolverCaso(ParseTreeNode nodo, Contexto ambiente, string nombreClase, String nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            //CASO.Rule= ToTerm(Constantes.CASO) + ToTerm(Constantes.ABRE_PAR)+ EXPRESION +ToTerm(Constantes.CIERRA_PAR) +ToTerm(Constantes.DE) + CUERPO_CASO;
            ParseTreeNode expresionPivote = nodo.ChildNodes[0];
            ParseTreeNode cuerpoCaso = nodo.ChildNodes[1];
            Valor resPivote = resolverExpresion(expresionPivote, ambiente, nombreClase, nombreMetodo, tabla).val;
            if (!esNulo(resPivote))
            {
                // VALOR.Rule= EXPRESION +ToTerm(Constantes.DOS_PUNTOS)+ CUERPO_FUNCION;
                bool banderaEntroCaso = false;
                ParseTreeNode temp, expresionTemp, cuerpoTemp;

                string tipoCaso = "";
                Valor resCasoTemp;
                for (int i = 0; i < cuerpoCaso.ChildNodes.Count; i++)
                {
                    temp = cuerpoCaso.ChildNodes[i];
                    expresionTemp= temp.ChildNodes[0];
                    cuerpoTemp = temp.ChildNodes[1];
                    tipoCaso = temp.Term.Name.ToString();
                    if ((banderaEntroCaso == false) || (banderaEntroCaso == true && ret.banderaRetorno == false))
                    {
                        if (tipoCaso.Equals(Constantes.VALOR, StringComparison.CurrentCultureIgnoreCase))
                        {
                            resCasoTemp = resolverExpresion(expresionTemp, ambiente, nombreClase, nombreMetodo, tabla).val;
                            Valor v = igualIgual(resPivote, resCasoTemp);
                            if (esNulo(v))
                            {
                                Constantes.erroresEjecucion.errorSemantico(temp, "La expresion pivote de la sentencia caso es de tipo, "+ resPivote.tipo+", no se puede operar con una operacion con Nulo");
                            }
                            banderaEntroCaso = true;
                                foreach (ParseTreeNode item in cuerpoCaso.ChildNodes[0].ChildNodes)
                                {
                                    if (ret.parar)
                                    {
                                        ret.parar = true;
                                        break;
                                    }
                                    else
                                    {
                                        ret = evaluarArbol(item, ambiente, nombreClase, nombreMetodo, tabla, ret);

                                    }
                                }
                               
                        }
                        else
                        {
                            //es defecto 
                        }

                    }
                    else
                    {
                        if (ret.parar)
                        {
                            ret.parar = false;
                        }
                        break;
                    }
                    
                }
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(expresionPivote, "Expresion no valida para la sentencia Caso");
            }




            return ret;
        }

#endregion

        #region Resolver Si
        private elementoRetorno resolverSino(ParseTreeNode nodo, Contexto ambiente, String nombreClase, String nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            ambiente.addElse();
            foreach (ParseTreeNode item in nodo.ChildNodes)
            {
                ret = evaluarArbol(item, ambiente, nombreClase, nombreMetodo, tabla, ret);
            }
            ambiente.Ambitos.Pop();
            ret.banderaSi = true;
            return ret;
        }

        private elementoRetorno resolverSI(ParseTreeNode nodo, Contexto ambiente, String nombreClase, String nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            #region S_SI
            ParseTreeNode expresionIf = nodo.ChildNodes[0];
            ParseTreeNode sentenciasCuerpo = nodo.ChildNodes[1];
            elementoRetorno g = resolverExpresion(expresionIf, ambiente, nombreClase, nombreMetodo, tabla);
            Valor v = g.val;
            if (!esNulo(v))
            {
                if (esBooleano(v))
                {
                    if (esVerdadero(v))
                    {
                        ambiente.addIf();
                        foreach (ParseTreeNode item in sentenciasCuerpo.ChildNodes)
                        {
                            ret = evaluarArbol(item, ambiente, nombreClase, nombreMetodo, tabla, ret);
                        }
                        ambiente.Ambitos.Pop();
                        ret.banderaSi = true;
                        return ret;
                    }
                    else
                    {
                        ret.banderaSi = false;
                        return ret;
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
            ret.banderaSi = false;
            return ret;
        }


        #endregion

        /*----------------------------------- Fin Estructuras de Control -----------------------------------------*/


        /*----------------------------------------- Ciclos -----------------------------------------------------*/

       
        #region Para
        private elementoRetorno resolverPara(ParseTreeNode nodo, Contexto ambiente, string nombreClase, String nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            // PARA.Rule = ToTerm(Constantes.PARA) + ToTerm(Constantes.ABRE_PAR) + DEC_ASIG + EXPRESION + ToTerm(Constantes.PUNTO_COMA) + EXPRESION + ToTerm(Constantes.CIERRA_PAR) + CUERPO_FUNCION;
            ParseTreeNode nodoDeclaracion = nodo.ChildNodes[0].ChildNodes[0];
            ParseTreeNode nodoCondicion = nodo.ChildNodes[1];
            ParseTreeNode asignaUnario = nodo.ChildNodes[2];
            ParseTreeNode nodoCuerpo = nodo.ChildNodes[3];

            ambiente.addPara();
            ret = evaluarArbol(nodoDeclaracion, ambiente, nombreClase, nombreMetodo, tabla, ret);
            Valor v = resolverExpresion(nodoCondicion, ambiente, nombreClase, nombreMetodo, tabla).val;
            if (!esNulo(v))
            {
                if (esBool(v))
                {
                    while (esVerdadero(v))
                    {
                        foreach (ParseTreeNode item in nodoCuerpo.ChildNodes[0].ChildNodes)
                        {
                            if (ret.continuar)
                            {
                                break;
                            }
                            else if (ret.parar)
                            {
                                break;
                            }
                            else
                            {
                                ret = evaluarArbol(item, ambiente, nombreClase, nombreMetodo, tabla, ret);

                            }

                        }
                        if (ret.parar)
                        {
                            ret.parar = false;
                            break;
                        }
                        if (ret.continuar)
                        {
                            //resExpr = resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla);
                            ret = evaluarArbol(asignaUnario, ambiente, nombreClase, nombreMetodo, tabla, ret);
                            v = resolverExpresion(nodoCondicion, ambiente, nombreClase, nombreMetodo, tabla).val;
                            ret.continuar = false;
                            continue;

                        }
                        ret = evaluarArbol(asignaUnario, ambiente, nombreClase, nombreMetodo, tabla, ret);
                        v = resolverExpresion(nodoCondicion, ambiente, nombreClase, nombreMetodo, tabla).val;

                    }
                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico(nodoCondicion, "Condicion no validad para un ciclo para, con tipo " + v.tipo);

                }
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodoCondicion, "Condicion no validad para un ciclo para ");
            }





            ambiente.salirAmbito();





            return ret;
        }

        #endregion

        #region RepetirHasta

        private elementoRetorno resolverRepetirHasta(ParseTreeNode nodo, Contexto ambiente, string nombreClase, String nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            //ToTerm(Constantes.HACER) + CUERPO_FUNCION + ToTerm(Constantes.MIENTRAS) + ToTerm(Constantes.ABRE_PAR) + EXPRESION + ToTerm(Constantes.CIERRA_PAR) + ToTerm(Constantes.PUNTO_COMA);
            ParseTreeNode nodoCuerpo = nodo.ChildNodes[0];
            ParseTreeNode nodoExpresion = nodo.ChildNodes[1];
            elementoRetorno r = resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla);
            Valor resExpr = r.val;
            if (esBooleano(resExpr))
            {
                ambiente.addRepetir();
                // while (esVerdadero(resExpr))

                do
                {

                    foreach (ParseTreeNode item in nodoCuerpo.ChildNodes[0].ChildNodes)
                    {
                        if (ret.continuar)
                        {
                            break;
                        }
                        else if (ret.parar)
                        {
                            break;
                        }
                        else
                        {
                            ret = evaluarArbol(item, ambiente, nombreClase, nombreMetodo, tabla, ret);

                        }

                    }
                    if (ret.parar)
                    {
                        ret.parar = false;
                        break;
                    }
                    if (ret.continuar)
                    {
                        //resExpr = resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla);
                        ret.continuar = false;
                        continue;

                    }

                    resExpr = resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla).val;
                } while (!esVerdadero(resExpr));
                ambiente.salirAmbito();
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "La expresion para el ciclo hacer mientras  no es valida");
            }

            return ret;



        }

        #endregion

        #region HacerMientras
        private elementoRetorno resolverHacerMientras(ParseTreeNode nodo, Contexto ambiente, string nombreClase, String nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            //ToTerm(Constantes.HACER) + CUERPO_FUNCION + ToTerm(Constantes.MIENTRAS) + ToTerm(Constantes.ABRE_PAR) + EXPRESION + ToTerm(Constantes.CIERRA_PAR) + ToTerm(Constantes.PUNTO_COMA);
            ParseTreeNode nodoCuerpo = nodo.ChildNodes[0];
            ParseTreeNode nodoExpresion = nodo.ChildNodes[1];
            Valor resExpr = resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla).val;
            if (esBooleano(resExpr))
            {
                ambiente.addHacerMientras();
                // while (esVerdadero(resExpr))

                do
                {

                    foreach (ParseTreeNode item in nodoCuerpo.ChildNodes[0].ChildNodes)
                    {
                        if (ret.continuar)
                        {
                            break;
                        }
                        else if (ret.parar)
                        {
                            break;
                        }
                        else
                        {
                            ret = evaluarArbol(item, ambiente, nombreClase, nombreMetodo, tabla, ret);

                        }

                    }
                    if (ret.parar)
                    {
                        ret.parar = false;
                        break;
                    }
                    if (ret.continuar)
                    {
                        //resExpr = resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla);
                        ret.continuar = false;
                        continue;

                    }

                    resExpr = resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla).val;
                } while (esVerdadero(resExpr));
                ambiente.salirAmbito();
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "La expresion para el ciclo hacer mientras  no es valida");
            }

            return ret;



        }
        #endregion

        #region Mientras
        private elementoRetorno resolverMientras(ParseTreeNode nodo, Contexto ambiente, string nombreClase, String nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            //MIENTRAS.Rule = ToTerm(Constantes.MIENTRAS) + ToTerm(Constantes.ABRE_PAR) + EXPRESION + ToTerm(Constantes.CIERRA_PAR) + CUERPO_FUNCION;
            ParseTreeNode nodoExpresion = nodo.ChildNodes[0];
            ParseTreeNode nodoCuerpo = nodo.ChildNodes[1];
            Valor resExpr = resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla).val;
            if (esBooleano(resExpr))
            {
                ambiente.addMientras();
                while (esVerdadero(resExpr))
                {

                    // resExpr = resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla);
                    foreach (ParseTreeNode item in nodoCuerpo.ChildNodes[0].ChildNodes)
                    {
                        if (ret.continuar)
                        {
                            break;
                        }
                        else if (ret.parar)
                        {
                            break;
                        }
                        else
                        {
                            ret = evaluarArbol(item, ambiente, nombreClase, nombreMetodo, tabla, ret);

                        }

                    }
                    if (ret.parar)
                    {
                        ret.parar = false;
                        break;
                    }
                    if (ret.continuar)
                    {
                        //resExpr = resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla);
                        ret.continuar = false;
                        continue;

                    }

                    resExpr = resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla).val;
                }
                ambiente.salirAmbito();
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "La expresion para el ciclo mientrass no es valida");
            }

            return ret;

        }


        #endregion

        /*----------------------------------------- Fin Ciclos ------------------------------------------------------*/




        /*-------------------------------------- Llamada a Funciones -----------------------------------------------*/
        #region llamadaFuncion

        private elementoRetorno AccesoLlamadaFuncion(ParseTreeNode nodo, Contexto ambiente, string nombreClase, String nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            ParseTreeNode nodoId = nodo.ChildNodes[0];
            ParseTreeNode nodoParametros = nodo.ChildNodes[1];
            string nombreFuncion = nodoId.Token.ValueString.ToLower();
            int noParametros = nodoParametros.ChildNodes[0].ChildNodes.Count;

            //verificando a las fucniones nativas
            if (nombreFuncion.Equals(Constantes.SUPER.ToLower()))
            {

            }
            else if (nombreFuncion.Equals(Constantes.MENSAJE.ToLower()))
            {

            }else if(nombreFuncion.Equals(Constantes.CADENA,StringComparison.InvariantCultureIgnoreCase)){

            }


            return ret;
        }


        private elementoRetorno llamadaFuncion(ParseTreeNode nodo, Contexto ambiente, string nombreClase, String nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            ParseTreeNode nodoId = nodo.ChildNodes[0];
            ParseTreeNode nodoParametros = nodo.ChildNodes[1];
            string nombreFuncion = nodoId.Token.ValueString;
            int noParametros = nodoParametros.ChildNodes[0].ChildNodes.Count;

            /*-------- Paso 1:    Resolver parametros  e ir obteniendo la cadena de tipos de parametros  ---------------*/
            ParseTreeNode temp;
            List<Valor> valoresParametros = new List<Valor>();
            elementoRetorno temp2;

            // Resuelvo las expresiones que viene para los parametros 
            for (int i = 0; i < nodoParametros.ChildNodes[0].ChildNodes.Count; i++)
            {
                temp = nodoParametros.ChildNodes[0].ChildNodes[i];
                temp2 = resolverExpresion(temp, ambiente, nombreClase, nombreMetodo, tabla);
                valoresParametros.Add(temp2.val);
            }
            //genero la cadena de tipo parametros para buscar la funcion
            string cadParametros = "";
            Valor x;
            for (int i = 0; i < valoresParametros.Count; i++)
            {
                x = valoresParametros.ElementAt(i);
                cadParametros += x.tipo;
            }
            // busco la funcion 
            Funcion funBuscada = this.claseArchivo.obtenerFuncion(nombreClase, nombreFuncion, cadParametros);
            if (funBuscada != null)
            {
                tabla.crearNuevoAmbito(nombreFuncion);
                ambiente.addAmbito(nombreFuncion);
                //agrego los parametros y el return a la tabla de simbolos
                elementoRetorno ret2;
                ParseTreeNode paramTemp;
                ParseTreeNode nodoParametrosDecla = funBuscada.obtenerNodoParametros();

                // ingresando a la tabla de simbolos las variables de los parametros y obtener nombres de parametros 
                List<string> nombresParametros = new List<string>();
                for (int i = 0; i < nodoParametrosDecla.ChildNodes.Count; i++)
                {
                    paramTemp = nodoParametrosDecla.ChildNodes[i];
                    ret = this.evaluarArbol(paramTemp, ambiente, nombreClase, nombreMetodo, tabla, ret);
                    nombresParametros.Add(paramTemp.ChildNodes[1].Token.ValueString);
                }

                //ingresando el return
                if (esObjecto(funBuscada.tipo))
                {
                    Objeto nuevoObj = new Objeto("retorno", funBuscada.tipo, ambiente.getAmbito(), false);
                    tabla.insertarSimbolo(nuevoObj);
                }
                else
                {
                    Variable nuevaVar = new Variable("retorno", funBuscada.tipo, ambiente.getAmbito(), false);
                    tabla.insertarSimbolo(nuevaVar);

                }

                //Asignar parametros
                if (nombresParametros.Count == valoresParametros.Count)
                {
                    string nomParTemp = "";
                    Valor temporalVal;
                    Simbolo simbTemp;
                    for (int i = 0; i < nombresParametros.Count; i++)
                    {
                        nomParTemp = nombresParametros.ElementAt(i);
                        temporalVal = valoresParametros.ElementAt(i);
                        simbTemp = tabla.buscarSimbolo(nomParTemp, ambiente);
                        if (simbTemp != null)
                        {
                            simbTemp.asignarValor(temporalVal, nodoParametros.ChildNodes[0].ChildNodes[i]);
                        }
                        else
                        {
                            Constantes.erroresEjecucion.errorSemantico(nodo, "No existe la vriable parametro " + nomParTemp + ", en la funcion " + funBuscada.nombreFuncion);
                            tabla.mostrarSimbolos();
                            //
                        }
                    }

                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico(nodo, "No se puede realizar la llamada, no encajan los numero de parametros ");
                }
                // Ejecutar sentencias

                ret = evaluarArbol(funBuscada.cuerpoFuncion, ambiente, nombreClase, nombreMetodo, tabla, ret);
                Simbolo simb = tabla.buscarSimbolo("retorno", ambiente);
                if (simb != null)
                {
                    ret.val = simb.valor;

                }
                ambiente.salirAmbito();
                tabla.salirAmbiente();

            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "La funcion " + nombreFuncion + ", no existe en la clase actual " + nombreClase);
                tabla.mostrarSimbolos();
                //
            }
            return ret;
        }


        #endregion
        /*--------------------------------- Fin de llamada --------------------------------------------------------*/

             

        #region Imprimir
        private elementoRetorno imprimir(ParseTreeNode nodo, Contexto ambiente, String nombreClase, String nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
           
            elementoRetorno ff = resolverExpresion(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
            Valor v = ff.val;
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
            return ret;

        }
        #endregion

        #region extras

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

        #endregion



        /*---------------------------------------------------------- Resolviendo Expresiones  ------------------------------------------------------*/

        #region Resolver una expresion 

        #region resolverExpresion
        public elementoRetorno resolverExpresion(ParseTreeNode nodo, Contexto ambiente, String nombreClase, String nombreMetodo, tablaSimbolos tabla)
        {
            string nombreNodo = nodo.Term.Name;
            switch (nombreNodo)
            {

                case Constantes.EXPRESION:
                    {
                        return resolverExpresion(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
                    }


                case Constantes.UNARIO:
                    {
                        #region unario
                        string nombreVar = nodo.ChildNodes[0].Token.ValueString;
                        string operador = nodo.ChildNodes[1].Token.ValueString;
                        Simbolo simb = tabla.buscarSimbolo(nombreVar, ambiente);
                        elementoRetorno e = new elementoRetorno();

                        if (simb != null)
                        {
                            if (esEntero(simb.valor))
                            {
                                if (operador.Equals(Constantes.MAS_MAS))
                                {
                                    int d = getEntero(simb.valor) + 1;
                                    e.val = new Valor(Constantes.ENTERO, d);
                                }
                                else
                                {
                                    int d = getEntero(simb.valor) - 1;
                                    e.val = new Valor(Constantes.ENTERO, d);
                                }
                                
                                
                            } if (esDecimal(simb.valor))
                            {
                                if (operador.Equals(Constantes.MAS_MAS))
                                {
                                    double d = getDecimal(simb.valor) + 1;
                                    e.val = new Valor(Constantes.DECIMAL, d);
                                }
                                else
                                {
                                    double d = getDecimal(simb.valor) - 1;
                                    e.val = new Valor(Constantes.DECIMAL, d);
                                }
                                
                               
                                
                            }
                            else
                            {
                                Constantes.erroresEjecucion.errorSemantico(nodo, "Tipo de variable " + simb.tipo + ", es incompatible para un unario");
                            }
                        }
                        else
                        {
                            Constantes.erroresEjecucion.errorSemantico(nodo, "No existe la variable " + nombreVar + ", en el ambito actual " + ambiente.getAmbito());
                            tabla.mostrarSimbolos();
                            //
                        }
                        return e;
                        #endregion
                    }
                case Constantes.ARITMETICA:
                    {
                        #region resolver expresion aritmetica 
                        elementoRetorno v11 = resolverExpresion(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
                        elementoRetorno v22 = resolverExpresion(nodo.ChildNodes[2], ambiente, nombreClase, nombreMetodo, tabla);
                        Valor v1 = v11.val;
                        Valor v2 = v22.val;
                        switch (nodo.ChildNodes[1].Token.ValueString)
                        {
                            case Constantes.MAS:
                                {
                                    Valor v =  sumar(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una suma con los siguientes tipos " + v1.tipo + " y " + v2.tipo);
                                    }
                                    elementoRetorno r = new elementoRetorno();
                                    r.val = v;
                                    return r;
                                }
                            case Constantes.MENOS:
                                {
                                    Valor v = restar(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una resta con los siguientes tipos " + v1.tipo + " y " + v2.tipo);
                                    }
                                    elementoRetorno r = new elementoRetorno();
                                    r.val = v;
                                    return r;
                                }
                            case Constantes.MULTIPLICACION:
                                {
                                    Valor v = multiplicar(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una multiplicacion con los siguientes tipos " + v1.tipo + " y " + v2.tipo);
                                    }
                                    elementoRetorno r = new elementoRetorno();
                                    r.val = v;
                                    return r;
                                }

                            case Constantes.DIVISION:
                                {
                                    Valor v = dividir(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una division con los siguientes tipos " + v1.tipo + " y " + v2.tipo);
                                    }
                                    elementoRetorno r = new elementoRetorno();
                                    r.val = v;
                                    return r;
                                }

                            case Constantes.POTENCIA:
                                {
                                    Valor v = potencia(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una potencia con los siguientes tipos " + v1.tipo + " y " + v2.tipo);
                                    }
                                    elementoRetorno r = new elementoRetorno();
                                    r.val = v;
                                    return r;
                                }

                            case Constantes.MODULO:
                                {
                                    Valor v = modulo(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar un modulo con los siguientes tipos " + v1.tipo + " y " + v2.tipo);
                                    }
                                    elementoRetorno r = new elementoRetorno();
                                    r.val = v;
                                    return r;
                                }

                        }

                        break;
#endregion
                    }

                case Constantes.RELACIONAL:
                    {
                        #region resolver expresion relacional
                        elementoRetorno v11 = resolverExpresion(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
                        elementoRetorno v22 = resolverExpresion(nodo.ChildNodes[2], ambiente, nombreClase, nombreMetodo, tabla);
                        Valor v1 = v11.val;
                        Valor v2 = v22.val;
                        switch (nodo.ChildNodes[1].Token.ValueString)
                        {
                            case Constantes.MENOR:
                                {
                                    Valor v = menor(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una operacion relacional Menor, con el tipo " + v1.tipo + ", con " + v2.tipo);
                                    }
                                    elementoRetorno r = new elementoRetorno();
                                    r.val = v;
                                    return r;
                                }

                            case Constantes.MAYOR:
                                {
                                    Valor v = mayor(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una operacion relacional Mayor, con el tipo " + v1.tipo + ", con " + v2.tipo);
                                    }
                                    elementoRetorno r = new elementoRetorno();
                                    r.val = v;
                                    return r;
                                }

                            case Constantes.MENOR_IGUAL:
                                {
                                    Valor v = menorIgual(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una operacion relacional Menor igual, con el tipo " + v1.tipo + ", con " + v2.tipo);
                                    }
                                    elementoRetorno r = new elementoRetorno();
                                    r.val = v;
                                    return r;
                                }

                            case Constantes.MAYOR_IGUAL:
                                {
                                    Valor v = mayorIgual(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una operacion relacional Mayor igual, con el tipo " + v1.tipo + ", con " + v2.tipo);
                                    }
                                    elementoRetorno r = new elementoRetorno();
                                    r.val = v;
                                    return r;
                                }

                            case Constantes.IGUAL_IGUAL:
                                {
                                    Valor v = igualIgual(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una operacion relacional Igual, con el tipo " + v1.tipo + ", con " + v2.tipo);
                                    }
                                    elementoRetorno r = new elementoRetorno();
                                    r.val = v;
                                    return r;
                                }

                            case Constantes.DISTINTO_A:
                                {
                                    Valor v = distintoA(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una operacion relacional Distinto a, con el tipo " + v1.tipo + ", con " + v2.tipo);
                                    }
                                    elementoRetorno r = new elementoRetorno();
                                    r.val = v;
                                    return r;
                                }


                        }


                        break;
                        #endregion                       
                    }


                case Constantes.LOGICA:
                    {
                        #region resolver una expresion logica 
                        elementoRetorno v11 = resolverExpresion(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
                        elementoRetorno v22 = resolverExpresion(nodo.ChildNodes[2], ambiente, nombreClase, nombreMetodo, tabla);
                        Valor v1 = v11.val;
                        Valor v2 = v22.val;
                        switch (nodo.ChildNodes[1].Token.ValueString)
                        {
                            case Constantes.AND:
                                {
                                    Valor v = And(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una operacion logica and, con el tipo " + v1.tipo + ", con " + v2.tipo);
                                    }
                                    elementoRetorno r = new elementoRetorno();
                                    r.val = v;
                                    return r;
                                }

                            case Constantes.OR:
                                {
                                    Valor v = Or(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una operacion logica Or, con el tipo " + v1.tipo + ", con " + v2.tipo);
                                    }
                                    elementoRetorno r = new elementoRetorno();
                                    r.val = v;
                                    return r;
                                }
                        }

                        break;
                        #endregion
                    }

                case Constantes.ACCESO:
                    {
                        #region resolver un acceso
                        int no = nodo.ChildNodes.Count;
                        if (no == 1)
                        {
                           

                            return resolverExpresion(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
                        }

                        /*foreach (ParseTreeNode item in nodo.ChildNodes)
                        {
                            evaluarArbol(item, ambiente, nombreClase, nombreMetodo, tabla);
                        }
                        break;*/

                        break;
                        #endregion
                    }

                case Constantes.INSTANCIA:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret = this.resolverInstancia(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return  ret;
                    }

                #region resolver Acceso
                case Constantes.ID:{
                    string nombreVar = nodo.ChildNodes[0].Token.ValueString;
                    string ruta= ambiente.getAmbito();
                    Simbolo simb = tabla.buscarSimbolo(nombreVar, ambiente);
                    if (simb != null)
                    {
                        elementoRetorno r = new elementoRetorno();
                        r.val = simb.valor;
                        return r;
                    }
                    else
                    {
                        Constantes.erroresEjecucion.errorSemantico(nodo, "La variable " + nombreVar + ", no existe en el ambito actual "+ ambiente.getAmbito());
                        tabla.mostrarSimbolos();
                        //
                        return new elementoRetorno();
                    }
                }

                case Constantes.LLAMADA:
                    {

                        elementoRetorno var = this.llamadaFuncion(nodo, ambiente, nombreClase, nombreMetodo, tabla, new elementoRetorno());
                        return var;
                    }

                case Constantes.POS_ARREGLO:
                    {
                        break;
                    }
                #endregion
                #region valoresNativos
                case Constantes.FECHA_HORA:
                    {
                        FechaHora nueva = new FechaHora(nodo,  nodo.ChildNodes[0].Token.ValueString);
                        elementoRetorno r = new elementoRetorno();
                        r.val = nueva.validarFechaHora();
                        return r;
                        
                    }

                case Constantes.HORA:
                    {
                        Hora nueva = new Hora(nodo, nodo.ChildNodes[0].Token.ValueString);
                        elementoRetorno r = new elementoRetorno();
                        r.val = nueva.validarHora();
                        return r;
                       
                    }
                case Constantes.FECHA:
                    {
                        Fecha nueva = new Fecha(nodo, nodo.ChildNodes[0].Token.ValueString);
                        elementoRetorno r = new elementoRetorno();
                        r.val = nueva.validarFecha();
                        return r;
                     
                    }

                case Constantes.ENTERO:
                    {
                        int imagen = int.Parse(nodo.ChildNodes[0].Token.ValueString);
                        Valor ret = new Valor(Constantes.ENTERO, imagen);
                        elementoRetorno r = new elementoRetorno();
                        r.val = ret;
                        return r;
                    }

                case Constantes.CADENA:
                    {
                        String cad = nodo.ChildNodes[0].Token.ValueString;
                        Valor v=  new Valor(Constantes.CADENA, cad);
                        elementoRetorno r = new elementoRetorno();
                        r.val = v;
                        return r;
                    }


                case Constantes.CHAR:
                    {
                        char cad = char.Parse(nodo.ChildNodes[0].Token.ValueString);
                        Valor v= new Valor(Constantes.CHAR, cad);
                        elementoRetorno r = new elementoRetorno();
                        r.val = v;
                        return r;
                    }

                case Constantes.DECIMAL:
                    {
                        double cad = double.Parse(nodo.ChildNodes[0].Token.ValueString);
                        Valor v=  new Valor(Constantes.DECIMAL, cad);
                        elementoRetorno r = new elementoRetorno();
                        r.val = v;
                        return r;
                    }

                case Constantes.BOOLEANO:
                    {
                        String cad = nodo.ChildNodes[0].Token.ValueString;
                        Valor v =  new Valor(Constantes.BOOLEANO, cad);
                        elementoRetorno r = new elementoRetorno();
                        r.val = v;
                        return r;
                    }

                #endregion

            }

            return new elementoRetorno();
        }

        #endregion

        private elementoRetorno resolverInstancia(ParseTreeNode nodo, Contexto ambiente, String nombreClase, String nombreMetodo, tablaSimbolos tabla)
        {
            //INSTANCIA.Rule= ToTerm(Constantes.NUEVO)+ TIPO_DATOS+ PARAMETROS_LLAMADA;
            elementoRetorno ret = new elementoRetorno();
            ret.val = new Valor(Constantes.NULO, Constantes.NULO);
            string tipoInstancia = nodo.ChildNodes[0].ChildNodes[0].Token.ValueString;
            ParseTreeNode nodoParametros = nodo.ChildNodes[1];
            List<Valor> valoresParametros = resolviendoParametros(nodoParametros, ambiente, nombreClase, nombreMetodo, tabla);
            string cadParametros = obtenerCadenaParametros(valoresParametros);

            Clase claseTemporal;
            for (int i = 0; i < claseArchivo.size(); i++)
            {
                claseTemporal = claseArchivo.get(i);
                if (claseTemporal.nombreClase.Equals(tipoInstancia, StringComparison.CurrentCultureIgnoreCase))
                {
                    Funcion funBuscada = this.claseArchivo.obtenerFuncion(tipoInstancia, tipoInstancia, cadParametros);
                    if (funBuscada != null)
                    {
                        

                        ambiente.addAmbito(tipoInstancia);
                        tabla.crearNuevoAmbito(tipoInstancia);

                        /*insertar parametros*/
                        elementoRetorno ret2;
                        ParseTreeNode paramTemp;
                        ParseTreeNode nodoParametrosDecla = funBuscada.obtenerNodoParametros();

                        // ingresando a la tabla de simbolos las variables de los parametros y obtener nombres de parametros 
                        List<string> nombresParametros = new List<string>();
                        for (int h = 0; h < nodoParametrosDecla.ChildNodes.Count; h++)
                        {
                            paramTemp = nodoParametrosDecla.ChildNodes[h];
                            ret = this.evaluarArbol(paramTemp, ambiente, nombreClase, nombreMetodo, tabla, ret);
                            nombresParametros.Add(paramTemp.ChildNodes[1].Token.ValueString);
                        }


                        //Asignamos los parametros 
                        if (nombresParametros.Count == valoresParametros.Count)
                        {
                            string nomParTemp = "";
                            Valor temporalVal;
                            Simbolo simbTemp;
                            for (int k = 0; k < nombresParametros.Count; k++)
                            {
                                nomParTemp = nombresParametros.ElementAt(k);
                                temporalVal = valoresParametros.ElementAt(k);
                                simbTemp = tabla.buscarSimbolo(nomParTemp, ambiente);
                                if (simbTemp != null)
                                {
                                    simbTemp.asignarValor(temporalVal, nodoParametros.ChildNodes[0].ChildNodes[k]);
                                }
                                else
                                {
                                    Constantes.erroresEjecucion.errorSemantico(nodo, "No existe la vriable parametro " + nomParTemp + ", en la funcion " + funBuscada.nombreFuncion);
                                    tabla.mostrarSimbolos();
                                    //
                                }
                            }

                        }
                        else
                        {
                            Constantes.erroresEjecucion.errorSemantico(nodo, "No se puede realizar la llamada, no encajan los numero de parametros ");
                        }

                        ret = evaluarArbol(funBuscada.cuerpoFuncion, ambiente, tipoInstancia, tipoInstancia, tabla, ret);
                        Simbolo simb = tabla.buscarSimbolo("retorno", ambiente);
                        if (simb != null)
                        {
                            ret.val = simb.valor;

                        }



                        tabla.salirAmbiente();
                        ambiente.salirAmbito();
                        /*insertamos el ambito del constructor*/

                    }

                }
            }
            

            /*

            // busco la funcion 
            Funcion funBuscada = this.claseArchivo.obtenerFuncion(tipoInstancia, tipoInstancia, cadParametros);
            if (funBuscada != null)
            {
                Clase temp;
                for (int i = 0; i < claseArchivo.size(); i++)
                {
                    temp = claseArchivo.get(i);
                    if (temp.nombreClase.Equals(tipoInstancia, StringComparison.CurrentCultureIgnoreCase))
                    {
                        
                        
                        ambiente.addAmbito(tipoInstancia);
                        tabla.crearNuevoAmbito(tipoInstancia);
                        //agrego los parametros 
                        ParseTreeNode paramTemp;
                        ParseTreeNode nodoParametrosDecla = funBuscada.obtenerNodoParametros();

                        // ingresando a la tabla de simbolos las variables de los parametros y obtener nombres de parametros 
                        List<string> nombresParametros = new List<string>();
                        for (int h = 0; h < nodoParametrosDecla.ChildNodes.Count; h++)
                        {
                            paramTemp = nodoParametrosDecla.ChildNodes[h];
                            ret = this.evaluarArbol(paramTemp, ambiente, temp.nombreClase, tipoInstancia, tabla, ret);
                            nombresParametros.Add(paramTemp.ChildNodes[1].Token.ValueString);
                        }

                        if (nombresParametros.Count == valoresParametros.Count)
                        {
                            string nomParTemp = "";
                            Valor temporalVal;
                            Simbolo simbTemp;
                            for (int h = 0; h < nombresParametros.Count; h++)
                            {
                                nomParTemp = nombresParametros.ElementAt(h);
                                temporalVal = valoresParametros.ElementAt(h);
                                simbTemp = tabla.buscarSimbolo(nomParTemp, ambiente);
                                if (simbTemp != null)
                                {
                                    simbTemp.asignarValor(temporalVal, nodoParametros.ChildNodes[0].ChildNodes[h]);
                                }
                                else
                                {
                                    Constantes.erroresEjecucion.errorSemantico(nodo, "No existe la vriable parametro " + nomParTemp + ", en la funcion " + funBuscada.nombreFuncion);
                                    tabla.mostrarSimbolos();
                                    //
                                }
                            }

                        }
                        else
                        {
                            Constantes.erroresEjecucion.errorSemantico(nodo, "No se puede realizar la llamada, no encajan los numero de parametros ");
                        }
                        //
                        ret = evaluarArbol(funBuscada.cuerpoFuncion, ambiente, temp.nombreClase, tipoInstancia, tabla, ret);
                        ambiente.Ambitos.Pop();
                        tabla.salirAmbiente();
                        ret.val = new Valor(tipoInstancia, tabla.listaSimbolos.Peek());
                        return ret;
                    }
                }
            }
            else
            {
                ret.val = new Valor(Constantes.NULO, Constantes.NULO);
                return ret;
            }*/

            return ret;
        }

        private string obtenerCadenaParametros(List<Valor> valoresParametros )
        {
            string cadParametros = "";
            Valor x;
            for (int i = 0; i < valoresParametros.Count; i++)
            {
                x = valoresParametros.ElementAt(i);
                cadParametros += x.tipo;
            }
            return cadParametros;

        }

        public List<Valor> resolviendoParametros(ParseTreeNode nodo, Contexto ambiente, String nombreClase, String nombreMetodo, tablaSimbolos tabla)
        {
            List<Valor> expresionesPArametros = new List<Valor>();
            ParseTreeNode temp;
            Valor temp2;
            for (int i = 0; i < nodo.ChildNodes[0].ChildNodes.Count; i++)
            {
                temp = nodo.ChildNodes[0].ChildNodes[i];
                temp2 = resolverExpresion(temp, ambiente, nombreClase, nombreMetodo, tabla).val;
                expresionesPArametros.Add(temp2);
            }

            return expresionesPArametros;
        }


        #region Operaciones Logicas
        #region Or
        private Valor Or(Valor v1, Valor v2)
        {
            Valor resp = new Valor(Constantes.NULO, Constantes.NULO);
            if (esBool(v1) && esBool(v2))
            {
                int b1 = getBooleanoNumero(v1);
                int b2 = getBooleanoNumero(v2);
                int res = b1 + b2;
                if (res > 0)
                {
                    resp = new Valor(Constantes.BOOLEANO, Constantes.VERDADERO);
                }
                else
                {
                    resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
                }

            }

            return resp;
        }

        #endregion


        #region And

        private Valor And(Valor v1, Valor v2)
        {
            Valor resp= new Valor(Constantes.NULO, Constantes.NULO);
            if (esBool(v1) && esBool(v2))
            {
                int b1 = getBooleanoNumero(v1);
                int b2 = getBooleanoNumero(v2);
                int res = b1 * b2;
                if (res > 0)
                {
                    resp = new Valor(Constantes.BOOLEANO, Constantes.VERDADERO);
                }else{
                    resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
                }

            }

            return resp;
        }


        #endregion

#endregion


        #region Operaciones Relacionales

        #region Menor

        private Valor menor(Valor v1, Valor v2)
        {
            Valor resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
            if (esEntero(v1) && esEntero(v2))
            {
                if (getEntero(v1) < getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                if (getEntero(v1) < getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esEntero(v1) && esCaracter(v2))
            {
                if (getEntero(v1) < getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                if (getDecimal(v1) < getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                if (getDecimal(v1) < getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esCaracter(v2))
            {
                if (getDecimal(v1) < getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esCaracter(v1) && esCaracter(v2))
            {
                if (getCharNumero(v1) < getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esEntero(v2))
            {
                if (getCharNumero(v1) < getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esDecimal(v2))
            {
                if (getCharNumero(v1) < getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCadena(v1) && esCadena(v2))
            {
                if(getCadena(v1).CompareTo(getCadena(v2)) < 0){
                    resp.valor = Constantes.VERDADERO;
                }
            }

            /*si se tuvieran que incluir los demas tipos*/
            return new Valor(Constantes.NULO, Constantes.NULO);
        }


        #endregion


        #region MayorIgual

        private Valor mayorIgual(Valor v1, Valor v2)
        {
            Valor resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
            if (esEntero(v1) && esEntero(v2))
            {
                if (getEntero(v1) >= getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                if (getEntero(v1) >= getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esEntero(v1) && esCaracter(v2))
            {
                if (getEntero(v1) >= getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                if (getDecimal(v1) >= getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                if (getDecimal(v1) >= getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esCaracter(v2))
            {
                if (getDecimal(v1) >= getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esCaracter(v1) && esCaracter(v2))
            {
                if (getCharNumero(v1) >= getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esEntero(v2))
            {
                if (getCharNumero(v1) >= getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esDecimal(v2))
            {
                if (getCharNumero(v1) >= getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCadena(v1) && esCadena(v2))
            {
                if (getCadena(v1).CompareTo(getCadena(v2)) >= 0)
                {
                    resp.valor = Constantes.VERDADERO;
                }
            }

            /*si se tuvieran que incluir los demas tipos*/
            return new Valor(Constantes.NULO, Constantes.NULO);
        }

        #endregion


        #region Mayor
        private Valor mayor(Valor v1, Valor v2)
        {
            Valor resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
            if (esEntero(v1) && esEntero(v2))
            {
                if (getEntero(v1) > getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                if (getEntero(v1) > getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esEntero(v1) && esCaracter(v2))
            {
                if (getEntero(v1) > getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                if (getDecimal(v1) > getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                if (getDecimal(v1) > getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esCaracter(v2))
            {
                if (getDecimal(v1) > getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esCaracter(v1) && esCaracter(v2))
            {
                if (getCharNumero(v1) > getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esEntero(v2))
            {
                if (getCharNumero(v1) > getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esDecimal(v2))
            {
                if (getCharNumero(v1) > getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }

            /*si se tuvieran que incluir los demas tipos*/
            return new Valor(Constantes.NULO, Constantes.NULO);
        }
        #endregion Mayor


        #region MenorIgual

        private Valor menorIgual(Valor v1, Valor v2)
        {
            Valor resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
            if (esEntero(v1) && esEntero(v2))
            {
                if (getEntero(v1) <= getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                if (getEntero(v1) <= getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esEntero(v1) && esCaracter(v2))
            {
                if (getEntero(v1) <= getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                if (getDecimal(v1) <= getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                if (getDecimal(v1) <= getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esCaracter(v2))
            {
                if (getDecimal(v1) <= getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esCaracter(v1) && esCaracter(v2))
            {
                if (getCharNumero(v1) <= getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esEntero(v2))
            {
                if (getCharNumero(v1) <= getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esDecimal(v2))
            {
                if (getCharNumero(v1) <= getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }

            /*si se tuvieran que incluir los demas tipos*/
            return new Valor(Constantes.NULO, Constantes.NULO);
        }
        #endregion

        #region igualIgual
        private Valor igualIgual(Valor v1, Valor v2)
        {
            Valor resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
            if (esEntero(v1) && esEntero(v2))
            {
                if (getEntero(v1) == getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                if (getEntero(v1) ==getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esEntero(v1) && esCaracter(v2))
            {
                if (getEntero(v1) == getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                if (getDecimal(v1) == getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                if (getDecimal(v1) == getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esCaracter(v2))
            {
                if (getDecimal(v1) == getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esCaracter(v1) && esCaracter(v2))
            {
                if (getCharNumero(v1) == getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esEntero(v2))
            {
                if (getCharNumero(v1) == getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esDecimal(v2))
            {
                if (getCharNumero(v1) == getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCadena(v1) && esCadena(v2))
            {
                if (getCadena(v1).CompareTo(getCadena(v2)) == 0)
                {
                    resp.valor = Constantes.VERDADERO;
                }
            }

            /*si se tuvieran que incluir los demas tipos*/
            return new Valor(Constantes.NULO, Constantes.NULO);
        }
        #endregion


        #region disntintoA
        private Valor distintoA(Valor v1, Valor v2)
        {
            Valor resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
            if (esEntero(v1) && esEntero(v2))
            {
                if (getEntero(v1) != getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                if (getEntero(v1) != getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esEntero(v1) && esCaracter(v2))
            {
                if (getEntero(v1) != getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                if (getDecimal(v1) != getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                if (getDecimal(v1) != getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esCaracter(v2))
            {
                if (getDecimal(v1) != getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esCaracter(v1) && esCaracter(v2))
            {
                if (getCharNumero(v1) != getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esEntero(v2))
            {
                if (getCharNumero(v1) != getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esDecimal(v2))
            {
                if (getCharNumero(v1) != getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCadena(v1) && esCadena(v2))
            {
                if (!(getCadena(v1).Equals(getCadena(v2))))
                {
                    resp.valor = Constantes.VERDADERO;
                }
            }

            /*si se tuvieran que incluir los demas tipos*/
            return new Valor(Constantes.NULO, Constantes.NULO);
        }
        #endregion


        #endregion


        #region Aritmeticas

        #region Potencia

        private Valor potencia(Valor v1, Valor v2)
        {
            Valor resp;
            if (esBool(v1) && esEntero(v2))
            {
                int a = getBooleanoNumero(v1);
                int b = getEntero(v2);
                double c = Math.Pow(a , b);
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esBool(v1) && esDecimal(v2))
            {
                int a = getBooleanoNumero(v1);
                double b = getDecimal(v2);
                double c = Math.Pow(a, b);
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esEntero(v1) && esEntero(v2))
            {
                int a = getEntero(v1);
                int b = getEntero(v2);
                double c = Math.Pow(a, b);
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                int a = getEntero(v1);
                double b = getDecimal(v2);
                double c = Math.Pow(a, b);
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                int a = getEntero(v2);
                double b = getDecimal(v1);
                double c = Math.Pow(b, a);
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                double a = getDecimal(v1);
                double b = getDecimal(v2);
                double c = Math.Pow(a, b);
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;
            }
            else if (esDecimal(v1) && esBool(v2))
            {
                double a = getDecimal(v1);
                int b = getBooleanoNumero(v2);
                double c = Math.Pow(a, b);
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esEntero(v1) && esBool(v2))
            {
                int a = getBooleanoNumero(v2);
                int b = getEntero(v1);
                double c = Math.Pow(b, a);
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            return new Valor(Constantes.NULO, Constantes.NULO);
        }
        #endregion


        #region modulo

        private Valor modulo(Valor v1, Valor v2)
        {
            Valor resp;
           if (esEntero(v1) && esEntero(v2))
            {
                int a = getEntero(v1);
                int b = getEntero(v2);
                double c = a % b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                int a = getEntero(v1);
                double b = getDecimal(v2);
                double c = a % b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                int a = getEntero(v2);
                double b = getDecimal(v1);
                double c = b % a;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                double a = getDecimal(v1);
                double b = getDecimal(v2);
                double c = a % b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;
            }
           
            return new Valor(Constantes.NULO, Constantes.NULO);
        }
        #endregion



        #region Divisiones

        private Valor dividir(Valor v1, Valor v2)
        {
            Valor resp;
            if (esBool(v1) && esEntero(v2))
            {
                int a = getBooleanoNumero(v1);
                int b = getEntero(v2);
                if (!esCero(b))
                {
                    int c = a / b;
                    resp = new Valor(Constantes.ENTERO, c);
                    return resp;
                }
                else
                {

                }
                
            }
            else if (esBool(v1) && esDecimal(v2))
            {
                int a = getBooleanoNumero(v1);
                double b = getDecimal(v2);
                if (!esCero(b))
                {
                    double c = a / b;
                    resp = new Valor(Constantes.DECIMAL, c);
                    return resp;

                }
                else
                {

                }
               

            }
            else if (esEntero(v1) && esEntero(v2))
            {
                int a = getEntero(v1);
                int b = getEntero(v2);
                if (!esCero(b))
                {
                    int c = a / b;
                    resp = new Valor(Constantes.ENTERO, c);
                    return resp;

                }
                else
                {

                }
                

            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                int a = getEntero(v1);
                double b = getDecimal(v2);
                if (!esCero(b))
                {
                    double c = a / b;
                    resp = new Valor(Constantes.DECIMAL, c);
                    return resp;

                }
                else
                {

                }
                

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                int a = getEntero(v2);
                double b = getDecimal(v1);
                if (!esCero(b))
                {
                    double c = b / a;
                    resp = new Valor(Constantes.DECIMAL, c);
                    return resp;

                }
                else
                {

                }
               

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                double a = getDecimal(v1);
                double b = getDecimal(v2);
                if (!esCero(b))
                {
                    double c = a / b;
                    resp = new Valor(Constantes.DECIMAL, c);
                    return resp;

                }
                else
                {

                }
                
            }
            else if (esDecimal(v1) && esBool(v2))
            {
                double a = getDecimal(v1);
                int b = getBooleanoNumero(v2);
                if (!esCero(b))
                {
                    double c = a / b;
                    resp = new Valor(Constantes.DECIMAL, c);
                    return resp;

                }
                else
                {

                }
                

            }
            else if (esEntero(v1) && esBool(v2))
            {
                int a = getBooleanoNumero(v2);
                int b = getEntero(v1);
                if (!esCero(b))
                {
                    int c = b / a;
                    resp = new Valor(Constantes.ENTERO, c);
                    return resp;

                }
                else
                {

                }
                

            }
            return new Valor(Constantes.NULO, Constantes.NULO);
        }

        #endregion



        #region multiplicaciones

        private Valor multiplicar(Valor v1, Valor v2)
        {
            Valor resp;
            if (esBool(v1) && esEntero(v2))
            {
                int a = getBooleanoNumero(v1);
                int b = getEntero(v2);
                int c = a * b;
                resp = new Valor(Constantes.ENTERO, c);
                return resp;

            }
            else if (esBool(v1) && esDecimal(v2))
            {
                int a = getBooleanoNumero(v1);
                double b = getDecimal(v2);
                double c = a * b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esEntero(v1) && esEntero(v2))
            {
                int a = getEntero(v1);
                int b = getEntero(v2);
                int c = a * b;
                resp = new Valor(Constantes.ENTERO, c);
                return resp;

            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                int a = getEntero(v1);
                double b = getDecimal(v2);
                double c = a * b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                int a = getEntero(v2);
                double b = getDecimal(v1);
                double c = b * a;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                double a = getDecimal(v1);
                double b = getDecimal(v2);
                double c = a * b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;
            }
            else if (esDecimal(v1) && esBool(v2))
            {
                double a = getDecimal(v1);
                int b = getBooleanoNumero(v2);
                double c = a * b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esEntero(v1) && esBool(v2))
            {
                int a = getBooleanoNumero(v2);
                int b = getEntero(v1);
                int c = b * a;
                resp = new Valor(Constantes.ENTERO, c);
                return resp;

            }
            else if (esBool(v1) && esBool(v2))
            {
                int a = getBooleanoNumero(v2);
                int b = getBooleanoNumero(v1);
                int c = b + a;
                if (c == 2)
                {
                    resp = new Valor(Constantes.BOOLEANO, Constantes.VERDADERO);
                }
                else
                {
                    resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);

                }
                return resp;

            }

            return new Valor(Constantes.NULO, Constantes.NULO);
        }

        #endregion



        #region restas

        private Valor restar(Valor v1, Valor v2)
        {
            Valor resp;
            if (esBool(v1) && esEntero(v2))
            {
                int a = getBooleanoNumero(v1);
                int b = getEntero(v2);
                int c = a - b;
                resp = new Valor(Constantes.ENTERO, c);
                return resp;

            }
            else if (esBool(v1) && esDecimal(v2))
            {
                int a = getBooleanoNumero(v1);
                double b = getDecimal(v2);
                double c = a - b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esEntero(v1) && esEntero(v2))
            {
                int a = getEntero(v1);
                int b = getEntero(v2);
                int c = a - b;
                resp = new Valor(Constantes.ENTERO, c);
                return resp;

            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                int a = getEntero(v1);
                double b = getDecimal(v2);
                double c = a - b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                int a = getEntero(v2);
                double b = getDecimal(v1);
                double c = b-a;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                double a = getDecimal(v1);
                double b = getDecimal(v2);
                double c = a - b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;
            }
            else if (esDecimal(v1) && esBool(v2))
            {
                double a = getDecimal(v1);
                int b = getBooleanoNumero(v2);
                double c = a - b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esEntero(v1) && esBool(v2))
            {
                int a = getBooleanoNumero(v2);
                int b = getEntero(v1);
                int c = b-a;
                resp = new Valor(Constantes.ENTERO, c);
                return resp;

            }
            return new Valor(Constantes.NULO, Constantes.NULO);
        }

        #endregion




        #region Sumas

        private Valor sumar(Valor v1, Valor v2)
        {
            Valor resp;
            /*--------------- Validaciones con booleanos -------------------*/
            if (esBool(v1) && esBool(v2))
            {
                int b1 = getBooleanoNumero(v1);
                int b2 = getBooleanoNumero(v2);
                int r = b1 + b2;
                if (r > 0)
                {
                    resp = new Valor(Constantes.BOOLEANO, Constantes.VERDADERO);
                    return resp;
                }
                else
                {
                    resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
                    return resp;
                }
            }
            else if (esBool(v1) && esEntero(v2))
            {
                int b1 = getBooleanoNumero(v1);
                int e1 = getEntero(v2);
                int r = b1 + e1;
                resp = new Valor(Constantes.ENTERO, r);
                return resp;
            }
            else if (esBool(v1) && esDecimal(v2))
            {
                int b1 = getBooleanoNumero(v1);
                double d2 = getDecimal(v2);
                double v = b1 + d2;
                resp = new Valor(Constantes.DECIMAL, v);
                return resp;
            }
            else if (esBool(v1) && esCadena(v2))
            {
                int b1 = getBooleanoNumero(v1);
                string c1 = getCadena(v2);
                resp = new Valor(Constantes.CADENA, b1 + c1 + "");
                return resp;
            }

            //validaciones con enteros

            else if (esEntero(v1) && esBool(v2))
            {
                int b1 = getEntero(v1);
                int b2 = getBooleanoNumero(v2);
                int c3 = b1 + b2;
                resp = new Valor(Constantes.ENTERO, c3);
                return resp;

            }else if(esEntero(v1) && esEntero(v2)){
                int b1 = getEntero(v1);
                int b2 = getEntero(v2);
                int c3 = b1 + b2;
                resp = new Valor(Constantes.ENTERO, c3);
                return resp;

            }else if(esEntero(v1) && esDecimal(v2)){
                int b1 = getEntero(v1);
                double b2 = getDecimal(v2);
                double c3 = b1 + b2;
                resp = new Valor(Constantes.DECIMAL, c3);
                return resp;

            }else if(esEntero(v1) && esCadena(v2)){
                int b1 = getEntero(v1);
                string b2 = getCadena(v2);
                String c3 = b1 + b2+"";
                resp = new Valor(Constantes.CADENA, c3);
                return resp;
            }
            /*--------------------- Validaciones con Decimales -----------------------------------*/
            else if(esDecimal(v1) && esBool(v2)){
                double d1 = getDecimal(v1);
                int b1 = getBooleanoNumero(v2);
                double d2 = d1 + b1;
                resp = new Valor(Constantes.DECIMAL, d2);
                return resp;

            }
            else if(esDecimal(v1) && esEntero(v2)){
                double d1 = getDecimal(v1);
                int b1 = getEntero(v2);
                double d2 = d1 + b1;
                resp = new Valor(Constantes.DECIMAL, d2);
                return resp;

            }
            else if(esDecimal(v1) && esDecimal(v2)){
                double d1 = getDecimal(v1);
                double b1 = getDecimal(v2);
                double d2 = d1 + b1;
                resp = new Valor(Constantes.DECIMAL, d2);
                return resp;
            }
            else if(esDecimal(v1) && esCadena(v2)){
                double b1 = getDecimal(v1);
                string b2 = getCadena(v2);
                String c3 = b1 + b2 + "";
                resp = new Valor(Constantes.CADENA, c3);
                return resp;
            }

            /*-------------------- Validaciones con cadenas --------------------------------*/
            else if(esCadena(v1) && esBool(v2) ){
                resp = new Valor(Constantes.CADENA, getCadena(v1) + getBooleanoLetra(v2) + "");
                return resp;

            }else if(esCadena(v1) && esEntero(v2)){
                resp = new Valor(Constantes.CADENA, getCadena(v1) + getEntero(v2) + "");
                return resp;

            }else if(esCadena(v1) && esDecimal(v2)){
                resp = new Valor(Constantes.CADENA, getCadena(v1) + getDecimal(v2) + "");
                return resp;

            }else if(esCadena(v1) && esCadena(v2)){
                resp = new Valor(Constantes.CADENA, getCadena(v1) + getCadena(v2) + "");
                return resp;
            }
            else if (esCadena(v1) && esHora(v2))
            {
                resp = new Valor(Constantes.CADENA, getCadena(v1) + getHora(v2) + "");
                return resp;

            }
            else if (esCadena(v1) && esFecha(v2))
            {
                resp = new Valor(Constantes.CADENA, getCadena(v1) + getFecha(v2) + "");
                return resp;

            }
            else if (esCadena(v1) && esFechaHora(v2))
            {
                resp = new Valor(Constantes.CADENA, getCadena(v1) + getFechaHora(v2) + "");
                return resp;

            }

            /*--------------------- Validaciones con Fechas/ horas/ fechas-Hora -----------------------*/

            else if (esFecha(v1) && esCadena(v2))
            {
                resp = new Valor(Constantes.CADENA, getFecha(v1) + getCadena(v2) + "");
                return resp;

            }
            else if (esHora(v1) && esCadena(v2))
            {
                resp = new Valor(Constantes.CADENA, getHora(v1) + getCadena(v2) + "");
                return resp;

            }
            else if (esFechaHora(v1) && esCadena(v2))
            {
                resp = new Valor(Constantes.CADENA, getFechaHora(v1) + getCadena(v2) + "");
                return resp;

            }


            return new Valor(Constantes.NULO, Constantes.NULO);
        }


        #endregion


        #endregion


        #region Extras

        private bool esCero(double valor)
        {
            return (valor == 0);
        }

        private bool esFecha(Valor v)
        {
            return v.tipo.Equals(Constantes.FECHA,StringComparison.CurrentCultureIgnoreCase);
        }
        private bool esFechaHora(Valor v)
        {
            return v.tipo.Equals(Constantes.FECHA_HORA, StringComparison.CurrentCultureIgnoreCase);
        }

        private bool esHora(Valor v)
        {
            return v.tipo.Equals(Constantes.HORA, StringComparison.CurrentCultureIgnoreCase);
        }


        private bool esEntero(Valor v){
            return v.tipo.Equals(Constantes.ENTERO, StringComparison.CurrentCultureIgnoreCase);
        }

        private bool esBool(Valor v)
        {
            return v.tipo.Equals(Constantes.BOOLEANO, StringComparison.CurrentCultureIgnoreCase);
        }

        private bool esDecimal(Valor v)
        {
            return v.tipo.Equals(Constantes.DECIMAL, StringComparison.CurrentCultureIgnoreCase);
        }

        private bool esCadena(Valor v)
        {
            return v.tipo.Equals(Constantes.CADENA, StringComparison.CurrentCultureIgnoreCase);
        }

        private bool esCaracter(Valor v)
        {
            return v.tipo.ToLower().Equals(Constantes.CARACTER.ToLower(), StringComparison.CurrentCultureIgnoreCase);
        }

     

       

        // obtener valores 
        private String getHora(Valor v)
        {
            Hora g = (Hora)v.valor;
            return g.valHoraCadena;
        }

        private String getFecha(Valor v)
        {
            Fecha g = (Fecha)v.valor;
            return g.valFechaCadena;
        }

        private String getFechaHora(Valor v)
        {
            FechaHora g = (FechaHora)v.valor;
            return g.cadenaRealFechaHora;
        }

        private String getCadena(Valor v)
        {
            return v.valor.ToString();
        }

        private int getEntero(Valor v)
        {
            return int.Parse(v.valor.ToString());
        }

        private double getDecimal(Valor v)
        {
            return double.Parse(v.valor.ToString());
        }

        private char getChar(Valor v)
        {
            return char.Parse(v.valor.ToString());
        }

        private int getCharNumero(Valor v)
        {
            int val = (int)Char.GetNumericValue(char.Parse(v.valor.ToString()));
            return val;
        }
        private string getBooleanoLetra(Valor v)
        {
            if (v.valor.ToString().ToLower().Equals(Constantes.VERDADERO.ToLower(), StringComparison.CurrentCultureIgnoreCase))
            {
                return Constantes.VERDADERO;
            }
            else
            {
                return Constantes.FALSO;
            }
        }

        private int getBooleanoNumero(Valor v)
        {
            if (v.valor.ToString().ToLower().Equals(Constantes.VERDADERO.ToLower(), StringComparison.CurrentCultureIgnoreCase))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }



        #endregion




#endregion



    }
}
