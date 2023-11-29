using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WOW_Fusion
{
    internal class RadwagController
    {
        public static string SocketWeighing(string command)
        {
            string response = "";
            TcpClient client = new TcpClient();
            var result = client.BeginConnect(Constants.RadwagIp, Constants.RadwagPort, null, null);
            result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3));

            if (client.Connected)
            {
                StreamWriter writer = new StreamWriter(client.GetStream());
                StreamReader reader = new StreamReader(client.GetStream());
                reader.BaseStream.ReadTimeout = 5000;

                try
                {
                    writer.Write(command + "\r\n");
                    writer.Flush();

                    string readLine = reader.ReadLine();

                    if (command.Equals("T") || command.Equals("S"))
                    {
                        switch (readLine.Substring(2, 1))
                        {
                            case "A":
                                response = SecondLineResponse(reader.ReadLine());
                                break;
                            default:
                                response = "(1) " + readLine;
                                break;
                        }
                    }
                    else if (command.Equals("OT"))
                    {
                        response = readLine.Substring(4, 9).Trim();
                    }
                    else
                    {
                        MessageBox.Show("Comando de báscula no encontrado", "Comando", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    writer.Close();
                    reader.Close();

                }
                catch (Exception ex)
                {
                    response = "EX";
                    //MessageBox.Show("Error. " + ex.Message, "Socket Báscula", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                response = "Báscula no encontrada";
            }

            return response;
        }

        /// <summary>
        /// Aplica a comando T y S
        /// </summary>
        /// <param name="secondLineResponse"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private static string SecondLineResponse(string secondLineResponse)
        {
            string response = "";
            switch (secondLineResponse.Substring(2, 1))
            {
                case "D":
                    response = "OK";
                    break;
                case "v":
                    response = "Comando entendido, pero se ha superado el rango de tara";
                    break;
                case "E":
                    response = "Límite de tiempo superado en espera del resultado estable";
                    break;
                case "I":
                    response = "Comando entendido, pero en el momento no está disponible";
                    break;
                case " ":
                    //Peso bascula
                    response = secondLineResponse.Substring(7, 9).Trim();
                    break;
                default:
                    response = "(2) " + secondLineResponse;
                    break;
            }
            return response;
        }

        public static string RemoveLines(string s, int linesToRemove)
        {
            return s.Split(Environment.NewLine.ToCharArray(), linesToRemove + 1).Skip(linesToRemove).FirstOrDefault();
        }
    }
}
