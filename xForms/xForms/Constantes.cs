using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xForms.Errores;

namespace xForms
{
    class Constantes
    {
        public static ListaErrores erroresEjecucion = new ListaErrores();


        //PALABRAS RESERVADAS 
        public const string INSTANCIA_ARREGLO = "INSTANCIA_ARREGLO";
        public const string LISTA_ELEMENTOS_CLASEE = "LISTA_ELEMENTOS_CLASE";
        public const string CUERPO_CLASE = "CUERPO_CLASE";
        public const string LISTA_CLASES = "LISTA_CLASES";
        public const string ARCHIVO = "ARCHIVO";
        public const string CADENA = "cadena";
        public const string ENTERO = "entero";
        public const string BOOLEANO = "booleano";
        public const string CHAR = "char";
        public const string FECHA = "fecha";
        public const string DECIMAL = "decimal";
        public const string HORA = "hora";
        public const string FECHA_HORA = "fechahora";
        public const string RESPUESTA = "respuesta";
        public const string FALSO = "falso";
        public const string VERDADERO = "verdadero";

        public const string NEGRILLA = "NEGRILLA";
        public const string CURSIVA = "CURSIVA";
        public const string SUBRAYADO = "SUBRAYADO";
        public const string PUBLICO = "PUBLICO";
        public const string PROTEGIDO = "PROTEGIDO";
        public const string PRIVADO = "PRIVADO";

        public const string XFORM = "XFORM";
        public const string PADRE = "PADRE";
        public const string CLASE = "CLASE";
        public const string NUEVO = "NUEVO";
        public const string NULO = "NULO";
        public const string VACIO = "VACIO";
        public const string DE = "DE";
        public const string DEFECTO = "DEFECTO";
        public const string HEXADECIMAL = "HEXADECIMAL";
        public const string COLOR = "COLOR";
        public const string TAM = "TAM";
        public const string REPETIR = "REPETIR";
        public const string HASTA = "HASTA";
        public const string DEC_ASIG = "DEC_ASIG";
        
        //PRODUCCIONES
        public const string PAR_CORCHETES = "PAR_CORCHETES";
        public const string EXPRESION = "EXPRESION";
        public const string TIPO_DATOS = "TIPO_DATOS";
        public const string TIPO_ESTILO = "TIPO_ESTILO";
        public const string LISTA_ESTILO = "LISTA_ESTILO";
        public const string ESTILO = "ESTILO";
        public const string VISIBILIDAD = "VISIBILIDAD";
        public const string IMPORTAR = "IMPORTAR";
        public const string IMPORTACIONES = "IMPORTACIONES";
        public const string SUPER = "SUPER";
        public const string LISTA_EXPRESIONES = "LISTA_EXPRESIONES";
        public const string DECLA_ATRIBUTO = "DECLA_ATRIBUTO";
        public const string DECLA_VARIABLE = "DECLA_VARIABLE";
        public const string ASIGNACION = "ASIGNACION";
        public const string DECLA_ARREGLO = "DECLA_ARREGLO";
        public const string LLAVE_EXPRESION = "LLAVE_EXPRESION";
        public const string LISTA_LLAVE_EXPRESION = "LISTA_LLAVE_EXPRESION";
        public const string L_LLAVES_EXPRESION = "L_LLAVES_EXPRESION";
        public const string L_CORCHETES_EXPRESION = "L_CORCHETES_EXPRESION";
        public const string L_CORCHETES_VACIOS = "L_CORCHETES_VACIOS";
        public const string LLAMADA_PARAMETROS = "LLAMADA_PARAMETROS";
        public const string PARAMETROS = "PARAMETROS";
        public const string LISTA_PARAMETROS = "LISTA_PARAMETROS";
        public const string PARAMETRO = "PARAMETRO";
        public const string INSTANCIA = "INSTANCIA";
        public const string FUNCION = "FUNCION";
        public const string CUERPO_FUNCION = "CUERPO_FUNCION";
        public const string RETORNO = "RETORNO";
        public const string PRINCIPAL = "PRINCIPAL";
        public const string CONSTRUCTOR = "CONSTRUCTOR";
        public const string L_SINO_SI = "L_SINO_SI";
        public const string S_SI = "S_SI";
        public const string SINO = "SINO";
        public const string SINO_SI = "SINO_SI";
        public const string SI = "SI";
        public const string ROMPER = "ROMPER";
        public const string CASO = "CASO";
        public const string CUERPO_CASO = "CUERPO_CASO";
        public const string OPCION_VALOR = "OPCION_VALOR";
        public const string LISTA_CASO = "LISTA_CASO";
        public const string VALOR = "VALOR";
        public const string SI_SIMPLIFICADO = "SI_SIMPLIFICADO";
        public const string CONTINUAR = "CONTINUAR";
        public const string MIENTRAS = "MIENTRAS";
        public const string HACER = "HACER";
        public const string REPETIR_HASTA = "REPETIR_HASTA";
        public const string PARA = "PARA";
        public const string IMPRIMIR = "IMPRIMIR";
        public const string MENSAJE = "MENSAJE";
        public const string PARAMETROS_LLAMADA = "PARAMETROS_LLAMADA";
        public const string LLAMADA = "LLAMADA";
        public const string PAR_CORCHETES_EXPRESION = "PAR_CORCHETES_EXPRESION";
        public const string ACCESO = "ACCESO";
        public const string SENTENCIA = "SENTENCIA";
        public const string SENTENCIAS = "SENTENCIAS";
        public const string ARITMETICA = "ARITMETICA";
        public const string RELACIONAL = "RELACIONAL";
        public const string LOGICA = "LOGICA";
       


        //SIMBOLOS
        public const string ABRE_COR = "[";
        public const string CIERRA_COR = "]";
        public const string MAS = "+";
        public const string ID = "id";
        public const string CARACTER = "CARACTER";
        public const string COMA = ",";
        public const string ABRE_LLAVE = "{";
        public const string CIERRA_LLAVE = "}";
        public const string ABRE_PAR = "(";
        public const string CIERRA_PAR = ")";
        public const string ARROBA = "@";
        public const string DOS_PUNTOS = ":";
        public const string PUNTO_COMA = ";";
        public const string MENOS = "-";
        public const string MULTIPLICACION = "*";
        public const string DIVISION = "/";
        public const string POTENCIA = "^";
        public const string MODULO = "%";
        public const string ADICION = "++";
        public const string SUSTRACCION = "--";
        public const string IGUAL_IGUAL = "==";
        public const string DISTINTO_A = "!=";
        public const string MENOR_IGUAL = "<=";
        public const string MAYOR_IGUAL = ">=";
        public const string MENOR = "<";
        public const string MAYOR = ">";
        public const string AND = "&&";
        public const string OR = "||";
        public const string NOT = "!";
        public const string IGUAL = "=";
        public const string PUNTO = ".";
        public const string INTERROGACION = "?";
        public const string NEGATIVO = "NEGATIVO";
        public const string NOT2 = "NOT";
        public const string POS_ARREGLO = "POS_ARREGLO";
        public const string MAS_MAS = "++";
        public const string MENOS_MENOS = "--";
        public const string ASIGNA_UNARIO = "ASIGNA_UNARIO";
        public const string UNARIO = "UNARIO";


        public const string ERROR_SEMANTICO = "SEMANTICO";
        public const string ERROR_SINTACTICO = "SINTACTICO";
        public const string ERROR_LEXICO = "LEXICO";






    }
}
