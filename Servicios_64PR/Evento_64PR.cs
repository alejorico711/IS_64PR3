using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios_64PR
{
    public class Evento_64PR
    {

        private string _login;

        public string Login
        {
            get { return _login; }
            set { _login = value; }
        }
        private DateTime _fechaHora;

        public DateTime FechaHora
        {
            get { return _fechaHora; }
            set { _fechaHora = value; }
        }
        private string _modulo;

        public string Modulo
        {
            get { return _modulo; }
            set { _modulo = value; }
        }

        private string _tipo;

        public string Tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }

        private int _criticidad;

        public int Criticidad
        {
            get { return _criticidad; }
            set { _criticidad = value; }
        }
        public Evento_64PR(string l, string m, string t, int c)
        {
            Login = l;
            Modulo = m;
            Tipo = t;
            Criticidad = c;
        }

        public Evento_64PR()
        {
        }
    }
}
