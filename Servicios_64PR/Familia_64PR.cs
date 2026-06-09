using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios_64PR
{
    public class Familia_64PR : Rol_64PR
    {
        public override bool TienePermiso(string nombre)
        {
            foreach (var hijo in Hijos)
                if (hijo.TienePermiso(nombre)) return true;
            return false;
        }
    }
}
