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
            if (textos.ContainsKey("frmMenu_idioma")) idiomaToolStripMenuItem1.Text = textos["frmMenu_idioma"];
            if (textos.ContainsKey("frmMenu_gestionFamilias")) gestionarPermisosToolStripMenuItem.Text = textos["frmMenu_gestionFamilias"];
            configuracionToolStripMenuItem.Text = textos["configuracion"];
            gestionarRolesToolStripMenuItem.Text = textos["gestionar_roles"];
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
        public void AbrirFormularioHijo(Form f)
        {
            if (formularioactual != null)
            {
                if (formularioactual.GetType() == f.GetType())
                {
                    formularioactual.Close();
                    pnlContenidoMenu.Controls.Clear();
                    formularioactual = null;
                    return;
                }

                formularioactual.Close();
                pnlContenidoMenu.Controls.Clear();
                formularioactual = null;
            }

            f.TopLevel = false;
            f.FormBorderStyle = FormBorderStyle.None;
            f.Dock = DockStyle.Fill;
            pnlContenidoMenu.Controls.Add(f);
            f.Show();
            formularioactual = f;
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

                FrmContenedor_64PR.Instancia.MostrarHijo(new FrmLogin_64PR());
                //var fLogin = new FrmLogin_64PR();
                //fLogin.Show();
                //this.Hide();
                ///aca lo idea seria usar el metodo .Close(), pero ese metodo me llama al metodo de abajo
                ///que contiene el application.exit y me detiene la ejecucion del programa
                SessionManager.GetInstance.Logout();
            }
        }

        private void gestionarRolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmGestionarRoles_64PR());
        }

        private void FrmMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void FrmMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            GestorIdioma_64PR.GetInstance.Desuscribir(this); ///observer del cambio de idioma
        }

        private void FrmMenu_Load(object sender, EventArgs e)
        {
            ConfigurarPermisos();
        }

        private void ConfigurarPermisos()
        {
            Servicios_64PR.Rol_64PR rolUsuario = Servicios_64PR.SessionManager.GetInstance.Usuario.Rol;

            bool puedeGestionarUsuarios = rolUsuario.TienePermiso(Patentes_64PR.CrearUsuario) ||
                                          rolUsuario.TienePermiso(Patentes_64PR.ModificarUsuario) ||
                                          rolUsuario.TienePermiso(Patentes_64PR.ActivarDesactivarUsuarios) ||
                                          rolUsuario.TienePermiso(Patentes_64PR.DesbloquearUsuario);
            gestionarUsuariosToolStripMenuItem.Visible = puedeGestionarUsuarios;

            bool puedeGestionarFamilias = rolUsuario.TienePermiso(Patentes_64PR.CrearFamilias) ||
                                          rolUsuario.TienePermiso(Patentes_64PR.EliminarFamilias) ||
                                          rolUsuario.TienePermiso(Patentes_64PR.ModificarFamilias);
            gestionarPermisosToolStripMenuItem.Visible = puedeGestionarFamilias;

            bool puedeGestionarRoles = rolUsuario.TienePermiso(Patentes_64PR.CrearRoles) ||
                                       rolUsuario.TienePermiso(Patentes_64PR.EliminarRoles) ||
                                       rolUsuario.TienePermiso(Patentes_64PR.ModificarRoles);
            gestionarRolesToolStripMenuItem.Visible = puedeGestionarRoles;

            eventosToolStripMenuItem.Visible = rolUsuario.TienePermiso(Patentes_64PR.Bitacora);
        }
    }
}
