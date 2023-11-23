using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
            string strLabel = File.ReadAllText($"{pathLabelFiles}\\{designSelected}.prn");

            foreach ( string item in lV )
            {
                strLabel = strLabel.Replace(item, labelDictionary[item]);
            }

            Stream responseStream = null;
            string pathLabelary = $"http://api.labelary.com/v1/printers/12dpmm/labels/4x2/0/ --data-urlencode {strLabel}";
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

        public static void Print(string zpl)
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(ipPrinter, portPrinter);
                StreamWriter writer = new StreamWriter(client.GetStream());
                writer.Write(zpl);
                writer.Flush();
                writer.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error al imprimir", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
