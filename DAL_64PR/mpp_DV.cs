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



        /*
        ESTA LOGICA QUEDO VIEJA YA QUE ERAN PARA NO CALCULAR TODOS LOS DV DE UNA TABLA CUANDO SE MODIFICABA
        /// Calcula el DVH de UNA sola fila (para usar después de un INSERT/UPDATE puntual)
        public DVH_64PR CalcularDVHFila(string nombreTabla, string[] columnasPK, Dictionary<string, object> pkValores)
        {
            string where = string.Join(" AND ", Array.ConvertAll(columnasPK, c => $"[{c}] = @{c}"));

            SqlParameter[] p = new SqlParameter[2];
            p[0] = new SqlParameter("@Condicion", where);
            p[1] = new SqlParameter("@nombreTabla", nombreTabla);

            string query = "SELECT * FROM @nombreTabla WHERE @Condicion";

            List<SqlParameter> parametros = new List<SqlParameter>();
            foreach (string colPK in columnasPK)
                parametros.Add(new SqlParameter("@" + colPK, pkValores[colPK]));

            DataTable tabla = Acceso.Instancia.leerQuery(query, parametros.ToArray());
            if (tabla.Rows.Count == 0) return null;

            DataRow fila = tabla.Rows[0];
            long dvh = 0;
            foreach (DataColumn col in tabla.Columns)
                dvh += ValorHexDeCelda(fila[col]);

            List<string> partesPK = new List<string>();
            foreach (string colPK in columnasPK)
                partesPK.Add(fila[colPK].ToString());

            return new DVH_64PR
            {
                Tabla = nombreTabla,
                IdFila = string.Join("-", partesPK),
                Valor = dvh
            };
        }
        /// Busca el DVH guardado de una fila. Si no existe (fila nueva), devuelve 0.
        private long ObtenerDVHGuardado(string nombreTabla, string idFila)
        {
            string query = "SELECT DVH FROM DVH_64PR WHERE Tabla = @Tabla AND IdFila = @IdFila";
            SqlParameter[] parametros = new SqlParameter[]
            {
        new SqlParameter("@Tabla", nombreTabla),
        new SqlParameter("@IdFila", idFila)
            };

            object resultado = Acceso.Instancia.leerEscalar(query, parametros);
            if (resultado == null || resultado == DBNull.Value)
                return 0;

            return Convert.ToInt64(resultado);
        }

        /// Actualiza el DV de tabla sumándole la diferencia (puede ser negativa)
        private void ActualizarDVTabla(string nombreTabla, long diferencia)
        {
            string query = @"
        IF EXISTS (SELECT 1 FROM DV_64PR WHERE Tabla = @Tabla)
            UPDATE DV_64PR SET DV = DV + @Diferencia WHERE Tabla = @Tabla
        ELSE
            INSERT INTO DV_64PR (Tabla, DV) VALUES (@Tabla, @Diferencia)";

            SqlParameter[] parametros = new SqlParameter[]
            {
        new SqlParameter("@Tabla", nombreTabla),
        new SqlParameter("@Diferencia", diferencia)
            };

            Acceso.Instancia.escribirQuery(query, parametros);
        }

        /// Punto de entrada público: esto es lo que vas a llamar después de cada INSERT/UPDATE
        public void ActualizarDVHFila(string nombreTabla, string[] columnasPK, Dictionary<string, object> pkValores)
        {
            DVH_64PR nuevo = CalcularDVHFila(nombreTabla, columnasPK, pkValores);
            if (nuevo == null) return; // la fila no existe (raro, pero por las dudas)

            long valorViejo = ObtenerDVHGuardado(nombreTabla, nuevo.IdFila);

            GuardarDVH(nuevo);

            long diferencia = nuevo.Valor - valorViejo;
            ActualizarDVTabla(nombreTabla, diferencia);
        }

        /// Para cuando se ELIMINA una fila: hay que sacar su DVH y restarlo del DV de tabla
        public void EliminarDVHFila(string nombreTabla, string idFila)
        {
            long valorViejo = ObtenerDVHGuardado(nombreTabla, idFila);
            if (valorViejo == 0) return; // no había nada guardado

            string query = "DELETE FROM DVH_64PR WHERE Tabla = @Tabla AND IdFila = @IdFila";
            SqlParameter[] parametros = new SqlParameter[]
            {
        new SqlParameter("@Tabla", nombreTabla),
        new SqlParameter("@IdFila", idFila)
            };
            Acceso.Instancia.escribirQuery(query, parametros);

            ActualizarDVTabla(nombreTabla, -valorViejo);
        }
        
        */
        
        
        public long ObtenerDVGuardado(string nombreTabla)
        {
            string query = "SELECT DV FROM DV_64PR WHERE Tabla = @Tabla";
            SqlParameter[] parametros = new SqlParameter[] { new SqlParameter("@Tabla", nombreTabla) };

            object resultado = Acceso.Instancia.leerEscalar(query, parametros);
            if (resultado == null || resultado == DBNull.Value)
                return 0;

            return Convert.ToInt64(resultado);
        }

        /// Recalcula el DV de una tabla "al vuelo", sumando los DVH de todas sus filas actuales
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
        /// Devuelve los IdFila que no coinciden (o que directamente no existen en DVH_64PR).
        public List<string> ObtenerFilasInconsistentes(string nombreTabla, string[] columnasPK)
        {
            // DVH calculado desde los datos actuales
            List<DVH_64PR> calculados = CalcularDVHTabla(nombreTabla, columnasPK);

            // DVH guardados en DVH_64PR para esta tabla
            string query = "SELECT IdFila, DVH FROM DVH_64PR WHERE Tabla = @Tabla";
            SqlParameter[] parametros = new SqlParameter[]
            {
        new SqlParameter("@Tabla", nombreTabla)
            };
            DataTable guardados = Acceso.Instancia.leerQuery(query, parametros);

            // Convertimos a Dictionary para búsqueda O(1)
            Dictionary<string, long> dvhGuardados = new Dictionary<string, long>();
            foreach (DataRow fila in guardados.Rows)
                dvhGuardados[fila["IdFila"].ToString()] = Convert.ToInt64(fila["DVH"]);

            List<string> filasAfectadas = new List<string>();
            foreach (DVH_64PR calculado in calculados)
            {
                // Fila que no tiene DVH guardado, o cuyo DVH no coincide
                if (!dvhGuardados.ContainsKey(calculado.IdFila) ||
                     dvhGuardados[calculado.IdFila] != calculado.Valor)
                {
                    filasAfectadas.Add(calculado.IdFila);
                }
            }

            return filasAfectadas;
        }
    }
}
