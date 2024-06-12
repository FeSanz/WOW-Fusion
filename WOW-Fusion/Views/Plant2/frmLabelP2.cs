﻿using Newtonsoft.Json;
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

namespace WOW_Fusion
{
    public partial class frmLabelP2 : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        private const uint SC_CLOSE = 0xF060;
        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint MF_GRAYED = 0x00000001;
        private const uint MF_ENABLED = 0x00000000;
        private const uint MF_DISABLED = 0x00000002;

        Random rnd = new Random();
        //Timer timerShift = new Timer();

        PopController pop;

        //Pesos params
        private float _tareWeight = 0.0f;
        private float _weightFromWeighing = 0.0f;
        private float _previousWeight = 0.0f;
        private float _lbs = 2.205f;
        //Ancho y espesor
        private string strWithThickness = string.Empty;
        private string _akaCustomer = "DEFAULT";

        private int _rowSelected = 0;

        //Rollos pallet control
        private int _rollCount = 0;
        private int _rollByPallet = 0;
        private int _palletCount = 0;
        private bool _isPalletStart = false;
        private bool _endWeight = false;
        private bool _completedHistory = false;

        //JObjets response
        private dynamic shifts = null;

        //Scheduling
        List<WorkOrderShedule> ordersForSchedule;
        private bool _startOrder = false;
        private bool _startThreadSchedule = false;

        #region Start
        public frmLabelP2()
        {
            InitializeComponent();
            InitializeFusionData();
        }

        private void frmLabelP2_Load(object sender, EventArgs e)
        {
            lblEnvironment.Text = Settings.Default.FusionUrl.Contains("-test") ? "TEST" : "PROD";
            pop = new PopController();

            richTextConsole.Clear();
            Console.SetOut(new ConsoleController(richTextConsole));

            AppController.ToolTip(btnSettings, "Configuración");
            AppController.ToolTip(pbRed, "Peso debajo del estándar");
            AppController.ToolTip(pbYellow, "Peso encima del estándar");

            TipStatusWO.SetToolTip(lblWOStatus, "Status orden");;

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
            pop.Close();
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
            pop.Close();
            WorkOrderUIFill(cmbWorkOrders.SelectedItem.ToString());
        }

