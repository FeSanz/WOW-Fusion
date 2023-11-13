using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WOW_Fusion.Services
{
    internal class LabelService
    {
        public Stream Create()
        {
            var linesRead = File.ReadLines(@"D:\\WoW\Etiquetas\Zebra Designer\FTP00DL.prn");
            string line = "";
            foreach (var lineRead in linesRead)
            {
                line += lineRead;
            }

            Stream responseStream = null;
            string pathLabelary = $"http://api.labelary.com/v1/printers/12dpmm/labels/4x2/0/ --data-urlencode {line}";
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
    }
}
