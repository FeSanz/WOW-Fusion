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
        public static string organizationId = "300000002650034";

        //Payload response
        private dynamic productionResoucesMachines = null;
        private dynamic worCenters = null;
        private dynamic organization = null;
        private string pylproductionResoucesMachines = string.Empty;

        private string workCenterId = string.Empty;
        private string WorkOrderId = string.Empty;
        private int machinesCount = 0;


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

            RequestOrganization();
        }
        private void Exit(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Application.Exit();
        }

        private async void RequestOrganization()
        {
            try
            {
                Task<string> tskOrganizationData = api.GetRequestAsync("/inventoryOrganizations?limit=500&totalResults=true&onlyData=true&" +
                                                                        "fields=OrganizationId,OrganizationCode,OrganizationName,LocationCode,ManagementBusinessUnitName&" +
                                                                        "q=OrganizationId=" + organizationId);
                string response = await tskOrganizationData;

                if (string.IsNullOrEmpty(response)) 
                {
                    Exit("Sin organización, la aplicación se cerrará");
                }

                organization = JsonConvert.DeserializeObject<dynamic>(response);
                int itemCount = (int)organization["count"];

                if (itemCount >= 0)
                {
                    lblOrganizationCode.Text = organization["items"][0]["OrganizationCode"].ToString();
                    lblOrganizationName.Text = organization["items"][0]["OrganizationName"].ToString();
                    lblLocationCode.Text = organization["items"][0]["LocationCode"].ToString();
                    lblBusinessUnit.Text = organization["items"][0]["ManagementBusinessUnitName"].ToString();

                    RequestProductionResourcesMachines();
                }
                else
                {
                    Exit("Sin organización, la aplicación se cerrará");
                }
                    
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }   

        private async void RequestProductionResourcesMachines()
        {
            try
            {
                Task<string> tskresourcesMachines = api.GetRequestAsync("/productionResources?limit=500&totalResults=true&onlyData=true&fields=ResourceId&" +
                                                                        "q=OrganizationId="+ organizationId +" and ResourceType='EQUIPMENT' and ResourceClassCode='EQU'");
                string response = await tskresourcesMachines;

                if (string.IsNullOrEmpty(response))
                {
                    Exit("Sin recursos de producción, la aplicacion se cerrará");
                }

                productionResoucesMachines = JsonConvert.DeserializeObject<dynamic>(response);
                machinesCount = (int)productionResoucesMachines["count"];

                if (machinesCount == 0)
                {
                    Exit("Sin recursos de producción, la aplicacion se cerrará");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void RequestWorkCenters()
        {

            picBoxWaitWC.Visible = true;
            try
            {
                Task<string> tskWorkCenters = api.GetRequestAsync("/workCenters?limit=500&totalResults=true&onlyData=true&" +
                                                                    "fields=WorkCenterId,WorkCenterName,WorkAreaId,WorkAreaName&" +
                                                                    "q=OrganizationId=" +organizationId);
                string response = await tskWorkCenters;
                if (string.IsNullOrEmpty(response)){ return; }

                worCenters = JsonConvert.DeserializeObject<dynamic>(response);

                int itemCount = (int)worCenters["count"];

                if (itemCount >= 1)
                {
                    cmbWorkCenters.Items.Clear();

                    for (int i = 0; i < itemCount; i++)
                    {
                        cmbWorkCenters.Items.Add(worCenters["items"][i]["WorkCenterName"].ToString());
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

        private void DropDownOpenWorkCenters(object sender, EventArgs e)
        {
            RequestWorkCenters();
        }

        private void SelectedIndexChangedWorkCenters(object sender, EventArgs e)
        {
            int index = cmbWorkCenters.SelectedIndex;

            if(worCenters == null) { return; }

            lblWorkAreaName.Text = worCenters["items"][index]["WorkAreaName"].ToString();
            workCenterId = worCenters["items"][index]["WorkCenterId"].ToString();
            
            cmbWorkOrders.Items.Clear();
            lblItemNumber.Text = string.Empty;
            lblItemDescription.Text = string.Empty;
            lblPlannedQuantity.Text = "0";
            lblUoM.Text = string.Empty;
            lblResourceDescription.Text= string.Empty;
            lblResourceCode.Text= string.Empty;
            lblPlannedStartDate.Text = string.Empty;
            lblPlannedCompletionDate.Text = string.Empty;

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
                Task<string> tskWorkOrdersData = api.GetRequestAsync("/workOrders?limit=500&totalResults=true&onlyData=true&expand=WorkOrderResource.WorkOrderOperationResourceInstance&fields=WorkOrderId,ItemNumber,Description,UOMCode,PlannedStartQuantity,PlannedStartDate,PlannedCompletionDate;WorkOrderResource:ResourceId,ResourceCode,ResourceDescription;WorkOrderResource.WorkOrderOperationResourceInstance:EquipmentInstanceId,EquipmentInstanceCode,EquipmentInstanceName&q=WorkOrderNumber='" + selectedWO +"'");
                string response = await tskWorkOrdersData;
                if (string.IsNullOrEmpty(response)) { return; }

                dynamic doWorkOrder = JsonConvert.DeserializeObject<dynamic>(response);

                //RequestResoucesMachines(doWorkOrderData["items"][0]["WorkOrderId"].ToString());

                lblPlannedQuantity.Text = doWorkOrder["items"][0]["PlannedStartQuantity"].ToString();
                lblUoM.Text = doWorkOrder["items"][0]["UOMCode"].ToString();

                lblItemNumber.Text = doWorkOrder["items"][0]["ItemNumber"].ToString();
                lblItemDescription.Text = doWorkOrder["items"][0]["Description"].ToString();
                lblPlannedStartDate.Text = doWorkOrder["items"][0]["PlannedStartDate"].ToString();
                lblPlannedCompletionDate.Text = doWorkOrder["items"][0]["PlannedCompletionDate"].ToString();

                int countResources = (int)doWorkOrder["items"][0]["WorkOrderResource"]["count"];
                if (countResources >= 1)
                {
                    int indexMachine = -1;

                    for (int i = 0; i < countResources; i++)
                    {
                        for (int j = 0; j < machinesCount; j++)
                        {
                            string resourceIdWO = doWorkOrder["items"][0]["WorkOrderResource"]["items"][i]["ResourceId"].ToString();
                            string resourceIdPR = productionResoucesMachines["items"][j]["ResourceId"].ToString();
                            if (resourceIdWO.Equals(resourceIdPR))
                            {
                                indexMachine = i;
                            }
                        }
                    }
                    if (indexMachine >= 0)
                    {
                        lblResourceCode.Text = doWorkOrder["items"][0]["WorkOrderResource"]["items"][indexMachine]["ResourceCode"].ToString();
                        lblResourceDescription.Text = doWorkOrder["items"][0]["WorkOrderResource"]["items"][indexMachine]["ResourceDescription"].ToString();
                        lblEquipmentInstanceCode.Text = doWorkOrder["items"][0]["WorkOrderResource"]["items"][indexMachine]["WorkOrderOperationResourceInstance"]["items"][0]["EquipmentInstanceCode"].ToString();
                        lblEquipmentInstanceName.Text = doWorkOrder["items"][0]["WorkOrderResource"]["items"][indexMachine]["WorkOrderOperationResourceInstance"]["items"][0]["EquipmentInstanceName"].ToString();
                    }
                    else
                    {
                        pop.Notifier("Datos de máquina no encontrados", Properties.Resources.warning_icon);
                    }
                }
                else
                {
                    pop.Notifier("Orden sin recursos", Properties.Resources.warning_icon);
                }
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
