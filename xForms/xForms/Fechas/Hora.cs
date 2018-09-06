using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using xForms.Ejecucion;

namespace xForms.Fechas
{
    class Hora
    {
        ParseTreeNode nodoHora;
        int horas;
        int minutos;
        int segundos;
        public String valHoraCadena;
        DateTime horaReal;
        String cadenaRealHora;

        public Hora(ParseTreeNode nodo, String cadena)
        {
            this.nodoHora = nodo;
            this.horas = 0;
            this.minutos = 0;
            this.segundos = 0;
            this.cadenaRealHora = cadena;
        }


        public Valor validarHora()
        {
            Valor ret = new Valor(Constantes.NULO, Constantes.NULO);
           // string cadenaFechaOriginal = nodoHora.ChildNodes[0].Token.ValueString; ;
            string cad = cadenaRealHora.Replace("'", "").Trim();
            string[] valores = cad.Split(':');
            int h = int.Parse(valores[0]);
            int m = int.Parse(valores[1]);
            int s = int.Parse(valores[2]);
            try
            {
                DateTime d = new DateTime(1992, 5, 25, h, m, s);
                this.horaReal = d;
                this.horas = h;
                this.minutos = m;
                this.segundos = s;
                this.valHoraCadena = h + ":" + m + ":" + s;
                ret = new Valor(Constantes.HORA, this);
            }
            catch (Exception e)
            {
                Constantes.erroresEjecucion.errorSemantico(this.nodoHora, "No es posible crear una hora con los parametros de horas: " + h + ", minutos: " + m + " y segundos: " + s);
            }

            return ret;

        }

    }
}
