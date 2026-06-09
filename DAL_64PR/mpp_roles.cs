using Servicios_64PR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_64PR
{
    public class mpp_roles
    {
        public List<Rol_64PR> ListarRoles()
        {
            List<Servicios_64PR.Rol_64PR> lista = new List<Servicios_64PR.Rol_64PR>();

            string query = "SELECT * FROM Roles_64PR";
            DataTable tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, null);

            foreach (DataRow dr in tabla.Rows)
            {
                Servicios_64PR.Rol_64PR r = new Servicios_64PR.Rol_64PR();
                r.Id = int.Parse(dr["ID_Rol"].ToString());
                r.Nombre = dr["Nombre"].ToString();
                lista.Add(r);
            }
            return lista;
        }
    }
}
