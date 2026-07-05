using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_64PR
{
    public class mpp_backup
    {
        public void CrearCarpetaSiNoExiste(string ruta)
        {
            string query = "EXEC master.dbo.xp_create_subdir @ruta";
            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("@ruta", ruta) };
            DAL_64PR.Acceso.Instancia.escribirQuery(query, parametros);
        }

        public void GenerarBackup(string rutaCompleta)
        {
            string query = @"BACKUP DATABASE BD_64PR TO DISK = @ruta WITH INIT, STATS = 10";
            SqlParameter[] parametros = new SqlParameter[]
            {
            new SqlParameter("@ruta", rutaCompleta)
            };
            DAL_64PR.Acceso.Instancia.EjecutarComandoSinTransaccion(query, parametros, 600);
        }
        public DataTable ObtenerFileList(string rutaBackup)
        {
            string query = "RESTORE FILELISTONLY FROM DISK = @ruta";
            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("@ruta", rutaBackup) };
            return DAL_64PR.Acceso.Instancia.leerQuery(query, parametros);
        }

        public string ObtenerRutaDefaultData()
        {
            string query = "SELECT SERVERPROPERTY('InstanceDefaultDataPath') AS Ruta";
            object resultado = DAL_64PR.Acceso.Instancia.leerEscalar(query, null);
            return resultado?.ToString();
        }

        public void RestaurarBackup(string rutaBackup, string logicalData, string logicalLog,
                             string rutaDestinoMdf, string rutaDestinoLdf)
        {
            string query = $@"
        ALTER DATABASE BD_64PR SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
        RESTORE DATABASE BD_64PR FROM DISK = @ruta
        WITH MOVE '{logicalData}' TO '{rutaDestinoMdf}',
             MOVE '{logicalLog}' TO '{rutaDestinoLdf}',
             REPLACE, STATS = 10;
        ALTER DATABASE BD_64PR SET MULTI_USER;";

            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("@ruta", rutaBackup) };
            DAL_64PR.Acceso.Instancia.EjecutarComandoMaster(query, parametros, 600);
        }
    }
}
