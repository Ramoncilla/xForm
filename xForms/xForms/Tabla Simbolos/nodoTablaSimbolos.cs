﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;

namespace xForms.Tabla_Simbolos
{
    class nodoTablaSimbolos
    {
        public String nombreFuncion;
        public Stack<Simbolo> variablesAmbito;


        public nodoTablaSimbolos(String nombre)
        {
            this.nombreFuncion= nombre;
            this.variablesAmbito = new Stack<Simbolo>();
        }

        public bool insertarSimbolo(Simbolo simb)
        {
            if (!(existe(simb)))
            {
                this.variablesAmbito.Push(simb);
                return true;
            }
            else
            {
                Constantes.erroresEjecucion.errorSemantico( "La variable " + simb.nombre + ", ya existe en el ambito actual");
                return false;
            }

        }

        private bool existe(Simbolo simb)
        {
            Simbolo temp;
            for (int i = 0; i < this.variablesAmbito.Count; i++)
            {
                temp = this.variablesAmbito.ElementAt(i);
                if(temp.nombre.ToLower().Equals(simb.nombre.ToLower()) &&
                    temp.rutaAcceso.ToLower().Equals(simb.rutaAcceso.ToLower()))
                {
                    return true;
                }
            }
            return false;

        }


        public Simbolo obtenerSimbolo(string nombre, string rutaAcceso)
        {
            Simbolo temp;
            for (int i = 0; i < this.variablesAmbito.Count; i++)
            {
                temp = this.variablesAmbito.ElementAt(i);
                if (temp.nombre.ToLower().Equals(nombre.ToLower()) &&
                    temp.rutaAcceso.ToLower().Equals(rutaAcceso.ToLower()))
                {
                    return temp;
                }
            }
            return null;

        }






    }
}
