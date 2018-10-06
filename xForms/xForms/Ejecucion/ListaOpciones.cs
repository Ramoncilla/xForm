using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xForms.Tabla_Simbolos;


namespace xForms.Ejecucion
{
    class ListaOpciones:Simbolo
    {

        public Matriz matriz;

        public ListaOpciones(string nombre, string tipo, string visibilidad, ParseTreeNode nodoExprsion, string ambito)
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.visibilidad = visibilidad;
            this.nodoExpresionValor = nodoExprsion;
            this.ambito = ambito;
            matriz = new Matriz();
        }
         public ListaOpciones(string nombre, string tipo, string ambito, bool atri)
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.ambito = ambito;
            this.esAtributo = atri;
            this.matriz = new Matriz();
        }

        public void insertar(List<Valor> valores)
        {
            this.matriz.insertar(valores);
        }


        public Valor  Obtener(int pos1, int pos2)
        {
            if (pos1 < 0 || pos1 > (matriz.noFilas() - 1))
            {
                return new Valor();
            }
            else
            {
                return this.matriz.Obtener(pos1, pos2);
            }  
        }

        public Valor buscar(Valor val1, int pos2)
        {
            //if (pos2 < 0 || pos2 > (matriz.noFilas() - 1))
            //{
              //  return new Valor();
            //}
            return this.matriz.buscar(val1, pos2);
        }



        public override Simbolo clonar()
        {
            ListaOpciones nueva = new ListaOpciones(this.nombre, this.tipo, this.ambito, this.esAtributo);
            nueva.valor = this.valor;
            nueva.usada = this.usada;
            nueva.visibilidad = this.visibilidad;
            nueva.nodoExpresionValor = this.nodoExpresionValor;
            nueva.rutaAcc = this.rutaAcc;
            nueva.matriz = this.matriz.clonar();
            return nueva;
        }




    }
}
