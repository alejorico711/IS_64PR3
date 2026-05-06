using Servicios_64PR;
using System;
using System.Collections;
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
    public partial class FrmGestionarUsuarios_64PR : Form
    {
        BLL_64PR.Usuario gusuarios = new BLL_64PR.Usuario();
        string modo = "consulta";
        UserControl uc;
        List<Usuario> lst;
        public FrmGestionarUsuarios_64PR()
        {
            InitializeComponent();
            radioButton3.Checked = true;
            lblModo.Text = modo;
            dgvUsuarios.ReadOnly = true;
            dgvUsuarios.MultiSelect = false;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void CargaData()
        {
            dgvUsuarios.DataSource = null;
            lst = gusuarios.Listar();
            dgvUsuarios.DataSource = lst;
            if (radioButton1.Checked == true)
            {
                radioButton1_CheckedChanged(this, EventArgs.Empty);
            }
            else if (radioButton2.Checked == true)
            {
                radioButton2_CheckedChanged(this, EventArgs.Empty);
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            modo = "crear";
            lblModo.Text = modo;
            uc = new ucCrearUsuario();
            pnlContenedor.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            pnlContenedor.Controls.Add(uc);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            switch (modo)
            {
                case "consulta":
                    MessageBox.Show("No esta realizando nigun cambio");
                    break;
                case "crear":

                    if(uc is ucCrearUsuario ucc) //esta validacion creo q no es necesaria, pero me sirve para acceder a los metodos del UC
                    {
                        Servicios_64PR.Usuario u = new Servicios_64PR.Usuario()
                        {
                            DNI = ucc.DNI(),
                            Apellido = ucc.Apellido(),
                            Nombre = ucc.Nombre(),
                            Login = ucc.Nombre() + "." + ucc.Apellido(),
                            Rol = ucc.Rol(),
                            Email = ucc.Email(),
                        };
                        gusuarios.Crear(u);
                        CargaData();
                        ucc.LimpiarCampos();
                        pnlContenedor.Controls.Clear();
                        lblModo.Text = "consulta";
                    }
                    break;
                case "modificar":
                    if (uc is ucModificarUsuario ucm) //esta validacion creo q no es necesaria, pero me sirve para acceder a los metodos del UC
                    {
                        Servicios_64PR.Usuario u = new Servicios_64PR.Usuario()
                        {
                            DNI = ucm.DNI(),
                            Apellido = ucm.Apellido(),
                            Nombre = ucm.Nombre(),
                            Login = ucm.Login(),
                            Rol = ucm.Rol(),
                            Email = ucm.Email(),
                        };
                        gusuarios.Modificar(u);
                        CargaData();
                        ucm.LimpiarCampos();
                        pnlContenedor.Controls.Clear();
                        lblModo.Text = "consulta";
                    }
                    break;
            }
        }

        private void btnDesbloquear_Click(object sender, EventArgs e)
        {
            Servicios_64PR.Usuario u = dgvUsuarios.SelectedRows[0].DataBoundItem as Servicios_64PR.Usuario;
            gusuarios.Desbloquear(u);
            CargaData();
        }

        private void btnActDesact_Click(object sender, EventArgs e)
        {
            Servicios_64PR.Usuario u = dgvUsuarios.SelectedRows[0].DataBoundItem as Servicios_64PR.Usuario;
            gusuarios.Actdesact(u);
            CargaData();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            modo = "modificar";
            lblModo.Text = modo;
            uc = new ucModificarUsuario();
            pnlContenedor.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            pnlContenedor.Controls.Add(uc);
        }

        private void dgvUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                Usuario u = dgvUsuarios.SelectedRows[0].DataBoundItem as Servicios_64PR.Usuario;
                if (uc is ucModificarUsuario ucc) //esta validacion creo q no es necesaria, pero me sirve para acceder a los metodos del UC
                {
                    ucc.EscribirControles(u);
                }
            }
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
        }

        private void dgvUsuarios_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            //activos
            if (radioButton1.Checked == true)
            {
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                dgvUsuarios.DataSource = null;
                dgvUsuarios.DataSource = lst.Where(u => u.Activo == true).ToList();
                lblCantidad.Text = "Cantidad de usuarios: " + lst.Where(u => u.Activo == true).ToList().Count();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            //no activos
            if (radioButton2.Checked == true)
            {
                radioButton1.Checked = false;
                radioButton3.Checked = false;
                dgvUsuarios.DataSource = null;
                dgvUsuarios.DataSource = lst.Where(u => u.Activo == false).ToList();
                lblCantidad.Text = "Cantidad de usuarios: " + lst.Where(u => u.Activo == false).ToList().Count();

            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                dgvUsuarios.DataSource = null;
                dgvUsuarios.DataSource = lst;
                if(lst != null)  //if necesario para que no ejecute esta linea de codigo durante la construccion del frm
                {
                    lblCantidad.Text = "Cantidad de usuarios: " + lst.Count.ToString();
                }
            }
        }

        private void FrmGestionarUsuarios_64PR_Load(object sender, EventArgs e)
        {
            CargaData();
            lblCantidad.Text = "Cantidad de usuarios: " + lst.Count.ToString();
        }

        private void dgvUsuarios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow row in dgvUsuarios.Rows)
            {
                if (!(bool)row.Cells["Activo"].Value)
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
            }
        }
    }
}
