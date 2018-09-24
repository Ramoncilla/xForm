﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;


namespace xForms.Formularios
{
    class Formulario
    {

        public string nombre;
        public ParseTreeNode parametros;
        public ParseTreeNode cuerpo;

        public Formulario(string nombre, ParseTreeNode parametros, ParseTreeNode cuerpo)
        {
            this.nombre = nombre;
            this.parametros = parametros;
            this.cuerpo = cuerpo;
        }



    }
}
