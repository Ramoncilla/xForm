using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;

namespace xForms.Errores
{
    class ListaErrores
    {
        public List<Error> lErrores;


        public ListaErrores()
        {
            this.lErrores= new List<Error>();

        }




        public void errorSemantico(ParseTreeNode nodo, String descripcion)
        {
            if (nodo != null)
            {
                int fila = nodo.FindToken().Location.Line;
                int col = nodo.FindToken().Location.Column;
                Error nuevo = new Error(col, fila, Constantes.ERROR_SEMANTICO, descripcion);
                this.lErrores.Add(nuevo);
            }
            else
            {
                int fila = 0;
                int col = 0;
                Error nuevo = new Error(col, fila, Constantes.ERROR_SEMANTICO, descripcion);
                this.lErrores.Add(nuevo);

            }
            
        }


        public void errorSintactico(String descripcion)
        {
            int fila = 0;
            int col = 0;
            Error nuevo = new Error(col, fila, Constantes.ERROR_SINTACTICO, descripcion);
            this.lErrores.Add(nuevo);
        }

        public void errorSemantico(String descripcion)
        {
            int fila = 0;
            int col = 0;
            Error nuevo = new Error(col, fila, Constantes.ERROR_SEMANTICO, descripcion);
            this.lErrores.Add(nuevo);
        }


        public void moostrarErrores()
        {

            String errores = "<!DOCTYPE html>\n" +
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
"<body>\n" +
"\n" +
"<table id=\"customers\">";
            errores += "<tr>\n" +
                "<td>Tipo</td>\n" +
                "<td>Fila</td>\n" +
                "<td>Columna</td>\n" +
                "<td>Descripcion</td>\n" +
                "</tr>\n";
            for (int i = 0; i < this.lErrores.Count; i++)
            {
                errores += this.lErrores.ElementAt(i).htmlError();
            }
            errores += "</table></body></html>";
            string path = @"C:\";
            path = path + "errores.html";
            System.IO.File.WriteAllText(path, errores);
        }

    }
}
