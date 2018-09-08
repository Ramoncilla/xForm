using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xForms.Ejecucion
{
    class elementoRetorno
    {
        public bool continuar;
        public bool parar;
        public bool banderaSi;
        public bool banderaRetorno;
        public Valor val;

        public elementoRetorno()
        {
            this.continuar = false;
            this.parar = false;
            this.banderaSi = false;
            this.banderaRetorno = false;
            val = new Valor(Constantes.NULO, Constantes.NULO);

        }
    }
}
