using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using WOW_Fusion.Controllers;
using WOW_Fusion.Models;
using WOW_Fusion.Properties;
using WOW_Fusion.Services;
using WOW_Fusion.Views.Plant3;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using WOW_Fusion.Views.Plant2;
using System.Reflection;
using System.Net.Sockets;
using System.Data.SqlClient;

namespace WOW_Fusion.Views.Plant3
{
    public partial class frmLabelP3 : Form
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

        //Recurso
        private JObject machines = null;
        private JObject resourcesMfg = null;
        private string resourceId = string.Empty;
        private string workCenterId = string.Empty;
        private string operationNumber = string.Empty;

        //JObjets response
        private dynamic shifts = null;
        private dynamic outputs = null;

        //Pesos params
        private float _tareWeight = 0.0f;
        private float _weightFromWeighing = 0.0f;
        private float _primaryQuantityAndTolerance = 0.0f;

        private int _rowSelected = 0;

        //Sacos pallet control
        private int _sackCount = 0;
        private bool _newSack = false;
        private bool _endWeight = false;
        private bool _completedHistory = false;

        //APEX Flags
        private string _lastApexCreate = string.Empty;
        private bool _apexCreated = false;
        private string _lastApexUpdate = string.Empty;
        private bool _apexUpdated = false;

        //Scheduling
        List<WorkOrderShedule> ordersForSchedule;
        private bool _startOrder = false;
        private bool _startThreadSchedule = false;

        //Hide-Reserve data
        private Int64 _workOrderId = 0;
        private string _workOrderNumber = string.Empty;
        private string _CustomerPONumber = string.Empty;

        private string _outputMain = "PRINCIPAL";
        private string _outputSecondary = "SECUNDARIO";

        //Identificar teclado o escaner
        private DateTime lastKeyPressTime = DateTime.MinValue;
        private const int scannerKeyDelayThreshold = 50;

        private static frmLabelP3 instance;

        /*------------------------------ INITIALIZE ----------------------------------*/
        #region Start
        public frmLabelP3()
        {
            InitializeComponent();
            InitializeFusionData();
        }
        
        private void frmLabelP3_Load(object sender, EventArgs e)
        {
            lblEnvironment.Text = Settings.Default.FusionUrl.Contains("-test") ? "TEST" : "PROD";
            lblStatusProcess.Text = string.Empty;

            richTextConsole.Clear();
            Console.SetOut(new ConsoleController(richTextConsole));

            AppController.ToolTip(btnSettings, "Configuración");
            AppController.ToolTip(pbRed, "Peso debajo del estándar");
            AppController.ToolTip(pbYellow, "Peso encima del estándar");

            AppController.ToolTip(btnEndProcess, "Terminar de pesar orden");

            TipStatusWO.SetToolTip(lblWOStatus, "");

            btnGetWeight.Enabled = false;
        }

