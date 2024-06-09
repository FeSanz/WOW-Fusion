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
        private string environment = string.Empty;
        private string environmentChanged = string.Empty;

        public frmSettingsP1()
        {
            InitializeComponent();
        }

        private void frmSettingsP1_Load(object sender, EventArgs e)
        {
            txtBoxIpPrinter.Text = Settings.Default.PrinterIP;
            txtBoxPortPrinter.Text = Settings.Default.PrinterPort.ToString();
            trackBarAdtional.Value = Settings.Default.Aditional;
            lblAditional.Text = Settings.Default.Aditional.ToString() + "%";

            if (Settings.Default.FusionUrl.Contains("-test"))
            {
                rdbTest.Checked = true;
                rdbProd.Checked = false;
                environment = "TEST";
                environmentChanged = "TEST";
            }
            else
            {
                rdbTest.Checked = false;
                rdbProd.Checked = true;
                environment = "PROD";
                environmentChanged = "PROD";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Verificando conexión...";
            if (!string.IsNullOrEmpty(txtBoxIpPrinter.Text) && !string.IsNullOrEmpty(txtBoxPortPrinter.Text))
            {
                Settings.Default.PrinterIP = txtBoxIpPrinter.Text;
                Settings.Default.PrinterPort = int.Parse(txtBoxPortPrinter.Text);
                Settings.Default.Aditional = trackBarAdtional.Value;
                Settings.Default.FusionUrl = rdbProd.Checked ? "https://iapxqy.fa.ocs.oraclecloud.com/fscmRestApi/resources/11.13.18.05" :
                                                               "https://iapxqy-test.fa.ocs.oraclecloud.com/fscmRestApi/resources/11.13.18.05";
                Settings.Default.ApexUrl = rdbProd.Checked ? "http://129.146.124.5:8080/ords/wow/wo" : "http://129.146.133.180:8080/ords/wow/wo";
                Settings.Default.Save();

                NotifierController.Success("Datos actualizados");

                if (!environment.Equals(environmentChanged))
                {
                    MessageBox.Show("Es necesario reiniciar la aplicación al cambiar de ambiente, la aplicación se cerrará automáticamente", "Reinicar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Exit();
                }
                Close();
            }
            else
            {
                lblStatus.Text = "";
                NotifierController.Error("Conexión fallida, verifique datos");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void trackBarAdtional_Scroll(object sender, EventArgs e)
        {
            lblAditional.Text = trackBarAdtional.Value.ToString() + "%";
        }

        private void rdbProd_CheckedChanged(object sender, EventArgs e)
        {
            environmentChanged = "PROD";
        }

        private void rdbTest_CheckedChanged(object sender, EventArgs e)
        {
            environmentChanged = "TEST";
        }
    }
}
