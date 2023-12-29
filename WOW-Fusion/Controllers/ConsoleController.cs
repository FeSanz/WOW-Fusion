using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WOW_Fusion.Controllers
{
    class ConsoleController : TextWriter
    {
        private Control _control;
        public ConsoleController(Control control)
        {
            _control = control;
        }

        public override void Write(char value)
        {
            _control.Text += value;
        }

        public override void Write(string value)
        {
            _control.Text += value;
        }

        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }
    }
}
