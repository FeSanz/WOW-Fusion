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

namespace WOW_Fusion.Services
{
    internal class LabelService
    {
        public static string ipPrinter = "127.0.0.1";
        public static int portPrinter = 9100;
        public static string pathLabelFiles = @"D:\Visual Studio Projects\WOW-Fusion\WOW-Fusion\Resources\Labels\LP1";

        public static Dictionary<string, string> labelDictionary = new Dictionary<string, string>();

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
            Stream responseStream = null;
            string zpl = ReadZplFile(designSelected);
            string pathLabelary = $"http://api.labelary.com/v1/printers/12dpmm/labels/4x2/0/ --data-urlencode { zpl }";
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(pathLabelary);
                var response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
            }
            catch (WebException ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Labelary", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return responseStream;
        }

        public static string[] FilesDesign() 
        {
            List<string> items = new List<string>();

            string[] files = Directory.GetFiles(pathLabelFiles, "*.prn");
            foreach (string file in files)
            {
                items.Add(Path.GetFileNameWithoutExtension(file));
            }
            return items.ToArray();
        }

        public static async Task<bool> Connected()
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(ipPrinter, portPrinter);
                _stream = _client.GetStream();
                if (_client.Connected)
                {
                    _client.Close();
                    await _stream.FlushAsync();
                    _stream.Close();
                    _client.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public static async Task Print(string designSelected)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(ipPrinter, portPrinter); 
                //_client.Connect(ipPrinter, portPrinter);
                _stream = _client.GetStream();

                if (_client.Connected)
                {
                    string zpl = ReadZplFile(designSelected);
                    Thread.Sleep(500);
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
                _stream.Close();
                _client.Close();
                Debug.Print(ex.Message);
            }
        }

       
        private static string ReadZplFile(string designSelected)
        {
            string strLabel = File.ReadAllText($"{pathLabelFiles}\\{designSelected}.prn");

            foreach (string item in lV)
            {
                strLabel = strLabel.Replace(item, labelDictionary[item]);
            }

            return strLabel;
        }
    }
}
