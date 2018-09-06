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
    class FechaHora
    {
        ParseTreeNode nodoFechaHora;
        public string cadenaRealFechaHora;
        Fecha fechaA;
        Hora horaA;


        public FechaHora(ParseTreeNode nodo, String cadena)
        {
            this.nodoFechaHora = nodo;
            this.cadenaRealFechaHora= cadena;
        }


        public Valor validarFechaHora()
        {
            Valor ret = new Valor(Constantes.NULO, Constantes.NULO);
            string cad = cadenaRealFechaHora.Replace("'", "").Trim();
            string[] valores = cad.Split(' ');
            String fecha = (valores[0]);
            String  hora = (valores[1]);
            Fecha fech = new Fecha(nodoFechaHora, "'" + fecha + "'");
            Hora hor = new Hora(nodoFechaHora, "'" + hora + "'");
            Valor v1 = fech.validarFecha();
            Valor v2 = hor.validarHora();
            if ((!(v1.tipo.Equals(Constantes.NULO))) && (!(v2.tipo.Equals(Constantes.NULO))))
            {
                this.fechaA = fech;
                this.horaA = hor;
                this.cadenaRealFechaHora = fechaA.valFechaCadena + " " + horaA.valHoraCadena;
                ret = new Valor(Constantes.FECHA_HORA, this);
            }else{
                Constantes.erroresEjecucion.errorSemantico(this.nodoFechaHora, "No es posible crear un valor de tipo FechaHora, con los valores dados");
            }
            return ret;
            
        }


    }
}
