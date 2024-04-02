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
        private JObject workCenters = null;
        private string workCenterId = string.Empty;

        public frmSettingsP2()
        { 
            InitializeComponent();
            InitializeFusionData();
        }

        private async void InitializeFusionData()
        {
            //Obtener datos de centro de trabajo
            /*dynamic wc = await CommonService.OneItem(String.Format(EndPoints.WorkCentersById, Constants.Plant2Id, Settings.Default.WorkCenterP2));

            if (wc != null)
            {
                cmbWorkCenters.Items.Clear();
                cmbWorkCenters.Items.Add(wc["WorkCenterName"].ToString());
                cmbWorkCenters.SelectedIndex = 0;
                txtBoxArea.Text = wc["WorkAreaName"].ToString();
            }
            else
            {
                NotifierController.Warning("No se encontraron centros de trabajo");
            }*/

            cmbWorkCenters.Items.Clear();
            cmbWorkCenters.Items.Add("LAMINADORA L01");
            cmbWorkCenters.SelectedIndex = 0;
            txtBoxArea.Text = "LAMINADORA L01";
        }

        private void frmSettingsP2_Load(object sender, EventArgs e)
        {
            workCenterId = Settings.Default.WorkCenterP2;

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
    
            if(!string.IsNullOrEmpty(cmbWorkCenters.Text) && !string.IsNullOrEmpty(txtBoxArea.Text) &&
                !string.IsNullOrEmpty(txtBoxIpWeighing.Text) && !string.IsNullOrEmpty(txtBoxPortWeighing.Text) &&
                !string.IsNullOrEmpty(txtBoxIpPrinter.Text) && !string.IsNullOrEmpty(txtBoxPortPrinter.Text) &&
                !string.IsNullOrEmpty(txtRoll.Text) && !string.IsNullOrEmpty(txtPallet.Text))
            {
                Settings.Default.WorkCenterP2 = workCenterId;

                Settings.Default.WeighingIP = txtBoxIpWeighing.Text;
                Settings.Default.WeighingPort = int.Parse(txtBoxPortWeighing.Text);

                Settings.Default.PrinterIP = txtBoxIpPrinter.Text;
                Settings.Default.PrinterPort = int.Parse(txtBoxPortPrinter.Text);

                Settings.Default.RollToPrint = int.Parse(txtRoll.Text);
                Settings.Default.PalletToPrint = int.Parse(txtPallet.Text);

                Settings.Default.Save();

                NotifierController.Success("Datos actualizados");
                this.Close();
            }
            else
            {
                lblStatus.Text = "";
                NotifierController.Warning("Llene todos los campos");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void cmbWorkCenters_DropDown(object sender, EventArgs e)
        {
            cmbWorkCenters.Items.Clear();
            picBoxWaitWC.Visible = true;

            workCenters = await CommonService.WorkCenters(Constants.Plant2Id); //Obtener Objeto CENTROS DE TRABAJO
            picBoxWaitWC.Visible = false;

            if (workCenters == null) return;

            dynamic items = workCenters["items"];

            foreach (var item in items)
            {
                cmbWorkCenters.Items.Add(item["WorkCenterName"].ToString());
            }
        }

        private void cmbWorkCenters_SelectedValueChanged(object sender, EventArgs e)
        {
            int index = cmbWorkCenters.SelectedIndex;

            if (workCenters == null) { return; }

            dynamic ct = workCenters["items"][index]; //Objeto CENTROS DE TRABAJO

            txtBoxArea.Text = ct["WorkAreaName"].ToString();
            workCenterId = ct["WorkCenterId"].ToString();
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
    }
}
