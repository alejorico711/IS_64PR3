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
        BLL_64PR.Rol_64PR groles = new BLL_64PR.Rol_64PR();
        public ucModificarUsuario()
        {
            InitializeComponent();
            cmbRol.DataSource = groles.ListarRoles();
            cmbRol.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRol.SelectedIndex = 0;
        }

        /// <summary>
        /// Practicamente esta clase contiene todos metodos para poder
        /// volcar los valores en los controles y para poder
        /// acceder a los valores de los controles del diseñador, ya que,
        /// no se lo puede acceder de otra forma
        /// </summary>
        public void EscribirControles(Servicios_64PR.Usuario u)
        {
            cmbRol.Text = u.Rol.Nombre;
            txtEmail.Text = u.Email;
        }

        public int Rol()
        {
            ///Este +1 es necesario para evitarl el indice 0
            return cmbRol.SelectedIndex+1;
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
