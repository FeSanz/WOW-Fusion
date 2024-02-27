using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Google.Apis.Requests.BatchRequest;
using WOW_Fusion.Properties;
using System.Net;
using WOW_Fusion.Services;
using System.Threading;
using System.Runtime.InteropServices.ComTypes;

namespace WOW_Fusion
{
    internal class RadwagController
    {
        private static TcpClient _client;
        private static NetworkStream _stream;

        public static async Task<string> SocketWeighing(string command)
        {
            string response = "";

            try
            {
                if (_client == null || !_client.Connected)
                {
                    _client = new TcpClient();
                    // Cambia la IP y el puerto según tu configuración
                    await _client.ConnectAsync(Settings.Default.WeighingIP, Settings.Default.WeighingPort); 
                    _stream = _client.GetStream();


                    byte[] data = Encoding.ASCII.GetBytes(command + "\r\n");
                    // Enviar datos al servidor de forma asíncrona
                    await _stream.WriteAsync(data, 0, data.Length);
                    // Leer Datos del servidor
                    response = await ReadDataUntilCR(_stream, 6000); //->6000
                    string readLine = response;

                    if (command.Equals("T") || command.Equals("S"))
                    {
                        switch (readLine.Substring(2, 1))
                        {
                            case "A":
                                //await _client.ConnectAsync(_ip, _port); // Cambia la IP y el puerto según tu configuración
                                //Thread.Sleep(100);
                                response = await ReadDataUntilCR(_stream, 6000);
                                response = SecondLineResponse(response);
                                break;
                            default:
                                response = "(1) " + readLine;
                                break;
                        }
                    }
                    else if (command.Equals("OT"))
                    {
                        response = readLine.Substring(3, 9).Trim();
                    }
                }

                if (_client != null)
                {
                    _client.Close();
                    _stream.Close();
                    _client = null;
                    _stream = null;
                }
            }
            catch (Exception ex)
            {
                response = "EX";
                Console.WriteLine($"{DateService.Today()} -> Error Socket Báscula. {ex.Message}");
                _client = null;
                _stream = null;
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
            Thread.Sleep(100);
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
                    response = secondLineResponse.Substring(6, 8).Trim();
                    break;
                default:
                    response = "(2) " + secondLineResponse;
                    break;
            }
            return response;
        }

        public static async Task<string> ReadDataUntilCR(NetworkStream stream, int timeoutMilliseconds)
        {
            try
            {
                StringBuilder dataBuffer = new StringBuilder();
                byte[] readBuffer = new byte[255];

                // Configurar un CancellationTokenSource con un tiempo de espera
                CancellationTokenSource cts = new CancellationTokenSource(timeoutMilliseconds);

                while (true)
                {
                    // Verificar si hay datos disponibles antes de intentar leer
                    if (stream.DataAvailable)
                    {
                        int bytesRead = await stream.ReadAsync(readBuffer, 0, readBuffer.Length, cts.Token);

                        if (bytesRead == 0)
                        {
                            // No se han leído bytes, la conexión probablemente se ha cerrado.
                            Console.WriteLine($"{DateService.Today()} -> Se ha cerrado la conexión a la báscula");
                            return null;
                        }

                        // Decodificar y agregar al búfer
                        string decodedData = Encoding.ASCII.GetString(readBuffer, 0, bytesRead);
                        dataBuffer.Append(decodedData);

                        // Verificar si '\n' está presente en el búfer
                        if (dataBuffer.ToString().Contains("\n"))
                        {
                            // Eliminar '\n' del final
                            int newlineIndex = dataBuffer.ToString().IndexOf('\n');
                            dataBuffer.Remove(newlineIndex, dataBuffer.Length - newlineIndex);

                            return dataBuffer.ToString();
                        }
                    }

                    // Comprobar si se ha cancelado la operación debido al tiempo de espera
                    if (cts.Token.IsCancellationRequested)
                    {
                        Console.WriteLine($"{DateService.Today()} -> Tiempo de espera excedido en la lécura de la báscula");
                        return "EX";
                    }

                    // Introducir una pausa para no sobrecargar la CPU
                    await Task.Delay(10);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateService.Today()} -> Error en lectura de la báscula. {ex.Message}");
                return "EX";
            }
        }
    }
}
