using BLL_64PR;
using Servicios_64PR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoIS_64PR
{
    public partial class FrmBitacora_64PR : Form, IObservadorIdioma_64PR
    {
        Bitacora_64PR bita = new Bitacora_64PR();
        List<Evento_64PR> lst = new List<Evento_64PR>();
        Dictionary<string, string> textos;
        public FrmBitacora_64PR()
        {
            InitializeComponent();

            cmbCriticidad.DropDownStyle = ComboBoxStyle.DropDownList;  //esto es para que no se pueda escribir en los cmb
            cmbLogins.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbModulos.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipos.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbLogins.DataSource = bita.ListarLogins().OrderBy(x => x).ToList(); //los cargo y ordeno por orden alfabetico
            cmbModulos.DataSource = bita.ListarModulos().OrderBy(x => x).ToList();
            cmbTipos.DataSource = bita.ListarTipos().OrderBy(x => x).ToList();

            LimpiarFiltros();

            dgvEventos.ReadOnly = true;
            dgvEventos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEventos.MultiSelect = false;
            lst = bita.ListarEventos();
            dgvEventos.DataSource = lst;

            GestorIdioma_64PR.GetInstance.Suscribir(this); ///observer del cambio de idioma

            ///Aplico el idioma que ya está cargado
            textos = GestorIdioma_64PR.GetInstance.ObtenerTextos();
            if (textos.Count > 0)
                ActualizarIdioma(textos);
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFiltros();
        }

        private void LimpiarFiltros()
        {
            cmbCriticidad.SelectedIndex = -1;
            cmbLogins.SelectedIndex = -1;
            cmbModulos.SelectedIndex = -1;
            cmbTipos.SelectedIndex = -1;

            dgvEventos.DataSource = null;
            dgvEventos.DataSource = lst;

            cbLogin.Checked = false;
            cmbLogins.Enabled = false;

            cbInicio.Checked = false;
            dtpInicio.Enabled = false;

            cbFin.Checked = false;
            dtpFin.Enabled = false;

            cbCriticidad.Checked = false;
            cmbCriticidad.Enabled = false;

            cbModulo.Checked = false;
            cmbModulos.Enabled = false;

            cbTipo.Checked = false;
            cmbTipos.Enabled = false;
        }

        private void btnAplicar_Click(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            if (dtpInicio.Value.Date > dtpFin.Value.Date)
            {
                MessageBox.Show("La fecha de inicio no puede ser posterior a la fecha de fin.");
                return;
            }
            IEnumerable<Evento_64PR> resultado = lst;
            if (cbLogin.Checked)
                resultado = resultado.Where(e => e.Login == cmbLogins.SelectedItem.ToString());

            if (cbModulo.Checked)
                resultado = resultado.Where(e => e.Modulo == cmbModulos.SelectedItem.ToString());

            if (cbTipo.Checked)
                resultado = resultado.Where(e => e.Tipo == cmbTipos.SelectedItem.ToString());

            if (cbCriticidad.Checked)
                resultado = resultado.Where(e => e.Criticidad == Convert.ToByte(cmbCriticidad.SelectedItem));

            if (cbInicio.Checked)
                resultado = resultado.Where(e => e.FechaHora.Date >= dtpInicio.Value.Date);

            if (cbFin.Checked)
                resultado = resultado.Where(e => e.FechaHora.Date <= dtpFin.Value.Date);

            ///Voy aplicando filtros y sumando
            dgvEventos.DataSource = resultado.ToList();
        }

        private void cbLogin_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbLogin.Checked)
            {
                cmbLogins.Enabled = false;
                cmbLogins.SelectedIndex = -1;
            }
            else
            {
                cmbLogins.Enabled = true;
                cmbLogins.SelectedIndex = 0;
            }
        }

        private void cbModulo_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbModulo.Checked)
            {
                cmbModulos.Enabled = false;
                cmbModulos.SelectedIndex = -1;
            }
            else
            {
                cmbModulos.Enabled = true;
                cmbModulos.SelectedIndex = 0;
            }
        }

        private void cbInicio_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbInicio.Checked)
            {
                dtpInicio.Enabled = false;
            }
            else
            {
                dtpInicio.Enabled = true;
            }
        }

        private void cbFin_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbFin.Checked)
            {
                dtpFin.Enabled = false;
            }
            else
            {
                dtpFin.Enabled = true;
            }
        }

        private void cbTipo_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbTipo.Checked)
            {
                cmbTipos.Enabled = false;
                cmbTipos.SelectedIndex = -1;
            }
            else
            {
                cmbTipos.Enabled = true;
                cmbTipos.SelectedIndex = 0;
            }
        }

        private void cbCriticidad_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbCriticidad.Checked)
            {
                cmbCriticidad.Enabled = false;
                cmbCriticidad .SelectedIndex = -1;
            }
            else
            {
                cmbCriticidad.Enabled = true;
                cmbCriticidad.SelectedIndex = 0;
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {

        }

        public void ActualizarIdioma(Dictionary<string, string> textoss)
        {
            textos = textoss;
            label1.Text = textos["lbl_BitacoraEventos"];
            label2.Text = textos["lbl_Login"];
            label3.Text = textos["lbl_FechaInicio"];
            label4.Text = textos["lbl_FechaFin"];
            label5.Text = textos["lbl_Modulo"];
            label6.Text = textos["lbl_Evento"];
            label7.Text = textos["lbl_Criticidad"];

            btnLimpiar.Text = textos["btn_Limpiar"];
            btnAplicar.Text = textos["btn_Aplicar"];
            btnImprimir.Text = textos["btn_Imprimir"];
        }
    }
}
