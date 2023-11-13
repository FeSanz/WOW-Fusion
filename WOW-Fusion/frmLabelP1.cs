using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tulpep.NotificationWindow;
using WOW_Fusion.Services;

namespace WOW_Fusion
{
    public partial class frmLabelP1 : Form
    {
        APIService api;
        LabelService label;

        PopController pop;

        //Fusion parametros
        public static string pylOrganization = string.Empty;
        public static string organizationId = "300000002650034";

        private string pylWorCenters = string.Empty;
        private string workCenterId = string.Empty;

        private string WorkOrderId = string.Empty;

        public frmLabelP1()
        {
            InitializeComponent();
        }

        private void frmLabelP1_Load(object sender, EventArgs e)
        {
            api = new APIService();
            label = new LabelService();
            pop = new PopController();

            picBoxWaitWO.Visible = false;

            RequestOrganizationData();
        }

        private async void RequestOrganizationData()
        {
            try
            {
                Task<string> tskOrganizationData = api.GetRequestAsync("/inventoryOrganizations?limit=500&totalResults=true&onlyData=true&" +
                                                                        "fields=OrganizationId,OrganizationCode,OrganizationName,LocationCode,ManagementBusinessUnitName&" +
                                                                        "q=OrganizationId=" + organizationId);
                string response = await tskOrganizationData;

                if (string.IsNullOrEmpty(response)) 
                {
                    MessageBox.Show("Sin organización la aplicación no funcionará", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Application.Exit();
                }
                
                var doOrganizationData = JsonConvert.DeserializeObject<dynamic>(response);
                int itemCount = (int)doOrganizationData["count"];

                if (itemCount >= 0)
                {
                    pylOrganization = response;
                    lblOrganizationName.Text = doOrganizationData["items"][0]["OrganizationName"].ToString();
                    lblLocationCode.Text = doOrganizationData["items"][0]["LocationCode"].ToString();
                    lblBusinessUnit.Text = doOrganizationData["items"][0]["ManagementBusinessUnitName"].ToString();
                }
                else
                {
                    MessageBox.Show("Sin organización la aplicación no funcionará", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Application.Exit();
                }
                    
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void RequestWorkCenters()
        {
            try
            {
                picBoxWaitWC.Visible = true;
                Task<string> tskWorkCenters = api.GetRequestAsync("/workCenters?limit=500&totalResults=true&onlyData=true&" +
                                                                    "fields=WorkCenterId,WorkCenterName,WorkAreaId,WorkAreaName&" +
                                                                    "q=OrganizationId=" +organizationId);
                pylWorCenters = await tskWorkCenters;
                if (string.IsNullOrEmpty(pylWorCenters)){ return; }

                var doWorCenters = JsonConvert.DeserializeObject<dynamic>(pylWorCenters);

                int itemCount = (int)doWorCenters["count"];

                if (itemCount >= 1)
                {
                    cmbWorkCenters.Items.Clear();

                    for (int i = 0; i < itemCount; i++)
                    {
                        cmbWorkCenters.Items.Add(doWorCenters["items"][i]["WorkCenterName"].ToString());
                    }
                }
                else
                {
                    pop.Notifier("Sin centros de trabajo", Properties.Resources.warning_icon);
                }

                picBoxWaitWC.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void RequestWorkOrdersList()
        {
            try
            {
                picBoxWaitWO.Visible = true;
                Task<string> tskWorkOrdersList = api.GetRequestAsync("/workOrders?limit=500&totalResults=true&onlyData=true&fields=WorkOrderNumber&" +
                                                                    "q=OrganizationId=" + organizationId + " and WorkOrderStatusCode='ORA_RELEASED' " +
                                                                    "and WorkOrderActiveOperation.WorkCenterId=" + workCenterId);
                string response = await tskWorkOrdersList;
                if (string.IsNullOrEmpty(response)) { return; }

                var doWorkOrderList = JsonConvert.DeserializeObject<dynamic>(response);
                int itemCount = (int)doWorkOrderList["count"];

                if (itemCount >= 1)
                {
                    cmbWorkOrders.Items.Clear();

                    for (int i = 0; i < itemCount; i++)
                    {
                        cmbWorkOrders.Items.Add(doWorkOrderList["items"][i]["WorkOrderNumber"].ToString());
                    }
                }
                else
                {
                    pop.Notifier("Sin ordenes de trabajo", Properties.Resources.warning_icon);
                }
                
                picBoxWaitWO.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void RequestResoucesMachines(string WorkOrderId)
        {
            try
            {
                Task<string> tskResorcesMachines = api.GetRequestAsync("/workOrders/"+ WorkOrderId + "/child/WorkOrderResource?limit=500&totalResults=true&onlyData=true&" +
                                                                        "fields=ResourceId,ResourceCode,ResourceName,ResourceDescription&" +
                                                                        "q=ResourceType='EQUIPMENT' and ResourceName NOT LIKE '%MOLDE%'");
                string response = await tskResorcesMachines;
                if (string.IsNullOrEmpty(response)) { return; }

                var doResourcesMachines = JsonConvert.DeserializeObject<dynamic>(response);

                int itemCount = (int)doResourcesMachines["count"];

                if (itemCount >= 1)
                {
                    lblMachineCode.Text = doResourcesMachines["items"][0]["ResourceCode"].ToString();
                    lblMachineName.Text = doResourcesMachines["items"][0]["ResourceDescription"].ToString();
                }
                else
                {
                    pop.Notifier("No se encontrarron máquinas", Properties.Resources.warning_icon);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DropDownOpenWorkCenters(object sender, EventArgs e)
        {
            RequestWorkCenters();
        }

        private void SelectedIndexChangedWorkCenters(object sender, EventArgs e)
        {
            int index = cmbWorkCenters.SelectedIndex;
            var doWorCenters = JsonConvert.DeserializeObject<dynamic>(pylWorCenters);

            lblWorkAreaName.Text = doWorCenters["items"][index]["WorkAreaName"].ToString();
            workCenterId = doWorCenters["items"][index]["WorkCenterId"].ToString();
            cmbWorkOrders.Items.Clear();
            cmbWorkOrders.Enabled = true;
        }

        private void DropDownOpenWorkOrders(object sender, EventArgs e)
        {
            RequestWorkOrdersList();
        }

        private async void SelectedIndexChangedWorkOrders(object sender, EventArgs e)
        {
            string selectedWO = cmbWorkOrders.SelectedItem.ToString();
            try
            {
                Task<string> tskWorkOrdersData = api.GetRequestAsync("/workOrders?limit=500&totalResults=true&onlyData=true&" +
                                                                    "fields=WorkOrderId,ItemNumber,Description,WorkDefinitionCode," +
                                                                    "PlannedStartQuantity,UOMCode,PlannedStartDate,PlannedCompletionDate&" +
                                                                    "q=WorkOrderNumber='"+ selectedWO +"'");
                string response = await tskWorkOrdersData;
                if (string.IsNullOrEmpty(response)) { return; }

                var doWorkOrderData = JsonConvert.DeserializeObject<dynamic>(response);

                RequestResoucesMachines(doWorkOrderData["items"][0]["WorkOrderId"].ToString());

                lblPlannedQuantity.Text = doWorkOrderData["items"][0]["PlannedStartQuantity"].ToString();
                lblUoM.Text = doWorkOrderData["items"][0]["UOMCode"].ToString();

                lblItemNumber.Text = doWorkOrderData["items"][0]["ItemNumber"].ToString();
                lblItemDescription.Text = doWorkOrderData["items"][0]["Description"].ToString();
                lblPlannedStartDate.Text = doWorkOrderData["items"][0]["PlannedStartDate"].ToString();
                lblPlannedCompletionDate.Text = doWorkOrderData["items"][0]["PlannedCompletionDate"].ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReloadWO_Click(object sender, EventArgs e)
        {
            //pictureLabel.Image = Image.FromStream(label.Create());
        }
    }
}
