using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_64PR
{
    public class Rol_64PR
    {
        DAL_64PR.mpp_roles mpp = new DAL_64PR.mpp_roles();
        private static readonly DV_64PR recalculador = new DV_64PR();
        public List<Servicios_64PR.Rol_64PR> ListarRoles()
        {
            return mpp.ListarRoles();
        }
        public void CrearRol(string nombre, List<Servicios_64PR.Rol_64PR> hijos)
        {
            mpp.CrearRol(nombre, hijos);
            recalculador.RecalcularTabla("Roles_64PR");
            recalculador.RecalcularTabla("RolFamilia_64PR");
            recalculador.RecalcularTabla("RolPatente_64PR");
        }

        public void ModificarRol(int id, string nombre, List<Servicios_64PR.Rol_64PR> hijos)
        {
            mpp.ModificarRol(id, nombre, hijos);
            recalculador.RecalcularTabla("Roles_64PR");
            recalculador.RecalcularTabla("RolFamilia_64PR");
            recalculador.RecalcularTabla("RolPatente_64PR");
        }

        public void EliminarRol(int id, bool reasignarUsuarios)
        {
            mpp.EliminarRol(id, reasignarUsuarios);
            recalculador.RecalcularTabla("Roles_64PR");
            recalculador.RecalcularTabla("RolFamilia_64PR");
            recalculador.RecalcularTabla("RolPatente_64PR");
            recalculador.RecalcularTabla("USUARIO_64PR");
        }

        public int ContarUsuariosConRol(int idRol)
        {
            return mpp.ContarUsuariosConRol(idRol);
        }

        public Servicios_64PR.Rol_64PR ObtenerRolCompleto(int idRol)
        {
            return mpp.ObtenerRolCompleto(idRol);
        }
    }
}
