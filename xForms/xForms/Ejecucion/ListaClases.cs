using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xForms.Tabla_Simbolos;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using System.IO;
using xForms.Analizar;
using xForms.Formularios;

namespace xForms.Ejecucion
{
    class ListaClases
    {
       public List<Clase> lClases;
       public  String rutaArchivo;
       public string rutaCarpeta;
       public List<ListaClases> archivosImportados;
       public ElementosFormulario fomrs;

       public Clase get(int i)
       {
           return this.lClases.ElementAt(i);
           
       }
        public ListaClases(string rutaA, string rutaC)
        {
            this.archivosImportados = new List<ListaClases>();
            this.lClases = new List<Clase>();
            this.rutaArchivo = rutaA;
            rutaCarpeta = rutaC;
            this.fomrs = new ElementosFormulario();
          //  importaciones = new List<string>();
        }


        public bool hayPrincipalClasesArchivoPrincipal()
        {
            Clase temp;
            for (int i = 0; i < this.lClases.Count; i++)
            {
                temp = this.lClases.ElementAt(i);
                if (temp.perteneceArchivoPrincipal)
                {
                    if (temp.tienePrincipal())
                    {
                        return true;
                    }
                }
            }
            return false;
        }



     

        public void insertarClase(Clase c)
        {
           
            if (!existeClase(c.nombreClase))
            {
                this.lClases.Add(c);
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico("No se ha podido ingresar la clase " + c.nombreClase + ", ya existe");
            }           
        }

