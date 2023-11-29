using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Controllers;
using static System.Net.Mime.MediaTypeNames;

namespace WOW_Fusion.Services
{
    internal class LabelService
    {
        private static string zplTemplate = string.Empty;
        private static TcpClient _client;
        private static NetworkStream _stream;
        public static Stream CreateFromFile(string designSelected, int plant)
        {            
            Stream responseStream;
            string zpl;

            if (plant == 1)
            {
                zplTemplate = File.ReadAllText($"{Constants.PathLabelsP1}\\{designSelected}.prn");
                zpl = ReplaceZPLP1(1);
            }
            else if(plant == 2)
            {
                zplTemplate = File.ReadAllText($"{Constants.PathLabelsP2}\\{designSelected}.prn");
                zpl = ReplaceZPLP2(1);
            }
            else
            {
                zpl = "Vacio";
            }

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(String.Format(Constants.LaberalyUrl, zpl));
                var response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
            }
            catch (WebException ex)
            {
                responseStream = null;
                NotifierController.DetailError("Error labelary", ex.Message);
            }
            
            return responseStream;
        }

        public static string[] FilesDesign(string path) 
        {
            List<string> items = new List<string>();

            string[] files = Directory.GetFiles(path, "*.prn");
            foreach (string file in files)
            {
                items.Add(Path.GetFileNameWithoutExtension(file));
            }
            return items.ToArray();
        }

        public static async Task PrintP1(int quantity)
        {
            for (int i = 1; i <= quantity; i++)
            {
                try
                {
                    _client = new TcpClient();
                    await _client.ConnectAsync(Constants.PrinterIp, Constants.PrinterPort);
                    //_client.Connect(ipPrinter, portPrinter);
                    _stream = _client.GetStream();

                    if (_client.Connected)
                    {
                        string zpl = ReplaceZPLP1(i);
                        Thread.Sleep(500);
                        byte[] data = Encoding.ASCII.GetBytes(zpl);

                        // Enviar datos al servidor de forma asíncrona
                        await _stream.WriteAsync(data, 0, data.Length);
                        //_stream.Write(data, 0, data.Length);
                        await _stream.FlushAsync();
                        _stream.Close();
                        _client.Close();
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _client.Close();
                    NotifierController.DetailError("Error al imprimir", ex.Message);
                    break;
                }
            }
        }
       
        private static string ReplaceZPLP1(int box)
        {
            string strLabel = zplTemplate; //Template sin reemplazos

            JObject label = new JObject(JObject.Parse(Constants.LabelJson));
            foreach (var item in label)
            {
                if(!string.IsNullOrEmpty(item.Value.ToString()))
                    strLabel = item.Key.Equals("BOXNUMBER") ? strLabel.Replace(item.Key, box.ToString().PadLeft(5, '0')) : strLabel.Replace(item.Key, item.Value.ToString());
            }
            return strLabel;
        }

        private static string ReplaceZPLP2(int roll)
        {
            string strLabel = zplTemplate; //Template sin reemplazos

            JObject label = new JObject(JObject.Parse(Constants.LabelJson));
            foreach (var item in label)
            {
                if (!string.IsNullOrEmpty(item.Value.ToString()))
                {
                    strLabel = item.Key.Equals("ROLLNUMBER") ? strLabel.Replace(item.Key, roll.ToString().PadLeft(5, '0')) : strLabel.Replace(item.Key, item.Value.ToString());
                }
            }
            return strLabel;
        }
    }
}
