using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Controllers;
using WOW_Fusion.Models;
using WOW_Fusion.Properties;
using WOW_Fusion.Services;
using WOW_Fusion.Views.Plant2;

namespace WOW_Fusion
{
    public partial class frmSettingsP2 : Form
    {
        private JObject resourcesMfg = null;
        private string resourceId = string.Empty;
        private string workCenterId = string.Empty;

        private string environment = string.Empty;
        private string environmentChanged = string.Empty;

        private bool flagStart = false;
        private string tempFusionUrl = string.Empty;

        public frmSettingsP2()
        { 
            InitializeComponent();
            InitializeFusionData();
        }

        public async void InitializeFusionData()
        {
            //Obtener datos de la máquina
            dynamic resource = await CommonService.OneItem(String.Format(EndPoints.ResourceById, Settings.Default.ResourceId2));

            if (resource != null)
            {
                cmbResources.Items.Clear();
                cmbResources.Items.Add(resource.ResourceName.ToString());
                cmbResources.SelectedIndex = 0;

                dynamic wc = await CommonService.OneItem(String.Format(EndPoints.WorkCenterByResourceId, Settings.Default.ResourceId2));

                if (wc != null)
                {
                    txtBoxWorkCenter.Text = wc.WorkCenterName.ToString();
                }
                else
                {
                    NotifierController.Warning("No se encontro centro de trabajo");
                }
            }
            else
            {
                NotifierController.Warning("No se encontro recurso");
            }
        }

        private void frmSettingsP2_Load(object sender, EventArgs e)
        {
            Console.WriteLine($"Acceso a configuración [{DateService.Today()}]", Color.Black);

            resourceId = Settings.Default.ResourceId2;
            workCenterId = Settings.Default.WorkCenterId;

            txtBoxIpWeighing.Text = Settings.Default.WeighingIP;
            txtBoxPortWeighing.Text = Settings.Default.WeighingPort.ToString();

            txtBoxIpPrinter.Text = Settings.Default.PrinterIP;
            txtBoxPortPrinter.Text = Settings.Default.PrinterPort.ToString();

            txtRoll.Text = Settings.Default.RollToPrint.ToString();
            txtPallet.Text = Settings.Default.PalletToPrint.ToString();

            txtCoreMax.Text = Settings.Default.CoreMaxWeight.ToString();
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

        private async void btnSave_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Verificando datos...";
    
            if(!string.IsNullOrEmpty(cmbResources.Text) && !string.IsNullOrEmpty(txtBoxWorkCenter.Text) &&
                !string.IsNullOrEmpty(txtBoxIpWeighing.Text) && !string.IsNullOrEmpty(txtBoxPortWeighing.Text) &&
                !string.IsNullOrEmpty(txtBoxIpPrinter.Text) && !string.IsNullOrEmpty(txtBoxPortPrinter.Text) &&
                !string.IsNullOrEmpty(txtRoll.Text) && !string.IsNullOrEmpty(txtPallet.Text) &&
                !string.IsNullOrEmpty(txtCoreMax.Text) && !string.IsNullOrEmpty(txtTareMin.Text) && !string.IsNullOrEmpty(txtTareMax.Text) &&
                !string.IsNullOrEmpty(txtUser.Text) && !string.IsNullOrEmpty(txtPassword.Text))
            {
                string credentials = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(txtUser.Text + ":" + txtPassword.Text));
                if (await CommonService.Authenticated(tempFusionUrl, Constants.Plant2Id, credentials))
                {
                    Settings.Default.ResourceId2 = resourceId;
                    Settings.Default.WorkCenterId = workCenterId;

                    Settings.Default.WeighingIP = txtBoxIpWeighing.Text;
                    Settings.Default.WeighingPort = int.Parse(txtBoxPortWeighing.Text);

                    Settings.Default.PrinterIP = txtBoxIpPrinter.Text;
                    Settings.Default.PrinterPort = int.Parse(txtBoxPortPrinter.Text);

                    Settings.Default.RollToPrint = int.Parse(txtRoll.Text);
                    Settings.Default.PalletToPrint = int.Parse(txtPallet.Text);

                    Settings.Default.CoreMaxWeight = float.Parse(txtCoreMax.Text);
                    Settings.Default.TareMinWeight = float.Parse(txtTareMin.Text);
                    Settings.Default.TareMaxWeight = float.Parse(txtTareMax.Text);

                    Settings.Default.FusionUrl = rdbProd.Checked ? "https://iapxqy.fa.ocs.oraclecloud.com/fscmRestApi/resources/11.13.18.05" :
                                  "https://iapxqy-test.fa.ocs.oraclecloud.com/fscmRestApi/resources/11.13.18.05";
                    Settings.Default.ApexUrl = rdbProd.Checked ? "http://129.146.124.5:8080/ords/wow/wo" : "http://129.146.133.180:8080/ords/wow/wo";

                    
                    Settings.Default.Credentials = credentials;

                    Settings.Default.Save();
                    NotifierController.Success("Datos actualizados");

                    if(!environment.Equals(environmentChanged))
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

        private async void cmbWorkCenters_DropDown(object sender, EventArgs e)
        {
            cmbResources.Items.Clear();
            picBoxWaitWC.Visible = true;

            resourcesMfg = await CommonService.ResourcesTypeMachine(Constants.Plant2Id);
            picBoxWaitWC.Visible = false;

            if (resourcesMfg == null) return;

            dynamic items = resourcesMfg["items"];

            foreach (var item in items)
            {
                cmbResources.Items.Add(item.ResourceName.ToString());
            }
        }

        private async void cmbWorkCenters_SelectedValueChanged(object sender, EventArgs e)
        {
            int index = cmbResources.SelectedIndex;

            if (resourcesMfg == null) { return; }

            dynamic resource = resourcesMfg["items"][index]; 

            resourceId = resource.ResourceId.ToString();

            dynamic wc = await CommonService.OneItem(String.Format(EndPoints.WorkCenterByResourceId, resourceId));

            if (wc != null)
            {
                workCenterId = wc.WorkCenterId.ToString();
                txtBoxWorkCenter.Text = wc.WorkCenterName.ToString();
            }
            else
            {
                NotifierController.Warning("No se encontro la centro de trabajo");
            }
        }

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

        private void txtRoll_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtRoll.Text, out _))
            {
                txtRoll.BackColor = Color.White;
                lblStatus.Text = string.Empty;
            }
            else
            {
                txtRoll.BackColor = Color.LightSalmon;
                lblStatus.Text = "Ingrese únicamente números";
            }
        }

