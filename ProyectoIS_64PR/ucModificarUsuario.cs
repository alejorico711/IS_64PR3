using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoIS_64PR
{
    public partial class ucModificarUsuario : UserControl
    {
        public ucModificarUsuario()
        {
            InitializeComponent();
        }

        public void EscribirControles(Servicios_64PR.Usuario u)
        {
            txtDNI.Text = u.DNI;
            txtNombre.Text = u.Nombre;
            txtApellido.Text = u.Apellido;
            txtLogin.Text = u.Login;
            cmbRol.Text = u.Rol;
            txtEmail.Text = u.Email;
        }
        public string DNI()
        {
            return txtDNI.Text;
        }

        public string Nombre()
        {
            return txtNombre.Text;
        }

        public string Apellido()
        {
            return txtApellido.Text;
        }

        public string Rol()
        {
            return cmbRol.Text;
        }

        public string Email()
        {
            return txtEmail.Text;
        }

        public string Login()
        {
            return txtLogin.Text;
        }
        public void LimpiarCampos()
        {
            txtApellido.Text = string.Empty;
            txtDNI.Text = string.Empty;
            txtNombre.Text = string.Empty;
            cmbRol.SelectedIndex = -1;
            txtEmail.Text = string.Empty;
            txtLogin.Text = string.Empty;
        }


    }
}
