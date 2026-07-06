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
    public class mpp_DV
    {
        private long ValorHexDeCelda(object valor)
        {
            if (valor == null || valor == DBNull.Value)
                return 0;

            string texto = valor.ToString();
            byte[] bytes = Encoding.UTF8.GetBytes(texto);

            long suma = 0;
            foreach (byte b in bytes)
                suma += b;

            return suma;
        }

        /// Calcula el DVH de TODAS las filas de una tabla (Cálculo Horizontal - columna a columna)
        public List<DVH_64PR> CalcularDVHTabla(string nombreTabla, string[] columnasPK)
        {
            List<DVH_64PR> resultado = new List<DVH_64PR>();

            SqlParameter[] p = new SqlParameter[1];
            p[0] = new SqlParameter("@nombreTabla", nombreTabla);
            string query = $"SELECT * FROM [{nombreTabla}]";
            DataTable tabla = Acceso.Instancia.leerQuery(query, null);

            foreach (DataRow fila in tabla.Rows)
            {
                long dvh = 0;
                foreach (DataColumn col in tabla.Columns)
                    dvh += ValorHexDeCelda(fila[col]);

                List<string> partesPK = new List<string>();
                foreach (string colPK in columnasPK)
                    partesPK.Add(fila[colPK].ToString());

                resultado.Add(new DVH_64PR
                {
                    Tabla = nombreTabla,
                    IdFila = string.Join("-", partesPK),
                    Valor = dvh
                });
            }

            return resultado;
        }

        /// Upsert del DVH de una fila puntual
        private void GuardarDVH(DVH_64PR dvh)
        {
            string query = @"
        IF EXISTS (SELECT 1 FROM DVH_64PR WHERE Tabla = @Tabla AND IdFila = @IdFila)
            UPDATE DVH_64PR SET DVH = @Valor WHERE Tabla = @Tabla AND IdFila = @IdFila
        ELSE
            INSERT INTO DVH_64PR (Tabla, IdFila, DVH) VALUES (@Tabla, @IdFila, @Valor)";

            SqlParameter[] parametros = new SqlParameter[]
            {
        new SqlParameter("@Tabla", dvh.Tabla),
        new SqlParameter("@IdFila", dvh.IdFila),
        new SqlParameter("@Valor", dvh.Valor)
            };

            Acceso.Instancia.escribirQuery(query, parametros);
        }   
        public long ObtenerDVGuardado(string nombreTabla)
        {
            string query = "SELECT DV FROM DV_64PR WHERE Tabla = @Tabla";
            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("@Tabla", nombreTabla) };

            object resultado = Acceso.Instancia.leerEscalar(query, parametros);
            if (resultado == null || resultado == DBNull.Value)
                return 0;

            return Convert.ToInt64(resultado);
        }

        /// Recalcula el DV de una tabla sumando los DVH de todas sus filas actuales
        public long CalcularDVTablaActual(string nombreTabla, string[] columnasPK)
        {
            List<DVH_64PR> filas = CalcularDVHTabla(nombreTabla, columnasPK);
            long suma = 0;
            foreach (DVH_64PR fila in filas)
                suma += fila.Valor;
            return suma;
        }
        /// Fuerza la regeneración completa del DVH/DV de una tabla, descartando lo guardado previamente
        public void RecalcularTabla(string nombreTabla, string[] columnasPK)
        {
            // 1. Borra todo lo que había guardado para esta tabla
            Acceso.Instancia.escribirQuery(
                "DELETE FROM DVH_64PR WHERE Tabla = @Tabla",
                new SqlParameter[] { new SqlParameter("@Tabla", nombreTabla) });

            Acceso.Instancia.escribirQuery(
                "DELETE FROM DV_64PR WHERE Tabla = @Tabla",
                new SqlParameter[] { new SqlParameter("@Tabla", nombreTabla) });

            // 2. Recalcula desde cero, como si cada fila se estuviera persistiendo por primera vez
            List<DVH_64PR> filas = CalcularDVHTabla(nombreTabla, columnasPK);
            long total = 0;
            foreach (DVH_64PR fila in filas)
            {
                GuardarDVH(fila);
                total += fila.Valor;
            }

            // 3. Inserta el nuevo DV de tabla
            Acceso.Instancia.escribirQuery(
                "INSERT INTO DV_64PR (Tabla, DV) VALUES (@Tabla, @DV)",
                new SqlParameter[]
                {
            new SqlParameter("@Tabla", nombreTabla),
            new SqlParameter("@DV", total)
                });
        }
        /// Compara el DVH calculado vs el guardado fila por fila.
        /// Clasifica cada anomalía en Insercion, Modificacion o Eliminacion.
        public List<FilaInconsistente_64PR> ObtenerFilasInconsistentes(string nombreTabla, string[] columnasPK)
        {
            /// DVH calculado desde los datos actuales
            List<DVH_64PR> calculados = CalcularDVHTabla(nombreTabla, columnasPK);

            /// DVH guardados en DVH_64PR para esta tabla
            string query = "SELECT IdFila, DVH FROM DVH_64PR WHERE Tabla = @Tabla";
            SqlParameter[] parametros = new SqlParameter[]
            {
        new SqlParameter("@Tabla", nombreTabla)
            };
            DataTable guardados = Acceso.Instancia.leerQuery(query, parametros);

            Dictionary<string, long> dvhGuardados = new Dictionary<string, long>();
            foreach (DataRow fila in guardados.Rows)
                dvhGuardados[fila["IdFila"].ToString()] = Convert.ToInt64(fila["DVH"]);

            /// Para después saber cuáles de los guardados ya no están en la tabla real
            HashSet<string> idsVigentes = new HashSet<string>();

            List<FilaInconsistente_64PR> filasAfectadas = new List<FilaInconsistente_64PR>();

            foreach (DVH_64PR calculado in calculados)
            {
                idsVigentes.Add(calculado.IdFila);

                if (!dvhGuardados.ContainsKey(calculado.IdFila))
                {
                    /// Existe en la tabla real pero nunca se guardó su DVH -> alta nueva
                    filasAfectadas.Add(new FilaInconsistente_64PR
                    {
                        Tabla = nombreTabla,
                        IdFila = calculado.IdFila,
                        Tipo = TipoAnomalia_64PR.Insercion
                    });
                }
                else if (dvhGuardados[calculado.IdFila] != calculado.Valor)
                {
                    /// Existe en ambos lados pero el contenido cambió
                    filasAfectadas.Add(new FilaInconsistente_64PR
                    {
                        Tabla = nombreTabla,
                        IdFila = calculado.IdFila,
                        Tipo = TipoAnomalia_64PR.Modificacion
                    });
                }
            }

            /// Lo que estaba guardado pero ya no aparece entre los calculados -> se borró
            foreach (string idGuardado in dvhGuardados.Keys)
            {
                if (!idsVigentes.Contains(idGuardado))
                {
                    filasAfectadas.Add(new FilaInconsistente_64PR
                    {
                        Tabla = nombreTabla,
                        IdFila = idGuardado,
                        Tipo = TipoAnomalia_64PR.Eliminacion
                    });
                }
            }

            return filasAfectadas;
        }
    }
}
