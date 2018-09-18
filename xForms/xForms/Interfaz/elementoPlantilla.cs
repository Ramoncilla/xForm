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
            string v = analizador.parse(cajaTexto.Text);
            return v;

        }

        





        

    }
}
