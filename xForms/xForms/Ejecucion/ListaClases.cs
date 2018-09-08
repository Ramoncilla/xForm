using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xForms.Tabla_Simbolos;

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


        public Funcion obtenerFuncion(string nombreClase, string nombreFuncion, string cadParametros)
        {
            Clase temp;
            for (int i = 0; i < this.lClases.Count; i++)
            {
                temp = this.lClases.ElementAt(i);
                if (temp.nombreClase.ToLower().Equals(nombreClase.ToLower()))
                {
                    return temp.obtenerFuncion(nombreFuncion, cadParametros);
                }
            }
            return null;
        }



    }
}
