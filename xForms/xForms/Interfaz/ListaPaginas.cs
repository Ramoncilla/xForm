using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
