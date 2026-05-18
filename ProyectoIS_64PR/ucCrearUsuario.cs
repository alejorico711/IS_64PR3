using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProyectoIS_64PR
{
    public partial class ucCrearUsuario : UserControl
    {
        public ucCrearUsuario()
        {
            InitializeComponent();
            cmbRol.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRol.SelectedIndex = 0;
        }

        public string DNI()
        {
            return txtDNI.Text.Trim();
        }

        public string Nombre()
        {
            return txtNombre.Text.Trim();
        }

        public string Apellido()
        {
            return txtApellido.Text.Trim();
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
            txtApellido.Text = string.Empty;
            txtDNI.Text = string.Empty;
            txtNombre.Text = string.Empty;
            cmbRol.SelectedIndex = -1;
            txtEmail.Text = string.Empty;
        }
    }
}
