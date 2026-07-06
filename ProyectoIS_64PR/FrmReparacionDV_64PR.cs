using Servicios_64PR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoIS_64PR
{
    public partial class FrmReparacionDV_64PR : Form, IObservadorIdioma_64PR
    {
        private Dictionary<string, List<FilaInconsistente_64PR>> _tablasInconsistentes;
        Dictionary<string, string> textos;

        public FrmReparacionDV_64PR(Dictionary<string, List<FilaInconsistente_64PR>> tablasInconsistentes)
        {
            InitializeComponent();

            GestorIdioma_64PR.GetInstance.Suscribir(this);

            CargarComboIdiomas();
            ///Aplico idioma actual al abrir
            var textos = GestorIdioma_64PR.GetInstance.ObtenerTextos();
            if (textos.Count > 0)
                ActualizarIdioma(textos);

            _tablasInconsistentes = tablasInconsistentes;
            textBox1.ReadOnly = true;
            CargarTreeView();
        }
        private void CargarComboIdiomas()
        {
            cmbIdioma.Items.Clear();

            foreach (string codigo in GestorIdioma_64PR.GetInstance.IdiomasDisponibles())
                cmbIdioma.Items.Add(codigo.ToUpper()); /// "ES", "EN"

            ///Seleccionar el idioma actual
            string actual = GestorIdioma_64PR.GetInstance.IdiomaActual.ToUpper();
            int index = cmbIdioma.Items.IndexOf(actual);
            if (index >= 0)
                cmbIdioma.SelectedIndex = index;

            cmbIdioma.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private void CargarTreeView()
        {
            treeResultados.Nodes.Clear();

            foreach (var entrada in _tablasInconsistentes)
            {
                TreeNode nodoTabla = new TreeNode($"{entrada.Key}  ({entrada.Value.Count})");
                nodoTabla.ForeColor = Color.DarkRed;

                foreach (FilaInconsistente_64PR fila in entrada.Value)
                {
                    string etiqueta;
                    Color color;

                    switch (fila.Tipo)
                    {
                        case TipoAnomalia_64PR.Insercion:
                            etiqueta = "(INSERT)";
                            color = Color.DarkGreen;
                            break;
                        case TipoAnomalia_64PR.Modificacion:
                            etiqueta = "(UPDATE)";
                            color = Color.DarkOrange;
                            break;
                        case TipoAnomalia_64PR.Eliminacion:
                            etiqueta = "(DELETE)";
                            color = Color.Firebrick;
                            break;
                        default:
                            etiqueta = "";
                            color = Color.Black;
                            break;
                    }

                    TreeNode nodoFila = new TreeNode($"ID: {fila.IdFila}  -  {etiqueta}");
                    nodoFila.ForeColor = color;
                    nodoTabla.Nodes.Add(nodoFila);
                }

                treeResultados.Nodes.Add(nodoTabla);
            }

            treeResultados.ExpandAll();
        }

        private void btnRecalcular_Click(object sender, EventArgs e)
        {
            var confirmacion = MessageBox.Show(
                textos["advertencia"],
                textos["Recalcular integridad"], MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes) return;

            try
            {
                BLL_64PR.DV_64PR bllDV = new BLL_64PR.DV_64PR();
                bllDV.RecalcularIntegridadCompleta();

                MessageBox.Show(textos["dvs_recalculados"],
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;

                BLL_64PR.Bitacora_64PR bita = new BLL_64PR.Bitacora_64PR();
                Servicios_64PR.Evento_64PR ev = new Evento_64PR(SessionManager.GetInstance.Usuario.Login, ((int)BLL_64PR.Bitacora_64PR.ModuloBitacora_64PR.Login).ToString(), ((int)BLL_64PR.Bitacora_64PR.TipoEventoBitacora_64PR.Logout).ToString(), 5);
                bita.RegistrarEvento(ev);

                Servicios_64PR.SessionManager.GetInstance.Logout();
                FrmContenedor_64PR.Instancia.MostrarHijo(new FrmLogin_64PR());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de Backup (*.bak)|*.bak";
                ofd.Title = textos["seleccionar_archivo"];

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string rutaBackup = ofd.FileName;

                    var confirmacion = MessageBox.Show(textos["msg_confirmacion"],
                        textos["Confirmar restauración"], MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (confirmacion != DialogResult.Yes) return;

                    try
                    {
                        BLL_64PR.Backup gBackup = new BLL_64PR.Backup();

                        /// leer los nombres lógicos del backup
                        DataTable fileList = gBackup.ObtenerFileList(rutaBackup);
                        string logicalData = null, logicalLog = null;

                        foreach (DataRow fila in fileList.Rows)
                        {
                            string tipo = fila["Type"].ToString();
                            if (tipo == "D") logicalData = fila["LogicalName"].ToString();
                            else if (tipo == "L") logicalLog = fila["LogicalName"].ToString();
                        }

                        /// calcular dónde van a ir los archivos físicos
                        string rutaDefaultData = gBackup.ObtenerRutaDefaultData();
                        string rutaDestinoMdf = System.IO.Path.Combine(rutaDefaultData, "BD_64PR.mdf");
                        string rutaDestinoLdf = System.IO.Path.Combine(rutaDefaultData, "BD_64PR_log.ldf");

                        gBackup.RestaurarBackup(rutaBackup, logicalData, logicalLog, rutaDestinoMdf, rutaDestinoLdf);

                        BLL_64PR.Bitacora_64PR bita = new BLL_64PR.Bitacora_64PR();
                        Servicios_64PR.Evento_64PR ev = new Evento_64PR(SessionManager.GetInstance.Usuario.Login, ((int)BLL_64PR.Bitacora_64PR.ModuloBitacora_64PR.Login).ToString(), ((int)BLL_64PR.Bitacora_64PR.TipoEventoBitacora_64PR.Restore).ToString(), 1);
                        bita.RegistrarEvento(ev);

                        MessageBox.Show(textos["msg_restauracion"],
                                        textos["Restauración exitosa"], MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Application.Restart();
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Error: " + ex.Message,
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            BLL_64PR.Bitacora_64PR bita = new BLL_64PR.Bitacora_64PR();
            Servicios_64PR.Evento_64PR ev = new Evento_64PR(SessionManager.GetInstance.Usuario.Login, ((int)BLL_64PR.Bitacora_64PR.ModuloBitacora_64PR.Login).ToString(), ((int)BLL_64PR.Bitacora_64PR.TipoEventoBitacora_64PR.Logout).ToString(), 5);
            bita.RegistrarEvento(ev);

            Servicios_64PR.SessionManager.GetInstance.Logout();
            FrmContenedor_64PR.Instancia.MostrarHijo(new FrmLogin_64PR());
        }

        public void ActualizarIdioma(Dictionary<string, string> textoss)
        {
            textos=textoss;
            btnSalir.Text = textos["frmMenu_salir"];
            btnRestore.Text = textos["restaurar"];
            btnRecalcular.Text = textos["recalcular"];
            textBox1.Text = textos["msg_inconsistencia"];
        }

        private void cmbIdioma_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbIdioma.SelectedItem == null) return;

            string seleccionado = cmbIdioma.SelectedItem.ToString().ToLower(); ///"es" o "en"
            GestorIdioma_64PR.GetInstance.SetIdioma(seleccionado);
            ///El Observer se encarga de actualizar el formulario automáticamente
        }
    }
}
