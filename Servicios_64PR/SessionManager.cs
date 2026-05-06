using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios_64PR
{
    public class SessionManager
    {
        private static SessionManager _instancia;
        private Servicios_64PR.Usuario _usuario = null;
        public Servicios_64PR.Usuario Usuario => _usuario;

        public DateTime FechaInicio;
        private SessionManager()  //ctor privado
        {
            
        }
        public static SessionManager GetInstance
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new SessionManager();
                }

                return _instancia;
            }
        }
        public void Login(Servicios_64PR.Usuario usuario)
        {
            if (Usuario != null)
            {
                throw new Exception("Sesión ya iniciada");
            }

            _usuario = usuario;
            FechaInicio = DateTime.Now;
        }

        public void Logout()
        {
            if (_instancia == null || _instancia._usuario == null)
            {
                throw new Exception("Sesión no iniciada");
            }

            _instancia._usuario = null;
        }
    }
}
