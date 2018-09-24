using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xForms.Formularios
{
    class ElementosFormulario
    {
        public List<Pregunta> preguntas;
        public List<Formulario> formularios;
        public List<Agrupacion> grupos;


        public ElementosFormulario()
        {
            this.preguntas = new List<Pregunta>();
            this.formularios = new List<Formulario>();
            this.grupos = new List<Agrupacion>();
        }


        public void insertarPregunta(Pregunta pNueva)
        {
            this.preguntas.Add(pNueva);
        }

        public void insertarFormular(Formulario fNuevo)
        {
            this.formularios.Add(fNuevo);
        }


        public void insertarGrupo(Agrupacion gNuevo)
        {
            this.grupos.Add(gNuevo);
        }

        public Formulario buscarForm(String nombre)
        {
            Formulario temp;
            for (int i = 0; i < formularios.Count; i++)
            {
                temp = formularios.ElementAt(i);
                if (temp.nombre.Equals(nombre, StringComparison.CurrentCultureIgnoreCase))
                {
                    return temp;
                }
            }
            return null;
        }


    }
}
