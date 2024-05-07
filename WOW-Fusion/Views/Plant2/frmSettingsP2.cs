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

namespace WOW_Fusion
{
    public partial class frmSettingsP2 : Form
    {
        private JObject resourcesMfg = null;
        private string resourceId = string.Empty;
        private string workCenterId = string.Empty;

        public frmSettingsP2()
        { 
            InitializeComponent();
            InitializeFusionData();
        }

        public async void InitializeFusionData()
        {
            //Obtener datos de la máquina
            dynamic resource = await CommonService.OneItem(String.Format(EndPoints.ResourceById, Settings.Default.ResourceId));

            if (resource != null)
            {
                cmbResources.Items.Clear();
                cmbResources.Items.Add(resource.ResourceName.ToString());
                cmbResources.SelectedIndex = 0;

                dynamic wc = await CommonService.OneItem(String.Format(EndPoints.WorkCenterByResourceId, Settings.Default.ResourceId));

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
            Console.WriteLine($"{DateService.Today()} > Acceso a configuración", Color.Black);

            resourceId = Settings.Default.ResourceId;
            workCenterId = Settings.Default.WorkCenterId;

            txtBoxIpWeighing.Text = Settings.Default.WeighingIP;
            txtBoxPortWeighing.Text = Settings.Default.WeighingPort.ToString();

            txtBoxIpPrinter.Text = Settings.Default.PrinterIP;
            txtBoxPortPrinter.Text = Settings.Default.PrinterPort.ToString();

            txtRoll.Text = Settings.Default.RollToPrint.ToString();
            txtPallet.Text = Settings.Default.PalletToPrint.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Verificando datos...";
    
            if(!string.IsNullOrEmpty(cmbResources.Text) && !string.IsNullOrEmpty(txtBoxWorkCenter.Text) &&
                !string.IsNullOrEmpty(txtBoxIpWeighing.Text) && !string.IsNullOrEmpty(txtBoxPortWeighing.Text) &&
                !string.IsNullOrEmpty(txtBoxIpPrinter.Text) && !string.IsNullOrEmpty(txtBoxPortPrinter.Text) &&
                !string.IsNullOrEmpty(txtRoll.Text) && !string.IsNullOrEmpty(txtPallet.Text))
            {
                Settings.Default.ResourceId = resourceId;
                Settings.Default.WorkCenterId = workCenterId;

                Settings.Default.WeighingIP = txtBoxIpWeighing.Text;
                Settings.Default.WeighingPort = int.Parse(txtBoxPortWeighing.Text);

                Settings.Default.PrinterIP = txtBoxIpPrinter.Text;
                Settings.Default.PrinterPort = int.Parse(txtBoxPortPrinter.Text);

                Settings.Default.RollToPrint = int.Parse(txtRoll.Text);
                Settings.Default.PalletToPrint = int.Parse(txtPallet.Text);

                Settings.Default.Save();

                NotifierController.Success("Datos actualizados");

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
    }
}