        //Obtener datos de la orden seleccionada
        private async void WorkOrderUIFill(string workOrder)
        {
            cmbWorkOrders.Enabled = false;
            pop.Show(this);

            try
            {
                //♥ Consultar WORKORDER ♥
                Task<string> tskWorkOrdersData = APIService.GetRequestAsync(String.Format(EndPoints.WOProcessData, workOrder, Constants.Plant2Id));
                string response = await tskWorkOrdersData;
                
                if (string.IsNullOrEmpty(response)) 
                { 
                    pop.Close();
                    cmbWorkOrders.Enabled = lblMode.Text.Equals("Auto.") ? false : true;
                    return; 
                }

                JObject objWorkOrder = JObject.Parse(response);
                if ((int)objWorkOrder["count"] == 0)
                {
                    pop.Close();
                    NotifierController.Warning("Datos de orden no encotrada");
                    cmbWorkOrders.Enabled = lblMode.Text.Equals("Auto.") ? false : true;
                    return;
                }
                dynamic wo = objWorkOrder["items"][0]; //Objeto WORKORDER
                lblPrimaryProductQuantity.Text = string.IsNullOrEmpty(wo.PrimaryProductQuantity.ToString()) ? 0 : wo.PrimaryProductQuantity.ToString();
                //lblCompletedQuantity.Text = wo.CompletedQuantity.ToString();
                lblUoM.Text = wo.UOMCode.ToString();
                if (!string.IsNullOrEmpty(wo.CompletedQuantity.ToString()))
                {
                    pop.Close();
                    NotifierController.Warning($"Orden con despacho registrado [{wo.CompletedQuantity.ToString()} {lblUoM.Text}], verifique antes de pesar");
                }

                lblItemNumber.Text = wo.ItemNumber.ToString();
                lblItemDescription.Text = wo.Description.ToString();
                lblItemDescriptionEnglish.Text = TranslateService.Translate(lblItemDescription.Text.ToString());

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
                        pop.Close();
                        MessageBox.Show("Peso estándar no definido", "Verificar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    lblAkaOrder.Text = "NA";
                    _akaCustomer = "DEFAULT";
                }
                else
                {
                    lblAkaOrder.Text = flexPV;

                    //♥ Consultar OM & AKA ♥
                    dynamic om = await CommonService.OneItem(String.Format(EndPoints.SalesOrders, lblAkaOrder.Text, Constants.BusinessUnitId));
                    if (om != null)
                    {
                        lblAkaCustomer.Text = om.BuyingPartyNumber.ToString();//BuyingPartyName
                        _akaCustomer = lblAkaCustomer.Text;

                        //♥ Consultar TradingPartnerItemRelationships ♥
                        dynamic aka = await CommonService.OneItem(String.Format(EndPoints.TradingPartnerItemRelationships, lblItemNumber.Text, lblAkaCustomer.Text));
                        if (aka != null)
                        {
                            lblAkaItem.Text = aka.TradingPartnerItemNumber.ToString();
                            lblAkaDescription.Text = aka.RelationshipDescription.ToString();
                        }
                        else
                        {
                            lblAkaItem.Text = string.Empty;
                            lblAkaDescription.Text = string.Empty;
                            NotifierController.Warning($"Producto no relacionado con el cliente AKA");
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
                    pop.Close();
                    MessageBox.Show("Etiqueta de cliente/producto no encontrada", "Verificar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    lblLabelName.Text = labelApex.LabelName.ToString();
                }

                //Verificar Historial Pesaje----------------------------
                Task<string> tskHistory = APIService.GetApexAsync(String.Format(EndPoints.WeightRollHistory, cmbWorkOrders.Text));
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
                    dynamic weightHistory = JsonConvert.DeserializeObject<dynamic>(responseHistory);
                    lblCompletedQuantity.Text = weightHistory.Completed.ToString();
                    if(weightHistory.Completed > 0)
                    {
                        _completedHistory = true;
                        lblCompletedQuantity.Text = weightHistory.Completed.ToString();
                        CalculateAdvace(float.Parse(lblCompletedQuantity.Text));
                        FillDatagridsFromHistory(weightHistory);
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
                    btnGetWeight.Enabled = lblAkaOrder.Text.Equals("NA") && _akaCustomer.Equals("DEFAULT") ? true : 
                                           btnGetWeight.Enabled = string.IsNullOrEmpty(lblAkaItem.Text) ? false : true;
                }
                else
                {
                    btnGetWeight.Enabled = false;
                }

                //Order completada
                if (float.Parse(lblCompletedQuantity.Text) >= float.Parse(lblPrimaryProductQuantity.Text))
                {
                    NotifierController.Warning("Orden completada");
                    btnGetWeight.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                pop.Close();
                cmbWorkOrders.Enabled = lblMode.Text.Equals("Auto.") ? false : true;
                MessageBox.Show("Error. " + ex.Message, "Error al seleccionar orden", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            pop.Close();
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
        private void FillDatagridsFromHistory(dynamic weightHistory)
        {
            dynamic pallets = weightHistory.Pallets;
            foreach (dynamic p in pallets)
            {
                float gross = p.Tare + p.Weight;
                float netLbs = p.Weight * _lbs;
                float grossLbs = gross * _lbs; 
                string[] palletData = new string[] { p.Pallet.ToString(), p.Tare.ToString(), p.Weight.ToString(),
                                                    gross.ToString(), netLbs.ToString(), grossLbs.ToString() };
                int indexNewPallet = dgPallets.Rows.Add(palletData);
                dgPallets.FirstDisplayedScrollingRowIndex = indexNewPallet;

                dynamic rolls = p.Rolls;
                foreach (dynamic r in rolls)
                {
                    gross = p.Tare + r.Weight;
                    netLbs = r.Weight * _lbs;
                    grossLbs = gross * _lbs;
                    string[] rollData = new string[] { p.Pallet.ToString(), r.Roll.ToString(), r.Weight.ToString(),
                                                       gross.ToString(), netLbs.ToString(), grossLbs.ToString() };

                    //table.Rows.Add(p.Pallet, r.Roll, r.Weight.ToString(), gross.ToString(), netLbs.ToString(), grossLbs.ToString());
                    int indexNewRoll = dgRolls.Rows.Add(rollData);
                    dgRolls.FirstDisplayedScrollingRowIndex = indexNewRoll;
                }
            }

            _completedHistory = false;

            _palletCount = dgPallets.Rows.Count;
            _rollCount = dgRolls.Rows.Count;

            // Asegurarse de que las columnas permiten la ordenación
            /*foreach (DataGridViewColumn column in dgRolls.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dgRolls.Columns[1].SortMode = DataGridViewColumnSortMode.Automatic;*/
            //dgRolls.Sort(dgRolls.Columns["R_Roll"], ListSortDirection.Ascending);

            lblPalletNumber.Text = _palletCount.ToString();
        }

        #endregion

        #region Buttons Actions
        private async void btnGetWeight_Click(object sender, EventArgs e)
        {
            btnGetWeight.Enabled = false;
            txtBoxWeight.Text = string.Empty;
            pop.Show(this);
            if (string.IsNullOrEmpty(lblTare.Text))
            {
                //Solicitar peso tara
                string responseTare = await RadwagController.SocketWeighing("T");
                if (responseTare.Equals("OK"))
                {
                    string requestTareWeight = await RadwagController.SocketWeighing("OT");
                    if (!requestTareWeight.Equals("EX"))
                    {
                        //Inicia a pesar
                        /*if (!_startOrder)
                        {
                            //ProductionScheduling(this, EventArgs.Empty);
                            _startOrder = true;
                        }
                        else
                        {
                            timerSchedule.Stop();
                        }*/

                        //TARAR
                        _tareWeight = float.Parse(requestTareWeight);
                        if (_tareWeight > 0)
                        {
                            lblTare.Text = float.Parse(requestTareWeight).ToString("F2");
                            txtBoxWeight.Text = float.Parse(requestTareWeight).ToString("F2");
                            btnGetWeight.Text = "PESAR";
                            btnGetWeight.BackColor = Color.LimeGreen;
                            btnReloadTare.Visible = true;
                            //_rollCount = 0;
                            _rollByPallet = 0;
                            _palletCount += 1;
                            _previousWeight = 0;


                            tabLayoutPallet.BackgroundImage = Resources.pallet_filled;
                            TableLayoutPalletControl(int.Parse(lblRollOnPallet.Text), 0);

                            lblPalletNumber.Text = _palletCount.ToString();
                            cmbWorkOrders.Enabled = false; // Deshabilitar combo de Ordenes

                            //Registrar pallet en DB APEX
                            lblPalletId.Text = DateService.EpochTime();
                            CreatePalletApex(_palletCount, 0.0f);

                            //Desactivar todo opcion para salir para evitar palets incompletos
                            DisableCloseButton();
                            btnSwapMode.Enabled = false;
                            timerSchedule.Stop();
                            cmbWorkOrders.Enabled = false;
                        }
                        else
                        {
                            NotifierController.Warning($"Peso de tara invalido [{_tareWeight.ToString("F2")} kg]");
                        }
                    }
                    else
                    {
                        NotifierController.Warning("Tiempo de espera agotado, vuelva a  intentar");
                    }
                }
                else
                {
                    pop.Close();
                    string res = responseTare.Equals("EX") ? "Error de comunicación a báscula" : responseTare;
                    NotifierController.Error(res);
                }
            }
            else
            {
                //Obtiene peso acomulado (sin tara)
                string responseWeighing = await RadwagController.SocketWeighing("S");

                if (responseWeighing == "EX")
                {
                    NotifierController.Warning("Tiempo de espera agotado, vuelva a  intentar");
                }
                else
                {
                    //La bascula solo acomula el peso neto (SIN TARA)
                    if (float.TryParse(responseWeighing, out float _weightFromWeighing))
                    { 
                        if (_weightFromWeighing > 0)
                        {
                            if (_weightFromWeighing == _previousWeight)
                            {
                                pop.Close();
                                MessageBox.Show("Pesaje no ha cambiado, verifique." +
                                               "EL PESO NO SE AGRAGARA A LA LISTA",
                                               "¡¡Precaucion!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else if (_weightFromWeighing < _previousWeight)
                            {
                                pop.Close();
                                MessageBox.Show("Se detecto menor peso al obtenido anteriormente, " +
                                                "verifique el producto colocado, " +
                                                "EL PESO NO SE AGRAGARA A LA LISTA",
                                                "¡¡Precaucion!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                pop.Close();
                                //Llenar campos de pallet (NET siempre sera el peso acomulado de la bascula)
                                lblPalletNetKg.Text = _weightFromWeighing.ToString("F2");
                                lblPalletGrossKg.Text = (_weightFromWeighing + _tareWeight).ToString("F2");

                                lblPalletNetLbs.Text = (_weightFromWeighing * _lbs).ToString("F2");
                                lblPalletGrossLbs.Text = ((_weightFromWeighing + _tareWeight) * _lbs).ToString("F2");

                                //Calcular pero neto de cada rollo (SIN TARA)
                                float rollNetKg = _weightFromWeighing - _previousWeight;
                                txtBoxWeight.Text = rollNetKg.ToString("F2");

                                float rollNetLbs = rollNetKg * _lbs;
                                btnReloadTare.Visible = false;

                                //Calcular pero bruto de cada rollo (con tara)
                                float rollGrossKg = rollNetKg + _tareWeight;
                                float rollGrossLbs = rollGrossKg * _lbs;

                                _rollCount++;

                                //Agregar pesos a datagrid
                                string[] row = new string[] { _palletCount.ToString(), _rollCount.ToString(), rollNetKg.ToString("F2"),rollGrossKg.ToString("F2"),
                                                                        rollNetLbs.ToString("F2"), rollGrossLbs.ToString("F2") };


                                int indexNewRoll = dgRolls.Rows.Add(row);
                                dgRolls.FirstDisplayedScrollingRowIndex = indexNewRoll;

                                //Reserver peso neto acomulado para sacar peso de rollo
                                _previousWeight = _weightFromWeighing;

                                FillLabelRoll(row);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Peso invalido [{_weightFromWeighing.ToString("F2")} kg]", "Báscula", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            pop.Close();
        }

        private async void btnReloadTare_Click(object sender, EventArgs e)
        {
            pop.Show(this);
            string responseTare = await RadwagController.SocketWeighing("T");
            if (responseTare.Equals("OK"))
            {
                string requestTareWeight = await RadwagController.SocketWeighing("OT");
                if (!requestTareWeight.Equals("EX"))
                {
                    //TARAR
                    _tareWeight = float.Parse(requestTareWeight);
                    lblTare.Text = float.Parse(requestTareWeight).ToString("F2");
                    txtBoxWeight.Text = float.Parse(requestTareWeight).ToString("F2");
                }
                else
                {
                    NotifierController.Warning("Tiempo de espera agotado, vuelva a  intentar");
                }
            }
            pop.Close();
        }

        private void btnAddPallet_Click(object sender, EventArgs e)
        {
            /*if (_isPalletStart)
            {
                DialogResult dialogResult = MessageBox.Show($"¿Desea agregar nuevo palet?", "Agregar palet", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    string[] rowPallet = new string[] { _palletCount.ToString(), _tareWeight.ToString(), lblPalletNetKg.Text,lblPalletGrossKg.Text,
                                                                        lblPalletNetLbs.Text, lblPalletGrossLbs.Text};
                    dgPallets.Rows.Add(rowPallet);

                    //Limpiar datos para pallet nuevo
                    _rollByPallet = 0;
                    _isPalletStart = false;
                    tabLayoutPallet.BackgroundImage = Resources.pallet_empty;
                    TableLayoutPalletControl(int.Parse(lblRollOnPallet.Text),_rollByPallet);
                    btnGetWeight.Text = "TARA";

                    lblPalletNetKg.Text = string.Empty;
                    lblPalletGrossKg.Text = string.Empty;
                    lblPalletNetLbs.Text = string.Empty;
                    lblPalletGrossLbs.Text = string.Empty;
                    lblTare.Text = string.Empty;
                }
                else if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }
            else
            {
                NotifierController.Warning("Aún no cuenta con datos de pesaje del nuevo palet");
            }*/
        }

        private void btnSwapMode_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cmbWorkOrders.Text))
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
                _endWeight = true;
                AddPallet();
            }
        }
        #endregion

        #region DataGrid Rollos
        private void TableLayoutPalletControl(int rollOnPallet, int rollNumber)
        {
            lblRollCount.Text = $"{rollOnPallet}";

            if (rollOnPallet <= 12)
            {
                lblRollCount.Visible = false;
                int count = 0;

                tabLayoutPallet.Controls.Clear();
                tabLayoutPallet.RowStyles.Clear();
                tabLayoutPallet.ColumnStyles.Clear();

                tabLayoutPallet.ColumnCount = rollOnPallet > 4 ? 4 : rollOnPallet;

                if (rollOnPallet >= 9)
                {
                    tabLayoutPallet.RowCount = 3;
                }
                else if (rollOnPallet >= 5 && rollOnPallet <= 8)
                {
                    tabLayoutPallet.RowCount = 2;
                }
                else
                {
                    tabLayoutPallet.RowCount = 1;
                }

                //tabLayoutPallet.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

                int cells = tabLayoutPallet.RowCount * tabLayoutPallet.ColumnCount;

                for (int row = 0; row < tabLayoutPallet.RowCount; row++)
                {
                    tabLayoutPallet.RowStyles.Add(new RowStyle(SizeType.Percent, 1));
                    for (int col = 0; col < tabLayoutPallet.ColumnCount; col++)
                    {
                        PictureBox picRoll = new PictureBox();
                        if (count < rollNumber)
                        {
                            IEnumerable<string> columnWeigthsNetKg = dgRolls.Rows.Cast<DataGridViewRow>()
                                                                    .Where(fila => fila.Cells["R_Pallet"].Value.ToString().Equals(lblPalletNumber.Text))
                                                                    .Select(fila => fila.Cells["R_NetKg"].Value.ToString());

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
                            else
                            {
                                continue;
                            }

                            //picRoll.Image = Resources.roll;
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
                        }
                        tabLayoutPallet.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1));
                    }
                }
            }
            else
            {
                lblRollCount.Visible = true;
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
                float rollNetKg = float.Parse(row.Cells["R_NetKg"].Value.ToString());
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
            TableLayoutPalletControl(int.Parse(lblRollOnPallet.Text), _rollByPallet);

            //Llenar campos de pallet (SUMA)
            float palletNetSum = dgRolls.Rows.Cast<DataGridViewRow>().Sum(t => float.Parse(t.Cells["R_NetKg"].Value.ToString()));
            lblCompletedQuantity.Text = palletNetSum.ToString();
            CalculateAdvace(palletNetSum);

            if (int.Parse(lblRollOnPallet.Text) > 1) { await LabelService.PrintP2(_rollCount, "ROLL"); }

            //PALLET TERMINADO AGREGAR NUEVO
            if (_rollByPallet == int.Parse(lblRollOnPallet.Text))
            {
                AddPallet();
            }
            
            CreateRollApex(_rollCount, float.Parse(dgRolls.Rows[e.RowIndex].Cells["R_NetKg"].Value.ToString()));

            //Activar boton para terminar orden
            btnEndProcess.Visible = _rollCount > 0 ? true : false;
        }

        //Click derecho sobre fila
        private void dgWeights_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (_rollByPallet > 0)
            {
                if (e.Button == MouseButtons.Right && e.RowIndex == dgRolls.Rows.Count - 1)
                {
                    dgRolls.Rows[e.RowIndex].Selected = true;
                    _rowSelected = e.RowIndex;
                    dgRolls.CurrentCell = dgRolls.Rows[e.RowIndex].Cells["R_NetKg"];
                    MenuShipWeight.Show(dgRolls, e.Location);
                    MenuShipWeight.Show(Cursor.Position);
                }
            }
            else
            {
                NotifierController.Warning("Acción no permitica");
            }
        }

        private void deleteMenuItem_Click(object sender, EventArgs e)
        {
            if (!dgRolls.Rows[_rowSelected].IsNewRow)
            {
                //Restar peso eliminado en PESO PREVIO peara evitar inconsistencias
                _previousWeight -= float.Parse(dgRolls.CurrentRow.Cells["R_NetKg"].Value.ToString());
                dgRolls.Rows.RemoveAt(_rowSelected);
                lblPalletNetKg.Text = _previousWeight.ToString("F2");
                //Restar 1 a la cantidad de rollos
                _rollCount -= 1;
                _rollByPallet -= 1;
            }
        }

        private async void ReWeightMenuItem_Click(object sender, EventArgs e)
        {
            if (!dgRolls.Rows[_rowSelected].IsNewRow)
            {
                txtBoxWeight.Text = string.Empty;
                pop.Show(this);

                //Restar peso a recalcular en PESO PREVIO peara evitar inconsistencias
                _previousWeight -= float.Parse(dgRolls.CurrentRow.Cells["R_NetKg"].Value.ToString());
               
                lblPalletNetKg.Text = _previousWeight.ToString("F2");

                //Obtiene peso acomulado (sin tara)
                string responseWeighing = await RadwagController.SocketWeighing("S");

                if (responseWeighing == "EX")
                {
                    NotifierController.Warning("Tiempo de espera agotado, vuelva a  intentar");
                }
                else
                {
                    //La bascula solo acomula el peso neto (SIN TARA)
                    _weightFromWeighing = float.Parse(responseWeighing);

                    if (_weightFromWeighing > 0)
                    {
                        if (_weightFromWeighing < _previousWeight)
                        {
                            pop.Close();
                            MessageBox.Show("Se detecto menor peso al obtenido anteriormente, verifique el producto colocado, " +
                                            "EL PESO NO SE AGRAGARÁ A LA LISTA", "¡¡Precaucion!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            pop.Close();
                            //Llenar campos de pallet (NET siempre sera el peso acomulado de la bascula)
                            lblPalletNetKg.Text = _weightFromWeighing.ToString("F2");
                            lblPalletGrossKg.Text = (_weightFromWeighing + _tareWeight).ToString("F2");

                            lblPalletNetLbs.Text = (_weightFromWeighing * _lbs).ToString("F2");
                            lblPalletGrossLbs.Text = ((_weightFromWeighing + _tareWeight) * _lbs).ToString("F2");

                            //Calcular pero neto de cada rollo (SIN TARA)
                            float rollNetKg = _weightFromWeighing - _previousWeight;
                            txtBoxWeight.Text = rollNetKg.ToString("F2");

                            float rollNetLbs = rollNetKg * _lbs;
                            btnReloadTare.Visible = false;

                            //Calcular pero bruto de cada rollo (con tara)
                            float rollGrossKg = rollNetKg + _tareWeight;
                            float rollGrossLbs = rollGrossKg * _lbs;


                            //Agregar pesos a datagrid
                            string[] rowRoll = new string[] { _palletCount.ToString(), _rollCount.ToString(), rollNetKg.ToString("F2"),rollGrossKg.ToString("F2"),
                                                                        rollNetLbs.ToString("F2"), rollGrossLbs.ToString("F2") };

                            //Actualizar rollo
                            dgRolls.Rows[_rowSelected].SetValues(rowRoll);

                            float palletNetSum = dgRolls.Rows.Cast<DataGridViewRow>().Sum(t => float.Parse(t.Cells["R_NetKg"].Value.ToString()));
                            lblCompletedQuantity.Text = palletNetSum.ToString();
                            CalculateAdvace(palletNetSum);

                            TableLayoutPalletControl(int.Parse(lblRollOnPallet.Text), _rollByPallet);
                            FillLabelRoll(rowRoll);

                            await LabelService.PrintP2(_rollCount, "ROLL"); //Imprimir rollo
                            UpdateRollApex(_palletCount, _rollCount, rollNetKg);

                            //Verificar si es el ultimo rollo del pallet
                            float rollPerPallet = _rollCount / _palletCount;

                            if (rollPerPallet == int.Parse(lblRollOnPallet.Text) && btnGetWeight.Text.Equals("TARAR"))
                            {
                                string[] rowPallet = new string[] { _palletCount.ToString(), _tareWeight.ToString(), lblPalletNetKg.Text,
                                                    lblPalletGrossKg.Text, lblPalletNetLbs.Text, lblPalletGrossLbs.Text };

                                //Actualizar pallet
                                int latestIndexPallet = dgPallets.Rows.Count - 1;
                                dgPallets.Rows[latestIndexPallet].SetValues(rowPallet);

                                string rollWeights = WeightsPallet(latestIndexPallet + 1);

                                string[] palletWeight = new string[6];
                                for (int i = 0; i < 6; i++)
                                {
                                    palletWeight[i] = dgPallets.Rows[latestIndexPallet].Cells[i].Value.ToString();
                                }

                                FillLabelPallet(palletWeight, rollWeights);
                                await LabelService.PrintP2(dgPallets.Rows.Count, "PALLET");
                                TableLayoutPalletControl(0, _rollByPallet);

                                UpdatePalletApex(latestIndexPallet + 1, float.Parse(dgPallets.Rows[latestIndexPallet].Cells["P_Tare"].Value.ToString()),
                                                              float.Parse(dgPallets.Rows[latestIndexPallet].Cells["P_NetKg"].Value.ToString()));
                            }
                            //Reserver peso neto acomulado para sacar peso de rollo
                            _previousWeight = _weightFromWeighing;
                            
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Peso invalido [{_weightFromWeighing.ToString("F2")} kg], vuelva a intentar", "Báscula", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
        #endregion

        #region DataGrid Pallets
        private void AddPallet()
        {
            if (_isPalletStart)
            {
                string[] rowPallet = new string[] { _palletCount.ToString(), _tareWeight.ToString(), lblPalletNetKg.Text,
                                                    lblPalletGrossKg.Text, lblPalletNetLbs.Text, lblPalletGrossLbs.Text };
                int indexNewPallet = dgPallets.Rows.Add(rowPallet);
                dgPallets.FirstDisplayedScrollingRowIndex = indexNewPallet;

                //Limpiar datos para pallet nuevo
                _rollByPallet = 0;
                _isPalletStart = false;
                tabLayoutPallet.BackgroundImage = Resources.pallet_empty;
                TableLayoutPalletControl(0, _rollByPallet);
                btnGetWeight.Text = "TARAR";
                btnGetWeight.BackColor = Color.DarkOrange;

                lblPalletNetKg.Text = string.Empty;
                lblPalletGrossKg.Text = string.Empty;
                lblPalletNetLbs.Text = string.Empty;
                lblPalletGrossLbs.Text = string.Empty;
                lblTare.Text = string.Empty;
            }
            else
            {
                //NotifierController.Warning("Aún no cuenta con datos de pesaje del nuevo palet");
                if(_endWeight)
                {
                    //await FileController.Write(cmbWorkOrders.SelectedItem.ToString(), Constants.OrdersPrintedP2);
                    ClearAll();
                }
            }
        }

        private string WeightsPallet(int palletSelected)
        {
            IEnumerable<string> columnWeigthsNetKg = dgRolls.Rows.Cast<DataGridViewRow>().Where(row => row.Cells["R_Pallet"].Value.ToString().Equals(palletSelected.ToString()))
                                                                                            .Select(row => row.Cells["R_NetKg"].Value.ToString());
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

            string[] palletWeight = new string[6];
            for (int i = 0; i < 6; i++)
            {
                palletWeight[i] = dgPallets.Rows[e.RowIndex].Cells[i].Value.ToString();
            }

            FillLabelPallet(palletWeight, rollWeights);
            await LabelService.PrintP2(palletAdded, "PALLET");

            UpdatePalletApex(palletAdded, float.Parse(dgPallets.Rows[e.RowIndex].Cells["P_Tare"].Value.ToString()),
                                          float.Parse(dgPallets.Rows[e.RowIndex].Cells["P_NetKg"].Value.ToString()));

            //TERMINA PROCESO DE PESAJE PARA LA ORDEN SELECCIONADA
            if (palletAdded == int.Parse(lblPalletTotal.Text) || _endWeight)
            {
                //await FileController.Write(cmbWorkOrders.SelectedItem.ToString(), Constants.OrdersPrintedP2);
                ClearAll();
            }
        }

        //Cambio de color de filas (Max-Min)
        private void dgPallets_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //Cambiar color
            foreach (DataGridViewRow row in dgPallets.Rows)
            {
                float palletNetKg = float.Parse(row.Cells["P_NetKg"].Value.ToString());
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

        #region Labels Fill
        private async void FillLabelRoll(string[] weights)
        {
            if (!string.IsNullOrEmpty(lblResourceCode.Text))
            {
                dynamic label = JObject.Parse(Constants.LabelJson);
                //WO Info
                label.WORKORDER = string.IsNullOrEmpty(cmbWorkOrders.Text) ? " " : cmbWorkOrders.Text/*.Substring(7)*/;
                label.ITEMNUMBER = string.IsNullOrEmpty(lblItemNumber.Text) ? " " : lblItemNumber.Text;
                label.ITEMDESCRIPTION = string.IsNullOrEmpty(lblItemDescription.Text) ? " " : lblItemDescription.Text;
                label.ENGLISHDESCRIPTION = string.IsNullOrEmpty(lblItemDescriptionEnglish.Text) || lblItemDescriptionEnglish.Text.Equals(lblItemDescription.Text) ? " " : lblItemDescriptionEnglish.Text;
                label.EQU = string.IsNullOrEmpty(lblResourceCode.Text) ? " " : lblResourceCode.Text;
                label.DATE = DateService.Now();
                label.SHIFT = string.IsNullOrEmpty(lblShift.Text) ? " " : lblShift.Text;
                //Roll Info
                label.ROLL = string.IsNullOrEmpty(weights[1]) ? " " : "R" + weights[1].PadLeft(4, '0');
                label.WNETKG = string.IsNullOrEmpty(weights[2]) ? " " : weights[2];
                label.WGROSSKG = string.IsNullOrEmpty(weights[3]) ? " " : weights[3];
                label.WNETLBS = string.IsNullOrEmpty(weights[4]) ? " " : weights[4];
                label.WGROSSLBS = string.IsNullOrEmpty(weights[5]) ? " " : weights[5];
                label.WIDTHTHICKNESS = string.IsNullOrEmpty(strWithThickness) ? " " : strWithThickness;
                //AKA Info
                label.AKAITEM = string.IsNullOrEmpty(lblAkaItem.Text) ? "NE" : lblAkaItem.Text;
                label.AKADESCRIPTION = string.IsNullOrEmpty(lblAkaDescription.Text) ? "NE" : lblAkaDescription.Text;
                label.LEGALENTITY = string.IsNullOrEmpty(lblAkaCustomer.Text) ? "NE" : lblAkaCustomer.Text;
                label.PURCHASEORDER = string.IsNullOrEmpty(lblAkaOrder.Text) ? " " : lblAkaOrder.Text;

                
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
                label.WORKORDER = string.IsNullOrEmpty(cmbWorkOrders.Text) ? " " : cmbWorkOrders.Text/*.Substring(7)*/;
                label.ITEMNUMBER = string.IsNullOrEmpty(lblItemNumber.Text) ? " " : lblItemNumber.Text;
                label.ITEMDESCRIPTION = string.IsNullOrEmpty(lblItemDescription.Text) ? " " : lblItemDescription.Text;
                label.ENGLISHDESCRIPTION = string.IsNullOrEmpty(lblItemDescriptionEnglish.Text) || lblItemDescriptionEnglish.Text.Equals(lblItemDescription.Text) ? " " : lblItemDescriptionEnglish.Text;
                label.EQU = string.IsNullOrEmpty(lblResourceCode.Text) ? " " : lblResourceCode.Text;
                label.DATE = DateService.Now();
                label.SHIFT = string.IsNullOrEmpty(lblShift.Text) ? " " : lblShift.Text;
                //AKA Info
                label.AKAITEM = string.IsNullOrEmpty(lblAkaItem.Text) ? "NE" : lblAkaItem.Text;
                label.AKADESCRIPTION = string.IsNullOrEmpty(lblAkaDescription.Text) ? "NE" : lblAkaDescription.Text;
                label.LEGALENTITY = string.IsNullOrEmpty(lblAkaCustomer.Text) ? "NE" : lblAkaCustomer.Text;
                label.PURCHASEORDER = string.IsNullOrEmpty(lblAkaOrder.Text) ? " " : lblAkaOrder.Text;
                //Pallet Info
                label.PALET = "P" + _palletCount.ToString().PadLeft(4, '0');
                label.WNETKG = string.IsNullOrEmpty(palletWeight[2]) ? " " : palletWeight[2];
                label.WGROSSKG = string.IsNullOrEmpty(palletWeight[3]) ? " " : palletWeight[3];
                label.WNETLBS = string.IsNullOrEmpty(palletWeight[4]) ? " " : palletWeight[4];
                label.WGROSSLBS = string.IsNullOrEmpty(palletWeight[5]) ? " " : palletWeight[5];
                label.WEIGHTS = string.IsNullOrEmpty(rollWeights) ? " " : rollWeights;

                Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);
                picLabelPallet.Image = System.Drawing.Image.FromStream(await LabelService.UpdateLabelLabelary(_palletCount, "PALLET"));
            }
        }
        #endregion

        #region Controls

        private void ClearAll()
        {
            //Weight Section
            txtBoxWeight.Text = string.Empty;
            btnGetWeight.Enabled = false;
            btnGetWeight.Text = "TARAR";
            btnGetWeight.BackColor = Color.DarkOrange;

            //Shift Section
            timerShift.Stop();
            /*lblShift.Text = string.Empty;*/

            //WorkOrder Section
            lblWOStatus.ForeColor = Color.DarkGray;
            TipStatusWO.SetToolTip(lblWOStatus, "Status orden");

            cmbWorkOrders.Items.Clear();
            cmbWorkOrders.Enabled = lblMode.Text.Equals("Auto.") ? false : true;
            lblPrimaryProductQuantity.Text = string.Empty;
            lblCompletedQuantity.Text = string.Empty;
            lblUoM.Text = "--";
            progressBarWO.Value = 0;
            lblAdvance.Text = "0%";
            lblPlannedStartDate.Text = string.Empty;
            lblPlannedCompletionDate.Text = string.Empty;
            /*lblResourceCode.Text = string.Empty;
            lblResourceName.Text = string.Empty;*/
            lblItemNumber.Text = string.Empty;
            lblItemDescription.Text = string.Empty;
            lblItemDescriptionEnglish.Text = string.Empty;

            //AKA Section
            _akaCustomer = "DEFAULT";
            lblAkaOrder.Text = string.Empty;
            lblAkaCustomer.Text = string.Empty;
            lblAkaItem.Text = string.Empty;
            lblAkaDescription.Text = string.Empty;
            strWithThickness = string.Empty; //Ancho y grosor

            //Pallet Anim Section
            _rollByPallet = 0;
            _isPalletStart = false;
            _completedHistory = false;
            tabLayoutPallet.BackgroundImage = Resources.pallet_empty;
            TableLayoutPalletControl(0, 0);

            //Weigth Section
            lblPalletId.Text = "*************";
            lblStdRoll.Text = string.Empty;
            lblWeightUOMRoll.Text = "--";
            lblStdPallet.Text = string.Empty;
            lblWeightUOMPallet.Text = "--";
            lblRollOnPallet.Text = string.Empty;
            lblPalletTotal.Text = string.Empty;

            lblPalletNetKg.Text = string.Empty;
            lblPalletGrossKg.Text = string.Empty;
            lblPalletNetLbs.Text = string.Empty;
            lblPalletGrossLbs.Text = string.Empty;
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

            if (lblMode.Text.Equals("Auto."))
            {
                ProductionScheduling(this, EventArgs.Empty);
            }
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
                        if(_tareWeight > 0)
                        {
                            e.Cancel = true;
                            NotifierController.Warning("No se permite cerrar la aplicación cuando se inicio a pesar");
                        }
                        else
                        {
                            e.Cancel = false;
                        }
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
        #endregion

        #region APEX
        private async void CreatePalletApex(int pallet, float weight)
        {
            dynamic jsonPallet = JObject.Parse(Payloads.weightPallet);
            
            jsonPallet.DateMark = lblPalletId.Text;
            jsonPallet.OrganizationId = Int64.Parse(Constants.Plant2Id);
            jsonPallet.WorkOrderNumber = cmbWorkOrders.Text;
            jsonPallet.ItemNumber = lblItemNumber.Text;
            jsonPallet.PalletNumber = pallet;
            jsonPallet.ContentRolls = int.Parse(lblRollOnPallet.Text);
            jsonPallet.Tare = float.Parse(lblTare.Text);
            jsonPallet.Weight = weight;
            jsonPallet.Shift = lblShift.Text;

            string jsonSerialized = JsonConvert.SerializeObject(jsonPallet, Formatting.Indented);

            Task<string> postWeightPallet = APIService.PostApexAsync(String.Format(EndPoints.WeightPallets, "WO", "Pallet", Constants.Plant2Id), jsonSerialized);
            string response = await postWeightPallet;

            if (!string.IsNullOrEmpty(response))
            {
                dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(response);
                Console.WriteLine($"{responsePayload.Message} [{DateService.Today()}]", Color.Green);           
            }
            else
            {
                Console.WriteLine($"Sin respuesta al registrar palet [{DateService.Today()}]", Color.Red);
            }
        }

        private async void UpdatePalletApex(int pallet, float tare, float weight)
        {
            dynamic jsonPallet = JObject.Parse(Payloads.weightPallet);

            jsonPallet.Tare = tare;
            jsonPallet.Weight = weight;

            string jsonSerialized = JsonConvert.SerializeObject(jsonPallet, Formatting.Indented);

            Task<string> putWeightPallet = APIService.PutApexAsync(String.Format(EndPoints.WeightPallets, cmbWorkOrders.Text, pallet, Constants.Plant2Id), jsonSerialized);
            string response = await putWeightPallet;

            if (!string.IsNullOrEmpty(response))
            {
                dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(response);
               Console.WriteLine($"{responsePayload.Message} [{DateService.Today()}]", Color.Green);           
            }
            else
            {
                Console.WriteLine($"Sin respuesta al actualizar palet [{DateService.Today()}]", Color.Red);
            }
        }

        private async void CreateRollApex(int roll, float weight)
        {
            dynamic jsonRoll = JObject.Parse(Payloads.weightRoll);
            string rollId = DateService.EpochTime();
            jsonRoll.DateMark = rollId;
            jsonRoll.PalletId = lblPalletId.Text;
            jsonRoll.RollNumber = roll;
            jsonRoll.Weight = weight;

            string jsonSerialized = JsonConvert.SerializeObject(jsonRoll, Formatting.Indented);

            Task<string> postWeightRoll = APIService.PostApexAsync(String.Format(EndPoints.WeightRolls, "WO", "Roll", Constants.Plant2Id), jsonSerialized);
            string response = await postWeightRoll;

            if (!string.IsNullOrEmpty(response))
            {
                dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(response);
                Console.WriteLine($"{responsePayload.Message} [{DateService.Today()}]", Color.Green);           
            }
            else
            {
                Console.WriteLine($"Sin respuesta al registrar rollo [{DateService.Today()}]", Color.Red);
            }
        }

        private async void UpdateRollApex(int pallet, float roll, float weight)
        {
            dynamic jsonRoll = JObject.Parse(Payloads.weightRollUpdate);

            jsonRoll.Pallet = pallet;
            jsonRoll.Weight = weight;

            string jsonSerialized = JsonConvert.SerializeObject(jsonRoll, Formatting.Indented);

            Task<string> putWeightRoll = APIService.PutApexAsync(String.Format(EndPoints.WeightRolls, cmbWorkOrders.Text, roll, Constants.Plant2Id), jsonSerialized);
            string response = await putWeightRoll;

            if (!string.IsNullOrEmpty(response))
            {
                dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(response);
                Console.WriteLine($"{responsePayload.Message} [{DateService.Today()}]", Color.Green);           
            }
            else
            {
                Console.WriteLine($"Sin respuesta al actualizar rollo [{DateService.Today()}]", Color.Red);
            }
        }

        #endregion

    }
}
