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
        private Dictionary<string, List<string>> _tablasInconsistentes;

        public FrmReparacionDV_64PR(Dictionary<string, List<string>> tablasInconsistentes)
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
                /// Nodo padre = nombre de la tabla
                TreeNode nodoTabla = new TreeNode($"{entrada.Key}  ({entrada.Value.Count})");
                nodoTabla.ForeColor = System.Drawing.Color.DarkRed;

                /// Nodos hijo = cada IdFila inconsistente
                foreach (string idFila in entrada.Value)
                {
                    TreeNode nodoFila = new TreeNode($"ID: {idFila}");
                    nodoTabla.Nodes.Add(nodoFila);
                }

                treeResultados.Nodes.Add(nodoTabla);
            }

            treeResultados.ExpandAll();
        }

        private void btnRecalcular_Click(object sender, EventArgs e)
        {
            var confirmacion = MessageBox.Show(
                "Esto va a recalcular los dígitos verificadores de todas las tablas con los datos actuales.\n¿Confirmar?",
                "Recalcular integridad", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes) return;

            try
            {
                BLL_64PR.DV_64PR bllDV = new BLL_64PR.DV_64PR();
                bllDV.RecalcularIntegridadCompleta();

                MessageBox.Show("Dígitos verificadores recalculados correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                Servicios_64PR.SessionManager.GetInstance.Logout();
                FrmContenedor_64PR.Instancia.MostrarHijo(new FrmLogin_64PR());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al recalcular: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de Backup (*.bak)|*.bak";
                ofd.Title = "Seleccione el archivo de backup a restaurar";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string rutaBackup = ofd.FileName;

                    var confirmacion = MessageBox.Show(
                        "Esto va a reemplazar la base de datos actual con el backup seleccionado.\n" +
                        "Todos los cambios no respaldados se perderán. ¿Desea continuar?",
                        "Confirmar restauración", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (confirmacion != DialogResult.Yes) return;

                    try
                    {
                        BLL_64PR.Backup gBackup = new BLL_64PR.Backup();
                        this.Cursor = Cursors.WaitCursor;

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

                        MessageBox.Show("Base de datos restaurada correctamente.\nLa aplicación se cerrará para reconectar.",
                                        "Restauración exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Application.Restart();
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Ocurrió un error al restaurar el backup: " + ex.Message,
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
            Servicios_64PR.SessionManager.GetInstance.Logout();
            FrmContenedor_64PR.Instancia.MostrarHijo(new FrmLogin_64PR());
        }

        public void ActualizarIdioma(Dictionary<string, string> textos)
        {
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
