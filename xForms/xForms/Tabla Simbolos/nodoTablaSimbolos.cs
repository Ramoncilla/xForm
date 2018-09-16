using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using xForms.Ejecucion;

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
                Constantes.erroresEjecucion.errorSemantico( "La variable " + simb.nombre + ", ya existe en el ambito actual ");
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
                    temp.ambito.ToLower().Equals(simb.ambito.ToLower()))
                {
                    return true;
                }
            }
            return false;

        }


        public Simbolo obtenerSimbolo2(string nombre, string amb)
        {
            Simbolo temp;
            for (int i = 0; i < this.variablesAmbito.Count; i++)
            {
                temp = this.variablesAmbito.ElementAt(i);
                if (temp.nombre.ToLower().Equals(nombre.ToLower()) &&
                    temp.ambito.ToLower().Equals(amb.ToLower()))
                {
                    return temp;
                }
            }
            return null;

        }



        public void imprimirNodoTabla()
        {
            Console.WriteLine(this.nombreFuncion);
            Simbolo simb;
            
            for (int i = 0; i < this.variablesAmbito.Count; i++)
            {
                simb = variablesAmbito.ElementAt(i);
                Console.WriteLine("--------------Simbolo  "+i+"------------------------------");
                Console.WriteLine("Nombre var:  " + simb.nombre);
                Console.WriteLine("Ambito:  " + simb.ambito);
                Console.WriteLine("ruta acceso:  " + simb.rutaAcc);
                Console.WriteLine("--------------------------------------------");
            }
        }


        public Simbolo obtenerSimbolo(String nombre, Contexto ruta)
        {
            Simbolo temp;
            Contexto ruta2 = ruta.clonarLista();
            String rutaTemporal;
            for (int i = 0; i < ruta.Ambitos.Count; i++)
            {
                rutaTemporal = ruta2.getAmbito();
                for (int j = 0; j < this.variablesAmbito.Count; j++)
                {
                    temp = this.variablesAmbito.ElementAt(j);
                    if (temp.nombre.Equals(nombre,StringComparison.CurrentCultureIgnoreCase) &&
                    temp.ambito.Equals(rutaTemporal,StringComparison.CurrentCultureIgnoreCase))
                    {
                        return temp;
                    }
                    
                }
                ruta2.salirAmbito();
            }
            
            return null;
        }




        public VairablesObjeto obtenerObjetoConAtributos(string nombreObj, string ambitoObjeto, string rutaA)
        {
            Simbolo temp;
            for (int i = 0; i < this.variablesAmbito.Count; i++)
            {
                temp = variablesAmbito.ElementAt(i);
                if(temp.nombre.Equals(nombreObj, StringComparison.InvariantCultureIgnoreCase) &&
                    temp.ambito.Equals(ambitoObjeto, StringComparison.InvariantCultureIgnoreCase))
                {
                    Simbolo temp2;
                    VairablesObjeto objRet = new VairablesObjeto(temp);
                    string nuevoAmbito = ambitoObjeto + "/" + nombreObj;
                    string nuevaRuta = rutaA + "/" + nombreObj;
                    for (int j = 0; j < this.variablesAmbito.Count; j++)
                    {
                        temp2 = variablesAmbito.ElementAt(j);
                        
                        if (temp2.ambito.Equals(nuevoAmbito, StringComparison.InvariantCultureIgnoreCase) ||
                            temp2.rutaAcc.ToLower().Contains(nuevaRuta.ToLower()))
                        {
                            objRet.guardarSimbolo(temp2);
                        }
                    }
                    return objRet;
                }
                
            }

            return null;
        }



    }
}
