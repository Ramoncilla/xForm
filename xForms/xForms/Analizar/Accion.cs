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

       
        public ListaClases claseArchivo;
        public List<Importar> importaciones;
        string variableInstancia = "";
        private bool esAtriAsigna = false;
        private bool esAtriRes = false;
        string cadenaImprimir;
        public string rutaCarpeta;

        public Accion(string ruta)
        {
            this.cadenaImprimir = "";
            this.rutaCarpeta = ruta;
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

        public void ejecutarImportaciones()
        {
            Importar temp;
            for (int i = 0; i < this.importaciones.Count; i++)
            {
                temp = importaciones.ElementAt(i);

            }
        }

        public string ejecutarArchivo()
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
                    contexto.addAmbito(Constantes.PRINCIPAL);
                    tabla.crearNuevoAmbito(Constantes.PRINCIPAL);
                  
                    for (int j = 0; j < principal.cuerpoFuncion.ChildNodes[0].ChildNodes.Count; j++)
                    {
                        sentencia = principal.cuerpoFuncion.ChildNodes[0].ChildNodes[j];
                        evaluarArbol(sentencia, contexto, temp.nombreClase, Constantes.PRINCIPAL, tabla, new elementoRetorno());
                    }
                    contexto.Ambitos.Pop();
                    tabla.mostrarSimbolos();
                    tabla.salirAmbiente();
                    break;

                }  
            }
            return cadenaImprimir;
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
                        ret = resolverAccesoVar(nodo, ambiente, nombreClase, nombreMetodo, tabla, ret);
                        return ret;
                        #endregion
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
                            asignarSimbolo("retorno", nodo, var.val, simb, ambiente, nombreClase, nombreMetodo, tabla);


                            /*
                            if (simb != null)
                            {
                                simb.asignarValor(var.val, nodo.ChildNodes[0]);
                            }*/
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


        private void declaraObjeto(string nombre, string tipo,ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            string rutaAcceso = ambiente.getAmbito();
            Clase claseBuscada = this.claseArchivo.obtenerClase(tipo);
            if (claseBuscada != null)
            {
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
                    valoresRuta = atriTemp.rutaAcc.Split('/');
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

                    atriTemp.rutaAcc = rutaTemp;
                    string ambitoTemporal= ambiente.getAmbito() + "/" + nombre;;
                    atriTemp.ambito = ambitoTemporal;
                    tabla.insertarSimbolo(atriTemp);

                    /*---- buscando nuevamente el simbolo para poderlo asignar a la tabla*/
                    Contexto c = new Contexto();
                    c.llenarAmbitos(ambitoTemporal);
                    if (atriTemp.nodoExpresionValor != null)
                    {
                        elementoRetorno r = new elementoRetorno();
                        Simbolo s = tabla.buscarSimbolo(atriTemp.nombre, c);
                        r = resolverExpresion(atriTemp.nodoExpresionValor, c, nombreClase, nombreMetodo, tabla);
                        asignarSimbolo(atriTemp.nombre, nodo, r.val, s, c, nombreClase, nombreMetodo, tabla);
                    }
                   // tabla.insertarSimbolo(atriTemp);
                }


            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico("No se pudo declarar la variable de nombre " + nombre + ", de tipo " + tipo + ", no existe esa clase");
            }                  
        }

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
                    varNueva.usada = true;
                    tabla.insertarSimbolo(varNueva, nodo);
                    asignarSimbolo(nombre, nodo, v.val, varNueva, ambiente, nombreClase, nombreMetodo, tabla);
                }
                else
                {
                    this.esAtriAsigna = false;

                    declaraObjeto(nombre, tipo, nodo, ambiente, nombreClase, nombreMetodo, tabla);

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
                    Simbolo nuevoObj = tabla.buscarSimbolo(nombre, ambiente);
        
                    asignarSimbolo(nombre, nodo, v.val, nuevoObj, ambiente, nombreClase, nombreMetodo, tabla);
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
                    declaraObjeto(nombre, tipo, nodo, ambiente, nombreClase, nombreMetodo, tabla);
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

            }
            return ret;
        }

        private elementoRetorno resolverAccesoVar(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            int noValores = nodo.ChildNodes.Count;
            ParseTreeNode elementoAcceso;
            string tipoObj = "";
            string nombreElemento = "";
            bool banderaSeguir = true;
            int i=0;
            Simbolo simbActual;
            tipoObj = nombreClase;
            int cont = 0;
            do
            {
                elementoAcceso = nodo.ChildNodes[i];
                if (elementoAcceso.Term.Name.Equals(Constantes.ID, StringComparison.CurrentCultureIgnoreCase))
                {
                    nombreElemento= elementoAcceso.ChildNodes[0].Token.ValueString;
                    simbActual = tabla.buscarSimbolo(nombreElemento, ambiente);
                    if (simbActual != null)
                    {
                        tipoObj = simbActual.tipo;
                        if (esObjecto(tipoObj))
                        {
                            string ambitoObj = simbActual.ambito;
                            VairablesObjeto obj = tabla.obtenerObjetoConAtributos(simbActual.nombre, simbActual.ambito, simbActual.rutaAcc);
                            ret.val = new Valor(tipoObj, obj);
                        }
                        else
                        {
                            ret.val = new Valor(simbActual.tipo, simbActual);
                            //ret.val = simbActual.valor;
                        }
                        ambiente.addAmbito(simbActual.nombre);
                        cont++;
                    }
                    else
                    {
                        banderaSeguir = false;
                        ret.val = new Valor(Constantes.NULO, Constantes.NULO);
                        Constantes.erroresEjecucion.errorSemantico(elementoAcceso, "No existe el elemento " + nombreElemento + ", en el ambito actual");
                    }
                }
                else if (elementoAcceso.Term.Name.Equals(Constantes.LLAMADA, StringComparison.CurrentCultureIgnoreCase))
                {
                    Clase claseTemporal = claseArchivo.obtenerClase(tipoObj);
                    if (claseTemporal != null)
                    {
                        ret = this.llamadaFuncion(elementoAcceso, ambiente, nombreClase, nombreMetodo, tabla, ret);
                    }
                    else
                    {
                        banderaSeguir = false;
                        ret.val = new Valor(Constantes.NULO, Constantes.NULO);
                        Constantes.erroresEjecucion.errorSemantico(elementoAcceso, "No existe el elemento " + tipoObj+ ", en las clases actuales");
                    }
                }
                else
                {

                }
                i++;

            } while (i < noValores && banderaSeguir);
            for (int j = 0; j < cont; j++)
            {
                ambiente.salirAmbito();
            }
            return ret;
        }


        private elementoRetorno asignar2(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            ParseTreeNode nodoAcceso = nodo.ChildNodes[0];
            ParseTreeNode expresion = nodo.ChildNodes[1];
            int cont = 0;
            ParseTreeNode nodoTemporal;
            string nombreElemento;
            Object simboloOVariablesClase;
            do
            {
                nodoTemporal = nodoAcceso.ChildNodes[cont];

                if (nodoTemporal.Term.Name.Equals(Constantes.ID, StringComparison.InvariantCultureIgnoreCase))
                {
                    nombreElemento= nodoTemporal.ChildNodes[0].Token.ValueString;
                    Simbolo simb = tabla.buscarSimbolo(nombreElemento, ambiente);
                    elementoRetorno r = new elementoRetorno();

                    if (simb != null)
                    {
                        if (expresion.Term.Name.Equals(Constantes.INSTANCIA, StringComparison.InvariantCultureIgnoreCase))
                        {
                            ambiente.addAmbito(nombreElemento);
                        }
                        r = resolverExpresion(expresion, ambiente, nombreClase, nombreMetodo, tabla);
                        if (expresion.Term.Name.Equals(Constantes.INSTANCIA, StringComparison.InvariantCultureIgnoreCase))
                        {
                            ambiente.salirAmbito();
                        }

                    }
                    else
                    {

                    }

                    
                    

                }
                else if (nodoTemporal.Term.Name.Equals(Constantes.LLAMADA, StringComparison.InvariantCultureIgnoreCase))
                {

                }
                else
                {// es una posicion de un arreglo

                }
                cont++;
            } while (cont < nodoAcceso.ChildNodes.Count);
            


            return ret;
        }

        private elementoRetorno resolverAsignar2(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
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
                 
                    asignarSimbolo(nombreElemento, nodo,r.val, simb, ambiente, nombreClase, nombreMetodo, tabla);

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
        private elementoRetorno resolverAsignar(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            ParseTreeNode acceso = nodo.ChildNodes[0];
            ParseTreeNode expresion = nodo.ChildNodes[1];
            int cont = acceso.ChildNodes.Count;
            elementoRetorno rr = new elementoRetorno();
            elementoRetorno retAcceso = resolverAccesoVar(acceso, ambiente, nombreClase, nombreMetodo, tabla, ret);

            if ((retAcceso.val.valor is Simbolo) ||(retAcceso.val.valor is VairablesObjeto))
            {
                if (retAcceso.val.valor is Simbolo)
                {
                    Simbolo s = (Simbolo)retAcceso.val.valor;
                    if (expresion.Term.Name.Equals(Constantes.INSTANCIA, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ambiente.addAmbito(s.nombre);
                        rr = resolverExpresion(expresion, ambiente, nombreClase, nombreMetodo, tabla);
                        ambiente.salirAmbito();
                    }
                    else
                    {
                        rr = resolverExpresion(expresion, ambiente, nombreClase, nombreMetodo, tabla);
                    }

                    asignarSimbolo(s.nombre, nodo, rr.val, s, ambiente, nombreClase, nombreMetodo, tabla);


                }
                else if (retAcceso.val.valor is VairablesObjeto)
                {
                    VairablesObjeto s = (VairablesObjeto)retAcceso.val.valor;
                    if (expresion.Term.Name.Equals(Constantes.INSTANCIA, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ambiente.addAmbito(s.simboloObjeto.nombre);
                        rr = resolverExpresion(expresion, ambiente, nombreClase, nombreMetodo, tabla);
                        ambiente.salirAmbito();
                    }
                    else
                    {
                        rr = resolverExpresion(expresion, ambiente, nombreClase, nombreMetodo, tabla);
                    }

                    asignarSimbolo(s.simboloObjeto.nombre, nodo, rr.val, s.simboloObjeto, ambiente, nombreClase, nombreMetodo, tabla);

                }

            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "Valor no valido para realizar una asignacion");

            }

              
            return ret;
        }


        private void asignarSimbolo(string nombreAsignar, ParseTreeNode nodo, Valor v, Simbolo simb, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            if (simb != null)
            {
                if (v.valor is Simbolo)
                {
                    Simbolo s = (Simbolo)v.valor;
                    simb.asignarValor(s.valor, nodo);

                }
                else if (v.valor is VairablesObjeto)
                {
                    VairablesObjeto vars = (VairablesObjeto)v.valor;
                    asignarVariablesObjeto(nodo, simb, vars, ambiente, nombreClase, nombreMetodo, tabla);
                }
                else
                {
                    simb.asignarValor(v, nodo);
                }

            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "No existe la variable " + nombreAsignar + ", en el ambito actual " + ambiente.getAmbito());
                tabla.mostrarSimbolos();
                //
            }

        }


        private void asignarVariablesObjeto(ParseTreeNode nodo, Simbolo simbAsignar, VairablesObjeto objetoRetorno, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            Simbolo simbPapaRetorno = objetoRetorno.simboloObjeto;
            Simbolo varTemp;
            string ambitoNuevo;
            Contexto nuevoAmbiente = new Contexto();
            Simbolo simbBuscado;
            if (simbAsignar.tipo.Equals(simbPapaRetorno.tipo, StringComparison.InvariantCultureIgnoreCase))
            {
                ambitoNuevo = simbAsignar.ambito + "/" + simbAsignar.nombre;
                nuevoAmbiente.llenarAmbitos(ambitoNuevo);
                for (int i = 0; i < objetoRetorno.variablesInstancia.Count; i++)
                {
                    varTemp = objetoRetorno.variablesInstancia.ElementAt(i);
                    simbBuscado = tabla.buscarSimbolo(varTemp.nombre, nuevoAmbiente);
                    if (simbBuscado != null)
                    {
                        simbBuscado.asignarValor(varTemp.valor, nodo);
                    }
                }
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "Tipo no valido para asignar a " + simbAsignar.nombre + " que es de tipo " + simbAsignar.tipo + " con " + simbPapaRetorno.tipo);

            }


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




      

        /*------------------------------------ Impresion -------------------------------------------------------*/
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
                    cadenaImprimir += (f.valFechaCadena)+"\n";
                }
                else if (v.valor is Hora)
                {
                    Hora f = (Hora)v.valor;
                    cadenaImprimir += (f.valHoraCadena) + "\n";
                }
                else if (v.valor is FechaHora)
                {
                    FechaHora f = (FechaHora)v.valor;
                    cadenaImprimir += (f.cadenaRealFechaHora) + "\n";
                }
                else
                {
                    cadenaImprimir += (v.valor) + "\n";

                }
                
            }
            return ret;

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

                        elementoRetorno r = new elementoRetorno();
                        Valor valTemporal;
                        valTemporal = this.resolverAccesoVar(nodo, ambiente, nombreClase, nombreMetodo, tabla, r).val;

                        if (valTemporal.valor is Simbolo)
                        {
                            Simbolo s = (Simbolo) valTemporal.valor;
                            r.val = s.valor;
                        }
                        else 
                        {
                            r.val = valTemporal;
                        }

                        return r;
                        #endregion
                    }

                case Constantes.INSTANCIA:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret = this.resolverInstancia(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return  ret;
                    }

           
                #region Funciones Nativas

                case Constantes.FUN_CADENA:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val= this.funcionCadena(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_SUB_CAD:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionSubCad(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_POS:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionPosCad(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_BOOL:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionBooleano(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_ENTERO:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionEntero(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_TAM:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val =this.funcionTam(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_RANDOM:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionRandom(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_MIN:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionMin(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_MAX:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionMax(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_POW:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionPow(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_LOG:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionLog(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_LOG10:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionLog10(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_ABS:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionAbs(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_SIN:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionSin(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_COS:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionCos(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_TAN:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionTan(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_SQRT:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionSqrt(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_PI:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionPi(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_HOY:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionHoy(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_AHORA:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionAhora(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_FECHA:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionFecha(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_HORA:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionHora(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }

                case Constantes.FUN_FECHA_HORA:
                    {
                        elementoRetorno ret = new elementoRetorno();
                        ret.val = this.funcionFechaHora(nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        return ret;
                    }
                #endregion


                case Constantes.NEGATIVO:
                    {
                        #region numero negativo
                        Valor v = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
                        elementoRetorno r = new elementoRetorno();
                        if (esDecimal(v))
                        {
                            double d = getDecimal(v);
                            d = d * -1;
                            r.val = new Valor(Constantes.DECIMAL, d);
                        }
                        else if (esEntero(v))
                        {
                            int d = getEntero(v);
                            d = d * -1;
                            r.val = new Valor(Constantes.ENTERO, d);
                        }
                        else
                        {
                            Constantes.erroresEjecucion.errorSemantico(nodo, "Tipo no valido para un negativo, " + v.tipo + ", debe de ser tipo numerico");
                        }
                        return r;
                        #endregion
                    }

                case Constantes.NOT2:
                    {
                        #region not
                        Valor v = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
                        elementoRetorno r = new elementoRetorno();
                        if (esBool(v))
                        {
                            if (esVerdadero(v))
                            {
                                r.val = new Valor(Constantes.BOOLEANO, "falso");
                            }
                            else
                            {
                                r.val = new Valor(Constantes.BOOLEANO, "verdadero");
                            }
                        }
                        else
                        {
                            Constantes.erroresEjecucion.errorSemantico(nodo, "Tipo no valido para un operacion NOT, " + v.tipo + " y debe de ser booleano");
                        }
                        return r;
                        #endregion

                    }

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


        private void declararAsignarParametrosLlamada(List<Valor> valoresParametros,ParseTreeNode nodo, ParseTreeNode nodoParametrosDecla, ParseTreeNode nodoParametros, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            ParseTreeNode paramTemp;
            elementoRetorno ret = new elementoRetorno();
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
                    asignarSimbolo(nomParTemp, nodoParametros.ChildNodes[0].ChildNodes[k], temporalVal, simbTemp, ambiente, nombreClase, nombreMetodo, tabla);
                }
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "No se puede realizar la llamada, no encajan los numero de parametros ");
            }

        }


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
                        ParseTreeNode nodoParametrosDecla = funBuscada.obtenerNodoParametros();
                        declararAsignarParametrosLlamada(valoresParametros,nodo, nodoParametrosDecla,nodoParametros, ambiente, nombreClase, nombreMetodo, tabla);
                        ret = evaluarArbol(funBuscada.cuerpoFuncion, ambiente, tipoInstancia, tipoInstancia, tabla, ret);
                        tabla.salirAmbiente();
                        ambiente.salirAmbito();
                        ret.val = new Valor(tipoInstancia, true);
                    }

                }
            }
            
            return ret;
        }


        /*-------------------------------------- Llamada a Funciones -----------------------------------------------*/
        #region llamadaFuncion

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

                ParseTreeNode nodoParametrosDecla = funBuscada.obtenerNodoParametros();
                declararAsignarParametrosLlamada(valoresParametros, nodo, nodoParametrosDecla, nodoParametros, ambiente, nombreClase, nombreMetodo, tabla);

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
            }
            return ret;
        }


        #endregion
        /*--------------------------------- Fin de llamada --------------------------------------------------------*/


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



        #region FUNCIONES NATIVAS

        private void ErrorFuncionNativa(String numeroParametro, String funcion, String tipoV, string tipoF, ParseTreeNode nodo)
        {
            Constantes.erroresEjecucion.errorSemantico(nodo, "El " + numeroParametro + " debe de ser de tipo " + tipoV + ", y es de tipo " + tipoF + ", Error en funcion " + funcion);
        }


        private bool hayParametrosNulos(List<Valor> valores)
        {
            bool band=true;
            Valor temp;
            for (int i = 0; i < valores.Count; i++)
            {
                temp = valores.ElementAt(i);
                band = band && (!(temp.tipo.Equals(Constantes.NULO, StringComparison.CurrentCultureIgnoreCase)));
            }
            return band;
        }


        private Valor funcionCadena(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla){
            // A_CADENA:= cadena + abrePAr+ EXPRESION + cierraPar
            Valor v1 = new Valor(Constantes.CADENA, "");
            Valor v2 = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
            return  sumar(v1, v2);

        }

        private  Valor funcionSubCad(ParseTreeNode nodo,  Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla){

            Valor v1 = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
            Valor v2 = resolverExpresion(nodo.ChildNodes[2], ambiente, nombreClase, nombreMetodo, tabla).val;
            Valor v3 = resolverExpresion(nodo.ChildNodes[3], ambiente, nombreClase, nombreMetodo, tabla).val;
            Valor ret = new Valor(Constantes.NULO, Constantes.NULO);
            if (v1.tipo.Equals(Constantes.CADENA, StringComparison.CurrentCultureIgnoreCase))
            {
                if (v2.tipo.Equals(Constantes.ENTERO, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (v3.tipo.Equals(Constantes.ENTERO, StringComparison.CurrentCultureIgnoreCase))
                    {
                        int inicio = int.Parse(v2.valor.ToString());
                        int fin = int.Parse(v3.valor.ToString()) - 1;
                        Valor resp = new Valor(Constantes.NULO, Constantes.NULO);
                        string cadena = v1.valor.ToString();
                        char c;
                        string res = "";
                        if (inicio < fin)
                        {
                            for (int i = 0; i < cadena.Count(); i++)
                            {
                                c = cadena.ElementAt(i);
                                if (i >= inicio && i < fin)
                                {
                                    res += c + "";
                                }
                            }
                            Valor V = new Valor(Constantes.CADENA, res);
                            return V;
                        }
                        else
                        {
                            Constantes.erroresEjecucion.errorSemantico(nodo.ChildNodes[1], "El indice inicio debe de ser menor al indice final, Error en Subcad");
                            return ret;
                        }
                     
                    }
                    else
                    {
                        Constantes.erroresEjecucion.errorSemantico(nodo.ChildNodes[3], "Tercer parametro de la funcion SubCad, debe de ser entero y es de tipo " + v3.tipo);
                        return ret;
                    }
                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico(nodo.ChildNodes[2], "Segundo parametro de la funcion SubCad, debe de ser entero y es de tipo " + v2.tipo);
                    return ret;
                }
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo.ChildNodes[1], "Primer parametro de la funcion SubCad, debe de ser cadena y es de tipo " + v1.tipo);
                return ret;
            }


    }


        private Valor funcionPosCad(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            Valor v1 = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
            Valor v2 = resolverExpresion(nodo.ChildNodes[2], ambiente, nombreClase, nombreMetodo, tabla).val;
            Valor ret = new Valor(Constantes.NULO, Constantes.NULO);
            if (esCadena(v1))
            {
                if (esEntero(v2))
                {
                    string cad = v1.valor.ToString();
                    int sizeCad = cad.Count();
                    int pos = int.Parse(v2.valor.ToString());
                    if (pos < (sizeCad - 1))
                    {
                        Valor v = new Valor(Constantes.CADENA, cad.ElementAt(pos));
                        return v;
                    }
                    else
                    {
                        Constantes.erroresEjecucion.errorSemantico(nodo.ChildNodes[2], "Posicion invalida para recuperar, es mayor a la cadena, en funcion PosCad ");
                    }

                }
                else
                {
                    ErrorFuncionNativa("segundo", "PosCad", "entero", v2.tipo, nodo.ChildNodes[2]);
                }
            }
            else
            {
                ErrorFuncionNativa("primer", "PosCad", "cadena", v1.tipo,nodo.ChildNodes[1]);
            }

            return ret;
        }



        private Valor funcionBooleano(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            Valor v1 = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
            Valor ret = new Valor();
            if(esEntero(v1) || esDecimal(v1))
            {
                double d = double.Parse(v1.valor.ToString());
                if (d > 0)
                {
                    ret = new Valor(Constantes.BOOLEANO, Constantes.VERDADERO);
                }
                else
                {
                    ret = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
                }
            }
            else if (esCadena(v1))
            {
                string cad = v1.valor.ToString();
                if (cad.Count() > 0)
                {
                    ret = new Valor(Constantes.BOOLEANO, Constantes.VERDADERO);
                }
                else
                {
                   ret = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
                }
            }
            else if (esObjecto(v1.tipo))
            {
                ret = new Valor(Constantes.BOOLEANO, Constantes.VERDADERO);
            }
            else
            {
                ErrorFuncionNativa("primer", "Booleano", v1.tipo, " numerico, cadena u objeto", nodo.ChildNodes[1]);
            }
           

            return ret;
        }



        private int obtenerSumaAsciiCadena(String cadena)
        {
   
            char c;
            int val = 0;
            for (int i = 0; i < cadena.Count(); i++)
            {
                c = Convert.ToChar(cadena.ElementAt(i));
                val=+ Convert.ToInt32(c);
            }
            return val;
        }


        private Valor funcionEntero(ParseTreeNode nodo,  Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            Valor v1 = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
            Valor ret = new Valor();
            if (esCadena(v1))
            {
                string cadena = v1.valor.ToString();
                int acum =obtenerSumaAsciiCadena(cadena);
                ret = new Valor(Constantes.ENTERO, acum);
            }
            else if (esEntero(v1))
            {
                ret = new Valor(Constantes.ENTERO, v1.valor);
            }
            else if (esDecimal(v1))
            {
                double d = double.Parse(v1.valor.ToString());
                d = Math.Round(d);
                ret = new Valor(Constantes.ENTERO, d);
            }
            else if (esBool(v1))
            {
                int v = getBooleanoNumero(v1);
                ret = new Valor(Constantes.ENTERO, v);
            }
            else if (esFecha(v1))
            {
                Fecha f = (Fecha)v1.valor;
                DateTime fechaR = f.fechaReal;
                

            }
            else if (esHora(v1))
            {

            }
            else if (esFechaHora(v1))
            {

            }
            else
            {
                ret = new Valor();
            }
            return ret;
        }
        private Valor funcionTam(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            Valor ret = new Valor();

            return ret;
        }

        private Valor funcionRandom(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            //random parametrosllamada
            Valor ret = new Valor();
            List<Valor> valoresRandom = resolviendoParametros(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla);
            if (valoresRandom.Count > 0)
            {
                if (hayParametrosNulos(valoresRandom))
                {
                    int n = valoresRandom.Count - 1;
                    Random r = new Random();
                    int indiceR = r.Next(0, n);
                    ret = new Valor(Constantes.ENTERO, valoresRandom.ElementAt(indiceR).valor);
                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico(nodo, "Error al resolver expresion para funcion random");
                    ret = new Valor();
                }

            }
            else
            {
                Random r = new Random();
                double d = Math.Round(r.NextDouble(),5);
                ret = new Valor(Constantes.DECIMAL, d);
            }
            return ret;
        }


        private Valor conseguirValorEnteroDecima(Valor v)
        {
            Valor ret = new Valor(Constantes.ENTERO, 0);
            if (esEntero(v))
            {
                ret = new Valor(Constantes.ENTERO, getEntero(v));
            }
            else if (esDecimal(v))
            {
                ret = new Valor(Constantes.ENTERO, getDecimal(v));
            }
            else if (esBool(v))
            {
                ret = new Valor(Constantes.ENTERO, getBooleanoNumero(v));
            }
            else if (esCadena(v))
            {
                ret = new Valor(Constantes.ENTERO, obtenerSumaAsciiCadena(v.valor.ToString()));
            }
            else
            {
                //por si retorna objetos
            }
            return ret;
        }
        private Valor funcionMin(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            List<Valor> valores = resolviendoParametros(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla);
            if (hayParametrosNulos(valores))
            {

                if (valores.Count > 0)
                {
                    Valor temp;
                    Valor minimo = conseguirValorEnteroDecima(valores.ElementAt(0));
                    int e1, e2;
                    double d1, d2;
                    for (int i = 1; i < valores.Count; i++)
                    {
                        temp = conseguirValorEnteroDecima(valores.ElementAt(i));
                        if (esEntero(minimo) && esEntero(temp))
                        {
                            e1 = getEntero(minimo);
                            e2 = getEntero(temp);
                            if (e2 <= e1)
                            {
                                minimo = temp;
                            }
                        }
                        else if (esDecimal(minimo) && esDecimal(temp))
                        {
                            d1 = getDecimal(minimo);
                            d2 = getDecimal(temp);
                            if (d2 <= d1)
                            {
                                minimo = temp;
                            }

                        }
                        else if (esDecimal(minimo) && esEntero(temp))
                        {
                            d1 = getDecimal(minimo);
                            e2 = getEntero(temp);
                            if (e2 <= d1)
                            {
                                minimo = temp;
                            }

                        }
                        else if (esEntero(minimo) && esDecimal(temp))
                        {
                            e1 = getEntero(minimo);
                            d2 = getDecimal(temp);
                            if (d2 <= e1)
                            {
                                minimo = temp;
                            }

                        }

                        
                    }

                    return  minimo;
                }
                else
                {
                   return new Valor(Constantes.ENTERO, 0);
                }
            }
            else
            {
                return new Valor(Constantes.ENTERO, 0);
            }
        }


        private Valor funcionMax(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {

             List<Valor> valores = resolviendoParametros(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla);
            if (hayParametrosNulos(valores))
            {

                if (valores.Count > 0)
                {
                    Valor temp;
                    Valor minimo = conseguirValorEnteroDecima(valores.ElementAt(0));
                    int e1, e2;
                    double d1, d2;
                    for (int i = 1; i < valores.Count; i++)
                    {
                        temp = conseguirValorEnteroDecima(valores.ElementAt(i));
                        if (esEntero(minimo) && esEntero(temp))
                        {
                            e1 = getEntero(minimo);
                            e2 = getEntero(temp);
                            if (e2 >= e1)
                            {
                                minimo = temp;
                            }
                        }
                        else if (esDecimal(minimo) && esDecimal(temp))
                        {
                            d1 = getDecimal(minimo);
                            d2 = getDecimal(temp);
                            if (d2 >= d1)
                            {
                                minimo = temp;
                            }

                        }
                        else if (esDecimal(minimo) && esEntero(temp))
                        {
                            d1 = getDecimal(minimo);
                            e2 = getEntero(temp);
                            if (e2 >= d1)
                            {
                                minimo = temp;
                            }

                        }
                        else if (esEntero(minimo) && esDecimal(temp))
                        {
                            e1 = getEntero(minimo);
                            d2 = getDecimal(temp);
                            if (d2 >= e1)
                            {
                                minimo = temp;
                            }

                        }

                        
                    }

                    return minimo;
                }
                else
                {
                   return new Valor(Constantes.ENTERO, 0);
                }
            }
            else
            {
                return  new Valor(Constantes.ENTERO, 0);
            }
        }

        private Valor funcionPow(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            Valor v1 = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
            Valor v2 = resolverExpresion(nodo.ChildNodes[2], ambiente, nombreClase, nombreMetodo, tabla).val;

            Valor ret = new Valor();
            if((esEntero(v1) || esDecimal(v1)) &&
               (esEntero(v2) || esDecimal(v2)))
            {
                double n1= double.Parse(v1.valor.ToString());
                double n2 = double.Parse(v2.valor.ToString());
                double d = Math.Round(Math.Pow(n1, n2), 5);
                ret = new Valor(Constantes.DECIMAL, d);
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "Los parametros para la funcion POW deben de ser numericos y son " + v1.tipo + " y " + v2.tipo);
            }

            return ret;
        }

        private Valor funcionLog(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            Valor v1 = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
            Valor ret = new Valor();

            if (esEntero(v1) || esDecimal(v1))
            {
                double d = double.Parse(v1.valor.ToString());
                if (d>=0)
                {
                    double b = Math.Round(Math.Log(d), 5);
                    ret = new Valor(Constantes.DECIMAL, b);
                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico(nodo.ChildNodes[1], "El parametro para la funcion Log debe de ser positivo");
                }
            }

            return ret;
        }


        private Valor funcionLog10(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            Valor v1 = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
            Valor ret = new Valor();

            if (esEntero(v1) || esDecimal(v1))
            {
                double d = double.Parse(v1.valor.ToString());
                if (d >= 0)
                {
                    double b = Math.Round(Math.Log10(d), 5);
                    ret = new Valor(Constantes.DECIMAL, b);
                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico(nodo.ChildNodes[1], "El parametro para la funcion Log debe de ser positivo");
                }
            }

            return ret;
        }


        private Valor funcionAbs(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            Valor v1 = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
            Valor ret = new Valor();

            if (esEntero(v1))
            {
                int d = int.Parse(v1.valor.ToString());
                int b = Math.Abs(d);
                ret = new Valor(Constantes.ENTERO, b);
            }
            else if (esDecimal(v1))
            {
                double d = double.Parse(v1.valor.ToString());
                double b = Math.Abs(d);
                ret = new Valor(Constantes.DECIMAL, b);
            }
            return ret;
        }


        private Valor funcionSin(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            Valor v1 = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
            Valor ret = new Valor();

            if (esEntero(v1) || esDecimal(v1))
            {
                double d = double.Parse(v1.valor.ToString());
                double b = Math.Round(Math.Sin(d), 5);
                ret = new Valor(Constantes.DECIMAL, b);
            }

            return ret;
        }


        private Valor funcionCos(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            Valor v1 = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
            Valor ret = new Valor();

            if (esEntero(v1) || esDecimal(v1))
            {
                double d = double.Parse(v1.valor.ToString());
                double b = Math.Round(Math.Cos(d), 5);
                ret = new Valor(Constantes.DECIMAL, b);
            }

            return ret;
        }


        private Valor funcionTan(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            Valor v1 = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
            Valor ret = new Valor();

            if (esEntero(v1) || esDecimal(v1))
            {
                double d = double.Parse(v1.valor.ToString());
                double b = Math.Round(Math.Tan(d), 5);
                ret = new Valor(Constantes.DECIMAL, b);
            }

            return ret;
        }


        private Valor funcionSqrt(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            Valor v1 = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
            Valor ret = new Valor();

            if (esEntero(v1) || esDecimal(v1))
            {
                double d = double.Parse(v1.valor.ToString());
                if (d >= 0)
                {
                    double b = Math.Round(Math.Sqrt(d), 5);
                    ret = new Valor(Constantes.DECIMAL, b);
                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico(nodo.ChildNodes[1], "El parametro para una raiz cuadrada debe ser positivo");
                }
            }

            return ret;
        }


        private Valor funcionPi(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            Valor ret = new Valor(Constantes.DECIMAL, Math.Round(Math.PI,5));
            return ret;
        }

        private Valor funcionHoy(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            DateTime f = DateTime.Today;
            Fecha nueva = new Fecha(nodo,f.ToString("dd/MM/yyyy"));
            Valor ret= nueva.validarFecha();
            return ret;
        }


        private Valor funcionAhora(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            DateTime f = DateTime.Now;
            FechaHora nueva = new FechaHora(nodo, f.ToString("dd/MM/yyyy HH:mm:ss"));
            Valor ret = nueva.validarFechaHora();
            return ret;
        }


        private Valor funcionFecha(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {

            Valor ret = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
            if (!esFecha(ret))
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "La tipo no valido para convertir en fecha " + ret.tipo);
                ret = new Valor(Constantes.NULO, Constantes.NULO); 
            }
            
            return ret;
        }


        private Valor funcionHora(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            Valor ret = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
            if (!esHora(ret))
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "La tipo no valido para convertir en hora " + ret.tipo);
                ret = new Valor(Constantes.NULO, Constantes.NULO);
            }

            return ret;
        }


        private Valor funcionFechaHora(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            Valor ret = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
            if (!esFechaHora(ret))
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "La tipo no valido para convertir en fechahora " + ret.tipo);
                ret = new Valor(Constantes.NULO, Constantes.NULO);
            }

            return ret;
        }


        private elementoRetorno funcionImagen(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {

            return ret;
        }


        private elementoRetorno funcionVideo(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {

            return ret;
        }


        private elementoRetorno funcionAudio(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {

            return ret;
        }




        #endregion


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




#endregion



    }
}