        public async void InitializeFusionData()
        {
            timerShift.Stop();
            lblEnvironment.Text = Settings.Default.FusionUrl.Contains("-test") ? "TEST" : "PROD";
            //Obtener datos de Organizacion
            dynamic org = await CommonService.OneItem(String.Format(EndPoints.InventoryOrganizations, Constants.Plant3Id));

            if (org == null)
            {
                NotifierController.Error("Sin organización, la aplicación no funcionará");
                cmbWorkOrders.Enabled = false;
                return;
            }
            else
            {
                //Constants.BusinessUnitId = org.ManagementBusinessUnitId.ToString();
                cmbWorkOrders.Enabled = true;
                lblLocationCode.Text = org.LocationCode.ToString();
                machines = await CommonService.ProductionResourcesMachines(String.Format(EndPoints.ProductionResourcesP3, Constants.Plant3Id)); //Obtener Objeto RECURSOS MAQUINAS

                //♥ Consultar template etiqueta en APEX  ♥
                dynamic labelApex = await LabelService.LabelInfo(Constants.Plant3Id, "HOJUELAPL3", "NULL");
                if (labelApex.LabelName.ToString().Equals("null"))
                {
                    MessageBox.Show("Etiqueta de cliente/producto no encontrada", "¡Alerta!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Console.WriteLine($"Etiqueta {labelApex.LabelName.ToString()} cargada [{DateService.Today()}]", Color.Black);
                }

            }
        }
        #endregion

        #region Scheduling
        private void CheckStatusScheduleOrder(DateTime start, DateTime end)
        {
            DateTimeOffset now = DateTimeOffset.Now;
            if (DateService.IsBetweenDates(start, end))
            {
                lblWOStatus.ForeColor = Color.Green; ;
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
            if (shifts != null && !string.IsNullOrEmpty(resourceId))
            {
                lblShift.Text = (shifts == null) ? string.Empty : DateService.CurrentShift(shifts, resourceId);
            }
        }
        #endregion

        private async void cmbResources_SelectedIndexChanged(object sender, EventArgs e)
        {
            timerShift.Stop();

            lblResourceCode.Text = string.Empty;
            lblWorkCenterName.Text = string.Empty;
            lblShift.Text = string.Empty;

            //int index = cmbResources.SelectedIndex;
            //dynamic resource = machines["items"][index];
            dynamic resource = machines["items"][0];

            resourceId = resource.ResourceId.ToString();
            lblResourceCode.Text = resource.ResourceCode.ToString();

            dynamic wc = await CommonService.OneItem(String.Format(EndPoints.WorkCenterByResourceId, resourceId));

            if (wc != null)
            {
                workCenterId = wc.WorkCenterId.ToString();
                lblWorkCenterName.Text = wc.WorkCenterName.ToString();

                shifts = await CommonService.OneItem(String.Format(EndPoints.ShiftByWorkCenter, workCenterId));

                lblShift.Text = (shifts == null) ? string.Empty : DateService.CurrentShift(shifts, resourceId);
                cmbWorkOrders.Enabled = true;

                timerShift.Tick += new EventHandler(CheckShift);
                timerShift.Start();
            }
            else
            {
                NotifierController.Warning("No se encontro la centro de trabajo");
            }
        }

        /*------------------------------ WORKORDERS ----------------------------------*/
        #region WorkOrders
        private async void DropDownWorkOrders(object sender, EventArgs e)
        {
            cmbWorkOrders.Items.Clear();
            picBoxWaitWO.Visible = true;

            List<string> workOrderNumbers = await CommonService.WOProcessByOrg(Constants.Plant3Id); //Obtener OT

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

            foreach (int order in workOrderNumberSort)
            {
                cmbWorkOrders.Items.Add(order.ToString());
            }
        }

        private void SelectedIndexChangedWorkOrders(object sender, EventArgs e)
        {
            ShowWait(false);
            ClearAll();
            WorkOrderUIFill(cmbWorkOrders.SelectedItem.ToString());
        }

        //Obtener datos de la orden seleccionada
        private async void WorkOrderUIFill(string workOrder)
        {
            ShowWait(true, "Cargando datos ...");
            try
            {
                //♥ Consultar WORKORDER ♥
                Task<string> tskWorkOrdersData = APIService.GetRequestAsync(String.Format(EndPoints.WOProcessDetailP3, workOrder, Constants.Plant3Id));
                string response = await tskWorkOrdersData;

                if (string.IsNullOrEmpty(response))
                {
                    ShowWait(false);
                    return;
                }

                JObject objWorkOrder = JObject.Parse(response);
                if ((int)objWorkOrder["count"] == 0)
                {
                    ShowWait(false);
                    NotifierController.Warning("Datos de orden no encotrada");
                    return;
                }
                dynamic wo = objWorkOrder["items"][0]; //Objeto WORKORDER
                _workOrderId = Int64.Parse(wo.WorkOrderId.ToString());
                _workOrderNumber = workOrder;

                lblPlannedStartDate.Text = wo.PlannedStartDate.ToString();
                lblPlannedCompletionDate.Text = wo.PlannedCompletionDate.ToString();

                /*
                lblPrimaryQuantity.Text = string.IsNullOrEmpty(wo.PrimaryProductQuantity.ToString()) ? 0 : wo.PrimaryProductQuantity.ToString();
                float tolerance = (Settings.Default.PL3Tolerance * float.Parse(lblPrimaryQuantity.Text)) / 100;
                _primaryQuantityAndTolerance = float.Parse(lblPrimaryQuantity.Text) + tolerance;

                lblUoM.Text = wo.UOMCode.ToString();

                if (!string.IsNullOrEmpty(wo.CompletedQuantity.ToString()))
                {
                    NotifierController.Warning($"Orden con despacho registrado [{wo.CompletedQuantity.ToString()} {lblUoM.Text}], verifique antes de pesar");
                }

                lblItemNumber.Text = wo.ItemNumber.ToString();
                lblItemDescription.Text = wo.Description.ToString();

                lblPlannedStartDate.Text = wo.PlannedStartDate.ToString();
                lblPlannedCompletionDate.Text = wo.PlannedCompletionDate.ToString();

                lblStdSack.Text = Settings.Default.SackMaxWeight.ToString();

                int palletTotal = (int)Math.Ceiling(float.Parse(lblPrimaryQuantity.Text) / Settings.Default.SackMaxWeight);
                lblPalletTotal.Text = palletTotal.ToString();*/

                dynamic operation = wo.Operation.items[0];
                operationNumber = operation.OperationSequenceNumber.ToString();

                if ((int)wo.Operation.count > 1)
                {
                    Console.WriteLine($"{(int)wo.Operation.count} operaciones detectadas en orden {cmbWorkOrders.Text}, se tomarán los datos de la primera operación [{DateService.Today()}]", Color.Red);
                }

                //Obtener maquina, centro de trabajo y turno
                timerShift.Stop();
                lblShift.Text = string.Empty;

                workCenterId = operation.WorkCenterId.ToString();
                lblWorkCenterName.Text = operation.WorkCenterName.ToString();
                dynamic resources = wo.ProcessWorkOrderResource;

                for (int i = 0; i < (int)resources.count; i++)
                {
                    if (resources.items[i].OperationSequenceNumber.ToString() == operationNumber)
                    {
                        for (int j = 0; j < (int)machines["count"]; j++)
                        {
                            if (resources.items[i].ResourceId.ToString() == machines["items"][j]["ResourceId"].ToString())
                            {
                                resourceId = machines["items"][j]["ResourceId"].ToString();
                                lblResourceCode.Text = machines["items"][j]["ResourceCode"].ToString();
                                lblResourceName.Text = machines["items"][j]["ResourceName"].ToString();
                                break;
                            }
                        }
                    }
                }

                outputs = wo.ProcessWorkOrderOutput;
                FillDatagridOutputs();

                shifts = await CommonService.OneItem(String.Format(EndPoints.ShiftByWorkCenter, workCenterId));
                lblShift.Text = (shifts == null) ? string.Empty : DateService.CurrentShift(shifts, resourceId);
                timerShift.Tick += new EventHandler(CheckShift);
                timerShift.Start();

                //Verificar Status de la programacion de la orden
                CheckStatusScheduleOrder(DateTime.Parse(wo.PlannedStartDate.ToString()), DateTime.Parse(wo.PlannedCompletionDate.ToString()));

                /*//♥ Consultar template etiqueta en APEX  ♥
                dynamic labelApex = await LabelService.LabelInfo(Constants.Plant3Id, "SUPERSACO", lblItemNumber.Text);
                if (labelApex.LabelName.ToString().Equals("null"))
                {
                    MessageBox.Show("Etiqueta de cliente/producto no encontrada", "¡Alerta!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // + Verificar Historial Pesaje +
                Task<string> tskHistory = APIService.GetApexAsync(String.Format(EndPoints.SacksOrder, Constants.Plant3Id, workOrder));
                string responseHistory = await tskHistory;

                progressBarWO.Value = 0;
                lblAdvance.Text = "0%";
                _sackCount = 0;

                dgSacks.Rows.Clear();
                dgSacks.Refresh();

                lblSackNumber.Text = string.Empty;

                if (!string.IsNullOrEmpty(responseHistory))
                {
                    dynamic sacksOnOrder = JsonConvert.DeserializeObject<dynamic>(responseHistory);
                    lblCompletedQuantity.Text = sacksOnOrder.Completed.ToString();
                    if (sacksOnOrder.Completed > 0)
                    {
                        _completedHistory = true;
                        lblCompletedQuantity.Text = sacksOnOrder.Completed.ToString();
                        CalculateAdvace(float.Parse(lblCompletedQuantity.Text));
                        FillDatagridsFromRecords(sacksOnOrder.Sacks);
                    }
                }
                else
                {
                    Console.WriteLine($"Sin respuesta del historial de pesaje [{DateService.Today()}]", Color.Red);
                }

                //Order completada
                if (dgSacks.Rows.Count > 0)
                {
                    if (float.Parse(lblCompletedQuantity.Text) >= _primaryQuantityAndTolerance)
                    {
                        NotifierController.Warning("Orden completada");
                        btnGetWeight.Enabled = false;

                        if (float.Parse(lblCompletedQuantity.Text) > float.Parse(lblPrimaryQuantity.Text))
                        {
                            Console.WriteLine($"Pesaje [{lblCompletedQuantity.Text} kg] excede la cantidad programada a producir [{lblPrimaryQuantity.Text} kg]", Color.Red);
                        }
                    }
                    else if (float.Parse(lblCompletedQuantity.Text) > _primaryQuantityAndTolerance)
                    {
                        btnGetWeight.Enabled = false;
                        NotifierController.Warning($"Se detectó más pesaje del programado, incluida la tolerancia [{Settings.Default.PL2Tolerance}%]");
                    }
                }*/
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error al seleccionar orden", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void FillDatagridOutputs()
        {
            foreach (dynamic output in outputs.items)
            {
                //Agregar salidas a datagrid
                string[] row = new string[] { output.OutputSequenceNumber.ToString(), output.OperationSequenceNumber.ToString(),
                                              output.ItemNumber.ToString()};
                int indexNewRow = dgOutputs.Rows.Add(row);

                // Cambiar el color de la fuente si OutputSequenceNumber es 10
                if (output.PrimaryFlag == true)
                {
                    dgOutputs.Rows[indexNewRow].DefaultCellStyle.ForeColor = Color.Green;
                    dgOutputs.Rows[indexNewRow].Cells[3].Value = true;
                }

                dgOutputs.FirstDisplayedScrollingRowIndex = indexNewRow;
            }

            dgOutputs.Sort(dgOutputs.Columns["O_OutputNumber"], ListSortDirection.Ascending);
        }

        //Llenar tabla con pesajes
        private void FillDatagridsFromRecords(dynamic sacksOnOrder)
        {
            foreach (dynamic S in sacksOnOrder)
            {
                float grossSack = S.Tare + S.Bag + S.Net;
                string[] sackData = new string[] { S.Sack.ToString(), S.Tare.ToString("F1"), S.Bag.ToString("F1"), grossSack.ToString("F1"), S.Net.ToString("F1")};
                int indexNewSack = dgSacks.Rows.Add(sackData);
                dgSacks.FirstDisplayedScrollingRowIndex = indexNewSack;
            }

            _completedHistory = false;
            _sackCount = dgSacks.Rows.Count;
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
                txtScannerInput.Enabled = false;

                //----------------- PESAR ---------------
                ShowWait(true, "Pesando producto ...");

                string responseWeighing = await RadwagController.SocketWeighing("S");

                if (responseWeighing == "EX")
                {
                    ShowWait(false);
                    NotifierController.Warning("Tiempo de espera agotado, vuelva a  intentar");
                    btnGetWeight.Enabled = true;
                }
                else
                {
                    if (float.TryParse(responseWeighing, out float _weightFromWeighing))
                    {
                        ShowWait(false);
                        if (_weightFromWeighing > 0)
                        {
                            float m_tare = string.IsNullOrEmpty(lblTare.Text) ? 0 : float.Parse(lblTare.Text);
                            float m_bag = string.IsNullOrEmpty(lblBag.Text) ? 0 : float.Parse(lblBag.Text);
                            float sackNet = _weightFromWeighing - (m_tare + m_bag); // Saco - (Tara + Bolsa)

                            lblWeight.Text = sackNet.ToString("F1");
                            if(lblOutputType.Text.Equals(_outputSecondary))
                            {
                                _sackCount += 1;
                            }

                            //Agregar pesos a datagrid
                            string[] row = new string[] { _sackCount.ToString(), m_tare.ToString("F1"), m_bag.ToString("F1"), _weightFromWeighing.ToString("F1"), sackNet.ToString("F1") };
                            int indexNewRoll = dgSacks.Rows.Add(row);
                            dgSacks.FirstDisplayedScrollingRowIndex = indexNewRoll;

                            FillLabelSack(row);
                        }
                        else
                        {
                            MessageBox.Show($"Peso invalido [{_weightFromWeighing.ToString("F1")} kg]", "Báscula", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            btnGetWeight.Enabled = true;
                        }
                    }
                    else
                    {
                        ShowWait(false);
                        Console.WriteLine("Valor invalido obtenido de la báscula", Color.Red);
                        NotifierController.Warning($"{responseWeighing}");
                        btnGetWeight.Enabled = true;
                    }
                }
            }
            else
            {
                MessageBox.Show($"Seleccione orden de trabajo antes de pesar", "¡Alerta!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void ReWeightMenuItem_Click(object sender, EventArgs e)
        {
            ShowWait(true, "Pesando saco nuevamente ...");
            int lastRow = dgSacks.Rows.Count - 1;
            if (!dgSacks.Rows[lastRow].IsNewRow)
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
                            int S_Number = int.Parse(dgSacks.Rows[lastRow].Cells["S_Number"].Value.ToString());
                            float S_Tare = float.Parse(dgSacks.Rows[lastRow].Cells["S_Tare"].Value.ToString());
                            float S_Sack = float.Parse(dgSacks.Rows[lastRow].Cells["S_Sack"].Value.ToString());


                            float S_Net = _weightFromWeighing - (S_Tare + S_Sack);//Calcular peso NETO saco

                            lblWeight.Text = S_Net.ToString("F1");

                            //Actualizar fila de SACO
                            string[] rowSack = new string[] { S_Number.ToString(), S_Tare.ToString("F1"), S_Sack.ToString("F1"), _weightFromWeighing.ToString("F1"), S_Net.ToString("F1") };
                            dgSacks.Rows[lastRow].SetValues(rowSack);

                            FillLabelSack(rowSack);

                            float totalNetSum = dgSacks.Rows.Cast<DataGridViewRow>().Sum(t => float.Parse(t.Cells["S_Net"].Value.ToString()));
                            lblCompletedQuantity.Text = totalNetSum.ToString();

                            CalculateAdvace(totalNetSum);
                            await LabelService.PrintP3(S_Number, "SACK", lblOutputType.Text);
                            UpdateSackApex(S_Number, S_Net);
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

        private void btnTareBag_Click(object sender, EventArgs e)
        {
            frmTare frmTare = new frmTare();
            frmTare.StartPosition = FormStartPosition.CenterParent;
            frmTare.ShowDialog();
        }

        private void btnWeighing_Click(object sender, EventArgs e)
        {
            frmWeighingP3 frmWeighingP3 = new frmWeighingP3();
            frmWeighingP3.StartPosition = FormStartPosition.CenterParent;
            frmWeighingP3.ShowDialog();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmLoginP3 frmLoginP3 = new frmLoginP3();
            frmLoginP3.StartPosition = FormStartPosition.CenterParent;
            frmLoginP3.ShowDialog();
        }

        private void btnEndProcess_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Seguro que desea terminar de pesar la orden?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _endWeight = true;
                //AddPallet();
                cmbWorkOrders.Items.Clear();
                ClearAll();
            }
        }
        #endregion

        /*-------------------------------- UOTPUTS ------------------------------------*/
        #region DataGrid Outputs
        private async void dgOutputs_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Si índice de fila no sea negativo y sea válido
            if (e.RowIndex >= 0 && e.RowIndex < dgOutputs.Rows.Count)
            {
                // Verificar si la columna STATUS fue afectada 
                if (e.ColumnIndex == 3)
                {
                    DataGridViewRow currentRow = dgOutputs.Rows[e.RowIndex];
                    if (currentRow.Cells[3].Value != null && (bool)currentRow.Cells[3].Value == true)
                    {
                        foreach (DataGridViewRow row in dgOutputs.Rows)
                        {
                            if (row.Index != e.RowIndex) // Evitar desmarcar la fila actual
                            {
                                row.Cells[3].Value = false; 
                            }
                        }

                        foreach (dynamic output in outputs.items)
                        {
                            if (output.OutputSequenceNumber.ToString() == currentRow.Cells[0].Value.ToString())
                            {
                                TableLayoutPalletControl("CLEAR");
                                lblItemNumber.Text = output.ItemNumber.ToString();
                                lblItemDescription.Text = output.ItemDescription.ToString();

                                lblPrimaryQuantity.Text = string.IsNullOrEmpty(output.OutputQuantity.ToString()) ? 0 : output.OutputQuantity.ToString();
                                float tolerance = (Settings.Default.PL3Tolerance * float.Parse(lblPrimaryQuantity.Text)) / 100;
                                _primaryQuantityAndTolerance = float.Parse(lblPrimaryQuantity.Text) + tolerance;

                                lblUoM.Text = output.UOMCode.ToString();

                                if (!string.IsNullOrEmpty(output.CompletedQuantity.ToString()))
                                {
                                    NotifierController.Warning($"Orden con despacho registrado [{output.CompletedQuantity.ToString()} {lblUoM.Text}], verifique antes de pesar");
                                }

                                //Verificar CHECK, que producto esta pesando
                                if (output.PrimaryFlag == true) // Si es el producto principal
                                {
                                    lblOutputType.Text = _outputMain;

                                    ShowWait(false);
                                    lblStatusProcess.Text = "¡Escaneé TARAMA o SACO!";
                                    lblStatusProcess.ForeColor = Color.Red;
                                    txtScannerInput.Enabled = true;
                                    txtScannerInput.Focus();
                                }
                                else
                                {
                                    lblOutputType.Text = _outputSecondary;

                                    ShowWait(false);
                                    lblStatusProcess.Text = $"¡Coloque y pese el producto {lblItemNumber.Text}!";
                                    lblStatusProcess.ForeColor = Color.Red;
                                    txtScannerInput.Enabled = false;
                                    btnGetWeight.Enabled = true;
                                    btnGetWeight.BackColor = Color.Red;
                                }

                                lblStdSack.Text = Settings.Default.SackMaxWeight.ToString();

                                int palletTotal = (int)Math.Ceiling(float.Parse(lblPrimaryQuantity.Text) / Settings.Default.SackMaxWeight);
                                lblPalletTotal.Text = palletTotal.ToString();

                                // + Verificar Historial Pesaje +
                                Task<string> tskHistory = APIService.GetApexAsync(String.Format(EndPoints.SacksOrder, Constants.Plant3Id, cmbWorkOrders.Text, lblItemNumber.Text));
                                string responseHistory = await tskHistory;

                                progressBarWO.Value = 0;
                                lblAdvance.Text = "0%";
                                _sackCount = 0;

                                dgSacks.Rows.Clear();
                                dgSacks.Refresh();

                                lblSackNumber.Text = string.Empty;

                                if (!string.IsNullOrEmpty(responseHistory))
                                {
                                    dynamic sacksOnOrder = JsonConvert.DeserializeObject<dynamic>(responseHistory);
                                    lblCompletedQuantity.Text = sacksOnOrder.Completed.ToString();
                                    if (sacksOnOrder.Completed > 0)
                                    {
                                        _completedHistory = true;
                                        lblCompletedQuantity.Text = sacksOnOrder.Completed.ToString();
                                        CalculateAdvace(float.Parse(lblCompletedQuantity.Text));
                                        FillDatagridsFromRecords(sacksOnOrder.Sacks);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Sin respuesta del historial de pesaje [{DateService.Today()}]", Color.Red);
                                }

                                //Order completada
                                if (dgSacks.Rows.Count > 0)
                                {
                                    if (float.Parse(lblCompletedQuantity.Text) >= _primaryQuantityAndTolerance)
                                    {
                                        NotifierController.Warning("Orden completada");
                                        btnGetWeight.Enabled = false;

                                        if (float.Parse(lblCompletedQuantity.Text) > float.Parse(lblPrimaryQuantity.Text))
                                        {
                                            Console.WriteLine($"Pesaje [{lblCompletedQuantity.Text} kg] excede la cantidad programada a producir [{lblPrimaryQuantity.Text} kg]", Color.Red);
                                        }
                                    }
                                    else if (float.Parse(lblCompletedQuantity.Text) > _primaryQuantityAndTolerance)
                                    {
                                        btnGetWeight.Enabled = false;
                                        NotifierController.Warning($"Se detectó más pesaje del programado, incluida la tolerancia [{Settings.Default.PL2Tolerance}%]");
                                    }
                                }

                                break;
                            }
                        }
                    }
                    else if (currentRow.Cells[3].Value != null && (bool)currentRow.Cells[3].Value == false)
                    {
                        //currentRow.Cells[3].Value = true;
                    }
                }
            }
        }

        //Evento CellValueChanged no se dispara hasta perder foco. Usar esteevento para detectar el cambio de inmediato (al hacer clic) 
        private void dgOutputs_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgOutputs.IsCurrentCellDirty)
            {
                dgOutputs.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }


        #endregion
        /*-------------------------------- SACKS ------------------------------------*/
        #region DataGrid Sacks
        private async void dgSacks_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (_completedHistory) { return; }
            _newSack = false;

            TableLayoutPalletControl("WEIGHT");

            float totalNetSum = dgSacks.Rows.Cast<DataGridViewRow>().Sum(t => float.Parse(t.Cells["S_Net"].Value.ToString()));
            lblCompletedQuantity.Text = totalNetSum.ToString();
            CalculateAdvace(totalNetSum);

            await LabelService.PrintP3(_sackCount, "SACK", lblOutputType.Text);
            CreateSackApex();

            if (lblOutputType.Text.Equals(_outputMain))
            {
                //Reiniciar pesar nuevo saco
                lblStatusProcess.Text = "¡Escaneé TARIMA o SACO!";
                lblStatusProcess.ForeColor = Color.Red;
                btnGetWeight.Enabled = false;
                txtScannerInput.Enabled = true;
                txtScannerInput.Focus();
                lblBag.Text = string.Empty;
                lblTare.Text = string.Empty;
            }
            else
            {
                lblStatusProcess.Text = $"¡Coloque y pese el producto {lblItemNumber.Text}!";
                lblStatusProcess.ForeColor = Color.Red;
                btnGetWeight.Enabled = true;
                btnGetWeight.BackColor = Color.Red;
            }

            //Activar boton para terminar orden
            btnEndProcess.Visible = _sackCount > 0 ? true : false;
        }

        //Cambio de color de filas (Max-Min)
        private void dgSacks_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if(lblOutputType.Text.Equals(_outputMain))
            //Cambiar color
            foreach (DataGridViewRow row in dgSacks.Rows)
            {
                float rollNetKg = float.Parse(row.Cells["S_Net"].Value.ToString());
                if (!string.IsNullOrEmpty(lblStdSack.Text))
                {
                    float _stdRollWeight = float.Parse(lblStdSack.Text);
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
        }

        //Click derecho sobre fila
        private void dgSacks_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex == dgSacks.Rows.Count - 1)
            {
                dgSacks.Rows[e.RowIndex].Selected = true;
                _rowSelected = e.RowIndex;
                dgSacks.CurrentCell = dgSacks.Rows[e.RowIndex].Cells["S_Net"];
                MenuShipWeight.Show(dgSacks, e.Location);
                MenuShipWeight.Show(Cursor.Position);
            }
        }

        private void TableLayoutPalletControl(string step)
        {
            tabLayoutPallet.Controls.Clear();
            tabLayoutPallet.RowStyles.Clear();
            tabLayoutPallet.ColumnStyles.Clear();

            if(step == "CLEAR")
            {
                TipTare.SetToolTip(tabLayoutPallet, lblTare.Text);
                tabLayoutPallet.BackgroundImage = Resources.pallet_empty;
                return;
            }

            PictureBox picRoll = new PictureBox();
            if(step == "TARE")
            {
                
                TipTare.SetToolTip(tabLayoutPallet, lblTare.Text);
                tabLayoutPallet.BackgroundImage = (!string.IsNullOrEmpty(lblBag.Text) && string.IsNullOrEmpty(lblTare.Text)) ? Resources.pallet_empty : Resources.pallet_filled;
                picRoll.Image = lblOutputType.Text.Equals(_outputMain) ? Resources.sack_empty : Resources.item;
                AppController.ToolTip(picRoll, lblBag.Text + " kg");
            }
            else if (step == "WEIGHT")
            { 
                float netSack =float.Parse(dgSacks.Rows[dgSacks.Rows.Count - 1].Cells["S_Net"].Value.ToString());

                if (lblOutputType.Text.Equals(_outputMain))
                {
                    if (netSack == float.Parse(lblStdSack.Text))
                    {
                        picRoll.Image = Resources.sack;
                    }
                    else if (netSack > float.Parse(lblStdSack.Text))
                    {
                        picRoll.Image = Resources.sack_yellow;
                    }
                    else if (netSack < float.Parse(lblStdSack.Text))
                    {
                        picRoll.Image = Resources.sack_red;
                    }
                }
                else
                {
                    tabLayoutPallet.BackgroundImage = Resources.pallet_filled;
                    picRoll.Image = Resources.item;
                }

                AppController.ToolTip(picRoll, netSack + " kg");
            }

            picRoll.BackColor = Color.Transparent;
            picRoll.SizeMode = PictureBoxSizeMode.Zoom;
            picRoll.Dock = DockStyle.Fill;

            tabLayoutPallet.ColumnCount = 1;
            tabLayoutPallet.RowCount = 1;
            tabLayoutPallet.Controls.Add(picRoll, 1, 1);

            tabLayoutPallet.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1));
        }
        #endregion

        /*-------------------------------- LABELS ------------------------------------*/
        #region Labels Fill
        private void FillLabelSack(string[] weights)
        {
            if (!string.IsNullOrEmpty(lblResourceCode.Text))
            {
                dynamic label = JObject.Parse(Constants.LabelJson);

                //WO Info
                label.WORKORDER = string.IsNullOrEmpty(_workOrderNumber) ? " " : _workOrderNumber;
                label.ITEMNUMBER = string.IsNullOrEmpty(lblItemNumber.Text) ? " " : lblItemNumber.Text;
                label.ITEMDESCRIPTION = string.IsNullOrEmpty(lblItemDescription.Text) ? " " : lblItemDescription.Text;
                label.EQU = string.IsNullOrEmpty(lblResourceCode.Text) ? " " : lblResourceCode.Text;
                label.SHIFT = string.IsNullOrEmpty(lblShift.Text) ? " " : lblShift.Text;
                label.DATE = DateService.Now();
                //Sack Info
                label.SACK = string.IsNullOrEmpty(weights[0]) ? " " : lblOutputType.Text.Equals(_outputMain) ? "S" + weights[0].PadLeft(4, '0') : weights[0].PadLeft(4, '0');
                label.WNETKG = string.IsNullOrEmpty(weights[4]) ? " " : weights[4];
                label.WGROSSKG = string.IsNullOrEmpty(weights[3]) ? " " : weights[3]; // tara + saco + hojuela 

                Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);
            }
        }
        #endregion

