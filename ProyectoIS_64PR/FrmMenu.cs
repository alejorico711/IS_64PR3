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
    public partial class FrmMenu : Form, IObservadorIdioma_64PR
    {
        public Form formularioactual = null;
        BLL_64PR.Usuario gusuarios = new BLL_64PR.Usuario();
        public FrmMenu()
        {
            InitializeComponent();
            GestorIdioma_64PR.GetInstance.Suscribir(this);

            ///Aplico idioma actual al abrir
            var textos = GestorIdioma_64PR.GetInstance.ObtenerTextos();
            if (textos.Count > 0)
                ActualizarIdioma(textos);

            ///Agrego el selector de idioma al menú en tiempo de ejecución
            AgregarSelectorIdioma();
        }
        public void ActualizarIdioma(Dictionary<string, string> textos)
        {
            if (textos.ContainsKey("frmMenu_titulo")) this.Text = textos["frmMenu_titulo"];
            if (textos.ContainsKey("frmMenu_login")) loginToolStripMenuItem1.Text = textos["frmMenu_login"];
            if (textos.ContainsKey("frmMenu_gestionUsuarios")) gestionarUsuariosToolStripMenuItem.Text = textos["frmMenu_gestionUsuarios"];
            if (textos.ContainsKey("frmMenu_cambiarContrasena")) cambiarContraseñaToolStripMenuItem1.Text = textos["frmMenu_cambiarContrasena"];
            if (textos.ContainsKey("frmMenu_eventos")) eventosToolStripMenuItem.Text = textos["frmMenu_eventos"];
            if (textos.ContainsKey("frmMenu_cerrarSesion")) cerrarSesionToolStripMenuItem1.Text = textos["frmMenu_cerrarSesion"];
            if (textos.ContainsKey("frmMenu_salir")) salirToolStripMenuItem1.Text = textos["frmMenu_salir"];
            if (textos.ContainsKey("frmMenu_idioma")) idiomaToolStripMenuItem1.Text = textos["frmMenu_idioma"];
            if (textos.ContainsKey("frmMenu_gestionFamilias")) gestionarPermisosToolStripMenuItem.Text = textos["frmMenu_gestionFamilias"];
        }
        private void AgregarSelectorIdioma()
        {
            var itemIdioma = idiomaToolStripMenuItem1;

            foreach (string codigo in GestorIdioma_64PR.GetInstance.IdiomasDisponibles())
            {
                string codigoLocal = codigo; ///Captura para el delegado de abajo
                string etiqueta = codigo.ToUpper(); /// Tipo "ES" / "EN"

                var subItem = new ToolStripMenuItem(etiqueta);
                subItem.Click += (s, e) =>
                {
                    ///No guardamos en BD aca, ya que se guarda al hacer logout
                    GestorIdioma_64PR.GetInstance.SetIdioma(codigoLocal);
                };
                itemIdioma.DropDownItems.Add(subItem);
            }
            configuracionToolStripMenuItem.DropDownItems.Add(itemIdioma);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma_64PR.GetInstance.Desuscribir(this);
            base.OnFormClosed(e);
        }
        public void AbrirFormularioHijo(Form f)
        {
            ///Esta funcion me permite abrir formularios hijos en el panel y cerrarlos sis se toca sobre el mismo modulo
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

        private void eventosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmBitacora_64PR());
        }

        private void gestionarPermisosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmGestionFamilias_64PR());
        }

        private void loginToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmLogin_64PR());
        }

        private void cambiarContraseñaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmCambiarClave_64PR());
        }

        private void cerrarSesionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var textos = GestorIdioma_64PR.GetInstance.ObtenerTextos();
            string msg = textos.ContainsKey("msg_cerrarSesion") ? textos["msg_cerrarSesion"] : "¿Está seguro de que desea cerrar la sesión?";
            string titulo = textos.ContainsKey("msg_cerrarSesion_titulo") ? textos["msg_cerrarSesion_titulo"] : "Cerrar Sesión";

            var resultado = MessageBox.Show(msg, titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                ///Guardamos el idioma en BD antes de cerrar sesión
                string loginActual = SessionManager.GetInstance.Usuario.Login;
                string idiomaActual = GestorIdioma_64PR.GetInstance.IdiomaActual;
                gusuarios.GuardarIdioma(loginActual, idiomaActual);

                ///Registrasmo ele vento en bitacora
                BLL_64PR.Bitacora_64PR bita3 = new BLL_64PR.Bitacora_64PR();
                Servicios_64PR.Evento_64PR ev3 = new Evento_64PR(loginActual, "1", "5", 5);
                bita3.RegistrarEvento(ev3);

                SessionManager.GetInstance.Logout();

                var fLogin = new FrmLogin_64PR();
                fLogin.Show();
                this.Close();
            }
        }

        private void salirToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ///Guardamos el idioma en BD
            string loginActual = SessionManager.GetInstance.Usuario.Login;
            string idiomaActual = GestorIdioma_64PR.GetInstance.IdiomaActual;
            gusuarios.GuardarIdioma(loginActual, idiomaActual);

            ///Registramos el evento en bitacora
            BLL_64PR.Bitacora_64PR bita3 = new BLL_64PR.Bitacora_64PR();
            Servicios_64PR.Evento_64PR ev3 = new Evento_64PR(loginActual, "1", "5", 5);
            bita3.RegistrarEvento(ev3);

            SessionManager.GetInstance.Logout();
            Application.Exit();
        }
    }
}
