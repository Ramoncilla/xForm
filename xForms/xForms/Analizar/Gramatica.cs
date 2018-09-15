
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;
using Irony.Interpreter;



namespace xForms.Analizar
{
    class Gramatica:Grammar
    {
        public Gramatica()
            : base(caseSensitive: false)
        {


            #region Expresioes
            var doble = new RegexBasedTerminal(Constantes.DECIMAL, "[0-9]+[.][0-9]+");
            var entero = new RegexBasedTerminal(Constantes.ENTERO, "[0-9]+");
            var identificador = TerminalFactory.CreateCSharpIdentifier(Constantes.ID);
            //var numero = TerminalFactory.CreateCSharpNumber("numero");
            StringLiteral cadena = new StringLiteral(Constantes.CADENA, "\"");
            var caracter = TerminalFactory.CreateCSharpChar(Constantes.CHAR);
            var hexadecimal = new RegexBasedTerminal(Constantes.HEXADECIMAL, "#([a-fA-F]|[0-9])+");
            var fecha = new RegexBasedTerminal(Constantes.FECHA, "(([\"]|['])([ ]*)([0-9][0-9])[/]([0-9][0-9])[/]([0-9][0-9][0-9][0-9])[ ]*([\"]|[']))");
            var hora = new RegexBasedTerminal(Constantes.HORA, "(([\"]|['])([ ]*)([0-9][0-9])[:]([0-9][0-9])[:]([0-9][0-9])[ ]*([\"]|[']))");
            var fechaHora = new RegexBasedTerminal(Constantes.FECHA_HORA, "(([\"]|['])([ ]*)([0-9][0-9])[/]([0-9][0-9])[/]([0-9][0-9][0-9][0-9])([ ])(([0-9][0-9])[:]([0-9][0-9])[:]([0-9][0-9]))[ ]*([\"]|[']))");

            CommentTerminal COMENT_BLOQUE = new CommentTerminal("COMENTARIO BLOQUE", "$#", "#$");
            CommentTerminal COMENT_LINEA = new CommentTerminal("COMENTARIO LINEA", "$$", "\n", "\r\n");
            NonGrammarTerminals.Add(COMENT_BLOQUE);
            NonGrammarTerminals.Add(COMENT_LINEA);
            #endregion

            #region Declaracion de No terminales
            NonTerminal NEGATIVO = new NonTerminal(Constantes.NEGATIVO);
            NonTerminal NOT = new NonTerminal(Constantes.NOT2);
            NonTerminal PAR_CORCHETES = new NonTerminal(Constantes.PAR_CORCHETES);
            NonTerminal TIPO_DATOS = new NonTerminal(Constantes.TIPO_DATOS);
            NonTerminal TIPO_ESTILO = new NonTerminal(Constantes.TIPO_ESTILO);
            NonTerminal LISTA_ESTILO = new NonTerminal(Constantes.LISTA_ESTILO);
            NonTerminal VISIBILIDAD = new NonTerminal(Constantes.VISIBILIDAD);
            NonTerminal IMPORTAR = new NonTerminal(Constantes.IMPORTAR);
            NonTerminal IMPORTACIONES = new NonTerminal(Constantes.IMPORTACIONES);
            NonTerminal ESTILO = new NonTerminal(Constantes.ESTILO);
            NonTerminal SUPER = new NonTerminal(Constantes.SUPER);
            NonTerminal LISTA_EXPRESIONES = new NonTerminal(Constantes.LISTA_EXPRESIONES);
            NonTerminal EXPRESION = new NonTerminal(Constantes.EXPRESION);
            NonTerminal PARAMETROS_LLAMADA= new NonTerminal(Constantes.PARAMETROS_LLAMADA);
            NonTerminal DECLA_ATRIBUTO = new NonTerminal(Constantes.DECLA_ATRIBUTO);
            NonTerminal DECLA_VARIABLE = new NonTerminal(Constantes.DECLA_VARIABLE);
            NonTerminal ASIGNACION = new NonTerminal(Constantes.ASIGNACION);
            NonTerminal DECLA_ARREGLO = new NonTerminal(Constantes.DECLA_ARREGLO);
            NonTerminal LLAVE_EXPRESION = new NonTerminal(Constantes.LLAVE_EXPRESION);
            NonTerminal LISTA_LLAVE_EXPRESION = new NonTerminal(Constantes.LISTA_LLAVE_EXPRESION);
            NonTerminal L_LLAVES_EXPRESION = new NonTerminal(Constantes.L_LLAVES_EXPRESION);
            NonTerminal L_CORCHETES_EXPRESION = new NonTerminal(Constantes.L_CORCHETES_EXPRESION);
            NonTerminal L_CORCHETES_VACIOS = new NonTerminal(Constantes.L_CORCHETES_VACIOS);
            NonTerminal LLAMADA_PARAMETROS = new NonTerminal(Constantes.LLAMADA_PARAMETROS);
            NonTerminal PARAMETROS = new NonTerminal(Constantes.PARAMETROS);
            NonTerminal LISTA_PARAMETROS = new NonTerminal(Constantes.LISTA_PARAMETROS);
            NonTerminal PARAMETRO = new NonTerminal(Constantes.PARAMETRO);
            NonTerminal INSTANCIA = new NonTerminal(Constantes.INSTANCIA);
            NonTerminal FUNCION = new NonTerminal(Constantes.FUNCION);
            NonTerminal CUERPO_FUNCION = new NonTerminal(Constantes.CUERPO_FUNCION);
            NonTerminal RETORNO = new NonTerminal(Constantes.RETORNO);
            NonTerminal PRINCIPAL = new NonTerminal(Constantes.PRINCIPAL);
            NonTerminal CONSTRUCTOR = new NonTerminal(Constantes.CONSTRUCTOR);
            NonTerminal S_SI = new NonTerminal(Constantes.S_SI);
            NonTerminal SINO = new NonTerminal(Constantes.SINO);
            NonTerminal SINO_SI = new NonTerminal(Constantes.SINO_SI);
            NonTerminal SI = new NonTerminal(Constantes.SI);
            NonTerminal ROMPER = new NonTerminal(Constantes.ROMPER);
            NonTerminal CASO = new NonTerminal(Constantes.CASO);
            NonTerminal CUERPO_CASO = new NonTerminal(Constantes.CUERPO_CASO);
            NonTerminal OPCION_VALOR = new NonTerminal(Constantes.OPCION_VALOR);
            NonTerminal LISTA_CASO = new NonTerminal(Constantes.LISTA_CASO);
            NonTerminal VALOR = new NonTerminal(Constantes.VALOR);
            NonTerminal SI_SIMPLIFICADO = new NonTerminal(Constantes.SI_SIMPLIFICADO);
            NonTerminal CONTINUAR = new NonTerminal(Constantes.CONTINUAR);
            NonTerminal MIENTRAS = new NonTerminal(Constantes.MIENTRAS);
            NonTerminal HACER = new NonTerminal(Constantes.HACER);
            NonTerminal REPETIR_HASTA = new NonTerminal(Constantes.REPETIR_HASTA);
            NonTerminal PARA = new NonTerminal(Constantes.PARA);
            NonTerminal IMPRIMIR = new NonTerminal(Constantes.IMPRIMIR);
            NonTerminal MENSAJE = new NonTerminal(Constantes.MENSAJE);
            NonTerminal LLAMADA = new NonTerminal(Constantes.LLAMADA);
            NonTerminal PAR_CORCHETES_EXPRESION = new NonTerminal(Constantes.PAR_CORCHETES_EXPRESION);
            NonTerminal L_SINO_SI = new NonTerminal(Constantes.L_SINO_SI);
            NonTerminal DEFECTO = new NonTerminal(Constantes.DEFECTO);
             NonTerminal ACCESO = new NonTerminal(Constantes.ACCESO);
             NonTerminal DEC_ASIG = new NonTerminal(Constantes.DEC_ASIG);
             NonTerminal SENTENCIA = new NonTerminal(Constantes.SENTENCIA);
             NonTerminal SENTENCIAS = new NonTerminal(Constantes.SENTENCIAS);
             NonTerminal ARITMETICA = new NonTerminal(Constantes.ARITMETICA);
             NonTerminal RELACIONAL = new NonTerminal(Constantes.RELACIONAL);
             NonTerminal LOGICA = new NonTerminal(Constantes.LOGICA);
             NonTerminal OP_LOGICO = new NonTerminal("OP_LOGICO");
             NonTerminal OP_RELACIONAL = new NonTerminal("OP_RELACIONAL");
             NonTerminal OP_ARITMETICO = new NonTerminal("OP_ARITMETICO");
             NonTerminal DECIMAL = new NonTerminal(Constantes.DECIMAL);
             NonTerminal CADENA = new NonTerminal(Constantes.CADENA);
             NonTerminal ENTERO = new NonTerminal(Constantes.ENTERO);
            NonTerminal CARACTER = new NonTerminal(Constantes.CARACTER);
            NonTerminal FECHA = new NonTerminal(Constantes.FECHA);
            NonTerminal FECHA_HORA = new NonTerminal(Constantes.FECHA_HORA);
            NonTerminal HORA = new NonTerminal(Constantes.HORA);
            NonTerminal BOOLEANO = new NonTerminal(Constantes.BOOLEANO);
            NonTerminal NULO = new NonTerminal(Constantes.NULO);
            NonTerminal TERMINO = new NonTerminal("TERMINIO");
            NonTerminal ID = new NonTerminal(Constantes.ID);
            NonTerminal POS_ARREGLO = new NonTerminal(Constantes.POS_ARREGLO);
            NonTerminal ELEMENTO_ACCESO = new NonTerminal("ELEMENTO_ACCESO");
            NonTerminal CLASE = new NonTerminal(Constantes.CLASE);
            NonTerminal ELEMENTO_CLASE = new NonTerminal("ELEMENTO_CLASE");
            NonTerminal LISTA_ELEMENTOS_CLASE = new NonTerminal(Constantes.LISTA_ELEMENTOS_CLASEE);
            NonTerminal CUERPO_CLASE = new NonTerminal(Constantes.CUERPO_CLASE);
            NonTerminal ARCHIVO = new NonTerminal(Constantes.ARCHIVO);
            NonTerminal LISTA_CLASES = new NonTerminal(Constantes.LISTA_CLASES);
            NonTerminal INSTANCIA_ARREGLO= new NonTerminal(Constantes.INSTANCIA_ARREGLO);
            NonTerminal ASIGNA_UNARIO = new NonTerminal(Constantes.ASIGNA_UNARIO);
            NonTerminal UNARIO = new NonTerminal(Constantes.UNARIO);


            NonTerminal FUN_CADENA = new NonTerminal(Constantes.FUN_CADENA);
            NonTerminal FUN_SUB_CAD = new NonTerminal(Constantes.FUN_SUB_CAD);
            NonTerminal FUN_POS = new NonTerminal(Constantes.FUN_POS);
            NonTerminal FUN_BOOL = new NonTerminal(Constantes.FUN_BOOL);
            NonTerminal FUN_ENTERO = new NonTerminal(Constantes.FUN_ENTERO);
            NonTerminal FUN_TAM = new NonTerminal(Constantes.FUN_TAM);
            NonTerminal FUN_RANDOM = new NonTerminal(Constantes.FUN_RANDOM);
            NonTerminal FUN_MIN = new NonTerminal(Constantes.FUN_MIN);
            NonTerminal FUN_MAX = new NonTerminal(Constantes.FUN_MAX);
            NonTerminal FUN_POW = new NonTerminal(Constantes.FUN_POW);
            NonTerminal FUN_LOG = new NonTerminal(Constantes.FUN_LOG);
            NonTerminal FUN_LOG10 = new NonTerminal(Constantes.FUN_LOG10);
            NonTerminal FUN_ABS = new NonTerminal(Constantes.FUN_ABS);
            NonTerminal FUN_SIN = new NonTerminal(Constantes.FUN_SIN);
            NonTerminal FUN_COS = new NonTerminal(Constantes.FUN_COS);
            NonTerminal FUN_TAN = new NonTerminal(Constantes.FUN_TAN);
            NonTerminal FUN_SQRT = new NonTerminal(Constantes.FUN_SQRT);
            NonTerminal FUN_PI = new NonTerminal(Constantes.FUN_PI);
            NonTerminal FUN_HOY = new NonTerminal(Constantes.FUN_HOY);
            NonTerminal FUN_AHORA = new NonTerminal(Constantes.FUN_AHORA);
            NonTerminal FUN_FECHA = new NonTerminal(Constantes.FUN_FECHA);
            NonTerminal FUN_HORA = new NonTerminal(Constantes.FUN_HORA);
            NonTerminal FUN_FECHA_HORA = new NonTerminal(Constantes.FUN_FECHA_HORA);
            NonTerminal FUN_IMAGEN = new NonTerminal(Constantes.FUN_IMAGEN);
            NonTerminal FUN_VIDEO = new NonTerminal(Constantes.FUN_VIDEO);
            NonTerminal FUN_AUDIO = new NonTerminal(Constantes.FUN_AUDIO);
            NonTerminal FUN_MENSAJE = new NonTerminal(Constantes.FUN_MENSAJE);
            NonTerminal FUN_NATICA = new NonTerminal("FUN_NATIVA");


            #endregion

            #region Gramatica

            //falta la produccion de estilo

            ARCHIVO.Rule = IMPORTACIONES + LISTA_CLASES
                | LISTA_CLASES;

            LISTA_CLASES.Rule = MakePlusRule(LISTA_CLASES, CLASE);

            CLASE.Rule = ToTerm(Constantes.CLASE) + identificador + VISIBILIDAD + CUERPO_CLASE
                | ToTerm(Constantes.CLASE) + identificador + VISIBILIDAD + ToTerm(Constantes.PADRE) + identificador + CUERPO_CLASE;

            CUERPO_CLASE.Rule = ToTerm(Constantes.ABRE_LLAVE) + Constantes.CIERRA_LLAVE
                | ToTerm(Constantes.ABRE_LLAVE) + LISTA_ELEMENTOS_CLASE + Constantes.CIERRA_LLAVE;
           

            LISTA_ELEMENTOS_CLASE.Rule = MakePlusRule(LISTA_ELEMENTOS_CLASE, ELEMENTO_CLASE);


            ELEMENTO_CLASE.Rule = DECLA_ATRIBUTO
                | PRINCIPAL
                | CONSTRUCTOR
                | FUNCION;


            LISTA_ESTILO.Rule = MakePlusRule(LISTA_ESTILO, ToTerm(Constantes.COMA), ESTILO);

            TIPO_ESTILO.Rule = ToTerm(Constantes.NEGRILLA)
                | ToTerm(Constantes.COLOR) + Constantes.DOS_PUNTOS + hexadecimal
                | ToTerm(Constantes.TAM) + Constantes.DOS_PUNTOS + entero
                | ToTerm(Constantes.CURSIVA)
                | ToTerm(Constantes.SUBRAYADO);


            TIPO_DATOS.Rule = ToTerm(Constantes.BOOLEANO)
                | ToTerm(Constantes.CADENA)
                | ToTerm(Constantes.CHAR)
                | ToTerm(Constantes.DECIMAL)
                | ToTerm(Constantes.ENTERO)
                | ToTerm(Constantes.FECHA)
                | ToTerm(Constantes.HORA)
                | ToTerm(Constantes.FECHA_HORA)
                | ToTerm(Constantes.RESPUESTA)
                | identificador;

            VISIBILIDAD.Rule = ToTerm(Constantes.PUBLICO)
                |ToTerm(Constantes.PROTEGIDO)
                |ToTerm(Constantes.PRIVADO);

            IMPORTAR.Rule = ToTerm(Constantes.IMPORTAR) + Constantes.ABRE_PAR + identificador + Constantes.PUNTO + Constantes.XFORM + Constantes.CIERRA_PAR + Constantes.PUNTO_COMA;

            IMPORTACIONES.Rule = MakeStarRule(IMPORTACIONES, IMPORTAR);
            
            LISTA_EXPRESIONES.Rule = MakeStarRule(LISTA_EXPRESIONES, ToTerm(Constantes.COMA), EXPRESION);

            PARAMETROS_LLAMADA.Rule = ToTerm(Constantes.ABRE_PAR)+ LISTA_EXPRESIONES + Constantes.CIERRA_PAR;

            SUPER.Rule = ToTerm(Constantes.SUPER) + PARAMETROS_LLAMADA + Constantes.PUNTO_COMA;

            PAR_CORCHETES.Rule = ToTerm(Constantes.ABRE_COR) + Constantes.CIERRA_COR;

            PAR_CORCHETES_EXPRESION.Rule = ToTerm(Constantes.ABRE_COR) + EXPRESION + Constantes.CIERRA_COR;

            L_CORCHETES_VACIOS.Rule = MakePlusRule(L_CORCHETES_VACIOS, PAR_CORCHETES);

            L_CORCHETES_EXPRESION.Rule = MakePlusRule(L_CORCHETES_EXPRESION, PAR_CORCHETES_EXPRESION);

            LLAVE_EXPRESION.Rule = ToTerm(Constantes.ABRE_LLAVE) + LISTA_EXPRESIONES + Constantes.CIERRA_LLAVE;

            LISTA_LLAVE_EXPRESION.Rule = MakePlusRule(LISTA_LLAVE_EXPRESION, ToTerm(Constantes.COMA), LLAVE_EXPRESION);

            L_LLAVES_EXPRESION.Rule = ToTerm(Constantes.ABRE_LLAVE) + LISTA_LLAVE_EXPRESION + Constantes.CIERRA_LLAVE;

            DECLA_ATRIBUTO.Rule = TIPO_DATOS + VISIBILIDAD + identificador + ToTerm(Constantes.PUNTO_COMA)
                | TIPO_DATOS + VISIBILIDAD + identificador + ToTerm(Constantes.IGUAL) + EXPRESION + Constantes.PUNTO_COMA
                | TIPO_DATOS + identificador + ToTerm(Constantes.IGUAL) + EXPRESION + Constantes.PUNTO_COMA
                | TIPO_DATOS + identificador + ToTerm(Constantes.PUNTO_COMA)
                | TIPO_DATOS + identificador + L_CORCHETES_VACIOS + ToTerm(Constantes.IGUAL) + EXPRESION + Constantes.PUNTO_COMA
                | TIPO_DATOS + identificador + L_CORCHETES_VACIOS + Constantes.PUNTO_COMA
                //| TIPO_DATOS + identificador + L_CORCHETES_VACIOS + ToTerm(Constantes.IGUAL) + L_LLAVES_EXPRESION + Constantes.PUNTO_COMA
                | TIPO_DATOS + VISIBILIDAD + identificador + L_CORCHETES_VACIOS + ToTerm(Constantes.IGUAL) + EXPRESION + Constantes.PUNTO_COMA
                | TIPO_DATOS + VISIBILIDAD + identificador + L_CORCHETES_VACIOS + ToTerm(Constantes.PUNTO_COMA);
                //| TIPO_DATOS + VISIBILIDAD + identificador + L_CORCHETES_VACIOS + ToTerm(Constantes.IGUAL) + L_LLAVES_EXPRESION + Constantes.PUNTO_COMA;


            DECLA_VARIABLE.Rule = TIPO_DATOS + identificador + ToTerm(Constantes.IGUAL) + EXPRESION
                | TIPO_DATOS + identificador;
	
            ASIGNACION.Rule= ACCESO + ToTerm(Constantes.IGUAL) +  EXPRESION + ToTerm(Constantes.PUNTO_COMA);


            DECLA_ARREGLO.Rule = TIPO_DATOS + identificador + L_CORCHETES_VACIOS + ToTerm(Constantes.IGUAL) + EXPRESION + ToTerm(Constantes.PUNTO_COMA)
	            | TIPO_DATOS+ identificador + L_CORCHETES_VACIOS+ ToTerm(Constantes.PUNTO_COMA)
	            | TIPO_DATOS+ identificador +  L_CORCHETES_VACIOS + ToTerm(Constantes.IGUAL) + L_LLAVES_EXPRESION + ToTerm(Constantes.PUNTO_COMA);
            
            
            PARAMETROS.Rule= //ToTerm(Constantes.ABRE_PAR)+ ToTerm(Constantes.CIERRA_PAR)
		            ToTerm(Constantes.ABRE_PAR)+ LISTA_PARAMETROS +ToTerm(Constantes.CIERRA_PAR);
		
            LISTA_PARAMETROS.Rule= MakeStarRule(LISTA_PARAMETROS,ToTerm(Constantes.COMA), DECLA_VARIABLE );

	
            PARAMETRO.Rule= TIPO_DATOS +identificador;
	
            INSTANCIA.Rule= ToTerm(Constantes.NUEVO)+ TIPO_DATOS+ PARAMETROS_LLAMADA;

            INSTANCIA_ARREGLO.Rule=  ToTerm(Constantes.NUEVO) +TIPO_DATOS +L_CORCHETES_EXPRESION;
	
            FUNCION.Rule= VISIBILIDAD +TIPO_DATOS+ identificador+ PARAMETROS+ CUERPO_FUNCION
	            |VISIBILIDAD +ToTerm(Constantes.VACIO) +identificador+ PARAMETROS+ CUERPO_FUNCION 
	            |TIPO_DATOS +identificador+ PARAMETROS +CUERPO_FUNCION
	            |ToTerm(Constantes.VACIO) +identificador +PARAMETROS +CUERPO_FUNCION;


            CUERPO_FUNCION.Rule = ToTerm(Constantes.ABRE_LLAVE) + Constantes.CIERRA_LLAVE
                | ToTerm(Constantes.ABRE_LLAVE) + SENTENCIAS + Constantes.CIERRA_LLAVE;

            RETORNO.Rule= ToTerm(Constantes.RETORNO)+ ToTerm(Constantes.PUNTO_COMA)
                | ToTerm(Constantes.RETORNO) + EXPRESION + ToTerm(Constantes.PUNTO_COMA);


                PRINCIPAL.Rule= ToTerm(Constantes.PRINCIPAL)+ ToTerm(Constantes.ABRE_PAR)+ ToTerm(Constantes.CIERRA_PAR)+ CUERPO_FUNCION;

                CONSTRUCTOR.Rule= identificador+ PARAMETROS +CUERPO_FUNCION
	                |ToTerm(Constantes.PUBLICO)+  identificador+ PARAMETROS +CUERPO_FUNCION;
	
                S_SI.Rule= ToTerm(Constantes.SI)+  ToTerm(Constantes.ABRE_PAR)+ EXPRESION +ToTerm(Constantes.CIERRA_PAR) +CUERPO_FUNCION;
                SINO.Rule= ToTerm(Constantes.SINO)+CUERPO_FUNCION;
                SINO_SI.Rule= ToTerm(Constantes.SINO)+ ToTerm(Constantes.SI)+ToTerm(Constantes.ABRE_PAR)+ EXPRESION +ToTerm(Constantes.CIERRA_PAR)+ CUERPO_FUNCION;


                L_SINO_SI.Rule= MakePlusRule(L_SINO_SI, SINO_SI);

	
                SI.Rule= S_SI +L_SINO_SI
	                |S_SI +L_SINO_SI+ SINO
	                |S_SI +SINO
	                |S_SI;
	
	
                ROMPER.Rule= ToTerm(Constantes.ROMPER)  + ToTerm(Constantes.PUNTO_COMA);

                CASO.Rule= ToTerm(Constantes.CASO) + ToTerm(Constantes.ABRE_PAR)+ EXPRESION +ToTerm(Constantes.CIERRA_PAR) +ToTerm(Constantes.DE) + CUERPO_CASO;

                CUERPO_CASO.Rule= ToTerm(Constantes.ABRE_LLAVE)+LISTA_CASO +  Constantes.CIERRA_LLAVE;
	
                OPCION_VALOR.Rule= VALOR
	                |DEFECTO;
	
                LISTA_CASO.Rule= MakeStarRule(LISTA_CASO, OPCION_VALOR);
	
                VALOR.Rule= EXPRESION +ToTerm(Constantes.DOS_PUNTOS)+ CUERPO_FUNCION;

                DEFECTO.Rule= ToTerm(Constantes.DEFECTO)  +ToTerm(Constantes.DOS_PUNTOS)+ CUERPO_FUNCION;

                SI_SIMPLIFICADO.Rule = EXPRESION + ToTerm(Constantes.INTERROGACION) + EXPRESION + ToTerm(Constantes.DOS_PUNTOS) + EXPRESION + ToTerm(Constantes.PUNTO_COMA);

                CONTINUAR.Rule = ToTerm(Constantes.CONTINUAR) + ToTerm(Constantes.PUNTO_COMA);

                MIENTRAS.Rule = ToTerm(Constantes.MIENTRAS) + ToTerm(Constantes.ABRE_PAR) + EXPRESION + ToTerm(Constantes.CIERRA_PAR) + CUERPO_FUNCION;

                HACER.Rule = ToTerm(Constantes.HACER) + CUERPO_FUNCION + ToTerm(Constantes.MIENTRAS) + ToTerm(Constantes.ABRE_PAR) + EXPRESION + ToTerm(Constantes.CIERRA_PAR) + ToTerm(Constantes.PUNTO_COMA);

                REPETIR_HASTA.Rule = ToTerm(Constantes.REPETIR) + CUERPO_FUNCION + ToTerm(Constantes.HASTA) + ToTerm(Constantes.ABRE_PAR) + EXPRESION + ToTerm(Constantes.CIERRA_PAR) + ToTerm(Constantes.PUNTO_COMA);

                PARA.Rule = ToTerm(Constantes.PARA) + ToTerm(Constantes.ABRE_PAR) + DEC_ASIG + EXPRESION + ToTerm(Constantes.PUNTO_COMA) + ASIGNA_UNARIO + ToTerm(Constantes.CIERRA_PAR) + CUERPO_FUNCION;

                IMPRIMIR.Rule = ToTerm(Constantes.IMPRIMIR) + ToTerm(Constantes.ABRE_PAR) + EXPRESION + ToTerm(Constantes.CIERRA_PAR) + ToTerm(Constantes.PUNTO_COMA);

                
                ASIGNA_UNARIO.Rule = identificador + ToTerm(Constantes.MAS_MAS)
                    | identificador + ToTerm(Constantes.MENOS_MENOS);


                DEC_ASIG.Rule = DECLA_VARIABLE
                    | ASIGNACION;

                SENTENCIA.Rule = FUN_MENSAJE
                    | IMPRIMIR
                    | PARA
                    | REPETIR_HASTA
                    | HACER
                    | MIENTRAS
                    | CONTINUAR
                    | SI_SIMPLIFICADO
                    | CASO
                    | ROMPER
                    | SI
                    | RETORNO
                    | ASIGNACION
                    | DECLA_ARREGLO
                    | DECLA_VARIABLE + ToTerm(Constantes.PUNTO_COMA)
                    | SUPER
                    | ACCESO + ToTerm(Constantes.PUNTO_COMA)
                    | ASIGNA_UNARIO + ToTerm(Constantes.PUNTO_COMA);


                SENTENCIAS.Rule = MakePlusRule(SENTENCIAS, SENTENCIA);

                //falta acceso
                #region Expresion

                EXPRESION.Rule = RELACIONAL
                    | ARITMETICA
                    | LOGICA
                    | TERMINO
                    | ACCESO
                    | INSTANCIA
                    | INSTANCIA_ARREGLO
                    | UNARIO;

           


            OP_ARITMETICO.Rule = ToTerm(Constantes.MAS)
                | ToTerm(Constantes.MENOS)
                | ToTerm(Constantes.MULTIPLICACION)
                | ToTerm(Constantes.DIVISION)
                | ToTerm(Constantes.MODULO)
                | ToTerm(Constantes.POTENCIA);

            OP_RELACIONAL.Rule = ToTerm(Constantes.MENOR)
                | ToTerm(Constantes.MAYOR)
                | ToTerm(Constantes.MENOR_IGUAL)
                | ToTerm(Constantes.MAYOR_IGUAL)
                | ToTerm(Constantes.DISTINTO_A)
                | ToTerm(Constantes.IGUAL_IGUAL);

            OP_LOGICO.Rule = ToTerm(Constantes.AND)
                | ToTerm(Constantes.OR);



            RELACIONAL.Rule = EXPRESION + OP_RELACIONAL + EXPRESION;
            LOGICA.Rule = EXPRESION + OP_LOGICO + EXPRESION;
            ARITMETICA.Rule = EXPRESION + OP_ARITMETICO + EXPRESION;



            TERMINO.Rule = ENTERO
                | CADENA
                | DECIMAL
                | CARACTER
                | BOOLEANO
                | NULO
                | ToTerm(Constantes.ABRE_PAR) + EXPRESION + ToTerm(Constantes.CIERRA_PAR)
                | NEGATIVO
                | NOT
                | FECHA
                | FECHA_HORA
                | HORA
                | FUN_NATICA;

            FECHA.Rule = fecha;
            HORA.Rule = hora;
            FECHA_HORA.Rule = fechaHora;

            ENTERO.Rule = entero;

            CADENA.Rule = cadena;

            DECIMAL.Rule = doble;

            CARACTER.Rule = caracter;

            BOOLEANO.Rule = ToTerm(Constantes.VERDADERO)
                | ToTerm(Constantes.FALSO);

            NULO.Rule = ToTerm(Constantes.NULO);
            NEGATIVO.Rule = ToTerm(Constantes.MENOS) + EXPRESION;
            NOT.Rule = ToTerm(Constantes.NOT) + EXPRESION;
            
            LLAMADA.Rule = identificador + PARAMETROS_LLAMADA;
            ID.Rule = identificador;
            POS_ARREGLO.Rule = identificador + L_CORCHETES_EXPRESION;

            ELEMENTO_ACCESO.Rule = LLAMADA
                | ID
                | POS_ARREGLO;

            ACCESO.Rule = MakePlusRule(ACCESO, ToTerm(Constantes.PUNTO), ELEMENTO_ACCESO);
            UNARIO.Rule = identificador + ToTerm(Constantes.MAS_MAS)
                    | identificador + ToTerm(Constantes.MENOS_MENOS);


            /*FUNCIONES NATIVAS*/

            FUN_NATICA.Rule = FUN_CADENA
                | FUN_SUB_CAD
                | FUN_POS
                | FUN_BOOL
                | FUN_ENTERO
                | FUN_TAM
                | FUN_RANDOM
                | FUN_MIN
                | FUN_MAX
                | FUN_POW
                | FUN_LOG
                | FUN_LOG10
                | FUN_SIN
                | FUN_COS
                | FUN_TAN
                | FUN_SQRT
                | FUN_PI
                | FUN_HOY
                | FUN_AHORA
                | FUN_FECHA
                | FUN_HORA
                | FUN_FECHA_HORA
                | FUN_ABS;

            FUN_MENSAJE.Rule = ToTerm(Constantes.MENSAJE) + ToTerm(Constantes.ABRE_PAR) + EXPRESION + ToTerm(Constantes.CIERRA_PAR) + ToTerm(Constantes.PUNTO_COMA);

            FUN_CADENA.Rule = ToTerm(Constantes.CADENA) + ToTerm(Constantes.ABRE_PAR) + EXPRESION + ToTerm(Constantes.CIERRA_PAR);

            FUN_SUB_CAD.Rule = ToTerm(Constantes.SUBCAD) + ToTerm(Constantes.ABRE_PAR) + EXPRESION + ToTerm(Constantes.COMA) + EXPRESION + ToTerm(Constantes.COMA) + EXPRESION + ToTerm(Constantes.CIERRA_PAR);

            FUN_POS.Rule = ToTerm(Constantes.POSCAD) + ToTerm(Constantes.ABRE_PAR) + EXPRESION + ToTerm(Constantes.COMA) + EXPRESION + ToTerm(Constantes.CIERRA_PAR);

            FUN_BOOL.Rule = ToTerm(Constantes.BOOLEANO) + ToTerm(Constantes.ABRE_PAR) + EXPRESION + ToTerm(Constantes.CIERRA_PAR);


            FUN_ENTERO.Rule = ToTerm(Constantes.ENTERO) + ToTerm(Constantes.ABRE_PAR) + EXPRESION + ToTerm(Constantes.CIERRA_PAR);

            FUN_TAM.Rule = ToTerm(Constantes.TAM) + ToTerm(Constantes.ABRE_PAR) + EXPRESION + ToTerm(Constantes.CIERRA_PAR);

            FUN_RANDOM.Rule = ToTerm(Constantes.RANDOM) + PARAMETROS_LLAMADA;

            FUN_MIN.Rule = ToTerm(Constantes.MIN) + PARAMETROS_LLAMADA;

            FUN_MAX.Rule = ToTerm(Constantes.MAX) + PARAMETROS_LLAMADA;

            FUN_POW.Rule = ToTerm(Constantes.POW) + ToTerm(Constantes.ABRE_PAR) + EXPRESION+ ToTerm(",")+ EXPRESION + ToTerm(Constantes.CIERRA_PAR);

            FUN_LOG.Rule = ToTerm(Constantes.LOG) + ToTerm("(") + EXPRESION + ToTerm(")");

            FUN_LOG10.Rule = ToTerm(Constantes.LOG10) + ToTerm("(") + EXPRESION + ToTerm(")");

            FUN_ABS.Rule = ToTerm(Constantes.ABS) + ToTerm("(") + EXPRESION + ToTerm(")");

            FUN_SIN.Rule = ToTerm(Constantes.SIN) + ToTerm("(") + EXPRESION + ToTerm(")");

            FUN_COS.Rule = ToTerm(Constantes.COS) + ToTerm("(") + EXPRESION + ToTerm(")");

            FUN_TAN.Rule = ToTerm(Constantes.TAN) + ToTerm("(") + EXPRESION + ToTerm(")");

            FUN_SQRT.Rule = ToTerm(Constantes.SQRT) + ToTerm("(") + EXPRESION + ToTerm(")");

            FUN_PI.Rule = ToTerm(Constantes.PI) + ToTerm("(") + ToTerm(")");

            FUN_HOY.Rule = ToTerm(Constantes.HOY) + ToTerm("(") + ToTerm(")");

            FUN_AHORA.Rule = ToTerm(Constantes.AHORA) + ToTerm("(") + ToTerm(")");

            FUN_FECHA.Rule = ToTerm(Constantes.FECHA) + ToTerm("(") + EXPRESION + ToTerm(")");
            FUN_HORA.Rule = ToTerm(Constantes.HORA) + ToTerm("(") + EXPRESION + ToTerm(")");
            FUN_FECHA_HORA.Rule = ToTerm(Constantes.FECHA_HORA) + ToTerm("(") + EXPRESION + ToTerm(")");


            #endregion


            this.RegisterOperators(9, Associativity.Left, "(",")");
            this.RegisterOperators(8, Associativity.Right, "++","--","!");
            this.RegisterOperators(6, Associativity.Right, "^");
            this.RegisterOperators(7, Associativity.Left, "/", "*","%");
            this.RegisterOperators(6, Associativity.Left, "-", "+");
            this.RegisterOperators(5, "<", ">", "<=", ">=", "==", "!=");
           // this.RegisterOperators(4, Associativity.Left, "==", "!=");
            this.RegisterOperators(3, Associativity.Left, "&&");
            this.RegisterOperators(2, Associativity.Left, "||");



            this.Root = ARCHIVO;



            #endregion

            MarkPunctuation("[","]","(",")",",","{","}", Constantes.IMPRIMIR, ";",":", Constantes.SI, Constantes.PADRE, Constantes.CLASE, Constantes.SINO, Constantes.IGUAL,
                Constantes.IMPORTAR, ".", "xform", Constantes.PRINCIPAL, Constantes.NUEVO, Constantes.MIENTRAS, Constantes.HACER, Constantes.PARA, Constantes.RETORNO, Constantes.CASO, Constantes.DE,
                Constantes.DOS_PUNTOS, Constantes.DEFECTO);



            MarkTransient(FUN_NATICA,TERMINO, OP_ARITMETICO,OP_LOGICO,OP_RELACIONAL, ELEMENTO_ACCESO, EXPRESION, SENTENCIA, ELEMENTO_CLASE, PARAMETROS, OPCION_VALOR);


        }




    }
}
