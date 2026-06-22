using Servicios_64PR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_64PR
{
    public class DV_64PR
    {
        public static readonly Dictionary<string, string[]> Tablas = new Dictionary<string, string[]>
        {
            { "Roles_64PR",         new string[] { "ID_Rol" } },
            { "Familia_64PR",       new string[] { "ID_Familia" } },
            { "Familia_N_64PR",     new string[] { "ID_FamiliaPadre", "ID_FamiliaHija" } },
            { "Patente_64PR",       new string[] { "ID_Patente" } },
            { "PatenteFamilia_64PR",new string[] { "ID_Familia", "ID_Patente" } },
            { "RolFamilia_64PR",    new string[] { "ID_Rol", "ID_Familia" } },
            { "RolPatente_64PR",    new string[] { "ID_Rol", "ID_Patente" } },
            { "USUARIO_64PR",       new string[] { "DNI" } }
        };

        DAL_64PR.mpp_DV mpp = new DAL_64PR.mpp_DV();

        /*
        ESTA LOGICA QUEDO VIEJA, NO LO ELIMINO POR LAS DUDAS, PERO LO Q ME HACIA ERA RECALCULAR EL DVH SE LA FILA AGREGADA/MODIFICADA/ELIMINADA
        PERO AHORA LO QUE HAGO ES RECALCULAR TODA LA TABLA, AUNQUE TODOS LOS DEMAS SIGAN IGUAL, TAL VEZ SEA MENOS EFICIENTE, PERO MAS SIMPLE
        /// Se llama después de cada INSERT/UPDATE sobre cualquier tabla del sistema
        public void ActualizarDVHFila(string nombreTabla, string[] columnasPK, Dictionary<string, object> pkValores)
        {
            mpp.ActualizarDVHFila(nombreTabla, columnasPK, pkValores);
        }

        /// Se llama después de cada DELETE
        public void EliminarDVHFila(string nombreTabla, string idFila)
        {
            mpp.EliminarDVHFila(nombreTabla, idFila);
        }
        */

        /// Recalcula el DVH de toda una tabla "al vuelo" (sin tocar lo guardado),
        /// lo vamos a usar en el flujo de REVISIÓN para comparar contra DVH_64PR/DV_64PR
        public List<DVH_64PR> CalcularDVHTabla(string nombreTabla, string[] columnasPK)
        {
            return mpp.CalcularDVHTabla(nombreTabla, columnasPK);
        }
        /// Compara el DV recalculado contra el DV guardado para UNA tabla
        public bool VerificarTabla(string nombreTabla, string[] columnasPK)
        {
            long calculado = mpp.CalcularDVTablaActual(nombreTabla, columnasPK);
            long guardado = mpp.ObtenerDVGuardado(nombreTabla);
            return calculado == guardado;
        }

        /// Recorre TODO el catálogo y devuelve la lista de tablas inconsistentes
        public Dictionary<string, List<string>> VerificarIntegridadCompleta()
        {
            Dictionary<string, List<string>> resultado = new Dictionary<string, List<string>>();

            foreach (var entrada in Tablas)
            {
                string nombreTabla = entrada.Key;
                string[] columnasPK = entrada.Value;

                if (!VerificarTabla(nombreTabla, columnasPK))
                {
                    List<string> filasAfectadas = mpp.ObtenerFilasInconsistentes(nombreTabla, columnasPK);
                    resultado[nombreTabla] = filasAfectadas;
                }
            }

            return resultado;
        }
        public void RecalcularIntegridadCompleta()
        {
            foreach (var entrada in Tablas)
            {
                string nombreTabla = entrada.Key;
                string[] columnasPK = entrada.Value;
                mpp.RecalcularTabla(nombreTabla, columnasPK);
            }
        }
        public void RecalcularTabla(string nombreTabla)
        {
            if (!Tablas.ContainsKey(nombreTabla))
                throw new ArgumentException($"Tabla '{nombreTabla}' no registrada en el sistema de DV.");

            string[] columnasPK = Tablas[nombreTabla];
            mpp.RecalcularTabla(nombreTabla, columnasPK);
        }
    }
}
