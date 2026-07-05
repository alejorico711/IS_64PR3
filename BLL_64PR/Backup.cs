using DAL_64PR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_64PR
{
    public class Backup
    {
        mpp_backup mpp = new mpp_backup();

        public void CrearCarpetaSiNoExiste(string ruta)
        {
            mpp.CrearCarpetaSiNoExiste(ruta);
        }

        public void GenerarBackup(string rutaCompleta)
        {
            mpp.GenerarBackup(rutaCompleta);
        }
        public DataTable ObtenerFileList(string rutaBackup)
        {
            return new DAL_64PR.mpp_backup().ObtenerFileList(rutaBackup);
        }

        public string ObtenerRutaDefaultData()
        {
            return new DAL_64PR.mpp_backup().ObtenerRutaDefaultData();
        }

        public void RestaurarBackup(string rutaBackup, string logicalData, string logicalLog,
                                     string rutaDestinoMdf, string rutaDestinoLdf)
        {
            new DAL_64PR.mpp_backup().RestaurarBackup(rutaBackup, logicalData, logicalLog, rutaDestinoMdf, rutaDestinoLdf);
        }
    }
}
