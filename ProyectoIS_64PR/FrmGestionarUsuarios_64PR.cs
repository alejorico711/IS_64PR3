using Servicios_64PR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace ProyectoIS_64PR
{

    public partial class FrmGestionarUsuarios_64PR : Form, IObservadorIdioma_64PR
    {
        BLL_64PR.Bitacora_64PR bita = new BLL_64PR.Bitacora_64PR();
        Servicios_64PR.Evento_64PR ev;

        BLL_64PR.Usuario gusuarios = new BLL_64PR.Usuario();
        public Dictionary<string, string> textos;

        ///Mi bandera para el switch
        ///El uc generico para poder intanciar mis 2 UC, sin tener que crear 2 especificos
        string modo = "consulta";
        UserControl uc;
        List<Usuario> lst;
        public FrmGestionarUsuarios_64PR()
        {
            InitializeComponent(); 

            radioButton3.Checked = true;
            dgvUsuarios.ReadOnly = true;
            dgvUsuarios.MultiSelect = false;

            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvUsuarios.BackgroundColor = SystemColors.Menu;
            dgvUsuarios.BorderStyle = BorderStyle.None;
            CargaData();

            btnGuardar.Enabled = false;

            GestorIdioma_64PR.GetInstance.Suscribir(this); ///Observer del cambio de idioma

            ///Aplico el idioma que ya está cargado
            textos = GestorIdioma_64PR.GetInstance.ObtenerTextos();
            if (textos.Count > 0)
                ActualizarIdioma(textos);
            lblModo.Text = textos["frmGestionUsuarios_lblModoConsulta"];
            lblCantidad.Text = textos["frmGestionUsuarios_lblCantidad"] + lst.Count.ToString();
        }
        
        public void ActualizarIdioma(Dictionary<string, string> textoss)
        {
            ///Esto lo que hace es actualizar los textos visibles
            textos = textoss;

            dgvUsuarios.Columns["Apellido"].HeaderText = textos["apellido"];
            dgvUsuarios.Columns["Nombre"].HeaderText = textos["nombre"];
            dgvUsuarios.Columns["Login"].HeaderText = textos["login"];
            dgvUsuarios.Columns["Rol"].HeaderText = textos["rol"];
            dgvUsuarios.Columns["Email"].HeaderText = textos["email"];
            dgvUsuarios.Columns["Bloqueado"].HeaderText = textos["bloqueado"];
            dgvUsuarios.Columns["Activo"].HeaderText = textos["activo"];
            dgvUsuarios.Columns["PrimeraVez"].HeaderText = textos["primera_vez"];


            if (textos.ContainsKey("frmGestionUsuarios_titulo")) this.Text = textos["frmGestionUsuarios_titulo"];
            if (textos.ContainsKey("frmGestionUsuarios_btnCrear")) btnCrear.Text = textos["frmGestionUsuarios_btnCrear"];
            if (textos.ContainsKey("frmGestionUsuarios_btnDesbloquear")) btnDesbloquear.Text = textos["frmGestionUsuarios_btnDesbloquear"];
            if (textos.ContainsKey("frmGestionUsuarios_btnModificar")) btnModificar.Text = textos["frmGestionUsuarios_btnModificar"];
            if (textos.ContainsKey("frmGestionUsuarios_btnActDesact")) btnActDesact.Text = textos["frmGestionUsuarios_btnActDesact"];
            if (textos.ContainsKey("frmGestionUsuarios_btnGuardar")) btnGuardar.Text = textos["frmGestionUsuarios_btnGuardar"];
            if (textos.ContainsKey("frmGestionUsuarios_rbActivos")) radioButton1.Text = textos["frmGestionUsuarios_rbActivos"];
            if (textos.ContainsKey("frmGestionUsuarios_rbNoActivos")) radioButton2.Text = textos["frmGestionUsuarios_rbNoActivos"];
            if (textos.ContainsKey("frmGestionUsuarios_rbTodos")) radioButton3.Text = textos["frmGestionUsuarios_rbTodos"];

            ///Necesario para no perder la cantidad de usuarios en memoria al momento de actualizar el idioma
            string[] aux = lblCantidad.Text.Split(':'); 
            if (textos.ContainsKey("frmGestionUsuarios_lblCantidad")) lblCantidad.Text = textos["frmGestionUsuarios_lblCantidad"] + aux[1];

            ///Necesario para no perder el modo al momento de actualizar el idioma
            switch (modo)
            {
                case "consulta":
                    lblModo.Text = textos["frmGestionUsuarios_lblModoConsulta"];
                    break;
                case "crear":
                    lblModo.Text = textos["frmGestionUsuarios_lblModoCrear"];
                    break;
                case "modificar":
                    lblModo.Text = textos["frmGestionUsuarios_lblModoModificar"];
                    break;
                default:
                    break;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma_64PR.GetInstance.Desuscribir(this);
            base.OnFormClosed(e);
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
            lblModo.Text = textos["frmGestionUsuarios_lblModoCrear"];
            uc = new ucCrearUsuario();
            pnlContenedor.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            pnlContenedor.Controls.Add(uc);
            btnGuardar.Enabled = true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            switch (modo)
            {
                case "consulta":
                    MessageBox.Show(textos["msg_NoCambios"]);
                    break;
                case "crear":

                    if(uc is ucCrearUsuario ucc) ///Esta validacion creo que no es necesaria, pero me sirve para acceder a los metodos del UC de crear
                    {
                        if (!Regex.IsMatch(ucc.DNI(), @"^\d{7,8}$"))
                        {
                            MessageBox.Show(textos["msg_DNIValido"]);
                            return;
                        }
                        if (!Regex.IsMatch(ucc.Nombre(), @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]{3,}$"))
                        {
                            MessageBox.Show(textos["msg_NombreValido"]);
                            return;
                        }
                        if (!Regex.IsMatch(ucc.Apellido(), @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]{3,}$"))
                        {
                            MessageBox.Show(textos["msg_ApellidoValido"]);
                            return;
                        }
                        if (!Regex.IsMatch(ucc.Email(), @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$"))
                        {
                            MessageBox.Show(textos["msg_EmailValido"]);
                            return;
                        }
                        try
                        {
                            Servicios_64PR.Usuario u = new Servicios_64PR.Usuario()
                            {
                                DNI = ucc.DNI(),
                                Apellido = ucc.Apellido(),
                                Nombre = ucc.Nombre(),
                                Login = ucc.Nombre() + "." + ucc.Apellido(),
                                Rol = new Servicios_64PR.Rol_64PR(),
                                Email = ucc.Email(),
                            };
                            u.Rol.Id = ucc.Rol();

                            ///Linea que me crea el usuario
                            gusuarios.Crear(u);

                            ///Registro el evento en bitacora
                            ev = new Evento_64PR(SessionManager.GetInstance.Usuario.Login, "2", "6", 4);
                            MessageBox.Show(textos["usuario_creado"] + u.Login);
                            bita.RegistrarEvento(ev);

                            CargaData();
                            ucc.LimpiarCampos();
                            pnlContenedor.Controls.Clear();
                            uc = null;
                            lblModo.Text = textos["frmGestionUsuarios_lblModoConsulta"]; ;
                            btnGuardar.Enabled = false;
                        }
                        catch (SqlException ex)
                        {
                            ///Cualquiera de los 2 numeros es para violacion de PK o UQ
                            if (ex.Number == 2627 || ex.Number == 2601)
                            {
                                if (ex.Message.Contains("PK__USUARIO"))
                                {
                                    MessageBox.Show(textos["msg_DNIDuplicado"],
                                                    "DNI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else if (ex.Message.Contains("UQ__USUARIO"))
                                {
                                    MessageBox.Show(textos["msg_EmailDuplicado"],
                                                    "Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                    }
                    break;
                case "modificar":
                    if (uc is ucModificarUsuario ucm) ///Esta validacion creo que no es necesaria, pero me sirve para acceder a los metodos del UC
                    {
                        if (!Regex.IsMatch(ucm.Email(), @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$"))
                        {
                            MessageBox.Show(textos["mail_invalido"]);
                            return;
                        }
                        try
                        {
                            ///Obtengo el usuario del DGV
                            Servicios_64PR.Usuario u = dgvUsuarios.SelectedRows[0].DataBoundItem as Servicios_64PR.Usuario;
                            u.Rol.Id = ucm.Rol();
                            u.Email = ucm.Email();

                            ///Linea que me modifica el usuario luego de asignarlo los nuevos valores
                            gusuarios.Modificar(u);

                            ///Registro el evento en bitacora
                            ev = new Evento_64PR(SessionManager.GetInstance.Usuario.Login, "2", "7", 4);
                            MessageBox.Show(textos["usuario_modificado"]);
                            bita.RegistrarEvento(ev);

                            CargaData();
                            ucm.LimpiarCampos();
                            pnlContenedor.Controls.Clear();
                            uc = null;
                            lblModo.Text = textos["frmGestionUsuarios_lblModoConsulta"];
                            btnGuardar.Enabled = false;
                        }
                        catch (SqlException ex)
                        {
                            ///Cualquiera de los 2 numeros es para violacion de PK o UQ
                            if (ex.Number == 2627 || ex.Number == 2601)
                            {
                                if (ex.Message.Contains("UQ__USUARIO")) ///Validacion no necesaria, ya que el unico campo que se puede actualizar que tiene UQ es el mail
                                {
                                    MessageBox.Show(textos["mail_duplicado"],
                                                    "Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void btnDesbloquear_Click(object sender, EventArgs e)
        {
            ///Obtengo el usuario del DGV
            Servicios_64PR.Usuario u = dgvUsuarios.SelectedRows[0].DataBoundItem as Servicios_64PR.Usuario;

            if (u.Bloqueado == true)
            {
                ///Linea que me desloquea el usuario
                gusuarios.Desbloquear(u);

                ///Registro el evento en bitacora
                ev = new Evento_64PR(SessionManager.GetInstance.Usuario.Login, "2", "8", 4);
                MessageBox.Show(textos["usuario_desbloqueado"]);
                bita.RegistrarEvento(ev);

                CargaData();
            }
            else
            {
                MessageBox.Show(textos["usuario_no_bloqueado"]);
            }
        }

        private void btnActDesact_Click(object sender, EventArgs e)
        {
            ///Obtengo el usuario del DGV
            Servicios_64PR.Usuario u = dgvUsuarios.SelectedRows[0].DataBoundItem as Servicios_64PR.Usuario;
            if (u.Login == SessionManager.GetInstance.Usuario.Login)
            {
                MessageBox.Show(textos["usuario_en_sesion"]);
                return;
            }

            ///Linea que me cambia el estado del usuario
            gusuarios.Actdesact(u);

            ev = new Evento_64PR(SessionManager.GetInstance.Usuario.Login, "2", u.Activo == true ? "9" : "10", 4);
             bita.RegistrarEvento(ev);
            MessageBox.Show(textos["operacion_exitosa"]);
            CargaData();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            modo = "modificar";
            lblModo.Text = textos["frmGestionUsuarios_lblModoModificar"];
            uc = new ucModificarUsuario();
            pnlContenedor.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            pnlContenedor.Controls.Add(uc);
            btnGuardar.Enabled = true;
        }

        private void dgvUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                ///Obtengo el usuario del DGV
                Usuario u = dgvUsuarios.SelectedRows[0].DataBoundItem as Servicios_64PR.Usuario;
                if (uc is ucModificarUsuario ucm) ///esta validacion creo que no es necesaria, pero me sirve para acceder a los metodos del UC
                {
                    ucm.EscribirControles(u);
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
            ///Activos
            if (radioButton1.Checked == true)
            {
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                dgvUsuarios.DataSource = null;
                dgvUsuarios.DataSource = lst.Where(u => u.Activo == true).ToList();
                lblCantidad.Text = textos["frmGestionUsuarios_lblCantidad"] + lst.Where(u => u.Activo == true).ToList().Count();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            ///No activos
            if (radioButton2.Checked == true)
            {
                radioButton1.Checked = false;
                radioButton3.Checked = false;
                dgvUsuarios.DataSource = null;
                dgvUsuarios.DataSource = lst.Where(u => u.Activo == false).ToList();
                lblCantidad.Text = textos["frmGestionUsuarios_lblCantidad"] + lst.Where(u => u.Activo == false).ToList().Count();

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
                if(lst != null)  ///Condicional necesario para que no ejecute esta linea de codigo durante la construccion del frm
                {
                    lblCantidad.Text = textos["frmGestionUsuarios_lblCantidad"] + lst.Count.ToString();
                }
            }
        }

        private void FrmGestionarUsuarios_64PR_Load(object sender, EventArgs e)
        {
            ConfigurarPermisos();
        }
        private void ConfigurarPermisos()
        {
            Servicios_64PR.Rol_64PR rolUsuario = Servicios_64PR.SessionManager.GetInstance.Usuario.Rol;

            btnCrear.Visible = rolUsuario.TienePermiso(Patentes_64PR.CrearUsuario);
            btnModificar.Visible = rolUsuario.TienePermiso(Patentes_64PR.ModificarUsuario);
            btnActDesact.Visible = rolUsuario.TienePermiso(Patentes_64PR.ActivarDesactivarUsuarios);
            btnDesbloquear.Visible = rolUsuario.TienePermiso(Patentes_64PR.DesbloquearUsuario);
        }

        /* private void dgvUsuarios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
         {
             ///Pongo en rojo los usuarios desactivados
             foreach (DataGridViewRow row in dgvUsuarios.Rows)
             {
                 if (!(bool)row.Cells["Activo"].Value)
                     row.DefaultCellStyle.BackColor = Color.LightCoral;
             }
         }*/

        private void FrmGestionarUsuarios_64PR_FormClosed(object sender, FormClosedEventArgs e)
        {
            GestorIdioma_64PR.GetInstance.Desuscribir(this); ///observer del cambio de idioma
        }
    }
}
