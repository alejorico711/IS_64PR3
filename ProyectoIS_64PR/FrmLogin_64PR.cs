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
                BLL_64PR.Bitacora_64PR bita2 = new BLL_64PR.Bitacora_64PR();
                Servicios_64PR.Evento_64PR ev2 = new Evento_64PR(SessionManager.GetInstance.Usuario.Login, "1","1",5);
                bita2.RegistrarEvento(ev2);
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
                            BLL_64PR.Bitacora_64PR bita3 = new BLL_64PR.Bitacora_64PR();
                            Servicios_64PR.Evento_64PR ev3 = new Evento_64PR(SessionManager.GetInstance.Usuario.Login, "1", "5", 5);
                            bita3.RegistrarEvento(ev3);
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
            BLL_64PR.Bitacora_64PR bita = new BLL_64PR.Bitacora_64PR();
            Servicios_64PR.Evento_64PR ev = new Evento_64PR(txtLogin.Text.Trim(), "1", "3", 4);
            bita.RegistrarEvento(ev);
            string temp = gusuarios.ObtenerIntentos(txtLogin.Text.Trim());
            lblMensaje.Text = "Intento " + temp +"/3";
            if(Convert.ToInt16(temp) == 3)
            {
                ev = new Evento_64PR(txtLogin.Text.Trim(), "1", "4", 3);
                bita.RegistrarEvento(ev);
            }
            MessageBox.Show("Contraseña incorrecta");
        }
    }
}
