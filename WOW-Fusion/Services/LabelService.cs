using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Controllers;
using WOW_Fusion.Properties;

namespace WOW_Fusion.Services
{
    internal class LabelService
    {
        private static string zplTemplate = string.Empty;
        private static string zplPalletTemplate = string.Empty;
        private static TcpClient _client;
        private static NetworkStream _stream;

        public static async Task<Stream> CreateFromApexAsync(string labelName, int mode)
        {
            Stream responseStream;
            string zpl;

            JObject labels = await CommonService.LabelTamplate(labelName);
            
            if (labels == null) { return null; }

            if (mode == 1) //Box
            {
                zplTemplate = labels["LabelZpl"].ToString().Replace("\r\n", String.Empty);
                zpl = ReplaceZplBox(1);
            }
            else if (mode == 2) //Roll
            {
                zplTemplate = labels["LabelZpl"].ToString().Replace("\r\n", String.Empty);
                zplPalletTemplate = labels["LabelPalletZpl"].ToString().Replace("\r\n", String.Empty);
                zpl = ReplaceZplRoll(1);
            }
            else if (mode == 3) //Pallet
            {
                zpl = ReplaceZplPallet(1);
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

        public static Stream UpdateLabelLabelary(int item)
        {
            Stream responseStream;
            if (string.IsNullOrEmpty(zplTemplate) && string.IsNullOrEmpty(zplPalletTemplate))
            {
                responseStream = null;
            }
            else
            {
                try
                {
                    string zpl = ReplaceZplRoll(item);
                    var request = (HttpWebRequest)WebRequest.Create(String.Format(Constants.LaberalyUrl, zpl));
                    var response = (HttpWebResponse)request.GetResponse();
                    //using (HttpWebResponse response = await request.GetResponseAsync())
                    responseStream = response.GetResponseStream();
                }
                catch (WebException ex)
                {
                    responseStream = null;
                    NotifierController.DetailError("Error labelary Update ", ex.Message);
                }
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
                    await _client.ConnectAsync(Settings.Default.PrinterIP, Settings.Default.PrinterPort);
                    //_client.Connect(ipPrinter, portPrinter);
                    _stream = _client.GetStream();

                    if (_client.Connected)
                    {
                        string zpl = ReplaceZplBox(i);
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

        public static async Task PrintP2(int number, string typee)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(Settings.Default.PrinterIP, Settings.Default.PrinterPort);
                //_client.Connect(ipPrinter, portPrinter);
                _stream = _client.GetStream();

                if (_client.Connected)
                {
                    string zpl = typee.Equals("ROLL") ? ReplaceZplRoll(number) : ReplaceZplPallet(number);
                    byte[] data = Encoding.ASCII.GetBytes(zpl);

                    // Enviar datos al servidor de forma asíncrona
                    await _stream.WriteAsync(data, 0, data.Length);
                    //_stream.Write(data, 0, data.Length);
                    await _stream.FlushAsync();
                    _stream.Close();
                    _client.Close();
                }
            }
            catch (Exception ex)
            {
                _client.Close();
                NotifierController.DetailError("Error al imprimir", ex.Message);
            }
        }

        private static string ReplaceZplBox(int box)
        {
            string strLabel = zplTemplate; //Template sin reemplazos

            JObject label = new JObject(JObject.Parse(Constants.LabelJson));
            foreach (var item in label)
            {
                if (!string.IsNullOrEmpty(item.Value.ToString()))
                    strLabel = item.Key.Equals("BOXNUMBER") ? strLabel.Replace(item.Key, box.ToString().PadLeft(4, '0')) : strLabel.Replace(item.Key, item.Value.ToString());
            }
            return strLabel;
        }

        private static string ReplaceZplRoll(int roll)
        {
            string strLabel = zplTemplate; //Template sin reemplazos

            JObject label = new JObject(JObject.Parse(Constants.LabelJson));
            foreach (var item in label)
            {
                if (!string.IsNullOrEmpty(item.Value.ToString()))
                {
                    strLabel = item.Key.Equals("ROLLNUMBER") ? strLabel.Replace(item.Key, "R" + roll.ToString().PadLeft(4, '0')) : strLabel.Replace(item.Key, item.Value.ToString());
                }
            }
            return strLabel;
        }

        private static string ReplaceZplPallet(int pallet)
        {
            string strLabel = zplPalletTemplate; //Template sin reemplazos

            JObject label = new JObject(JObject.Parse(Constants.LabelJson));
            foreach (var item in label)
            {
                if (!string.IsNullOrEmpty(item.Value.ToString()))
                {
                    strLabel = item.Key.Equals("PALLETNUMBER") ? strLabel.Replace(item.Key, "P" + pallet.ToString().PadLeft(4, '0')) : strLabel.Replace(item.Key, item.Value.ToString());
                }
            }
            return strLabel;
        }
    }
}
