using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using System.IO;

namespace xForms.Ejecucion
{
    class ListaArchivos
    {

         List<ListaClases> lArchivos;
         string rutaCarpeta;

        public ListaArchivos(string carpeta)
        {
            this.lArchivos = new List<ListaClases>();
            this.rutaCarpeta = carpeta;
        }


        public void insertarArchivo(string ruta, ParseTreeNode nodoRaiz)
        {
            if (!(existeArchivo(ruta)))
            {
                ListaClases nueva = new ListaClases(ruta,rutaCarpeta);
                nueva.generarClases(nodoRaiz);
                guardarNuevoArchivo(nueva);
                Console.WriteLine("holaaa");
            }
            else
            {
                Console.WriteLine("El archivo " + ruta + "  ya fue importado anteriormente");
            }
        }


        private void guardarNuevoArchivo(ListaClases clases)
        {
            if(!(existeArchivo(clases.rutaArchivo))){
                lArchivos.Add(clases);
                if (clases.archivosImportados.Count > 0)
                {
                    ListaClases temp;
                    for (int i = 0; i < clases.archivosImportados.Count; i++)
                    {
                        temp= clases.archivosImportados.ElementAt(i);
                        guardarNuevoArchivo(temp);
                    }
                }
            }
        }


        private bool existeArchivo(string ruta)
        {
            ListaClases temp;
            for (int i = 0; i < this.lArchivos.Count; i++)
            {
                temp = lArchivos.ElementAt(i);
                if (temp.rutaArchivo.Equals(ruta, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }


           public ListaClases crearLista()
        {
            ListaClases nueva = new ListaClases("", rutaCarpeta);
            ListaClases temp;
            Clase claseTemp;
            for (int i = 0; i < this.lArchivos.Count; i++)
            {
                temp = lArchivos.ElementAt(i);
                if (i == 0)
                {
                    for (int j = 0; j < temp.lClases.Count; j++)
                    {
                        claseTemp = temp.lClases.ElementAt(j);
                        claseTemp.perteneceArchivoPrincipal = true;
                        nueva.insertarClase(claseTemp);
                    }
                }
                else
                {
                    for (int j = 0; j < temp.lClases.Count; j++)
                    {
                        claseTemp = temp.lClases.ElementAt(j);
                        if (claseTemp.visibilidad.Equals(Constantes.PUBLICO, StringComparison.CurrentCultureIgnoreCase))
                        {
                            nueva.insertarClase(claseTemp);
                        }
                        
                    }

                }
            }

            return nueva;
        }

    }
}
