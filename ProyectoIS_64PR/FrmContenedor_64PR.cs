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
    public partial class FrmContenedor_64PR : Form
    {
        public static FrmContenedor_64PR Instancia { get; private set; }

        public FrmContenedor_64PR()
        {
            InitializeComponent();
            Instancia = this;
            this.WindowState = FormWindowState.Normal;
            MostrarHijo(new FrmLogin_64PR());
        }
        public void MostrarHijo(Form hijo)
        {
            /// Cierra el hijo anterior
            foreach (Control c in pnlContenido.Controls)
            {
                if (c is Form f) f.Close();
            }
            pnlContenido.Controls.Clear();

            /// Embebe el nuevo form como control dentro del panel
            hijo.TopLevel = false;
            hijo.FormBorderStyle = FormBorderStyle.None;
            hijo.Dock = DockStyle.Fill;
            pnlContenido.Controls.Add(hijo);
            hijo.Show();
        }

        private void FrmContenedor_64PR_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SessionManager.GetInstance.Usuario != null)
            {
                string loginActual = SessionManager.GetInstance.Usuario.Login;
                string idiomaActual = GestorIdioma_64PR.GetInstance.IdiomaActual;
                new BLL_64PR.Usuario().GuardarIdioma(loginActual, idiomaActual);

                BLL_64PR.Bitacora_64PR bita = new BLL_64PR.Bitacora_64PR();
                bita.RegistrarEvento(new Servicios_64PR.Evento_64PR(loginActual, "1", "5", 5));

                SessionManager.GetInstance.Logout();
            }
        }
    }
}
