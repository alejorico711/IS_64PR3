using Servicios_64PR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_64PR
{
    public class Bitacora_64PR
    {
        DAL_64PR.mpp_bitacora mpp = new DAL_64PR.mpp_bitacora();
        public List<Evento_64PR> ListarEventos()
        {
            return mpp.ListarEventos();
        }

        public List<string> ListarLogins()
        {
            return mpp.ListarLogins();
        }

        public List<string> ListarModulos()
        {
            return mpp.ListarModulos();
        }

        public List<string> ListarTipos()
        {
            return mpp.ListarTipos();
        }

        public void RegistrarEvento(Evento_64PR e)
        {
            mpp.RegistrarEvento(e);
        }
    }
}
