using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xForms
{
    public partial class guardarForm : UserControl
    {
        public guardarForm()
        {
            InitializeComponent();
        }

        public String obtenerNombreForm()
        {
            return textBox1.Text;
        }


        private void guardarForm_Load(object sender, EventArgs e)
        {

        }
    }
}
