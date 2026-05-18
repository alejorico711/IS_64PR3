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
    public partial class FrmLogin_64PR : Form
    {
        BLL_64PR.Usuario gusuarios = new BLL_64PR.Usuario();
        public FrmLogin_64PR()
        {
            InitializeComponent();
            this.AcceptButton = btnIniciarSesion;
            txtContra.UseSystemPasswordChar = true;
        }

        private void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtContra.Text.Trim()) || string.IsNullOrEmpty(txtLogin.Text.Trim()))
            {
                MessageBox.Show("Completá todos los campos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (SessionManager.GetInstance.Usuario != null)
            {
                MessageBox.Show("Ya hay una sesion activa");
                return;
            }

            if (!gusuarios.ExisteUsuario(txtLogin.Text.Trim()))
            {
                MessageBox.Show("Usuario no encontrado.");
                return;
            }

            if (gusuarios.BloqueadoInactivo(txtLogin.Text.Trim()))
            {
                MessageBox.Show("El usuario se encuentra bloqueado o inactivo");
                return;
            }

            if (gusuarios.VerificarClave(txtLogin.Text.Trim(), txtContra.Text.Trim()))
            {
                gusuarios.ReiniciarIntentos(txtLogin.Text.Trim());
                Servicios_64PR.Usuario u = gusuarios.ObtenerUsuario(txtLogin.Text.Trim());
                SessionManager.GetInstance.Login(u);
                if (SessionManager.GetInstance.Usuario.PrimeraVez)
                {
                    MessageBox.Show(
                        "Su contraseña es temporal. Debe cambiarla antes de continuar.",
                        "Cambio de Contraseña Requerido",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    using (var fcc = new FrmCambiarClave_64PR())
                    {
                        if (fcc.ShowDialog() != DialogResult.OK)
                        {
                            SessionManager.GetInstance.Logout();
                            return;
                        }
                    }
                }
                FrmMenu f = new FrmMenu();
                f.Show();
                this.Hide();
                return;
            }

            gusuarios.SumarIntento(txtLogin.Text.Trim());
            lblMensaje.Text = "Intento " + gusuarios.ObtenerIntentos(txtLogin.Text.Trim())+"/3";
            MessageBox.Show("Contraseña incorrecta");
        }
    }
}
