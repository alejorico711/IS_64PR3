using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
namespace Servicios_64PR
{
    public class GestorIdioma_64PR
    {
        private static GestorIdioma_64PR _instancia;

        public static GestorIdioma_64PR GetInstance
        {
            get
            {
                if (_instancia == null)
                    _instancia = new GestorIdioma_64PR();
                return _instancia;
            }
        }

        private GestorIdioma_64PR() { }

        private string _idiomaActual = "es";   ///Idioma por defecto
        private Dictionary<string, string> _textos = new Dictionary<string, string>();
        private readonly List<IObservadorIdioma_64PR> _observadores = new List<IObservadorIdioma_64PR>();

        public string IdiomaActual => _idiomaActual;

        ///Carpeta donde están los JSON. Usamos AppDomain para que funcione tanto en Debug como publicado.
        private string RutaIdiomas => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "idiomas");

        public void Suscribir(IObservadorIdioma_64PR observador)
        {
            if (!_observadores.Contains(observador))
                _observadores.Add(observador);
        }

        public void Desuscribir(IObservadorIdioma_64PR observador)
        {
            _observadores.Remove(observador);
        }

        /// <summary>
        /// Cambiar idioma:
        /// Carga el JSON del idioma indicado y notifica a todos los observadores.
        /// </summary>
        /// <param name="codigoIdioma">Ej: "es" o "en"</param>
        public void SetIdioma(string codigoIdioma)
        {
            string ruta = Path.Combine(RutaIdiomas, codigoIdioma + ".json");

            if (!File.Exists(ruta))
            {
                throw new FileNotFoundException(
                    $"No se encontró el archivo de idioma: {ruta}");
            }

            string json = File.ReadAllText(ruta, System.Text.Encoding.UTF8);

            ///El JavaScriptSerializer está disponible en .NET sin necesidad de NuGet
            var serializer = new JavaScriptSerializer();
            _textos = serializer.Deserialize<Dictionary<string, string>>(json);

            _idiomaActual = codigoIdioma;

            NotificarObservadores();
        }
        private void NotificarObservadores()
        {
            ///Iteramos sobre una copia para evitar problemas si algún
            ///observador se desuscribe durante la notificación.
            foreach (var obs in new List<IObservadorIdioma_64PR>(_observadores))
                obs.ActualizarIdioma(_textos);
        }

        /// <summary>
        /// Devuelve los textos actuales (útil para que un form recién
        /// abierto se inicialice sin esperar una notificación).
        /// </summary>
        public Dictionary<string, string> ObtenerTextos() => _textos;
        public List<string> IdiomasDisponibles()
        {
            /// Lista los idiomas disponibles leyendo los JSON en la carpeta.
            var lista = new List<string>();
            if (!Directory.Exists(RutaIdiomas)) return lista;

            foreach (string archivo in Directory.GetFiles(RutaIdiomas, "*.json"))
                lista.Add(Path.GetFileNameWithoutExtension(archivo));

            return lista;
        }
    }
}
