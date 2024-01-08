using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WOW_Fusion.Controllers
{
    internal class AppController
    {
        public static void Exit(string message)
        {
            MessageBox.Show(message, "Sin datos suficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Application.Exit();
        }

        public static void ToolTip(Control control, string message)
        {
            ToolTip toolTip = new ToolTip();
            toolTip.ShowAlways = true;
            toolTip.SetToolTip(control, message);
        }

        public static string ComputeHash(string input, HashAlgorithm algorithm)
        {
            // Convert the input string to a byte array
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);

            // Compute the hash value
            byte[] hashBytes = algorithm.ComputeHash(inputBytes);

            // Convert the byte array to a hexadecimal string
            StringBuilder hashStringBuilder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashStringBuilder.Append(hashBytes[i].ToString("x2"));
            }

            return hashStringBuilder.ToString();
        }
    }
}
