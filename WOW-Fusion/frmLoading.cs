using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WOW_Fusion
{
    public partial class frmLoading : Form
    {
        public frmLoading()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            //lblLoading.Text = "";
        }

        public frmLoading(Form parent)
        {
            InitializeComponent();
            //lblLoading.Text = "";
            if(parent != null)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(parent.Location.X + parent.Width / 2 - this.Width / 2, parent.Location.Y + parent.Height / 2 - this.Height / 2);
            }
            else
            {
                this.StartPosition= FormStartPosition.CenterParent;
            }
        }

        public void LoadingClose()
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
            if(pcbIcon.Image != null)
            {
                pcbIcon.Image.Dispose();
            }
        }

     
    }
}
