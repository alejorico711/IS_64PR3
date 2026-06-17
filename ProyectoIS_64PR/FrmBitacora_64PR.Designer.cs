namespace ProyectoIS_64PR
{
    partial class FrmBitacora_64PR
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnImprimir = new System.Windows.Forms.Button();
            this.btnAplicar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.dgvEventos = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpInicio = new System.Windows.Forms.DateTimePicker();
            this.dtpFin = new System.Windows.Forms.DateTimePicker();
            this.cmbLogins = new System.Windows.Forms.ComboBox();
            this.cmbModulos = new System.Windows.Forms.ComboBox();
            this.cmbTipos = new System.Windows.Forms.ComboBox();
            this.cmbCriticidad = new System.Windows.Forms.ComboBox();
            this.cbTipo = new System.Windows.Forms.CheckBox();
            this.cbCriticidad = new System.Windows.Forms.CheckBox();
            this.cbFin = new System.Windows.Forms.CheckBox();
            this.cbInicio = new System.Windows.Forms.CheckBox();
            this.cbModulo = new System.Windows.Forms.CheckBox();
            this.cbLogin = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEventos)).BeginInit();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(227, 383);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 16);
            this.label7.TabIndex = 21;
            this.label7.Text = "Criticidad";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(241, 346);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 16);
            this.label6.TabIndex = 20;
            this.label6.Text = "Evento";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 380);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 16);
            this.label5.TabIndex = 19;
            this.label5.Text = "Modulo";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(334, 421);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 16);
            this.label4.TabIndex = 18;
            this.label4.Text = "Fecha fin";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 417);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 16);
            this.label3.TabIndex = 17;
            this.label3.Text = "Fecha inicio";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 341);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 16);
            this.label2.TabIndex = 16;
            this.label2.Text = "Login";
            // 
            // btnImprimir
            // 
            this.btnImprimir.Location = new System.Drawing.Point(722, 399);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(75, 26);
            this.btnImprimir.TabIndex = 15;
            this.btnImprimir.Text = "Imprimir";
            this.btnImprimir.UseVisualStyleBackColor = true;
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            // 
            // btnAplicar
            // 
            this.btnAplicar.Location = new System.Drawing.Point(722, 367);
            this.btnAplicar.Name = "btnAplicar";
            this.btnAplicar.Size = new System.Drawing.Size(75, 26);
            this.btnAplicar.TabIndex = 14;
            this.btnAplicar.Text = "Aplicar";
            this.btnAplicar.UseVisualStyleBackColor = true;
            this.btnAplicar.Click += new System.EventHandler(this.btnAplicar_Click);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Location = new System.Drawing.Point(722, 336);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(75, 26);
            this.btnLimpiar.TabIndex = 13;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // dgvEventos
            // 
            this.dgvEventos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEventos.Location = new System.Drawing.Point(22, 30);
            this.dgvEventos.Name = "dgvEventos";
            this.dgvEventos.RowHeadersWidth = 51;
            this.dgvEventos.RowTemplate.Height = 24;
            this.dgvEventos.Size = new System.Drawing.Size(759, 299);
            this.dgvEventos.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 16);
            this.label1.TabIndex = 11;
            this.label1.Text = "Bitacora de eventos";
            // 
            // dtpInicio
            // 
            this.dtpInicio.Location = new System.Drawing.Point(92, 416);
            this.dtpInicio.MaxDate = new System.DateTime(2026, 5, 19, 0, 0, 0, 0);
            this.dtpInicio.MinDate = new System.DateTime(2026, 4, 1, 0, 0, 0, 0);
            this.dtpInicio.Name = "dtpInicio";
            this.dtpInicio.Size = new System.Drawing.Size(184, 22);
            this.dtpInicio.TabIndex = 22;
            this.dtpInicio.Value = new System.DateTime(2026, 5, 19, 0, 0, 0, 0);
            // 
            // dtpFin
            // 
            this.dtpFin.Location = new System.Drawing.Point(406, 416);
            this.dtpFin.MaxDate = new System.DateTime(2026, 5, 19, 0, 0, 0, 0);
            this.dtpFin.MinDate = new System.DateTime(2026, 4, 1, 0, 0, 0, 0);
            this.dtpFin.Name = "dtpFin";
            this.dtpFin.Size = new System.Drawing.Size(184, 22);
            this.dtpFin.TabIndex = 24;
            this.dtpFin.Value = new System.DateTime(2026, 5, 19, 0, 0, 0, 0);
            // 
            // cmbLogins
            // 
            this.cmbLogins.FormattingEnabled = true;
            this.cmbLogins.Location = new System.Drawing.Point(68, 338);
            this.cmbLogins.Name = "cmbLogins";
            this.cmbLogins.Size = new System.Drawing.Size(121, 24);
            this.cmbLogins.TabIndex = 25;
            // 
            // cmbModulos
            // 
            this.cmbModulos.FormattingEnabled = true;
            this.cmbModulos.Location = new System.Drawing.Point(68, 377);
            this.cmbModulos.Name = "cmbModulos";
            this.cmbModulos.Size = new System.Drawing.Size(121, 24);
            this.cmbModulos.TabIndex = 26;
            // 
            // cmbTipos
            // 
            this.cmbTipos.FormattingEnabled = true;
            this.cmbTipos.Location = new System.Drawing.Point(303, 341);
            this.cmbTipos.Name = "cmbTipos";
            this.cmbTipos.Size = new System.Drawing.Size(121, 24);
            this.cmbTipos.TabIndex = 27;
            // 
            // cmbCriticidad
            // 
            this.cmbCriticidad.FormattingEnabled = true;
            this.cmbCriticidad.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.cmbCriticidad.Location = new System.Drawing.Point(303, 379);
            this.cmbCriticidad.Name = "cmbCriticidad";
            this.cmbCriticidad.Size = new System.Drawing.Size(121, 24);
            this.cmbCriticidad.TabIndex = 28;
            // 
            // cbTipo
            // 
            this.cbTipo.AutoSize = true;
            this.cbTipo.Location = new System.Drawing.Point(430, 345);
            this.cbTipo.Name = "cbTipo";
            this.cbTipo.Size = new System.Drawing.Size(18, 17);
            this.cbTipo.TabIndex = 29;
            this.cbTipo.UseVisualStyleBackColor = true;
            this.cbTipo.CheckedChanged += new System.EventHandler(this.cbTipo_CheckedChanged);
            // 
            // cbCriticidad
            // 
            this.cbCriticidad.AutoSize = true;
            this.cbCriticidad.Location = new System.Drawing.Point(430, 384);
            this.cbCriticidad.Name = "cbCriticidad";
            this.cbCriticidad.Size = new System.Drawing.Size(18, 17);
            this.cbCriticidad.TabIndex = 30;
            this.cbCriticidad.UseVisualStyleBackColor = true;
            this.cbCriticidad.CheckedChanged += new System.EventHandler(this.cbCriticidad_CheckedChanged);
            // 
            // cbFin
            // 
            this.cbFin.AutoSize = true;
            this.cbFin.Location = new System.Drawing.Point(596, 419);
            this.cbFin.Name = "cbFin";
            this.cbFin.Size = new System.Drawing.Size(18, 17);
            this.cbFin.TabIndex = 31;
            this.cbFin.UseVisualStyleBackColor = true;
            this.cbFin.CheckedChanged += new System.EventHandler(this.cbFin_CheckedChanged);
            // 
            // cbInicio
            // 
            this.cbInicio.AutoSize = true;
            this.cbInicio.Location = new System.Drawing.Point(282, 418);
            this.cbInicio.Name = "cbInicio";
            this.cbInicio.Size = new System.Drawing.Size(18, 17);
            this.cbInicio.TabIndex = 32;
            this.cbInicio.UseVisualStyleBackColor = true;
            this.cbInicio.CheckedChanged += new System.EventHandler(this.cbInicio_CheckedChanged);
            // 
            // cbModulo
            // 
            this.cbModulo.AutoSize = true;
            this.cbModulo.Location = new System.Drawing.Point(195, 384);
            this.cbModulo.Name = "cbModulo";
            this.cbModulo.Size = new System.Drawing.Size(18, 17);
            this.cbModulo.TabIndex = 33;
            this.cbModulo.UseVisualStyleBackColor = true;
            this.cbModulo.CheckedChanged += new System.EventHandler(this.cbModulo_CheckedChanged);
            // 
            // cbLogin
            // 
            this.cbLogin.AutoSize = true;
            this.cbLogin.Location = new System.Drawing.Point(195, 340);
            this.cbLogin.Name = "cbLogin";
            this.cbLogin.Size = new System.Drawing.Size(18, 17);
            this.cbLogin.TabIndex = 34;
            this.cbLogin.UseVisualStyleBackColor = true;
            this.cbLogin.CheckedChanged += new System.EventHandler(this.cbLogin_CheckedChanged);
            // 
            // FrmBitacora_64PR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cbLogin);
            this.Controls.Add(this.cbModulo);
            this.Controls.Add(this.cbInicio);
            this.Controls.Add(this.cbFin);
            this.Controls.Add(this.cbCriticidad);
            this.Controls.Add(this.cbTipo);
            this.Controls.Add(this.cmbCriticidad);
            this.Controls.Add(this.cmbTipos);
            this.Controls.Add(this.cmbModulos);
            this.Controls.Add(this.cmbLogins);
            this.Controls.Add(this.dtpFin);
            this.Controls.Add(this.dtpInicio);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnImprimir);
            this.Controls.Add(this.btnAplicar);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.dgvEventos);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmBitacora_64PR";
            this.Text = "FrmBitacora_64PR";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmBitacora_64PR_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEventos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnImprimir;
        private System.Windows.Forms.Button btnAplicar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.DataGridView dgvEventos;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpInicio;
        private System.Windows.Forms.DateTimePicker dtpFin;
        private System.Windows.Forms.ComboBox cmbLogins;
        private System.Windows.Forms.ComboBox cmbModulos;
        private System.Windows.Forms.ComboBox cmbTipos;
        private System.Windows.Forms.ComboBox cmbCriticidad;
        private System.Windows.Forms.CheckBox cbTipo;
        private System.Windows.Forms.CheckBox cbCriticidad;
        private System.Windows.Forms.CheckBox cbFin;
        private System.Windows.Forms.CheckBox cbInicio;
        private System.Windows.Forms.CheckBox cbModulo;
        private System.Windows.Forms.CheckBox cbLogin;
    }
}