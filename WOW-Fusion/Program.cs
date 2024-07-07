using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Views.Plant1;
using WOW_Fusion.Views.Plant3;

namespace WOW_Fusion
{
    internal static class Program
    {
        private static readonly string MutexName = "WOW_Fusion";
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (Mutex mutex = new Mutex(false, MutexName, out bool createdNew))
            {
                if(createdNew)
                { 
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new frmLoginP1());
                    //Application.Run(new frmLabelP2());
                    //Application.Run(new frmLabelP3());
                }
                else
                {
                    MessageBox.Show("Esta aplicación ya esta abierta", "Aplicación en ejecución", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}
