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
        Dictionary<string, string> textos;
        public ucCrearUsuario()
        {
            InitializeComponent();
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

        public void ActualizarIdioma(Dictionary<string, string> textoss)
        {
            textos = textoss;
            label2.Text = textos["ucCrear_Apellido"];
            label3.Text = textos["ucCrear_Nombre"];
        }
    }
}
