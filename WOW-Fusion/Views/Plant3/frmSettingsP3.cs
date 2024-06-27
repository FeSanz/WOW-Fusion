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
        private JObject resourcesMfg = null;
        private string resourceId = string.Empty;
        private string workCenterId = string.Empty;

        private string environment = string.Empty;
        private string environmentChanged = string.Empty;

        public frmSettingsP3()
        {
            InitializeComponent();
            InitializeFusionData();
        }

        public async void InitializeFusionData()
        {
            //Obtener datos de la máquina
            dynamic resource = await CommonService.OneItem(String.Format(EndPoints.ResourceById, Settings.Default.ResourceId3));

            if (resource != null)
            {
                cmbResources.Items.Clear();
                cmbResources.Items.Add(resource.ResourceName.ToString());
                cmbResources.SelectedIndex = 0;

                dynamic wc = await CommonService.OneItem(String.Format(EndPoints.WorkCenterByResourceId, Settings.Default.ResourceId3));

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

        private void frmSettingsP3_Load(object sender, EventArgs e)
        {
            Console.WriteLine($"Acceso a configuración [{DateService.Today()}]", Color.Black);

            resourceId = Settings.Default.ResourceId3;
            workCenterId = Settings.Default.WorkCenterId;

            txtBoxIpWeighing.Text = Settings.Default.WeighingIP;
            txtBoxPortWeighing.Text = Settings.Default.WeighingPort.ToString();

            txtBoxIpPrinter.Text = Settings.Default.PrinterIP;
            txtBoxPortPrinter.Text = Settings.Default.PrinterPort.ToString();

            txtBagToPrint.Text = Settings.Default.BagToPrint.ToString();
            txtWeightStd.Text = Settings.Default.WeightStdP3.ToString();

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
            lblStatus.Text = "Verificando datos...";

            if (!string.IsNullOrEmpty(cmbResources.Text) && !string.IsNullOrEmpty(txtBoxWorkCenter.Text) &&
                !string.IsNullOrEmpty(txtBoxIpWeighing.Text) && !string.IsNullOrEmpty(txtBoxPortWeighing.Text) &&
                !string.IsNullOrEmpty(txtBoxIpPrinter.Text) && !string.IsNullOrEmpty(txtBoxPortPrinter.Text) &&
                !string.IsNullOrEmpty(txtBagToPrint.Text) && !string.IsNullOrEmpty(txtWeightStd.Text))
            {
                Settings.Default.ResourceId3 = resourceId;
                Settings.Default.WorkCenterId = workCenterId;

                Settings.Default.WeighingIP = txtBoxIpWeighing.Text;
                Settings.Default.WeighingPort = int.Parse(txtBoxPortWeighing.Text);

                Settings.Default.PrinterIP = txtBoxIpPrinter.Text;
                Settings.Default.PrinterPort = int.Parse(txtBoxPortPrinter.Text);

                Settings.Default.BagToPrint = int.Parse(txtBagToPrint.Text);
                Settings.Default.WeightStdP3 = int.Parse(txtWeightStd.Text);

                Settings.Default.FusionUrl = rdbProd.Checked ? "https://iapxqy.fa.ocs.oraclecloud.com/fscmRestApi/resources/11.13.18.05" :
                                                               "https://iapxqy-test.fa.ocs.oraclecloud.com/fscmRestApi/resources/11.13.18.05";

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

            resourcesMfg = await CommonService.ResourcesTypeMachine(Constants.Plant3Id);
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

        private void txtBagtoPrint_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtBagToPrint.Text, out _))
            {
                txtBagToPrint.BackColor = Color.White;
                lblStatus.Text = string.Empty;
            }
            else
            {
                txtBagToPrint.BackColor = Color.LightSalmon;
                lblStatus.Text = "Ingrese únicamente números enteros";
            }
        }

        private void txtWeightStd_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtWeightStd.Text, out _))
            {
                txtWeightStd.BackColor = Color.White;
                lblStatus.Text = string.Empty;
            }
            else
            {
                txtWeightStd.BackColor = Color.LightSalmon;
                lblStatus.Text = "Ingrese únicamente números enteros";
            }
        }

        public event EventHandler FormClosedEvent;
        private void frmSettingsP3_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClosedEvent?.Invoke(this, EventArgs.Empty);

            frmLabelP3 frmLabelP3 = Application.OpenForms.OfType<frmLabelP3>().FirstOrDefault();
            if (frmLabelP3 != null)
            {
                //frmLabelP3.InitializeFusionData();
            }
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
