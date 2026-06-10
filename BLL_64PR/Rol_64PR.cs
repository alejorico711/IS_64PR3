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

        public void CrearFamilia(string nombre, List<Servicios_64PR.Rol_64PR> hijos)
        {
            mpp.CrearFamilia(nombre, hijos);
        }

        public List<Servicios_64PR.Rol_64PR> ListarRoles()
        {
            return mpp.ListarRoles();
        }
        public List<Servicios_64PR.Rol_64PR> ObtenerTodosLosNodos()
        {
            return mpp.ObtenerTodosLosNodos();
        }
    }
}
