using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xForms.Analizar;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using xForms.Ejecucion;

namespace xForms.Fechas
{
    class Fecha
    {
        ParseTreeNode nodoFecha;
        int dias;
        int mes;
        int anio;
        public String valFechaCadena;
        DateTime fechaReal;
        String cadenaRealFecha;

        public Fecha(ParseTreeNode nodo, String cadena)
        {
            this.dias = 0;
            this.anio = 0;
            this.anio = 0;
            this.nodoFecha = nodo;
            this.cadenaRealFecha = cadena;
        }

        public Valor validarFecha()
        {
            Valor ret = new Valor(Constantes.NULO, Constantes.NULO);
            //string cadenaFechaOriginal = nodoFecha.ChildNodes[0].Token.ValueString;;
            string cad = cadenaRealFecha.Replace("'", "").Trim();
            string[] valores = cad.Split('/');
            int d = int.Parse(valores[0]);
            int m = int.Parse(valores[1]);
            int a = int.Parse(valores[2]);
            try
            {
                DateTime f = new DateTime(a, m, d);
                this.fechaReal = f;
                this.dias = d;
                this.mes = m;
                this.anio = a;
                this.valFechaCadena = d + "/" + m + "/" + a;
                ret = new Valor(Constantes.FECHA, this);
            }
            catch (Exception e)
            {
                Constantes.erroresEjecucion.errorSemantico(this.nodoFecha, "No es posible crear una fecha con los parametros de dia: " + d + ", mes: " + m + " y anio: " + a);
            }

            return ret;

        }

    }
}
