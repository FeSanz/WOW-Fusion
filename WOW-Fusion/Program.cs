using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Views.Plant1;
using WOW_Fusion.Views.Plant3;

namespace WOW_Fusion
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
            Application.Run(new frmLoginP1());
            //Application.Run(new frmLabelP2());
            //Application.Run(new frmLabelP3());
        }
    }
}
