﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xForms.Tabla_Simbolos
{
    class Contexto
    {
        public Stack<string> Ambitos;
        int contIF;
        int contElse;
        int contGeneral;
        int contFor;
        int contWhile;
        int contRepetir;
        int contCaso;
        int contHacerMientras;
        int contDefecto;

        public Contexto()
        {
            this.Ambitos = new Stack<string>();
            this.contIF = 0;
            this.contElse = 0;
            this.contFor = 0;
            this.contWhile = 0;
            this.contRepetir = 0;
            this.contCaso = 0;
            this.contHacerMientras = 0;
            this.contDefecto = 0;
        }

        public void salirAmbito()
        {
            this.Ambitos.Pop();

        }
        public void addAmbito(string nombre)
        {
            this.Ambitos.Push(nombre);
        }

        public void addIf()
        {
            contIF++;
            this.Ambitos.Push("IF"+ contIF);
        }

        public void addElse()
        {
            contElse++;
            this.Ambitos.Push("Else"+ contElse);
        }

        public void addCaso()
        {
            contCaso++;
            this.Ambitos.Push("Caso" + contCaso);
        }

        public void addDefecto()
        {
            contDefecto++;
            this.Ambitos.Push("Defecto" + contDefecto);
        }

        public void addMientras()
        {
            contWhile++;
            this.Ambitos.Push("Mientras" + contWhile);
        }

        public void addHacerMientras()
        {
            contHacerMientras++;
            this.Ambitos.Push("HacerMientras" + contHacerMientras);
        }

        public void addRepetir()
        {
            contRepetir++;
            this.Ambitos.Push("Repetir" + contRepetir);
        }

        public void addPara()
        {
            contFor++;
            this.Ambitos.Push("Para" + contFor);
        }


  public string getAmbito(){
	string contexto ="";
	string valTemporal;
	for(int i =this.Ambitos.Count-1; i>=0;i--){
		valTemporal= this.Ambitos.ElementAt(i);

		if(i==0){
			contexto +=valTemporal;
		}else{
			contexto+=valTemporal+"/";
		}
	}
   
	return contexto;
  }


  public void llenarAmbitos(String cadena)
  {
      string[] valores = cadena.Split('/');
      for (int i = 0; i < valores.Length; i++)
      {
          addAmbito(valores[i]);
      }
  }

  public Contexto clonarLista()
  {
      Contexto nuevo = new Contexto();
      string temp;
      for (int i = (this.Ambitos.Count-1); i>=0; i--)
      {
          temp = this.Ambitos.ElementAt(i);
          nuevo.addAmbito(temp);
      }

      return nuevo;

  }


        
    }
}
