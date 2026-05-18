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
            cmbRol.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRol.SelectedIndex = 0;
        }

        public void EscribirControles(Servicios_64PR.Usuario u)
        {
            cmbRol.Text = u.Rol;
            txtEmail.Text = u.Email;
        }

        public string Rol()
        {
            return cmbRol.Text.Trim();
        }

        public string Email()
        {
            return txtEmail.Text.Trim();
        }

        public void LimpiarCampos()
        {
            cmbRol.SelectedIndex = -1;
            txtEmail.Text = string.Empty;
        }


    }
}
