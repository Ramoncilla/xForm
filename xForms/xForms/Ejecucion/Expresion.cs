using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using xForms.Tabla_Simbolos;
using xForms.Analizar;
using xForms.Fechas;

namespace xForms.Ejecucion
{
    class Expresion
    {

        public Expresion()
        {

        }





        public Valor resolverExpresion(ParseTreeNode nodo, Contexto ambiente, String nombreClase, String nombreMetodo, tablaSimbolos tabla)
        {
            string nombreNodo = nodo.Term.Name;
            switch (nombreNodo)
            {

                case Constantes.EXPRESION:
                    {
                        return resolverExpresion(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
                    }

                case Constantes.ARITMETICA:
                    {
                        #region resolver expresion aritmetica 
                        Valor v1 = resolverExpresion(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
                        Valor v2 = resolverExpresion(nodo.ChildNodes[2], ambiente, nombreClase, nombreMetodo, tabla);
                        switch (nodo.ChildNodes[1].Token.ValueString)
                        {
                            case Constantes.MAS:
                                {
                                    Valor v =  sumar(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una suma con los siguientes tipos " + v1.tipo + " y " + v2.tipo);
                                    }
                                    return v;
                                }
                            case Constantes.MENOS:
                                {
                                    Valor v = restar(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una resta con los siguientes tipos " + v1.tipo + " y " + v2.tipo);
                                    }
                                    return v;
                                }
                            case Constantes.MULTIPLICACION:
                                {
                                    Valor v = multiplicar(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una multiplicacion con los siguientes tipos " + v1.tipo + " y " + v2.tipo);
                                    }
                                    return v;
                                }

                            case Constantes.DIVISION:
                                {
                                    Valor v = dividir(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una division con los siguientes tipos " + v1.tipo + " y " + v2.tipo);
                                    }
                                    return v;
                                }

                            case Constantes.POTENCIA:
                                {
                                    Valor v = potencia(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una potencia con los siguientes tipos " + v1.tipo + " y " + v2.tipo);
                                    }
                                    return v;
                                }

                            case Constantes.MODULO:
                                {
                                    Valor v = modulo(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar un modulo con los siguientes tipos " + v1.tipo + " y " + v2.tipo);
                                    }
                                    return v;
                                }

                        }

                        break;
#endregion
                    }

                case Constantes.RELACIONAL:
                    {
                        #region resolver expresion relacional

                        Valor v1 = resolverExpresion(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
                        Valor v2 = resolverExpresion(nodo.ChildNodes[2], ambiente, nombreClase, nombreMetodo, tabla);
                        switch (nodo.ChildNodes[1].Token.ValueString)
                        {
                            case Constantes.MENOR:
                                {
                                    Valor v = menor(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una operacion relacional Menor, con el tipo " + v1.tipo + ", con " + v2.tipo);
                                    }
                                    return v;
                                }

                            case Constantes.MAYOR:
                                {
                                    Valor v = mayor(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una operacion relacional Mayor, con el tipo " + v1.tipo + ", con " + v2.tipo);
                                    }
                                    return v;
                                }

                            case Constantes.MENOR_IGUAL:
                                {
                                    Valor v = menorIgual(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una operacion relacional Menor igual, con el tipo " + v1.tipo + ", con " + v2.tipo);
                                    }
                                    return v;
                                }

                            case Constantes.MAYOR_IGUAL:
                                {
                                    Valor v = mayorIgual(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una operacion relacional Mayor igual, con el tipo " + v1.tipo + ", con " + v2.tipo);
                                    }
                                    return v;
                                }

                            case Constantes.IGUAL_IGUAL:
                                {
                                    Valor v = igualIgual(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una operacion relacional Igual, con el tipo " + v1.tipo + ", con " + v2.tipo);
                                    }
                                    return v;
                                }

                            case Constantes.DISTINTO_A:
                                {
                                    Valor v = distintoA(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una operacion relacional Distinto a, con el tipo " + v1.tipo + ", con " + v2.tipo);
                                    }
                                    return v;
                                }


                        }


                        break;
                        #endregion                       
                    }


                case Constantes.LOGICA:
                    {
                        #region resolver una expresion logica 
                        Valor v1 = resolverExpresion(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
                        Valor v2 = resolverExpresion(nodo.ChildNodes[2], ambiente, nombreClase, nombreMetodo, tabla);
                        switch (nodo.ChildNodes[1].Token.ValueString)
                        {
                            case Constantes.AND:
                                {
                                    Valor v = And(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una operacion logica and, con el tipo " + v1.tipo + ", con " + v2.tipo);
                                    }
                                    return v;
                                }

                            case Constantes.OR:
                                {
                                    Valor v = Or(v1, v2);
                                    if (esNulo(v))
                                    {
                                        Constantes.erroresEjecucion.errorSemantico(nodo, "No es valido realizar una operacion logica Or, con el tipo " + v1.tipo + ", con " + v2.tipo);
                                    }
                                    return v;
                                }
                        }

                        break;
                        #endregion
                    }

                case Constantes.ACCESO:
                    {
                        #region resolver un acceso
                        int no = nodo.ChildNodes.Count;
                        if (no == 1)
                        {
                            Valor v = resolverExpresion(nodo.ChildNodes[0], ambiente, nombreClase, nombreMetodo, tabla);
                            return v;
                        }

                        /*foreach (ParseTreeNode item in nodo.ChildNodes)
                        {
                            evaluarArbol(item, ambiente, nombreClase, nombreMetodo, tabla);
                        }
                        break;*/

                        break;
                        #endregion
                    }

                case Constantes.ID:{
                    string nombreVar = nodo.ChildNodes[0].Token.ValueString;
                    string ruta= ambiente.getAmbito();
                    Simbolo simb = tabla.buscarSimbolo(nombreVar, ambiente);
                    if (simb != null)
                    {
                        return simb.valor;
                    }
                    else
                    {
                        Constantes.erroresEjecucion.errorSemantico(nodo, "La variable " + nombreVar + ", no existe en el ambito actual "+ ambiente.getAmbito());
                        return new Valor(Constantes.NULO, Constantes.NULO);
                    }
                }

                case Constantes.LLAMADA:
                    {
                        break;
                    }

                case Constantes.POS_ARREGLO:
                    {
                        break;
                    }
                #region valoresNativos
                case Constantes.FECHA_HORA:
                    {
                        FechaHora nueva = new FechaHora(nodo,  nodo.ChildNodes[0].Token.ValueString);
                        return nueva.validarFechaHora();
                    }

                case Constantes.HORA:
                    {
                        Hora nueva = new Hora(nodo, nodo.ChildNodes[0].Token.ValueString);
                        return nueva.validarHora();
                    }
                case Constantes.FECHA:
                    {
                        Fecha nueva = new Fecha(nodo, nodo.ChildNodes[0].Token.ValueString);
                        return nueva.validarFecha();
                    }

                case Constantes.ENTERO:
                    {
                        int imagen = int.Parse(nodo.ChildNodes[0].Token.ValueString);
                        Valor ret = new Valor(Constantes.ENTERO, imagen);
                        return ret;
                    }

                case Constantes.CADENA:
                    {
                        String cad = nodo.ChildNodes[0].Token.ValueString;
                        return new Valor(Constantes.CADENA, cad);
                    }


                case Constantes.CHAR:
                    {
                        char cad = char.Parse(nodo.ChildNodes[0].Token.ValueString);
                        return new Valor(Constantes.CHAR, cad);
                    }

                case Constantes.DECIMAL:
                    {
                        double cad = double.Parse(nodo.ChildNodes[0].Token.ValueString);
                        return new Valor(Constantes.DECIMAL, cad);
                    }

                case Constantes.BOOLEANO:
                    {
                        String cad = nodo.ChildNodes[0].Token.ValueString;
                        return new Valor(Constantes.BOOLEANO, cad);
                    }

                #endregion

            }


            return new Valor(Constantes.NULO, Constantes.NULO);
        }

        #region Operaciones Logicas
        #region Or
        private Valor Or(Valor v1, Valor v2)
        {
            Valor resp = new Valor(Constantes.NULO, Constantes.NULO);
            if (esBool(v1) && esBool(v2))
            {
                int b1 = getBooleanoNumero(v1);
                int b2 = getBooleanoNumero(v2);
                int res = b1 + b2;
                if (res > 0)
                {
                    resp = new Valor(Constantes.BOOLEANO, Constantes.VERDADERO);
                }
                else
                {
                    resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
                }

            }

            return resp;
        }

        #endregion


        #region And

        private Valor And(Valor v1, Valor v2)
        {
            Valor resp= new Valor(Constantes.NULO, Constantes.NULO);
            if (esBool(v1) && esBool(v2))
            {
                int b1 = getBooleanoNumero(v1);
                int b2 = getBooleanoNumero(v2);
                int res = b1 * b2;
                if (res > 0)
                {
                    resp = new Valor(Constantes.BOOLEANO, Constantes.VERDADERO);
                }else{
                    resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
                }

            }

            return resp;
        }


        #endregion

#endregion


        #region Operaciones Relacionales

        #region Menor

        private Valor menor(Valor v1, Valor v2)
        {
            Valor resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
            if (esEntero(v1) && esEntero(v2))
            {
                if (getEntero(v1) < getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                if (getEntero(v1) < getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esEntero(v1) && esCaracter(v2))
            {
                if (getEntero(v1) < getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                if (getDecimal(v1) < getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                if (getDecimal(v1) < getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esCaracter(v2))
            {
                if (getDecimal(v1) < getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esCaracter(v1) && esCaracter(v2))
            {
                if (getCharNumero(v1) < getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esEntero(v2))
            {
                if (getCharNumero(v1) < getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esDecimal(v2))
            {
                if (getCharNumero(v1) < getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCadena(v1) && esCadena(v2))
            {
                if(getCadena(v1).CompareTo(getCadena(v2)) < 0){
                    resp.valor = Constantes.VERDADERO;
                }
            }

            /*si se tuvieran que incluir los demas tipos*/
            return new Valor(Constantes.NULO, Constantes.NULO);
        }


        #endregion


        #region MayorIgual

        private Valor mayorIgual(Valor v1, Valor v2)
        {
            Valor resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
            if (esEntero(v1) && esEntero(v2))
            {
                if (getEntero(v1) >= getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                if (getEntero(v1) >= getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esEntero(v1) && esCaracter(v2))
            {
                if (getEntero(v1) >= getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                if (getDecimal(v1) >= getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                if (getDecimal(v1) >= getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esCaracter(v2))
            {
                if (getDecimal(v1) >= getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esCaracter(v1) && esCaracter(v2))
            {
                if (getCharNumero(v1) >= getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esEntero(v2))
            {
                if (getCharNumero(v1) >= getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esDecimal(v2))
            {
                if (getCharNumero(v1) >= getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCadena(v1) && esCadena(v2))
            {
                if (getCadena(v1).CompareTo(getCadena(v2)) >= 0)
                {
                    resp.valor = Constantes.VERDADERO;
                }
            }

            /*si se tuvieran que incluir los demas tipos*/
            return new Valor(Constantes.NULO, Constantes.NULO);
        }

        #endregion


        #region Mayor
        private Valor mayor(Valor v1, Valor v2)
        {
            Valor resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
            if (esEntero(v1) && esEntero(v2))
            {
                if (getEntero(v1) > getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                if (getEntero(v1) > getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esEntero(v1) && esCaracter(v2))
            {
                if (getEntero(v1) > getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                if (getDecimal(v1) > getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                if (getDecimal(v1) > getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esCaracter(v2))
            {
                if (getDecimal(v1) > getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esCaracter(v1) && esCaracter(v2))
            {
                if (getCharNumero(v1) > getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esEntero(v2))
            {
                if (getCharNumero(v1) > getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esDecimal(v2))
            {
                if (getCharNumero(v1) > getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }

            /*si se tuvieran que incluir los demas tipos*/
            return new Valor(Constantes.NULO, Constantes.NULO);
        }
        #endregion Mayor


        #region MenorIgual

        private Valor menorIgual(Valor v1, Valor v2)
        {
            Valor resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
            if (esEntero(v1) && esEntero(v2))
            {
                if (getEntero(v1) <= getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                if (getEntero(v1) <= getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esEntero(v1) && esCaracter(v2))
            {
                if (getEntero(v1) <= getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                if (getDecimal(v1) <= getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                if (getDecimal(v1) <= getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esCaracter(v2))
            {
                if (getDecimal(v1) <= getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esCaracter(v1) && esCaracter(v2))
            {
                if (getCharNumero(v1) <= getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esEntero(v2))
            {
                if (getCharNumero(v1) <= getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esDecimal(v2))
            {
                if (getCharNumero(v1) <= getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }

            /*si se tuvieran que incluir los demas tipos*/
            return new Valor(Constantes.NULO, Constantes.NULO);
        }
        #endregion

        #region igualIgual
        private Valor igualIgual(Valor v1, Valor v2)
        {
            Valor resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
            if (esEntero(v1) && esEntero(v2))
            {
                if (getEntero(v1) == getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                if (getEntero(v1) ==getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esEntero(v1) && esCaracter(v2))
            {
                if (getEntero(v1) == getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                if (getDecimal(v1) == getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                if (getDecimal(v1) == getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esCaracter(v2))
            {
                if (getDecimal(v1) == getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esCaracter(v1) && esCaracter(v2))
            {
                if (getCharNumero(v1) == getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esEntero(v2))
            {
                if (getCharNumero(v1) == getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esDecimal(v2))
            {
                if (getCharNumero(v1) == getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCadena(v1) && esCadena(v2))
            {
                if (getCadena(v1).CompareTo(getCadena(v2)) == 0)
                {
                    resp.valor = Constantes.VERDADERO;
                }
            }

            /*si se tuvieran que incluir los demas tipos*/
            return new Valor(Constantes.NULO, Constantes.NULO);
        }
        #endregion


        #region disntintoA
        private Valor distintoA(Valor v1, Valor v2)
        {
            Valor resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
            if (esEntero(v1) && esEntero(v2))
            {
                if (getEntero(v1) != getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                if (getEntero(v1) != getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esEntero(v1) && esCaracter(v2))
            {
                if (getEntero(v1) != getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                if (getDecimal(v1) != getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                if (getDecimal(v1) != getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esDecimal(v1) && esCaracter(v2))
            {
                if (getDecimal(v1) != getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;
            }
            else if (esCaracter(v1) && esCaracter(v2))
            {
                if (getCharNumero(v1) != getCharNumero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esEntero(v2))
            {
                if (getCharNumero(v1) != getEntero(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCaracter(v1) && esDecimal(v2))
            {
                if (getCharNumero(v1) != getDecimal(v2))
                {
                    resp.valor = Constantes.VERDADERO;
                }
                return resp;

            }
            else if (esCadena(v1) && esCadena(v2))
            {
                if (!(getCadena(v1).Equals(getCadena(v2))))
                {
                    resp.valor = Constantes.VERDADERO;
                }
            }

            /*si se tuvieran que incluir los demas tipos*/
            return new Valor(Constantes.NULO, Constantes.NULO);
        }
        #endregion


        #endregion


        #region Aritmeticas

        #region Potencia

        private Valor potencia(Valor v1, Valor v2)
        {
            Valor resp;
            if (esBool(v1) && esEntero(v2))
            {
                int a = getBooleanoNumero(v1);
                int b = getEntero(v2);
                double c = Math.Pow(a , b);
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esBool(v1) && esDecimal(v2))
            {
                int a = getBooleanoNumero(v1);
                double b = getDecimal(v2);
                double c = Math.Pow(a, b);
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esEntero(v1) && esEntero(v2))
            {
                int a = getEntero(v1);
                int b = getEntero(v2);
                double c = Math.Pow(a, b);
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                int a = getEntero(v1);
                double b = getDecimal(v2);
                double c = Math.Pow(a, b);
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                int a = getEntero(v2);
                double b = getDecimal(v1);
                double c = Math.Pow(b, a);
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                double a = getDecimal(v1);
                double b = getDecimal(v2);
                double c = Math.Pow(a, b);
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;
            }
            else if (esDecimal(v1) && esBool(v2))
            {
                double a = getDecimal(v1);
                int b = getBooleanoNumero(v2);
                double c = Math.Pow(a, b);
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esEntero(v1) && esBool(v2))
            {
                int a = getBooleanoNumero(v2);
                int b = getEntero(v1);
                double c = Math.Pow(b, a);
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            return new Valor(Constantes.NULO, Constantes.NULO);
        }
        #endregion


        #region modulo

        private Valor modulo(Valor v1, Valor v2)
        {
            Valor resp;
           if (esEntero(v1) && esEntero(v2))
            {
                int a = getEntero(v1);
                int b = getEntero(v2);
                double c = a % b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                int a = getEntero(v1);
                double b = getDecimal(v2);
                double c = a % b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                int a = getEntero(v2);
                double b = getDecimal(v1);
                double c = b % a;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                double a = getDecimal(v1);
                double b = getDecimal(v2);
                double c = a % b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;
            }
           
            return new Valor(Constantes.NULO, Constantes.NULO);
        }
        #endregion



        #region Divisiones

        private Valor dividir(Valor v1, Valor v2)
        {
            Valor resp;
            if (esBool(v1) && esEntero(v2))
            {
                int a = getBooleanoNumero(v1);
                int b = getEntero(v2);
                if (!esCero(b))
                {
                    int c = a / b;
                    resp = new Valor(Constantes.ENTERO, c);
                    return resp;
                }
                else
                {

                }
                
            }
            else if (esBool(v1) && esDecimal(v2))
            {
                int a = getBooleanoNumero(v1);
                double b = getDecimal(v2);
                if (!esCero(b))
                {
                    double c = a / b;
                    resp = new Valor(Constantes.DECIMAL, c);
                    return resp;

                }
                else
                {

                }
               

            }
            else if (esEntero(v1) && esEntero(v2))
            {
                int a = getEntero(v1);
                int b = getEntero(v2);
                if (!esCero(b))
                {
                    int c = a / b;
                    resp = new Valor(Constantes.ENTERO, c);
                    return resp;

                }
                else
                {

                }
                

            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                int a = getEntero(v1);
                double b = getDecimal(v2);
                if (!esCero(b))
                {
                    double c = a / b;
                    resp = new Valor(Constantes.DECIMAL, c);
                    return resp;

                }
                else
                {

                }
                

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                int a = getEntero(v2);
                double b = getDecimal(v1);
                if (!esCero(b))
                {
                    double c = b / a;
                    resp = new Valor(Constantes.DECIMAL, c);
                    return resp;

                }
                else
                {

                }
               

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                double a = getDecimal(v1);
                double b = getDecimal(v2);
                if (!esCero(b))
                {
                    double c = a / b;
                    resp = new Valor(Constantes.DECIMAL, c);
                    return resp;

                }
                else
                {

                }
                
            }
            else if (esDecimal(v1) && esBool(v2))
            {
                double a = getDecimal(v1);
                int b = getBooleanoNumero(v2);
                if (!esCero(b))
                {
                    double c = a / b;
                    resp = new Valor(Constantes.DECIMAL, c);
                    return resp;

                }
                else
                {

                }
                

            }
            else if (esEntero(v1) && esBool(v2))
            {
                int a = getBooleanoNumero(v2);
                int b = getEntero(v1);
                if (!esCero(b))
                {
                    int c = b / a;
                    resp = new Valor(Constantes.ENTERO, c);
                    return resp;

                }
                else
                {

                }
                

            }
            return new Valor(Constantes.NULO, Constantes.NULO);
        }

        #endregion



        #region multiplicaciones

        private Valor multiplicar(Valor v1, Valor v2)
        {
            Valor resp;
            if (esBool(v1) && esEntero(v2))
            {
                int a = getBooleanoNumero(v1);
                int b = getEntero(v2);
                int c = a * b;
                resp = new Valor(Constantes.ENTERO, c);
                return resp;

            }
            else if (esBool(v1) && esDecimal(v2))
            {
                int a = getBooleanoNumero(v1);
                double b = getDecimal(v2);
                double c = a * b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esEntero(v1) && esEntero(v2))
            {
                int a = getEntero(v1);
                int b = getEntero(v2);
                int c = a * b;
                resp = new Valor(Constantes.ENTERO, c);
                return resp;

            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                int a = getEntero(v1);
                double b = getDecimal(v2);
                double c = a * b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                int a = getEntero(v2);
                double b = getDecimal(v1);
                double c = b * a;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                double a = getDecimal(v1);
                double b = getDecimal(v2);
                double c = a * b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;
            }
            else if (esDecimal(v1) && esBool(v2))
            {
                double a = getDecimal(v1);
                int b = getBooleanoNumero(v2);
                double c = a * b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esEntero(v1) && esBool(v2))
            {
                int a = getBooleanoNumero(v2);
                int b = getEntero(v1);
                int c = b * a;
                resp = new Valor(Constantes.ENTERO, c);
                return resp;

            }
            else if (esBool(v1) && esBool(v2))
            {
                int a = getBooleanoNumero(v2);
                int b = getBooleanoNumero(v1);
                int c = b + a;
                if (c == 2)
                {
                    resp = new Valor(Constantes.BOOLEANO, Constantes.VERDADERO);
                }
                else
                {
                    resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);

                }
                return resp;

            }

            return new Valor(Constantes.NULO, Constantes.NULO);
        }

        #endregion



        #region restas

        private Valor restar(Valor v1, Valor v2)
        {
            Valor resp;
            if (esBool(v1) && esEntero(v2))
            {
                int a = getBooleanoNumero(v1);
                int b = getEntero(v2);
                int c = a - b;
                resp = new Valor(Constantes.ENTERO, c);
                return resp;

            }
            else if (esBool(v1) && esDecimal(v2))
            {
                int a = getBooleanoNumero(v1);
                double b = getDecimal(v2);
                double c = a - b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esEntero(v1) && esEntero(v2))
            {
                int a = getEntero(v1);
                int b = getEntero(v2);
                int c = a - b;
                resp = new Valor(Constantes.ENTERO, c);
                return resp;

            }
            else if (esEntero(v1) && esDecimal(v2))
            {
                int a = getEntero(v1);
                double b = getDecimal(v2);
                double c = a - b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esDecimal(v1) && esEntero(v2))
            {
                int a = getEntero(v2);
                double b = getDecimal(v1);
                double c = b-a;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esDecimal(v1) && esDecimal(v2))
            {
                double a = getDecimal(v1);
                double b = getDecimal(v2);
                double c = a - b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;
            }
            else if (esDecimal(v1) && esBool(v2))
            {
                double a = getDecimal(v1);
                int b = getBooleanoNumero(v2);
                double c = a - b;
                resp = new Valor(Constantes.DECIMAL, c);
                return resp;

            }
            else if (esEntero(v1) && esBool(v2))
            {
                int a = getBooleanoNumero(v2);
                int b = getEntero(v1);
                int c = b-a;
                resp = new Valor(Constantes.ENTERO, c);
                return resp;

            }
            return new Valor(Constantes.NULO, Constantes.NULO);
        }

        #endregion




        #region Sumas

        private Valor sumar(Valor v1, Valor v2)
        {
            Valor resp;
            /*--------------- Validaciones con booleanos -------------------*/
            if (esBool(v1) && esBool(v2))
            {
                int b1 = getBooleanoNumero(v1);
                int b2 = getBooleanoNumero(v2);
                int r = b1 + b2;
                if (r > 0)
                {
                    resp = new Valor(Constantes.BOOLEANO, Constantes.VERDADERO);
                    return resp;
                }
                else
                {
                    resp = new Valor(Constantes.BOOLEANO, Constantes.FALSO);
                    return resp;
                }
            }
            else if (esBool(v1) && esEntero(v2))
            {
                int b1 = getBooleanoNumero(v1);
                int e1 = getEntero(v2);
                int r = b1 + e1;
                resp = new Valor(Constantes.ENTERO, r);
                return resp;
            }
            else if (esBool(v1) && esDecimal(v2))
            {
                int b1 = getBooleanoNumero(v1);
                double d2 = getDecimal(v2);
                double v = b1 + d2;
                resp = new Valor(Constantes.DECIMAL, v);
                return resp;
            }
            else if (esBool(v1) && esCadena(v2))
            {
                int b1 = getBooleanoNumero(v1);
                string c1 = getCadena(v2);
                resp = new Valor(Constantes.CADENA, b1 + c1 + "");
                return resp;
            }

            //validaciones con enteros

            else if (esEntero(v1) && esBool(v2))
            {
                int b1 = getEntero(v1);
                int b2 = getBooleanoNumero(v2);
                int c3 = b1 + b2;
                resp = new Valor(Constantes.ENTERO, c3);
                return resp;

            }else if(esEntero(v1) && esEntero(v2)){
                int b1 = getEntero(v1);
                int b2 = getEntero(v2);
                int c3 = b1 + b2;
                resp = new Valor(Constantes.ENTERO, c3);
                return resp;

            }else if(esEntero(v1) && esDecimal(v2)){
                int b1 = getEntero(v1);
                double b2 = getDecimal(v2);
                double c3 = b1 + b2;
                resp = new Valor(Constantes.DECIMAL, c3);
                return resp;

            }else if(esEntero(v1) && esCadena(v2)){
                int b1 = getEntero(v1);
                string b2 = getCadena(v2);
                String c3 = b1 + b2+"";
                resp = new Valor(Constantes.CADENA, c3);
                return resp;
            }
            /*--------------------- Validaciones con Decimales -----------------------------------*/
            else if(esDecimal(v1) && esBool(v2)){
                double d1 = getDecimal(v1);
                int b1 = getBooleanoNumero(v2);
                double d2 = d1 + b1;
                resp = new Valor(Constantes.DECIMAL, d2);
                return resp;

            }
            else if(esDecimal(v1) && esEntero(v2)){
                double d1 = getDecimal(v1);
                int b1 = getEntero(v2);
                double d2 = d1 + b1;
                resp = new Valor(Constantes.DECIMAL, d2);
                return resp;

            }
            else if(esDecimal(v1) && esDecimal(v2)){
                double d1 = getDecimal(v1);
                double b1 = getDecimal(v2);
                double d2 = d1 + b1;
                resp = new Valor(Constantes.DECIMAL, d2);
                return resp;
            }
            else if(esDecimal(v1) && esCadena(v2)){
                double b1 = getDecimal(v1);
                string b2 = getCadena(v2);
                String c3 = b1 + b2 + "";
                resp = new Valor(Constantes.CADENA, c3);
                return resp;
            }

            /*-------------------- Validaciones con cadenas --------------------------------*/
            else if(esCadena(v1) && esBool(v2) ){
                resp = new Valor(Constantes.CADENA, getCadena(v1) + getBooleanoLetra(v2) + "");
                return resp;

            }else if(esCadena(v1) && esEntero(v2)){
                resp = new Valor(Constantes.CADENA, getCadena(v1) + getEntero(v2) + "");
                return resp;

            }else if(esCadena(v1) && esDecimal(v2)){
                resp = new Valor(Constantes.CADENA, getCadena(v1) + getDecimal(v2) + "");
                return resp;

            }else if(esCadena(v1) && esCadena(v2)){
                resp = new Valor(Constantes.CADENA, getCadena(v1) + getCadena(v2) + "");
                return resp;
            }
            else if (esCadena(v1) && esHora(v2))
            {
                resp = new Valor(Constantes.CADENA, getCadena(v1) + getHora(v2) + "");
                return resp;

            }
            else if (esCadena(v1) && esFecha(v2))
            {
                resp = new Valor(Constantes.CADENA, getCadena(v1) + getFecha(v2) + "");
                return resp;

            }
            else if (esCadena(v1) && esFechaHora(v2))
            {
                resp = new Valor(Constantes.CADENA, getCadena(v1) + getFechaHora(v2) + "");
                return resp;

            }

            /*--------------------- Validaciones con Fechas/ horas/ fechas-Hora -----------------------*/

            else if (esFecha(v1) && esCadena(v2))
            {
                resp = new Valor(Constantes.CADENA, getFecha(v1) + getCadena(v2) + "");
                return resp;

            }
            else if (esHora(v1) && esCadena(v2))
            {
                resp = new Valor(Constantes.CADENA, getHora(v1) + getCadena(v2) + "");
                return resp;

            }
            else if (esFechaHora(v1) && esCadena(v2))
            {
                resp = new Valor(Constantes.CADENA, getFechaHora(v1) + getCadena(v2) + "");
                return resp;

            }


            return new Valor(Constantes.NULO, Constantes.NULO);
        }


        #endregion


        #endregion


        #region Extras

        private bool esCero(double valor)
        {
            return (valor == 0);
        }

        private bool esFecha(Valor v)
        {
            return v.tipo.Equals(Constantes.FECHA);
        }
        private bool esFechaHora(Valor v)
        {
            return v.tipo.Equals(Constantes.FECHA_HORA);
        }

        private bool esHora(Valor v)
        {
            return v.tipo.Equals(Constantes.HORA);
        }


        private bool esEntero(Valor v){
            return v.tipo.Equals(Constantes.ENTERO);
        }

        private bool esBool(Valor v)
        {
            return v.tipo.Equals(Constantes.BOOLEANO);
        }

        private bool esDecimal(Valor v)
        {
            return v.tipo.Equals(Constantes.DECIMAL);
        }

        private bool esCadena(Valor v)
        {
            return v.tipo.Equals(Constantes.CADENA);
        }

        private bool esCaracter(Valor v)
        {
            return v.tipo.Equals(Constantes.CARACTER);
        }

        private bool esNulo(Valor v)
        {
            return v.tipo.Equals(Constantes.NULO);
        }


        // obtener valores 
        private String getHora(Valor v)
        {
            Hora g = (Hora)v.valor;
            return g.valHoraCadena;
        }

        private String getFecha(Valor v)
        {
            Fecha g = (Fecha)v.valor;
            return g.valFechaCadena;
        }

        private String getFechaHora(Valor v)
        {
            FechaHora g = (FechaHora)v.valor;
            return g.cadenaRealFechaHora;
        }

        private String getCadena(Valor v)
        {
            return v.valor.ToString();
        }

        private int getEntero(Valor v)
        {
            return int.Parse(v.valor.ToString());
        }

        private double getDecimal(Valor v)
        {
            return double.Parse(v.valor.ToString());
        }

        private char getChar(Valor v)
        {
            return char.Parse(v.valor.ToString());
        }

        private int getCharNumero(Valor v)
        {
            int val = (int)Char.GetNumericValue(char.Parse(v.valor.ToString()));
            return val;
        }
        private string getBooleanoLetra(Valor v)
        {
            if (v.valor.ToString().ToLower().Equals(Constantes.VERDADERO.ToLower()))
            {
                return Constantes.VERDADERO;
            }
            else
            {
                return Constantes.FALSO;
            }
        }

        private int getBooleanoNumero(Valor v)
        {
            if (v.valor.ToString().ToLower().Equals(Constantes.VERDADERO.ToLower()))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }



        #endregion



    }
}
