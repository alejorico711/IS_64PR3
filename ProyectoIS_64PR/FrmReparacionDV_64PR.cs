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
    public partial class FrmReparacionDV_64PR : Form
    {
        private Dictionary<string, List<string>> _tablasInconsistentes;

        public FrmReparacionDV_64PR(Dictionary<string, List<string>> tablasInconsistentes)
        {
            InitializeComponent();
            _tablasInconsistentes = tablasInconsistentes;
            textBox1.ReadOnly = true;
            textBox1.Text= $"Se detecto una inconsistencia en la base de datos, lo que podria comprometer la integridad de los datos. {Environment.NewLine} Le solicitamos que seleccione la opcion que quiere realizar a continuacion {Environment.NewLine} Salir: Sale del programa sin tomar ninguna accion al respecto {Environment.NewLine} Restore: Selecciona un respaldo de la base de datos para restaurar a ese estaso {Environment.NewLine} Recalcular: Acepta los cambios realizados en la base de datos e ingresa al sistema, asumiendo el riesgo sobre la integridad de los datos";
            CargarTreeView();
        }
        private void CargarTreeView()
        {
            treeResultados.Nodes.Clear();

            foreach (var entrada in _tablasInconsistentes)
            {
                // Nodo padre = nombre de la tabla
                TreeNode nodoTabla = new TreeNode($"{entrada.Key}  ({entrada.Value.Count})");
                nodoTabla.ForeColor = System.Drawing.Color.DarkRed;

                // Nodos hijo = cada IdFila inconsistente
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
                //FrmLogin_64PR f = new FrmLogin_64PR();
                //f.Show();
                //this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al recalcular: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Servicios_64PR.SessionManager.GetInstance.Logout();
            FrmContenedor_64PR.Instancia.MostrarHijo(new FrmLogin_64PR());
            //FrmLogin_64PR f = new FrmLogin_64PR();
            //f.Show();
            //this.Close();
        }
    }
}
