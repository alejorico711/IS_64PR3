using Servicios_64PR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_64PR
{
    public class mpp_usuario
    {
        public void Actdesact(Servicios_64PR.Usuario u)
        {
            string query = "UPDATE USUARIO_64PR SET Activo = Activo^1 WHERE DNI = @DNI";
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@DNI", u.DNI);
            int fa = DAL_64PR.Acceso.Instancia.escribirQuery(query, parametros);
        }

        public bool BloqueadoInactivo(string login)
        {
            string query = "SELECT CASE WHEN Bloqueo = 'True' OR Activo = 'False' THEN 'True' ELSE 'False' END AS EsInvalido FROM USUARIO_64PR WHERE Login = @Login";
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@Login", login);
            DataTable tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, parametros);
            if (tabla != null && tabla.Rows.Count > 0)
            {
                return Convert.ToBoolean(tabla.Rows[0][0]);
            }
            return false;
        }

        public void Crear(Servicios_64PR.Usuario u)
        {
            try
            {
                int fa;
                string query = "SELECT COUNT(*) AS total FROM USUARIO_64PR WHERE Login LIKE @Login";
                SqlParameter[] parametros2 = new SqlParameter[1];
                parametros2[0] = new SqlParameter("@Login", u.Login + "%");
                fa = Convert.ToInt32(DAL_64PR.Acceso.Instancia.leerEscalar(query, parametros2));

                query = "INSERT INTO USUARIO_64PR (DNI, Apellido, Nombre, Login, Rol, Email, Contraseña) VALUES (@DNI, @Apellido, @Nombre, @Login, @Rol, @Email, @Contraseña)";
                SqlParameter[] parametros = new SqlParameter[7];
                parametros[0] = new SqlParameter("@DNI", u.DNI);
                parametros[1] = new SqlParameter("@Apellido", u.Apellido);
                parametros[2] = new SqlParameter("@Nombre", u.Nombre);
                if (fa == 0)
                {
                    parametros[3] = new SqlParameter("@Login", u.Login);
                }
                else
                {
                    parametros[3] = new SqlParameter("@Login", (u.Login + (fa + 1).ToString())); //el +1 es necesario para que la primera vez que ocurra la repeticion no use el numero 1
                }
                parametros[4] = new SqlParameter("@Rol", u.Rol.Id);
                parametros[5] = new SqlParameter("@Email", Encriptación.Instancia.EncriptarAESBase64(u.Email));
                parametros[6] = new SqlParameter("@Contraseña", Encriptación.Instancia.Encriptar(u.Apellido + u.DNI));
                fa = DAL_64PR.Acceso.Instancia.escribirQuery(query, parametros);
            }
            catch
            {
                throw;
            }
        }

        public void Desbloquear(Servicios_64PR.Usuario u)
        {
            string query = "UPDATE USUARIO_64PR SET Bloqueo = 0, Intentos = 0, PrimeraVez = 1, Contraseña = @Contraseña WHERE DNI = @DNI";
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@DNI", u.DNI);
            parametros[1] = new SqlParameter("@Contraseña", Encriptación.Instancia.Encriptar(u.Apellido + u.DNI));
            int fa = DAL_64PR.Acceso.Instancia.escribirQuery(query, parametros);
        }

        public List<Servicios_64PR.Usuario> Listar()
        {
            List<Servicios_64PR.Usuario> lista = new List<Servicios_64PR.Usuario>();

            string query = "SELECT \r\n    U.[DNI],\r\n    U.[Apellido],\r\n    U.[Nombre],\r\n    U.[Login],\r\n    U.[Email],\r\n    R.[Nombre] AS [Rol], \r\n    U.[Bloqueo],\r\n    U.[Activo],\r\n    U.[Idioma]\r\n,\r\n    U.[PrimeraVez]\r\nFROM \r\n    [dbo].[USUARIO_64PR] U\r\nINNER JOIN \r\n    [dbo].[Roles_64PR] R ON U.[Rol] = R.[ID_Rol];";
            DataTable tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, null);

            foreach (DataRow dr in tabla.Rows)
            {
                Servicios_64PR.Usuario u = new Servicios_64PR.Usuario();
                u.DNI = dr["DNI"].ToString();
                u.Nombre = dr["Nombre"].ToString();
                u.Apellido = dr["Apellido"].ToString();
                u.Login = dr["Login"].ToString();
                Servicios_64PR.Rol_64PR r = new Servicios_64PR.Rol_64PR();
                u.Rol = r;
                u.Rol.Nombre = dr["Rol"].ToString();
                u.Email = Encriptación.Instancia.DesencriptarAESBase64(dr["Email"].ToString());
                u.Activo = bool.Parse(dr["Activo"].ToString());
                u.Bloqueado = bool.Parse(dr["Bloqueo"].ToString());
                u.PrimeraVez = bool.Parse(dr["PrimeraVez"].ToString());
                lista.Add(u);
            }
            return lista;
        }

        public void Modificar(Servicios_64PR.Usuario u)
        {
            try
            {
                string query = "UPDATE USUARIO_64PR SET Rol = @Rol, Email = @Email WHERE DNI = @DNI";
                SqlParameter[] parametros = new SqlParameter[3];
                parametros[0] = new SqlParameter("@DNI", u.DNI);
                parametros[1] = new SqlParameter("@Rol", u.Rol.Id);
                parametros[2] = new SqlParameter("@Email", Encriptación.Instancia.EncriptarAESBase64(u.Email));
                int fa = DAL_64PR.Acceso.Instancia.escribirQuery(query, parametros);
            }
            catch
            {
                throw;
            }
        }

        public bool VerificarClave(string login, string contra)
        {
            byte[] hashalmacenado = ObtenerHashAlmacenado(login);
            if (Servicios_64PR.Encriptación.Instancia.VerifyPassword(contra, hashalmacenado))
            {
                return true;
            }
            return false;
        }

        public byte[] ObtenerHashAlmacenado(string login)
        {
            string query = "SELECT Contraseña FROM USUARIO_64PR WHERE Login = @Login";
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@Login", login);
            DataTable tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, parametros);
            return (byte[])tabla.Rows[0]["Contraseña"];
        }

        public void SumarIntento(string login)
        {
            string query = " UPDATE USUARIO_64PR SET Intentos = Intentos + 1, Bloqueo = CASE WHEN Intentos + 1 >= 3 THEN 'True' ELSE Bloqueo END WHERE Login = @Login";
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@Login", login);
            int fa = DAL_64PR.Acceso.Instancia.escribirQuery(query, parametros);
        }

        public bool ExisteUsuario(string login)
        {
            string query = "SELECT CASE WHEN EXISTS (SELECT 1 FROM USUARIO_64PR WHERE Login = @Login) THEN 1 ELSE 0 END AS Existe";
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@Login", login);
            DataTable tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, parametros);
            if (tabla != null && tabla.Rows.Count > 0)
            {
                return Convert.ToBoolean(tabla.Rows[0][0]);
            }
            return false;
        }

        public void ReiniciarIntentos(string login)
        {
            string query = " UPDATE USUARIO_64PR SET Intentos = 0 WHERE Login = @Login";
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@Login", login);
            int fa = DAL_64PR.Acceso.Instancia.escribirQuery(query, parametros);
        }

        public Servicios_64PR.Usuario ObtenerUsuario(string login)
        {
            string query = "SELECT * FROM USUARIO_64PR WHERE Login = @Login";
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@Login", login);
            DataTable tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, parametros);
            Servicios_64PR.Usuario u = new Servicios_64PR.Usuario();

            foreach (DataRow dr in tabla.Rows)
            {
                u.DNI = dr["DNI"].ToString();
                u.Nombre = dr["Nombre"].ToString();
                u.Apellido = dr["Apellido"].ToString();
                u.Login = dr["Login"].ToString();
                u.Rol = u.Rol = ObtenerRolCompleto(int.Parse(dr["Rol"].ToString()));
                u.Email = dr["Email"].ToString();
                u.Activo = bool.Parse(dr["Activo"].ToString());
                u.Bloqueado = bool.Parse(dr["Bloqueo"].ToString());
                u.PrimeraVez = bool.Parse(dr["PrimeraVez"].ToString());
            }
            return u;
        }

        private Servicios_64PR.Rol_64PR ObtenerRolCompleto(int idRol)
        {
            ///Funcion un tanto compleja para obtener el arbol completo de permisos del usuario al momemento del login
            /// 1. Datos básicos del rol
            string query = "SELECT ID_Rol, Nombre FROM Roles_64PR WHERE ID_Rol = @IdRol";
            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("@IdRol", idRol) };
            DataTable tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, parametros);

            Servicios_64PR.Rol_64PR rol = new Servicios_64PR.Rol_64PR();
            rol.Id = Convert.ToInt32(tabla.Rows[0]["ID_Rol"]);
            rol.Nombre = tabla.Rows[0]["Nombre"].ToString();

            /// 2. Patentes directas del rol
            query = @"SELECT P.ID_Patente, P.Nombre 
              FROM Patente_64PR P
              INNER JOIN RolPatente_64PR RP ON P.ID_Patente = RP.ID_Patente
              WHERE RP.ID_Rol = @IdRol";
            parametros = new SqlParameter[] { new SqlParameter("@IdRol", idRol) };
            tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, parametros);

            foreach (DataRow dr in tabla.Rows)
            {
                Servicios_64PR.Permiso_64PR p = new Servicios_64PR.Permiso_64PR();
                p.Id = Convert.ToInt32(dr["ID_Patente"]);
                p.Nombre = dr["Nombre"].ToString();
                rol.Agregar(p);
            }

            /// 3. Familias del rol (con sus patentes adentro)
            query = @"SELECT F.ID_Familia, F.Nombre 
              FROM Familia_64PR F
              INNER JOIN RolFamilia_64PR RF ON F.ID_Familia = RF.ID_Familia
              WHERE RF.ID_Rol = @IdRol";
            parametros = new SqlParameter[] { new SqlParameter("@IdRol", idRol) };
            tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, parametros);

            foreach (DataRow dr in tabla.Rows)
            {
                Servicios_64PR.Familia_64PR f = new Servicios_64PR.Familia_64PR();
                f.Id = Convert.ToInt32(dr["ID_Familia"]);
                f.Nombre = dr["Nombre"].ToString();
                CargarHijosFamilia(f); /// carga las patentes y subfamilias de esta familia
                rol.Agregar(f);
            }

            return rol;
        }

        private void CargarHijosFamilia(Servicios_64PR.Familia_64PR familia)
        {
            /// Patentes de esta familia
            string query = @"SELECT P.ID_Patente, P.Nombre 
                     FROM Patente_64PR P
                     INNER JOIN PatenteFamilia_64PR PF ON P.ID_Patente = PF.ID_Patente
                     WHERE PF.ID_Familia = @IdFamilia";
            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("@IdFamilia", familia.Id) };
            DataTable tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, parametros);

            foreach (DataRow dr in tabla.Rows)
            {
                Servicios_64PR.Permiso_64PR p = new Servicios_64PR.Permiso_64PR();
                p.Id = Convert.ToInt32(dr["ID_Patente"]);
                p.Nombre = dr["Nombre"].ToString();
                familia.Agregar(p);
            }

            /// Subfamilias de esta familia (tabla Familia_N)
            query = @"SELECT F.ID_Familia, F.Nombre 
              FROM Familia_64PR F
              INNER JOIN Familia_N_64PR FN ON F.ID_Familia = FN.ID_FamiliaHija
              WHERE FN.ID_FamiliaPadre = @IdFamilia";
            parametros = new SqlParameter[] { new SqlParameter("@IdFamilia", familia.Id) };
            tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, parametros);

            foreach (DataRow dr in tabla.Rows)
            {
                Servicios_64PR.Familia_64PR sub = new Servicios_64PR.Familia_64PR();
                sub.Id = Convert.ToInt32(dr["ID_Familia"]);
                sub.Nombre = dr["Nombre"].ToString();
                CargarHijosFamilia(sub); /// recursivo para subfamilias de subfamilias
                familia.Agregar(sub);
            }
        }

        public void CambiarClave(string nueva, string confirmacion)
        {
            if (string.IsNullOrWhiteSpace(nueva))
                throw new Exception("La nueva contraseña no puede estar vacia.");
            if (nueva != confirmacion)
                throw new Exception("La nueva contraseña y la confirmacion no coinciden.");
            if (nueva.Length < 8)
                throw new Exception("La nueva contrasenia debe tener al menos 8 caracteres.");

            string query = "UPDATE USUARIO_64PR SET Contraseña = @Contraseña, PrimeraVez = 0 WHERE DNI = @DNI";
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@DNI", SessionManager.GetInstance.Usuario.DNI);
            parametros[1] = new SqlParameter("@Contraseña", Encriptación.Instancia.Encriptar(nueva));
            int fa = DAL_64PR.Acceso.Instancia.escribirQuery(query, parametros);
        }

        public string ObtenerIntentos(string login)
        {
            string query = "SELECT Intentos FROM USUARIO_64PR WHERE Login = @Login";
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@Login", login);
            DataTable tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, parametros);
            return Convert.ToInt16(tabla.Rows[0][0]).ToString();
        }
        public string ObtenerIdioma(string login)
        {
            string query = "SELECT Idioma FROM USUARIO_64PR WHERE Login = @Login";
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter("@Login", login);
            DataTable tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, parametros);

            if (tabla != null && tabla.Rows.Count > 0)
                return tabla.Rows[0]["Idioma"].ToString();

            return "es"; // valor por defecto si no hay dato
        }

        public void GuardarIdioma(string login, string idioma)
        {
            string query = "UPDATE USUARIO_64PR SET Idioma = @Idioma WHERE Login = @Login";
            SqlParameter[] parametros = new SqlParameter[2];
            parametros[0] = new SqlParameter("@Login", login);
            parametros[1] = new SqlParameter("@Idioma", idioma);
            DAL_64PR.Acceso.Instancia.escribirQuery(query, parametros);
        }
    }
}
