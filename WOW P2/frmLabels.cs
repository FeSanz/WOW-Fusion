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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WOW_P2
{
    public partial class MainP2 : Form
    {
        PopupNotifier pop = new PopupNotifier();
        Random rnd = new Random();

        API api;
        WeighingController weighing;

        //Fusion parametros
        public static string pylOrganization = string.Empty;
        public static string organizationId = "300000002650049";

        //Datagrid parametros
        private int rollNumber = 0;

        //Pesos params
        private int tareWeight = 0;



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
                Task<string> tskWorkOrdersList = api.GetRequestAsync("/workOrders?limit=500&totalResults=true&onlyData=true&fields=WorkOrderNumber&" +
                                                                    "q=OrganizationId=" + organizationId);
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
            string selectedWO = cmbWorkOrders.SelectedItem.ToString();
            try
            {
                Task<string> tskWorkOrdersData = api.GetRequestAsync("/workOrders?limit=500&totalResults=true&onlyData=true&expand=WorkOrderOperation&" +
                                                                    "fields=WorkOrderId,WorkOrderNumber,WorkOrderStatusCode,ItemNumber,Description,UOMCode;" +
                                                                    "WorkOrderOperation:OperationSequenceNumber,OperationName,PlannedStartDate," +
                                                                    "PlannedCompletionDate,ReadyQuantity,CompletedQuantity&" +
                                                                    "q=OrganizationId="+ organizationId + ";WorkOrderStatusCode=ORA_RELEASED;WorkOrderNumber=" + selectedWO);
                string response = await tskWorkOrdersData;
                if (!string.IsNullOrEmpty(response))
                {
                    var doWorkOrderData = JsonConvert.DeserializeObject<dynamic>(response);

                    lblPlannedQuantity.Text = (string)doWorkOrderData["items"][0]["WorkOrderOperation"][1]["ReadyQuantity"];
                    lblCompletedQuantity.Text = (string)doWorkOrderData["items"][0]["WorkOrderOperation"][1]["CompletedQuantity"];
                    
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

        private async void btnGetWeight_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lblTareWeight.Text))
            {
                //Solicitar peso de tara a bascula
                string responseTare = weighing.SocketWeighing("T");
                if (responseTare.Equals("OK"))
                {
                    string requestTareWeight = weighing.SocketWeighing("OT");
                    lblTareWeight.Text = requestTareWeight;
                    btnGetWeight.Text = "OBTENER";
                    rollNumber = 0;
                }
                else
                {
                    MessageBox.Show(responseTare, "Báscula", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                //Solictar peso de rollos
               
                //int rollWeight = rnd.Next(300,400);

                //Obtener peso neto (Solo peso rollo sin peso tara)
                string net = weighing.SocketWeighing("S");
                
                if (net == "EX")
                {
                    PopupNotification("Tiempo de espera agotado, vuelva a  intentar", null);
                }
                else
                {
                    rollNumber++;
                    //Agregar a datagrid (Rollo, Neto, Bruto)
                    string[] row = new string[] { rollNumber.ToString(), net.ToString(), net.ToString() };
                    dgWeights.Rows.Add(row);

                    if (dgWeights.RowCount == 1)
                    {
                        DataGridViewButtonColumn dgViewButtonPrint = new DataGridViewButtonColumn();
                        {
                            dgViewButtonPrint.HeaderText = "Acción";
                            dgViewButtonPrint.Name = "btnPrintLabel";
                            dgViewButtonPrint.FlatStyle = FlatStyle.Flat;
                            //dgViewButtonPrint.CellTemplate.Style.BackColor = Color.Transparent;
                            //dgViewButtonPrint.DefaultCellStyle.BackColor = Color.Transparent;
                            dgViewButtonPrint.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            dgViewButtonPrint.UseColumnTextForButtonValue = true;
                        }

                        dgWeights.Columns.Add(dgViewButtonPrint);
                    }
                }
                    
                
                /*int palletWeight = dgWeights.Rows.Cast<DataGridViewRow>().Sum(t => Convert.ToInt32(t.Cells[1].Value)) + 
                                   int.Parse(lblTareWeight.Text.Remove(lblTareWeight.Text.Length - 3, 3));
                lblPalletWeight.Text = palletWeight.ToString() + " KG";*/
            }
        }
        private void dgWeight_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            //Columna a colocar icono
            if (e.ColumnIndex == 3)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Resources.printer_01.Width / 20;
                var h = Resources.printer_01.Height / 20;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Resources.printer_01, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        private void btnReloadWO_Click(object sender, EventArgs e)
        {
            RequestWorkOrdersList();
        }

        private void dgWeights_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                DataGridViewRow row = dgWeights.Rows[e.RowIndex];
                string data = row.Cells[0].Value.ToString();
                MessageBox.Show(data);
                //MessageBox.Show(e.RowIndex.ToString() + "," + e.ColumnIndex.ToString());
            }
        }
    }
}
