using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_64PR
{
    public class Familia_64PR
    {
        DAL_64PR.mpp_roles mpp = new DAL_64PR.mpp_roles();
        private static readonly DV_64PR recalculador = new DV_64PR();
        public void CrearFamilia(string nombre, List<Servicios_64PR.Rol_64PR> hijos)
        {
            mpp.CrearFamilia(nombre, hijos);
            recalculador.RecalcularTabla("Familia_64PR");
            recalculador.RecalcularTabla("Familia_N_64PR");
            recalculador.RecalcularTabla("PatenteFamilia_64PR");
        }

        public void EliminarFamilia(int id)
        {
            mpp.EliminarFamilia(id);
            recalculador.RecalcularTabla("Familia_64PR");
            recalculador.RecalcularTabla("Familia_N_64PR");
            recalculador.RecalcularTabla("PatenteFamilia_64PR");
            recalculador.RecalcularTabla("RolFamilia_64PR");
        }
        public void ModificarFamilia(int id, string nombre, List<Servicios_64PR.Rol_64PR> hijos)
        {
            mpp.ModificarFamilia(id, nombre, hijos);
            recalculador.RecalcularTabla("Familia_64PR");
            recalculador.RecalcularTabla("Familia_N_64PR");
            recalculador.RecalcularTabla("PatenteFamilia_64PR");
        }
    }
}
