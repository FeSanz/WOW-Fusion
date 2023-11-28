using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
        public static Dictionary<string, string> labelDictionary = new Dictionary<string, string>();
        private static string zplTemplate = string.Empty;
        private static TcpClient _client;
        private static NetworkStream _stream;

        public static string[] lV = { 
                                            "WORKORDER",
                                            "ITEMNUMBER",
                                            "ITEMDESCRIPTION",
                                            "DESCRIPTIONENGLISH",
                                            "EQU",
                                            "DATE",
                                            "BOXNUMBER"
                                    };
        public static Stream CreateFromFile(string designSelected)
        {            
            Stream responseStream;
            zplTemplate = File.ReadAllText($"{Constants.PathLabelsP1}\\{designSelected}.prn");

            string zpl = ReplaceZPL(1);

            //string pathLabelary = $"http://api.labelary.com/v1/printers/12dpmm/labels/4x2/0/ --data-urlencode {zpl}";
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

        public static string[] FilesDesign() 
        {
            List<string> items = new List<string>();

            string[] files = Directory.GetFiles(Constants.PathLabelsP1, "*.prn");
            foreach (string file in files)
            {
                items.Add(Path.GetFileNameWithoutExtension(file));
            }
            return items.ToArray();
        }

        public static async Task Print(int quantity)
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
                        string zpl = ReplaceZPL(i);
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
       
        private static string ReplaceZPL(int box)
        {
            string strLabel = zplTemplate;
            foreach (string item in lV)
            {
                strLabel = item.Equals("BOXNUMBER") ? strLabel.Replace(item, box.ToString().PadLeft(5, '0')) : strLabel.Replace(item, labelDictionary[item]);
            }
            return strLabel;
        }
    }
}
