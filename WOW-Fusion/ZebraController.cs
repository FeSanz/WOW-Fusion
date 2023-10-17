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
    internal class ZebraController
    {
        static readonly string pathZPL = @"D:\WoW\Etiquetas\Zebra Designer\P2_Standard.txt";
        private string ip = "192.168.12.10";
        private int port = 80;

        private void ZPLFormatt()
        {
            string strZPLReader = File.ReadAllText(pathZPL);
            strZPLReader = strZPLReader.Replace("ITEM", "");
            strZPLReader = strZPLReader.Replace("DESCRIPTION", "");
            strZPLReader = strZPLReader.Replace("ENGLISH ", "");
            strZPLReader = strZPLReader.Replace("WO", "");
            strZPLReader = strZPLReader.Replace("ROLL", "");
            strZPLReader = strZPLReader.Replace("WBRUTO", "");
            strZPLReader = strZPLReader.Replace("WGROSS", "");
            strZPLReader = strZPLReader.Replace("WNETO", "");
            strZPLReader = strZPLReader.Replace("WNET", "");
            strZPLReader = strZPLReader.Replace("DATE", "");
        }

        private void ZPrinterTCP(string strPrinterLabel)
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(ip, port);
                StreamWriter writer = new StreamWriter(client.GetStream());
                writer.Write(strPrinterLabel);
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
