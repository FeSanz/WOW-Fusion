using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Controllers;
using WOW_Fusion.Models;
using WOW_Fusion.Properties;
using WOW_Fusion.Views.Plant3;

namespace WOW_Fusion.Services
{
    internal class LabelService
    {
        private static string zplTemplate = string.Empty;
        private static string zplPalletTemplate = string.Empty;
        private static TcpClient _client;
        private static NetworkStream _stream;

        public static async Task<dynamic> LabelInfo(string organization, string customer, string item)
        {
            dynamic labels = await CommonService.LabelTamplate(String.Format(EndPoints.LabelTamplate, organization, customer, item));

            if (labels == null) { return null; }

            zplTemplate = labels["LabelZpl"].ToString().Replace("\r\n", String.Empty);
            zplPalletTemplate = labels["LabelPalletZpl"].ToString().Replace("\r\n", String.Empty);

            return labels;
        }

        public static async Task<Stream> UpdateLabelLabelary(int item, string labelType)
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
                    string zpl = labelType.Equals("ROLL") ? ReplaceZplRoll(item) : labelType.Equals("BOX") ? ReplaceZplBox(item) : 
                                 labelType.Equals("SACK") ? ReplaceZplSack(item) : ReplaceZplPallet(item);
                    if (!string.IsNullOrEmpty(zpl))
                    {
                        WebRequest request = WebRequest.Create(String.Format(Constants.LaberalyUrl, zpl));
                        WebResponse response = await request.GetResponseAsync();
                        responseStream = response.GetResponseStream();
                        responseStream = (responseStream != null && responseStream != Stream.Null) ? responseStream : null;
                    }
                    else
                    {
                        responseStream = null;
                    }

                }
                catch (WebException ex)
                {
                    Bitmap image = Resources.empty;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] imageData = memoryStream.ToArray();
                        responseStream = new MemoryStream(imageData);
                    }

                    Console.WriteLine($"Error al procesar previsualización de etiqueta. {ex.Message} [{DateService.Today()}]", Color.Red);
                }
            }

            if (responseStream == null || responseStream == Stream.Null)
            {
                Bitmap image = Resources.empty;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] imageData = memoryStream.ToArray();
                    responseStream = new MemoryStream(imageData);
                    Console.WriteLine($"Previsualización de etiqueta no disponible [{DateService.Today()}]", Color.Black);
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

        public static async Task<bool> PrintP1(int start, int end)
        {
            bool status = false;

            for (int pag = start; pag <= end; pag++)
            {
                try
                {
                    using (TcpClient client = new TcpClient())
                    {
                        await client.ConnectAsync(Settings.Default.PrinterIP, Settings.Default.PrinterPort);

                        if (client.Connected)
                        {
                            using (NetworkStream stream = client.GetStream())
                            {
                                string zpl = ReplaceZplBox(pag);
                                await Task.Delay(500);

                                byte[] data = Encoding.UTF8.GetBytes(zpl);

                                await stream.WriteAsync(data, 0, data.Length);
                                await stream.FlushAsync();

                                frmLabelP1.SetLabelStatusPrint($"Imprimiendo {pag} de {end}");
                                Constants.LastPrint = pag;
                            }

                            status = true;
                        }
                        else
                        {
                            status = false;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NotifierController.DetailError("Error al imprimir", ex.Message);
                    status = false;
                    break;
                }
            }

            return status;
        }

        public static async Task PrintP2(int number, string typee)
        {
            int end = typee.Equals("ROLL") ? Settings.Default.RollToPrint : Settings.Default.PalletToPrint;
            for (int pag = 0; pag < end; pag++)
            {
                try
                {
                    /*_client = new TcpClient();
                    await _client.ConnectAsync(Settings.Default.PrinterIP, Settings.Default.PrinterPort);
                    //_client.Connect(ipPrinter, portPrinter);
                    _stream = _client.GetStream();

                    if (_client.Connected)
                    {
                        string zpl = typee.Equals("ROLL") ? ReplaceZplRoll(number) : ReplaceZplPallet(number);
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
                    }*/
                    using (TcpClient client = new TcpClient())
                    {
                        await client.ConnectAsync(Settings.Default.PrinterIP, Settings.Default.PrinterPort);
                        if (client.Connected)
                        {
                            using (NetworkStream stream = client.GetStream())
                            {
                                string zpl = typee.Equals("ROLL") ? ReplaceZplRoll(number) : ReplaceZplPallet(number);
                                await Task.Delay(1000);
                                byte[] data = Encoding.UTF8.GetBytes(zpl);
                                await stream.WriteAsync(data, 0, data.Length);
                                await stream.FlushAsync();

                                //string t_message = typee.Equals("ROLL") ? $"Imprimiendo etiqueta [rollo] {pag} de {end}" : $"Imprimiendo etiqueta [palet] {pag} de {end}";
                                //frmLabelP2.SetLabelStatusPrint(t_message);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _client.Close();
                    NotifierController.DetailError("Error al imprimir", ex.Message);
                }
            }
        }

        public static async Task<bool> PrintP3(int number, string typee)
        {
            bool status = false;
            int end = typee.Equals("SACK") ? Settings.Default.SackToPrint : 1;

            for (int pag = 0; pag < end; pag++)
            {
                try
                {
                    using (TcpClient client = new TcpClient())
                    {
                        await client.ConnectAsync(Settings.Default.PrinterIP, Settings.Default.PrinterPort);

                        if (client.Connected)
                        {
                            using (NetworkStream stream = client.GetStream())
                            {
                                string zpl = typee.Equals("SACK") ? ReplaceZplSack(number) : ReplaceZplWeighingP3();
                                await Task.Delay(500);

                                byte[] data = Encoding.UTF8.GetBytes(zpl);

                                await stream.WriteAsync(data, 0, data.Length);
                                await stream.FlushAsync();

                                //frmLabelP3.SetLabelStatusPrint($"Imprimiendo {pag} de {end}");
                                //Constants.LastPrint = pag;
                            }

                            status = true;
                        }
                        else
                        {
                            status = false;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NotifierController.DetailError("Error al imprimir", ex.Message);
                    status = false;
                    break;
                }
            }

            return status;
        }
        private static string ReplaceZplBox(int box)
        {
            string strLabel = zplTemplate; //Template sin reemplazos

            JObject label = new JObject(JObject.Parse(Constants.LabelJson));
            foreach (var item in label)
            {
                if (!string.IsNullOrEmpty(item.Value.ToString()))
                    strLabel = item.Key.Equals("BOX") ? strLabel.Replace(item.Key, box.ToString().PadLeft(5, '0')) : strLabel.Replace(item.Key, item.Value.ToString());
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
                    strLabel = item.Key.Equals("ROLL") ? strLabel.Replace(item.Key, "R" + roll.ToString().PadLeft(4, '0')) : strLabel.Replace(item.Key, item.Value.ToString());
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
                    strLabel = item.Key.Equals("PALLET") ? strLabel.Replace(item.Key, "P" + pallet.ToString().PadLeft(4, '0')) : strLabel.Replace(item.Key, item.Value.ToString());
                }
            }
            return strLabel;
        }

        private static string ReplaceZplSack(int sack)
        {
            string strLabel = zplTemplate; //Template sin reemplazos

            JObject label = new JObject(JObject.Parse(Constants.LabelJson));
            foreach (var item in label)
            {
                if (!string.IsNullOrEmpty(item.Value.ToString()))
                {
                    strLabel = item.Key.Equals("SACK") ? strLabel.Replace(item.Key, "S" + sack.ToString().PadLeft(4, '0')) : strLabel.Replace(item.Key, item.Value.ToString());
                }
            }
            return strLabel;
        }

        private static string ReplaceZplWeighingP3()
        {
            string strLabel = zplTemplate; //Template sin reemplazos

            JObject label = new JObject(JObject.Parse(Constants.LabelJson));
            foreach (var item in label)
            {
                if (!string.IsNullOrEmpty(item.Value.ToString()))
                {
                    strLabel = strLabel.Replace(item.Key, item.Value.ToString());
                }
            }
            return strLabel;
        }
    }
}
