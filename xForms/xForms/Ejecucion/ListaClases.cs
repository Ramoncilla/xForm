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

        public void instanciarAtributosClase()
        {
            Clase temp;
            Contexto ambitos;
            Simbolo atriTemp;
            for (int i = 0; i < this.lClases.Count; i++)
            {
                ambitos = new Contexto();
                temp = this.lClases.ElementAt(i);
                ambitos.addAmbito(temp.nombreClase);
                int h = temp.atributosClase.lAtributos.Count;
                for (int j = 0; j < h; j++)
                {
                    atriTemp = temp.atributosClase.lAtributos.ElementAt(j);
                    if (esObjecto(atriTemp.tipo))
                    {
                        ambitos.addAmbito(atriTemp.nombre);
                        asignarAtributos(atriTemp.tipo, temp.atributosClase, ambitos);
                        ambitos.salirAmbito();
                    }
                }
            }
        }

        private void asignarAtributos(string nomClase, ListaAtributos atributosClase, Contexto ambiente)
        {
            Clase claseTemp;
            Simbolo atriTemp;
            List<Simbolo> atributosTemporales;
            for (int i = 0; i < lClases.Count; i++)
            {
                claseTemp = lClases.ElementAt(i);
                if (claseTemp.nombreClase.Equals(nomClase, StringComparison.CurrentCultureIgnoreCase))
                {
                    atributosTemporales = claseTemp.atributosClase.clonarLista();
                    for (int j = 0; j < atributosTemporales.Count; j++)
                    {
                        atriTemp = atributosTemporales.ElementAt(j);

                        if (atriTemp.rutaAcceso.Equals(nomClase, StringComparison.InvariantCultureIgnoreCase))
                        {
                            atriTemp.rutaAcceso = ambiente.getAmbito();
                            atributosClase.insertarAtributo(atriTemp);
                            if (esObjecto(atriTemp.tipo))
                            {
                                ambiente.addAmbito(atriTemp.nombre);
                                asignarAtributos(atriTemp.tipo, atributosClase, ambiente);
                                ambiente.salirAmbito();
                            }

                        }
                        else
                        {
                            continue;
                        }
                        
                       
                    }
                }
            }
        }


        public ListaAtributos instanciaLocal(string nombClase, ListaAtributos atributosClase, Contexto ambiente){
            asignarAtributosLocales(nombClase, atributosClase,ambiente);
            return atributosClase;
        }


        private void asignarAtributosLocales(string nomClase, ListaAtributos atributosClase, Contexto ambiente)
        {
            Clase claseTemp;
            Simbolo atriTemp;
            List<Simbolo> atributosTemporales;
            for (int i = 0; i < lClases.Count; i++)
            {
                claseTemp = lClases.ElementAt(i);
                if (claseTemp.nombreClase.Equals(nomClase, StringComparison.CurrentCultureIgnoreCase))
                {
                    atributosTemporales = claseTemp.atributosClase.clonarLista();
                    for (int j = 0; j < atributosTemporales.Count; j++)
                    {
                        atriTemp = atributosTemporales.ElementAt(j);

                        if (atriTemp.rutaAcceso.Equals(nomClase, StringComparison.CurrentCultureIgnoreCase))
                        {
                            atriTemp.rutaAcceso = ambiente.getAmbito();
                            atriTemp.esAtributo = false;
                            atributosClase.insertarAtributo(atriTemp);
                            if (esObjecto(atriTemp.tipo))
                            {
                                ambiente.addAmbito(atriTemp.nombre);
                                asignarAtributosLocales(atriTemp.tipo, atributosClase, ambiente);
                                ambiente.salirAmbito();
                            }

                        }
                        else
                        {
                            continue;
                        }
                        
                    }
                }
            }
        }

        private bool esObjecto(String tipo)
        {
            tipo = tipo.ToLower();
            if (tipo.ToLower().Equals(Constantes.CADENA.ToLower()) ||
               tipo.ToLower().Equals(Constantes.BOOLEANO.ToLower()) ||
                tipo.ToLower().Equals(Constantes.CARACTER.ToLower()) ||
                tipo.ToLower().Equals(Constantes.ENTERO.ToLower()) ||
                tipo.ToLower().Equals(Constantes.DECIMAL.ToLower()) ||
                tipo.ToLower().Equals(Constantes.FECHA.ToLower()) ||
                tipo.ToLower().Equals(Constantes.FECHA_HORA.ToLower()) ||
                tipo.ToLower().Equals(Constantes.HORA.ToLower()))
            {
                return false;
            }
            return true;
        }



        public Clase obtenerClase(String nombreClase)
        {
            Clase temp;
            for (int i = 0; i < this.lClases.Count; i++)
            {
                temp = this.lClases.ElementAt(i);
                if (temp.nombreClase.Equals(nombreClase, StringComparison.CurrentCultureIgnoreCase))
                {
                    return temp;
                }
            }
            return null;
        }


    }
}
