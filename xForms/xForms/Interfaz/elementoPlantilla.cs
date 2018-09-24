using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xForms.Analizar;

namespace xForms.Interfaz
{
    class elementoPlantilla
    {
        public string rutaCarpeta;
        public string tituloPestanha;
        public FastColoredTextBox cajaTexto;
        string codigoRespuesta = "clase Respuesta publico{" +
"Cadena esCadena;" +
"entero esEntero;" +
"decimal esDecimal;" +
"fecha esFecha;" +
"hora esHora;" +
"fechahora esFechaHora;" +
"booleano esBooleano;" +

"respuesta(){" +

"}" +


"}";


        public elementoPlantilla()
        {
            this.cajaTexto = new FastColoredTextBox();
            cajaTexto.Dock = DockStyle.Fill;
            this.rutaCarpeta = "";
            this.tituloPestanha = "";
        }

        public string ejecutar()
        {
            Constantes.erroresEjecucion = new Errores.ListaErrores();
            Arbol analizador = new Arbol(rutaCarpeta, tituloPestanha);
            string v = analizador.parse(cajaTexto.Text+ codigoRespuesta);
            return v;

        }

        





        

    }
}
