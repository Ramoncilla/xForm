﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Irony.Ast;
using Irony.Parsing;
using xForms.Errores;

namespace xForms.Analizar
{
    public class Arbol
    {

        private int niv = 0;
        private Accion accion;
        private LanguageData lenguaje;
        private Parser p;
        private String path;
        

        public Arbol(string rutaCarpeta, string nombreA)
        {
            
            accion = new Accion(rutaCarpeta, nombreA);
            lenguaje = new LanguageData(new Gramatica());//clase de la gramatica
            p = new Parser(lenguaje);
            path = @"C:\";
        }

        #region 1. ANALIZAR ARBOL


        public ParseTreeNode parseArchivoImportado(string contenido, string nombreA){
            ParseTree s_tree = p.Parse(contenido);
            if (s_tree.HasErrors())
                foreach (var item in s_tree.ParserMessages)
                {   
                    Console.WriteLine("en Fil: {0}, Col: {1}, Pos: {2}, Tipo: {3}", item.Location.Line + 1, item.Location.Column + 1, item.Location.Position, item.Message);
                }
            if (s_tree.Root != null)
            {


                Console.WriteLine("========ACCIONES AL EVVALUAR EL ARBOL=============");
                /*accion.generarClases(s_tree.Root);
                accion.ejecutarImportaciones();
                accion.instanciarAtributosClase();
                cad = accion.ejecutarArchivo();*/
                Console.WriteLine("==================================================");
                //dispTree(s_tree.Root, 0);//IMPRIMO EL ARBOL
                graficarArbol(s_tree.Root);//GRAFICO EL ARBOL
                return s_tree.Root;

            }
            else
                MessageBox.Show(null, "Archivo de importacion "+nombreA+" posee errores", "Error", 0);
            return null;


        }


        public string parse(String str)
        {
            string cad = "";
            ParseTree s_tree = p.Parse(str); //parsear la entrada
            if (s_tree.HasErrors())//SE VERIFICA SI LA ENTRADA POSEE ERRORES
                foreach (var item in s_tree.ParserMessages)
                {   //SI EXISTEN ERRORES LOS IMPRIME van errores <--------- 
                    Console.WriteLine("en Fil: {0}, Col: {1}, Pos: {2}, Tipo: {3}", item.Location.Line + 1, item.Location.Column + 1, item.Location.Position, item.Message);
                    Error nuevo = new Error(item.Location.Column, item.Location.Line, "Sintactico", item.Message);
                    Constantes.erroresEjecucion.lErrores.Add(nuevo);
                }
            if (s_tree.Root != null)
            {
                Console.WriteLine("========ACCIONES AL EVVALUAR EL ARBOL=============");
                accion.generarArchivosClase(s_tree.Root);
                // accion.generarClases(s_tree.Root);
                //accion.ejecutarImportaciones();
                accion.instanciarAtributosClase();
                cad = accion.ejecutarArchivo();
                Console.WriteLine("==================================================");
                 //dispTree(s_tree.Root, 0);//IMPRIMO EL ARBOL
                 graficarArbol(s_tree.Root);//GRAFICO EL ARBOL
                //return s_tree.Root;
            }
            else
                MessageBox.Show(null, "Entrada posee errores", "Error", 0);
            return cad;
        }


        public void dispTree(ParseTreeNode node, int level)
        {
            for (int i = 0; i < level; i++)
                Console.Write("  ");
            Console.WriteLine(node);

            foreach (ParseTreeNode child in node.ChildNodes)
                dispTree(child, level + 1);


        }

        #endregion

        #region 2. GRAFICAR ARBOL 
        //Generar Arbol


        public void graficarArbol(ParseTreeNode root)
        {
            String arbolDerivacion = path + "Arbol.txt";
            String texto = getCodigoArbol(root);
            System.IO.File.WriteAllText(arbolDerivacion, texto);
            Console.WriteLine(arbolDerivacion);
            try
            {
                var command = string.Format("C:\\Program Files (x86)\\Graphviz2.38\bin\\dot.exe  -Tjpg {0} -o {1}", "\"" + arbolDerivacion + "\"", "\"" + arbolDerivacion.Replace(".txt", ".jpg") + "\"");
                Console.WriteLine(command);
                var procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/C " + command);
                var proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception x)
            {
                Console.WriteLine(":(");
                Console.WriteLine(x.ToString());
                Console.WriteLine(":(");
            }
        }

        private String getCodigoArbol(ParseTreeNode raiz)
        {
            String texto = "digraph G{\n graph [ dpi = 100 ]; \n",
                nivel = "0";
            texto += graficarNodo(raiz, nivel);
            texto += "}";
            return texto;
        }


        private String graficarNodo(ParseTreeNode raiz, String nivel)
        {
            String texto = String.Empty;
            //            texto += ("nnivel" + "n_" + niv) + " [label=\"" + generarNodo(raiz.ToString()) + "\"];\n ";
            texto += generarNodo("nivel", nivel) + " [label=\"" + generarNodo(raiz.ToString()) + "\"];\n ";
            for (int i = 0; i < raiz.ChildNodes.Count; i++)
            {
                texto += generarNodo("nivel", nivel) + " -> " + generarNodo("nivel", nivel + "_" + i) + ";\n ";
                texto += graficarNodo(raiz.ChildNodes[i], nivel + "_" + i);
                niv++;
            }
            return texto;


        }

        private String generarNodo(String texto, String nivel) 
        {
            String txt = texto
                         .Replace("+=", "asgmas")
                         .Replace("=", "asg")
                         .Replace("-", "biMns")//
                         .Replace("+", "biMas")
                         .Replace("%", "mod")
                         .Replace("/", "div")
                         .Replace("*", "mul")
                         .Replace("^", "pot")
                         .Replace("++", "umas")
                         .Replace("--", "umns")
                         .Replace("&&", "and")//
                         .Replace("!&&", "nand")
                         .Replace("||", "or")
                         .Replace("!||", "nor")
                         .Replace("&|", "xor")
                         .Replace("!", "not")
                         .Replace("!¡", "esNulo")
                         .Replace("==", "igual")//
                         .Replace("!=", "dif")
                         .Replace(">", "myr")
                         .Replace("<", "mnr")
                         .Replace(">=", "myri")
                         .Replace("<=", "mnri")
                         .Replace("_", "gb")
                         .Replace(".", "pto")
                         .Replace(":", "dpts");

            return "n" + generarNodo(txt) + nivel;
        }

        private String generarNodo(String texto)
        {
            return
                texto
                .Replace(" (Keyword)", "")
                .Replace(" (Key symbol)", "")
                .Replace(" (numero)", "")
                .Replace(" (entero)", "")
                .Replace(" (doble)", "")
                .Replace(" (identificador)", "")
                .Replace(" (cadena)", "")
                .Replace(" (caracter)", "")
                .Replace(" (booleano)", "");
        }

        #endregion


    }
}
