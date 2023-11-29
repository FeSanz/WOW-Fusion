using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tulpep.NotificationWindow;
using WOW_Fusion.Controllers;
using WOW_Fusion.Models;
using WOW_Fusion.Properties;
using WOW_Fusion.Services;

namespace WOW_Fusion
{
    public partial class frmLabelP2 : Form
    {
        Random rnd = new Random();

        PopController pop;
        //Datagrid parametros
        private int _rollNumber = 0;

        //Pesos params
        private float _tareWeight = 0;
        private float _palletWeight = 0;

        //JObjets response
        private JObject machines = null;

        public frmLabelP2()
        {
            InitializeComponent();
            InitializeFusionData();
        }

        private void frmLabelP2_Load(object sender, EventArgs e)
        {
            pop = new PopController();

            //MessageBox.Show(Settings.Default.RadwagIP);

            /*Settings.Default.RadwagIP = "127.0.0.5";
            Settings.Default.Save();*/

            btnGetWeight.Text = "TARA";
        }

        private async void InitializeFusionData()
        {
            //Obtener datos de Organizacion
            dynamic org = await CommonService.OneItem(String.Format(EndPoints.InventoryOrganizations, Constants.Plant2Id));
            //Obtener datos de centro de trabajo
            dynamic wc = await CommonService.OneItem(String.Format(EndPoints.WorkCentersById, Constants.Plant2Id, Constants.WorkCenterIdP2));

            if (org == null || wc == null)
            {
                AppController.Exit("Sin organización o centro de trabajo, la aplicación se cerrará");
                return;
            }
            else
            {
                lblLocationCode.Text = org["LocationCode"].ToString();
                lblWorkCenterName.Text = wc["WorkCenterName"].ToString();
                lblWorkAreaName.Text = wc["WorkAreaName"].ToString();
            }

            machines = await CommonService.ProductionResourcesMachines(String.Format(EndPoints.ProductionResourcesP2, Constants.Plant2Id)); //Obtener Objeto RECURSOS MAQUINAS 
        }

        private async void DropDownWorkOrders(object sender, EventArgs e)
        {
            cmbWorkOrders.Items.Clear();
            picBoxWaitWO.Visible = true;

            List<string> workOrderNumbers = await CommonService.WorkOrdersByWorkCenter(Constants.Plant2Id, Constants.WorkCenterIdP2); //Obtener OT
            picBoxWaitWO.Visible = false;

            if (workOrderNumbers == null) return;

            foreach (var item in workOrderNumbers)
            {
                cmbWorkOrders.Items.Add(item.ToString());
            }
        }

