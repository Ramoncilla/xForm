using FastColoredTextBoxNS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xForms.Analizar;
using xForms.Errores;

namespace xForms
{
    public partial class Form1 : Form
    {
       

        public Form1()
        {

            InitializeComponent();
        }


        public String leerArchivo()
        {
            string fichero = "\\pruebasX.xform";
            //Path.Combine(str_uploadpath, "");
            string arrText = "";

            

    try
    {
        using (StreamReader lector = new StreamReader(fichero))
        {
            while (lector.Peek() > -1)
            {
                string linea = lector.ReadLine();
                if (!String.IsNullOrEmpty(linea))
                {
                    arrText += linea;
                }
            }
        }
    }
    catch(Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
            
            return arrText;
        }


        public void abrirArchivo()
        {
            // Create an OpenFileDialog to request a file to open.
            OpenFileDialog openFile1 = new OpenFileDialog();

            // Initialize the OpenFileDialog to look for RTF files.
            //  openFile1.DefaultExt = "*.XForm";
            // openFile1.Filter = "Archivo XForm (XForm)|*.xform";

            if (openFile1.ShowDialog() != DialogResult.OK) return;


            string rutacarpeta = Path.GetDirectoryName(openFile1.FileName);
            string title = Path.GetFileName(openFile1.FileName);
            TabPage myTabPage = new TabPage(title);
            myTabPage.Tag = openFile1.FileName;
            myTabPage.Name = title;
            tabControl1.TabPages.Add(myTabPage);

            FastColoredTextBox newTextBox = new FastColoredTextBox();
            newTextBox = cargarFCTB(openFile1.FileName);
            newTextBox.Dock = DockStyle.Fill;

            myTabPage.Controls.Add(newTextBox);
            tabControl1.SelectedTab = myTabPage;

            
        }

        public FastColoredTextBox cargarFCTB(string path)
        {
            StreamReader reader = null;
            FastColoredTextBox txtSource = new FastColoredTextBox();
            txtSource.Dock = DockStyle.Fill;
            try
            {
                reader = new StreamReader(path);
                txtSource.Text = null;  //to clear any old formatting
                txtSource.ClearUndo();
                txtSource.ClearStylesBuffer();
                txtSource.Text = reader.ReadToEnd();
                txtSource.SetVisibleState(0, FastColoredTextBoxNS.VisibleState.Visible);
                txtSource.Selection = txtSource.GetRange(0, 0);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();

            }

            return txtSource;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            abrirArchivo();
          
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Arbol n = new Arbol();
            string line;
            string contenido = "";
            //StreamReader file =  new StreamReader(@"c:\test.txt");
            StreamReader file = new StreamReader("pruebasX.xform");
            while ((line = file.ReadLine()) != null)
            {
                contenido += line+"\n";
            }

            file.Close();
            //Console.WriteLine(contenido);

           n.parse(contenido);
           Console.WriteLine("hola");

        }
    }
}
