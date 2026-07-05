using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios_64PR
{
    public static class Patentes_64PR
    {
        ///esta clase contiene unicamente los "identificadores" para las patentes (se utilizo el nombre patente y no permiso, para diferenciarlas aunque sean sinonimos)
        ///se hace con el objetivo de no usar strings en crudo, y si en la base de datos se modifica una patente, esa
        ///patente se cambie una sola vez aqui. Cabe resaltar que el valor de la constante debe coincidir EXACTAMENTE con la BD

        public const string CrearUsuario = "Crear usuario";
        public const string DesbloquearUsuario = "Desbloquear usuario";
        public const string ModificarUsuario = "Modificar usuario";
        public const string ActivarDesactivarUsuarios = "Activar y desactivar usuarios";
        public const string Bitacora = "Bitacora";
        public const string CrearRoles = "Crear roles";
        public const string ModificarRoles = "Modificar roles";
        public const string EliminarRoles = "Eliminar roles";
        public const string CrearFamilias = "Crear familias";
        public const string ModificarFamilias = "Modificar familias";
        public const string EliminarFamilias = "Eliminar familias";
        public const string CambiarContra = "Cambiar clave";
        public const string CambiarIdioma = "Cambiar idioma";
        public const string Respaldos = "Hacer respaldos";
        public const string Restauraciones = "Hacer restauraciones";
    }
}
