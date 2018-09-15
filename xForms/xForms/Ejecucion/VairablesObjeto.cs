using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xForms.Tabla_Simbolos;

namespace xForms.Ejecucion
{
    class VairablesObjeto
    {

        public List<Simbolo> variablesInstancia;

        public VairablesObjeto (){
            this.variablesInstancia = new List<Simbolo>();
        }

        public void guardarSimbolo(Simbolo simb)
        {
            this.variablesInstancia.Add(simb);
        }



    }
}
