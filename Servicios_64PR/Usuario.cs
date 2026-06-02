using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios_64PR
{
    public class Usuario
    {

        private string _dni;

        public string DNI
        {
            get { return _dni; }
            set { _dni = value; }
        }

        private string _apelllido;

        public string Apellido
        {
            get { return _apelllido; }
            set { _apelllido = value; }
        }

        private string _nombre;

        public string Nombre
        {
            get { return _nombre; }
            set { _nombre = value; }
        }

        private string _login;

        public string Login
        {
            get { return _login; }
            set { _login = value; }
        }

        private string _rol;

        public string Rol
        {
            get { return _rol; }
            set { _rol = value; }
        }
        private string _email;

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        private bool _bloqueado;

        public bool Bloqueado
        {
            get { return _bloqueado; }
            set { _bloqueado = value; }
        }

        private bool _activo;

        public bool Activo
        {
            get { return _activo; }
            set { _activo = value; }
        }

        private bool _primeravez;

        public bool PrimeraVez
        {
            get { return _primeravez; }
            set { _primeravez = value; }
        }
    }
}
