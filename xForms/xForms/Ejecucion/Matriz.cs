﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xForms.Ejecucion
{
    class Matriz
    {

        List<ElementoLista> matriz;


        public Matriz()
        {
            this.matriz = new List<ElementoLista>();
        }


        public void insertar(List<Valor> valores)
        {
            ElementoLista nuevo = new ElementoLista();
            nuevo.insertarValores(valores);
            this.matriz.Add(nuevo);
        }


        public int noFilas()
        {
            return this.matriz.Count;
        }

        public Valor Obtener(int pos1, int pos2)
        {
            ElementoLista temp;
            for (int i = 0; i < this.matriz.Count; i++)
            {
                temp= matriz.ElementAt(i);
                if (i == pos1)
                {
                    return temp.obtener(pos2);
                }
            }
            return new Valor();
        }

        public Valor buscar(Valor val1, int pos2)
        {
            ElementoLista temp;
            Valor c;
            for (int i = 0; i < this.matriz.Count; i++)
            {
                temp = matriz.ElementAt(i);
                int n = temp.elementos.Count;
                if (n > 0)
                {
                    c = temp.elementos.ElementAt(0);
                    if (c.tipo.Equals(val1.tipo, StringComparison.CurrentCultureIgnoreCase) &&
                        c.valor.Equals(val1.valor))
                    {
                        if ((n - 1) >= pos2)
                        {
                            Valor r = temp.elementos.ElementAt(pos2);
                            return r;
                        }
                        else
                        {
                            return new Valor();
                        }
                    }
                }
            }

            return new Valor();
        }

        public Matriz clonar()
        {
            Matriz n = new Matriz();

            ElementoLista temp, nuevo;
            for (int i = 0; i < this.matriz.Count; i++)
            {
                temp = matriz.ElementAt(i);
                nuevo = temp.clonar();
                n.matriz.Add(nuevo);
            }
            return n;

        }


        public List<Opcion> obtenerOpciones()
        {
            List<Opcion> opciones = new List<Opcion>();
            ElementoLista temp;
            Opcion op;
            for (int i = 0; i < matriz.Count; i++)
            {
                temp = matriz.ElementAt(i);
                op = temp.generarOpcion();
                if (op != null)
                {
                    opciones.Add(op);
                }
            }
            return opciones;
        }


    }
}
