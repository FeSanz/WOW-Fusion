using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Controllers;
using WOW_Fusion.Properties;

namespace WOW_Fusion
{
    public partial class frmSettingsP1 : Form
    {
        public frmSettingsP1()
        {
            InitializeComponent();
        }

        private void frmSettingsP1_Load(object sender, EventArgs e)
        {
            txtBoxIpWeight.Text = Settings.Default.PrinterIP;
            txtBoxPortWeight.Text = Settings.Default.PrinterPort.ToString();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Verificando conexión...";
            if (await RadwagController.CheckConnection(txtBoxIpWeight.Text, int.Parse(txtBoxPortWeight.Text)))
            {
                Settings.Default.PrinterIP = txtBoxIpWeight.Text;
                Settings.Default.PrinterPort = int.Parse(txtBoxPortWeight.Text);
                Settings.Default.Save();

                NotifierController.Success("Conexión exitosa, datos modificados");
                this.Close();
            }
            else
            {
                lblStatus.Text = "";
                NotifierController.Error("Conexión fallida, verifique datos");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