        /*------------------------------- CONTROLS -----------------------------------*/
        #region Controls
        private void ClearAll()
        {
            //cmbResources.Enabled = true;
            cmbWorkOrders.Enabled = true;
            //Weight Section
            lblTare.Text = string.Empty;
            lblBag.Text = string.Empty;
            lblWeight.Text = string.Empty;

            btnGetWeight.Enabled = false;
            btnGetWeight.BackColor = Color.Gray;
            txtScannerInput.Enabled = false;

            //Shift Section
            timerShift.Stop();

            //WorkOrder Section
            lblWOStatus.ForeColor = Color.DarkGray;
            TipStatusWO.SetToolTip(lblWOStatus, "Status orden");

            //cmbWorkOrders.Items.Clear();
            _workOrderId = 0;
            _workOrderNumber = string.Empty;
            lblPrimaryQuantity.Text = string.Empty;
            _primaryQuantityAndTolerance = 0;
            lblCompletedQuantity.Text = string.Empty;
            lblUoM.Text = "--";
            progressBarWO.Value = 0;
            lblAdvance.Text = "0%";
            lblWorkCenterName.Text = string.Empty;
            lblShift.Text = string.Empty;
            lblResourceCode.Text = string.Empty;
            lblResourceName.Text = string.Empty;
            lblPlannedStartDate.Text = string.Empty;
            lblPlannedCompletionDate.Text = string.Empty;
            lblItemNumber.Text = string.Empty;
            lblItemDescription.Text = string.Empty;

            //AKA Section
            //_akaCustomer = "DEFAULT";
            //_CustomerPONumber = string.Empty;
            //_tradingPartnerName = string.Empty;
            //strWithThickness = string.Empty; //Ancho y grosor

            //Pallet Anim Section
            _completedHistory = false;
            TableLayoutPalletControl("CLEAR");

            //Weigth Section
            lblStdSack.Text = string.Empty;
            lblPalletTotal.Text = string.Empty;

            lblTare.Text = string.Empty;
            lblSackNumber.Text = string.Empty;

            //DataGrid Outputs Section
            dgOutputs.Rows.Clear();
            dgOutputs.Refresh();

            //DataGrid Rolls Section
            _sackCount = 0;
            dgSacks.Rows.Clear();
            dgSacks.Refresh();

            _endWeight = false;
            btnEndProcess.Visible = false;

            //Orden inicio a pesar
            _startOrder = false;

            _tareWeight = 0;

            //APEX Flags
            _apexCreated = false;
            _apexUpdated = false;
        }

