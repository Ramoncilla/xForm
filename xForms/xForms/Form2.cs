using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xForms
{
    public partial class Form2 : Form
    {
        public static string respuestaCadena = "";
        public static int respuestaEntero = 0;
        public static double respuestaDecimal = 0;
        public static string respuestaCondicion = "";
        public static string respuestaFecha = "";
        public static string respuestaHora = "";
        public static string respuestaFechaHora = "";
        public static string respuestaSeleccionar1 = "";
        public static List<string> respuestaSeleccionarMuchos = new List<string>();
        public  modeloPregunta mod = new modeloPregunta();

        public Form2()
        {
            InitializeComponent();
        }


        public void setValor(modeloPregunta m)
        {
            this.mod = m;
            this.Controls.Add(mod);
            this.Controls.Add(button1);
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            
        }

        private void button1_Click_3(object sender, EventArgs e)
        {
            #region tipoCadena
            if (mod.tipoPregunta == 1)
            {
               respuestaCadena = mod.obtenerRespuestaCadena();
                if(mod.requerido)
                {
                    if (respuestaCadena.Equals(""))
                    {
                        if (mod.msgRequerido.Equals(""))
                        {
                            MessageBox.Show("Debe de responder preguntar para continuar");
                        }
                        else
                        {
                            MessageBox.Show(mod.msgRequerido);
                        }
                    }
                    else
                    {
                        this.DialogResult = DialogResult.OK;
                    }
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                }
            }
            #endregion
            if (mod.tipoPregunta == 2)
            {
                respuestaEntero = mod.obtenerRespuestaEntero();
                this.DialogResult = DialogResult.OK;
            }

            if (mod.tipoPregunta == 3)
            {
                respuestaDecimal = mod.obtenerRespuestaDecimal();
                this.DialogResult = DialogResult.OK;
            }


            #region tipoBooleano
            if (mod.tipoPregunta == 4)
            {
                respuestaCondicion = mod.obtenerRepuestaCondicion();
                if (mod.requerido)
                {
                    if (respuestaCondicion.Equals(""))
                    {
                        if (mod.msgRequerido.Equals(""))
                        {
                            MessageBox.Show("Debe de responder preguntar para continuar");
                        }
                        else
                        {
                            MessageBox.Show(mod.msgRequerido);
                        }
                    }
                    else
                    {
                        this.DialogResult = DialogResult.OK;
                    }
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                }
            }
            #endregion


            if (mod.tipoPregunta == 5)
            {
                this.DialogResult = DialogResult.OK;
            }

            #region tipoFecha
            if (mod.tipoPregunta == 7)
            {
                respuestaFecha = mod.obtenerRespuestaFecha();
                if (mod.requerido)
                {
                    if (respuestaFecha.Equals(""))
                    {
                        if (mod.msgRequerido.Equals(""))
                        {
                            MessageBox.Show("Debe de responder preguntar para continuar");
                        }
                        else
                        {
                            MessageBox.Show(mod.msgRequerido);
                        }
                    }
                    else
                    {
                        this.DialogResult = DialogResult.OK;
                    }
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                }
            }
            #endregion

            #region tipoHora
            if (mod.tipoPregunta == 8)
            {
                respuestaHora = mod.obtenerRespuestaHora();
                if (mod.requerido)
                {
                    if (respuestaHora.Equals(""))
                    {
                        if (mod.msgRequerido.Equals(""))
                        {
                            MessageBox.Show("Debe de responder preguntar para continuar");
                        }
                        else
                        {
                            MessageBox.Show(mod.msgRequerido);
                        }
                    }
                    else
                    {
                        this.DialogResult = DialogResult.OK;
                    }
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                }
            }


            #endregion

            #region tipoFechaHora
            if (mod.tipoPregunta == 9)
            {
                respuestaFechaHora = mod.obtenerRespuestaFechaHora();
                if (mod.requerido)
                {
                    if (respuestaFechaHora.Equals(""))
                    {
                        if (mod.msgRequerido.Equals(""))
                        {
                            MessageBox.Show("Debe de responder preguntar para continuar");
                        }
                        else
                        {
                            MessageBox.Show(mod.msgRequerido);
                        }
                    }
                    else
                    {
                        this.DialogResult = DialogResult.OK;
                    }
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                }
            }


            #endregion

            #region Seleecionar 1
            if (mod.tipoPregunta == 10)
            {
                respuestaSeleccionar1 = mod.obtenerSeleccionUno();
                if (mod.requerido)
                {
                    if (respuestaSeleccionar1.Equals(""))
                    {
                        if (mod.msgRequerido.Equals(""))
                        {
                            MessageBox.Show("Debe de responder preguntar para continuar");
                        }
                        else
                        {
                            MessageBox.Show(mod.msgRequerido);
                        }
                    }
                    else
                    {
                        this.DialogResult = DialogResult.OK;
                    }
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                }
            }
            #endregion


            #region Seleecionar muchos
            if (mod.tipoPregunta == 11)
            {
                respuestaSeleccionarMuchos = mod.obtenerSeleccionadosMuchos();
                if (mod.requerido)
                {
                    if (respuestaSeleccionarMuchos.Count==0)
                    {
                        if (mod.msgRequerido.Equals(""))
                        {
                            MessageBox.Show("Debe de responder preguntar para continuar");
                        }
                        else
                        {
                            MessageBox.Show(mod.msgRequerido);
                        }
                    }
                    else
                    {
                        this.DialogResult = DialogResult.OK;
                    }
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                }
            }
            #endregion


            #region rango
            if (mod.tipoPregunta == 6)
            {
                respuestaEntero = mod.obtenerRespuestaEntero();
                if (mod.requerido)
                {
                    if (respuestaEntero.Equals(""))
                    {
                        if (mod.msgRequerido.Equals(""))
                        {
                            MessageBox.Show("Debe de responder preguntar para continuar");
                        }
                        else
                        {
                            MessageBox.Show(mod.msgRequerido);
                        }
                    }
                    else
                    {
                        this.DialogResult = DialogResult.OK;
                    }
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                }
            }
            #endregion


        }


    }
}
