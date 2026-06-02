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
    public partial class FrmLogin_64PR : Form, IObservadorIdioma_64PR
    {
        BLL_64PR.Usuario gusuarios = new BLL_64PR.Usuario();
        Dictionary<string, string> textos;
        public FrmLogin_64PR()
        {
            InitializeComponent();
            this.AcceptButton = btnIniciarSesion;
            txtContra.UseSystemPasswordChar = true;
            lblMensaje.Enabled = false;

            GestorIdioma_64PR.GetInstance.Suscribir(this); ///Evento del observer

            CargarComboIdiomas();

            ///Aplico el idioma que ya está cargado
            textos = GestorIdioma_64PR.GetInstance.ObtenerTextos();
            if (textos.Count > 0)
                ActualizarIdioma(textos);
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

        private void cmbIdioma_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cmbIdioma.SelectedItem == null) return;

            string seleccionado = cmbIdioma.SelectedItem.ToString().ToLower(); ///"es" o "en"
            GestorIdioma_64PR.GetInstance.SetIdioma(seleccionado);
            ///El Observer se encarga de actualizar el formulario automáticamente
        }
        public void ActualizarIdioma(Dictionary<string, string> textoss)
        {
            ///Esto me actualiza los textos visibles
            textos = textoss;
            this.Text = textos.ContainsKey("frmLogin_titulo") ? textos["frmLogin_titulo"] : "Iniciar sesion";
            label1.Text = textos.ContainsKey("frmLogin_lblUsuario") ? textos["frmLogin_lblUsuario"] : "Usuario" ;
            label2.Text = textos.ContainsKey("frmLogin_lblContrasena") ? textos["frmLogin_lblContrasena"] : "Contraseña";
            btnIniciarSesion.Text = textos.ContainsKey("frmLogin_btnIniciar") ? textos["frmLogin_btnIniciar"] : "Iniciar sesion";
            if (lblMensaje.Enabled)
            {
                string[] aux = lblMensaje.Text.Split(':');
                aux[0]= textos.ContainsKey("intentos") ? textos["intentos"] : "Intentos";
                lblMensaje.Text = aux[0]+":" + aux[1];
            }
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma_64PR.GetInstance.Desuscribir(this);
            base.OnFormClosed(e);
        }
        private void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            var textos = GestorIdioma_64PR.GetInstance.ObtenerTextos();

            if (string.IsNullOrEmpty(txtContra.Text.Trim()) || string.IsNullOrEmpty(txtLogin.Text.Trim()))
            {
                string msg = textos.ContainsKey("msg_camposVacios") ? textos["msg_camposVacios"] : "Completá todos los campos.";
                MessageBox.Show(msg, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (SessionManager.GetInstance.Usuario != null)
            {
                string msg = textos.ContainsKey("msg_sesionYaIniciada") ? textos["msg_sesionYaIniciada"] : "Ya hay una sesión activa.";
                MessageBox.Show(msg);
                return;
            }

            if (!gusuarios.ExisteUsuario(txtLogin.Text.Trim()))
            {
                string msg = textos.ContainsKey("msg_usuarioNoEncontrado") ? textos["msg_usuarioNoEncontrado"] : "Usuario no encontrado.";
                MessageBox.Show(msg);
                return;
            }

            if (gusuarios.BloqueadoInactivo(txtLogin.Text.Trim()))
            {
                string msg = textos.ContainsKey("msg_bloqueadoInactivo") ? textos["msg_bloqueadoInactivo"] : "El usuario se encuentra bloqueado o inactivo.";
                MessageBox.Show(msg);
                return;
            }

            if (gusuarios.VerificarClave(txtLogin.Text.Trim(), txtContra.Text.Trim()))
            {
                ///Si la clave es correcta entra aca y lo primeo que hacemos es poner en 0 el contador de intentos
                ///en la base de datos por si erro a la contraseña
                gusuarios.ReiniciarIntentos(txtLogin.Text.Trim());

                ///Cargo idioma del usuario desde la BD
                string idiomaGuardado = gusuarios.ObtenerIdioma(txtLogin.Text.Trim());
                GestorIdioma_64PR.GetInstance.SetIdioma(idiomaGuardado);

                ///Obtengo los datos del usuario por el login y lo pongo en sesion
                Servicios_64PR.Usuario u = gusuarios.ObtenerUsuario(txtLogin.Text.Trim());
                SessionManager.GetInstance.Login(u);

                ///Registro el evennto en bitacora
                BLL_64PR.Bitacora_64PR bita2 = new BLL_64PR.Bitacora_64PR();
                Servicios_64PR.Evento_64PR ev2 = new Evento_64PR(SessionManager.GetInstance.Usuario.Login, "1", "1", 5);
                bita2.RegistrarEvento(ev2);

                if (SessionManager.GetInstance.Usuario.PrimeraVez)
                {
                    string msgTemp = textos.ContainsKey("msg_contrasenaTemporal") ? textos["msg_contrasenaTemporal"] : "Su contraseña es temporal. Debe cambiarla antes de continuar.";
                    string tituTemp = textos.ContainsKey("msg_contrasenaTemporal_titulo") ? textos["msg_contrasenaTemporal_titulo"] : "Cambio de Contraseña Requerido";

                    MessageBox.Show(msgTemp, tituTemp, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    using (var fcc = new FrmCambiarClave_64PR())
                    {
                        if (fcc.ShowDialog() != DialogResult.OK)
                        {
                            SessionManager.GetInstance.Logout();
                            return;
                        }
                    }
                }
                else
                {
                    FrmMenu f = new FrmMenu();
                    f.Show();
                    this.Close();
                }
            }
            else
            {
                ///Si la contraseña no es correcta entra aca y sumamos un intento, registrandolo en bitacora
                gusuarios.SumarIntento(txtLogin.Text.Trim());
                BLL_64PR.Bitacora_64PR bita = new BLL_64PR.Bitacora_64PR();
                Servicios_64PR.Evento_64PR ev = new Evento_64PR(txtLogin.Text.Trim(), "1", "3", 4);
                bita.RegistrarEvento(ev);

                ///Obtenemos los intentos del usuario en base de datos y lo volcamos en el label
                string temp = gusuarios.ObtenerIntentos(txtLogin.Text.Trim());
                lblMensaje.Enabled = true;
                lblMensaje.Text = textos["intentos"]+ ": " + temp + "/3";

                if (Convert.ToInt16(temp) == 3)
                {
                    ///Si los intentos llegan a 3 el bloqueo se hace desde la BD, aca lo que hago en registrar en la bitaora nomas
                    ev = new Evento_64PR(txtLogin.Text.Trim(), "1", "4", 3);
                    bita.RegistrarEvento(ev);
                }

                string msgIncorrecta = textos.ContainsKey("msg_contrasenaIncorrecta") ? textos["msg_contrasenaIncorrecta"] : "Contraseña incorrecta.";
                MessageBox.Show(msgIncorrecta);
            }


            /*if (string.IsNullOrEmpty(txtContra.Text.Trim()) || string.IsNullOrEmpty(txtLogin.Text.Trim()))
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
                            return;
                        }
                    }
                }
                else
                {
                    FrmMenu f = new FrmMenu();
                    f.Show();
                    this.Close();
                    return;
                }
            }
            else
            {
                gusuarios.SumarIntento(txtLogin.Text.Trim());
                BLL_64PR.Bitacora_64PR bita = new BLL_64PR.Bitacora_64PR();
                Servicios_64PR.Evento_64PR ev = new Evento_64PR(txtLogin.Text.Trim(), "1", "3", 4);
                bita.RegistrarEvento(ev);
                string temp = gusuarios.ObtenerIntentos(txtLogin.Text.Trim());
                lblMensaje.Text = "Intento " + temp + "/3";
                if (Convert.ToInt16(temp) == 3)
                {
                    ev = new Evento_64PR(txtLogin.Text.Trim(), "1", "4", 3);
                    bita.RegistrarEvento(ev);
                }
                MessageBox.Show("Contraseña incorrecta");
            }*/
        }
    }
}
