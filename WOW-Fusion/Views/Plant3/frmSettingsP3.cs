using Newtonsoft.Json.Linq;
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
using WOW_Fusion.Models;
using WOW_Fusion.Properties;
using WOW_Fusion.Services;

namespace WOW_Fusion.Views.Plant3
{
    public partial class frmSettingsP3 : Form
    {

        private string environment = string.Empty;
        private string environmentChanged = string.Empty;

        private bool flagStart = false;
        private string tempFusionUrl = string.Empty;

        public event EventHandler FormClosedEvent;

        public frmSettingsP3()
        {
            InitializeComponent();
        }

        private void frmSettingsP3_Load(object sender, EventArgs e)
        {
            Console.WriteLine($"Acceso a configuración [{DateService.Today()}]", Color.Black);

            txtBoxIpPrinter.Text = Settings.Default.PrinterIP;
            txtBoxPortPrinter.Text = Settings.Default.PrinterPort.ToString();

            txtBoxIpWeighing.Text = Settings.Default.WeighingIP;
            txtBoxPortWeighing.Text = Settings.Default.WeighingPort.ToString();

            txtSackToPrint.Text = Settings.Default.SackToPrint.ToString();

            txtSackMax.Text = Settings.Default.SackMaxWeight.ToString();
            txtBagMax.Text = Settings.Default.BagMaxWeight.ToString();
            txtTareMin.Text = Settings.Default.TareMinWeight.ToString();
            txtTareMax.Text = Settings.Default.TareMaxWeight.ToString();

            string decodedCredentials = Encoding.GetEncoding("ISO-8859-1").GetString(Convert.FromBase64String(Settings.Default.Credentials.ToString()));
            txtUser.Text = decodedCredentials.Split(':')[0];
            txtPassword.Text = decodedCredentials.Split(':')[1];

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
            flagStart = true;
        }

        /*--------------------------------- BUTTON ACTION -------------------------------------*/
        private async void btnSave_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Verificando datos...";

            if (!string.IsNullOrEmpty(txtBoxIpPrinter.Text) && !string.IsNullOrEmpty(txtBoxPortPrinter.Text) && 
                !string.IsNullOrEmpty(txtBoxIpWeighing.Text) && !string.IsNullOrEmpty(txtBoxPortWeighing.Text) &&
                !string.IsNullOrEmpty(txtSackToPrint.Text) && !string.IsNullOrEmpty(txtSackMax.Text) &&
                !string.IsNullOrEmpty(txtBagMax.Text) && !string.IsNullOrEmpty(txtTareMin.Text) && !string.IsNullOrEmpty(txtTareMax.Text) &&
                !string.IsNullOrEmpty(txtUser.Text) && !string.IsNullOrEmpty(txtPassword.Text))
            {
                string credentials = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(txtUser.Text + ":" + txtPassword.Text));
                if (await CommonService.Authenticated(tempFusionUrl, Constants.Plant2Id, credentials))
                {
                    Settings.Default.PrinterIP = txtBoxIpPrinter.Text;
                    Settings.Default.PrinterPort = int.Parse(txtBoxPortPrinter.Text);

                    Settings.Default.WeighingIP = txtBoxIpWeighing.Text;
                    Settings.Default.WeighingPort = int.Parse(txtBoxPortWeighing.Text);

                    Settings.Default.SackToPrint = int.Parse(txtSackToPrint.Text);
                    Settings.Default.SackMaxWeight = float.Parse(txtSackMax.Text);
                    Settings.Default.BagMaxWeight = float.Parse(txtBagMax.Text);
                    Settings.Default.TareMinWeight = float.Parse(txtTareMin.Text);
                    Settings.Default.TareMaxWeight = float.Parse(txtTareMax.Text);

                    Settings.Default.FusionUrl = rdbProd.Checked ? "https://iapxqy.fa.ocs.oraclecloud.com/fscmRestApi/resources/11.13.18.05" :
                                  "https://iapxqy-test.fa.ocs.oraclecloud.com/fscmRestApi/resources/11.13.18.05";
                    Settings.Default.ApexUrl = rdbProd.Checked ? "http://129.146.124.5:8080/ords/wow/wo" : "http://129.146.133.180:8080/ords/wow/wo";


                    Settings.Default.Credentials = credentials;

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
                    lblStatus.Text = "Acceso no autorizado, verifique credenciales";
                }

            }
            else
            {
                lblStatus.Text = "";
                NotifierController.Warning("Llene todos los campos");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /*--------------------------------- RADIO CHANGED -------------------------------------*/
        private void rdbProd_CheckedChanged(object sender, EventArgs e)
        {
            environmentChanged = "PROD";
            tempFusionUrl = "https://iapxqy.fa.ocs.oraclecloud.com/fscmRestApi/resources/11.13.18.05";

            if (flagStart)
            {
                txtUser.Text = string.Empty;
                txtPassword.Text = string.Empty;
            }
        }

        private void rdbTest_CheckedChanged(object sender, EventArgs e)
        {
            environmentChanged = "TEST";
            tempFusionUrl = "https://iapxqy-test.fa.ocs.oraclecloud.com/fscmRestApi/resources/11.13.18.05";

            if (flagStart)
            {
                txtUser.Text = string.Empty;
                txtPassword.Text = string.Empty;
            }
        }

        /*--------------------------------- TEXT CHANGED -------------------------------------*/
        private void txtBoxPortWeighing_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtBoxPortWeighing.Text, out _))
            {
                txtBoxPortWeighing.BackColor = Color.White;
                lblStatus.Text = string.Empty;
            }
            else
            {
                txtBoxPortWeighing.BackColor = Color.LightSalmon;
                lblStatus.Text = "Ingrese únicamente números";
            }
        }

        private void txtBoxPortPrinter_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtBoxPortPrinter.Text, out _))
            {
                txtBoxPortPrinter.BackColor = Color.White;
                lblStatus.Text = string.Empty;
            }
            else
            {
                txtBoxPortPrinter.BackColor = Color.LightSalmon;
                lblStatus.Text = "Ingrese únicamente números";
            }
        }

        private void txtSacktoPrint_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtSackToPrint.Text, out _))
            {
                txtSackToPrint.BackColor = Color.White;
                lblStatus.Text = string.Empty;
            }
            else
            {
                txtSackToPrint.BackColor = Color.LightSalmon;
                lblStatus.Text = "Ingrese únicamente números enteros";
            }
        }

        private void txtSackMax_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(txtSackMax.Text, out _))
            {
                txtSackMax.BackColor = Color.White;
                lblStatus.Text = string.Empty;
            }
            else
            {
                txtSackMax.BackColor = Color.LightSalmon;
                lblStatus.Text = "Ingrese únicamente números";
            }
        }

        private void txtBagMax_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(txtBagMax.Text, out _))
            {
                txtBagMax.BackColor = Color.White;
                lblStatus.Text = string.Empty;
            }
            else
            {
                txtBagMax.BackColor = Color.LightSalmon;
                lblStatus.Text = "Ingrese únicamente números";
            }
        }

        private void txtTareMin_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(txtTareMin.Text, out _))
            {
                txtTareMin.BackColor = Color.White;
                lblStatus.Text = string.Empty;
            }
            else
            {
                txtTareMin.BackColor = Color.LightSalmon;
            }
        }

        private void txtTareMax_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(txtTareMax.Text, out _))
            {
                txtTareMax.BackColor = Color.White;
                lblStatus.Text = string.Empty;
            }
            else
            {
                txtTareMax.BackColor = Color.LightSalmon;
                lblStatus.Text = "Ingrese únicamente números";
            }
        }
    }
}