        private void ShowWait(bool show, string message = "")
        {
            lblStatusProcess.ForeColor = Color.Black;
            lblStatusProcess.Text = message;
            pbWaitProcess.Visible = show;
        }

        private void CalculateAdvace(float completed)
        {
            if (string.IsNullOrEmpty(lblPrimaryQuantity.Text))
            {
                return;
            }
            else
            {
                float goal = float.Parse(lblPrimaryQuantity.Text);
                if (goal == 0 || completed == 0)
                {
                    progressBarWO.Value = 0;
                    lblAdvance.Text = "0%";
                }
                else
                {
                    if (completed >= float.Parse(lblPrimaryQuantity.Text))
                    {
                        progressBarWO.Value = 100;
                        lblAdvance.Text = "100%";
                    }
                    else
                    {
                        int advance = Convert.ToInt32(Math.Round((completed * 100) / float.Parse(lblPrimaryQuantity.Text)));
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
            if (_CustomerPONumber.Contains("TPRM"))
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
                    else if (result == DialogResult.Yes)
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

        /*------------------------------ SCANER TEXT ---------------------------------*/
        #region SCANER TEXT
        private void txtScannerInput_LostFocus(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtScannerInput.Text))
            {
                if (float.TryParse(txtScannerInput.Text, out float scanerInput))
                {
                    if (scanerInput >= Settings.Default.TareMinWeight && scanerInput <= Settings.Default.TareMaxWeight)
                    {
                        NotifierController.Success($"Tarima {txtScannerInput.Text} kg");
                        lblTare.Text = txtScannerInput.Text;
                        txtScannerInput.Text = string.Empty;
                        txtScannerInput.Focus();
                    }
                    else if (scanerInput > 0 && scanerInput <= Settings.Default.BagMaxWeight)
                    {
                        NotifierController.Success($"Saco {txtScannerInput.Text} kg");
                        lblBag.Text = txtScannerInput.Text;
                        txtScannerInput.Text = string.Empty;
                        txtScannerInput.Focus();
                    }
                    else
                    {
                        NotifierController.Warning($"Peso [{txtScannerInput.Text}] no coincide con un saco o tarima estándar, verifique");
                        txtScannerInput.Text = string.Empty;
                        txtScannerInput.Focus();
                    }
                }
                else
                {
                    NotifierController.Warning($"Código {txtScannerInput.Text} invalido");
                    txtScannerInput.Text = string.Empty;
                    txtScannerInput.Focus();
                }
            }
        }

        private void lblTare_TextChanged(object sender, EventArgs e)
        {
            if(lblOutputType.Text.Equals(_outputMain))
            {
                if (!string.IsNullOrEmpty(lblTare.Text) && string.IsNullOrEmpty(lblBag.Text))
                {
                    lblStatusProcess.Text = "¡Escaneé SACO!";
                    lblStatusProcess.ForeColor = Color.Red;
                    TableLayoutPalletControl("CLEAR");
                    tabLayoutPallet.BackgroundImage = Resources.pallet_filled;
                }
                else
                {
                    btnGetWeight.Enabled = false;
                    btnGetWeight.BackColor = Color.Gray;
                }

                TareBagFilled();
            } 
        }

        private void lblBag_TextChanged(object sender, EventArgs e)
        {
            if (lblOutputType.Text.Equals(_outputMain))
            {
                if (!string.IsNullOrEmpty(lblBag.Text) && string.IsNullOrEmpty(lblTare.Text))
                {
                    lblStatusProcess.Text = "¡Escaneé TARIMA!";
                    lblStatusProcess.ForeColor = Color.Red;
                    TableLayoutPalletControl("TARE");
                }
                else
                {
                    btnGetWeight.Enabled = false;
                    btnGetWeight.BackColor = Color.Gray;
                }

                TareBagFilled();
            }
        }

        private void TareBagFilled()
        {
            if (!string.IsNullOrEmpty(lblTare.Text) && !string.IsNullOrEmpty(lblBag.Text))
            {
                btnGetWeight.Enabled = true;
                btnGetWeight.BackColor = Color.Red;

                lblStatusProcess.Text = $"¡Pese el producto {lblItemNumber.Text}!";
                lblStatusProcess.ForeColor = Color.Red;

                if (!_newSack)
                {
                    _newSack = true;
                    _sackCount += 1;

                    //cmbResources.Enabled = false;
                    cmbWorkOrders.Enabled = false;
                }

                lblSackNumber.Text = _sackCount.ToString();

                TableLayoutPalletControl("TARE");
            }
            else
            {
                btnGetWeight.Enabled = false;
                btnGetWeight.BackColor = Color.Gray;
            }
        }
        #endregion

        /*--------------------------------- APEX -------------------------------------*/
        #region APEX
        private async void CreateSackApex()
        {
            if (!_apexCreated)
            {
                dynamic jsonSack = JObject.Parse(Payloads.weightSack);
                jsonSack.DateMark = DateService.EpochTime();
                jsonSack.OrganizationId = Int64.Parse(Constants.Plant3Id);
                jsonSack.WorkOrderId = _workOrderId;
                jsonSack.WorkOrder = _workOrderNumber;
                jsonSack.ItemNumber = lblItemNumber.Text;
                jsonSack.Sack = _sackCount.ToString();
                jsonSack.Tare = string.IsNullOrEmpty(lblTare.Text) ? "0" : lblTare.Text;
                jsonSack.Bag = string.IsNullOrEmpty(lblBag.Text) ? "0" : lblBag.Text;
                jsonSack.Net = string.IsNullOrEmpty(lblWeight.Text) ? "0" : lblWeight.Text;
                jsonSack.Shift = lblShift.Text;

                _lastApexCreate = JsonConvert.SerializeObject(jsonSack, Formatting.Indented);
                _apexCreated = true;
            }

            if (AppController.CheckInternetConnection())
            {
                Task<string> postWeight = APIService.PostApexAsync(EndPoints.WeightSacks, _lastApexCreate);
                string response = await postWeight;

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
                            CreateSackApex();
                        }
                        else
                        {
                            CreateSackApex();
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Sin respuesta al registrar saco [{DateService.Today()}]", Color.Red);
                }
            }
            else
            {
                NotifierController.Warning("Sin conexión a internet");
                DialogResult result = MessageBox.Show("Verificar la conexión internet", "Sin Internet", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                if (result == DialogResult.OK)
                {
                    CreateSackApex();
                }
                else
                {
                    CreateSackApex();
                }
            }
        }

        private async void UpdateSackApex(int sack, float net)
        {
            if (!_apexUpdated)
            {
                dynamic jsonSack = JObject.Parse(Payloads.weightSackUpdate);

                jsonSack.OrganizationId = Int64.Parse(Constants.Plant3Id); ;
                jsonSack.WorkOrder = _workOrderNumber;
                jsonSack.ItemNumber = lblItemNumber.Text;
                jsonSack.Sack = sack;
                jsonSack.Net = net;

                _lastApexUpdate = JsonConvert.SerializeObject(jsonSack, Formatting.Indented);
                _apexUpdated = true;
            }

            if (AppController.CheckInternetConnection())
            {
                Task<string> putWeight = APIService.PutApexAsync(String.Format(EndPoints.WeightSacks, _workOrderNumber, sack, Constants.Plant3Id), _lastApexUpdate);
                string response = await putWeight;

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
                            UpdateSackApex(sack, net);
                        }
                        else
                        {
                            UpdateSackApex(sack, net);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Sin respuesta al actualizar saco [{DateService.Today()}]", Color.Red);
                }
            }
            else
            {
                NotifierController.Warning("Sin conexión a internet");
                DialogResult result = MessageBox.Show("Verificar la conexión internet", "Sin Internet", MessageBoxButtons.OK, MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    UpdateSackApex(sack, net);
                }
                else
                {
                    UpdateSackApex(sack, net);
                }
            }
        }
        #endregion

        private void frmLabelP3_Activated(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(cmbWorkOrders.Text))
            {
                this.BeginInvoke(new Action(() =>
                {
                    txtScannerInput.Focus();
                }));
            }
        }

