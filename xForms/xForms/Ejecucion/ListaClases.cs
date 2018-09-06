using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xForms.Ejecucion
{
    class ListaClases
    {
       List<Clase> lClases;



       public Clase get(int i)
       {
           return this.lClases.ElementAt(i);
       }
        public ListaClases()
        {
            this.lClases = new List<Clase>();
        }

        public void insertarClase(Clase c)
        {
            this.lClases.Add(c);
        }


        public int size()
        {
            return this.lClases.Count;
        }

    }
}