        private bool existeClase(string nombreClase)
        {
            Clase temp;
            for (int i = 0; i < this.lClases.Count; i++)
            {
                temp = lClases.ElementAt(i);
                if (temp.nombreClase.Equals(nombreClase, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }


            return false;
        }



        public int size()
        {
            return this.lClases.Count;
        }


        public Funcion obtenerFuncion(string nombreClase, string nombreFuncion, string cadParametros)
        {
            Clase temp;
            for (int i = 0; i < this.lClases.Count; i++)
            {
                temp = this.lClases.ElementAt(i);
                if (temp.nombreClase.ToLower().Equals(nombreClase.ToLower()))
                {
                    return temp.obtenerFuncion(nombreFuncion, cadParametros);
                }
            }
            return null;
        }

        public void instanciarAtributosClase()
        {
            Clase temp;
            Contexto ambitos;
            Simbolo atriTemp;
            for (int i = 0; i < this.lClases.Count; i++)
            {
                ambitos = new Contexto();
                temp = this.lClases.ElementAt(i);
                ambitos.addAmbito(temp.nombreClase);
                int h = temp.atributosClase.lAtributos.Count;
                for (int j = 0; j < h; j++)
                {
                    atriTemp = temp.atributosClase.lAtributos.ElementAt(j);
                    if (esObjecto(atriTemp.tipo))
                    {
                        ambitos.addAmbito(atriTemp.nombre);
                        asignarAtributos(atriTemp.tipo, temp.atributosClase, ambitos);
                        ambitos.salirAmbito();
                    }
                }
            }
        }

        private void asignarAtributos(string nomClase, ListaAtributos atributosClase, Contexto ambiente)
        {
            Clase claseTemp;
            Simbolo atriTemp;
            List<Simbolo> atributosTemporales;
            for (int i = 0; i < lClases.Count; i++)
            {
                claseTemp = lClases.ElementAt(i);
                if (claseTemp.nombreClase.Equals(nomClase, StringComparison.CurrentCultureIgnoreCase))
                {
                    atributosTemporales = claseTemp.atributosClase.clonarLista();
                    for (int j = 0; j < atributosTemporales.Count; j++)
                    {
                        atriTemp = atributosTemporales.ElementAt(j);

                        if (atriTemp.ambito.Equals(nomClase, StringComparison.InvariantCultureIgnoreCase))
                        {
                            atriTemp.rutaAcc= ambiente.getAmbito();
                            atriTemp.ambito = ambiente.getAmbito();
                            atributosClase.insertarAtributo(atriTemp);
                            if (esObjecto(atriTemp.tipo))
                            {
                                ambiente.addAmbito(atriTemp.nombre);
                                asignarAtributos(atriTemp.tipo, atributosClase, ambiente);
                                ambiente.salirAmbito();
                            }

                        }
                        else
                        {
                            continue;
                        }
                        
                       
                    }
                }
            }
        }



        private bool existe(string ruta)
        {
            if (this.rutaArchivo.Equals(ruta, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            else if (this.archivosImportados.Count > 0)
            {
                ListaClases temp;
                for (int i = 0; i < this.archivosImportados.Count; i++)
                {
                    temp = archivosImportados.ElementAt(i);
                    if (temp.rutaArchivo.Equals(ruta, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }
                    
                }
            }

            return false;
        }


        private ParseTreeNode obtenerRaizArchivo(string ruta)
        {
            string contenidoArchivo = obtenerContenidoArchivo(ruta);
            if (!(contenidoArchivo.Equals("")))
            {
                Arbol b = new Arbol(rutaCarpeta, ruta);
                return b.parseArchivoImportado(contenidoArchivo, ruta);
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico("Archivo " + ruta + " no valido");
            }

            return null;
        }

        private string obtenerContenidoArchivo(string ruta)
        {
            string contenido = "";
            try {
                StreamReader objReader = new StreamReader(ruta);
                string sLine = "";
                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                        contenido += sLine + "\n";
                }
                objReader.Close();

                return contenido;

            }
            catch (Exception e)
            {
                Constantes.erroresEjecucion.errorSemantico("No existe el archivo " + ruta);
                return "";
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



        public Clase obtenerClase(String nombreClase)
        {
            Clase temp;
            for (int i = 0; i < this.lClases.Count; i++)
            {
                temp = this.lClases.ElementAt(i);
                if (temp.nombreClase.Equals(nombreClase, StringComparison.CurrentCultureIgnoreCase))
                {
                    return temp;
                }
            }
            return null;
        }




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
                        string nuevaRuta = rutaCarpeta+"\\"+nombreArchivo+".xform";
                        ListaClases claseImportada = new ListaClases(nuevaRuta, rutaCarpeta);
                        if (!existe(nuevaRuta))
                        {
                            ParseTreeNode nodoRaiz = obtenerRaizArchivo(nuevaRuta);
                            if (nodoRaiz != null)
                            {
                                claseImportada.generarClases(nodoRaiz);
                                archivosImportados.Add(claseImportada);
                            }
                        }
                        
                        /*
                        Importar nuevo = new Importar(nombreArchivo);
                        importaciones.Add(nombreArchivo);*/
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
                            insertarClase(c);
                        }
                        else
                        {
                            nombre = nodo.ChildNodes[0].Token.ValueString;
                            visibilidad = nodo.ChildNodes[1].ChildNodes[0].Token.ValueString;
                            cuerpoClase = nodo.ChildNodes[3];
                            herencia = nodo.ChildNodes[2].Token.ValueString;
                            Clase c = new Clase(nombre, herencia, visibilidad, cuerpoClase);
                            insertarClase(c);

                        }
                        break;
                    }
            }

            

        }



        /*---------- Agregar los atributos a los objetos que sean atributos de una clase ------------*/

   
        public void iniciarAtributos()
        {
            Clase claseTemporal;
            Contexto ambitos;
            Simbolo atributoTemporal;
            for (int i = 0; i < lClases.Count; i++)
            {
                claseTemporal = lClases.ElementAt(i);
                ambitos = new Contexto();
                ambitos.addAmbito(claseTemporal.nombreClase);
                int sizeAtributos = claseTemporal.atributosClase.lAtributos.Count;
                for (int j = 0; j < sizeAtributos; j++)
                {
                    atributoTemporal = claseTemporal.atributosClase.lAtributos.ElementAt(j);
                    if ((esObjecto(atributoTemporal.tipo) && !(esLista(atributoTemporal.tipo))))
                    {
                        Objeto objTemp = (Objeto)atributoTemporal;
                        ambitos.addAmbito(atributoTemporal.nombre);
                        agregarAtributosTabla(objTemp.tipo, objTemp.variablesObjeto, ambitos, objTemp.nombre);
                        ambitos.salirAmbito();
                    }
                }
                
                
            }

        }



        private void agregarAtributosTabla(string nomClase, tablaSimbolos tabla, Contexto ambiente, string nombreAtributo)
        {
            Clase claseTemp = obtenerClase(nomClase);
            Simbolo atributoTemp;
            List<Simbolo> atributosClase;
            if (claseTemp != null)
            {
                atributosClase = claseTemp.atributosClase.clonarLista();
                for (int i = 0; i < atributosClase.Count; i++)
                {
                    atributoTemp = atributosClase.ElementAt(i);
                    if (atributoTemp.ambito.Equals(nomClase, StringComparison.CurrentCultureIgnoreCase))
                    {
                        atributoTemp.rutaAcc = ambiente.getAmbito();
                        atributoTemp.ambito = nomClase;
                        tabla.insertarSimbolo(atributoTemp);
                        if( (esObjecto(atributoTemp.tipo) && !(esLista(atributoTemp.tipo))))
                        {
                            Objeto obj = (Objeto)atributoTemp;
                            ambiente.addAmbito(obj.nombre);
                            agregarAtributosTabla(obj.tipo, obj.variablesObjeto, ambiente, obj.nombre);
                            ambiente.salirAmbito();
                        }
                    }
                }


            }

        }


        public void agregarPreguntas()
        {
            Clase temp;
            List<Clase> preguntasTemporals = new List<Clase>();
            for (int i = 0; i < lClases.Count; i++)
            {
                temp = lClases.ElementAt(i);
                for (int j = 0; j < temp.preguntas.Count; j++)
                {
                    preguntasTemporals.Add(temp.preguntas.ElementAt(j));
                }
            }

            for (int i = 0; i < preguntasTemporals.Count; i++)
            {
                this.lClases.Add(preguntasTemporals.ElementAt(i));
            }

        }

        public void llenarListaFormularios()
        {
            Clase temp;
            Formulario fomrTemp;
            Pregunta pregTemp;
            Agrupacion grupTemp;
            for (int i = 0; i < lClases.Count; i++)
            {
                temp = lClases.ElementAt(i);
                //para forms
                for (int j = 0; j < temp.objetosForm.formularios.Count; j++)
                {
                    fomrTemp= temp.objetosForm.formularios.ElementAt(j);
                    this.fomrs.formularios.Add(fomrTemp);
                }
                //para los grupos
                for (int j = 0; j < temp.objetosForm.grupos.Count; j++)
                {
                    grupTemp = temp.objetosForm.grupos.ElementAt(j);
                    this.fomrs.grupos.Add(grupTemp);
                }

                //para las preguntas
                for (int j = 0; j < temp.objetosForm.preguntas.Count; j++)
                {
                    pregTemp = temp.objetosForm.preguntas.ElementAt(j);
                    this.fomrs.preguntas.Add(pregTemp);
                }
            }
        }


        public List<Clase> obtenerPreguntas()
        {
            List<Clase> clases = new List<Clase>();
            Clase temp;
            for (int i = 0; i < lClases.Count; i++)
            {
                temp = lClases.ElementAt(i);
                if (temp.esPregunta)
                {
                    clases.Add(temp);
                }
                
            }
            return clases;
        }


        public Formulario buscarFormulario(string nombre)
        {
            return this.fomrs.buscarForm(nombre);
        }


        public bool esLista(String tipo)
        {
            return (tipo.Equals(Constantes.OPCIONES, StringComparison.CurrentCultureIgnoreCase));
        }

    }
}
