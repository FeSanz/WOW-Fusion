using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tulpep.NotificationWindow;
using WOW_Fusion.Services;

namespace WOW_Fusion
{
    public partial class frmLabelP1 : Form
    {
        PopupNotifier pop = new PopupNotifier();

        APIService api;
        LabelService label;

        LoadingController loading;

        //Fusion parametros
        public static string pylOrganization = string.Empty;
        public static string organizationId = "300000002650034";

        public frmLabelP1()
        {
            InitializeComponent();
        }

        private void frmLabelP1_Load(object sender, EventArgs e)
        {
            api = new APIService();
            loading = new LoadingController();

            RequestOrganizationData();
            RequestWorkOrdersList();
        }

        private async void RequestOrganizationData()
        {
            try
            {
                Task<string> tskOrganizationData = api.GetRequestAsync("/inventoryOrganizations?onlyData=true&limit=500&totalResults=true&fields=OrganizationId,OrganizationCode,OrganizationName,LocationCode&q=OrganizationId=" + organizationId);
                string response = await tskOrganizationData;
                if (!string.IsNullOrEmpty(response))
                {
                    pylOrganization = response;
                    var doOrganizationData = JsonConvert.DeserializeObject<dynamic>(response);
                    lblLocationCode.Text = (string)doOrganizationData["items"][0]["LocationCode"];
                }
                else
                {
                    PopupNotification("Sin respuesta", null);
                }
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
                loading.Show(this);
                Task<string> tskWorkOrdersList = api.GetRequestAsync("/workOrders?limit=500&totalResults=true&onlyData=true&fields=WorkOrderNumber&" +
                                                                    "q=OrganizationId=" + organizationId + " and WorkOrderStatusCode='ORA_RELEASED'");
                string response = await tskWorkOrdersList;
                loading.Close();
                if (!string.IsNullOrEmpty(response))
                {
                    var doWorkOrderList = JsonConvert.DeserializeObject<dynamic>(response);

                    int itemCount = (int)doWorkOrderList["count"];

                    if (itemCount >= 1)
                    {
                        cmbWorkOrders.Items.Clear();

                        for (int i = 0; i < itemCount; i++)
                        {
                            cmbWorkOrders.Items.Add((string)doWorkOrderList["items"][i]["WorkOrderNumber"]);
                        }
                    }
                    else
                    {
                        PopupNotification("Sin ordenes de trabajo", null);
                    }
                }
                else
                {
                    PopupNotification("Sin respuesta", null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void SelectedIndexChangedWorkOrders(object sender, EventArgs e)
        {
            string selectedWO = cmbWorkOrders.SelectedItem.ToString();
            try
            {
                Task<string> tskWorkOrdersData = api.GetRequestAsync("/workOrders?limit=500&totalResults=true&onlyData=true&expand=WorkOrderOperation&" +
                                                                    "fields=WorkOrderId,WorkOrderNumber,WorkOrderStatusCode,ItemNumber,Description,UOMCode;" +
                                                                    "WorkOrderOperation:OperationSequenceNumber,OperationName,PlannedStartDate," +
                                                                    "PlannedCompletionDate,ReadyQuantity,CompletedQuantity&" +
                                                                    "q=OrganizationId=" + organizationId + ";WorkOrderStatusCode=ORA_RELEASED;WorkOrderNumber=" + selectedWO);
                string response = await tskWorkOrdersData;
                if (!string.IsNullOrEmpty(response))
                {
                    var doWorkOrderData = JsonConvert.DeserializeObject<dynamic>(response);

                    lblPlannedQuantity.Text = (string)doWorkOrderData["items"][0]["WorkOrderOperation"][1]["ReadyQuantity"];

                    lblOperation.Text = (string)doWorkOrderData["items"][0]["WorkOrderOperation"][1]["OperationName"];

                    lblPlannedStartDate.Text = (string)doWorkOrderData["items"][0]["WorkOrderOperation"][1]["PlannedStartDate"];
                    lblPlannedCompletionDate.Text = (string)doWorkOrderData["items"][0]["WorkOrderOperation"][1]["PlannedCompletionDate"];

                    //lblMachineCode.Text = (string)doWorkOrderData["items"][0]["WorkOrderOperation"][1]["PlannedStartDate"];
                    //lblMachineName.Text = (string)doWorkOrderData["items"][0]["PlannedStartDate"];

                    lblItemNumber.Text = (string)doWorkOrderData["items"][0]["ItemNumber"];
                    lblItemDescription.Text = (string)doWorkOrderData["items"][0]["Description"];
                    lblUoM.Text = (string)doWorkOrderData["items"][0]["UOMCode"];
                }
                else
                {
                    PopupNotification("No se encontraron datos", null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopupNotification(string content, Image icon)
        {
            pop.ContentText = content;
            pop.ContentColor = Color.Black;
            pop.ContentFont = new Font("Arial", 14);
            pop.ContentPadding = new Padding(10, 15, 10, 5);
            pop.ShowGrip = false;
            pop.HeaderHeight = 1;
            pop.BorderColor = Color.DarkGray; //Color.FromArgb(35, 35, 35);
            pop.Image = icon;
            pop.ImageSize = new Size(70, 70);
            pop.ImagePadding = new Padding(10);
            pop.Size = new Size(350, 90);
            pop.IsRightToLeft = false;
            pop.Popup();
        }

        private void btnReloadWO_Click(object sender, EventArgs e)
        {
            RequestWorkOrdersList();
        }
    }
}
