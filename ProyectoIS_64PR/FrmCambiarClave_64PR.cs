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
    public partial class FrmCambiarClave_64PR : Form, IObservadorIdioma_64PR
    {
        Dictionary<string, string> textos;
        public FrmCambiarClave_64PR()
        {
            InitializeComponent();
            txtContra.UseSystemPasswordChar = true;
            txtConfirmar.UseSystemPasswordChar = true;
            txtNueva.UseSystemPasswordChar = true;

            GestorIdioma_64PR.GetInstance.Suscribir(this); ///observer del cambio de idioma

            ///Aplico el idioma que ya está cargado
            textos = GestorIdioma_64PR.GetInstance.ObtenerTextos();
            if (textos.Count > 0)
                ActualizarIdioma(textos);
        }
        public void ActualizarIdioma(Dictionary<string, string> textoss)
        {
            ///esto lo que hace es actualizar los textos visibles
            textos = textoss;
            if (textos.ContainsKey("frmCambiarClave_titulo")) this.Text = textos["frmCambiarClave_titulo"];
            if (textos.ContainsKey("frmCambiarClave_lblActual")) label1.Text = textos["frmCambiarClave_lblActual"];
            if (textos.ContainsKey("frmCambiarClave_lblNueva")) label2.Text = textos["frmCambiarClave_lblNueva"];
            if (textos.ContainsKey("frmCambiarClave_lblConfirmar")) label3.Text = textos["frmCambiarClave_lblConfirmar"];
            if (textos.ContainsKey("frmCambiarClave_btnConfirmar")) btnConfirmar.Text = textos["frmCambiarClave_btnConfirmar"];
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                BLL_64PR.Usuario gusuario = new BLL_64PR.Usuario();

                ///Obtengo el hash en BD y comparo con la contraseña ingresada
                byte[] hashalmacenado = gusuario.ObtenerHashAlmacenado(SessionManager.GetInstance.Usuario.Login);
                if (Encriptación.Instancia.VerifyPassword(txtContra.Text.Trim(), hashalmacenado))
                {
                    if(txtContra.Text.Trim() == txtNueva.Text.Trim() && txtNueva.Text.Trim() == txtConfirmar.Text.Trim())
                    {
                        MessageBox.Show(textos["msg_claveActualComoNueva"]);
                    }
                    else
                    {

                        ///Linea que me cambia la contraseña
                        gusuario.CambiarClave(txtNueva.Text.Trim(), txtConfirmar.Text.Trim());

                        ///registro el evento en bitacora
                        BLL_64PR.Bitacora_64PR bita = new BLL_64PR.Bitacora_64PR();
                        Servicios_64PR.Evento_64PR ev = new Evento_64PR(SessionManager.GetInstance.Usuario.Login, "1", "2", 4);
                        bita.RegistrarEvento(ev);

                        MessageBox.Show(textos["msg_cambio_exitoso"], "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;

                        FrmContenedor_64PR.Instancia.MostrarHijo(new FrmLogin_64PR());

                        SessionManager.GetInstance.Logout();
                    }
                }
                else
                {
                    MessageBox.Show(textos["msg_contrasenaIncorrecta"]);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                txtNueva.Clear();
                txtConfirmar.Clear();
            }
        }

        private void FrmCambiarClave_64PR_FormClosed(object sender, FormClosedEventArgs e)
        {
            GestorIdioma_64PR.GetInstance.Desuscribir(this);
        }
    }
}
