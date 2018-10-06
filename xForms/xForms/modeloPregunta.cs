using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xForms.Fechas;
using xForms.Ejecucion;

namespace xForms
{
    public partial class modeloPregunta : UserControl
    {
        public modeloPregunta()
        {
            InitializeComponent();
        }

        /* Set Valores */
        public bool banderaContinuar = true;
        string sugerir="";
        bool lectura=false;
        string idPregunta;
       public int tipoPregunta;

        int cad_max=-1;
        int cad_min=-1;
        int cad_fila=-1;

        string cadT;
        string cadF;

        int Linf;
        int Lsup;
        Object predeterminado=null;
        string contenidoNota;
        string cadenaPregunta = "";
        public bool requerido = false;
        public string msgRequerido = "";
        List<Opcion> listaValores;

        // cadena
        public modeloPregunta(string sugerir, bool lectura, string idP, int max, int min, int fil, string val, string CadenaPregunta)
        {
            this.tipoPregunta = 1;
            this.cad_max = max;
            this.cad_min = min;
            this.cad_fila = fil;
            this.sugerir = sugerir;
            this.lectura = lectura;
            this.idPregunta = idP;
            this.predeterminado = val;
            this.cadenaPregunta = CadenaPregunta;
            InitializeComponent();
        }

        //entero
        public modeloPregunta(string sugerir, bool lectura, string idP, int val, string cadenaPreg)
        {
            this.tipoPregunta = 2;
            this.sugerir = sugerir;
            this.lectura = lectura;
            this.idPregunta = idP;
            this.predeterminado = val;
            this.cadenaPregunta = cadenaPreg;
            InitializeComponent();
        }


        //decimal
        public modeloPregunta(string sugerir, bool lectura, string idP, double val, string cadenaPr)
        {
            this.tipoPregunta = 3;
            this.sugerir = sugerir;
            this.lectura = lectura;
            this.idPregunta = idP;
            this.predeterminado = val;
            this.cadenaPregunta = cadenaPr;
            InitializeComponent();
        }

        //bool
        public modeloPregunta(string sugerir, bool lectura, string idP, string cadt, string cadf, Object val, string cad)
        {
            this.tipoPregunta = 4;
            this.sugerir = sugerir;
            this.lectura = lectura;
            this.idPregunta = idP;
            this.cadF = cadf;
            this.cadT = cadt;
            this.predeterminado = val;
            this.cadenaPregunta = cad;
            InitializeComponent();

        }

        // nota
        public modeloPregunta(string sugerir, bool lectura, string nombrePregunta, string contendioNota)
        {
            this.sugerir = sugerir;
            this.lectura = lectura;
            this.idPregunta = nombrePregunta;
            this.tipoPregunta =5;
            this.contenidoNota = contendioNota;
            InitializeComponent();
        }

        //rango
        public modeloPregunta(string sugerir, bool lectura, string nombreP, int Linf, int sup, int val, string cadE)
        {
            this.tipoPregunta = 6;
            this.sugerir = sugerir;
            this.lectura = lectura;
            this.idPregunta = nombreP;
            this.Linf = Linf;
            this.Lsup = sup;
            this.predeterminado = val;
            this.cadenaPregunta = cadE;
            InitializeComponent();
        }

        //7 fecha 8 hora 9 fechahora
        public modeloPregunta(string sugerir, bool lectura, string nombreP, Object val,  int tipo, string cadPreg)
        {
            this.tipoPregunta = tipo;
            this.sugerir = sugerir;
            this.lectura = lectura;
            this.idPregunta = nombreP;
            this.predeterminado = val;
            this.cadenaPregunta = cadPreg;
            InitializeComponent();

        }

        //10 uno, 11 multiples
        public modeloPregunta(string sugerir, bool lectura, string nombreP, List<Opcion> listaValores, string val, int tipo, string cadC)
        {
            this.tipoPregunta = tipo;
            this.sugerir = sugerir;
            this.lectura = lectura;
            this.idPregunta = nombreP;
            this.predeterminado = val;
            this.cadenaPregunta = cadC;
            this.listaValores = listaValores;
            InitializeComponent();
        }

        //12 fichero
        public modeloPregunta(string sugerir, bool lectura, string nombreP, Object valor)
        {
            this.tipoPregunta = 12;
            this.sugerir = sugerir;
            this.lectura = lectura;
            this.idPregunta = nombreP;
            this.predeterminado = valor;
            InitializeComponent();
        }