        private async void SelectedIndexChangedWorkOrders(object sender, EventArgs e)
        {
            try
            {
                Task<string> tskWorkOrdersData = APIService.GetRequestAsync(String.Format(EndPoints.WorkOrderDetail, cmbWorkOrders.SelectedItem.ToString()));
                string response = await tskWorkOrdersData;
                if (string.IsNullOrEmpty(response)) { return; }

                JObject objWorkOrder = JObject.Parse(response);
                dynamic wo = objWorkOrder["items"][0]; //Objeto WORKORDER

                lblPlannedQuantity.Text = wo["PlannedStartQuantity"].ToString();
                lblCompletedQuantity.Text = wo["CompletedQuantity"].ToString();
                lblUoM.Text = wo["UOMCode"].ToString();

                lblItemNumber.Text = wo["ItemNumber"].ToString();
                lblItemDescription.Text = wo["Description"].ToString();
                lblItemDescriptionEnglish.Text = TranslateService.Translate(wo["Description"].ToString());
                lblPlannedStartDate.Text = wo["PlannedStartDate"].ToString();
                lblPlannedCompletionDate.Text = wo["PlannedCompletionDate"].ToString();

                int countResources = (int)wo["WorkOrderResource"]["count"];
                if (countResources >= 1)
                {
                    int indexMachine = -1;

                    for (int i = 0; i < countResources; i++)
                    {
                        for (int j = 0; j < (int)machines["count"]; j++)
                        {
                            string resourceOrder = wo["WorkOrderResource"]["items"][i]["ResourceId"].ToString();
                            string resourceMachines = machines["items"][j]["ResourceId"].ToString();
                            if (resourceOrder.Equals(resourceMachines))
                            {
                                indexMachine = i;
                            }
                        }
                    }
                    if (indexMachine >= 0)
                    {
                        dynamic resource = wo["WorkOrderResource"]["items"][indexMachine]; //Objeto RESURSO
                        lblResourceCode.Text = resource["ResourceCode"].ToString();
                        lblResourceDescription.Text = resource["ResourceDescription"].ToString();

                        if ((int)resource["WorkOrderOperationResourceInstance"]["count"] >= 1)
                        {
                            dynamic instance = resource["WorkOrderOperationResourceInstance"]["items"][0]; // Objeto INSTANCIA
                            lblEquipmentInstanceCode.Text = instance["EquipmentInstanceCode"].ToString();
                            lblEquipmentInstanceName.Text = instance["EquipmentInstanceName"].ToString();
                        }
                        cmbDesignLabels.Enabled = true; 
                    }
                    else
                    {
                        NotifierController.Warning("Datos de máquina no encontrados");
                    }
                }
                else
                {
                    NotifierController.Warning("Orden sin recursos");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error[WO Selected]", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGetWeight_Click(object sender, EventArgs e)
        {
            txtBoxWeight.Text = "";
            pop.Show(this);
            if (string.IsNullOrEmpty(lblPalletTare.Text))
            {
                //Solicitar peso tara
                string responseTare = RadwagController.SocketWeighing("T");
                if (responseTare.Equals("OK"))
                {
                    string requestTareWeight = RadwagController.SocketWeighing("OT");
                    if (!requestTareWeight.Equals("EX"))
                    {
                        pop.Close();
                        _tareWeight = float.Parse(requestTareWeight);
                        lblPalletTare.Text = requestTareWeight;
                        txtBoxWeight.Text = requestTareWeight;
                        btnGetWeight.Text = "OBTENER";
                        _rollNumber = 0;
                    }
                    else
                    {
                        pop.Close();
                        NotifierController.Warning("Tiempo de espera agotado, vuelva a  intentar");
                    }
                }
                else
                {
                    pop.Close();
                    MessageBox.Show(responseTare, "Báscula", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                //Obtiene peso neto acomulado del pallet (sin tara)
                string palletNetWeight = RadwagController.SocketWeighing("S");

                if (palletNetWeight == "EX")
                {
                    pop.Close();
                    NotifierController.Warning("Tiempo de espera agotado, vuelva a  intentar");
                }
                else
                {
                    pop.Close();
                    _rollNumber++;

                    //Calcular pero neto de cada rollo (sin tara)
                    float rollNetKg = float.Parse(palletNetWeight) - _palletWeight;
                    txtBoxWeight.Text = rollNetKg.ToString();

                    //Calcular pero bruto de cada rollo (con tara)
                    float rollGrossKg = rollNetKg + _tareWeight;

                    //Agregar a datagrid (Rollo, Neto, Bruto)
                    string[] row = new string[] { _rollNumber.ToString(), rollNetKg.ToString(), rollGrossKg.ToString() };
                    dgWeights.Rows.Add(row);

                    _palletWeight = float.Parse(palletNetWeight);
                    lblPalletNet.Text = palletNetWeight;
                    lblPalletGross.Text = (_palletWeight + _tareWeight).ToString();

                    if (dgWeights.RowCount == 1)
                    {
                        DataGridViewButtonColumn dgViewButtonPrint = new DataGridViewButtonColumn();
                        {
                            dgViewButtonPrint.HeaderText = "Acción";
                            dgViewButtonPrint.Name = "btnPrintLabel";
                            dgViewButtonPrint.FlatStyle = FlatStyle.Flat;
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

        private void cmbDesignLabels_DropDown(object sender, EventArgs e)
        {
            picBoxWaitLD.Visible = true;
            cmbDesignLabels.Items.Clear();
            cmbDesignLabels.Items.AddRange(LabelService.FilesDesign(Constants.PathLabelsP2));
            picBoxWaitLD.Visible = false;
        }

        private void cmbDesignLabels_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbDesignLabels.SelectedItem != null)
            {
                dynamic label = JObject.Parse(Constants.LabelJson);

                label.WORKORDER = cmbWorkOrders.Text;
                label.ITEMNUMBER = lblItemNumber.Text;
                label.ITEMDESCRIPTION = lblItemDescription.Text;
                label.DESCRIPTIONENGLISH = lblItemDescriptionEnglish.Text;
                label.EQU = lblEquipmentInstanceCode.Text;
                label.DATE = DateService.Now();
                label.ROLLNUMBER = "1".PadLeft(5, '0');
                label.PALLETNUMBER = "1".PadLeft(5, '0');
                label.WNETKG = lblPalletNet.Text;
                label.WNETLBS = lblPalletNet.Text;
                label.WGROSSKG = lblPalletGross.Text;
                label.WGROSSLBS = lblPalletGross.Text;

                Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);

                picLabel.Image = Image.FromStream(LabelService.CreateFromFile(cmbDesignLabels.SelectedItem.ToString(), 2));
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

        private void btnPrintPallet_Click(object sender, EventArgs e)
        {

        }
    }
}
