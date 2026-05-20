using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoIS_64PR
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FrmLogin_64PR formInicio = new FrmLogin_64PR();
            formInicio.Show();

            Application.Run();  
            //se cambio el Application.Run(New FrmLogin_64PR) a Application.Run() simplemente
            //ya que necesitabamos cerrar todos los formularios y volver a abrir el de login sin que el programa
            //se detuviera al momento de cambiar la contraseña, ya que se deben cerrar todos los formularios abiertos,
            //cerrar la sesion y abrir un nuevo formulario de login
        }
    }
}
