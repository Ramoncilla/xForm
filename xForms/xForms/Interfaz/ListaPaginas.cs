using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xForms.Interfaz
{
    class ListaPaginas
    {
        public List<elementoPlantilla> paginas;



        public ListaPaginas()
        {
            this.paginas = new List<elementoPlantilla>();
        }

        public void insertarPagina(elementoPlantilla pagina)
        {
            this.paginas.Add(pagina);
        }


        public void eliminarPlantilla(TabPage tab)
        {
            elementoPlantilla temp;
            int k = -1;
            for (int i = 0; i < paginas.Count; i++)
            {
                temp = paginas.ElementAt(i);
                if (temp.tab.Equals(tab))
                {
                    k = i;
                    break;
                }
            }
            
            if (k != -1)
            {
                paginas.RemoveAt(k);

            }


        }


        public void escribirTexto(int indice, string texto, string nombre, string ruta)
        {
            elementoPlantilla temp;
            for (int i = 0; i < this.paginas.Count; i++)
            {
                temp = this.paginas.ElementAt(i);
                if (i == indice)
                {
                    temp.rutaCarpeta = ruta;
                    temp.tituloPestanha = nombre;
                    temp.cajaTexto.Text = texto;
                }
            }
        }


        public string ejecutar(int no)
        {
            elementoPlantilla temp;
            string cad = "";
            for (int i = 0; i <this.paginas.Count; i++)
            {
                temp = paginas.ElementAt(i);
                if (i == no)
                {
                    cad=temp.ejecutar();
                }
            }
            return cad;
        }


    }
}
