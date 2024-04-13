using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace WOW_Fusion.Controllers
{
    class ConsoleController : TextWriter
    {
        private static RichTextBox _output;

        public ConsoleController(RichTextBox output)
        {
            _output = output;
        }

        public override void Write(char value)
        {
            MethodInvoker append = delegate 
            {
                if (value != '\n')
                    _output.AppendText(value.ToString());
            };
            _output.BeginInvoke(append);
        }

        public override void Write(string value, dynamic color)
        {
            MethodInvoker append = delegate 
            {
                _output.SelectionColor = color; 
                _output.AppendText(value.ToString());
                _output.ScrollToCaret();
            };
            _output.BeginInvoke(append);
        }

        public override void WriteLine(string value, dynamic color)
        {
            MethodInvoker append = delegate 
            {
                value = value.Replace("\n", "");
                _output.SelectionColor = color; 
                _output.AppendText("\n" + value.ToString());
                _output.ScrollToCaret();
            };
            _output.BeginInvoke(append);
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
