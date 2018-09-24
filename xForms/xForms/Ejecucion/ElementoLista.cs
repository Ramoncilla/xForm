using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xForms.Ejecucion
{
    class ElementoLista
    {
        public List<Valor> elementos;

        public ElementoLista()
        {
            this.elementos = new List<Valor>();
        }


        public void insertarValores(List<Valor> valores)
        {
            Valor temp;
            for (int i = 0; i < valores.Count; i++)
            {
                temp = valores.ElementAt(i);
                this.elementos.Add(temp);
            }
        }


        public ElementoLista clonar()
        {
            ElementoLista nuevo = new ElementoLista();
            Valor temp, temp2;
            for (int i = 0; i < this.elementos.Count; i++)
            {
                temp = this.elementos.ElementAt(i);
                temp2 = new Valor(temp.tipo, temp.valor);
                nuevo.elementos.Add(temp2);
            }
            return nuevo;
        }


        public Valor obtener(int pos)
        {
            Valor temp;
            for (int i = 0; i < elementos.Count; i++)
            {
                temp = elementos.ElementAt(i);
                if (i == pos)
                {
                    return temp;
                }
            }
            return new Valor();
        }


        public Opcion generarOpcion()
        {
            Opcion op;
            if (elementos.Count >= 3)
            {
                op = new Opcion(elementos.ElementAt(0).valor.ToString(), elementos.ElementAt(1).valor.ToString(), elementos.ElementAt(2).valor.ToString());
                return op;
            }
            return null;
        }

    }
}
