using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WOW_P2
{
    internal class WeighingController
    {
        public string SocketWeighing(string ip, int port, string status)
        {
            string response = "0";
            TcpClient client = new TcpClient();
            var result = client.BeginConnect(ip, port, null, null);
            result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3));

            if (client.Connected)
            {
                StreamWriter writer = new StreamWriter(client.GetStream());
                StreamReader reader = new StreamReader(client.GetStream());
                reader.BaseStream.ReadTimeout = 3000;

                try
                {
                    string Peticion = RequestWeighing(status);
                    writer.Write(Peticion + "\r\n");
                    writer.Flush();
                    response = reader.ReadLine();
                    if (Peticion != "OT ")
                    {
                        response = response + "\r\n" + reader.ReadLine();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error. " + ex.Message, "Socket Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    writer.Close();
                    reader.Close();
                    client.Close();
                }
            }
            else
            {
                MessageBox.Show("Báscula no encontrada", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return response;
        }

        private string RequestWeighing(string request)
        {
            string response;
            switch (request)
            {
                case "Tara":
                    response = "T ";
                    break;
                case "Peso":
                    response = "S ";
                    break;
                case "Peso Tara":
                    response = "OT ";
                    break;
                default:
                    response = null;
                    break;
            }
            return response;
        }
    }
}
