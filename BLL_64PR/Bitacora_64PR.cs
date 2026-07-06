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
        public enum TipoEventoBitacora_64PR
        {
            LoginExitoso = 1,
            CambioClave = 2,
            LoginFallido = 3,
            UsuarioBloqueado = 4,
            Logout = 5,
            AltaUsuario = 6,
            ModificacionUsuario = 7,
            CreacionRol = 9,
            EliminacionRol = 10,
            ModificacionRol = 11,
            CreacionFamilia = 12,
            EliminacionFamilia = 13,
            ModificacionFamilia = 14,
            Backup = 15,
            Restore=16
        }
        public enum ModuloBitacora_64PR
        {
            Login = 1,
            GestionUsuarios = 2,
            GestionRoles = 3,
            GestionFamilias = 4
        }

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
