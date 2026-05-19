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
    public partial class FrmMenu : Form
    {
        public Form formularioactual = null;
        public FrmMenu()
        {
            InitializeComponent();
        }
        public void AbrirFormularioHijo(Form f)
        {
            if (formularioactual == null)
            {
                f.MdiParent = this;
                f.Show();
                f.Enabled = true;
                formularioactual = f;
                f.Dock = DockStyle.Fill;
            }
            else if (formularioactual.GetType() == f.GetType())
            {
                formularioactual.Close();
                formularioactual = null;
            }
            else
            {
                formularioactual.Close();
                formularioactual = null;
                AbrirFormularioHijo(f);
            }
        }
        private void gestionarUsuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmGestionarUsuarios_64PR());
        }

        private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var resultado = MessageBox.Show(
                "¿Está seguro de que desea cerrar la sesión?",
                "Cerrar Sesión", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                BLL_64PR.Bitacora_64PR bita3 = new BLL_64PR.Bitacora_64PR();
                Servicios_64PR.Evento_64PR ev3 = new Evento_64PR(SessionManager.GetInstance.Usuario.Login, "1", "5", 5);
                bita3.RegistrarEvento(ev3);
                SessionManager.GetInstance.Logout();
                var fLogin = new FrmLogin_64PR();
                fLogin.Show();
                this.Close();
            }
        }

        private void cambiarContraseñaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmCambiarClave_64PR());
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BLL_64PR.Bitacora_64PR bita3 = new BLL_64PR.Bitacora_64PR();
            Servicios_64PR.Evento_64PR ev3 = new Evento_64PR(SessionManager.GetInstance.Usuario.Login, "1", "5", 5);
            bita3.RegistrarEvento(ev3);
            SessionManager.GetInstance.Logout();
            Application.Exit();
        }

        private void eventosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmBitacora_64PR());
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmLogin_64PR());
        }
    }
}
