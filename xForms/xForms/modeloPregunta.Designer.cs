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
            this.txtRespuesta = new System.Windows.Forms.TextBox();
            this.lblNota = new System.Windows.Forms.Label();
            this.fecha = new System.Windows.Forms.DateTimePicker();
            this.hora = new System.Windows.Forms.DateTimePicker();
            this.listaCheques = new System.Windows.Forms.CheckedListBox();
            this.respuestaNumerica = new System.Windows.Forms.NumericUpDown();
            this.respuestaDecimal = new System.Windows.Forms.NumericUpDown();
            this.comboOpciones = new System.Windows.Forms.ComboBox();
            this.gbCondicion = new System.Windows.Forms.GroupBox();
            this.opFalso = new System.Windows.Forms.RadioButton();
            this.opVerdadero = new System.Windows.Forms.RadioButton();
            this.lblSugerir = new System.Windows.Forms.Label();
            this.lblEtiqueta = new System.Windows.Forms.Label();
            this.gbIdPregunta.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.respuestaNumerica)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.respuestaDecimal)).BeginInit();
            this.gbCondicion.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbIdPregunta
            // 
            this.gbIdPregunta.Controls.Add(this.txtRespuesta);
            this.gbIdPregunta.Controls.Add(this.lblNota);
            this.gbIdPregunta.Controls.Add(this.fecha);
            this.gbIdPregunta.Controls.Add(this.hora);
            this.gbIdPregunta.Controls.Add(this.listaCheques);
            this.gbIdPregunta.Controls.Add(this.respuestaNumerica);
            this.gbIdPregunta.Controls.Add(this.respuestaDecimal);
            this.gbIdPregunta.Controls.Add(this.comboOpciones);
            this.gbIdPregunta.Controls.Add(this.gbCondicion);
            this.gbIdPregunta.Controls.Add(this.lblSugerir);
            this.gbIdPregunta.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbIdPregunta.Location = new System.Drawing.Point(16, 74);
            this.gbIdPregunta.Name = "gbIdPregunta";
            this.gbIdPregunta.Size = new System.Drawing.Size(853, 297);
            this.gbIdPregunta.TabIndex = 0;
            this.gbIdPregunta.TabStop = false;
            this.gbIdPregunta.Text = "idPregunta";
            // 
            // txtRespuesta
            // 
            this.txtRespuesta.Location = new System.Drawing.Point(431, 61);
            this.txtRespuesta.Name = "txtRespuesta";
            this.txtRespuesta.Size = new System.Drawing.Size(368, 30);
            this.txtRespuesta.TabIndex = 4;
            this.txtRespuesta.Visible = false;
            // 
            // lblNota
            // 
            this.lblNota.AutoSize = true;
            this.lblNota.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNota.Location = new System.Drawing.Point(62, 62);
            this.lblNota.Name = "lblNota";
            this.lblNota.Size = new System.Drawing.Size(184, 29);
            this.lblNota.TabIndex = 0;
            this.lblNota.Text = "Contenido nota";
            this.lblNota.Visible = false;
            // 
            // fecha
            // 
            this.fecha.Location = new System.Drawing.Point(599, 62);
            this.fecha.Name = "fecha";
            this.fecha.Size = new System.Drawing.Size(200, 30);
            this.fecha.TabIndex = 0;
            this.fecha.Visible = false;
            // 
            // hora
            // 
            this.hora.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.hora.Location = new System.Drawing.Point(538, 133);
            this.hora.Name = "hora";
            this.hora.ShowUpDown = true;
            this.hora.Size = new System.Drawing.Size(270, 30);
            this.hora.TabIndex = 0;
            this.hora.Visible = false;
            // 
            // listaCheques
            // 
            this.listaCheques.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listaCheques.FormattingEnabled = true;
            this.listaCheques.Location = new System.Drawing.Point(587, 61);
            this.listaCheques.Name = "listaCheques";
            this.listaCheques.Size = new System.Drawing.Size(212, 154);
            this.listaCheques.TabIndex = 0;
            this.listaCheques.Visible = false;
            // 
            // respuestaNumerica
            // 
            this.respuestaNumerica.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.respuestaNumerica.Location = new System.Drawing.Point(679, 62);
            this.respuestaNumerica.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.respuestaNumerica.Name = "respuestaNumerica";
            this.respuestaNumerica.Size = new System.Drawing.Size(120, 36);
            this.respuestaNumerica.TabIndex = 5;
            this.respuestaNumerica.Visible = false;
            this.respuestaNumerica.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
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
            this.respuestaDecimal.Location = new System.Drawing.Point(679, 61);
            this.respuestaDecimal.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.respuestaDecimal.Name = "respuestaDecimal";
            this.respuestaDecimal.Size = new System.Drawing.Size(120, 36);
            this.respuestaDecimal.TabIndex = 8;
            this.respuestaDecimal.Visible = false;
            // 
            // comboOpciones
            // 
            this.comboOpciones.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboOpciones.FormattingEnabled = true;
            this.comboOpciones.Location = new System.Drawing.Point(484, 65);
            this.comboOpciones.Name = "comboOpciones";
            this.comboOpciones.Size = new System.Drawing.Size(315, 33);
            this.comboOpciones.TabIndex = 0;
            this.comboOpciones.Visible = false;
            this.comboOpciones.SelectedIndexChanged += new System.EventHandler(this.comboOpciones_SelectedIndexChanged);
            // 
            // gbCondicion
            // 
            this.gbCondicion.Controls.Add(this.opFalso);
            this.gbCondicion.Controls.Add(this.opVerdadero);
            this.gbCondicion.Location = new System.Drawing.Point(584, 61);
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
            // lblSugerir
            // 
            this.lblSugerir.AutoSize = true;
            this.lblSugerir.Font = new System.Drawing.Font("MV Boli", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSugerir.Location = new System.Drawing.Point(37, 147);
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
            this.lblEtiqueta.Location = new System.Drawing.Point(51, 23);
            this.lblEtiqueta.Name = "lblEtiqueta";
            this.lblEtiqueta.Size = new System.Drawing.Size(300, 38);
            this.lblEtiqueta.TabIndex = 0;
            this.lblEtiqueta.Text = "Cadena Pregunta";
            this.lblEtiqueta.Visible = false;
            // 
            // modeloPregunta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblEtiqueta);
            this.Controls.Add(this.gbIdPregunta);
            this.Name = "modeloPregunta";
            this.Size = new System.Drawing.Size(908, 401);
            this.Load += new System.EventHandler(this.modeloPregunta_Load);
            this.gbIdPregunta.ResumeLayout(false);
            this.gbIdPregunta.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.respuestaNumerica)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.respuestaDecimal)).EndInit();
            this.gbCondicion.ResumeLayout(false);
            this.gbCondicion.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbIdPregunta;
        private System.Windows.Forms.TextBox txtRespuesta;
        private System.Windows.Forms.NumericUpDown respuestaNumerica;
        private System.Windows.Forms.Label lblEtiqueta;
        private System.Windows.Forms.Label lblSugerir;
        private System.Windows.Forms.CheckedListBox listaCheques;
        private System.Windows.Forms.Label lblNota;
        private System.Windows.Forms.ComboBox comboOpciones;
        private System.Windows.Forms.NumericUpDown respuestaDecimal;
        private System.Windows.Forms.GroupBox gbCondicion;
        private System.Windows.Forms.RadioButton opFalso;
        private System.Windows.Forms.RadioButton opVerdadero;
        private System.Windows.Forms.DateTimePicker hora;
        private System.Windows.Forms.DateTimePicker fecha;
    }
}
