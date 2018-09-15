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
using xForms.Interfaz;


namespace xForms
{
    public partial class Form1 : Form
    {

        ListaPaginas paginas;
        int contador;
        public Form1()
        {
            
            InitializeComponent();
            paginas = new ListaPaginas();
            contador = 0;
        }


        private void nuevaPestanha()
        {
            elementoPlantilla nuevaPagina = new elementoPlantilla();
            paginas.insertarPagina(nuevaPagina);
            TabPage nuevoTab= new TabPage("Nueva "+ contador);
            nuevoTab.Controls.Add(nuevaPagina.cajaTexto);
            tabControl1.TabPages.Add(nuevoTab);
            tabControl1.SelectedTab = nuevoTab;
            contador++;

        }

        private string[] cargarArchivo()
        {
            string fileName = null;
            string rutaCarpeta = null;

            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    fileName = openFileDialog1.FileName;
                    rutaCarpeta = Path.GetDirectoryName(openFileDialog1.FileName);  
                }
            }

            if (fileName != null)
            {
                string text = File.ReadAllText(fileName);
                string[] valores = new string[3];
                valores[0] = rutaCarpeta;
                valores[1] = fileName;
                valores[2] = text;
                return valores;

            }
            else
            {
                MessageBox.Show("Error, no se ha podido abrir el archivo");
            }
            return null;
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
         //   imprimir();
         /*
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

           txtImpresion.Text = n.parse(contenido);*/
           
            
        }

        private void button3_Click(object sender, EventArgs e)
        {

            Console.WriteLine(Math.Sqrt(-10));
            
            string cadena = "hola Mundo";
            char c;
            int val=0;
            for (int i = 0; i < cadena.Count(); i++)
            {
                c = Convert.ToChar(cadena.ElementAt(i));
                Console.WriteLine(c);
                val = Convert.ToInt32(c);
                Console.WriteLine(val);
            }
        }

        private void nuevaPestanhaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nuevaPestanha();
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string [] valores = cargarArchivo();
            if (valores != null)
            {
                int no = tabControl1.SelectedIndex;
                tabControl1.SelectedTab.Text = valores[1];
                string ruta = valores[0];
                Console.WriteLine(ruta);
                string nombre = valores[1];
                string texto = valores[2];
                paginas.escribirTexto(no, texto, nombre, ruta);
            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
           int no = tabControl1.SelectedIndex;
           string impresion= paginas.ejecutar(no);
           txtImpresion.Text = impresion;
           Constantes.erroresEjecucion.moostrarErrores();
           if (Constantes.erroresEjecucion.lErrores.Count > 0)
           {
               MessageBox.Show("Han ocurrido errores al analizar el archivo, favor de revisar");
           }

        }
    }
}
