using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios_64PR
{
    public class Rol_64PR
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<Rol_64PR> Hijos { get; set; } = new List<Rol_64PR>();

        public virtual bool TienePermiso(string nombre)
        {
            foreach (var hijo in Hijos)
                if (hijo.TienePermiso(nombre)) return true;
            return false;
        }

        public void Agregar(Rol_64PR r) => Hijos.Add(r);
        public void Quitar(Rol_64PR r) => Hijos.Remove(r);

        public override string ToString()
        {
            return Nombre;
        }
    }
}
