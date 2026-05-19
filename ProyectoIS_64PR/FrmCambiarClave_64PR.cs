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
            txtContra.UseSystemPasswordChar = true;
            txtConfirmar.UseSystemPasswordChar = true;
            txtNueva.UseSystemPasswordChar = true;
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                BLL_64PR.Usuario gusuario = new BLL_64PR.Usuario();
                byte[] hashalmacenado = gusuario.ObtenerHashAlmacenado(SessionManager.GetInstance.Usuario.Login);
                if (Encriptación.Instancia.VerifyPassword(txtContra.Text.Trim(), hashalmacenado))
                {
                    if(txtContra.Text.Trim() == txtNueva.Text.Trim() && txtNueva.Text.Trim() == txtConfirmar.Text.Trim())
                    {
                        MessageBox.Show("No puede establacer como nueva clave su clave actual");
                    }
                    else
                    {
                        gusuario.CambiarClave(txtNueva.Text.Trim(), txtConfirmar.Text.Trim());
                        BLL_64PR.Bitacora_64PR bita = new BLL_64PR.Bitacora_64PR();
                        Servicios_64PR.Evento_64PR ev = new Evento_64PR(SessionManager.GetInstance.Usuario.Login, "1", "2", 4);
                        bita.RegistrarEvento(ev);
                        MessageBox.Show("Contraseña cambiada exitosamente.", "Exito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("La contraseña actual es incorrecta");
                }
                
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
