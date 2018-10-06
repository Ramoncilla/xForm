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
    public partial class FormResultados : Form
    {
        public static String  nombreF = "";
        public FormResultados()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String nombreForm = guardarForm1.obtenerNombreForm();
            if (nombreForm.Equals(""))
            {
                MessageBox.Show("Debe de ingresar un nombre para el formulario");
            }
            else
            {
                nombreF = nombreForm;
                this.DialogResult = DialogResult.OK;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
