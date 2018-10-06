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
using System.IO;
using xForms.Formularios;
using System.Windows.Forms;

namespace xForms.Analizar
{
    class Accion
    {

        public string cadenaRespuestas;
        public ListaClases claseArchivo;
        public List<Importar> importaciones;
        string variableInstancia = "";
        private bool esAtriAsigna = false;
        private bool esAtriRes = false;
        string cadenaImprimir;
        public string rutaCarpeta;
        public string nombreArchivoPrincipal;
        tablaSimbolos temporalParametros;
        int contadorF = 0;
        String rutaReportes = @"‪C:\Reportes\";
        int contP = 0;
        string cadenaRespuestasHtml="<!DOCTYPE html>\n" +
"<html>\n" +
"<head>\n" +
"<style>\n" +
"#customers {\n" +
"    font-family: \"Trebuchet MS\", Arial, Helvetica, sans-serif;\n" +
"    border-collapse: collapse;\n" +
"    width: 100%;\n" +
"}\n" +
"\n" +
"#customers td, #customers th {\n" +
"    border: 1px solid #ddd;\n" +
"    padding: 8px;\n" +
"}\n" +
"\n" +
"#customers tr:nth-child(even){background-color: #f2f2f2;}\n" +
"\n" +
"#customers tr:hover {background-color: #ddd;}\n" +
"\n" +
"#customers th {\n" +
"    padding-top: 12px;\n" +
"    padding-bottom: 12px;\n" +
"    text-align: left;\n" +
"    background-color: #4CAF50;\n" +
"    color: white;\n" +
"}\n" +
"</style>\n" +
"</head>\n" +
"<body><br><br><center> Respuesta Formulario</center>\n" +
"\n" +
"<table id=\"customers\">"+
            "<tr>\n" +
                "<td>No.</td>\n" +
                "<td>Id Pregunta</td>\n" +
                "<td>Respuesta</td>\n" +
                "</tr>\n";

        public Accion(string ruta, string nombreA)
        {
            this.cadenaRespuestas = "";
            this.cadenaImprimir = "";
            nombreArchivoPrincipal = nombreA;
            this.rutaCarpeta = ruta;
            this.claseArchivo = new ListaClases(nombreA,ruta);
            this.importaciones = new List<Importar>();
            this.temporalParametros = new tablaSimbolos();
        }

        #region Inicio Archivo
        public void generarArchivosClase(ParseTreeNode nodoRaiz)
        {
            ListaArchivos archivos = new ListaArchivos(rutaCarpeta);
            archivos.insertarArchivo(nombreArchivoPrincipal, nodoRaiz);
            this.claseArchivo = archivos.crearLista();
            this.claseArchivo.agregarPreguntas();
            //claseArchivo.llenarListaFormularios();
        }


        public void instanciarAtributosClase(){
            this.claseArchivo.iniciarAtributos();
        }