        private void txtPallet_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtPallet.Text, out _))
            {
                txtPallet.BackColor = Color.White;
                lblStatus.Text = string.Empty;
            }
            else
            {
                txtPallet.BackColor = Color.LightSalmon;
                lblStatus.Text = "Ingrese únicamente números";
            }
        }

        public event EventHandler FormClosedEvent;
        private void frmSettingsP2_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClosedEvent?.Invoke(this, EventArgs.Empty);

            frmLabelP2 frmLabelP2 = Application.OpenForms.OfType<frmLabelP2>().FirstOrDefault();
            if (frmLabelP2 != null)
            {
                frmLabelP2.InitializeFusionData();
            }
        }

        private void rdbProd_CheckedChanged(object sender, EventArgs e)
        {
            environmentChanged = "PROD";
            tempFusionUrl = "https://iapxqy.fa.ocs.oraclecloud.com/fscmRestApi/resources/11.13.18.05";

            if(flagStart)
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

        private void txtCoreMax_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(txtCoreMax.Text, out _))
            {
                txtCoreMax.BackColor = Color.White;
                lblStatus.Text = string.Empty;
            }
            else
            {
                txtCoreMax.BackColor = Color.LightSalmon;
                lblStatus.Text = "Ingrese únicamente números";
            }
        }

        private void txtTareMin_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(txtCoreMax.Text, out _))
            {
                txtTareMin.BackColor = Color.White;
                lblStatus.Text = string.Empty;
            }
            else
            {
                txtTareMin.BackColor = Color.LightSalmon;
                lblStatus.Text = "Ingrese únicamente números";
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
