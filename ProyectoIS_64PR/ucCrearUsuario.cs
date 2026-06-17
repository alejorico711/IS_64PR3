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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProyectoIS_64PR
{
    public partial class ucCrearUsuario : UserControl, IObservadorIdioma_64PR
    {
        BLL_64PR.Rol_64PR groles = new BLL_64PR.Rol_64PR();
        Dictionary<string, string> textos;
        List<Servicios_64PR.Rol_64PR> lst;
        public ucCrearUsuario()
        {
            InitializeComponent();
            lst = groles.ListarRoles();
            cmbRol.DataSource = lst;
            cmbRol.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRol.SelectedIndex = 0;

            GestorIdioma_64PR.GetInstance.Suscribir(this); ///Evento del observer

            textos = GestorIdioma_64PR.GetInstance.ObtenerTextos();
            if (textos.Count > 0)
                ActualizarIdioma(textos);
        }

        /// <summary>
        /// Practicamente esta clase contiene todos metodos para poder
        /// acceder a los valores de los controles del diseñador, ya que,
        /// no se lo puede acceder de otra forma
        /// </summary>
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

        public int Rol()
        {
            Servicios_64PR.Rol_64PR rol = cmbRol.SelectedItem as Servicios_64PR.Rol_64PR;
            return rol.Id;
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

        public void ActualizarIdioma(Dictionary<string, string> textoss)
        {
            textos = textoss;
            label2.Text = textos["ucCrear_Apellido"];
            label3.Text = textos["ucCrear_Nombre"];
        }
    }
}
