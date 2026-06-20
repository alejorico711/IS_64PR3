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
    public partial class FrmGestionFamilias_64PR : Form,IObservadorIdioma_64PR
    {
        BLL_64PR.Rol_64PR bllRol = new BLL_64PR.Rol_64PR();
        List<Servicios_64PR.Rol_64PR> nodos = new List<Servicios_64PR.Rol_64PR>();
        List<Servicios_64PR.Rol_64PR> nodos2 = new List<Servicios_64PR.Rol_64PR>();
        Dictionary<string, string> textos;

        private enum Modo { Crear, Modificar, Eliminar }
        private Modo modoActual = Modo.Crear;
        private int idFamiliaEnEdicion = -1; /// guarda el ID cuando estás modificando

        public FrmGestionFamilias_64PR()
        {
            InitializeComponent();
            CargaPermisosYFamilias();

            btnEliminar.Hide();
            btnAgregar.Hide();
            btnQuitar.Hide();
            btnAplicar.Hide();
            label1.Visible = false;
            txtNombre.Visible = false;
            treeView2.Visible = false;

            GestorIdioma_64PR.GetInstance.Suscribir(this); ///observer del cambio de idioma
                                                           ///Aplico el idioma que ya está cargado
            textos = GestorIdioma_64PR.GetInstance.ObtenerTextos();
            if (textos.Count > 0)
                ActualizarIdioma(textos);
        }
        public void ActualizarIdioma(Dictionary<string, string> textoss)
        {
            textos = textoss;
            rbCrear.Text = textos["crear"];
            rbModificar.Text = textos["modificar"];
            rbEliminar.Text = textos["eliminar"];

            btnAgregar.Text = textos["agregar"] + "-->";
            btnQuitar.Text = textos["quitar"] + "<--";
            btnAplicar.Text = textos["aplicar"];
            btnEliminar.Text = textos["eliminar"];

            label1.Text = textos["nombre"];
        }

        private void CargaPermisosYFamilias()
        {
            treeView1.Nodes.Clear();
            nodos = bllRol.ObtenerTodosLosNodos();

            foreach (var nodo in nodos)
            {
                TreeNode tn = CrearNodoVisual(nodo);
                treeView1.Nodes.Add(tn);
            }

            treeView1.ExpandAll();
        }

        private void rbCrear_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbCrear.Checked) return;
            modoActual = Modo.Crear;
            txtNombre.Clear();
            txtNombre.Enabled = true;
            treeView2.Nodes.Clear();
            nodos2.Clear();
            btnAplicar.Visible = true;
            idFamiliaEnEdicion = -1;
            btnEliminar.Hide();
            btnAgregar.Show();
            btnQuitar.Show();
            btnAplicar.Show();
            label1.Visible = true;
            txtNombre.Visible = true;
            treeView2.Visible = true;
        }

        private void rbModificar_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbModificar.Checked) return;
            modoActual = Modo.Modificar;
            txtNombre.Text = string.Empty;
            txtNombre.Enabled = true;
            treeView2.Nodes.Clear();
            nodos2.Clear();
            btnAplicar.Visible = true;
            idFamiliaEnEdicion = -1;
            btnEliminar.Hide();
            btnAgregar.Show();
            btnQuitar.Show();
            btnAplicar.Show();
            label1.Visible = true;
            txtNombre.Visible = true;
            treeView2.Visible = true;
        }

        private void rbEliminar_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbEliminar.Checked) return;
            modoActual = Modo.Eliminar;
            txtNombre.Text= string.Empty;
            txtNombre.Enabled = false;
            treeView2.Nodes.Clear();
            nodos2.Clear();
            btnAplicar.Visible = false;
            btnEliminar.Show();
            btnAgregar.Hide();
            btnQuitar.Hide();
            btnAplicar.Hide();
            label1.Visible = false;
            txtNombre.Visible = false;
            treeView2.Visible = false;
        }


        private TreeNode CrearNodoVisual(Servicios_64PR.Rol_64PR rol)
        {
            TreeNode tn = new TreeNode(rol.Nombre);
            tn.Tag = rol; /// guardamos el objeto para usarlo después

            foreach (var hijo in rol.Hijos)
            {
                tn.Nodes.Add(CrearNodoVisual(hijo)); /// recursivo
            }

            return tn;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null) return;

            Servicios_64PR.Rol_64PR nodoSeleccionado = (Servicios_64PR.Rol_64PR)treeView1.SelectedNode.Tag;

            /// Verificar duplicados y conflictos contra todo el TreeView2
            foreach (TreeNode tn in treeView2.Nodes)
            {
                Servicios_64PR.Rol_64PR nodoExistente = (Servicios_64PR.Rol_64PR)tn.Tag;

                /// Mismo nodo exacto
                if (nodoExistente.GetType() == nodoSeleccionado.GetType() &&
                    nodoExistente.Id == nodoSeleccionado.Id)
                {
                    MessageBox.Show(textos["ya_agregado"]);
                    return;
                }

                /// El seleccionado ya está como hijo de algo en TreeView2
                if (EsHijo(nodoExistente, nodoSeleccionado))
                {
                    MessageBox.Show($"'{nodoSeleccionado.Nombre}'" + textos["ya_incluido"] + $"'{nodoExistente.Nombre}'.");
                    return;
                }

                /// El seleccionado es una familia que ya contiene algo del TreeView2
                if (EsHijo(nodoSeleccionado, nodoExistente))
                {
                    MessageBox.Show($"'{nodoExistente.Nombre}'" + textos["ya_incluido"] + $"'{nodoSeleccionado.Nombre}'.");
                    return;
                }
            }
            nodos2.Add(nodoSeleccionado);
            treeView2.Nodes.Add(CrearNodoVisual(nodoSeleccionado));
            treeView2.ExpandAll();
        }

        private bool EsHijo(Servicios_64PR.Rol_64PR padre, Servicios_64PR.Rol_64PR buscado)
        {
            foreach (var hijo in padre.Hijos)
            {
                if (hijo.GetType() == buscado.GetType() && hijo.Id == buscado.Id)
                    return true;

                if (EsHijo(hijo, buscado)) /// recursivo para subfamilias
                    return true;
            }
            return false;
        }

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            if (treeView2.SelectedNode == null) return;

            /// Solo permitir quitar nodos raíz, no hijos. Ya que sino estariamos hablando de una familia distinta
            if (treeView2.SelectedNode.Parent != null)
            {
                MessageBox.Show(textos["solo_quitar_raiz"]);
                return;
            }
            Servicios_64PR.Rol_64PR nodoSeleccionado = (Servicios_64PR.Rol_64PR)treeView2.SelectedNode.Tag;
            nodos2.Remove(nodoSeleccionado);
            treeView2.Nodes.Remove(treeView2.SelectedNode);
        }

        private void btnAplicar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show(textos["nombre_para_familia"]);
                return;
            }
            if (treeView2.Nodes.Count == 0)
            {
                MessageBox.Show(textos["almenos_un_elemento"]);
                return;
            }

            List<Servicios_64PR.Rol_64PR> hijos = new List<Servicios_64PR.Rol_64PR>();
            foreach (TreeNode tn in treeView2.Nodes)
                hijos.Add((Servicios_64PR.Rol_64PR)tn.Tag);

            try
            {
                if (modoActual == Modo.Crear)
                {
                    bllRol.CrearFamilia(txtNombre.Text.Trim(), hijos);
                    MessageBox.Show(textos["familia_creada"]);
                }
                else if (modoActual == Modo.Modificar)
                {
                    if (idFamiliaEnEdicion == -1)
                    {
                        MessageBox.Show(textos["seleccionar_familia"]);
                        return;
                    }
                    bllRol.ModificarFamilia(idFamiliaEnEdicion, txtNombre.Text.Trim(), hijos);
                    MessageBox.Show(textos["familia_modificada"]);
                }

                CargaPermisosYFamilias();
                treeView2.Nodes.Clear();
                nodos2.Clear();
                txtNombre.Text = string.Empty;
                idFamiliaEnEdicion = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null) return;

            Servicios_64PR.Rol_64PR nodoSeleccionado = (Servicios_64PR.Rol_64PR)treeView1.SelectedNode.Tag;

            if (!(nodoSeleccionado is Servicios_64PR.Familia_64PR))
            {
                MessageBox.Show(textos["solo_eliminar_familias"]);
                return;
            }

            DialogResult confirm = MessageBox.Show(textos["pregunta_eliminacion_familias"] +" "+ nodoSeleccionado.Nombre + "?",
                textos["confirmar_eliminacion"],
                MessageBoxButtons.YesNo);

            if (confirm != DialogResult.Yes) return;

            try
            {
                bllRol.EliminarFamilia(nodoSeleccionado.Id);
                MessageBox.Show(textos["familia_eliminada"]);
                CargaPermisosYFamilias();
                treeView2.Nodes.Clear();
                nodos2.Clear();
                idFamiliaEnEdicion = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (modoActual != Modo.Modificar) return;

            Servicios_64PR.Rol_64PR nodoSeleccionado = (Servicios_64PR.Rol_64PR)e.Node.Tag;

            /// aca si es una patente, no tocamos nada, el usuario la va a agregar con btnAgregar
            if (nodoSeleccionado is Servicios_64PR.Permiso_64PR) return;

            /// pero si es una familia, la cargamos para editar
            idFamiliaEnEdicion = nodoSeleccionado.Id;
            txtNombre.Text = nodoSeleccionado.Nombre;
            treeView2.Nodes.Clear();
            nodos2.Clear();

            foreach (var hijo in nodoSeleccionado.Hijos)
            {
                nodos2.Add(hijo);
                treeView2.Nodes.Add(CrearNodoVisual(hijo));
            }
            treeView2.ExpandAll();
        }

        private void FrmGestionFamilias_64PR_FormClosed(object sender, FormClosedEventArgs e)
        {
            GestorIdioma_64PR.GetInstance.Desuscribir(this); ///observer del cambio de idioma
        }

        private void FrmGestionFamilias_64PR_Load(object sender, EventArgs e)
        {
            ConfigurarPermisos();
        }

        private void ConfigurarPermisos()
        {
            Servicios_64PR.Rol_64PR rolUsuario = Servicios_64PR.SessionManager.GetInstance.Usuario.Rol;

            rbCrear.Visible = rolUsuario.TienePermiso(Patentes_64PR.CrearFamilias);
            rbModificar.Visible = rolUsuario.TienePermiso(Patentes_64PR.ModificarFamilias);
            rbEliminar.Visible = rolUsuario.TienePermiso(Patentes_64PR.EliminarFamilias);
        }
    }
}