        private void txtScannerInput_KeyDown(object sender, KeyEventArgs e)
        {
            //txtScannerInput.Clear();
            //txtScannerInput.Focus();
            NotifierController.Warning("Teclado no permitido en este campo");
            /*DateTime currentKeyPressTime = DateTime.Now;

            // Si la tecla es presionada demasiado rápido, probablemente sea un escáner, no bloquear
            if ((currentKeyPressTime - lastKeyPressTime).TotalMilliseconds < scannerKeyDelayThreshold)
            {
                e.SuppressKeyPress = false;
                txtScannerInput.Clear();
                txtScannerInput.Focus();
                lastKeyPressTime = currentKeyPressTime;
            }
            else
            {
                e.SuppressKeyPress = true; //Suprimir caracter insertado
                txtScannerInput.Clear();
                txtScannerInput.Focus();
                lastKeyPressTime = currentKeyPressTime; // Actualizar la última vez que se presionó una tecla
                NotifierController.Warning("Teclado no permitido en este campo");
            }*/
        }

        private void txtScannerInput_KeyUp(object sender, KeyEventArgs e)
        {
            //txtScannerInput.Clear();
            //txtScannerInput.Focus();
            //NotifierController.Warning("Teclado no permitido en este campo");
            /*txtScannerInput.Clear();
            txtScannerInput.Focus();*/
        }

        private void txtScannerInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            //e.Handled = true;
        }
    }
}

