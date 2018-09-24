namespace xForms
{
    partial class modeloPregunta
    {
        /// <summary> 
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar 
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.gbIdPregunta = new System.Windows.Forms.GroupBox();
            this.gbHora = new System.Windows.Forms.GroupBox();
            this.hora = new System.Windows.Forms.DateTimePicker();
            this.gbFecha = new System.Windows.Forms.GroupBox();
            this.fecha = new System.Windows.Forms.DateTimePicker();
            this.gbCondicion = new System.Windows.Forms.GroupBox();
            this.opFalso = new System.Windows.Forms.RadioButton();
            this.opVerdadero = new System.Windows.Forms.RadioButton();
            this.gbDecimales = new System.Windows.Forms.GroupBox();
            this.respuestaDecimal = new System.Windows.Forms.NumericUpDown();
            this.gbNota = new System.Windows.Forms.GroupBox();
            this.lblNota = new System.Windows.Forms.Label();
            this.gbCheques = new System.Windows.Forms.GroupBox();
            this.listaCheques = new System.Windows.Forms.CheckedListBox();
            this.gbNumeros = new System.Windows.Forms.GroupBox();
            this.respuestaNumerica = new System.Windows.Forms.NumericUpDown();
            this.gbComboBox = new System.Windows.Forms.GroupBox();
            this.comboOpciones = new System.Windows.Forms.ComboBox();
            this.lblSugerir = new System.Windows.Forms.Label();
            this.lblEtiqueta = new System.Windows.Forms.Label();
            this.txtRespuesta = new System.Windows.Forms.TextBox();
            this.gbCadena = new System.Windows.Forms.GroupBox();
            this.gbIdPregunta.SuspendLayout();
            this.gbHora.SuspendLayout();
            this.gbFecha.SuspendLayout();
            this.gbCondicion.SuspendLayout();
            this.gbDecimales.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.respuestaDecimal)).BeginInit();
            this.gbNota.SuspendLayout();
            this.gbCheques.SuspendLayout();
            this.gbNumeros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.respuestaNumerica)).BeginInit();
            this.gbComboBox.SuspendLayout();
            this.gbCadena.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbIdPregunta
            // 
            this.gbIdPregunta.Controls.Add(this.gbCondicion);
            this.gbIdPregunta.Controls.Add(this.gbComboBox);
            this.gbIdPregunta.Controls.Add(this.gbHora);
            this.gbIdPregunta.Controls.Add(this.gbFecha);
            this.gbIdPregunta.Controls.Add(this.gbCadena);
            this.gbIdPregunta.Controls.Add(this.lblSugerir);
            this.gbIdPregunta.Controls.Add(this.gbDecimales);
            this.gbIdPregunta.Controls.Add(this.gbNota);
            this.gbIdPregunta.Controls.Add(this.gbNumeros);
            this.gbIdPregunta.Controls.Add(this.gbCheques);
            this.gbIdPregunta.Location = new System.Drawing.Point(12, 123);
            this.gbIdPregunta.Name = "gbIdPregunta";
            this.gbIdPregunta.Size = new System.Drawing.Size(1430, 351);
            this.gbIdPregunta.TabIndex = 0;
            this.gbIdPregunta.TabStop = false;
            this.gbIdPregunta.Text = "idPregunta";
            // 
            // gbHora
            // 
            this.gbHora.Controls.Add(this.hora);
            this.gbHora.Location = new System.Drawing.Point(1061, 237);
            this.gbHora.Name = "gbHora";
            this.gbHora.Size = new System.Drawing.Size(330, 100);
            this.gbHora.TabIndex = 12;
            this.gbHora.TabStop = false;
            this.gbHora.Text = "Hora:  ";
            this.gbHora.Visible = false;
            // 
            // hora
            // 
            this.hora.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.hora.Location = new System.Drawing.Point(30, 35);
            this.hora.Name = "hora";
            this.hora.ShowUpDown = true;
            this.hora.Size = new System.Drawing.Size(270, 22);
            this.hora.TabIndex = 0;
            // 
            // gbFecha
            // 
            this.gbFecha.Controls.Add(this.fecha);
            this.gbFecha.Location = new System.Drawing.Point(762, 237);
            this.gbFecha.Name = "gbFecha";
            this.gbFecha.Size = new System.Drawing.Size(269, 76);
            this.gbFecha.TabIndex = 11;
            this.gbFecha.TabStop = false;
            this.gbFecha.Text = "Fecha:  ";
            this.gbFecha.Visible = false;
            // 
            // fecha
            // 
            this.fecha.Location = new System.Drawing.Point(39, 30);
            this.fecha.Name = "fecha";
            this.fecha.Size = new System.Drawing.Size(200, 22);
            this.fecha.TabIndex = 0;
            // 
            // gbCondicion
            // 
            this.gbCondicion.Controls.Add(this.opFalso);
            this.gbCondicion.Controls.Add(this.opVerdadero);
            this.gbCondicion.Location = new System.Drawing.Point(521, 157);
            this.gbCondicion.Name = "gbCondicion";
            this.gbCondicion.Size = new System.Drawing.Size(224, 150);
            this.gbCondicion.TabIndex = 10;
            this.gbCondicion.TabStop = false;
            this.gbCondicion.Text = "Condicion:  ";
            this.gbCondicion.Visible = false;
            // 
            // opFalso
            // 
            this.opFalso.AutoSize = true;
            this.opFalso.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opFalso.Location = new System.Drawing.Point(24, 93);
            this.opFalso.Name = "opFalso";
            this.opFalso.Size = new System.Drawing.Size(177, 33);
            this.opFalso.TabIndex = 1;
            this.opFalso.TabStop = true;
            this.opFalso.Text = "radioButton2";
            this.opFalso.UseVisualStyleBackColor = true;
            // 
            // opVerdadero
            // 
            this.opVerdadero.AutoSize = true;
            this.opVerdadero.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opVerdadero.Location = new System.Drawing.Point(24, 30);
            this.opVerdadero.Name = "opVerdadero";
            this.opVerdadero.Size = new System.Drawing.Size(177, 33);
            this.opVerdadero.TabIndex = 0;
            this.opVerdadero.TabStop = true;
            this.opVerdadero.Text = "radioButton1";
            this.opVerdadero.UseVisualStyleBackColor = true;
            // 
            // gbDecimales
            // 
            this.gbDecimales.Controls.Add(this.respuestaDecimal);
            this.gbDecimales.Location = new System.Drawing.Point(1061, 164);
            this.gbDecimales.Name = "gbDecimales";
            this.gbDecimales.Size = new System.Drawing.Size(146, 67);
            this.gbDecimales.TabIndex = 9;
            this.gbDecimales.TabStop = false;
            this.gbDecimales.Text = "Decimal:  ";
            this.gbDecimales.Visible = false;
            // 
            // respuestaDecimal
            // 
            this.respuestaDecimal.DecimalPlaces = 2;
            this.respuestaDecimal.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.respuestaDecimal.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.respuestaDecimal.Location = new System.Drawing.Point(15, 21);
            this.respuestaDecimal.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.respuestaDecimal.Name = "respuestaDecimal";
            this.respuestaDecimal.Size = new System.Drawing.Size(120, 36);
            this.respuestaDecimal.TabIndex = 8;
            // 
            // gbNota
            // 
            this.gbNota.Controls.Add(this.lblNota);
            this.gbNota.Location = new System.Drawing.Point(58, 38);
            this.gbNota.Name = "gbNota";
            this.gbNota.Size = new System.Drawing.Size(681, 102);
            this.gbNota.TabIndex = 6;
            this.gbNota.TabStop = false;
            this.gbNota.Text = "Nota:   ";
            this.gbNota.Visible = false;
            // 
            // lblNota
            // 
            this.lblNota.AutoSize = true;
            this.lblNota.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNota.Location = new System.Drawing.Point(31, 48);
            this.lblNota.Name = "lblNota";
            this.lblNota.Size = new System.Drawing.Size(184, 29);
            this.lblNota.TabIndex = 0;
            this.lblNota.Text = "Contenido nota";
            // 
            // gbCheques
            // 
            this.gbCheques.Controls.Add(this.listaCheques);
            this.gbCheques.Location = new System.Drawing.Point(751, 38);
            this.gbCheques.Name = "gbCheques";
            this.gbCheques.Size = new System.Drawing.Size(262, 193);
            this.gbCheques.TabIndex = 5;
            this.gbCheques.TabStop = false;
            this.gbCheques.Text = "Seleccion Multiple:  ";
            this.gbCheques.Visible = false;
            // 
            // listaCheques
            // 
            this.listaCheques.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listaCheques.FormattingEnabled = true;
            this.listaCheques.Location = new System.Drawing.Point(27, 22);
            this.listaCheques.Name = "listaCheques";
            this.listaCheques.Size = new System.Drawing.Size(212, 154);
            this.listaCheques.TabIndex = 0;
            // 
            // gbNumeros
            // 
            this.gbNumeros.Controls.Add(this.respuestaNumerica);
            this.gbNumeros.Location = new System.Drawing.Point(1232, 156);
            this.gbNumeros.Name = "gbNumeros";
            this.gbNumeros.Size = new System.Drawing.Size(159, 75);
            this.gbNumeros.TabIndex = 4;
            this.gbNumeros.TabStop = false;
            this.gbNumeros.Text = "Entero:  ";
            this.gbNumeros.Visible = false;
            // 
            // respuestaNumerica
            // 
            this.respuestaNumerica.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.respuestaNumerica.Location = new System.Drawing.Point(25, 21);
            this.respuestaNumerica.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.respuestaNumerica.Name = "respuestaNumerica";
            this.respuestaNumerica.Size = new System.Drawing.Size(120, 36);
            this.respuestaNumerica.TabIndex = 5;
            this.respuestaNumerica.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // gbComboBox
            // 
            this.gbComboBox.Controls.Add(this.comboOpciones);
            this.gbComboBox.Location = new System.Drawing.Point(1081, 38);
            this.gbComboBox.Name = "gbComboBox";
            this.gbComboBox.Size = new System.Drawing.Size(310, 110);
            this.gbComboBox.TabIndex = 7;
            this.gbComboBox.TabStop = false;
            this.gbComboBox.Text = "Seleccion:   ";
            this.gbComboBox.Visible = false;
            // 
            // comboOpciones
            // 
            this.comboOpciones.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboOpciones.FormattingEnabled = true;
            this.comboOpciones.Location = new System.Drawing.Point(28, 31);
            this.comboOpciones.Name = "comboOpciones";
            this.comboOpciones.Size = new System.Drawing.Size(246, 33);
            this.comboOpciones.TabIndex = 0;
            this.comboOpciones.SelectedIndexChanged += new System.EventHandler(this.comboOpciones_SelectedIndexChanged);
            // 
            // lblSugerir
            // 
            this.lblSugerir.AutoSize = true;
            this.lblSugerir.Font = new System.Drawing.Font("MV Boli", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSugerir.Location = new System.Drawing.Point(53, 281);
            this.lblSugerir.Name = "lblSugerir";
            this.lblSugerir.Size = new System.Drawing.Size(147, 26);
            this.lblSugerir.TabIndex = 1;
            this.lblSugerir.Text = "Cadena sugerir";
            this.lblSugerir.Visible = false;
            // 
            // lblEtiqueta
            // 
            this.lblEtiqueta.AutoSize = true;
            this.lblEtiqueta.Font = new System.Drawing.Font("Lucida Fax", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEtiqueta.Location = new System.Drawing.Point(45, 50);
            this.lblEtiqueta.Name = "lblEtiqueta";
            this.lblEtiqueta.Size = new System.Drawing.Size(300, 38);
            this.lblEtiqueta.TabIndex = 0;
            this.lblEtiqueta.Text = "Cadena Pregunta";
            this.lblEtiqueta.Visible = false;
            // 
            // txtRespuesta
            // 
            this.txtRespuesta.Location = new System.Drawing.Point(37, 31);
            this.txtRespuesta.Name = "txtRespuesta";
            this.txtRespuesta.Size = new System.Drawing.Size(368, 22);
            this.txtRespuesta.TabIndex = 4;
            // 
            // gbCadena
            // 
            this.gbCadena.Controls.Add(this.txtRespuesta);
            this.gbCadena.Location = new System.Drawing.Point(58, 146);
            this.gbCadena.Name = "gbCadena";
            this.gbCadena.Size = new System.Drawing.Size(428, 97);
            this.gbCadena.TabIndex = 3;
            this.gbCadena.TabStop = false;
            this.gbCadena.Text = "Cadena:   ";
            this.gbCadena.Visible = false;
            // 
            // modeloPregunta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblEtiqueta);
            this.Controls.Add(this.gbIdPregunta);
            this.Name = "modeloPregunta";
            this.Size = new System.Drawing.Size(1471, 492);
            this.Load += new System.EventHandler(this.modeloPregunta_Load);
            this.gbIdPregunta.ResumeLayout(false);
            this.gbIdPregunta.PerformLayout();
            this.gbHora.ResumeLayout(false);
            this.gbFecha.ResumeLayout(false);
            this.gbCondicion.ResumeLayout(false);
            this.gbCondicion.PerformLayout();
            this.gbDecimales.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.respuestaDecimal)).EndInit();
            this.gbNota.ResumeLayout(false);
            this.gbNota.PerformLayout();
            this.gbCheques.ResumeLayout(false);
            this.gbNumeros.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.respuestaNumerica)).EndInit();
            this.gbComboBox.ResumeLayout(false);
            this.gbCadena.ResumeLayout(false);
            this.gbCadena.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbIdPregunta;
        private System.Windows.Forms.TextBox txtRespuesta;
        private System.Windows.Forms.NumericUpDown respuestaNumerica;
        private System.Windows.Forms.Label lblEtiqueta;
        private System.Windows.Forms.Label lblSugerir;
        private System.Windows.Forms.GroupBox gbCadena;
        private System.Windows.Forms.GroupBox gbNumeros;
        private System.Windows.Forms.GroupBox gbCheques;
        private System.Windows.Forms.CheckedListBox listaCheques;
        private System.Windows.Forms.GroupBox gbNota;
        private System.Windows.Forms.Label lblNota;
        private System.Windows.Forms.GroupBox gbComboBox;
        private System.Windows.Forms.ComboBox comboOpciones;
        private System.Windows.Forms.GroupBox gbDecimales;
        private System.Windows.Forms.NumericUpDown respuestaDecimal;
        private System.Windows.Forms.GroupBox gbCondicion;
        private System.Windows.Forms.RadioButton opFalso;
        private System.Windows.Forms.RadioButton opVerdadero;
        private System.Windows.Forms.GroupBox gbHora;
        private System.Windows.Forms.DateTimePicker hora;
        private System.Windows.Forms.GroupBox gbFecha;
        private System.Windows.Forms.DateTimePicker fecha;
    }
}
