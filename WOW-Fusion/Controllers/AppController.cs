using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
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
            toolTip.ShowAlways = false;
            toolTip.SetToolTip(control, message);
        }

        public static string ComputeHash(string input, HashAlgorithm algorithm)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = algorithm.ComputeHash(inputBytes);
            StringBuilder hashStringBuilder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashStringBuilder.Append(hashBytes[i].ToString("x2"));
            }

            return hashStringBuilder.ToString();
        }

        public static bool CheckInternetConnection()
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send("www.google.com", 3000); // Timeout de 3000 ms
                    if (reply.Status == IPStatus.Success)
                    {
                        return true;
                    }
                }
            }
            catch (PingException)
            {
                // Ignorar cualquier excepción de ping y devolver false
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}", Color.Red);
            }

            return false;
        }
    }
}