        private void instanciarPreguntas(Contexto ambiente, tablaSimbolos tabla, string nombreClase, string nombreMetodo)
        {
            List<Clase> preguntas = claseArchivo.obtenerPreguntas();

            Clase temp;
            for (int i = 0; i < preguntas.Count; i++)
            {
                temp = preguntas.ElementAt(i);
                try
                {
                    declaraObjeto(temp.nombreClase, temp.nombreClase, null, ambiente, nombreClase, nombreMetodo, tabla);
                }
                catch (Exception e)
                {
                    Constantes.erroresEjecucion.errorSemantico("No se ha podido instanciar la pregunta " + temp.nombreClase);
                }
                
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
                    instanciarPreguntas(contexto, tabla, temp.nombreClase, Constantes.PRINCIPAL);
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

        #endregion

        #region evualuar sentencias
        public elementoRetorno evaluarArbol(ParseTreeNode nodo, Contexto ambiente, String nombreClase, String nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            switch (nodo.Term.Name)
            {


                case Constantes.GUARDARINFO:
                    {

                        string message = "Deseas guardar el formulario?";
                        string title = "Guardar";
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result = MessageBox.Show(message, title, buttons);
                        if (result == DialogResult.Yes)
                        {
                            String arbolDerivacion = rutaCarpeta + "\\Formulario" + contadorF + ".txt";
                            System.IO.File.WriteAllText(arbolDerivacion, cadenaRespuestas);
                        }
                        else
                        {
                            // Do something  
                        }
                      
                        return ret;
                    }

                case Constantes.FUN_MENSAJE:
                    {
                        ParseTreeNode nodoExpresionMensaje = nodo.ChildNodes[1];
                        Valor v = resolverExpresion(nodoExpresionMensaje, ambiente, nombreClase, nombreMetodo, tabla).val;
                        String msj = v.valor.ToString();
                        MessageBox.Show( msj, "Mensaje");
                        return ret;
                    }

                #region Formularios
                case Constantes.NUEVO_FORM:
                    {
                        ret = this.llamadaFuncion(nodo.ChildNodes[0].ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla, ret);
                        return ret;
                    }

                case Constantes.LLAMADA_PREGUNTA:
                    {

                        ret = this.resolverLlamadaPregunta(nodo, ambiente, nombreClase, nombreMetodo, tabla, ret);

                        return ret;
                    }
                #endregion




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
                            temporalParametros = tabla;
                            elementoRetorno var = resolverExpresion(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
                            Simbolo simb = tabla.buscarSimbolo("retorno", ambiente);
                            asignarSimbolo("retorno", nodo, var.val, simb, ambiente, nombreClase, nombreMetodo, tabla);
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



        #region llamadaPregunta

        private void ejecutarNota(Objeto simb, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla){
            string id = simb.nombre;
            string valNota = "";
            Simbolo etiqueta = simb.variablesObjeto.obtenerPregunta("etiqueta", Constantes.CADENA);
            if (etiqueta != null)
            {
                if (etiqueta.nodoExpresionValor != null)
                {
                    Valor v = resolverExpresion(etiqueta.nodoExpresionValor, ambiente, nombreClase, nombreMetodo, tabla).val;
                    if (esCadena(v) || esNulo(v))
                    {
                        valNota = v.valor.ToString();
                        modeloPregunta nota = new modeloPregunta("", false, id, valNota);
                        DialogResult resultado = new DialogResult();
                        Form2 frmPregunta = new Form2();
                        frmPregunta.Text = " - Nota - ";
                        nota.Visible = true;
                        frmPregunta.setValor(nota);
                        resultado = frmPregunta.ShowDialog();
                        if (resultado == DialogResult.OK)
                        {
                            Valor ingresoUsuario = new Valor(Constantes.CADENA, "Siguiente");
                            responder(id, "Siguiente");
                            Console.WriteLine("Respuesta del usuario   " + Form2.respuestaCadena);
                        }

                    }
                }

            }
            
        }

        private Valor ObtenerValorAtri(Simbolo simb, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            if (simb != null)
            {
                if (simb.nodoExpresionValor != null)
                {
                    return resolverExpresion(simb.nodoExpresionValor, ambiente, nombreClase, nombreMetodo, tabla).val;
                }
            }
            return new Valor();
        }



        private void responder(string idPregunta, Object valor)
        {
            cadenaRespuestas += "< " + idPregunta + " , " + valor.ToString() + ">\n";
            cadenaRespuestasHtml += "<tr>\n" +
                "<td>" + contP + "</td>\n" +
                "<td>" + (idPregunta + 1) + "</td>\n" +
                "<td>" + (valor) + "</td>\n" +
                "</tr>\n";
            contP++;
        }

        private Valor ejecutarCadena( ParseTreeNode nodo, Objeto simb, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla) {
                string esTipo = nodo.ChildNodes[3].ChildNodes[0].Token.ValueString;
                string idPregunta = simb.nombre;
                Simbolo etiquetaPregunta = simb.variablesObjeto.obtenerPregunta("etiqueta", Constantes.CADENA);
                Simbolo respuestas = simb.variablesObjeto.obtenerPregunta("respuestas", esTipo.Replace("es",""));
                Simbolo sugerir = simb.variablesObjeto.obtenerPregunta("sugerir", Constantes.CADENA);
                Simbolo requerido = simb.variablesObjeto.obtenerPregunta("requerido", Constantes.BOOLEANO);
                Simbolo requeridoMsn = simb.variablesObjeto.obtenerPregunta("requeridoMsn", Constantes.CADENA);
                Simbolo lectura = simb.variablesObjeto.obtenerPregunta("lectura", Constantes.BOOLEANO);
                Valor sugerirV = ObtenerValorAtri(sugerir, ambiente, nombreClase, nombreMetodo, tabla);
                Valor requeridoV = ObtenerValorAtri(requerido, ambiente, nombreClase, nombreMetodo, tabla);
                Valor requeridoMsnV = ObtenerValorAtri(requeridoMsn, ambiente, nombreClase, nombreMetodo, tabla);
                Valor lecturaV = ObtenerValorAtri(lectura, ambiente, nombreClase, nombreMetodo, tabla);
                Valor etiqueta = ObtenerValorAtri(etiquetaPregunta, ambiente, nombreClase, nombreMetodo, tabla);
                Valor porDefecto = ObtenerValorAtri(respuestas, ambiente, nombreClase, nombreMetodo, tabla);
                string sugerirC = "";
                string msgRequeridoC = "";
                bool requeridoC = false;
                bool lecturaC = false;
                string cadenaC = "";
                string defecto = "";
                if (esCadena(sugerirV))
                {
                    sugerirC = sugerirV.valor.ToString();
                }
                if (esCadena(requeridoMsnV))
                {
                    msgRequeridoC = requeridoMsnV.valor.ToString();
                }
                if (esBooleano(requeridoV))
                {
                    requeridoC = (getBooleanoNumero(requeridoV) == 1);
                }
                if (esBooleano(lecturaV))
                {
                    lecturaC = (getBooleanoNumero(lecturaV) == 1);
                }
                if (esCadena(etiqueta))
                {
                    cadenaC = etiqueta.valor.ToString();

                }

                if (esCadena(porDefecto))
                {
                    defecto = porDefecto.valor.ToString();
                }

                //Resolviendo los valores de los parametros
                ParseTreeNode exp1 = nodo.ChildNodes[5];
                ParseTreeNode exp2 = nodo.ChildNodes[6];
                ParseTreeNode exp3 = nodo.ChildNodes[7];
                Valor max, min, fil;
                max = new Valor(Constantes.ENTERO, -1);
                min = new Valor(Constantes.ENTERO, -1);
                fil = new Valor(Constantes.ENTERO, -1);
                if (!esNada(exp1))
                {
                    min = resolverExpresion(exp1, ambiente, nombreClase, nombreMetodo, tabla).val;
                }
                if (!esNada(exp2))
                {
                    max = resolverExpresion(exp2, ambiente, nombreClase, nombreMetodo, tabla).val;
                }
                if (!esNada(exp3))
                {
                    fil = resolverExpresion(exp3, ambiente, nombreClase, nombreMetodo, tabla).val;
                }
                if (!esEntero(min))
                {
                    min = new Valor(Constantes.ENTERO, -1);
                }
                if (!esEntero(max))
                {
                    max = new Valor(Constantes.ENTERO, -1);
                }
                if (!esEntero(fil))
                {
                    fil = new Valor(Constantes.ENTERO, -1);
                }
                //public modeloPregunta(string sugerir, bool lectura, string idP, int max, int min, int fil, string val)
                modeloPregunta preguntaCadena = new modeloPregunta(sugerirC, lecturaC, idPregunta, getEntero(max), getEntero(min), getEntero(fil), defecto, cadenaC);
                preguntaCadena.requerido = requeridoC;
                preguntaCadena.msgRequerido = msgRequeridoC;
                DialogResult resultado = new DialogResult();
                Form2 frmPregunta = new Form2();
                preguntaCadena.Visible = true;
                frmPregunta.setValor(preguntaCadena);
                resultado = frmPregunta.ShowDialog();
                if (resultado == DialogResult.OK)
                {
                    Valor ingresoUsuario = castear(esTipo, Form2.respuestaCadena);

                    String result = Form2.respuestaCadena;
                    responder(idPregunta, Form2.respuestaCadena);
                    Console.WriteLine("Respuesta del usuario   " + Form2.respuestaCadena);
                    tabla.insertarSimbolo(respuestas,1);
                    respondeUsuario(tabla, ingresoUsuario, Constantes.RESPUESTA, idPregunta, ambiente);
                    tabla.sacarSimbolo();
                    return ingresoUsuario;
                }
            return new Valor();
        }


        #region casteos y esNada
        private Valor castear(string tipo, Object valor)
        {
            if (tipo.Equals("esFecha", StringComparison.CurrentCultureIgnoreCase))
            {
                Fecha f = new Fecha(null, valor.ToString());
                return f.validarFecha();
            }
            if (tipo.Equals("esCadena", StringComparison.CurrentCultureIgnoreCase))
            {
                return new Valor(Constantes.CADENA, valor.ToString());
            }
            if (tipo.Equals("esEntero", StringComparison.CurrentCultureIgnoreCase))
            {
                try
                {
                    int v = int.Parse(valor.ToString());
                    return new Valor(Constantes.ENTERO, v);
                }
                catch (Exception e)
                {
                    Constantes.erroresEjecucion.errorSemantico("La respuesta ingresada por el usuario no se pudo castear a entero  "+ valor);
                }
            }

            if (tipo.Equals("esDecimal", StringComparison.CurrentCultureIgnoreCase))
            {
                try
                {
                    double v = double.Parse(valor.ToString());
                    return new Valor(Constantes.DECIMAL, v);
                }
                catch (Exception e)
                {
                    Constantes.erroresEjecucion.errorSemantico("La respuesta ingresada por el usuario no se pudo castear a decimal  " + valor);
                }

            }

            if (tipo.Equals("esBooleano", StringComparison.CurrentCultureIgnoreCase))
            {
                if (valor.ToString().Equals(Constantes.VERDADERO, StringComparison.CurrentCultureIgnoreCase))
                {
                    return new Valor(Constantes.BOOLEANO, Constantes.VERDADERO);
                }
                return new Valor(Constantes.BOOLEANO, Constantes.FALSO);

            }

            if (tipo.Equals("esHora", StringComparison.CurrentCultureIgnoreCase))
            {
                Hora h = new Hora(null, valor.ToString());
                return h.validarHora();
            }

            if (tipo.Equals("esFechaHora", StringComparison.CurrentCultureIgnoreCase))
            {
                FechaHora f = new FechaHora(null, valor.ToString());
                return f.validarFechaHora();
            }
            return new Valor();
        }


        private bool esNada(ParseTreeNode nodo)
        {
            if (nodo.Term.Name.Equals(Constantes.ACCESO, StringComparison.CurrentCultureIgnoreCase))
            {
                if (nodo.ChildNodes[0].Term.Name.Equals(Constantes.ID, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (nodo.ChildNodes[0].ChildNodes[0].Token.ValueString.Equals("nada", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }
                }

            }
            return false;
        }

        #endregion


        private Valor ejecutarEntero(ParseTreeNode nodo, Objeto simb, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {

            string esTipo = nodo.ChildNodes[3].ChildNodes[0].Token.ValueString;
            string idPregunta = simb.nombre;
            Simbolo etiquetaPregunta = simb.variablesObjeto.obtenerPregunta("etiqueta", Constantes.CADENA);
            Simbolo respuestas = simb.variablesObjeto.obtenerPregunta("respuestas", esTipo.Replace("es", ""));
            Simbolo sugerir = simb.variablesObjeto.obtenerPregunta("sugerir", Constantes.CADENA);
            Simbolo requerido = simb.variablesObjeto.obtenerPregunta("requerido", Constantes.BOOLEANO);
            Simbolo requeridoMsn = simb.variablesObjeto.obtenerPregunta("requeridoMsn", Constantes.CADENA);
            Simbolo lectura = simb.variablesObjeto.obtenerPregunta("lectura", Constantes.BOOLEANO);
            Valor sugerirV = ObtenerValorAtri(sugerir, ambiente, nombreClase, nombreMetodo, tabla);
            Valor requeridoV = ObtenerValorAtri(requerido, ambiente, nombreClase, nombreMetodo, tabla);
            Valor requeridoMsnV = ObtenerValorAtri(requeridoMsn, ambiente, nombreClase, nombreMetodo, tabla);
            Valor lecturaV = ObtenerValorAtri(lectura, ambiente, nombreClase, nombreMetodo, tabla);
            Valor etiqueta = ObtenerValorAtri(etiquetaPregunta, ambiente, nombreClase, nombreMetodo, tabla);
            Valor porDefecto = ObtenerValorAtri(respuestas, ambiente, nombreClase, nombreMetodo, tabla);
            string sugerirC = "";
            string msgRequeridoC = "";
            bool requeridoC = false;
            bool lecturaC = false;
            string cadenaC = "";
            int defecto = 0;
            if (esCadena(sugerirV))
            {
                sugerirC = sugerirV.valor.ToString();
            }
            if (esCadena(requeridoMsnV))
            {
                msgRequeridoC = requeridoMsnV.valor.ToString();
            }
            if (esBooleano(requeridoV))
            {
                requeridoC = (getBooleanoNumero(requeridoV) == 1);
            }
            if (esBooleano(lecturaV))
            {
                lecturaC = (getBooleanoNumero(lecturaV) == 1);
            }
            if (esCadena(etiqueta))
            {
                cadenaC = etiqueta.valor.ToString();

            }

            if (esEntero(porDefecto))
            {
                defecto = int.Parse(porDefecto.valor.ToString());
            }
            //public modeloPregunta(string sugerir, bool lectura, string idP, int val)
            modeloPregunta preguntaCadena = new modeloPregunta(sugerirC, lecturaC, idPregunta, defecto, cadenaC);
            preguntaCadena.requerido = requeridoC;
            preguntaCadena.msgRequerido = msgRequeridoC;
            DialogResult resultado = new DialogResult();
            Form2 frmPregunta = new Form2();
            preguntaCadena.Visible = true;
            frmPregunta.setValor(preguntaCadena);
            resultado = frmPregunta.ShowDialog();
            if (resultado == DialogResult.OK)
            {
                Valor ingresoUsuario = castear(esTipo, Form2.respuestaEntero);
                String result = Form2.respuestaCadena;
                Console.WriteLine("Respuesta del usuario   " + Form2.respuestaEntero);
               // tabla.insertarSimbolo(simb);
                tabla.insertarSimbolo(respuestas,3);
                respondeUsuario(tabla, ingresoUsuario, Constantes.RESPUESTA, idPregunta, ambiente);
               // tabla.sacarSimbolo();
                tabla.sacarSimbolo();
                responder(idPregunta, Form2.respuestaEntero);
                return ingresoUsuario;
            }
            return new Valor();
        }


        private Valor ejecutarDecimal(ParseTreeNode nodo, Objeto simb, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {

            string esTipo = nodo.ChildNodes[3].ChildNodes[0].Token.ValueString;
            string idPregunta = simb.nombre;
            Simbolo etiquetaPregunta = simb.variablesObjeto.obtenerPregunta("etiqueta", Constantes.CADENA);
            Simbolo respuestas = simb.variablesObjeto.obtenerPregunta("respuestas", esTipo.Replace("es", ""));
            Simbolo sugerir = simb.variablesObjeto.obtenerPregunta("sugerir", Constantes.CADENA);
            Simbolo requerido = simb.variablesObjeto.obtenerPregunta("requerido", Constantes.BOOLEANO);
            Simbolo requeridoMsn = simb.variablesObjeto.obtenerPregunta("requeridoMsn", Constantes.CADENA);
            Simbolo lectura = simb.variablesObjeto.obtenerPregunta("lectura", Constantes.BOOLEANO);
            Valor sugerirV = ObtenerValorAtri(sugerir, ambiente, nombreClase, nombreMetodo, tabla);
            Valor requeridoV = ObtenerValorAtri(requerido, ambiente, nombreClase, nombreMetodo, tabla);
            Valor requeridoMsnV = ObtenerValorAtri(requeridoMsn, ambiente, nombreClase, nombreMetodo, tabla);
            Valor lecturaV = ObtenerValorAtri(lectura, ambiente, nombreClase, nombreMetodo, tabla);
            Valor etiqueta = ObtenerValorAtri(etiquetaPregunta, ambiente, nombreClase, nombreMetodo, tabla);
            Valor porDefecto = ObtenerValorAtri(respuestas, ambiente, nombreClase, nombreMetodo, tabla);
            string sugerirC = "";
            string msgRequeridoC = "";
            bool requeridoC = false;
            bool lecturaC = false;
            string cadenaC = "";
            double defecto = 0;
            if (esCadena(sugerirV))
            {
                sugerirC = sugerirV.valor.ToString();
            }
            if (esCadena(requeridoMsnV))
            {
                msgRequeridoC = requeridoMsnV.valor.ToString();
            }
            if (esBooleano(requeridoV))
            {
                requeridoC = (getBooleanoNumero(requeridoV) == 1);
            }
            if (esBooleano(lecturaV))
            {
                lecturaC = (getBooleanoNumero(lecturaV) == 1);
            }
            if (esCadena(etiqueta))
            {
                cadenaC = etiqueta.valor.ToString();

            }

            if (esDecimal(porDefecto))
            {
                defecto = int.Parse(porDefecto.valor.ToString());
            }
            //public modeloPregunta(string sugerir, bool lectura, string idP, double val, string cadenaPr)
            modeloPregunta preguntaCadena = new modeloPregunta(sugerirC, lecturaC, idPregunta, defecto, cadenaC);
            preguntaCadena.requerido = requeridoC;
            preguntaCadena.msgRequerido = msgRequeridoC;
            DialogResult resultado = new DialogResult();
            Form2 frmPregunta = new Form2();
            preguntaCadena.Visible = true;
            frmPregunta.setValor(preguntaCadena);
            resultado = frmPregunta.ShowDialog();
            if (resultado == DialogResult.OK)
            {
                Valor ingresoUsuario = castear(esTipo, Form2.respuestaDecimal);
                Console.WriteLine("Respuesta del usuario   " + Form2.respuestaDecimal);
                tabla.insertarSimbolo(respuestas,4);
                respondeUsuario(tabla, ingresoUsuario, Constantes.RESPUESTA, idPregunta, ambiente);
                tabla.sacarSimbolo();
                responder(idPregunta, Form2.respuestaDecimal);
                return ingresoUsuario;
            }
            return new Valor();
        }

        private Valor ejecutarCondicion(ParseTreeNode nodo, Objeto simb, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            string esTipo = nodo.ChildNodes[3].ChildNodes[0].Token.ValueString;
            string idPregunta = simb.nombre;
            Simbolo etiquetaPregunta = simb.variablesObjeto.obtenerPregunta("etiqueta", Constantes.CADENA);
            Simbolo respuestas = simb.variablesObjeto.obtenerPregunta("respuestas", esTipo.Replace("es", ""));
            Simbolo sugerir = simb.variablesObjeto.obtenerPregunta("sugerir", Constantes.CADENA);
            Simbolo requerido = simb.variablesObjeto.obtenerPregunta("requerido", Constantes.BOOLEANO);
            Simbolo requeridoMsn = simb.variablesObjeto.obtenerPregunta("requeridoMsn", Constantes.CADENA);
            Simbolo lectura = simb.variablesObjeto.obtenerPregunta("lectura", Constantes.BOOLEANO);
            Valor sugerirV = ObtenerValorAtri(sugerir, ambiente, nombreClase, nombreMetodo, tabla);
            Valor requeridoV = ObtenerValorAtri(requerido, ambiente, nombreClase, nombreMetodo, tabla);
            Valor requeridoMsnV = ObtenerValorAtri(requeridoMsn, ambiente, nombreClase, nombreMetodo, tabla);
            Valor lecturaV = ObtenerValorAtri(lectura, ambiente, nombreClase, nombreMetodo, tabla);
            Valor etiqueta = ObtenerValorAtri(etiquetaPregunta, ambiente, nombreClase, nombreMetodo, tabla);
            Valor porDefecto = ObtenerValorAtri(respuestas, ambiente, nombreClase, nombreMetodo, tabla);
            string sugerirC = "";
            string msgRequeridoC = "";
            bool requeridoC = false;
            bool lecturaC = false;
            string cadenaC = "";
            object defecto = null;
            if (esCadena(sugerirV))
            {
                sugerirC = sugerirV.valor.ToString();
            }
            if (esCadena(requeridoMsnV))
            {
                msgRequeridoC = requeridoMsnV.valor.ToString();
            }
            if (esBooleano(requeridoV))
            {
                requeridoC = (getBooleanoNumero(requeridoV) == 1);
            }
            if (esBooleano(lecturaV))
            {
                lecturaC = (getBooleanoNumero(lecturaV) == 1);
            }
            if (esCadena(etiqueta))
            {
                cadenaC = etiqueta.valor.ToString();

            }

            if (esBooleano(porDefecto))
            {
                defecto = getBooleanoLetra(porDefecto);
            }

            //Resolviendo los valores de los parametros
            ParseTreeNode exp1 = nodo.ChildNodes[5];
            ParseTreeNode exp2 = nodo.ChildNodes[6];
            Valor v1 = resolverExpresion(exp1, ambiente, nombreClase, nombreMetodo, tabla).val;
            Valor v2 = resolverExpresion(exp2, ambiente, nombreClase, nombreMetodo, tabla).val;
            string cadT = "Verdadero";
            string cadF = "Falso";
            if (esCadena(v1) && esCadena(v2))
            {
                cadT = v1.valor.ToString();
                cadF = v2.valor.ToString();
            }

            //public modeloPregunta(string sugerir, bool lectura, string idP, string cadt, string cadf, bool val)
            modeloPregunta preguntaCadena = new modeloPregunta(sugerirC, lecturaC, idPregunta, cadT, cadF,defecto,cadenaC);
            preguntaCadena.requerido = requeridoC;
            preguntaCadena.msgRequerido = msgRequeridoC;
            DialogResult resultado = new DialogResult();
            Form2 frmPregunta = new Form2();
            preguntaCadena.Visible = true;
            frmPregunta.setValor(preguntaCadena);
            resultado = frmPregunta.ShowDialog();
            if (resultado == DialogResult.OK)
            {
                Valor ingresoUsuario = castear(esTipo, Form2.respuestaCondicion);
                String result = Form2.respuestaCondicion;
                responder(idPregunta, Form2.respuestaCondicion);
                Console.WriteLine("Respuesta del usuario   " + Form2.respuestaCondicion);
                tabla.insertarSimbolo(respuestas,5);
                respondeUsuario(tabla, ingresoUsuario, Constantes.RESPUESTA, idPregunta, ambiente);
                tabla.sacarSimbolo();
                return ingresoUsuario;
            }
            return new Valor();

        }

        private Valor ejecutarRango(ParseTreeNode nodo, Objeto simb, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {

            string esTipo = nodo.ChildNodes[3].ChildNodes[0].Token.ValueString;
            string idPregunta = simb.nombre;
            Simbolo etiquetaPregunta = simb.variablesObjeto.obtenerPregunta("etiqueta", Constantes.CADENA);
            Simbolo respuestas = simb.variablesObjeto.obtenerPregunta("respuestas", esTipo.Replace("es", ""));
            Simbolo sugerir = simb.variablesObjeto.obtenerPregunta("sugerir", Constantes.CADENA);
            Simbolo requerido = simb.variablesObjeto.obtenerPregunta("requerido", Constantes.BOOLEANO);
            Simbolo requeridoMsn = simb.variablesObjeto.obtenerPregunta("requeridoMsn", Constantes.CADENA);
            Simbolo lectura = simb.variablesObjeto.obtenerPregunta("lectura", Constantes.BOOLEANO);
            Valor sugerirV = ObtenerValorAtri(sugerir, ambiente, nombreClase, nombreMetodo, tabla);
            Valor requeridoV = ObtenerValorAtri(requerido, ambiente, nombreClase, nombreMetodo, tabla);
            Valor requeridoMsnV = ObtenerValorAtri(requeridoMsn, ambiente, nombreClase, nombreMetodo, tabla);
            Valor lecturaV = ObtenerValorAtri(lectura, ambiente, nombreClase, nombreMetodo, tabla);
            Valor etiqueta = ObtenerValorAtri(etiquetaPregunta, ambiente, nombreClase, nombreMetodo, tabla);
            Valor porDefecto = ObtenerValorAtri(respuestas, ambiente, nombreClase, nombreMetodo, tabla);
            string sugerirC = "";
            string msgRequeridoC = "";
            bool requeridoC = false;
            bool lecturaC = false;
            string cadenaC = "";
            int defecto = 0;
            if (esCadena(sugerirV))
            {
                sugerirC = sugerirV.valor.ToString();
            }
            if (esCadena(requeridoMsnV))
            {
                msgRequeridoC = requeridoMsnV.valor.ToString();
            }
            if (esBooleano(requeridoV))
            {
                requeridoC = (getBooleanoNumero(requeridoV) == 1);
            }
            if (esBooleano(lecturaV))
            {
                lecturaC = (getBooleanoNumero(lecturaV) == 1);
            }
            if (esCadena(etiqueta))
            {
                cadenaC = etiqueta.valor.ToString();

            }

            if (esEntero(porDefecto))
            {
                defecto = int.Parse(porDefecto.valor.ToString());
            }

            //Resolviendo los valores de los parametros
            ParseTreeNode exp1 = nodo.ChildNodes[5];
            ParseTreeNode exp2 = nodo.ChildNodes[6];
            Valor max, min;
            max = new Valor(Constantes.ENTERO, 10);
            min = new Valor(Constantes.ENTERO, 0);
            min = resolverExpresion(exp1, ambiente, nombreClase, nombreMetodo, tabla).val;
            max = resolverExpresion(exp2, ambiente, nombreClase, nombreMetodo, tabla).val;
            if (!esEntero(min))
            {
                min = new Valor(Constantes.ENTERO, 0);
            }
            if (!esEntero(max))
            {
                max = new Valor(Constantes.ENTERO, 10);
            }

            // public modeloPregunta(string sugerir, bool lectura, string nombreP, int Linf, int sup, int val, string cadE)
            modeloPregunta preguntaCadena = new modeloPregunta(sugerirC, lecturaC, idPregunta, getEntero(min), getEntero(max), defecto, cadenaC);
            preguntaCadena.requerido = requeridoC;
            preguntaCadena.msgRequerido = msgRequeridoC;
            DialogResult resultado = new DialogResult();
            Form2 frmPregunta = new Form2();
            preguntaCadena.Visible = true;
            frmPregunta.setValor(preguntaCadena);
            resultado = frmPregunta.ShowDialog();
            if (resultado == DialogResult.OK)
            {
                Valor ingresoUsuario = castear(esTipo, Form2.respuestaEntero);
                Console.WriteLine("Respuesta del usuario   " + Form2.respuestaEntero);
                tabla.insertarSimbolo(respuestas,9);
                respondeUsuario(tabla, ingresoUsuario, Constantes.RESPUESTA, idPregunta, ambiente);
                tabla.sacarSimbolo();
                responder(idPregunta, Form2.respuestaEntero);
                return ingresoUsuario;
            }
            return new Valor();
        }

        private Valor ejecutarFecha(ParseTreeNode nodo, Objeto simb, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {

            string esTipo = nodo.ChildNodes[3].ChildNodes[0].Token.ValueString;
            string idPregunta = simb.nombre;
            Simbolo etiquetaPregunta = simb.variablesObjeto.obtenerPregunta("etiqueta", Constantes.CADENA);
            Simbolo respuestas = simb.variablesObjeto.obtenerPregunta("respuestas", esTipo.Replace("es", ""));
            Simbolo sugerir = simb.variablesObjeto.obtenerPregunta("sugerir", Constantes.CADENA);
            Simbolo requerido = simb.variablesObjeto.obtenerPregunta("requerido", Constantes.BOOLEANO);
            Simbolo requeridoMsn = simb.variablesObjeto.obtenerPregunta("requeridoMsn", Constantes.CADENA);
            Simbolo lectura = simb.variablesObjeto.obtenerPregunta("lectura", Constantes.BOOLEANO);
            Valor sugerirV = ObtenerValorAtri(sugerir, ambiente, nombreClase, nombreMetodo, tabla);
            Valor requeridoV = ObtenerValorAtri(requerido, ambiente, nombreClase, nombreMetodo, tabla);
            Valor requeridoMsnV = ObtenerValorAtri(requeridoMsn, ambiente, nombreClase, nombreMetodo, tabla);
            Valor lecturaV = ObtenerValorAtri(lectura, ambiente, nombreClase, nombreMetodo, tabla);
            Valor etiqueta = ObtenerValorAtri(etiquetaPregunta, ambiente, nombreClase, nombreMetodo, tabla);
            Valor porDefecto = ObtenerValorAtri(respuestas, ambiente, nombreClase, nombreMetodo, tabla);
            string sugerirC = "";
            string msgRequeridoC = "";
            bool requeridoC = false;
            bool lecturaC = false;
            string cadenaC = "";
            Fecha defecto = null;

            if (esCadena(sugerirV))
            {
                sugerirC = sugerirV.valor.ToString();
            }
            if (esCadena(requeridoMsnV))
            {
                msgRequeridoC = requeridoMsnV.valor.ToString();
            }
            if (esBooleano(requeridoV))
            {
                requeridoC = (getBooleanoNumero(requeridoV) == 1);
            }
            if (esBooleano(lecturaV))
            {
                lecturaC = (getBooleanoNumero(lecturaV) == 1);
            }
            if (esCadena(etiqueta))
            {
                cadenaC = etiqueta.valor.ToString();

            }
            if (esFecha(porDefecto))
            {
                Fecha f = (Fecha)porDefecto.valor;
                defecto = f;
            }
            //public modeloPregunta(string sugerir, bool lectura, string nombreP, DateTime val, int tipo, string cadPreg)
            modeloPregunta preguntaCadena = new modeloPregunta(sugerirC, lecturaC, idPregunta, defecto, 7,cadenaC);
            preguntaCadena.requerido = requeridoC;
            preguntaCadena.msgRequerido = msgRequeridoC;
            DialogResult resultado = new DialogResult();
            Form2 frmPregunta = new Form2();
            preguntaCadena.Visible = true;
            frmPregunta.setValor(preguntaCadena);
            resultado = frmPregunta.ShowDialog();
            if (resultado == DialogResult.OK)
            {
                Valor ingresoUsuario = castear(esTipo, Form2.respuestaFecha);
                responder(idPregunta, Form2.respuestaFecha);
                Console.WriteLine("Respuesta del usuario   " + Form2.respuestaFecha);
                tabla.insertarSimbolo(respuestas,6);
                respondeUsuario(tabla, ingresoUsuario, Constantes.RESPUESTA, idPregunta, ambiente);
                tabla.sacarSimbolo();
                return ingresoUsuario;
            }
            return new Valor();
        }


        private Valor ejecutarHora(ParseTreeNode nodo, Objeto simb, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            string esTipo = nodo.ChildNodes[3].ChildNodes[0].Token.ValueString;
            string idPregunta = simb.nombre;
            Simbolo etiquetaPregunta = simb.variablesObjeto.obtenerPregunta("etiqueta", Constantes.CADENA);
            Simbolo respuestas = simb.variablesObjeto.obtenerPregunta("respuestas", esTipo.Replace("es", ""));
            Simbolo sugerir = simb.variablesObjeto.obtenerPregunta("sugerir", Constantes.CADENA);
            Simbolo requerido = simb.variablesObjeto.obtenerPregunta("requerido", Constantes.BOOLEANO);
            Simbolo requeridoMsn = simb.variablesObjeto.obtenerPregunta("requeridoMsn", Constantes.CADENA);
            Simbolo lectura = simb.variablesObjeto.obtenerPregunta("lectura", Constantes.BOOLEANO);
            Valor sugerirV = ObtenerValorAtri(sugerir, ambiente, nombreClase, nombreMetodo, tabla);
            Valor requeridoV = ObtenerValorAtri(requerido, ambiente, nombreClase, nombreMetodo, tabla);
            Valor requeridoMsnV = ObtenerValorAtri(requeridoMsn, ambiente, nombreClase, nombreMetodo, tabla);
            Valor lecturaV = ObtenerValorAtri(lectura, ambiente, nombreClase, nombreMetodo, tabla);
            Valor etiqueta = ObtenerValorAtri(etiquetaPregunta, ambiente, nombreClase, nombreMetodo, tabla);
            Valor porDefecto = ObtenerValorAtri(respuestas, ambiente, nombreClase, nombreMetodo, tabla);
            string sugerirC = "";
            string msgRequeridoC = "";
            bool requeridoC = false;
            bool lecturaC = false;
            string cadenaC = "";
            Hora defecto = null;

            if (esCadena(sugerirV))
            {
                sugerirC = sugerirV.valor.ToString();
            }
            if (esCadena(requeridoMsnV))
            {
                msgRequeridoC = requeridoMsnV.valor.ToString();
            }
            if (esBooleano(requeridoV))
            {
                requeridoC = (getBooleanoNumero(requeridoV) == 1);
            }
            if (esBooleano(lecturaV))
            {
                lecturaC = (getBooleanoNumero(lecturaV) == 1);
            }
            if (esCadena(etiqueta))
            {
                cadenaC = etiqueta.valor.ToString();

            }
            if (esHora(porDefecto))
            {
                Hora f = (Hora)porDefecto.valor;
                defecto = f;
            }
            //public modeloPregunta(string sugerir, bool lectura, string nombreP, DateTime val, int tipo, string cadPreg)
            modeloPregunta preguntaCadena = new modeloPregunta(sugerirC, lecturaC, idPregunta, defecto, 8, cadenaC);
            preguntaCadena.requerido = requeridoC;
            preguntaCadena.msgRequerido = msgRequeridoC;
            DialogResult resultado = new DialogResult();
            Form2 frmPregunta = new Form2();
            preguntaCadena.Visible = true;
            frmPregunta.setValor(preguntaCadena);
            resultado = frmPregunta.ShowDialog();
            if (resultado == DialogResult.OK)
            {
                Valor ingresoUsuario = castear(esTipo, Form2.respuestaHora);
                responder(idPregunta, Form2.respuestaHora);
                Console.WriteLine("Respuesta del usuario   " + Form2.respuestaHora);
                tabla.insertarSimbolo(respuestas,10);
                respondeUsuario(tabla, ingresoUsuario, Constantes.RESPUESTA, idPregunta, ambiente);
                tabla.sacarSimbolo();
                return ingresoUsuario;
            }
            return new Valor();
        }

        private Valor ejecutarFechaHora(ParseTreeNode nodo, Objeto simb, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {

            string esTipo = nodo.ChildNodes[3].ChildNodes[0].Token.ValueString;
            string idPregunta = simb.nombre;
            Simbolo etiquetaPregunta = simb.variablesObjeto.obtenerPregunta("etiqueta", Constantes.CADENA);
            Simbolo respuestas = simb.variablesObjeto.obtenerPregunta("respuestas", esTipo.Replace("es", ""));
            Simbolo sugerir = simb.variablesObjeto.obtenerPregunta("sugerir", Constantes.CADENA);
            Simbolo requerido = simb.variablesObjeto.obtenerPregunta("requerido", Constantes.BOOLEANO);
            Simbolo requeridoMsn = simb.variablesObjeto.obtenerPregunta("requeridoMsn", Constantes.CADENA);
            Simbolo lectura = simb.variablesObjeto.obtenerPregunta("lectura", Constantes.BOOLEANO);
            Valor sugerirV = ObtenerValorAtri(sugerir, ambiente, nombreClase, nombreMetodo, tabla);
            Valor requeridoV = ObtenerValorAtri(requerido, ambiente, nombreClase, nombreMetodo, tabla);
            Valor requeridoMsnV = ObtenerValorAtri(requeridoMsn, ambiente, nombreClase, nombreMetodo, tabla);
            Valor lecturaV = ObtenerValorAtri(lectura, ambiente, nombreClase, nombreMetodo, tabla);
            Valor etiqueta = ObtenerValorAtri(etiquetaPregunta, ambiente, nombreClase, nombreMetodo, tabla);
            Valor porDefecto = ObtenerValorAtri(respuestas, ambiente, nombreClase, nombreMetodo, tabla);
            string sugerirC = "";
            string msgRequeridoC = "";
            bool requeridoC = false;
            bool lecturaC = false;
            string cadenaC = "";
            FechaHora defecto = null;

            if (esCadena(sugerirV))
            {
                sugerirC = sugerirV.valor.ToString();
            }
            if (esCadena(requeridoMsnV))
            {
                msgRequeridoC = requeridoMsnV.valor.ToString();
            }
            if (esBooleano(requeridoV))
            {
                requeridoC = (getBooleanoNumero(requeridoV) == 1);
            }
            if (esBooleano(lecturaV))
            {
                lecturaC = (getBooleanoNumero(lecturaV) == 1);
            }
            if (esCadena(etiqueta))
            {
                cadenaC = etiqueta.valor.ToString();

            }
            if (esFechaHora(porDefecto))
            {
                FechaHora f = (FechaHora)porDefecto.valor;
                defecto = f;
            }
            //public modeloPregunta(string sugerir, bool lectura, string nombreP, DateTime val, int tipo, string cadPreg)
            modeloPregunta preguntaCadena = new modeloPregunta(sugerirC, lecturaC, idPregunta, defecto, 9, cadenaC);
            preguntaCadena.requerido = requeridoC;
            preguntaCadena.msgRequerido = msgRequeridoC;
            DialogResult resultado = new DialogResult();
            Form2 frmPregunta = new Form2();
            preguntaCadena.Visible = true;
            frmPregunta.setValor(preguntaCadena);
            resultado = frmPregunta.ShowDialog();
            if (resultado == DialogResult.OK)
            {
                Valor ingresoUsuario = castear(esTipo, Form2.respuestaFechaHora);
                responder(idPregunta, Form2.respuestaFechaHora);
                Console.WriteLine("Respuesta del usuario   " + Form2.respuestaFechaHora);
                tabla.insertarSimbolo(respuestas,12);
                respondeUsuario(tabla, ingresoUsuario, Constantes.RESPUESTA, idPregunta, ambiente);
                tabla.sacarSimbolo();
                return ingresoUsuario;
            }
            return new Valor();
        }

        private Valor ejecutarSeleccionaUno(ParseTreeNode nodo, Objeto simb, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla, List<Opcion> opciones)
        {

            string esTipo = nodo.ChildNodes[3].ChildNodes[0].Token.ValueString;
            string idPregunta = simb.nombre;
            Simbolo etiquetaPregunta = simb.variablesObjeto.obtenerPregunta("etiqueta", Constantes.CADENA);
            Simbolo respuestas = simb.variablesObjeto.obtenerPregunta("respuestas", esTipo.Replace("es", ""));
            Simbolo sugerir = simb.variablesObjeto.obtenerPregunta("sugerir", Constantes.CADENA);
            Simbolo requerido = simb.variablesObjeto.obtenerPregunta("requerido", Constantes.BOOLEANO);
            Simbolo requeridoMsn = simb.variablesObjeto.obtenerPregunta("requeridoMsn", Constantes.CADENA);
            Simbolo lectura = simb.variablesObjeto.obtenerPregunta("lectura", Constantes.BOOLEANO);
            Valor sugerirV = ObtenerValorAtri(sugerir, ambiente, nombreClase, nombreMetodo, tabla);
            Valor requeridoV = ObtenerValorAtri(requerido, ambiente, nombreClase, nombreMetodo, tabla);
            Valor requeridoMsnV = ObtenerValorAtri(requeridoMsn, ambiente, nombreClase, nombreMetodo, tabla);
            Valor lecturaV = ObtenerValorAtri(lectura, ambiente, nombreClase, nombreMetodo, tabla);
            Valor etiqueta = ObtenerValorAtri(etiquetaPregunta, ambiente, nombreClase, nombreMetodo, tabla);
            Valor porDefecto = ObtenerValorAtri(respuestas, ambiente, nombreClase, nombreMetodo, tabla);
            Valor sumaD = sumar(porDefecto , new Valor(Constantes.CADENA , ""));
            string sugerirC = "";
            string msgRequeridoC = "";
            bool requeridoC = false;
            bool lecturaC = false;
            string cadenaC = "";
            string defecto = "";
            if (esCadena(sugerirV))
            {
                sugerirC = sugerirV.valor.ToString();
            }
            if (esCadena(requeridoMsnV))
            {
                msgRequeridoC = requeridoMsnV.valor.ToString();
            }
            if (esBooleano(requeridoV))
            {
                requeridoC = (getBooleanoNumero(requeridoV) == 1);
            }
            if (esBooleano(lecturaV))
            {
                lecturaC = (getBooleanoNumero(lecturaV) == 1);
            }
            if (esCadena(etiqueta))
            {
                cadenaC = etiqueta.valor.ToString();

            }

            if (esCadena(sumaD))
            {
                defecto = porDefecto.valor.ToString();
            }

            // modeloPregunta(string sugerir, bool lectura, string nombreP, List<Opcion> listaValores, string val, int tipo, string cadC)
            modeloPregunta preguntaCadena = new modeloPregunta(sugerirC, lecturaC, idPregunta, opciones, defecto,10, cadenaC);
            preguntaCadena.requerido = requeridoC;
            preguntaCadena.msgRequerido = msgRequeridoC;
            DialogResult resultado = new DialogResult();
            Form2 frmPregunta = new Form2();
            preguntaCadena.Visible = true;
            frmPregunta.setValor(preguntaCadena);
            resultado = frmPregunta.ShowDialog();
            if (resultado == DialogResult.OK)
            {
                Valor ingresoUsuario = castear(esTipo, Form2.respuestaSeleccionar1);
                String result = Form2.respuestaSeleccionar1;
                responder(idPregunta, Form2.respuestaSeleccionar1);
                Console.WriteLine("Respuesta del usuario   " + Form2.respuestaSeleccionar1);
                tabla.insertarSimbolo(respuestas,7);
                respondeUsuario(tabla, ingresoUsuario, Constantes.RESPUESTA, idPregunta, ambiente);
                tabla.sacarSimbolo();
                return ingresoUsuario;
            }
            return new Valor();
        }


        private Valor ejecutarSeleccionaMuchos(ParseTreeNode nodo, Objeto simb, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla, List<Opcion> opciones)
        {

            string esTipo = nodo.ChildNodes[3].ChildNodes[0].Token.ValueString;
            string idPregunta = simb.nombre;
            Simbolo etiquetaPregunta = simb.variablesObjeto.obtenerPregunta("etiqueta", Constantes.CADENA);
            Simbolo respuestas = simb.variablesObjeto.obtenerPregunta("respuestas", esTipo.Replace("es", ""));
            Simbolo sugerir = simb.variablesObjeto.obtenerPregunta("sugerir", Constantes.CADENA);
            Simbolo requerido = simb.variablesObjeto.obtenerPregunta("requerido", Constantes.BOOLEANO);
            Simbolo requeridoMsn = simb.variablesObjeto.obtenerPregunta("requeridoMsn", Constantes.CADENA);
            Simbolo lectura = simb.variablesObjeto.obtenerPregunta("lectura", Constantes.BOOLEANO);
            Valor sugerirV = ObtenerValorAtri(sugerir, ambiente, nombreClase, nombreMetodo, tabla);
            Valor requeridoV = ObtenerValorAtri(requerido, ambiente, nombreClase, nombreMetodo, tabla);
            Valor requeridoMsnV = ObtenerValorAtri(requeridoMsn, ambiente, nombreClase, nombreMetodo, tabla);
            Valor lecturaV = ObtenerValorAtri(lectura, ambiente, nombreClase, nombreMetodo, tabla);
            Valor etiqueta = ObtenerValorAtri(etiquetaPregunta, ambiente, nombreClase, nombreMetodo, tabla);
            Valor porDefecto = ObtenerValorAtri(respuestas, ambiente, nombreClase, nombreMetodo, tabla);
            Valor sumaD = sumar(porDefecto, new Valor(Constantes.CADENA, ""));
            string sugerirC = "";
            string msgRequeridoC = "";
            bool requeridoC = false;
            bool lecturaC = false;
            string cadenaC = "";
            string defecto = "";
            if (esCadena(sugerirV))
            {
                sugerirC = sugerirV.valor.ToString();
            }
            if (esCadena(requeridoMsnV))
            {
                msgRequeridoC = requeridoMsnV.valor.ToString();
            }
            if (esBooleano(requeridoV))
            {
                requeridoC = (getBooleanoNumero(requeridoV) == 1);
            }
            if (esBooleano(lecturaV))
            {
                lecturaC = (getBooleanoNumero(lecturaV) == 1);
            }
            if (esCadena(etiqueta))
            {
                cadenaC = etiqueta.valor.ToString();

            }

            if (esCadena(sumaD))
            {
                defecto = porDefecto.valor.ToString();
            }
            // modeloPregunta(string sugerir, bool lectura, string nombreP, List<Opcion> listaValores, string val, int tipo, string cadC)
            modeloPregunta preguntaCadena = new modeloPregunta(sugerirC, lecturaC, idPregunta, opciones, defecto, 11, cadenaC);
            preguntaCadena.requerido = requeridoC;
            preguntaCadena.msgRequerido = msgRequeridoC;
            DialogResult resultado = new DialogResult();
            Form2 frmPregunta = new Form2();
            preguntaCadena.Visible = true;
            frmPregunta.setValor(preguntaCadena);
            resultado = frmPregunta.ShowDialog();
            if (resultado == DialogResult.OK)
            {
                Valor ingresoUsuario = castear(esTipo, Form2.respuestaSeleccionarMuchos.ElementAt(0));
                responder(idPregunta, Form2.respuestaSeleccionarMuchos);
                Console.WriteLine("Respuesta del usuario   " + Form2.respuestaSeleccionarMuchos.ElementAt(0));
                tabla.insertarSimbolo(respuestas,13);
                respondeUsuario(tabla, ingresoUsuario, Constantes.RESPUESTA, idPregunta, ambiente);
                tabla.sacarSimbolo();
                return ingresoUsuario;
            }
            return new Valor();
        }

        private Valor ejecutarFichero(Objeto simb, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {

            return new Valor();
        }


        private Valor ejecutarCalcular(ParseTreeNode nodo, Objeto simb, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {

            string idPregunta = simb.nombre;

            Simbolo respuestas = simb.variablesObjeto.buscarSimboloRes("respuestas");
            if (respuestas != null)
            {
                Valor porDefecto = ObtenerValorAtri(respuestas, ambiente, nombreClase, nombreMetodo, tabla);
                if (!esNulo(porDefecto))
                {
                    respuestas.tipo = porDefecto.tipo;
                    respuestas.asignarValor(porDefecto,nodo);
                }
                else
                {
                    respuestas.tipo = Constantes.DECIMAL;
                    respuestas.asignarValor(porDefecto, nodo);
                }
                
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "La variable respuestas no fue encontrada en la pregunta " + idPregunta);
            }
           
/*
            //public modeloPregunta(string sugerir, bool lectura, string idP, int val)
            modeloPregunta preguntaCadena = new modeloPregunta(sugerirC, lecturaC, idPregunta, defecto, cadenaC);
            preguntaCadena.requerido = requeridoC;
            preguntaCadena.msgRequerido = msgRequeridoC;
            DialogResult resultado = new DialogResult();
            Form2 frmPregunta = new Form2();
            preguntaCadena.Visible = true;
            frmPregunta.setValor(preguntaCadena);
            resultado = frmPregunta.ShowDialog();
            if (resultado == DialogResult.OK)
            {
                Valor ingresoUsuario = castear(esTipo, Form2.respuestaEntero);
                String result = Form2.respuestaCadena;
                Console.WriteLine("Respuesta del usuario   " + Form2.respuestaEntero);
                tabla.insertarSimbolo(respuestas);
                respondeUsuario(tabla, ingresoUsuario, Constantes.RESPUESTA, idPregunta, ambiente);
                
                tabla.sacarSimbolo();
                responder(idPregunta, Form2.respuestaEntero);
                return ingresoUsuario;
            }*/
            return new Valor();
        }

        

        private elementoRetorno resolverLlamadaPregunta(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            string tipoPregunta = nodo.ChildNodes[0].Term.Name;
            ParseTreeNode nodoPregunta = nodo.ChildNodes[0];
            switch (tipoPregunta)
            {
                case Constantes.LLA_CALCULAR:
                    {
                        // LLA_CALCULAR.Rule = identificador + abrePar + cierraPar + punto + ToTerm(Constantes.LLA_CALCULAR) + abrePar + cierraPar;
                        #region EjecutarCadena
                        string nombre = nodoPregunta.ChildNodes[0].Token.ValueString;
                        Simbolo s = tabla.obtenerPregunta(nombre, nombre);
                        if (s == null)
                        {
                            s = temporalParametros.obtenerPregunta(nombre, nombre);
                            if (s != null)
                            {
                                if (s is Objeto)
                                {
                                    Objeto r = (Objeto)s;
                                    ret.val = this.ejecutarCalcular(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, temporalParametros);
                                }
                            }
                        }
                        else
                        {
                            if (s is Objeto)
                            {
                                Objeto r = (Objeto)s;

                                ret.val = this.ejecutarCalcular(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, tabla);
                            }

                        }
                        break;
                        #endregion
                    }
                case Constantes.LLA_NOTA:
                    {
                        #region EjecutarNota
                        string nombre = nodoPregunta.ChildNodes[0].Token.ValueString;
                        Simbolo s = tabla.obtenerPregunta(nombre, nombre);
                        if (s == null)
                        {
                            s = temporalParametros.obtenerPregunta(nombre, nombre);
                            if (s != null)
                            {
                                if (s is Objeto)
                                {
                                    Objeto r = (Objeto)s;
                                    this.ejecutarNota(r, ambiente, nombreClase, nombreMetodo, temporalParametros);
                                }
                            }
                        }
                        else
                        {
                            if (s is Objeto)
                            {
                                Objeto r = (Objeto)s;
                                this.ejecutarNota(r, ambiente, nombreClase, nombreMetodo, tabla);
                            }
                            
                        }

                        break;
                        #endregion
                    }

                case Constantes.LLA_CADENA:
                    {
                        #region EjecutarCadena
                        string nombre = nodoPregunta.ChildNodes[0].Token.ValueString;
                        Simbolo s = tabla.obtenerPregunta(nombre, nombre);
                        if (s == null)
                        {
                            s = temporalParametros.obtenerPregunta(nombre, nombre);
                            if (s != null)
                            {
                                if (s is Objeto)
                                {
                                    Objeto r = (Objeto)s;
                                    ret.val = this.ejecutarCadena(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, temporalParametros);
                                }
                            }
                        }
                        else
                        {
                            if (s is Objeto )
                            {
                                Objeto r = (Objeto)s;
                                ret.val = this.ejecutarCadena(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, tabla);
                            }

                        }
                        break;
                        #endregion
                    }

                case Constantes.LLA_ENTERO:
                    {
                        #region EjecutarCadena
                        string nombre = nodoPregunta.ChildNodes[0].Token.ValueString;
                        Simbolo s = tabla.obtenerPregunta(nombre, nombre);
                        if (s == null)
                        {
                            s = temporalParametros.obtenerPregunta(nombre, nombre);
                            if (s != null)
                            {
                                if (s is Objeto)
                                {
                                    Objeto r = (Objeto)s;
                                    ret.val = this.ejecutarEntero(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, temporalParametros);
                                }
                            }
                        }
                        else
                        {
                            if (s is Objeto)
                            {
                                Objeto r = (Objeto)s;

                                ret.val = this.ejecutarEntero(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, tabla);
                            }

                        }
                        break;
                        #endregion
                    }


                case Constantes.LLA_DECIMAL:
                    {
                        #region EjecutarCadena
                        string nombre = nodoPregunta.ChildNodes[0].Token.ValueString;
                        Simbolo s = tabla.obtenerPregunta(nombre, nombre);
                        if (s == null)
                        {
                            s = temporalParametros.obtenerPregunta(nombre, nombre);
                            if (s != null)
                            {
                                if (s is Objeto)
                                {
                                    Objeto r = (Objeto)s;
                                    ret.val = this.ejecutarDecimal(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, temporalParametros);
                                }
                            }
                        }
                        else
                        {
                            if (s is Objeto)
                            {
                                Objeto r = (Objeto)s;
                                ret.val = this.ejecutarDecimal(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, tabla);
                            }

                        }
                        break;
                        #endregion
                    }


                case Constantes.LLA_CONDICION:
                    {
                        #region EjecutarCadena
                        string nombre = nodoPregunta.ChildNodes[0].Token.ValueString;
                        Simbolo s = tabla.obtenerPregunta(nombre, nombre);
                        if (s == null)
                        {
                            s = temporalParametros.obtenerPregunta(nombre, nombre);
                            if (s != null)
                            {
                                if (s is Objeto)
                                {
                                    Objeto r = (Objeto)s;
                                    ret.val = this.ejecutarCondicion(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, temporalParametros);
                                }
                            }
                        }
                        else
                        {
                            if (s is Objeto)
                            {
                                Objeto r = (Objeto)s;
                                ret.val = this.ejecutarCondicion(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, tabla);
                            }

                        }
                        break;
                        #endregion
                    }


                case Constantes.LLA_RANGO:
                    {
                        #region EjecutarCadena
                        string nombre = nodoPregunta.ChildNodes[0].Token.ValueString;
                        Simbolo s = tabla.obtenerPregunta(nombre, nombre);
                        if (s == null)
                        {
                            s = temporalParametros.obtenerPregunta(nombre, nombre);
                            if (s != null)
                            {
                                if (s is Objeto)
                                {
                                    Objeto r = (Objeto)s;
                                    ret.val = this.ejecutarRango(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, temporalParametros);
                                }
                            }
                        }
                        else
                        {
                            if (s is Objeto)
                            {
                                Objeto r = (Objeto)s;
                                ret.val = this.ejecutarRango(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, tabla);
                            }

                        }
                        break;
                        #endregion
                    }


                case Constantes.LLA_FECHA:
                    {
                        #region EjecutarCadena
                        string nombre = nodoPregunta.ChildNodes[0].Token.ValueString;
                        Simbolo s = tabla.obtenerPregunta(nombre, nombre);
                        if (s == null)
                        {
                            s = temporalParametros.obtenerPregunta(nombre, nombre);
                            if (s != null)
                            {
                                if (s is Objeto)
                                {
                                    Objeto r = (Objeto)s;
                                    ret.val = this.ejecutarFecha(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, temporalParametros);
                                }
                            }
                        }
                        else
                        {
                            if (s is Objeto)
                            {
                                Objeto r = (Objeto)s;
                                ret.val = this.ejecutarFecha(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, tabla);
                            }

                        }
                        break;
                        #endregion
                    }


                case Constantes.LLA_FECHAHORA:
                    {
                        #region EjecutarCadena
                        string nombre = nodoPregunta.ChildNodes[0].Token.ValueString;
                        Simbolo s = tabla.obtenerPregunta(nombre, nombre);
                        if (s == null)
                        {
                            s = temporalParametros.obtenerPregunta(nombre, nombre);
                            if (s != null)
                            {
                                if (s is Objeto)
                                {
                                    Objeto r = (Objeto)s;
                                    ret.val = this.ejecutarFechaHora(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, temporalParametros);
                                }
                            }
                        }
                        else
                        {
                            if (s is Objeto)
                            {
                                Objeto r = (Objeto)s;
                                ret.val = this.ejecutarFechaHora(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, tabla);
                            }

                        }
                        break;
                        #endregion
                    }

                case Constantes.LLA_HORA:
                    {
                        #region EjecutarCadena
                        string nombre = nodoPregunta.ChildNodes[0].Token.ValueString;
                        Simbolo s = tabla.obtenerPregunta(nombre, nombre);
                        if (s == null)
                        {
                            s = temporalParametros.obtenerPregunta(nombre, nombre);
                            if (s != null)
                            {
                                if (s is Objeto)
                                {
                                    Objeto r = (Objeto)s;
                                    ret.val = this.ejecutarHora(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, temporalParametros);
                                }
                            }
                        }
                        else
                        {
                            if (s is Objeto)
                            {
                                Objeto r = (Objeto)s;
                                ret.val = this.ejecutarHora(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, tabla);
                            }

                        }
                        break;
                        #endregion
                    }

                case Constantes.LLA_SELECCIONA_1:
                    {
                        #region EjecutarCadena
                        string nombre = nodoPregunta.ChildNodes[0].Token.ValueString;
                        string nombreLista = nodoPregunta.ChildNodes[5].Token.ValueString;
                        Simbolo s2 = tabla.buscarSimbolo(nombreLista, ambiente);
                        if (s2 == null) 
                        {
                            s2 = temporalParametros.buscarSimbolo(nombreLista, ambiente);
                        }
                        List<Opcion> opciones = new List<Opcion>();
                        if (s2 != null)
                        {
                            if (s2 is ListaOpciones)
                            {
                                ListaOpciones op = (ListaOpciones)s2;
                                opciones = op.matriz.obtenerOpciones();
                            }
                        }
                        Simbolo s = tabla.obtenerPregunta(nombre, nombre);
                        if (s == null)
                        {
                            s = temporalParametros.obtenerPregunta(nombre, nombre);
                            if (s != null)
                            {
                                if (s is Objeto)
                                {
                                    Objeto r = (Objeto)s;
                                    ret.val = this.ejecutarSeleccionaUno(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, temporalParametros, opciones);
                                }
                            }
                        }
                        else
                        {
                            if (s is Objeto)
                            {
                                Objeto r = (Objeto)s;
                                ret.val = this.ejecutarSeleccionaUno(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, tabla, opciones);
                            }

                        }
                        break;
                        #endregion
                    }

                case Constantes.LLA_SELECCIONA_MULTIPLES:
                    {
                        #region EjecutarCadena
                        string nombre = nodoPregunta.ChildNodes[0].Token.ValueString;
                        string nombreLista = nodoPregunta.ChildNodes[5].Token.ValueString;
                        Simbolo s2 = tabla.buscarSimbolo(nombreLista, ambiente);
                        if (s2 == null)
                        {
                            s2 = temporalParametros.buscarSimbolo(nombreLista, ambiente);
                        }
                        List<Opcion> opciones = new List<Opcion>();
                        if (s2 != null)
                        {
                            if (s2 is ListaOpciones)
                            {
                                ListaOpciones op = (ListaOpciones)s2;
                                opciones = op.matriz.obtenerOpciones();
                            }
                        }
                        Simbolo s = tabla.obtenerPregunta(nombre, nombre);
                        if (s == null)
                        {
                            s = temporalParametros.obtenerPregunta(nombre, nombre);
                            if (s != null)
                            {
                                if (s is Objeto)
                                {
                                    Objeto r = (Objeto)s;
                                    ret.val = this.ejecutarSeleccionaMuchos(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, temporalParametros, opciones);
                                }
                            }
                        }
                        else
                        {
                            if (s is Objeto)
                            {
                                Objeto r = (Objeto)s;
                                ret.val = this.ejecutarSeleccionaMuchos(nodo.ChildNodes[0], r, ambiente, nombreClase, nombreMetodo, tabla, opciones);
                            }

                        }
                        break;
                        #endregion
                    }
                case Constantes.LLA_FICHERO:
                    {
                        #region EjecutarCadena

                        break;
                        #endregion
                    }

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
                lTemporal.lAtributos = cambiarAmbito(ambiente.getAmbito(), lTemporal.lAtributos);
                for (int j = 0; j < lTemporal.lAtributos.Count; j++)
                {
                    string rutaTemp = "";
                    atriTemp = lTemporal.lAtributos.ElementAt(j);
                    valoresRuta = atriTemp.rutaAcc.Split('/');
                    valoresRuta[0] = ambiente.getAmbito();// +"/" + nombre;
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
                    string ambitoTemporal = ambiente.getAmbito();// +"/" + nombre; ;
                    atriTemp.ambito = ambitoTemporal; //
                    nuevo.variablesObjeto.insertarSimbolo(atriTemp,11);
                    nuevo.rutaAcc = ambitoTemporal;
                    /*---- buscando nuevamente el simbolo para poderlo asignar a la tabla*/
                    Contexto c = new Contexto();
                    c.llenarAmbitos(ambitoTemporal);
                    if (atriTemp.nodoExpresionValor != null)
                    {
                        elementoRetorno r = new elementoRetorno();
                        Simbolo s = nuevo.variablesObjeto.buscarSimbolo(atriTemp.nombre, c);
                        if (s != null)
                        {
                            if (s is Objeto && atriTemp.nodoExpresionValor.Term.Name.Equals(Constantes.INSTANCIA, StringComparison.CurrentCultureIgnoreCase))
                            {
                                Objeto ob = (Objeto)s;
                                temporalParametros = nuevo.variablesObjeto;
                                r = resolverExpresion(atriTemp.nodoExpresionValor, c, nombreClase, nombreMetodo, ob.variablesObjeto);
                            }
                            else
                            {
                                temporalParametros = nuevo.variablesObjeto;
                                r = resolverExpresion(atriTemp.nodoExpresionValor, c, nombreClase, nombreMetodo, nuevo.variablesObjeto);
                            }
                            asignarSimbolo(atriTemp.nombre, nodo, r.val, s, c, nombreClase, nombreMetodo, nuevo.variablesObjeto);
                        }
                        else
                        {
                            Constantes.erroresEjecucion.errorSemantico(nodo, "No se ha podido encontrar el atributo " + atriTemp.nombre);

                        }                      
                    }
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
            bool esLista = this.esLista(tipo);
            int no = nodo.ChildNodes.Count;
            elementoRetorno v;
            if (no == 3)
            {
                if (!esObj)
                {
                    if (esLista)
                    {
                        v = resolverExpresion(nodo.ChildNodes[2], ambiente, nombreClase, nombreMetodo, tabla);
                        ListaOpciones nuevaLista = new ListaOpciones(nombre, tipo, rutaAcceso, false);
                        nuevaLista.usada = true;
                        tabla.insertarSimbolo(nuevaLista, nodo);
                        asignarSimbolo(nombre, nodo, v.val, nuevaLista, ambiente, nombreClase, nombreMetodo, tabla);


                    }
                    else
                    {
                        this.esAtriAsigna = false;
                        temporalParametros = tabla;
                        v = resolverExpresion(nodo.ChildNodes[2], ambiente, nombreClase, nombreMetodo, tabla);
                        Variable varNueva = new Variable(nombre, tipo, rutaAcceso, false);
                        varNueva.usada = true;
                        tabla.insertarSimbolo(varNueva, nodo);
                        asignarSimbolo(nombre, nodo, v.val, varNueva, ambiente, nombreClase, nombreMetodo, tabla);

                    }

                    
                }
                else
                {
                    this.esAtriAsigna = false;
                    declaraObjeto(nombre, tipo, nodo, ambiente, nombreClase, nombreMetodo, tabla);
                    Simbolo s = tabla.buscarSimbolo(nombre, ambiente);
                    if (s != null)
                    {
                        if (s is Objeto && nodo.ChildNodes[2].Term.Name.Equals(Constantes.INSTANCIA, StringComparison.CurrentCultureIgnoreCase))
                        {
                            Objeto ob = (Objeto)s;
                            temporalParametros = tabla;
                            v = resolverExpresion(nodo.ChildNodes[2], ambiente, nombreClase, nombreMetodo, ob.variablesObjeto);
                        }
                        else
                        {
                            temporalParametros = tabla;
                            v = resolverExpresion(nodo.ChildNodes[2], ambiente, nombreClase, nombreMetodo, tabla);
                        }
                        asignarSimbolo(nombre, nodo, v.val, s, ambiente, nombreClase, nombreMetodo, tabla);
                    }
                    else
                    {
                        Constantes.erroresEjecucion.errorSemantico(nodo, "No se ha encontrado la variable "+ nombre+" en el ambito actual");

                    }  
                   
                }
            }
            else
            { //no ==2
                #region declaracion sin asignacion
                this.esAtriAsigna = false;
                if (!esObj)
                {
                    if (esLista)
                    {
                        ListaOpciones nueva = new ListaOpciones(nombre, tipo, rutaAcceso, false);
                        tabla.insertarSimbolo(nueva,15);
                    }
                    else
                    {
                        Variable varNueva = new Variable(nombre, tipo, rutaAcceso, false);
                        tabla.insertarSimbolo(varNueva, nodo);
                    }
                    
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


        private elementoRetorno insertarLista(ListaOpciones lista, ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            ParseTreeNode nodoParametros = nodo.ChildNodes[1];
            List<Valor> valoresParametros = resolviendoParametros(nodoParametros, ambiente, nombreClase, nombreMetodo, temporalParametros);
            lista.insertar(valoresParametros);
            return ret;
        }

        private elementoRetorno buscar(ListaOpciones lista, ParseTreeNode nodoFilaLista, ParseTreeNode indiceEnFila, Contexto ambiente, string nombreclase, string nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {

            Valor v1 = resolverExpresion(nodoFilaLista, ambiente, nombreclase, nombreMetodo, temporalParametros).val;
            Valor v2 = resolverExpresion(indiceEnFila, ambiente, nombreclase, nombreMetodo, temporalParametros).val;
            if (!esNulo(v1) && !esNulo(v2))
            {
                if (esEntero(v2))
                {
                    int v22 = getEntero(v2);
                    ret.val = lista.buscar(v1, v22);
                    return ret;
                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico(nodoFilaLista, "Funcion Buscar, lista de opciones. El segundo  valor debe de ser entero, y es "  + v2.tipo);
                    ret.val = new Valor();
                    return ret;
                }
            }
            else
            {
                ret.val = new Valor();
                return ret;
            }

        }

        private elementoRetorno obtener(ListaOpciones lista, ParseTreeNode nodoFilaLista, ParseTreeNode indiceEnFila, Contexto ambiente, string nombreclase, string nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            Valor v1 = resolverExpresion(nodoFilaLista, ambiente, nombreclase, nombreMetodo, temporalParametros).val;
            Valor v2 = resolverExpresion(indiceEnFila, ambiente, nombreclase, nombreMetodo, temporalParametros).val;
            if (!esNulo(v1) && !esNulo(v2))
            {
                if (esEntero(v1) && esEntero(v2))
                {
                    int v11 = getEntero(v1);
                    int v22= getEntero(v2);
                    ret.val = lista.Obtener(v11, v22);
                    return ret;
                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico(nodoFilaLista, "Funcion Obtener, lista de opciones, los valores deben de ser enteros, tanto el indice como la posicion a obtener en la fila y son " + v1.tipo + " y " + v2.tipo);
                    ret.val = new Valor();
                    return ret;
                }
            }
            else
            {
                ret.val = new Valor();
                return ret;
            }
        }

        private elementoRetorno resolverAccesoVar(ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            int noValores = nodo.ChildNodes.Count;
            ParseTreeNode elementoAcceso;
            string tipoObj = "";
            string nombreElemento = "";
            bool banderaSeguir = true;
            int i=0;
            Simbolo simbActual= new Simbolo();
            tipoObj = nombreClase;
            tablaSimbolos tabla2 = new tablaSimbolos();
            int cont = 0;
            bool banderaLista = false;
            string nombreClase2 = nombreClase;
            if (noValores == 2)
            {
                Console.WriteLine("aqui");
            }
            do
            {
                elementoAcceso = nodo.ChildNodes[i];
                if (elementoAcceso.Term.Name.Equals(Constantes.ID, StringComparison.CurrentCultureIgnoreCase))
                {
                   nombreElemento= elementoAcceso.ChildNodes[0].Token.ValueString;
                   if (i == 0)
                   {
                       simbActual = tabla.buscarSimbolo(nombreElemento, ambiente);
                       if (simbActual == null)
                       {
                           simbActual = temporalParametros.buscarSimbolo(nombreElemento, ambiente);
                       }
                   }
                   else
                   {
                       simbActual = tabla2.buscarSimbolo(nombreElemento, ambiente);
                       if (simbActual == null)
                       {
                           simbActual = temporalParametros.buscarSimbolo(nombreElemento, ambiente);
                       }

                   }
                                   
                    if (simbActual != null)
                    {
                        if (simbActual is Objeto)
                        {
                            Objeto g = (Objeto)simbActual;
                            tabla2 = g.variablesObjeto;
                        }
                        if (simbActual is ListaOpciones)
                        {
                            banderaLista = true;
                        }
                       /* if (simbActual is Objeto && esNulo(simbActual.valor))
                        {
                            ret.val = new Valor();
                            ambiente.addAmbito(simbActual.nombre);
                            cont++;
                        }
                        else
                        {*/
                            ret.val = new Valor(simbActual.tipo, simbActual);
                            ambiente.addAmbito(simbActual.nombre);
                            cont++;
                      //  }

                        
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
                        temporalParametros = tabla;
                        ParseTreeNode nodoId = elementoAcceso.ChildNodes[0];
                        string nombreFuncion = nodoId.Token.ValueString;
                        if (nombreFuncion.Equals("insertar", StringComparison.CurrentCultureIgnoreCase) && banderaLista)
                        {
                            if (simbActual is ListaOpciones)
                            {
                                ListaOpciones op = (ListaOpciones)simbActual;
                                ret.val= this.insertarLista(op,elementoAcceso,ambiente, nombreClase, nombreMetodo, tabla2, ret).val;
                                banderaLista = false;
                                banderaSeguir= false;
                            }
                            else
                            {
                                Constantes.erroresEjecucion.errorSemantico(elementoAcceso, "El Simbolo no es una lista, no se puede realizar el metodo insertar");
                            }
                        }
                        else
                        {
                            if (cont == 0)
                            {
                                ret = this.llamadaFuncion(elementoAcceso, ambiente, nombreClase2, nombreMetodo, tabla, ret);
                            }
                            else
                            {
                                ret = this.llamadaFuncion(elementoAcceso, ambiente, nombreClase2, nombreMetodo, tabla2, ret);
                            }
                            
                        }
                        if (ret.val.valor is Objeto)
                        {
                            Objeto c = (Objeto) ret.val.valor;
                            tabla2 = c.variablesObjeto;
                            tabla2.cambiarAmbito(ambiente.getAmbito());
                            nombreClase2 = c.tipo;
                        }
                        else if (ret.val.valor is ListaOpciones)
                        {
                            simbActual = (Simbolo)ret.val.valor;
                            banderaLista = true;
                        }
                        else if (ret.val.valor is Matriz)
                        {
                            banderaLista = true;
                        }
                    }
                    else
                    {
                        banderaSeguir = false;
                        ret.val = new Valor(Constantes.NULO, Constantes.NULO);
                        Constantes.erroresEjecucion.errorSemantico(elementoAcceso, "No existe el elemento " + tipoObj+ ", en las clases actuales");
                    }
                }
                else if(elementoAcceso.Term.Name.Equals(Constantes.OBTENER, StringComparison.CurrentCultureIgnoreCase)){
                    ParseTreeNode noFila = elementoAcceso.ChildNodes[1];
                    ParseTreeNode noPos = elementoAcceso.ChildNodes[2];
                    if (banderaLista)
                    {
                        if (simbActual is ListaOpciones)
                        {
                            ListaOpciones op = (ListaOpciones)simbActual;
                            ret.val = this.obtener(op, noFila, noPos, ambiente, nombreClase, nombreMetodo, tabla2, ret).val;
                            banderaLista = false;
                            banderaSeguir = false;
                        }
                        else
                        {
                            Constantes.erroresEjecucion.errorSemantico(elementoAcceso, "El Simbolo no es una lista, no se puede realizar el metodo obtener");
                        }
                    }
                    else
                    {
                        Constantes.erroresEjecucion.errorSemantico(elementoAcceso, "El Simbolo no es una lista, no se puede realizar el metodo obtener");
                    }

                }else if(elementoAcceso.Term.Name.Equals(Constantes.BUSCAR, StringComparison.CurrentCultureIgnoreCase)){
                    ParseTreeNode noFila = elementoAcceso.ChildNodes[1];
                    ParseTreeNode noPos = elementoAcceso.ChildNodes[2];
                     if (banderaLista)
                        {
                            if (simbActual is ListaOpciones)
                            {
                                ListaOpciones op = (ListaOpciones)simbActual;
                                ret.val = this.buscar(op, noFila, noPos, ambiente, nombreClase, nombreMetodo, tabla2, ret).val;
                                banderaLista = false;
                            }
                            else
                            {
                                Constantes.erroresEjecucion.errorSemantico(elementoAcceso, "El Simbolo no es una lista, no se puede realizar el metodo insertar");
                            }
                            
                        }
                      

                }else
                {
                    //mrreglos

                }
                i++;

            } while (i < noValores && banderaSeguir);
            for (int j = 0; j < cont; j++)
            {
                ambiente.salirAmbito();
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

            if (retAcceso.val.valor is Simbolo)
            {
                Simbolo s = (Simbolo)retAcceso.val.valor;
                if (esObjecto(s.tipo) && expresion.Term.Name.Equals(Constantes.INSTANCIA, StringComparison.InvariantCultureIgnoreCase))
                {
                    Objeto t = (Objeto)s;
                    temporalParametros = tabla;
                    rr = resolverExpresion(expresion, ambiente, nombreClase, nombreMetodo, t.variablesObjeto);
                }
                else
                {
                    temporalParametros = tabla;
                    rr = resolverExpresion(expresion, ambiente, nombreClase, nombreMetodo, tabla);
                }
                asignarSimbolo(s.nombre, nodo, rr.val, s, ambiente, nombreClase, nombreMetodo, tabla);
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "Valor "+retAcceso.val.valor+" no valido para realizar una asignacion");
            }
            return ret;
        }


        private void asignarSimbolo(string nombreAsignar, ParseTreeNode nodo, Valor v, Simbolo simb, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
        {
            if (simb != null)
            {
                if (v.valor is Variable)
                {
                    Variable s = (Variable)v.valor;
                    simb.asignarValor(s.valor, nodo);

                }
                else if (v.valor is Objeto)
                {
                    Objeto s = (Objeto)v.valor;
                    bool a = simb.asignarValor(s.valor, nodo);
                    if (a)
                    {
                        Objeto c = (Objeto)simb;

                        c.variablesObjeto = s.variablesObjeto.clonarTabla();
                        c.variablesObjeto.cambiarAmbito(c.ambito);
                        simb = c;
                    }

                }else if(v.valor is ListaOpciones){
                    ListaOpciones s = (ListaOpciones)v.valor;
                    if (simb is ListaOpciones)
                    {
                        ListaOpciones dos = (ListaOpciones)simb;
                        dos.matriz = s.matriz;
                    }
                    else
                    {
                        Constantes.erroresEjecucion.errorSemantico(nodo, "No se pudo realizar asignacion a lista de opciones, valores no compatibles con " + nombreAsignar);
                    }

                }
                else if (v.valor is Matriz)
                {
                    Matriz s = (Matriz)v.valor;
                    if (simb is ListaOpciones)
                    {
                        ListaOpciones dos = (ListaOpciones)simb;
                        dos.matriz = s;
                    }
                    else
                    {
                        Constantes.erroresEjecucion.errorSemantico(nodo, "No se pudo realizar asignacion a lista de opciones, valores no compatibles con " + nombreAsignar);
                    }

                }
                else
                {
                    simb.asignarValor(v, nodo);
                }
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "No existe la variable " + nombreAsignar + ", en el ambito actual " + ambiente.getAmbito());
            }

        }

        #endregion

        /*-------------------------------- Fin declaraciones y Asignaciones ---------------------------------------*/


        /*----------------------------------- Estructuras de Control -----------------------------------------*/

        #region caso (Switch)

        private elementoRetorno resolverCaso(ParseTreeNode nodo, Contexto ambiente, string nombreClase, String nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            ParseTreeNode expresionPivote = nodo.ChildNodes[0];
            ParseTreeNode cuerpoCaso = nodo.ChildNodes[1];
            Valor resPivote = resolverExpresion(expresionPivote, ambiente, nombreClase, nombreMetodo, tabla).val;
            if (!esNulo(resPivote))
            {
                bool banderaEntroCaso = false;
                ParseTreeNode temp, expresionTemp, cuerpoTemp;

                string tipoCasoDefecto = "";
                Valor resCasoTemp;
                for (int i = 0; i < cuerpoCaso.ChildNodes.Count; i++)
                {
                    temp = cuerpoCaso.ChildNodes[i];
                    tipoCasoDefecto = temp.Term.Name.ToString();
                    if ((banderaEntroCaso == false) || (banderaEntroCaso == true && ret.banderaRetorno == false))
                    {
                        if (tipoCasoDefecto.Equals(Constantes.VALOR, StringComparison.CurrentCultureIgnoreCase))
                        {
                            expresionTemp = temp.ChildNodes[0];
                            cuerpoTemp = temp.ChildNodes[1];
                            resCasoTemp = resolverExpresion(expresionTemp, ambiente, nombreClase, nombreMetodo, tabla).val;
                            Valor v = igualIgual(resPivote, resCasoTemp);
                            if (esNulo(v))
                            {
                                Constantes.erroresEjecucion.errorSemantico(temp, "La expresion pivote de la sentencia caso es de tipo, " + resPivote.tipo + ", no se puede operar con una operacion con Nulo");
                            }
                            else
                            {
                                if (esVerdadero(v) || (banderaEntroCaso == true && ret.banderaRetorno == false))
                                {
                                    banderaEntroCaso = true;
                                    foreach (ParseTreeNode item in cuerpoTemp.ChildNodes)
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
                            }
                        }
                        else
                        {
                            cuerpoTemp = temp.ChildNodes[0];
                            banderaEntroCaso = true;
                            foreach (ParseTreeNode item in cuerpoTemp.ChildNodes)
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

                    }
                }

                if (ret.parar)
                {
                    ret.parar = false;
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
            
            ret = evaluarArbol(nodoDeclaracion, ambiente, nombreClase, nombreMetodo, tabla, ret);
            Valor v = resolverExpresion(nodoCondicion, ambiente, nombreClase, nombreMetodo, tabla).val;
            if (!esNulo(v))
            {
                if (esBool(v))
                {
                    while (esVerdadero(v))
                    {
                        ambiente.addPara();
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
                            ambiente.salirAmbito();
                            break;
                        }
                        if (ret.continuar)
                        {
                            //resExpr = resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla);
                            ret = evaluarArbol(asignaUnario, ambiente, nombreClase, nombreMetodo, tabla, ret);
                            v = resolverExpresion(nodoCondicion, ambiente, nombreClase, nombreMetodo, tabla).val;
                            ret.continuar = false;
                            ambiente.salirAmbito();
                            continue;

                        }
                        ret = evaluarArbol(asignaUnario, ambiente, nombreClase, nombreMetodo, tabla, ret);
                        v = resolverExpresion(nodoCondicion, ambiente, nombreClase, nombreMetodo, tabla).val;
                        ambiente.salirAmbito();

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
           /* if (esBooleano(resExpr))
            {*/
                
                // while (esVerdadero(resExpr))

                do
                {
                    ambiente.addRepetir();
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
                        ambiente.salirAmbito();
                        break;
                    }
                    if (ret.continuar)
                    {
                        //resExpr = resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla);
                        ret.continuar = false;
                        ambiente.salirAmbito();
                        continue;

                    }

                    resExpr = resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla).val;
                    ambiente.salirAmbito();
                } while (esVerdadero(resExpr)==false);
                
          /*  }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "La expresion para el ciclo repetir hasta  no es valida");
            }*/

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
                
                // while (esVerdadero(resExpr))

                do
                {
                    ambiente.addHacerMientras();
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
                        ambiente.salirAmbito();
                        break;
                    }
                    if (ret.continuar)
                    {
                        //resExpr = resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla);
                        ret.continuar = false;
                        ambiente.salirAmbito();
                        continue;

                    }

                    resExpr = resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla).val;
                    ambiente.salirAmbito();
                } while (esVerdadero(resExpr));
                
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
                
                while (esVerdadero(resExpr))
                {
                    ambiente.addMientras();

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
                        ambiente.salirAmbito();
                        break;
                    }
                    if (ret.continuar)
                    {
                        //resExpr = resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla);
                        ret.continuar = false;
                        ambiente.salirAmbito();
                        continue;

                    }

                    resExpr = resolverExpresion(nodoExpresion, ambiente, nombreClase, nombreMetodo, tabla).val;
                    ambiente.salirAmbito();
                }
                
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


                case Constantes.INSTANCIA_OPCIONES:
                    {
                        Matriz nuevaMatriz = new Matriz();
                        elementoRetorno r = new elementoRetorno();
                        r.val = new Valor(Constantes.OPCIONES, nuevaMatriz);
                        return r;
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

                        if (valTemporal.valor is Variable)
                        {
                            Variable v = (Variable)valTemporal.valor;
                            r.val = v.valor;
                        }
                        else if (valTemporal.valor is Objeto)
                        {
                            r.val = valTemporal;
                        }
                        else if (valTemporal.valor is Arreglo)
                        {

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
                    if (nodoParametros != null)
                    {
                        asignarSimbolo(nomParTemp, nodoParametros.ChildNodes[0].ChildNodes[k], temporalVal, simbTemp, ambiente, nombreClase, nombreMetodo, tabla);
                    }
                    else
                    {
                        asignarSimbolo(nomParTemp, null, temporalVal, simbTemp, ambiente, nombreClase, nombreMetodo, tabla);
                    }
                    
                }
            }
            else
            {
                if (nodo != null)
                {
                    Constantes.erroresEjecucion.errorSemantico(nodo, "No se puede realizar la llamada, no encajan los numero de parametros ");

                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico("No se puede realizar la llamada, no encajan los numero de parametros ");
                }
               
            }

        }


        private elementoRetorno resolverInstancia(ParseTreeNode nodo, Contexto ambiente, String nombreClase, String nombreMetodo, tablaSimbolos tabla)
        {
            //INSTANCIA.Rule= ToTerm(Constantes.NUEVO)+ TIPO_DATOS+ PARAMETROS_LLAMADA;
            elementoRetorno ret = new elementoRetorno();
            ret.val = new Valor(Constantes.NULO, Constantes.NULO);
            string tipoInstancia = nodo.ChildNodes[0].ChildNodes[0].Token.ValueString;
            ParseTreeNode nodoParametros = nodo.ChildNodes[1];
            List<Valor> valoresParametros = resolviendoParametros(nodoParametros, ambiente, nombreClase, nombreMetodo, temporalParametros);
            string cadParametros = obtenerCadenaParametros(valoresParametros);

            Clase claseTemporal;
            for (int i = 0; i < claseArchivo.size(); i++)
            {
                claseTemporal = claseArchivo.get(i);
                if (claseTemporal.nombreClase.Equals(tipoInstancia, StringComparison.CurrentCultureIgnoreCase))
                {
                    Funcion funBuscada = this.claseArchivo.obtenerFuncion(tipoInstancia, tipoInstancia, cadParametros, valoresParametros.Count);
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
            List<Valor> valoresParametros = resolviendoParametros(nodoParametros, ambiente, nombreClase, nombreMetodo, temporalParametros);
            string cadParametros = "";
            Valor x;
            for (int i = 0; i < valoresParametros.Count; i++)
            {
                x = valoresParametros.ElementAt(i);
                cadParametros += x.tipo;
            }

            Funcion funBuscada = this.claseArchivo.obtenerFuncion(nombreClase, nombreFuncion, cadParametros, valoresParametros.Count);
            if (funBuscada != null)
            {
                tabla.crearNuevoAmbito(nombreFuncion);
                ambiente.addAmbito(nombreFuncion);

                //ingresando el return
                if (funBuscada.tipo.Equals(Constantes.VACIO, StringComparison.CurrentCultureIgnoreCase))
                {

                }
                else
                {
                    if (esObjecto(funBuscada.tipo))
                    {
                        declaraRetornoObjeto("retorno", funBuscada.tipo, nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        // Objeto nuevoObj = new Objeto("retorno", funBuscada.tipo, ambiente.getAmbito(), false);

                        // tabla.insertarSimbolo(nuevoObj);
                    }
                    else
                    {
                        Variable nuevaVar = new Variable("retorno", funBuscada.tipo, ambiente.getAmbito(), false);
                        tabla.insertarSimbolo(nuevaVar,14);

                    }
                }
                

                ParseTreeNode nodoParametrosDecla = funBuscada.obtenerNodoParametros();
                declararAsignarParametrosLlamada(valoresParametros, nodo, nodoParametrosDecla, nodoParametros, ambiente, nombreClase, nombreMetodo, tabla);

                ret = evaluarArbol(funBuscada.cuerpoFuncion, ambiente, nombreClase, nombreMetodo, tabla, ret);

                if (funBuscada.esFormulario)
                {
                    guardarRes();

                }

                Simbolo simb = tabla.buscarSimbolo("retorno", ambiente);
               
                if (simb != null)
                {
                    if (simb is Objeto)
                    {
                        ret.banderaRetorno = false;
                        ret.val = new Valor(simb.tipo, simb);

                    }
                    else if (simb is Variable)
                    {
                        ret.banderaRetorno = false;
                        ret.val = simb.valor;

                    }
                    else if (simb is Arreglo)
                    {

                    }
                    else
                    {
                        ret.banderaRetorno = false;
                        ret.val = simb.valor;

                    }
                    

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


        private void guardarRes()
        {

            DialogResult resultado = new DialogResult();
            FormResultados f = new FormResultados();
            resultado = f.ShowDialog();
            if (resultado == DialogResult.OK)
            {
                String result = FormResultados.nombreF+"";
                String arbolDerivacion = rutaCarpeta + "\\" + result + ".txt";
                System.IO.File.WriteAllText(arbolDerivacion, cadenaRespuestas);

                string path = @"c:\Reportes\"+result+".html";
                if (!File.Exists(path))
                {
                    string createText = cadenaRespuestasHtml;
                    File.WriteAllText(path, createText);
                }
            }
            else if (resultado == DialogResult.Cancel)
            {
                String arbolDerivacion = rutaCarpeta + "\\Formulario" + contadorF + ".txt";
                contadorF++;
                System.IO.File.WriteAllText(arbolDerivacion, cadenaRespuestas);
                string path = @"c:\Reportes\Formulario" + contadorF + ".html";
                if (!File.Exists(path))
                {
                    string createText = cadenaRespuestasHtml;
                    File.WriteAllText(path, createText);
                }
               
            }
        }

        private void declaraRetornoObjeto(string nombre, string tipo, ParseTreeNode nodo, Contexto ambiente, string nombreClase, string nombreMetodo, tablaSimbolos tabla)
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
                lTemporal.lAtributos = cambiarAmbito(ambiente.getAmbito(), lTemporal.lAtributos);
                for (int j = 0; j < lTemporal.lAtributos.Count; j++)
                {
                    string rutaTemp = "";
                    atriTemp = lTemporal.lAtributos.ElementAt(j);
                    valoresRuta = atriTemp.rutaAcc.Split('/');
                    valoresRuta[0] = ambiente.getAmbito();// +"/" + nombre;
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
                    string ambitoTemporal = ambiente.getAmbito();// +"/" + nombre; ;
                    atriTemp.ambito = ambitoTemporal; //
                    nuevo.variablesObjeto.insertarSimbolo(atriTemp,16);
                    nuevo.rutaAcc = ambitoTemporal;
                    /*---- buscando nuevamente el simbolo para poderlo asignar a la tabla*/
                    Contexto c = new Contexto();
                    c.llenarAmbitos(ambitoTemporal);
                    if (atriTemp.nodoExpresionValor != null)
                    {
                        elementoRetorno r = new elementoRetorno();
                        Simbolo s = nuevo.variablesObjeto.buscarSimbolo(atriTemp.nombre, c);
                        if (s != null)
                        {
                            if (s is Objeto && atriTemp.nodoExpresionValor.Term.Name.Equals(Constantes.INSTANCIA, StringComparison.CurrentCultureIgnoreCase))
                            {
                                Objeto ob = (Objeto)s;
                                temporalParametros = nuevo.variablesObjeto;
                                r = resolverExpresion(atriTemp.nodoExpresionValor, c, nombreClase, nombreMetodo, ob.variablesObjeto);
                            }
                            else
                            {
                                temporalParametros = nuevo.variablesObjeto;
                                r = resolverExpresion(atriTemp.nodoExpresionValor, c, nombreClase, nombreMetodo, nuevo.variablesObjeto);
                            }
                            asignarSimbolo(atriTemp.nombre, nodo, r.val, s, c, nombreClase, nombreMetodo, nuevo.variablesObjeto);
                        }
                        else
                        {
                            Constantes.erroresEjecucion.errorSemantico(nodo, "No se ha podido encontrar el atributo " + atriTemp.nombre);

                        }
                    }
                }


            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico("No se pudo declarar la variable de nombre " + nombre + ", de tipo " + tipo + ", no existe esa clase");
            }
        }





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
                val= val + Convert.ToInt32(c);
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
            Valor v1 = resolverExpresion(nodo.ChildNodes[1], ambiente, nombreClase, nombreMetodo, tabla).val;
            Valor ret = new Valor();
            if (esCadena(v1))
            {
                string cadena = v1.valor.ToString();
                ret = new Valor(Constantes.ENTERO, cadena.Length);
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
                ret = new Valor(Constantes.ENTERO, 1);
            }
            else if (esObjecto(v1.tipo))
            {
                Objeto obj = (Objeto)v1.valor;
                int n = obj.variablesObjeto.listaSimbolos.Peek().variablesAmbito.Count;
                ret = new Valor(Constantes.ENTERO, n);
            }
            else
            {
                ret = new Valor();
            }
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
            else if (esFecha(v1) && esFecha(v2))
            {
                Fecha f = (Fecha)v1.valor;
                Fecha f2 = (Fecha)v2.valor;
                if (f.fechaReal < f2.fechaReal)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esFecha(v1) && esFechaHora(v2))
            {
                Fecha f = (Fecha)v1.valor;
                FechaHora f2 = (FechaHora)v2.valor;
                DateTime tiempoR2 = new DateTime(f2.fechaA.fechaReal.Year, f2.fechaA.fechaReal.Month, f2.fechaA.fechaReal.Day, f2.horaA.horaReal.Hour, f2.horaA.horaReal.Minute, f2.horaA.horaReal.Second);
                if (f.fechaReal < tiempoR2)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esFechaHora(v1) && esFecha(v2))
            {
                Fecha f = (Fecha)v2.valor;
                FechaHora f2 = (FechaHora)v1.valor;
                DateTime tiempoR2 = new DateTime(f2.fechaA.fechaReal.Year, f2.fechaA.fechaReal.Month, f2.fechaA.fechaReal.Day, f2.horaA.horaReal.Hour, f2.horaA.horaReal.Minute, f2.horaA.horaReal.Second);
                if (tiempoR2< f.fechaReal)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esFechaHora(v1) && esFechaHora(v2))
            {
                FechaHora f = (FechaHora)v1.valor;
                FechaHora f2 = (FechaHora)v2.valor;
                DateTime tiempoR1 = new DateTime(f.fechaA.fechaReal.Year, f.fechaA.fechaReal.Month, f.fechaA.fechaReal.Day, f.horaA.horaReal.Hour, f.horaA.horaReal.Minute, f.horaA.horaReal.Second);
                DateTime tiempoR2 = new DateTime(f2.fechaA.fechaReal.Year, f2.fechaA.fechaReal.Month, f2.fechaA.fechaReal.Day, f2.horaA.horaReal.Hour, f2.horaA.horaReal.Minute, f2.horaA.horaReal.Second);
                if (tiempoR1< tiempoR2)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esHora(v1) && esHora(v2))
            {
                Hora h1 = (Hora)v1.valor;
                Hora h2 = (Hora)v2.valor;
                if (h1.horaReal < h2.horaReal)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
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
            else if (esFecha(v1) && esFecha(v2))
            {
                Fecha f = (Fecha)v1.valor;
                Fecha f2 = (Fecha)v2.valor;
                if (f.fechaReal >= f2.fechaReal)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esFecha(v1) && esFechaHora(v2))
            {
                Fecha f = (Fecha)v1.valor;
                FechaHora f2 = (FechaHora)v2.valor;
                DateTime tiempoR2 = new DateTime(f2.fechaA.fechaReal.Year, f2.fechaA.fechaReal.Month, f2.fechaA.fechaReal.Day, f2.horaA.horaReal.Hour, f2.horaA.horaReal.Minute, f2.horaA.horaReal.Second);
                if (f.fechaReal >= tiempoR2)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esFechaHora(v1) && esFecha(v2))
            {
                Fecha f = (Fecha)v2.valor;
                FechaHora f2 = (FechaHora)v1.valor;
                DateTime tiempoR2 = new DateTime(f2.fechaA.fechaReal.Year, f2.fechaA.fechaReal.Month, f2.fechaA.fechaReal.Day, f2.horaA.horaReal.Hour, f2.horaA.horaReal.Minute, f2.horaA.horaReal.Second);
                if (tiempoR2 >= f.fechaReal)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esFechaHora(v1) && esFechaHora(v2))
            {
                FechaHora f = (FechaHora)v1.valor;
                FechaHora f2 = (FechaHora)v2.valor;
                DateTime tiempoR1 = new DateTime(f.fechaA.fechaReal.Year, f.fechaA.fechaReal.Month, f.fechaA.fechaReal.Day, f.horaA.horaReal.Hour, f.horaA.horaReal.Minute, f.horaA.horaReal.Second);
                DateTime tiempoR2 = new DateTime(f2.fechaA.fechaReal.Year, f2.fechaA.fechaReal.Month, f2.fechaA.fechaReal.Day, f2.horaA.horaReal.Hour, f2.horaA.horaReal.Minute, f2.horaA.horaReal.Second);
                if (tiempoR1 >= tiempoR2)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esHora(v1) && esHora(v2))
            {
                Hora h1 = (Hora)v1.valor;
                Hora h2 = (Hora)v2.valor;
                if (h1.horaReal >= h2.horaReal)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
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
            else if (esFecha(v1) && esFecha(v2))
            {
                Fecha f = (Fecha)v1.valor;
                Fecha f2 = (Fecha)v2.valor;
                if (f.fechaReal > f2.fechaReal)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esFecha(v1) && esFechaHora(v2))
            {
                Fecha f = (Fecha)v1.valor;
                FechaHora f2 = (FechaHora)v2.valor;
                DateTime tiempoR2 = new DateTime(f2.fechaA.fechaReal.Year, f2.fechaA.fechaReal.Month, f2.fechaA.fechaReal.Day, f2.horaA.horaReal.Hour, f2.horaA.horaReal.Minute, f2.horaA.horaReal.Second);
                if (f.fechaReal > tiempoR2)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esFechaHora(v1) && esFecha(v2))
            {
                Fecha f = (Fecha)v2.valor;
                FechaHora f2 = (FechaHora)v1.valor;
                DateTime tiempoR2 = new DateTime(f2.fechaA.fechaReal.Year, f2.fechaA.fechaReal.Month, f2.fechaA.fechaReal.Day, f2.horaA.horaReal.Hour, f2.horaA.horaReal.Minute, f2.horaA.horaReal.Second);
                if (tiempoR2 > f.fechaReal)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esFechaHora(v1) && esFechaHora(v2))
            {
                FechaHora f = (FechaHora)v1.valor;
                FechaHora f2 = (FechaHora)v2.valor;
                DateTime tiempoR1 = new DateTime(f.fechaA.fechaReal.Year, f.fechaA.fechaReal.Month, f.fechaA.fechaReal.Day, f.horaA.horaReal.Hour, f.horaA.horaReal.Minute, f.horaA.horaReal.Second);
                DateTime tiempoR2 = new DateTime(f2.fechaA.fechaReal.Year, f2.fechaA.fechaReal.Month, f2.fechaA.fechaReal.Day, f2.horaA.horaReal.Hour, f2.horaA.horaReal.Minute, f2.horaA.horaReal.Second);
                if (tiempoR1 > tiempoR2)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esHora(v1) && esHora(v2))
            {
                Hora h1 = (Hora)v1.valor;
                Hora h2 = (Hora)v2.valor;
                if (h1.horaReal > h2.horaReal)
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
            else if (esFecha(v1) && esFecha(v2))
            {
                Fecha f = (Fecha)v1.valor;
                Fecha f2 = (Fecha)v2.valor;
                if (f.fechaReal <= f2.fechaReal)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esFecha(v1) && esFechaHora(v2))
            {
                Fecha f = (Fecha)v1.valor;
                FechaHora f2 = (FechaHora)v2.valor;
                DateTime tiempoR2 = new DateTime(f2.fechaA.fechaReal.Year, f2.fechaA.fechaReal.Month, f2.fechaA.fechaReal.Day, f2.horaA.horaReal.Hour, f2.horaA.horaReal.Minute, f2.horaA.horaReal.Second);
                if (f.fechaReal <= tiempoR2)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esFechaHora(v1) && esFecha(v2))
            {
                Fecha f = (Fecha)v2.valor;
                FechaHora f2 = (FechaHora)v1.valor;
                DateTime tiempoR2 = new DateTime(f2.fechaA.fechaReal.Year, f2.fechaA.fechaReal.Month, f2.fechaA.fechaReal.Day, f2.horaA.horaReal.Hour, f2.horaA.horaReal.Minute, f2.horaA.horaReal.Second);
                if (tiempoR2 <= f.fechaReal)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esFechaHora(v1) && esFechaHora(v2))
            {
                FechaHora f = (FechaHora)v1.valor;
                FechaHora f2 = (FechaHora)v2.valor;
                DateTime tiempoR1 = new DateTime(f.fechaA.fechaReal.Year, f.fechaA.fechaReal.Month, f.fechaA.fechaReal.Day, f.horaA.horaReal.Hour, f.horaA.horaReal.Minute, f.horaA.horaReal.Second);
                DateTime tiempoR2 = new DateTime(f2.fechaA.fechaReal.Year, f2.fechaA.fechaReal.Month, f2.fechaA.fechaReal.Day, f2.horaA.horaReal.Hour, f2.horaA.horaReal.Minute, f2.horaA.horaReal.Second);
                if (tiempoR1 <= tiempoR2)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esHora(v1) && esHora(v2))
            {
                Hora h1 = (Hora)v1.valor;
                Hora h2 = (Hora)v2.valor;
                if (h1.horaReal <= h2.horaReal)
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
                if (getCadena(v1).Equals(getCadena(v2)))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esFecha(v1) && esFecha(v2))
            {
                Fecha f = (Fecha)v1.valor;
                Fecha f2 = (Fecha)v2.valor;
                if (f.fechaReal == f2.fechaReal)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esFecha(v1) && esFechaHora(v2))
            {
                Fecha f = (Fecha)v1.valor;
                FechaHora f2 = (FechaHora)v2.valor;
                DateTime tiempoR2 = new DateTime(f2.fechaA.fechaReal.Year, f2.fechaA.fechaReal.Month, f2.fechaA.fechaReal.Day, f2.horaA.horaReal.Hour, f2.horaA.horaReal.Minute, f2.horaA.horaReal.Second);
                if (f.fechaReal == tiempoR2)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
                
            }
            else if (esFechaHora(v1) && esFecha(v2))
            {
                Fecha f = (Fecha)v2.valor;
                FechaHora f2 = (FechaHora)v1.valor;
                DateTime tiempoR2 = new DateTime(f2.fechaA.fechaReal.Year, f2.fechaA.fechaReal.Month, f2.fechaA.fechaReal.Day, f2.horaA.horaReal.Hour, f2.horaA.horaReal.Minute, f2.horaA.horaReal.Second);
                if (f.fechaReal == tiempoR2)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esFechaHora(v1) && esFechaHora(v2))
            {
                FechaHora f = (FechaHora)v1.valor;
                FechaHora f2 = (FechaHora)v2.valor;
                DateTime tiempoR1 = new DateTime(f.fechaA.fechaReal.Year, f.fechaA.fechaReal.Month, f.fechaA.fechaReal.Day, f.horaA.horaReal.Hour, f.horaA.horaReal.Minute, f.horaA.horaReal.Second);
                DateTime tiempoR2 = new DateTime(f2.fechaA.fechaReal.Year, f2.fechaA.fechaReal.Month, f2.fechaA.fechaReal.Day, f2.horaA.horaReal.Hour, f2.horaA.horaReal.Minute, f2.horaA.horaReal.Second);
                if (tiempoR1 == tiempoR2)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esHora(v1) && esHora(v2))
            {
                Hora h1 = (Hora)v1.valor;
                Hora h2 = (Hora)v2.valor;
                if (h1.horaReal == h2.horaReal)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esBool(v1) && esBool(v2))
            {
                int a = getBooleanoNumero(v1);
                int b = getBooleanoNumero(v2);
                if (a == b)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esNulo(v1) && !esNulo(v2))
            {
                return resp;
            }
            else if (esNulo(v1) && esNulo(v2))
            {
                resp.valor = Constantes.VERDADERO;
                return resp;
            }
            else if (!esNulo(v1) && esNulo(v2))
            {
                return resp;
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
                if (getEntero(v1)!= getEntero(v2))
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
                return resp;
            }
            else if (esFecha(v1) && esFecha(v2))
            {
                Fecha f = (Fecha)v1.valor;
                Fecha f2 = (Fecha)v2.valor;
                if (f.fechaReal != f2.fechaReal)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esFecha(v1) && esFechaHora(v2))
            {
                Fecha f = (Fecha)v1.valor;
                FechaHora f2 = (FechaHora)v2.valor;
                DateTime tiempoR2 = new DateTime(f2.fechaA.fechaReal.Year, f2.fechaA.fechaReal.Month, f2.fechaA.fechaReal.Day, f2.horaA.horaReal.Hour, f2.horaA.horaReal.Minute, f2.horaA.horaReal.Second);
                if (f.fechaReal != tiempoR2)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esFechaHora(v1) && esFecha(v2))
            {
                Fecha f = (Fecha)v2.valor;
                FechaHora f2 = (FechaHora)v1.valor;
                DateTime tiempoR2 = new DateTime(f2.fechaA.fechaReal.Year, f2.fechaA.fechaReal.Month, f2.fechaA.fechaReal.Day, f2.horaA.horaReal.Hour, f2.horaA.horaReal.Minute, f2.horaA.horaReal.Second);
                if (f.fechaReal != tiempoR2)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esFechaHora(v1) && esFechaHora(v2))
            {
                FechaHora f = (FechaHora)v1.valor;
                FechaHora f2 = (FechaHora)v2.valor;
                DateTime tiempoR1 = new DateTime(f.fechaA.fechaReal.Year, f.fechaA.fechaReal.Month, f.fechaA.fechaReal.Day, f.horaA.horaReal.Hour, f.horaA.horaReal.Minute, f.horaA.horaReal.Second);
                DateTime tiempoR2 = new DateTime(f2.fechaA.fechaReal.Year, f2.fechaA.fechaReal.Month, f2.fechaA.fechaReal.Day, f2.horaA.horaReal.Hour, f2.horaA.horaReal.Minute, f2.horaA.horaReal.Second);
                if (tiempoR1 != tiempoR2)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esHora(v1) && esHora(v2))
            {
                Hora h1 = (Hora)v1.valor;
                Hora h2 = (Hora)v2.valor;
                if (h1.horaReal != h2.horaReal)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esBool(v1) && esBool(v2))
            {
                int a = getBooleanoNumero(v1);
                int b = getBooleanoNumero(v2);
                if (a != b)
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esNulo(v1) && !esNulo(v2))
            {
                resp.valor = Constantes.VERDADERO;
                return resp;
            }
            else if (esNulo(v1) && esNulo(v2))
            {
                return resp;
            }
            else if (!esNulo(v1) && esNulo(v2))
            {
                resp.valor = Constantes.VERDADERO;
                return resp;
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
                double a = getBooleanoNumero(v1);
                double b = getEntero(v2);
                if (!esCero(b))
                {
                    double c = a / b;
                    resp = new Valor(Constantes.DECIMAL, c);
                    return resp;
                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico("No es valido la division entre los valores  " + a + "  entre " + b);

                }
                
            }
            else if (esBool(v1) && esDecimal(v2))
            {
                double a = getBooleanoNumero(v1);
                double b = getDecimal(v2);
                if (!esCero(b))
                {
                    double c = a / b;
                    resp = new Valor(Constantes.DECIMAL, c);
                    return resp;

                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico("No es valido la division entre los valores  " + a + "  entre " + b);
                }
               

            }
            else if (esEntero(v1) && esEntero(v2))
            {
                double a = getEntero(v1);
                double b = getEntero(v2);
                if (!esCero(b))
                {
                    double c = a / b;
                    resp = new Valor(Constantes.DECIMAL, c);
                    return resp;

                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico("No es valido la division entre los valores  " + a + "  entre " + b);
                }
                

            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                double a = getEntero(v1);
                double b = getDecimal(v2);
                if (!esCero(b))
                {
                    double c = a / b;
                    resp = new Valor(Constantes.DECIMAL, c);
                    return resp;

                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico("No es valido la division entre los valores  " + a + "  entre " + b);
                }
                

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                double a = getDecimal(v1);
                double b = getEntero(v2);
                if (!esCero(b))
                {
                    double c = a / b;
                    resp = new Valor(Constantes.DECIMAL, c);
                    return resp;

                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico("No es valido la division entre los valores  " + a + "  entre " + b);
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
                    Constantes.erroresEjecucion.errorSemantico("No es valido la division entre los valores  " + a + "  entre " + b);

                }
                
            }
            else if (esDecimal(v1) && esBool(v2))
            {
                double a = getDecimal(v1);
                double b = getBooleanoNumero(v2);
                if (!esCero(b))
                {
                    double c = a / b;
                    resp = new Valor(Constantes.DECIMAL, c);
                    return resp;

                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico("No es valido la division entre los valores  " + a + "  entre " + b);
                }
                

            }
            else if (esEntero(v1) && esBool(v2))
            {
                double b = getBooleanoNumero(v2);
                double a = getEntero(v1);
                if (!esCero(b))
                {
                    double c = a / b;
                    resp = new Valor(Constantes.DECIMAL, c);
                    return resp;

                }
                else
                {
                    Constantes.erroresEjecucion.errorSemantico("No es valido la division entre los valores  " + a + "  entre " + b);
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
                string b1 = getBooleanoLetra(v1);
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
                tipo.ToLower().Equals(Constantes.HORA.ToLower())|| 
                tipo.ToLower().Equals(Constantes.OPCIONES.ToLower())||
                tipo.ToLower().Equals(Constantes.NULO.ToLower()))
            {
                return false;
            }
            return true;
        }

        private bool esNulo(Valor v)
        {
            if(esObjecto(v.tipo)){
                Objeto v2 = (Objeto)v.valor;
                return v2.valor.tipo.ToLower().Equals(Constantes.NULO.ToLower());
            }
            else
            {
                return v.tipo.ToLower().Equals(Constantes.NULO.ToLower());
            }
            
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


        private List<Simbolo> cambiarAmbito(string ambito, List<Simbolo> simbs)
        {
            Simbolo temp;
            for (int i = 0; i < simbs.Count; i++)
            {
                temp = simbs.ElementAt(i);
                temp.ambito = ambito;
                if (temp is Objeto)
                {
                    Objeto c = (Objeto)temp;
                    c.variablesObjeto.cambiarAmbito(ambito);
                    temp = c;
                }
            }

            return simbs;
        }

        private bool esLista(String tipo)
        {
            if (tipo.ToLower().Equals(Constantes.OPCIONES.ToLower()))
            {
                return true;
            }
            return false;
        }



        private void respondeUsuario( tablaSimbolos tabla, Valor respuestaUsuario, string nombreFuncion, string nombreClase, Contexto ambiente)
        {

            Clase c = claseArchivo.obtenerClase(nombreClase);
            elementoRetorno ret = new elementoRetorno();
            if (c != null)
            {
                Funcion fbusC = c.obtenerFuncion(nombreFuncion, respuestaUsuario.tipo);
                if (fbusC != null)
                {
                    List<Valor> valoresParametros = new List<Valor>();
                    valoresParametros.Add(respuestaUsuario);
                    tabla.crearNuevoAmbito(nombreFuncion);
                    ambiente.addAmbito(nombreFuncion);
                    ParseTreeNode nodoParametrosDecla = fbusC.obtenerNodoParametros();
                    declararAsignarParametrosLlamada(valoresParametros, null, nodoParametrosDecla, null, ambiente, nombreClase, nombreFuncion, tabla);
                    ret = evaluarArbol(fbusC.cuerpoFuncion, ambiente, nombreClase, nombreFuncion, tabla, ret);
                    ambiente.salirAmbito();
                    tabla.salirAmbiente();
                }
            }

        }


        //buscar la funcion respuesta de una pregunta   
        private void responderPregunta(ParseTreeNode nodo, Contexto ambiente, string nombreClase, String nombreMetodo, tablaSimbolos tabla, elementoRetorno ret)
        {
            ParseTreeNode nodoId = nodo.ChildNodes[0];
            ParseTreeNode nodoParametros = nodo.ChildNodes[1];
            string nombreFuncion = nodoId.Token.ValueString;
            int noParametros = nodoParametros.ChildNodes[0].ChildNodes.Count;
            List<Valor> valoresParametros = resolviendoParametros(nodoParametros, ambiente, nombreClase, nombreMetodo, temporalParametros);
            string cadParametros = "";
            Valor x;
            for (int i = 0; i < valoresParametros.Count; i++)
            {
                x = valoresParametros.ElementAt(i);
                cadParametros += x.tipo;
            }

            Funcion funBuscada = this.claseArchivo.obtenerFuncion(nombreClase, nombreFuncion, cadParametros, valoresParametros.Count);
            if (funBuscada != null)
            {
                tabla.crearNuevoAmbito(nombreFuncion);
                ambiente.addAmbito(nombreFuncion);

                //ingresando el return
                if (funBuscada.tipo.Equals(Constantes.VACIO, StringComparison.CurrentCultureIgnoreCase))
                {

                }
                else
                {
                    if (esObjecto(funBuscada.tipo))
                    {
                        declaraRetornoObjeto("retorno", funBuscada.tipo, nodo, ambiente, nombreClase, nombreMetodo, tabla);
                        // Objeto nuevoObj = new Objeto("retorno", funBuscada.tipo, ambiente.getAmbito(), false);

                        // tabla.insertarSimbolo(nuevoObj);
                    }
                    else
                    {
                        Variable nuevaVar = new Variable("retorno", funBuscada.tipo, ambiente.getAmbito(), false);
                        tabla.insertarSimbolo(nuevaVar,17);

                    }
                }


                ParseTreeNode nodoParametrosDecla = funBuscada.obtenerNodoParametros();
                declararAsignarParametrosLlamada(valoresParametros, nodo, nodoParametrosDecla, nodoParametros, ambiente, nombreClase, nombreMetodo, tabla);

                ret = evaluarArbol(funBuscada.cuerpoFuncion, ambiente, nombreClase, nombreMetodo, tabla, ret);
                Simbolo simb = tabla.buscarSimbolo("retorno", ambiente);

                if (simb != null)
                {
                    if (simb is Objeto)
                    {
                        ret.val = new Valor(simb.tipo, simb);

                    }
                    else if (simb is Variable)
                    {
                        ret.val = simb.valor;

                    }
                    else if (simb is Arreglo)
                    {

                    }
                    else
                    {
                        ret.val = simb.valor;

                    }


                }
                ambiente.salirAmbito();
                tabla.salirAmbiente();

            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico(nodo, "La funcion " + nombreFuncion + ", no existe en la clase actual " + nombreClase);
                tabla.mostrarSimbolos();
            }
            
        }



    }
}
