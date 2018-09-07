using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xForms.Errores
{
    class Error
    {
         int columna;
         int fila;
         string tipo;
         string descripcion;

        public Error(int col, int fil, string tipo, string desc)
        {
            this.columna = col;
            this.fila = fil;
            this.tipo = tipo;
            this.descripcion = desc;
        }


        public string htmlError()
        {
            string cad = "";

            cad = "<tr>\n" +
                "<td>"+this.tipo+"</td>\n" +
                "<td>"+(this.fila+1)+"</td>\n" +
                "<td>"+(this.columna+1)+"</td>\n" +
                "<td>" + this.descripcion + "</td>\n" +
                "</tr>\n";
            return cad;
        }



        


    }
}
