using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios_64PR
{
    public class Permiso_64PR : Rol_64PR
    {
        public override bool TienePermiso(string nombre)
        {
            return this.Nombre == nombre;
        }
    }
}
