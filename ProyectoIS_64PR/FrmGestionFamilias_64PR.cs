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
    public partial class FrmGestionFamilias_64PR : Form
    {
        BLL_64PR.Rol_64PR bllRol = new BLL_64PR.Rol_64PR();
        List<Servicios_64PR.Rol_64PR> nodos = new List<Servicios_64PR.Rol_64PR>();
        List<Servicios_64PR.Rol_64PR> nodos2 = new List<Servicios_64PR.Rol_64PR>();

        public FrmGestionFamilias_64PR()
        {
            InitializeComponent();
            CargaPermisosYFamilias();
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
                    MessageBox.Show("Ya fue agregado.");
                    return;
                }

                /// El seleccionado ya está como hijo de algo en TreeView2
                if (EsHijo(nodoExistente, nodoSeleccionado))
                {
                    MessageBox.Show($"'{nodoSeleccionado.Nombre}' ya está incluido dentro de '{nodoExistente.Nombre}'.");
                    return;
                }

                /// El seleccionado es una familia que ya contiene algo del TreeView2
                if (EsHijo(nodoSeleccionado, nodoExistente))
                {
                    MessageBox.Show($"'{nodoExistente.Nombre}' ya está incluido dentro de '{nodoSeleccionado.Nombre}'.");
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
                MessageBox.Show("Solo podés quitar elementos raíz. Para quitar una patente específica, quitá la familia completa.");
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
                MessageBox.Show("Ingresá un nombre para la familia.");
                return;
            }

            if (treeView2.Nodes.Count == 0)
            {
                MessageBox.Show("Agregá al menos un elemento a la familia.");
                return;
            }

            List<Servicios_64PR.Rol_64PR> hijos = new List<Servicios_64PR.Rol_64PR>();
            foreach (TreeNode tn in treeView2.Nodes)
            {
                hijos.Add((Servicios_64PR.Rol_64PR)tn.Tag);
            }

            try
            {
                bllRol.CrearFamilia(txtNombre.Text.Trim(), hijos);
                MessageBox.Show("Familia creada correctamente.");
                CargaPermisosYFamilias();
                treeView2.Nodes.Clear();
                txtNombre.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear la familia: " + ex.Message);
            }
        }
    }
}
