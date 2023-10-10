using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tulpep.NotificationWindow;
using WOW_P2.Properties;

namespace WOW_P2
{
    public partial class MainP2 : Form
    {
        PopupNotifier pop = new PopupNotifier();
        API api;
        WeighingController weighing;

        public static string pylOrganization = string.Empty;
        public static string organizationId = "300000002650049";

        

        public MainP2()
        {
            InitializeComponent();
        }

        private void MainP2_Load(object sender, EventArgs e)
        {
            lblVersion.Text = "v " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            api = new API();
            weighing = new WeighingController();

            btnGetWeight.Text = "TARA";

            RequestOrganizationData();
            RequestWorkOrdersList();
        }

        private void LoadResourcesInit()
        {
            
        }

        private async void RequestOrganizationData()
        {
            try
            {
                Task<string> tskOrganizationData = api.GetRequestAsync("/inventoryOrganizations?fields=OrganizationId,OrganizationCode,OrganizationName,LocationCode&onlyData=true" +
                                                                     "&limit=500&totalResults=true&q=OrganizationId=" + organizationId);
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
                Task<string> tskWorkOrdersList = api.GetRequestAsync("/workOrders?limit=500&totalResults=true&onlyData=true&fields=WorkOrderNumber&q=OrganizationId=" + organizationId);
                string response = await tskWorkOrdersList;
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
            try
            {
                Task<string> tskWorkOrdersData = api.GetRequestAsync("/workOrders?q=OrganizationCode=" + lblLocationCode.Text + ";WorkOrderStatusCode=RELEASED&limit=500&totalResults=true&onlyData=true");
                string response = await tskWorkOrdersData;
                if (!string.IsNullOrEmpty(response))
                {
                    var doWorkOrderData = JsonConvert.DeserializeObject<dynamic>(response);

                    lblPlannedQuantity.Text = (string)doWorkOrderData["items"][0]["PlannedStartQuantity"];
                    lblCompletedQuantity.Text = (string)doWorkOrderData["items"][0]["CompletedQuantity"];
                    
                    lblOperation.Text = (string)doWorkOrderData["items"][0]["PlannedStartDate"];

                    lblPlannedStartDate.Text = (string)doWorkOrderData["items"][0]["PlannedStartDate"];
                    lblPlannedCompletionDate.Text = (string)doWorkOrderData["items"][0]["PlannedCompletionDate"];

                    lblMachineCode.Text = (string)doWorkOrderData["items"][0]["PlannedStartDate"];
                    lblMachineName.Text = (string)doWorkOrderData["items"][0]["PlannedStartDate"];

                    lblItemNumber.Text = (string)doWorkOrderData["items"][0]["ItemNumber"];
                    lblItemDescription.Text = (string)doWorkOrderData["items"][0]["Description"];
                    lblUoM.Text = (string)doWorkOrderData["items"][0]["UnitOfMeasure"];
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

        private void btnGetWeight_Click(object sender, EventArgs e)
        {
            btnGetWeight.Text = "OBTENER";
            if (string.IsNullOrEmpty(lblTareWeight.Text))
            {
                //Solicitar peso de tara a bascula
                string response = weighing.SocketWeighing("192.168.0.12", 80, btnGetWeight.Text);

                string[] data = response.Split(' ');
                if (data.Length >= 4)
                {
                    if (data[3] == "D")
                    {
                        //response = weighing.SocketWeighing(ipValue, portValue, "Peso Tara");
                        response = weighing.SocketWeighing("192.168.0.12", 80, btnGetWeight.Text);
                        txtResponse.Text = response;
                    }
                    else if (btnCon.Text == "Peso")
                    {
                        txtResponse.Text = response;

                    }
                    else
                    {
                        MessageBox.Show("error vuelve a realizar el peso de la tara");
                    }
                }


                btnCon.Text = btnCon.Text == "Tara" ? "Peso" : "Peso";
            }
            else
            {
                //Solictar peso de rollos
            }
        }

        private void GetWeight()
        {

        }
    }
}
