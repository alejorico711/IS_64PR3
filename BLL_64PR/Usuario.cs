using Servicios_64PR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_64PR
{
    public class Usuario
    {
        DAL_64PR.mpp_usuario mpp = new DAL_64PR.mpp_usuario();
        public void Actdesact(Servicios_64PR.Usuario u)
        {
            mpp.Actdesact(u);
        }

        public bool BloqueadoInactivo(string login)
        {
            return mpp.BloqueadoInactivo(login);
        }

        public void Crear(Servicios_64PR.Usuario u)
        {
            mpp.Crear(u);
        }

        public void Desbloquear(Servicios_64PR.Usuario u)
        {
            mpp.Desbloquear(u);
        }

        public List<Servicios_64PR.Usuario> Listar()
        {
            return mpp.Listar();
        }

        public void Modificar(Servicios_64PR.Usuario u)
        {
            mpp.Modificar(u);
        }

        public bool VerificarClave(string login, string contra)
        {
            return mpp.VerificarClave(login, contra);
        }

        public byte[] ObtenerHashAlmacenado(string login)
        {
            return mpp.ObtenerHashAlmacenado(login);
        }

        public void SumarIntento(string login)
        {
            mpp.SumarIntento(login);
        }

        public bool ExisteUsuario(string login)
        {
            return mpp.ExisteUsuario(login);
        }

        public void ReiniciarIntentos(string login)
        {
            mpp.ReiniciarIntentos(login);
        }

        public Servicios_64PR.Usuario ObtenerUsuario(string login)
        {
            return mpp.ObtenerUsuario(login);
        }
        public void CambiarClave(string nueva, string confirmacion)
        {
            mpp.CambiarClave(nueva, confirmacion);
        }

        public string ObtenerIntentos(string login)
        {
            return mpp.ObtenerIntentos(login);
        }
    }
}