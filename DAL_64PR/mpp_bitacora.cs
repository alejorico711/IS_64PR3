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
    public class mpp_bitacora
    {
        public List<Evento_64PR> ListarEventos()
        {
            string query = "SELECT \r\n    e.Id_evento,\r\n    e.Login,\r\n    e.Fecha,\r\n    e.Hora,\r\n    m.Nombre AS Modulo,\r\n    t.Nombre AS TipoEvento,\r\n    e.criticidad\r\nFROM Evento_64PR e\r\n    INNER JOIN USUARIO_64PR u  ON e.Login     = u.Login\r\n    INNER JOIN Modulo_64PR  m  ON e.Id_modulo = m.Id_modulo\r\n    INNER JOIN TipoEvento_64PR t ON e.Id_Tipo = t.Id_tipo\r\nWHERE e.Fecha >= CAST(GETDATE() - 3 AS DATE)\r\nORDER BY e.Fecha DESC, e.Hora DESC;";
            DataTable tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, null);
            List<Evento_64PR> lista = new List<Evento_64PR>();
            foreach (DataRow dr in tabla.Rows)
            {
                Servicios_64PR.Evento_64PR e = new Servicios_64PR.Evento_64PR();
                e.Login = dr["Login"].ToString();
                DateTime fecha = Convert.ToDateTime(dr["Fecha"]);
                TimeSpan hora = (TimeSpan)dr["Hora"];
                e.FechaHora = fecha.Date + hora;
                e.Modulo = dr["Modulo"].ToString();
                e.Tipo = dr["TipoEvento"].ToString();
                e.Criticidad = Convert.ToInt16(dr["Criticidad"].ToString());
                lista.Add(e);
            }
            return lista;
        }

        public List<string> ListarLogins()
        {
            string query = "SELECT Login FROM USUARIO_64PR";
            DataTable tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, null);
            List<string> lista = new List<string>();
            foreach (DataRow dr in tabla.Rows)
            {
                lista.Add(dr["Login"].ToString());
            }
            return lista;
        }

        public List<string> ListarModulos()
        {
            string query = "SELECT Nombre FROM Modulo_64PR";
            DataTable tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, null);
            List<string> lista = new List<string>();
            foreach (DataRow dr in tabla.Rows)
            {
                lista.Add(dr["Nombre"].ToString());
            }
            return lista;
        }

        public List<string> ListarTipos()
        {
            string query = "SELECT Nombre FROM TipoEvento_64PR";
            DataTable tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, null);
            List<string> lista = new List<string>();
            foreach (DataRow dr in tabla.Rows)
            {
                lista.Add(dr["Nombre"].ToString());
            }
            return lista;
        }

        public void RegistrarEvento(Evento_64PR e)
        {
            int fa;
            string query = "INSERT INTO Evento_64PR (Login, Id_modulo, Id_tipo, Criticidad) VALUES (@Login, @Id_modulo, @Id_tipo, @Criticidad)";
            SqlParameter[] parametros = new SqlParameter[4];
            parametros[0] = new SqlParameter("@Login", e.Login);
            parametros[1] = new SqlParameter("@Id_modulo", Convert.ToInt16(e.Modulo));
            parametros[2] = new SqlParameter("@Id_tipo", Convert.ToInt16(e.Tipo));
            parametros[3] = new SqlParameter("@Criticidad", e.Criticidad);
            fa = DAL_64PR.Acceso.Instancia.escribirQuery(query, parametros);
        }
    }
}
