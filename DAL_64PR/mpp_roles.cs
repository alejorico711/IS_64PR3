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
    public class mpp_roles
    {
        public List<Rol_64PR> ListarRoles()
        {
            List<Servicios_64PR.Rol_64PR> lista = new List<Servicios_64PR.Rol_64PR>();

            string query = "SELECT * FROM Roles_64PR";
            DataTable tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, null);

            foreach (DataRow dr in tabla.Rows)
            {
                Servicios_64PR.Rol_64PR r = new Servicios_64PR.Rol_64PR();
                r.Id = int.Parse(dr["ID_Rol"].ToString());
                r.Nombre = dr["Nombre"].ToString();
                lista.Add(r);
            }
            return lista;
        }

        public Servicios_64PR.Rol_64PR ObtenerRolCompleto(int idRol)
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

        public List<Servicios_64PR.Rol_64PR> ObtenerTodosLosNodos()
        {
            List<Servicios_64PR.Rol_64PR> lista = new List<Servicios_64PR.Rol_64PR>();

            /// Primero todas las Familias con sus patentes y subfamilias
            string query = "SELECT ID_Familia, Nombre FROM Familia_64PR";
            DataTable tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, null);

            foreach (DataRow dr in tabla.Rows)
            {
                Servicios_64PR.Familia_64PR f = new Servicios_64PR.Familia_64PR();
                f.Id = Convert.ToInt32(dr["ID_Familia"]);
                f.Nombre = dr["Nombre"].ToString();
                CargarHijosFamilia(f);
                lista.Add(f);
            }

            /// Después las Patentes
            query = @"SELECT P.ID_Patente, P.Nombre FROM Patente_64PR P";
            tabla = DAL_64PR.Acceso.Instancia.leerQuery(query, null);

            foreach (DataRow dr in tabla.Rows)
            {
                Servicios_64PR.Permiso_64PR p = new Servicios_64PR.Permiso_64PR();
                p.Id = Convert.ToInt32(dr["ID_Patente"]);
                p.Nombre = dr["Nombre"].ToString();
                lista.Add(p);
            }

            return lista;
        }

        public void CrearFamilia(string nombre, List<Rol_64PR> hijos)
        {
            try
            {
                /// 1. Insertar la familia y obtener su ID
                string queryInsert = "INSERT INTO Familia_64PR (Nombre) VALUES (@Nombre); SELECT SCOPE_IDENTITY();";
                SqlParameter[] parametros = new SqlParameter[]
                {
                    new SqlParameter("@Nombre", nombre)
                };
                int nuevoId = Convert.ToInt32(DAL_64PR.Acceso.Instancia.leerEscalar(queryInsert, parametros));

                foreach (var hijo in hijos)
                {
                    if (hijo is Servicios_64PR.Familia_64PR)
                    {
                        string query = "INSERT INTO Familia_N_64PR (ID_FamiliaPadre, ID_FamiliaHija) VALUES (@Padre, @Hija)";
                        SqlParameter[] p = new SqlParameter[]
                        {
                            new SqlParameter("@Padre", nuevoId),
                            new SqlParameter("@Hija", hijo.Id)
                        };
                        DAL_64PR.Acceso.Instancia.escribirQuery(query, p);
                    }
                    else if (hijo is Servicios_64PR.Permiso_64PR)
                    {
                        string query = "INSERT INTO PatenteFamilia_64PR (ID_Familia, ID_Patente) VALUES (@Familia, @Patente)";
                        SqlParameter[] p = new SqlParameter[]
                        {
                            new SqlParameter("@Familia", nuevoId),
                            new SqlParameter("@Patente", hijo.Id)
                        };
                        DAL_64PR.Acceso.Instancia.escribirQuery(query, p);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void EliminarFamilia(int id)
        {
            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@IdFamilia", id)
            };
            ///Esto se hace asi, ya que familiaN tiene 2 FK y el cascade se puede poner en uno solo para no generar dependencias circulares
            /// 1. Sacarla de donde era hija (manual)
            DAL_64PR.Acceso.Instancia.escribirQuery(
                "DELETE FROM Familia_N_64PR WHERE ID_FamiliaHija = @IdFamilia",
                parametros);    

            /// 2. Eliminar la familia (CASCADE hace el resto)
            DAL_64PR.Acceso.Instancia.escribirQuery(
                "DELETE FROM Familia_64PR WHERE ID_Familia = @IdFamilia",
                parametros);
        }

        public void ModificarFamilia(int id, string nombre, List<Rol_64PR> hijos)
        {
            /// 1. Actualizar el nombre
            DAL_64PR.Acceso.Instancia.escribirQuery(
                "UPDATE Familia_64PR SET Nombre = @Nombre WHERE ID_Familia = @Id",
                new SqlParameter[]
                {
            new SqlParameter("@Nombre", nombre),
            new SqlParameter("@Id", id)
                });

            /// 2. Borrar relaciones anteriores
            SqlParameter[] parametros = new SqlParameter[]
            {
        new SqlParameter("@IdFamilia", id)
            };

            DAL_64PR.Acceso.Instancia.escribirQuery(
                "DELETE FROM Familia_N_64PR WHERE ID_FamiliaPadre = @IdFamilia",
                parametros);

            DAL_64PR.Acceso.Instancia.escribirQuery(
                "DELETE FROM PatenteFamilia_64PR WHERE ID_Familia = @IdFamilia",
                parametros);

            /// 3. Insertar las nuevas relaciones
            foreach (var hijo in hijos)
            {
                if (hijo is Servicios_64PR.Familia_64PR)
                {
                    DAL_64PR.Acceso.Instancia.escribirQuery(
                        "INSERT INTO Familia_N_64PR (ID_FamiliaPadre, ID_FamiliaHija) VALUES (@Padre, @Hija)",
                        new SqlParameter[]
                        {
                    new SqlParameter("@Padre", id),
                    new SqlParameter("@Hija", hijo.Id)
                        });
                }
                else if (hijo is Servicios_64PR.Permiso_64PR)
                {
                    DAL_64PR.Acceso.Instancia.escribirQuery(
                        "INSERT INTO PatenteFamilia_64PR (ID_Familia, ID_Patente) VALUES (@Familia, @Patente)",
                        new SqlParameter[]
                        {
                    new SqlParameter("@Familia", id),
                    new SqlParameter("@Patente", hijo.Id)
                        });
                }
            }
        }
        public void CrearRol(string nombre, List<Servicios_64PR.Rol_64PR> hijos)
        {
            int nuevoId = Convert.ToInt32(DAL_64PR.Acceso.Instancia.leerEscalar(
                "INSERT INTO Roles_64PR (Nombre) VALUES (@Nombre); SELECT SCOPE_IDENTITY();",
                new SqlParameter[] { new SqlParameter("@Nombre", nombre) }));

            foreach (var hijo in hijos)
            {
                if (hijo is Servicios_64PR.Familia_64PR)
                {
                    DAL_64PR.Acceso.Instancia.escribirQuery(
                        "INSERT INTO RolFamilia_64PR (ID_Rol, ID_Familia) VALUES (@Rol, @Familia)",
                        new SqlParameter[]
                        {
                    new SqlParameter("@Rol", nuevoId),
                    new SqlParameter("@Familia", hijo.Id)
                        });
                }
                else if (hijo is Servicios_64PR.Permiso_64PR)
                {
                    DAL_64PR.Acceso.Instancia.escribirQuery(
                        "INSERT INTO RolPatente_64PR (ID_Rol, ID_Patente) VALUES (@Rol, @Patente)",
                        new SqlParameter[]
                        {
                    new SqlParameter("@Rol", nuevoId),
                    new SqlParameter("@Patente", hijo.Id)
                        });
                }
            }
        }

        public void ModificarRol(int id, string nombre, List<Servicios_64PR.Rol_64PR> hijos)
        {
            DAL_64PR.Acceso.Instancia.escribirQuery(
                "UPDATE Roles_64PR SET Nombre = @Nombre WHERE ID_Rol = @Id",
                new SqlParameter[]
                {
            new SqlParameter("@Nombre", nombre),
            new SqlParameter("@Id", id)
                });

            SqlParameter[] parametros = new SqlParameter[]
            {
        new SqlParameter("@IdRol", id)
            };

            DAL_64PR.Acceso.Instancia.escribirQuery(
                "DELETE FROM RolFamilia_64PR WHERE ID_Rol = @IdRol", parametros);

            DAL_64PR.Acceso.Instancia.escribirQuery(
                "DELETE FROM RolPatente_64PR WHERE ID_Rol = @IdRol", parametros);

            foreach (var hijo in hijos)
            {
                if (hijo is Servicios_64PR.Familia_64PR)
                {
                    DAL_64PR.Acceso.Instancia.escribirQuery(
                        "INSERT INTO RolFamilia_64PR (ID_Rol, ID_Familia) VALUES (@Rol, @Familia)",
                        new SqlParameter[]
                        {
                    new SqlParameter("@Rol", id),
                    new SqlParameter("@Familia", hijo.Id)
                        });
                }
                else if (hijo is Servicios_64PR.Permiso_64PR)
                {
                    DAL_64PR.Acceso.Instancia.escribirQuery(
                        "INSERT INTO RolPatente_64PR (ID_Rol, ID_Patente) VALUES (@Rol, @Patente)",
                        new SqlParameter[]
                        {
                    new SqlParameter("@Rol", id),
                    new SqlParameter("@Patente", hijo.Id)
                        });
                }
            }
        }

        public void EliminarRol(int id, bool reasignarUsuarios)
        {
            if (reasignarUsuarios)
            {
                DAL_64PR.Acceso.Instancia.escribirQuery(
                    "UPDATE USUARIO_64PR SET Rol = 1 WHERE Rol = @IdRol",
                    new SqlParameter[] { new SqlParameter("@IdRol", id) });
            }

            // CASCADE se encarga de RolFamilia_64PR y RolPatente_64PR
            DAL_64PR.Acceso.Instancia.escribirQuery(
                "DELETE FROM Roles_64PR WHERE ID_Rol = @IdRol",
                new SqlParameter[] { new SqlParameter("@IdRol", id) });
        }

        public int ContarUsuariosConRol(int idRol)
        {
            object resultado = DAL_64PR.Acceso.Instancia.leerEscalar(
                "SELECT COUNT(*) FROM USUARIO_64PR WHERE Rol = @IdRol",
                new SqlParameter[] { new SqlParameter("@IdRol", idRol) });

            return Convert.ToInt32(resultado);
        }
    }
}
