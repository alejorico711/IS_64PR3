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
    public partial class FrmCambiarClave_64PR : Form
    {
        public FrmCambiarClave_64PR()
        {
            InitializeComponent();
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                //agregar contra actual
                BLL_64PR.Usuario.CambiarClave(txtNueva.Text.Trim(), txtConfirmar.Text.Trim());

                MessageBox.Show("Contraseña cambiada exitosamente.", "Exito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                txtNueva.Clear();
                txtConfirmar.Clear();
            }
        }
    }
}