        private void iniciar()
        {
            lblSugerir.Text = sugerir;
            lblEtiqueta.Text = cadenaPregunta;
            gbIdPregunta.Text = idPregunta + ":  ";
            lblEtiqueta.Visible = true;
            lblSugerir.Visible = true;
             gbIdPregunta.Text = this.idPregunta + ":  ";
            switch (tipoPregunta)
            {
                case 1:
                    {
                        #region validaciones Cadena
                        txtRespuesta.Visible = true;
                       // gbCadena.Visible = true;
                        if(lectura)
                        { txtRespuesta.Enabled = false; }
                        
                        if (cad_max != -1)
                        {
                            txtRespuesta.MaxLength = cad_max;
                        }
                        if (cad_fila > 0)
                        {
                            txtRespuesta.Multiline = true;
                        }
                        if (this.predeterminado != null)
                        {
                            txtRespuesta.Text = predeterminado.ToString();
                        }
                        break;
                        #endregion
                    }

                case 2:
                    {
                        respuestaNumerica.Visible = true;
                        //gbNumeros.Visible = true;
                        if (predeterminado != null)
                        {
                            try
                            {
                                int p = int.Parse(predeterminado.ToString());
                                respuestaNumerica.Value = p;
                            }catch(Exception e){

                            }
                            
                        }
                        if (lectura)
                        {
                            respuestaNumerica.Enabled = false;
                        }

                        break;
                    }

                case 3:
                    {
                        respuestaDecimal.Visible = true;

                        //gbDecimales.Visible = true;
                        if (predeterminado != null)
                        {
                            try
                            {
                                Decimal d = decimal.Parse(predeterminado.ToString());
                                respuestaDecimal.Value = d;
                            }
                            catch (Exception e)
                            {

                            }
                        }
                        if (lectura)
                        {
                            respuestaDecimal.Enabled = false;
                        }

                        break;
                    }

                case 4:
                    {
                        gbCondicion.Visible = true;
                        if (lectura)
                        {
                            opVerdadero.Enabled = false;
                            opFalso.Enabled = false;
                        }
                        if (predeterminado != null)
                        {
                           string g = predeterminado.ToString();
                           if(g.Equals(Constantes.VERDADERO,StringComparison.CurrentCultureIgnoreCase)){
                               opVerdadero.Checked= true;
                           }else{
                               opFalso.Checked= true;
                           }
                        }else{
                            opVerdadero.Checked= false;
                            opFalso.Checked= false;
                        }
                        break;
                    }

                case 5:
                    {

                        lblNota.Visible = true;
                        //gbNota.Visible = true;
                        lblNota.Text = contenidoNota;
                        break;
                    }
                case 6:
                    {
                       // gbNumeros.Visible = true;
                        respuestaNumerica.Visible = true;
                        respuestaNumerica.Minimum = Linf;
                        respuestaNumerica.Maximum = Lsup;
                        if (predeterminado != null)
                        {
                            try
                            {
                                int p = int.Parse(predeterminado.ToString());
                                if (Linf <= p && Lsup >= p)
                                {
                                    respuestaNumerica.Value = p;
                                }
                                else
                                {
                                    respuestaNumerica.Value = Linf;
                                }
                                
                            }
                            catch (Exception e)
                            {

                            }

                        }
                        if (lectura)
                        {
                            respuestaNumerica.Enabled = false;
                        }

                        break;
                    }

                case 7://fecha
                    {
                        fecha.Visible = true;
                       // gbFecha.Visible = true;
                        if (predeterminado != null)
                        {
                            try
                            {
                                Fecha f = (Fecha)predeterminado;
                                int anio = f.fechaReal.Year;
                                int mes = f.fechaReal.Month;
                                int dia = f.fechaReal.Day;
                                fecha.Value = new DateTime(anio, mes, dia);
                            }catch(Exception e){

                            }
                            if (lectura)
                            {
                                fecha.Enabled = false;
                            }
                        }
                        break;
                    }

                case 8://hora
                    {
                        hora.Visible = true;
                       // gbHora.Visible = true;
                        if (predeterminado != null)
                        {
                            try
                            {
                                Hora f = (Hora)predeterminado;
                                int anio = f.horaReal.Year;
                                int mes = f.horaReal.Month;
                                int dia = f.horaReal.Day;
                                int horas = f.horaReal.Hour;
                                int min = f.horaReal.Minute;
                                int seg = f.horaReal.Second;
                                hora.Value = new DateTime(anio, mes, dia, horas, min, seg);
                            }
                            catch (Exception e)
                            {

                            }
                            if (lectura)
                            {
                                hora.Enabled = false;
                            }
                        }
                        break;
                    }

                case 9://fechahora
                    {
                        hora.Visible = true;
                        fecha.Visible = true;
                        if (predeterminado != null)
                        {
                            try
                            {
                                Fecha f1 = (Fecha)predeterminado;
                                int anio1 = f1.fechaReal.Year;
                                int mes1 = f1.fechaReal.Month;
                                int dia1 = f1.fechaReal.Day;
                                fecha.Value = new DateTime(anio1, mes1, dia1);
                                Hora f = (Hora)predeterminado;
                                int anio = f.horaReal.Year;
                                int mes = f.horaReal.Month;
                                int dia = f.horaReal.Day;
                                int horas = f.horaReal.Hour;
                                int min = f.horaReal.Minute;
                                int seg = f.horaReal.Second;
                                hora.Value = new DateTime(anio, mes, dia, horas, min, seg);
                            }
                            catch (Exception e)
                            {

                            }
                            if (lectura)
                            {
                                hora.Enabled = false;
                                fecha.Enabled = false;
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        comboOpciones.Visible = true;
                       // gbComboBox.Visible = true;
                        if (listaValores != null)
                        {
                            BindingSource bSource = new BindingSource();
                            bSource.DataSource = listaValores;
                            comboOpciones.DataSource = bSource.DataSource;
                            comboOpciones.DisplayMember = "etiqueta";
                            comboOpciones.ValueMember = "nombre";
                        }
                        if (lectura)
                        {
                            comboOpciones.Enabled = false;
                        }

                        break;
                    }

                case 11:
                    {
                        listaCheques.Visible = true;
                        //gbCheques.Visible = true;
                        if (listaValores != null)
                        {
                            ((ListBox)listaCheques).DataSource = listaValores;
                            ((ListBox)listaCheques).DisplayMember = "etiqueta";
                            ((ListBox)listaCheques).ValueMember = "nombre";
                        }

                        if (lectura)
                        {
                            listaCheques.Enabled = false;
                        }
                       


                        


                        break;
                    }

            }
        }



       

        private void modeloPregunta_Load(object sender, EventArgs e)
        {
            iniciar();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }


        public string obtenerRepuestaCondicion()
        {
            if (opVerdadero.Checked)
            {
                return Constantes.VERDADERO;
            }
            else if (opFalso.Checked)
            {
                return Constantes.FALSO;
            }
            return "";
        }


        public string obtenerRespuestaCadena()
        {
            return txtRespuesta.Text;
        }

        public int obtenerRespuestaEntero()
        {
            return decimal.ToInt32(respuestaNumerica.Value);
        }


        public double obtenerRespuestaDecimal()
        {
            return decimal.ToDouble(respuestaDecimal.Value);
        }

        public string obtenerRespuestaFecha()
        {
            string theDate = fecha.Value.ToString("dd/MM/yyyy");
            return theDate;
        }


        public string obtenerRespuestaHora()
        {
            string theTime = hora.Value.ToString("hh:mm:ss");
            return theTime;
        }


        public string obtenerRespuestaFechaHora()
        {
            string theDate = fecha.Value.ToString("dd/MM/yyyy");
            string theTime = hora.Value.ToString("hh:mm:ss");
            string s = "\"" + theDate + " " + theTime + "\"";
            return s;
        }



        public string obtenerSeleccionUno()
        {
            Opcion p = (Opcion)comboOpciones.SelectedItem;
            return p.etiqueta;
        }



        public List<string> obtenerSeleccionadosMuchos()
        {
            List<string> valores = new List<string>();

            for (int i = 0; i < listaCheques.Items.Count; i++)
            {
                if (listaCheques.GetItemChecked(i))
                {
                    Opcion str = (Opcion)listaCheques.Items[i];
                    valores.Add(str.etiqueta);
                }
            }
            return valores;
        }

        private void comboOpciones_SelectedIndexChanged(object sender, EventArgs e)
        {
            Opcion p = (Opcion)comboOpciones.SelectedItem;
            Console.WriteLine(p.nombre);
        }


    }
}
