using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using WOW_Fusion.Controllers;
using WOW_Fusion.Models;
using WOW_Fusion.Properties;
using WOW_Fusion.Services;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms.Design;
using Google.Apis.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using WOW_Fusion.Views.Plant2;
using static System.Net.Mime.MediaTypeNames;
using Google.Api.Gax;
using System.Data.SqlClient;
using static Google.Apis.Requests.BatchRequest;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace WOW_Fusion
{
    public partial class frmLabelP2 : Form
    {
        #region Variables Close
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        private const uint SC_CLOSE = 0xF060;
        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint MF_GRAYED = 0x00000001;
        private const uint MF_ENABLED = 0x00000000;
        private const uint MF_DISABLED = 0x00000002;
        #endregion

        Random rnd = new Random();

        //Pesos params
        private float _tareWeight = 0.0f;
        private float _weightFromWeighing = 0.0f;
        private float _netPallet = 0.0f;
        private float _previousWeight = 0.0f;
        //Ancho y espesor
        private string strWithThickness = string.Empty;
        private string _akaCustomer = "DEFAULT";
        private string _tradingPartnerName = " ";

        private int _rowSelected = 0;

        //Rollos pallet control
        private int _rollCount = 0;
        private int _rollByPallet = 0;
        private int _palletCount = 0;
        private bool _isPalletStart = false;
        private bool _endWeight = false;
        private bool _completedHistory = false;
        
        private bool _reloadTare = false;

        //APEX Flags
        private string _lastApexCreate = string.Empty;
        private bool _apexCreated = false;
        private string _lastApexUpdate = string.Empty;
        private bool _apexUpdated = false;

        //JObjets response
        private dynamic shifts = null;

        //Scheduling
        List<WorkOrderShedule> ordersForSchedule;
        private bool _startOrder = false;
        private bool _startThreadSchedule = false;

        //Hide-Reserve data
        private Int64 _workOrderId = 0;
        private string _workOrderNumber = string.Empty;
        private string _CustomerPONumber = string.Empty;

        private static frmLabelP2 instance;
        /*------------------------------ INITIALIZE ----------------------------------*/
        #region Start
        public frmLabelP2()
        {
            InitializeComponent();
            InitializeFusionData();
        }

        private void frmLabelP2_Load(object sender, EventArgs e)
        {
            lblEnvironment.Text = Settings.Default.FusionUrl.Contains("-test") ? "TEST" : "PROD";
            lblStatusProcess.Text = string.Empty;

            richTextConsole.Clear();
            Console.SetOut(new ConsoleController(richTextConsole));

            AppController.ToolTip(btnSettings, "Configuración");
            AppController.ToolTip(pbRed, "Peso debajo del estándar");
            AppController.ToolTip(pbYellow, "Peso encima del estándar");

            AppController.ToolTip(btnEndProcess, "Terminar de pesar orden");
            AppController.ToolTip(btnSwapMode, "Cambiar modo");

            AppController.ToolTip(btnReloadTare, "Volver a pesar tara");
            AppController.ToolTip(btnReloadCore, "Volver a pesar core");

            TipStatusWO.SetToolTip(lblWOStatus, "Status orden");

            btnGetWeight.Text = "TARAR";
            btnGetWeight.Enabled = false;
        }

        public async void InitializeFusionData()
        {
            timerShift.Stop();
            lblEnvironment.Text = Settings.Default.FusionUrl.Contains("-test") ? "TEST" : "PROD";

            List<string> endPoints = new List<string>
            {
                String.Format(BatchPoints.Organizations, Constants.Plant2Id),
                String.Format(BatchPoints.ResourceById, Settings.Default.ResourceId2),
                String.Format(BatchPoints.WorkCenterByResourceId, Settings.Default.ResourceId2),
                String.Format(BatchPoints.ShiftByWorkCenter, Settings.Default.WorkCenterId)
            };

            Task<string> batchTsk = APIService.PostBatchRequestAsync(Batchs.BatchPayload(endPoints));
            string batchResponse = await batchTsk;
            if (!string.IsNullOrEmpty(batchResponse))
            {
                JObject obj = JObject.Parse(batchResponse);

                if ((int)obj["parts"][0]["payload"]["count"] > 0 && 
                    (int)obj["parts"][1]["payload"]["count"] > 0 && 
                    (int)obj["parts"][2]["payload"]["count"] > 0) 
                {
                    dynamic org = obj["parts"][0]["payload"]["items"][0];
                    dynamic rs = obj["parts"][1]["payload"]["items"][0];
                    dynamic wc = obj["parts"][2]["payload"]["items"][0];
                    shifts = obj["parts"][3]["payload"]["items"][0];

                    Constants.BusinessUnitId = org.ManagementBusinessUnitId.ToString();

                    lblLocationCode.Text = org.LocationCode.ToString();
                    lblResourceCode.Text = rs.ResourceCode.ToString();
                    lblResourceName.Text = rs.ResourceName.ToString();
                    lblWorkCenterName.Text = wc.WorkCenterName.ToString();

                    //Verificar e iniciar hilo de TURNO 
                    lblShift.Text = (shifts == null) ? string.Empty : DateService.CurrentShift(shifts, Settings.Default.ResourceId2);
                    timerShift.Tick += new EventHandler(CheckShift);
                    timerShift.Start();

                    Refresh();
                }
                else
                {
                    Console.WriteLine($"Sin organización, recurso o centro de trabajo, la aplicación no funcionará adecuadamente [{DateService.Today()}]", Color.Red);
                    return;
                }

            }
            else
            {
                Console.WriteLine($"Sin organización, recurso o centro de trabajo, la aplicación no funcionará correctamente [{DateService.Today()}]", Color.Red);
                return;
            }

            ProductionScheduling(this, EventArgs.Empty);
        }
        #endregion

        #region Scheduling
        private async void ProductionScheduling(object sender, EventArgs e)
        {
            lblWOStatus.Visible = false;
            ShowWait(false);
            ordersForSchedule = await CommonService.WOProcessSchedule(Constants.Plant2Id, Settings.Default.ResourceId2); //Obtener OT Schedule
           
            if (ordersForSchedule.Count > 0)
            {
                List<WorkOrderShedule> schedule = CommonService.OrderByPriority(ordersForSchedule, "PlannedStartDate");

                /*List<string> ordersPrinted = FileController.ContentFile(Constants.OrdersPrintedP2);

                for (int i = 0; i < schedule.Count; i++)
                {
                    if (FileController.IsOrderPrinted(ordersPrinted, schedule[i].WorkOrderNumber))
                    {
                        //Quitar ordenes ya impresas
                        schedule.RemoveAt(i);
                        i--;
                    }
                }*/
                
                /*if (cmbWorkOrders.Text != schedule[0].WorkOrderNumber)
                {
                    cmbWorkOrders.Items.Clear();
                    cmbWorkOrders.Items.Add(schedule[0].WorkOrderNumber);
                    cmbWorkOrders.SelectedIndex = 0;
                }*/
                
                foreach (var wo in schedule)
                {
                    if (DateService.IsBetweenDates(wo.PlannedStartDate, wo.PlannedCompletionDate))
                    {
                        if (cmbWorkOrders.Text.Equals(wo.WorkOrderNumber.ToString()))
                        {
                            break;
                        }
                        else
                        {
                            cmbWorkOrders.Items.Clear();
                            cmbWorkOrders.Items.Add(wo.WorkOrderNumber.ToString());
                            cmbWorkOrders.Text = wo.WorkOrderNumber.ToString();
                            break;
                        }

                        /*cmbWorkOrders.Items.Clear();
                        cmbWorkOrders.Items.Add(wo.WorkOrderNumber.ToString());
                        cmbWorkOrders.Text = wo.WorkOrderNumber.ToString();
                        break;*/
                    }
                }
            }
            else
            {
                NotifierController.Warning("Sin ordenes de trabajo");
            }

            lblWOStatus.Visible = true;

            if (lblMode.Text.Equals("Auto.") && !timerSchedule.Enabled)
            {
                timerSchedule.Tick += new EventHandler(ProductionScheduling);
                timerSchedule.Start();
            }
            else if (lblMode.Text.Equals("Manual") && timerSchedule.Enabled)
            {
                timerSchedule.Stop();   
            }
        }

        private void CheckStatusScheduleOrder(DateTime start, DateTime end)
        {
            DateTimeOffset now = DateTimeOffset.Now;
            if (DateService.IsBetweenDates(start, end))
            {
                lblWOStatus.ForeColor = Color.Green;;
                TipStatusWO.SetToolTip(lblWOStatus, "Orden en tiempo");
            }
            else
            {
                if (start < now)
                {
                    lblWOStatus.ForeColor = Color.Red;
                    TipStatusWO.SetToolTip(lblWOStatus, "Orden atrazada");
                }
                else if (start > now)
                {
                    lblWOStatus.ForeColor = Color.Yellow;
                    TipStatusWO.SetToolTip(lblWOStatus, "Orden adelantada");
                }
                else
                {
                    lblWOStatus.ForeColor = Color.DarkGray;
                    TipStatusWO.SetToolTip(lblWOStatus, "Status orden");
                }
            }
        }

        private void CheckShift(object sender, EventArgs e)
        {
            if(shifts != null && !string.IsNullOrEmpty(Settings.Default.ResourceId2))
            {
                lblShift.Text = (shifts == null) ? string.Empty : DateService.CurrentShift(shifts, Settings.Default.ResourceId2);
            }
        }
        #endregion

        /*------------------------------ WORKORDERS ----------------------------------*/
        #region WorkOrders
        private async void DropDownWorkOrders(object sender, EventArgs e)
        {
            cmbWorkOrders.Items.Clear();
            picBoxWaitWO.Visible = true;

            List<string> workOrderNumbers = await CommonService.WOProcessByResource(Constants.Plant2Id, Settings.Default.ResourceId2); //Obtener OT

            //-------------------------- Ordenar --------------------
            List<int> workOrderNumberSort = new List<int>();
            foreach (string order in workOrderNumbers)
            {
                if (int.TryParse(order, out int orderNumber)) { workOrderNumberSort.Add(orderNumber); }
            }
            workOrderNumberSort.Sort();
            //-------------------------- Ordenado -------------------

            picBoxWaitWO.Visible = false;

            if (workOrderNumbers == null) return;

            //List<string> ordersPrinted = FileController.ContentFile(Constants.OrdersPrintedP2);

            foreach (int order in workOrderNumberSort)
            {
                cmbWorkOrders.Items.Add(order.ToString());
                //if (!FileController.IsOrderPrinted(ordersPrinted, order.ToString())) { cmbWorkOrders.Items.Add(order.ToString()); }

            }

            /*foreach (var item in workOrderNumbers)
            {
                cmbWorkOrders.Items.Add(item.ToString());
            }*/
        }

        private void SelectedIndexChangedWorkOrders(object sender, EventArgs e)
        {
            ShowWait(false);

            //lblLabelName.Text = string.Empty;
            //lblAkaOrder.Text = string.Empty;
            //lblAkaCustomerNumber.Text = string.Empty;
            //lblAkaCustomerName.Text = string.Empty;
            //_CustomerPONumber = string.Empty;
            //_tradingPartnerName = string.Empty;
            //lblAkaItem.Text = string.Empty;
            //lblAkaDescription.Text = string.Empty;

            //lblStdRoll.Text = string.Empty;
            //lblStdPallet.Text = string.Empty;
            //lblRollOnPallet.Text = string.Empty;
            //lblPalletTotal.Text = string.Empty;
            ClearAll();
            WorkOrderUIFill(cmbWorkOrders.SelectedItem.ToString());
        }

        //Obtener datos de la orden seleccionada
        private async void WorkOrderUIFill(string workOrder)
        {
            cmbWorkOrders.Enabled = false;
            ShowWait(true, "Cargando datos ...");
            try
            {
                //♥ Consultar WORKORDER ♥
                Task<string> tskWorkOrdersData = APIService.GetRequestAsync(String.Format(EndPoints.WOProcessData, workOrder, Constants.Plant2Id));
                string response = await tskWorkOrdersData;
                
                if (string.IsNullOrEmpty(response)) 
                {
                    ShowWait(false);
                    cmbWorkOrders.Enabled = lblMode.Text.Equals("Auto.") ? false : true;
                    return; 
                }

                JObject objWorkOrder = JObject.Parse(response);
                if ((int)objWorkOrder["count"] == 0)
                {
                    ShowWait(false);
                    NotifierController.Warning("Datos de orden no encotrada");
                    cmbWorkOrders.Enabled = lblMode.Text.Equals("Auto.") ? false : true;
                    return;
                }
                dynamic wo = objWorkOrder["items"][0]; //Objeto WORKORDER
                _workOrderId = Int64.Parse(wo.WorkOrderId.ToString());
                _workOrderNumber = workOrder;
                lblPrimaryProductQuantity.Text = string.IsNullOrEmpty(wo.PrimaryProductQuantity.ToString()) ? 0 : wo.PrimaryProductQuantity.ToString();
                //lblCompletedQuantity.Text = wo.CompletedQuantity.ToString();
                lblUoM.Text = wo.UOMCode.ToString();
                if (!string.IsNullOrEmpty(wo.CompletedQuantity.ToString()))
                {
                    NotifierController.Warning($"Orden con despacho registrado [{wo.CompletedQuantity.ToString()} {lblUoM.Text}], verifique antes de pesar");
                }

                lblItemNumber.Text = wo.ItemNumber.ToString();
                lblItemDescription.Text = wo.Description.ToString();
                lblItemDescriptionEnglish.Text = lblItemDescription.Text.Contains("PET CRIST") ? wo.Description.ToString().Replace("PET CRIST", "PET SHEET CRIST") : 
                                                 TranslateService.Translate(lblItemDescription.Text.ToString());

                WithThickness();//Obtener espesor y ancho productos PCR

                lblPlannedStartDate.Text = wo.PlannedStartDate.ToString();
                lblPlannedCompletionDate.Text = wo.PlannedCompletionDate.ToString();

                //Verificar Status de la programacion de la orden
                CheckStatusScheduleOrder(DateTime.Parse(wo.PlannedStartDate.ToString()), DateTime.Parse(wo.PlannedCompletionDate.ToString()));
                
                //♥ Consultar ITEM ♥
                dynamic itemsV2 = await CommonService.OneItem(String.Format(EndPoints.ItemP2, lblItemNumber.Text, Constants.Plant2Id));

                if (itemsV2 != null)
                {
                    if (string.IsNullOrEmpty(itemsV2.UnitWeightQuantity.ToString()) || string.IsNullOrEmpty(itemsV2.MaximumLoadWeight.ToString()))
                    {
                        MessageBox.Show("Peso estándar no definido, pesaje restringido", "Verificar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        lblStdRoll.Text = itemsV2.UnitWeightQuantity.ToString();
                        lblWeightUOMRoll.Text = itemsV2.WeightUOMValue.ToString();
                        lblWeightUOMPallet.Text = lblWeightUOMRoll.Text;
                        lblStdPallet.Text = itemsV2.MaximumLoadWeight.ToString();
                        lblContainerType.Text = itemsV2.ContainerTypeValue.ToString();

                        int rollsOnPallet = int.Parse(itemsV2.MaximumLoadWeight.ToString()) / int.Parse(itemsV2.UnitWeightQuantity.ToString());
                        lblRollOnPallet.Text = rollsOnPallet.ToString();

                        int palletTotal = (int)Math.Ceiling(float.Parse(lblPrimaryProductQuantity.Text) / (float.Parse(lblStdRoll.Text) * float.Parse(lblRollOnPallet.Text)));
                        lblPalletTotal.Text = palletTotal.ToString();
                    }
                }
                else
                {
                    NotifierController.Warning("Peso estándar del producto no encontrado");
                }

                //Flex orden de venta
                string flexPV = wo.ProcessWorkOrderDFF.items[0].pedidoDeVenta.ToString();//"34"
                if (string.IsNullOrEmpty(flexPV))
                {
                    lblAkaSaleOrder.Text = "NA";
                    _akaCustomer = "DEFAULT";
                }
                else
                {
                    lblAkaSaleOrder.Text = flexPV;

                    //♥ Consultar OM & AKA ♥
                    dynamic om = await CommonService.OneItem(String.Format(EndPoints.SalesOrders, lblAkaSaleOrder.Text, Constants.BusinessUnitId));
                    if (om != null)
                    {
                        lblAkaCustomerNumber.Text = om.BuyingPartyNumber.ToString();//BuyingPartyName
                        lblAkaCustomerName.Text = om.BuyingPartyName.ToString();
                        lblAkaCustomerPO.Text = om.CustomerPONumber.ToString();
                        _CustomerPONumber = lblAkaCustomerPO.Text;
                        _akaCustomer = lblAkaCustomerNumber.Text;

                        //♥ Consultar TradingPartnerItemRelationships ♥
                        dynamic aka = await CommonService.OneItem(String.Format(EndPoints.TradingPartnerItemRelationships, lblItemNumber.Text, lblAkaCustomerNumber.Text));
                        if (aka != null)
                        {
                            lblAkaItem.Text = aka.TradingPartnerItemNumber.ToString();
                            lblAkaDescription.Text = aka.RelationshipDescription.ToString();
                            _tradingPartnerName = aka.TradingPartnerName.ToString();
                        }
                        else
                        {
                            lblAkaItem.Text = string.Empty;
                            lblAkaDescription.Text = string.Empty;
                            _tradingPartnerName = string.Empty;
                            Console.WriteLine($"Producto no relacionado con el cliente AKA [{DateService.Today()}]", Color.Red);
                            MessageBox.Show("Producto no relacionado con el cliente AKA, pesaje restringido", "AKA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Pedido de venta no encontrado [{DateService.Today()}]", Color.Red);
                    }
                }

                //♥ Consultar template etiqueta en APEX  ♥
                dynamic labelApex = await LabelService.LabelInfo(Constants.Plant2Id, _akaCustomer, lblItemNumber.Text);
                if (labelApex.LabelName.ToString().Equals("null"))
                {
                    _akaCustomer = "DEFAULT";
                    DialogResult result = MessageBox.Show("Etiqueta de cliente/producto no encontrada, se cargará la etiqueta estándar", "¡Alerta!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (result == DialogResult.OK)
                    {
                        labelApex = await LabelService.LabelInfo(Constants.Plant2Id, _akaCustomer, lblItemNumber.Text);
                    }
                    else
                    {
                        labelApex = await LabelService.LabelInfo(Constants.Plant2Id, _akaCustomer, lblItemNumber.Text);
                    }
                    lblLabelName.Text = labelApex.LabelName.ToString();
                }
                else
                {
                    lblLabelName.Text = labelApex.LabelName.ToString();
                }

                //Verificar Historial Pesaje----------------------------
                Task<string> tskHistory = APIService.GetApexAsync(String.Format(EndPoints.RollsOrder, Constants.Plant2Id, workOrder));
                string responseHistory = await tskHistory;

                progressBarWO.Value = 0;
                lblAdvance.Text = "0%";

                _palletCount = 0;
                _rollCount = 0;

                dgRolls.Rows.Clear();
                dgRolls.Refresh();

                dgPallets.Rows.Clear();
                dgPallets.Refresh();

                lblPalletNumber.Text = string.Empty;

                if (!string.IsNullOrEmpty(responseHistory))
                {
                    dynamic rollsOnOrder = JsonConvert.DeserializeObject<dynamic>(responseHistory);
                    lblCompletedQuantity.Text = rollsOnOrder.Completed.ToString();
                    if(rollsOnOrder.Completed > 0)
                    {
                        _completedHistory = true;
                        lblCompletedQuantity.Text = rollsOnOrder.Completed.ToString();
                        CalculateAdvace(float.Parse(lblCompletedQuantity.Text));
                        FillDatagridsFromRecords(rollsOnOrder.Rolls);
                    }
                }
                else
                {
                    Console.WriteLine($"Sin respuesta del historial de pesaje [{DateService.Today()}]", Color.Red);
                }

                //Validar activacion de boton de pesaje
                if (!string.IsNullOrEmpty(cmbWorkOrders.Text) && !string.IsNullOrEmpty(lblResourceName.Text) && !string.IsNullOrEmpty(lblLabelName.Text) &&
                   !string.IsNullOrEmpty(lblStdRoll.Text) && !string.IsNullOrEmpty(lblStdPallet.Text))
                {
                    btnGetWeight.Enabled = _akaCustomer.Equals("DEFAULT") ? true : btnGetWeight.Enabled = string.IsNullOrEmpty(lblAkaItem.Text) ? false : true;
                }
                else
                {
                    btnGetWeight.Enabled = false;
                }

                //Order completada
                if (dgRolls.Rows.Count > 0)
                {
                    if(int.Parse(lblPalletTotal.Text) * int.Parse(lblRollOnPallet.Text) == dgRolls.Rows.Count)
                    {
                        NotifierController.Warning("Orden completada");
                        btnGetWeight.Enabled = false;

                        if (float.Parse(lblCompletedQuantity.Text) > float.Parse(lblPrimaryProductQuantity.Text))
                        {
                            Console.WriteLine($"Pesaje [{lblCompletedQuantity.Text} kg] excede la cantidad programada a producir [{lblPrimaryProductQuantity.Text} kg]", Color.Red);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                cmbWorkOrders.Enabled = lblMode.Text.Equals("Auto.") ? false : true;
                MessageBox.Show("Error. " + ex.Message, "Error al seleccionar orden", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ShowWait(false);
            lblStatusProcess.Text = dgRolls.Rows.Count > 0 ? "¡Coloque y pese CORE!" : btnGetWeight.Enabled ? "¡Coloque y pese TARA!" : string.Empty;
            lblStatusProcess.ForeColor = dgRolls.Rows.Count > 0 ? Color.Red : btnGetWeight.Enabled ? Color.Red : Color.Black;
            cmbWorkOrders.Enabled = lblMode.Text.Equals("Auto.") ? false : true;
        }

        //Obtener espesor y ancho
        private void WithThickness()
        {
            if (!string.IsNullOrEmpty(lblItemNumber.Text) && lblItemNumber.Text.Substring(0, 3).Equals("PCR"))
            {
                try
                {
                    string strMeasures = lblItemDescription.Text.Split(' ')[2];
                    string[] partsMeasures = strMeasures.Split('x');
                    float mils = float.TryParse(partsMeasures[0].Substring(0, partsMeasures[0].Length - 4), out _) ? float.Parse(partsMeasures[0].Substring(0, partsMeasures[0].Length - 4)) : 0;
                    float inches = float.TryParse(partsMeasures[1].Substring(0, partsMeasures[1].Length - 2), out _) ? float.Parse(partsMeasures[1].Substring(0, partsMeasures[1].Length - 2)) : 0;
                    if (mils > 0 && inches > 0)
                    {
                        float inTomm = inches * 25.4f;
                        string strWithThickness = $"{mils} mil X {inTomm.ToString("F2")} mm";
                    }
                    else
                    {
                        strWithThickness = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    strWithThickness = string.Empty;
                    Console.WriteLine($"Error al obtener espesor y ancho. {ex.Message} [{DateService.Today()}]", Color.Red);
                }
            }
        }

        //Llenar tabla con pesajes
        private void FillDatagridsFromRecords(dynamic rollsOnOrder)
        {
            int currentPallet = rollsOnOrder[0].Pallet;
            float currentTare = rollsOnOrder[0].Tare;
            float coresPallet = 0;
            float netpallet = 0;
            int countRollsOnPallet = 0;

            foreach (dynamic R in rollsOnOrder)
            {
                float grossRoll = R.Core + R.Net;
                string[] palletData = new string[] { R.Pallet.ToString(), R.Roll.ToString(), R.Core.ToString("F1"), R.Net.ToString("F1"), grossRoll.ToString("F1") };
                int indexNewRoll = dgRolls.Rows.Add(palletData);
                dgRolls.FirstDisplayedScrollingRowIndex = indexNewRoll;

                if (R.Pallet != currentPallet)
                {
                    float grossPallet = currentTare + coresPallet + netpallet;
                    string[] rollData = new string[] { currentPallet.ToString(), currentTare.ToString("F1"), coresPallet.ToString("F1"), netpallet.ToString("F1"), grossPallet.ToString("F1") };
                    int indexNewPallet = dgPallets.Rows.Add(rollData);
                    dgPallets.FirstDisplayedScrollingRowIndex = indexNewPallet;

                    netpallet = 0;
                    coresPallet = 0;
                    currentPallet = R.Pallet;
                    currentTare = R.Tare;
                    countRollsOnPallet = 0;
                }

                netpallet += float.Parse(R.Net.ToString());
                coresPallet += float.Parse(R.Core.ToString());

                countRollsOnPallet += 1;
            }

            if (rollsOnOrder.Count > 0)
            {
                float grossPallet = currentTare + coresPallet + netpallet;
                string[] rollData = new string[] { currentPallet.ToString(), currentTare.ToString("F1"), coresPallet.ToString("F1"), netpallet.ToString("F1"), grossPallet.ToString("F1") };
                int indexNewPallet = dgPallets.Rows.Add(rollData);
                dgPallets.FirstDisplayedScrollingRowIndex = indexNewPallet;
            }

            _completedHistory = false;

            _palletCount = dgPallets.Rows.Count;
            _rollCount = dgRolls.Rows.Count;

            lblPalletNumber.Text = _palletCount.ToString();

            if (int.Parse(lblRollOnPallet.Text) > 1)
            {
                //Validar si el pallet esta completo SINO contunuar donde se quedo
                if (countRollsOnPallet != int.Parse(lblRollOnPallet.Text))
                {
                    _tareWeight = float.Parse(dgPallets.Rows[dgPallets.Rows.Count - 1].Cells["P_Tare"].Value.ToString());
                    float mGrossPallet = float.Parse(dgPallets.Rows[dgPallets.Rows.Count - 1].Cells["P_Gross"].Value.ToString());
                    _previousWeight = mGrossPallet - _tareWeight;

                    Console.WriteLine($"Palet {dgPallets.Rows.Count} incompleto, continue pesando [Báscula: {_previousWeight.ToString("F1")} kg]", Color.Red);
                    lblTare.Text = _tareWeight.ToString("F1");
                    int rollTemp = 0;
                    foreach (DataGridViewRow row in dgRolls.Rows)
                    {
                        if (row.Cells["R_Pallet"].Value.ToString().Equals(dgPallets.Rows[dgPallets.Rows.Count - 1].Cells["P_Pallet"].Value.ToString()))
                        {
                            rollTemp += 1;
                            _netPallet += float.Parse(row.Cells["R_Net"].Value.ToString());
                            tabLayoutPallet.BackgroundImage = Resources.pallet_filled;
                            TableLayoutPalletControl(int.Parse(lblRollOnPallet.Text), rollTemp, row.Cells["R_Pallet"].Value.ToString());
                        }
                    }
                    _rollByPallet = rollTemp;

                    dgPallets.Rows.RemoveAt(dgPallets.Rows.Count - 1);

                    lblStatusProcess.Text = "¡Coloque y pese CORE!";
                    lblStatusProcess.ForeColor = Color.Red;

                    btnGetWeight.Text = "CORE";
                    btnGetWeight.BackColor = Color.DarkOrange;
                    btnGetWeight.ForeColor = Color.Black;
                }
            }

            btnEndProcess.Visible = _rollCount > 0 ? true : false;
            _isPalletStart = true;
        }
        #endregion

        /*-------------------------------- BUTTONS -----------------------------------*/
        #region Buttons Actions
        private async void btnGetWeight_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cmbWorkOrders.Text))
            {
                btnGetWeight.Enabled = false;
                cmbWorkOrders.Enabled = false;
                if (btnGetWeight.Text.Equals("TARAR"))
                {
                    ShowWait(true, "Pesando tara ...");
                    await GetTare();
                }
                else
                {
                    //----------------- PESAR ---------------
                    string messageWait = btnGetWeight.Text.Equals("CORE") ? "Pesando core ..." : btnGetWeight.Text.Equals("PESAR") ? "Pesando rollo ..." : "Procesando ...";
                    ShowWait(true, messageWait);

                    string responseWeighing = await RadwagController.SocketWeighing("S");

                    if (responseWeighing == "EX")
                    {
                        NotifierController.Warning("Tiempo de espera agotado, vuelva a  intentar");
                    }
                    else
                    {
                        if (float.TryParse(responseWeighing, out float _weightFromWeighing))
                        {
                            if (_weightFromWeighing > 0)
                            {
                                if (_weightFromWeighing == _previousWeight)
                                {
                                    MessageBox.Show("Pesaje no ha cambiado, verifique.", "¡Precaucion!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else if (_weightFromWeighing < _previousWeight)
                                {
                                    MessageBox.Show("Peso menor al obtenido anteriormente, verifique.", "¡Precaucion!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else
                                {
                                    if (btnGetWeight.Text.Equals("CORE"))
                                    {
                                        float coreWeight = _weightFromWeighing - _previousWeight;
                                        if (coreWeight <= Settings.Default.CoreMaxWeight)
                                        {
                                            _previousWeight = _weightFromWeighing;

                                            lblWeight.Text = coreWeight.ToString("F1");
                                            lblCore.Text = coreWeight.ToString("F1");
                                            btnGetWeight.Text = "PESAR";
                                            btnGetWeight.BackColor = Color.LimeGreen;
                                            btnReloadCore.Visible = true;
                                        }
                                        else
                                        {
                                            MessageBox.Show("Peso por encima del estándar de un CORE, verifique.", "¡No esta pesando un CORE!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        }

                                    }
                                    else if (btnGetWeight.Text.Equals("PESAR"))
                                    {
                                        btnReloadTare.Visible = false;
                                        btnReloadCore.Visible = false;
                                        float rollNet = _weightFromWeighing - _previousWeight; // 
                                        float rollGross = float.Parse(lblCore.Text) + rollNet; //Core + rollo
                                        _previousWeight = _weightFromWeighing; //Reserver peso
                                        _netPallet += rollNet;

                                        lblWeight.Text = rollNet.ToString("F1");
                                        lblPalletNet.Text = _netPallet.ToString("F1");
                                        btnReloadTare.Visible = false;

                                        _rollCount++;

                                        //Agregar pesos a datagrid
                                        string[] row = new string[] { _palletCount.ToString(), _rollCount.ToString(), lblCore.Text, rollNet.ToString("F1"), rollGross.ToString("F1") };
                                        int indexNewRoll = dgRolls.Rows.Add(row);
                                        dgRolls.FirstDisplayedScrollingRowIndex = indexNewRoll;

                                        if (int.Parse(lblRollOnPallet.Text) > 1)
                                        {
                                            FillLabelRoll(row);

                                            btnGetWeight.Text = "CORE";
                                            btnGetWeight.BackColor = Color.DarkOrange;
                                            btnGetWeight.ForeColor = Color.Black;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Peso invalido [{_weightFromWeighing.ToString("F1")} kg]", "Báscula", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Valor invalido obtenido de la báscula", Color.Red);
                            NotifierController.Warning($"{responseWeighing}");
                        }
                    }
                }

                btnGetWeight.Enabled = true;
                ShowWait(false);
                lblStatusProcess.Text = btnGetWeight.Text.Equals("CORE") ? "¡Coloque y pese CORE!" : btnGetWeight.Text.Equals("PESAR") ? "¡Coloque y pese ROLLO!" : btnGetWeight.Text.Equals("TARAR") ? "¡Coloque y pese TARA!" : string.Empty;
                lblStatusProcess.ForeColor = string.IsNullOrEmpty(lblStatusProcess.Text) ? Color.Black : Color.Red;
            }
            else
            {
                MessageBox.Show($"Seleccione orden de trabajo antes de pesar", "¡Alerta!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task GetTare()
        {
            string responseTare = await RadwagController.SocketWeighing("T");
            if (responseTare.Equals("OK"))
            {
                string requestTareWeight = await RadwagController.SocketWeighing("OT");
                if (!requestTareWeight.Equals("EX"))
                {
                    //----------------- TARAR ---------------
                    _tareWeight = float.Parse(requestTareWeight);
                    if (_tareWeight > 0)
                    {
                        if (_tareWeight >= Settings.Default.TareMinWeight && _tareWeight <= Settings.Default.TareMaxWeight)
                        {
                            timerSchedule.Stop();
                            lblWeight.Text = float.Parse(requestTareWeight).ToString("F1");
                            lblTare.Text = float.Parse(requestTareWeight).ToString("F1");

                            if (!_reloadTare) // SINO presiono volver a calcular 
                            {
                                btnGetWeight.Text = "CORE";
                                btnGetWeight.BackColor = Color.DarkOrange;
                                btnGetWeight.ForeColor = Color.Black;
                                btnReloadTare.Visible = true;

                                _rollByPallet = 0;
                                _netPallet = 0;
                                _palletCount += 1;
                                _previousWeight = 0;

                                tabLayoutPallet.BackgroundImage = Resources.pallet_filled;
                                TableLayoutPalletControl(int.Parse(lblRollOnPallet.Text), 0, lblPalletNumber.Text);

                                lblPalletNumber.Text = _palletCount.ToString();
                            }
                        }
                        else
                        {
                            if (_tareWeight > Settings.Default.TareMaxWeight)
                            {
                                lblWeight.Text = _tareWeight.ToString("F1");
                                MessageBox.Show("Peso por encima del estándar de una TARA, verifique.", "¡No esta pesando una TARA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else if (_tareWeight < Settings.Default.TareMinWeight)
                            {
                                lblWeight.Text = _tareWeight.ToString("F1");
                                MessageBox.Show("Peso debajo del estándar de una TARA, verifique.", "¡No esta pesando una TARA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                lblWeight.Text = _tareWeight.ToString("F1");
                                MessageBox.Show("Peso de tara invalido, verifique.", "¡Precaución!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                    {
                        lblWeight.Text = _tareWeight.ToString("F1");
                        NotifierController.Warning($"Peso de tara invalido [{_tareWeight.ToString("F1")} kg]");
                    }
                }
                else
                {
                    NotifierController.Warning("Tiempo de espera agotado, vuelva a  intentar");
                }
            }
            else
            {
                string res = responseTare.Equals("EX") ? "Error de comunicación a báscula" : responseTare;
                NotifierController.Error(res);
            }
            ShowWait(false);

            lblStatusProcess.Text = string.IsNullOrEmpty(lblCore.Text) ? "¡Coloque y pese CORE!" : "¡Coloque y pese ROLLO!";
            lblStatusProcess.ForeColor = Color.Red;
            _reloadTare = false;
        }

        private async void btnReloadTare_Click(object sender, EventArgs e)
        {
            _reloadTare = true;
            ShowWait(true, "Recalculando tara ...");
            await GetTare();
        }

        private async void btnReloadCore_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(lblCore.Text))
            {
                MessageBox.Show("Acción no permitida, sin pesaje de core", "¡Precaucion!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ShowWait(true, "Recalculando core ...");
            string responseWeighing = await RadwagController.SocketWeighing("S");

            if (responseWeighing == "EX")
            {
                NotifierController.Warning("Tiempo de espera agotado, vuelva a  intentar");
            }
            else
            {
                if (float.TryParse(responseWeighing, out float _weightFromWeighing))
                {
                    if (_weightFromWeighing > 0)
                    {
                        if (_weightFromWeighing < (_previousWeight - float.Parse(lblCore.Text)))
                        {
                            MessageBox.Show("Peso menor al obtenido anteriormente, verifique.", "¡Precaucion!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            float coreWeight = _weightFromWeighing - (_previousWeight - float.Parse(lblCore.Text));
                            if (coreWeight <= Settings.Default.CoreMaxWeight)
                            {
                                _previousWeight -= float.Parse(lblCore.Text); //Quitar peso anterior de CORE al acomulado
                                _previousWeight = _weightFromWeighing;

                                lblWeight.Text = coreWeight.ToString("F1");
                                lblCore.Text = coreWeight.ToString("F1");
                                btnGetWeight.Text = "PESAR";
                                btnGetWeight.BackColor = Color.LimeGreen;
                            }
                            else
                            {
                                MessageBox.Show("Peso por encima del estándar de un CORE, verifique.", "¡No esta pesando un CORE!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Peso invalido [{_weightFromWeighing.ToString("F1")} kg]", "Báscula", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    Console.WriteLine("Valor invalido obtenido de la báscula", Color.Red);
                    NotifierController.Warning($"{responseWeighing}");
                }
            }
            ShowWait(false);
            lblStatusProcess.Text = "¡Coloque y pese ROLLO!";
            lblStatusProcess.ForeColor = Color.Red;
        }

        private async void ReWeightMenuItem_Click(object sender, EventArgs e)
        {
            ShowWait(true, "Pesando rollo nuevamente ...");
            int lastRow = dgRolls.Rows.Count - 1;
            if (!dgRolls.Rows[lastRow].IsNewRow)
            {
                string responseWeighing = await RadwagController.SocketWeighing("S");

                if (responseWeighing == "EX")
                {
                    NotifierController.Warning("Tiempo de espera agotado, vuelva a  intentar");
                }
                else
                {
                    if (float.TryParse(responseWeighing, out float _weightFromWeighing))
                    {
                        if (_weightFromWeighing > 0)
                        {
                            float dgNet = float.Parse(dgRolls.Rows[lastRow].Cells["R_Net"].Value.ToString());
                            if (_weightFromWeighing < (_previousWeight - dgNet))
                            {
                                MessageBox.Show("Se detecto menor peso al obtenido anteriormente, verifique el producto colocado", "¡Precaucion!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                _previousWeight -= dgNet;
                                int dgPallet = int.Parse(dgRolls.Rows[lastRow].Cells["R_Pallet"].Value.ToString());
                                int dgRoll = int.Parse(dgRolls.Rows[lastRow].Cells["R_Roll"].Value.ToString());
                                float dgCore = float.Parse(dgRolls.Rows[lastRow].Cells["R_Core"].Value.ToString());
                                

                                float rollNet = _weightFromWeighing - _previousWeight;//Calcular peso rollo
                                float rollGross = dgCore + rollNet;

                                _previousWeight = _weightFromWeighing; //Reserver peso
                                _netPallet -= dgNet; //Descontar del neto acomulado
                                _netPallet += rollNet; //Aumentar con nuevo neto

                                lblWeight.Text = rollNet.ToString("F1");
                                lblPalletNet.Text = _netPallet.ToString("F1");

                                //Actualizar fila de ROLLO
                                string[] rowRoll = new string[] { dgPallet.ToString(), dgRoll.ToString(), dgCore.ToString("F1"), rollNet.ToString("F1"), rollGross.ToString("F1") };
                                dgRolls.Rows[lastRow].SetValues(rowRoll);

                                float palletNetSum = dgRolls.Rows.Cast<DataGridViewRow>().Sum(t => float.Parse(t.Cells["R_Net"].Value.ToString()));
                                lblCompletedQuantity.Text = palletNetSum.ToString();
                                CalculateAdvace(palletNetSum);

                                TableLayoutPalletControl(int.Parse(lblRollOnPallet.Text), _rollByPallet, lblPalletNumber.Text);

                                if (int.Parse(lblRollOnPallet.Text) > 1)
                                {
                                    FillLabelRoll(rowRoll);
                                    await LabelService.PrintP2(dgRoll, "ROLL"); //Imprimir etiqueta rollo
                                }

                                //Verificar si es el ultimo rollo del pallet
                                if (_rollByPallet == 0 && _rollCount > 0)
                                {
                                    int lastPallet = dgPallets.Rows.Count - 1;

                                    float dgTare = float.Parse(dgPallets.Rows[lastPallet].Cells["P_Tare"].Value.ToString());
                                    float dgCoresPallet = float.Parse(dgPallets.Rows[lastPallet].Cells["P_Cores"].Value.ToString());

                                    //Sumar neto de rollos
                                    float newNet = dgRolls.Rows.Cast<DataGridViewRow>().Where(t => t.Cells["R_Pallet"].Value.ToString().Equals(dgPallet.ToString()))
                                    .Sum(t => {
                                        float value;
                                        return float.TryParse(t.Cells["R_Net"].Value?.ToString(), out value) ? value : 0;
                                    });
                                    float grossPallet = dgTare + dgCoresPallet + newNet;

                                    //Actualizar fila de PALLET
                                    string[] rowPallet = new string[] { dgPallet.ToString(), dgTare.ToString("F1"), dgCoresPallet.ToString("F1"), newNet.ToString("F1"), grossPallet.ToString("F1") };
                                    dgPallets.Rows[lastPallet].SetValues(rowPallet);

                                    string rollWeights = WeightsPallet(dgPallets.Rows.Count);

                                    string[] palletWeight = new string[5];
                                    for (int i = 0; i < 5; i++)
                                    {
                                        palletWeight[i] = dgPallets.Rows[lastPallet].Cells[i].Value.ToString();
                                    }

                                    FillLabelPallet(palletWeight, rollWeights);
                                    await LabelService.PrintP2(dgPallets.Rows.Count, "PALLET");
                                    TableLayoutPalletControl(0, _rollByPallet, lblPalletNumber.Text);
                                }

                                UpdateRollApex(dgRoll, rollNet);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Peso invalido [{_weightFromWeighing.ToString("F1")} kg], vuelva a intentar", "Báscula", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Valor invalido obtenido de la báscula", Color.Red);
                        NotifierController.Warning($"{responseWeighing}");
                    }
                }
            }
            ShowWait(false);
        }

        private void btnSwapMode_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_workOrderNumber))
            {
                DialogResult dialogResult = MessageBox.Show($"¿Desea cambiar el modo de trabajo? Los datos de la operación actual se perderán",
                                                            "Modo",
                                                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    if (lblMode.Text.Equals("Auto."))
                    {
                        lblMode.Text = "Manual";
                        ClearAll();
                        timerSchedule.Stop();
                        cmbWorkOrders.Items.Clear();
                        cmbWorkOrders.Enabled = true;
                        picBoxWaitWO.Visible = false;
                    }
                    else
                    {
                        lblMode.Text = "Auto.";
                        ClearAll();
                        cmbWorkOrders.Items.Clear();
                        cmbWorkOrders.Enabled = false;
                        //ProductionScheduling(this, EventArgs.Empty);
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }
            else
            {
                if (lblMode.Text.Equals("Auto."))
                {
                    lblMode.Text = "Manual";
                    ClearAll();
                    timerSchedule.Stop();
                    cmbWorkOrders.Items.Clear();
                    cmbWorkOrders.Enabled = true;
                    picBoxWaitWO.Visible = false;
                }
                else
                {
                    lblMode.Text = "Auto.";
                    ClearAll();
                    cmbWorkOrders.Items.Clear();
                    cmbWorkOrders.Enabled = false;
                    //ProductionScheduling(this, EventArgs.Empty);
                }
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmLogin frmLogin = new frmLogin();
            frmLogin.StartPosition = FormStartPosition.CenterParent;
            frmLogin.ShowDialog();
        }

        private void btnEndProcess_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Seguro que desea terminar de pesar la orden?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if(_rollByPallet < int.Parse(lblRollOnPallet.Text))
                {
                    DialogResult printLabel = MessageBox.Show("Se detecto palet incompleto ¿Desea imprimir etiqueta del palet?", 
                                                              "¡Palet incompleto!", 
                                                               MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if(printLabel == DialogResult.Yes)
                    {
                        _endWeight = true;
                        AddPallet();
                    }
                }
                else
                {
                    _endWeight = true;
                    cmbWorkOrders.Items.Clear();
                    ClearAll();
                }
            }
        }
        #endregion

        /*-------------------------------- ROLLOS ------------------------------------*/
        #region DataGrid Rollos
        private void TableLayoutPalletControl(int rollOnPallet, int rollNumber, string palletNumber)
        {
            if (rollOnPallet <= 25)
            {
                int count = 0;

                tabLayoutPallet.Controls.Clear();
                tabLayoutPallet.RowStyles.Clear();
                tabLayoutPallet.ColumnStyles.Clear();

                //Definir FILAS Y COLUMNAS
                switch(rollOnPallet)
                {
                    case 3:
                        tabLayoutPallet.ColumnCount = 2;
                        tabLayoutPallet.RowCount = 2;
                        break;
                    case 4:
                        tabLayoutPallet.ColumnCount = 2;
                        tabLayoutPallet.RowCount = 2;
                        break;
                    case 5: 
                    case 6:
                        tabLayoutPallet.ColumnCount = 2;
                        tabLayoutPallet.RowCount = 3;
                        break;
                    case 7: 
                    case 8: 
                    case 9:
                        tabLayoutPallet.ColumnCount = 3;
                        tabLayoutPallet.RowCount = 3;
                        break;
                    case 10:
                    case 11 :
                    case 12:
                        tabLayoutPallet.ColumnCount = 3;
                        tabLayoutPallet.RowCount = 4;
                        break;
                    case 13: 
                    case 14: 
                    case 15: 
                    case 16:
                        tabLayoutPallet.ColumnCount = 4;
                        tabLayoutPallet.RowCount = 4;
                        break;
                    case 17: 
                    case 18: 
                    case 19: 
                    case 20:
                        tabLayoutPallet.ColumnCount = 4;
                        tabLayoutPallet.RowCount = 5;
                        break;
                    case 21: 
                    case 22: 
                    case 23: 
                    case 24:
                    case 25:
                        tabLayoutPallet.ColumnCount = 5;
                        tabLayoutPallet.RowCount = 5;
                        break;
                    default: // 1 o 2 rollos
                        tabLayoutPallet.ColumnCount = rollOnPallet;
                        tabLayoutPallet.RowCount = 1;
                        break;
                }

                for (int row = 0; row < tabLayoutPallet.RowCount; row++)
                {
                    tabLayoutPallet.RowStyles.Add(new RowStyle(SizeType.Percent, 1));
                    for (int col = 0; col < tabLayoutPallet.ColumnCount; col++)
                    {
                        PictureBox picRoll = new PictureBox();
                        if (count < rollNumber)
                        {
                            IEnumerable<string> columnWeigthsNetKg = dgRolls.Rows.Cast<DataGridViewRow>().Where(fila => fila.Cells["R_Pallet"].Value.ToString().Equals(palletNumber))
                                                                    .Select(fila => fila.Cells["R_Net"].Value.ToString());

                            string[] weigthRoll = columnWeigthsNetKg.ToArray();

                            if(float.Parse(weigthRoll[count]) == float.Parse(lblStdRoll.Text))
                            {
                                picRoll.Image = Resources.roll;
                            }
                            else if(float.Parse(weigthRoll[count]) > float.Parse(lblStdRoll.Text))
                            {
                                picRoll.Image = Resources.roll_yellow;
                            }
                            else if(float.Parse(weigthRoll[count]) < float.Parse(lblStdRoll.Text))
                            {
                                picRoll.Image = Resources.roll_red;
                            }

                            AppController.ToolTip(picRoll, weigthRoll[count].ToString() + " kg");
                        }
                        else
                        {
                            picRoll.Image = Resources.roll_empty;
                        }

                        picRoll.BackColor = Color.Transparent;
                        picRoll.SizeMode = PictureBoxSizeMode.Zoom;
                        picRoll.Dock = DockStyle.Fill;

                        count++;

                        if (count <= rollOnPallet)
                        {
                            tabLayoutPallet.Controls.Add(picRoll, col, row);

                            if(rollOnPallet == 3 && count == 3)
                            {
                                tabLayoutPallet.SetColumnSpan(picRoll, 2);
                            }
                            else if (rollOnPallet == 5 && count == 5)
                            {
                                tabLayoutPallet.SetColumnSpan(picRoll, 2);
                            }
                            else if (rollOnPallet == 7 && count == 7)
                            {
                                tabLayoutPallet.SetColumnSpan(picRoll, 3);
                            }
                            else if (rollOnPallet == 10 && count == 10)
                            {
                                tabLayoutPallet.SetColumnSpan(picRoll, 3);
                            }
                            else if (rollOnPallet == 11 && count == 10)
                            {
                                tabLayoutPallet.SetColumnSpan(picRoll, 2);
                            }
                            else if (rollOnPallet == 13 && count == 13)
                            {
                                tabLayoutPallet.SetColumnSpan(picRoll, 4);
                            }
                            else if (rollOnPallet == 14 && count == 13)
                            {
                                tabLayoutPallet.SetColumnSpan(picRoll, 2);
                            }
                            else if (rollOnPallet == 14 && count == 14)
                            {
                                tabLayoutPallet.SetColumnSpan(picRoll, 2);
                            }
                            else if (rollOnPallet == 14 && count == 14)
                            {
                                tabLayoutPallet.SetColumnSpan(picRoll, 2);
                            }
                            else if (rollOnPallet == 17 && count == 17)
                            {
                                tabLayoutPallet.SetColumnSpan(picRoll, 4);
                            }
                            else if (rollOnPallet == 18 && count == 17)
                            {
                                tabLayoutPallet.SetColumnSpan(picRoll, 2);
                            }
                            else if (rollOnPallet == 19 && count == 18)
                            {
                                tabLayoutPallet.SetColumnSpan(picRoll, 2);
                            }
                            else if (rollOnPallet == 21 && count == 21)
                            {
                                tabLayoutPallet.SetColumnSpan(picRoll, 5);
                            }
                            else if (rollOnPallet == 22 && count == 21)
                            {
                                tabLayoutPallet.SetColumnSpan(picRoll, 3);
                            }
                            else if (rollOnPallet == 23 && count == 21)
                            {
                                tabLayoutPallet.SetColumnSpan(picRoll, 2);
                            }
                        }
                        tabLayoutPallet.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1));
                    }
                }
            }
            else
            {
                NotifierController.Warning($"{rollOnPallet} rollos rebasan el estándar de un palet");
            }
        }

        bool IsTheSameCellValue(int column, int row)
        {
            DataGridViewCell cell1 = dgRolls[column, row];
            DataGridViewCell cell2 = dgRolls[column, row - 1];
            if (cell1.Value == null || cell2.Value == null)
            {
                return false;
            }
            return cell1.Value.ToString() == cell2.Value.ToString();
        }

        //Modificaciones visuales del DataGrid
        private void dgWeight_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            /*if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            //Combinar celdas repetidas
            e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
            if (e.RowIndex < 1 || e.ColumnIndex < 0)
                return;
            if (IsTheSameCellValue(e.ColumnIndex, e.RowIndex))
            {
                e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            }
            else
            {
                e.AdvancedBorderStyle.Top = dgRolls.AdvancedCellBorderStyle.Top;
            }*/
        }

        //Cambio de color de filas (Max-Min)
        private void dgWeights_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //Cambiar color
            foreach (DataGridViewRow row in dgRolls.Rows)
            {
                float rollNetKg = float.Parse(row.Cells["R_Net"].Value.ToString());
                if (!string.IsNullOrEmpty(lblStdRoll.Text))
                {
                    float _stdRollWeight = float.Parse(lblStdRoll.Text);
                    if (rollNetKg > _stdRollWeight)
                    {
                        row.DefaultCellStyle.BackColor = Color.Yellow;
                    }
                    else if (rollNetKg < _stdRollWeight)
                    {
                        row.DefaultCellStyle.BackColor = Color.Red;
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            /*//Combinar celdas (Quitar valor repetido)
            if (e.RowIndex == 0)
                return;
            if (IsTheSameCellValue(e.ColumnIndex, e.RowIndex))
            {
                e.Value = "";
                e.FormattingApplied = true;
            }*/
        }

        private async void dgRolls_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (_completedHistory) { return; }
            _rollByPallet++;
            _isPalletStart = true;

            if (lblMode.Text.Equals("Auto.")) { timerSchedule.Stop(); }

            TableLayoutPalletControl(int.Parse(lblRollOnPallet.Text), _rollByPallet, lblPalletNumber.Text);

            //Llenar campos de pallet (SUMA)
            float palletNetSum = dgRolls.Rows.Cast<DataGridViewRow>().Sum(t => float.Parse(t.Cells["R_Net"].Value.ToString()));
            lblCompletedQuantity.Text = palletNetSum.ToString();
            CalculateAdvace(palletNetSum);

            if (int.Parse(lblRollOnPallet.Text) > 1) { await LabelService.PrintP2(_rollCount, "ROLL"); }

            CreateRollApex();

            lblCore.Text = string.Empty; //Limpiar core hasta registrar

            //PALLET TERMINADO AGREGAR NUEVO
            if (_rollByPallet == int.Parse(lblRollOnPallet.Text))
            {
                AddPallet();
            }

            //Activar boton para terminar orden
            btnEndProcess.Visible = _rollCount > 0 ? true : false;
            btnSwapMode.Enabled = false;
        }

        //Click derecho sobre fila
        private void dgWeights_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            //if (_rollByPallet > 0)
            //{
                if (e.Button == MouseButtons.Right && e.RowIndex == dgRolls.Rows.Count - 1)
                {
                    dgRolls.Rows[e.RowIndex].Selected = true;
                    _rowSelected = e.RowIndex;
                    dgRolls.CurrentCell = dgRolls.Rows[e.RowIndex].Cells["R_Net"];
                    MenuShipWeight.Show(dgRolls, e.Location);
                    MenuShipWeight.Show(Cursor.Position);
                }
            /*}
            else
            {
                NotifierController.Warning("Acción no permitida");
            }*/
        }
        #endregion

        /*-------------------------------- PALLETS -----------------------------------*/
        #region DataGrid Pallets
        private void AddPallet()
        {
            if (_isPalletStart)
            {
                float totalCore = dgRolls.Rows.Cast<DataGridViewRow>().Where(t => t.Cells["R_Pallet"].Value.ToString().Equals(_palletCount.ToString()))
                .Sum(t => {
                    float value;
                    return float.TryParse(t.Cells["R_Core"].Value?.ToString(), out value) ? value : 0;
                });

                float paletNet = dgRolls.Rows.Cast<DataGridViewRow>().Where(t => t.Cells["R_Pallet"].Value.ToString().Equals(_palletCount.ToString()))
                .Sum(t => {
                    float value;
                    return float.TryParse(t.Cells["R_Net"].Value?.ToString(), out value) ? value : 0;
                });

                float grosPalletKG = paletNet + _tareWeight + totalCore; //Lamina + Tara + Cores

                string[] rowPallet = new string[] { _palletCount.ToString(), _tareWeight.ToString("F1"), totalCore.ToString("F1"), paletNet.ToString("F1"), grosPalletKG.ToString("F1") };
                int indexNewPallet = dgPallets.Rows.Add(rowPallet);
                dgPallets.FirstDisplayedScrollingRowIndex = indexNewPallet;

                //Limpiar datos para pallet nuevo
                _rollByPallet = 0;
                _isPalletStart = false;
                tabLayoutPallet.BackgroundImage = Resources.pallet_empty;
                TableLayoutPalletControl(0, _rollByPallet, lblPalletNumber.Text);

                lblStatusProcess.Text = "¡Coloque y pese TARA!";
                lblStatusProcess.ForeColor =  Color.Red;

                btnGetWeight.Text = "TARAR";
                btnGetWeight.BackColor = Color.Red;
                btnGetWeight.ForeColor = Color.White;


                lblPalletNet.Text = string.Empty;
                lblCore.Text = string.Empty;
                lblTare.Text = string.Empty;
            }
            else
            {
                //NotifierController.Warning("Aún no cuenta con datos de pesaje del nuevo palet");
                if(_endWeight)
                {
                    //await FileController.Write(cmbWorkOrders.SelectedItem.ToString(), Constants.OrdersPrintedP2);
                    cmbWorkOrders.Items.Clear();
                    ClearAll();
                }
            }
        }

        private string WeightsPallet(int palletSelected)
        {
            IEnumerable<string> columnWeigthsNetKg = dgRolls.Rows.Cast<DataGridViewRow>().Where(row => row.Cells["R_Pallet"].Value.ToString().Equals(palletSelected.ToString()))
                                                                                            .Select(row => row.Cells["R_Net"].Value.ToString());
            IEnumerable<string> columnRollNumber = dgRolls.Rows.Cast<DataGridViewRow>().Where(row => row.Cells["R_Pallet"].Value.ToString().Equals(palletSelected.ToString()))
                                                                                            .Select(row => row.Cells["R_Roll"].Value.ToString());

            string[] weigthsArray = columnWeigthsNetKg.ToArray();
            string[] rollsNumArray = columnRollNumber.ToArray();
            string strWeights = "";

            //weigthsArray.ToList().ForEach(i => );
            for (int i = 0; i < weigthsArray.Length; i++)
            {
                strWeights += $"{rollsNumArray[i]}-{weigthsArray[i]},";
            }

            return strWeights.TrimEnd(',');
        }

        private async void dgPallets_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if(_completedHistory) { return; }

            EnableCloseButton();
            int palletAdded = int.Parse(dgPallets.Rows[e.RowIndex].Cells["P_Pallet"].Value.ToString());
            string rollWeights = WeightsPallet(palletAdded);

            string[] palletWeight = new string[5];
            for (int i = 0; i < 5; i++)
            {
                palletWeight[i] = dgPallets.Rows[e.RowIndex].Cells[i].Value.ToString();
            }

            FillLabelPallet(palletWeight, rollWeights);
            await LabelService.PrintP2(palletAdded, "PALLET");

            //TERMINA PROCESO DE PESAJE PARA LA ORDEN SELECCIONADA
            if (palletAdded == int.Parse(lblPalletTotal.Text) || _endWeight)
            {
                //await FileController.Write(cmbWorkOrders.SelectedItem.ToString(), Constants.OrdersPrintedP2);
                cmbWorkOrders.Items.Clear();
                ClearAll();
            }
        }

        //Cambio de color de filas (Max-Min)
        private void dgPallets_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //Cambiar color
            foreach (DataGridViewRow row in dgPallets.Rows)
            {
                float palletNetKg = float.Parse(row.Cells["P_Net"].Value.ToString());
                if (!string.IsNullOrEmpty(lblStdPallet.Text))
                {
                    float _stdPalletWeight = float.Parse(lblStdPallet.Text);
                    if (palletNetKg > _stdPalletWeight)
                    {
                        row.DefaultCellStyle.BackColor = Color.Yellow;
                    }
                    else if (palletNetKg < _stdPalletWeight)
                    {
                        row.DefaultCellStyle.BackColor = Color.Red;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        #endregion

        /*-------------------------------- LABELS ------------------------------------*/
        #region Labels Fill
        private async void FillLabelRoll(string[] weights)
        {
            if (!string.IsNullOrEmpty(lblResourceCode.Text))
            {
                dynamic label = JObject.Parse(Constants.LabelJson);

                //WO Info
                label.WORKORDER = string.IsNullOrEmpty(_workOrderNumber) ? " " : _workOrderNumber/*.Substring(7)*/;
                label.ITEMNUMBER = string.IsNullOrEmpty(lblItemNumber.Text) ? " " : lblItemNumber.Text;
                label.ITEMDESCRIPTION = string.IsNullOrEmpty(lblItemDescription.Text) ? " " : lblItemDescription.Text;
                label.ENGLISHDESCRIPTION = string.IsNullOrEmpty(lblItemDescriptionEnglish.Text) || lblItemDescriptionEnglish.Text.Equals(lblItemDescription.Text) ? " " : lblItemDescriptionEnglish.Text;
                label.EQU = string.IsNullOrEmpty(lblResourceCode.Text) ? " " : lblResourceCode.Text;
                label.DATE = DateService.Now();
                label.SHIFT = string.IsNullOrEmpty(lblShift.Text) ? " " : lblShift.Text;
                //Roll Info
                label.ROLL = string.IsNullOrEmpty(weights[1]) ? " " : "R" + weights[1].PadLeft(4, '0');
                label.WNETKG = string.IsNullOrEmpty(weights[3]) ? " " : weights[3];
                label.WGROSSKG = string.IsNullOrEmpty(weights[4]) ? " " : weights[4]; // tara + core + rollo 
                label.WNETLBS = string.IsNullOrEmpty(weights[3]) ? " " : Pounds(float.Parse(weights[3]));
                label.WGROSSLBS = string.IsNullOrEmpty(weights[4]) ? " " : Pounds(float.Parse(weights[4]));
                label.WIDTHTHICKNESS = string.IsNullOrEmpty(strWithThickness) ? " " : strWithThickness;
                //AKA Info
                label.AKAITEM = string.IsNullOrEmpty(lblAkaItem.Text) ? "NE" : lblAkaItem.Text;
                label.AKADESCRIPTION = string.IsNullOrEmpty(lblAkaDescription.Text) ? "NE" : lblAkaDescription.Text;
                label.LEGALENTITY = string.IsNullOrEmpty(_tradingPartnerName) ? "NE" : _tradingPartnerName;
                label.PURCHASEORDER = string.IsNullOrEmpty(_CustomerPONumber) ? " " : _CustomerPONumber;
                label.PONUM = string.IsNullOrEmpty(_CustomerPONumber) ? " " : _CustomerPONumber.Contains("TPRM") ? _CustomerPONumber.Replace("TPRM", "") : _CustomerPONumber;

                Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);
                picLabelRoll.Image = System.Drawing.Image.FromStream(await LabelService.UpdateLabelLabelary(_rollCount, "ROLL"));
            }
        }

        private async void FillLabelPallet(string[] palletWeight, string rollWeights)
        {
            if (!string.IsNullOrEmpty(lblResourceCode.Text))
            {
                dynamic label = JObject.Parse(Constants.LabelJson);
                //WO Info
                label.WORKORDER = string.IsNullOrEmpty(_workOrderNumber) ? " " : _workOrderNumber/*.Substring(7)*/;
                label.ITEMNUMBER = string.IsNullOrEmpty(lblItemNumber.Text) ? " " : lblItemNumber.Text;
                label.ITEMDESCRIPTION = string.IsNullOrEmpty(lblItemDescription.Text) ? " " : lblItemDescription.Text;
                label.ENGLISHDESCRIPTION = string.IsNullOrEmpty(lblItemDescriptionEnglish.Text) || lblItemDescriptionEnglish.Text.Equals(lblItemDescription.Text) ? " " : lblItemDescriptionEnglish.Text;
                label.EQU = string.IsNullOrEmpty(lblResourceCode.Text) ? " " : lblResourceCode.Text;
                label.DATE = DateService.Now();
                label.SHIFT = string.IsNullOrEmpty(lblShift.Text) ? " " : lblShift.Text;
                //AKA Info
                label.AKAITEM = string.IsNullOrEmpty(lblAkaItem.Text) ? "NE" : lblAkaItem.Text;
                label.AKADESCRIPTION = string.IsNullOrEmpty(lblAkaDescription.Text) ? "NE" : lblAkaDescription.Text;
                label.LEGALENTITY = string.IsNullOrEmpty(_tradingPartnerName) ? "NE" : _tradingPartnerName;
                label.PURCHASEORDER = string.IsNullOrEmpty(_CustomerPONumber) ? " " : _CustomerPONumber;
                label.PONUM = string.IsNullOrEmpty(_CustomerPONumber) ? " " : _CustomerPONumber.Contains("TPRM") ? _CustomerPONumber.Replace("TPRM", "") : _CustomerPONumber;
                //Pallet Info
                label.PALET = "P" + _palletCount.ToString().PadLeft(4, '0');
                label.WNETKG = string.IsNullOrEmpty(palletWeight[3]) ? " " : palletWeight[3];
                label.WGROSSKG = string.IsNullOrEmpty(palletWeight[4]) ? " " : palletWeight[4];
                label.WNETLBS = string.IsNullOrEmpty(palletWeight[3]) ? " " : Pounds(float.Parse(palletWeight[3]));
                label.WGROSSLBS = string.IsNullOrEmpty(palletWeight[4]) ? " " : Pounds(float.Parse(palletWeight[4]));
                label.WEIGHTS = string.IsNullOrEmpty(rollWeights) ? " " : rollWeights;

                Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);
                picLabelPallet.Image = System.Drawing.Image.FromStream(await LabelService.UpdateLabelLabelary(_palletCount, "PALLET"));
            }
        }
        #endregion

        /*------------------------------- CONTROLS -----------------------------------*/
        #region Controls
        private void ClearAll()
        {
            //Weight Section
            lblTare.Text = string.Empty;
            lblCore.Text = string.Empty;
            lblWeight.Text = string.Empty;

            btnGetWeight.Enabled = false;
            btnGetWeight.Text = "TARAR";
            btnGetWeight.BackColor = Color.Red;
            btnGetWeight.ForeColor = Color.White;

            //Shift Section
            timerShift.Stop();
            /*lblShift.Text = string.Empty;*/

            //WorkOrder Section
            lblWOStatus.ForeColor = Color.DarkGray;
            TipStatusWO.SetToolTip(lblWOStatus, "Status orden");

            //cmbWorkOrders.Items.Clear();
            _workOrderId = 0;
            _workOrderNumber = string.Empty;
            cmbWorkOrders.Enabled = lblMode.Text.Equals("Auto.") ? false : true;
            lblPrimaryProductQuantity.Text = string.Empty;
            lblCompletedQuantity.Text = string.Empty;
            lblUoM.Text = "--";
            progressBarWO.Value = 0;
            lblAdvance.Text = "0%";
            lblPlannedStartDate.Text = string.Empty;
            lblPlannedCompletionDate.Text = string.Empty;
            lblItemNumber.Text = string.Empty;
            lblItemDescription.Text = string.Empty;
            lblItemDescriptionEnglish.Text = string.Empty;

            //AKA Section
            _akaCustomer = "DEFAULT";
            lblAkaSaleOrder.Text = string.Empty;
            lblAkaCustomerPO.Text = string.Empty;
            lblAkaCustomerNumber.Text = string.Empty;
            lblAkaCustomerName.Text = string.Empty;
            _CustomerPONumber = string.Empty;
            _tradingPartnerName = string.Empty;
            lblAkaItem.Text = string.Empty;
            lblAkaDescription.Text = string.Empty;
            strWithThickness = string.Empty; //Ancho y grosor

            //Pallet Anim Section
            _rollByPallet = 0;
            _isPalletStart = false;
            _completedHistory = false;
            tabLayoutPallet.BackgroundImage = Resources.pallet_empty;
            TableLayoutPalletControl(0, 0, lblPalletNumber.Text);

            //Weigth Section
            lblStdRoll.Text = string.Empty;
            lblWeightUOMRoll.Text = "--";
            lblStdPallet.Text = string.Empty;
            lblWeightUOMPallet.Text = "--";
            lblRollOnPallet.Text = string.Empty;
            lblPalletTotal.Text = string.Empty;

            lblTare.Text = string.Empty;
            lblPalletNumber.Text = string.Empty;

            //DataGrid Rolls Section
            _rollCount = 0;
            dgRolls.Rows.Clear();
            dgRolls.Refresh();
            lblLabelName.Text = string.Empty;
            picLabelRoll.Image = null;

            //DataGrid Pallets Section
            _palletCount = 0;
            dgPallets.Rows.Clear();
            dgPallets.Refresh();
            picLabelPallet.Image = null;

            _endWeight = false;
            btnEndProcess.Visible = false;

            //Orden inicio a pesar
            _startOrder = false;
            btnSwapMode.Enabled = true;

            _tareWeight = 0;

            //APEX Flags
            _apexCreated = false;
            _apexUpdated = false;

            if (lblMode.Text.Equals("Auto."))
            {
                ProductionScheduling(this, EventArgs.Empty);
            }
        }

        private void ShowWait(bool show, string message = "")
        {
            lblStatusProcess.ForeColor = Color.Black;
            lblStatusProcess.Text = message;
            pbWaitProcess.Visible = show;
        }

        private void CalculateAdvace(float completed)
        {
            if (string.IsNullOrEmpty(lblPrimaryProductQuantity.Text))
            {
                return;
            }
            else
            {
                float goal = float.Parse(lblPrimaryProductQuantity.Text);
                if (goal == 0 || completed == 0)
                {
                    progressBarWO.Value = 0;
                    lblAdvance.Text = "0%";
                }
                else
                {
                    if (completed >= float.Parse(lblPrimaryProductQuantity.Text))
                    {
                        progressBarWO.Value = 100;
                        lblAdvance.Text = "100%";
                    }
                    else
                    {
                        int advance = Convert.ToInt32(Math.Round((completed * 100) / float.Parse(lblPrimaryProductQuantity.Text)));
                        if (advance > 100)
                        {
                            progressBarWO.Value = 100;
                            lblAdvance.Text = "100%";
                        }
                        else
                        {
                            progressBarWO.Value = advance;
                            lblAdvance.Text = advance + "%";
                        }
                    }
                }
            }
        }

        private string Pounds(float kilo)
        {
            float conversion = kilo * 2.20462f;
            string result = string.Empty;
            if(_CustomerPONumber.Contains("TPRM"))
            {
                result = Math.Round(conversion).ToString();
            }
            else
            {
                result = (Math.Truncate(conversion * 10) / 10).ToString("0.0");
            }
            return result;
        }

        private string UnRound(float quantity)
        {
            return (Math.Truncate(quantity * 10) / 10).ToString("0.0");
        }

        private void flowLayoutMain_SizeChanged(object sender, EventArgs e)
        {
            if (flowLayoutMain.Size.Height > 700)
            {
                int boxes = groupBoxProd.Size.Height + groupBoxAka.Size.Height + groupBoxWeight.Size.Height;
                int total = (flowLayoutMain.Size.Height - boxes) - 20;
                int divide = total / 3;
                groupBoxProd.Height = groupBoxProd.Size.Height + divide;
                groupBoxAka.Height = groupBoxAka.Size.Height + divide;
                groupBoxWeight.Height = groupBoxWeight.Size.Height + divide;
            }

            flowLayoutMain.HorizontalScroll.Maximum = 0;
            flowLayoutMain.HorizontalScroll.Enabled = false;
        }

        private void frmLabelP2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (!string.IsNullOrEmpty(lblCompletedQuantity.Text) || !string.IsNullOrEmpty(lblTare.Text))
                {
                    DialogResult result = MessageBox.Show("¿Seguro que desea cerra la aplicación?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.No)
                    {
                        e.Cancel = true;
                    }
                    else if(result == DialogResult.Yes)
                    { 
                        /*if(_tareWeight > 0)
                        {
                            e.Cancel = true;
                            NotifierController.Warning("No se permite cerrar la aplicación cuando se inicio a pesar");
                        }
                        else
                        {
                            e.Cancel = false;
                        }*/

                        /*if (_rollByPallet > 1)
                        {
                            _endWeight = true;
                            AddPallet();
                            NotifierController.Success("Palet registrado");
                            Thread.Sleep(2000);
                            e.Cancel = false;
                        }*/
                    }
                }
            }
        }

        private void DisableCloseButton()
        {
            IntPtr hMenu = GetSystemMenu(this.Handle, false);
            EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED | MF_DISABLED);
        }

        private void EnableCloseButton()
        {
            IntPtr hMenu = GetSystemMenu(this.Handle, false);
            EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_ENABLED);
        }

        public static void SetLabelStatusPrint(string text)
        {
            if (instance != null && instance.lblStatusProcess != null)
            {
                instance.lblStatusProcess.Text = text;
            }
        }
        #endregion

        /*--------------------------------- APEX -------------------------------------*/
        #region APEX
        private async void CreateRollApex()
        {
            if (!_apexCreated)
            {
                dynamic jsonRoll = JObject.Parse(Payloads.weightRolls);
                jsonRoll.DateMark = DateService.EpochTime();
                jsonRoll.OrganizationId = Int64.Parse(Constants.Plant2Id);
                jsonRoll.WorkOrderId = _workOrderId;
                jsonRoll.WorkOrder = _workOrderNumber;
                jsonRoll.ItemNumber = lblItemNumber.Text;
                jsonRoll.Pallet = lblPalletNumber.Text;
                jsonRoll.Roll = _rollCount.ToString();
                jsonRoll.Tare = lblTare.Text;
                jsonRoll.Core = lblCore.Text;
                jsonRoll.Net = lblWeight.Text;
                jsonRoll.Shift = lblShift.Text;

                _lastApexCreate = JsonConvert.SerializeObject(jsonRoll, Formatting.Indented);
                _apexCreated = true;
            }

            if (AppController.CheckInternetConnection())
            {
                Task<string> postWeightRoll = APIService.PostApexAsync(EndPoints.WeightRolls, _lastApexCreate);
                string response = await postWeightRoll;

                if (!string.IsNullOrEmpty(response))
                {
                    dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(response);
                    if (responsePayload.ErrorsExistFlag.ToString() == "false")
                    {
                        Console.WriteLine($"{responsePayload.Message} [{DateService.Today()}]", Color.Green);
                        _apexCreated = false;
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show($"{responsePayload.Message}, vuelva a reintentar", "[APEX] Registro fallido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (result == DialogResult.OK)
                        {
                            CreateRollApex();
                        }
                        else
                        {
                            CreateRollApex();
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Sin respuesta al registrar rollo [{DateService.Today()}]", Color.Red);
                }
            }
            else
            {
                NotifierController.Warning("Sin conexión a internet");
                DialogResult result = MessageBox.Show("Verificar la conexión internet", "Sin Internet", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                if (result == DialogResult.OK)
                {
                    CreateRollApex();
                }
                else
                {
                    CreateRollApex();
                }
            }
        }

        private async void UpdateRollApex(int roll, float net)
        {
            if (!_apexUpdated)
            {
                dynamic jsonRoll = JObject.Parse(Payloads.weightRollUpdate);

                jsonRoll.OrganizationId = Int64.Parse(Constants.Plant2Id); ;
                jsonRoll.WorkOrder = _workOrderNumber;
                jsonRoll.Roll = roll;
                jsonRoll.Net = net;

                _lastApexUpdate = JsonConvert.SerializeObject(jsonRoll, Formatting.Indented);
                _apexUpdated = true;
            }
                
            if (AppController.CheckInternetConnection())
            {
                Task<string> putWeightRoll = APIService.PutApexAsync(String.Format(EndPoints.WeightRolls, _workOrderNumber, roll, Constants.Plant2Id), _lastApexUpdate);
                string response = await putWeightRoll;

                if (!string.IsNullOrEmpty(response))
                {
                    dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(response);
                    if (responsePayload.ErrorsExistFlag.ToString() == "false")
                    {
                        Console.WriteLine($"{responsePayload.Message} [{DateService.Today()}]", Color.Green);
                        _apexUpdated = false;
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show($"{responsePayload.Message}, vuelva a reintentar", "[APEX] Actualización fallida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (result == DialogResult.OK)
                        {
                            UpdateRollApex(roll, net);
                        }
                        else
                        {
                            UpdateRollApex(roll, net);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Sin respuesta al actualizar rollo [{DateService.Today()}]", Color.Red);
                }
            }
            else
            {
                NotifierController.Warning("Sin conexión a internet");
                DialogResult result = MessageBox.Show("Verificar la conexión internet", "Sin Internet", MessageBoxButtons.OK, MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    UpdateRollApex(roll, net);
                }
                else
                {
                    UpdateRollApex(roll, net);
                }
            }
        }
        #endregion
    }
}
