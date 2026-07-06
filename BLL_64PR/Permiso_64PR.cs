using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_64PR
{
    public class Permiso_64PR
    {
        DAL_64PR.mpp_roles mpp = new DAL_64PR.mpp_roles();
        private static readonly DV_64PR recalculador = new DV_64PR();
        public List<Servicios_64PR.Rol_64PR> ObtenerTodosLosNodos()
        {
            return mpp.ObtenerTodosLosNodos();
        }
    }
}
