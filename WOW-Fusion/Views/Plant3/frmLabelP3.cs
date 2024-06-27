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
        private int _sackCount = 0;
        private bool _endWeight = false;
        private bool _completedHistory = false;

        private bool _reloadTare = false;

        //APEX Flags
        private string _lastApexCreate = string.Empty;
        private bool _apexCreated = false;
        private string _lastApexUpdate = string.Empty;
        private bool _apexUpdated = false;
        private string _labelName = string.Empty;

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

            TipStatusWO.SetToolTip(lblWOStatus, "Status orden");

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
                return;
            }
            else
            {
                //Constants.BusinessUnitId = org.ManagementBusinessUnitId.ToString();
                lblLocationCode.Text = org.LocationCode.ToString();
                machines = await CommonService.ProductionResourcesMachines(String.Format(EndPoints.ProductionResourcesP3, Constants.Plant3Id)); //Obtener Objeto RECURSOS MAQUINAS
                cmbResources.Enabled = true;
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

        /*------------------------------ WORKORDERS ----------------------------------*/
        #region WorkOrders
        private async void DropDownWorkOrders(object sender, EventArgs e)
        {
            cmbWorkOrders.Items.Clear();
            picBoxWaitWO.Visible = true;

            List<string> workOrderNumbers = await CommonService.WOProcessByResource(Constants.Plant3Id, resourceId); //Obtener OT

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
            cmbWorkOrders.Enabled = false;
            ShowWait(true, "Cargando datos ...");
            try
            {
                //♥ Consultar WORKORDER ♥
                Task<string> tskWorkOrdersData = APIService.GetRequestAsync(String.Format(EndPoints.WOProcessData, workOrder, Constants.Plant3Id));
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
                lblPrimaryProductQuantity.Text = string.IsNullOrEmpty(wo.PrimaryProductQuantity.ToString()) ? 0 : wo.PrimaryProductQuantity.ToString();
                lblUoM.Text = wo.UOMCode.ToString();
                if (!string.IsNullOrEmpty(wo.CompletedQuantity.ToString()))
                {
                    NotifierController.Warning($"Orden con despacho registrado [{wo.CompletedQuantity.ToString()} {lblUoM.Text}], verifique antes de pesar");
                }

                lblItemNumber.Text = wo.ItemNumber.ToString();
                lblItemDescription.Text = wo.Description.ToString();

                lblPlannedStartDate.Text = wo.PlannedStartDate.ToString();
                lblPlannedCompletionDate.Text = wo.PlannedCompletionDate.ToString();

                lblStdSack.Text = Settings.Default.WeightStdP3.ToString();

                int palletTotal = (int)Math.Ceiling(float.Parse(lblPrimaryProductQuantity.Text) / Settings.Default.WeightStdP3);
                lblPalletTotal.Text = palletTotal.ToString();

                //Verificar Status de la programacion de la orden
                CheckStatusScheduleOrder(DateTime.Parse(wo.PlannedStartDate.ToString()), DateTime.Parse(wo.PlannedCompletionDate.ToString()));

                //♥ Consultar template etiqueta en APEX  ♥
                dynamic labelApex = await LabelService.LabelInfo(Constants.Plant3Id, _akaCustomer, lblItemNumber.Text);
                if (labelApex.LabelName.ToString().Equals("null"))
                {
                    _akaCustomer = "DEFAULT";
                    DialogResult result = MessageBox.Show("Etiqueta de cliente/producto no encontrada, se cargará la etiqueta estándar", "¡Alerta!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (result == DialogResult.OK)
                    {
                        labelApex = await LabelService.LabelInfo(Constants.Plant3Id, _akaCustomer, lblItemNumber.Text);
                    }
                    else
                    {
                        labelApex = await LabelService.LabelInfo(Constants.Plant3Id, _akaCustomer, lblItemNumber.Text);
                    }
                    _labelName = labelApex.LabelName.ToString();
                }
                else
                {
                    _labelName = labelApex.LabelName.ToString();
                }

                //Verificar Historial Pesaje----------------------------
                /*Task<string> tskHistory = APIService.GetApexAsync(String.Format(EndPoints.RollsOrder, Constants.Plant3Id, workOrder));
                string responseHistory = await tskHistory;

                progressBarWO.Value = 0;
                lblAdvance.Text = "0%";

                _palletCount = 0;
                _sackCount = 0;

                dgSacks.Rows.Clear();
                dgSacks.Refresh();

                lblSackNumber.Text = string.Empty;

                if (!string.IsNullOrEmpty(responseHistory))
                {
                    dynamic rollsOnOrder = JsonConvert.DeserializeObject<dynamic>(responseHistory);
                    lblCompletedQuantity.Text = rollsOnOrder.Completed.ToString();
                    if (rollsOnOrder.Completed > 0)
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
                }*/

                //Order completada
                if (dgSacks.Rows.Count > 0)
                {
                    if (int.Parse(lblPalletTotal.Text)  == dgSacks.Rows.Count)
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
                MessageBox.Show("Error. " + ex.Message, "Error al seleccionar orden", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ShowWait(false);
            lblStatusProcess.Text = "¡Escaneé TARA!";
            lblStatusProcess.ForeColor = Color.Red;
        }

        //Llenar tabla con pesajes
        private void FillDatagridsFromRecords(dynamic rollsOnOrder)
        {
            foreach (dynamic R in rollsOnOrder)
            {
                float grossRoll = R.Core + R.Net;
                string[] palletData = new string[] { R.Pallet.ToString(), R.Roll.ToString(), R.Core.ToString("F1"), R.Net.ToString("F1"), grossRoll.ToString("F1") };
                int indexNewRoll = dgSacks.Rows.Add(palletData);
                dgSacks.FirstDisplayedScrollingRowIndex = indexNewRoll;
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

                //----------------- PESAR ---------------
                ShowWait(true, "Pesando rollo ...");

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
                            float sackNet = _weightFromWeighing - float.Parse(lblBag.Text); // Saco - Bolsa
                            float sackGross = sackNet + float.Parse(lblBag.Text); //Saco + Bolsa

                            lblWeight.Text = sackNet.ToString("F1");

                            _sackCount++;

                            //Agregar pesos a datagrid
                            string[] row = new string[] { _sackCount.ToString(), lblTare.Text, lblBag.Text, sackNet.ToString("F1"), sackGross.ToString("F1") };
                            int indexNewRoll = dgSacks.Rows.Add(row);
                            dgSacks.FirstDisplayedScrollingRowIndex = indexNewRoll;
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

                btnGetWeight.Enabled = true;
                ShowWait(false);
            }
            else
            {
                MessageBox.Show($"Seleccione orden de trabajo antes de pesar", "¡Alerta!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void ReWeightMenuItem_Click(object sender, EventArgs e)
        {
            ShowWait(true, "Pesando rollo nuevamente ...");
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
                            float dgNet = float.Parse(dgSacks.Rows[lastRow].Cells["R_Net"].Value.ToString());
                            if (_weightFromWeighing < (_previousWeight - dgNet))
                            {
                                MessageBox.Show("Se detecto menor peso al obtenido anteriormente, verifique el producto colocado", "¡Precaucion!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                _previousWeight -= dgNet;
                                int dgPallet = int.Parse(dgSacks.Rows[lastRow].Cells["R_Pallet"].Value.ToString());
                                int dgRoll = int.Parse(dgSacks.Rows[lastRow].Cells["R_Roll"].Value.ToString());
                                float dgCore = float.Parse(dgSacks.Rows[lastRow].Cells["R_Core"].Value.ToString());


                                float rollNet = _weightFromWeighing - _previousWeight;//Calcular peso rollo
                                float rollGross = dgCore + rollNet;

                                _previousWeight = _weightFromWeighing; //Reserver peso
                                _netPallet -= dgNet; //Descontar del neto acomulado
                                _netPallet += rollNet; //Aumentar con nuevo neto

                                lblWeight.Text = rollNet.ToString("F1");

                                //Actualizar fila de ROLLO
                                string[] rowRoll = new string[] { dgPallet.ToString(), dgRoll.ToString(), dgCore.ToString("F1"), rollNet.ToString("F1"), rollGross.ToString("F1") };
                                dgSacks.Rows[lastRow].SetValues(rowRoll);

                                float palletNetSum = dgSacks.Rows.Cast<DataGridViewRow>().Sum(t => float.Parse(t.Cells["R_Net"].Value.ToString()));
                                lblCompletedQuantity.Text = palletNetSum.ToString();
                                CalculateAdvace(palletNetSum);

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

        private void btnSettings_Click(object sender, EventArgs e)
        {
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

        /*-------------------------------- SACKS ------------------------------------*/
        #region DataGrid Sacks
        private async void dgSacks_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (_completedHistory) { return; }

            TableLayoutPalletControl(1, 1, lblSackNumber.Text);

            _sackCount += 1;

            //Llenar campos de pallet (SUMA)
            float palletNetSum = dgSacks.Rows.Cast<DataGridViewRow>().Sum(t => float.Parse(t.Cells["S_Net"].Value.ToString()));
            lblCompletedQuantity.Text = palletNetSum.ToString();
            CalculateAdvace(palletNetSum);

            //await LabelService.PrintP2(_sackCount, "ROLL");
            //CreateRollApex();

            lblBag.Text = string.Empty; //Limpiar core hasta registrar
            lblTare.Text = string.Empty;

            //Activar boton para terminar orden
            btnEndProcess.Visible = _sackCount > 0 ? true : false;
        }

        //Cambio de color de filas (Max-Min)
        private void dgSacks_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //Cambiar color
            foreach (DataGridViewRow row in dgSacks.Rows)
            {
                float rollNetKg = float.Parse(row.Cells["R_Net"].Value.ToString());
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
                dgSacks.CurrentCell = dgSacks.Rows[e.RowIndex].Cells["R_Net"];
                MenuShipWeight.Show(dgSacks, e.Location);
                MenuShipWeight.Show(Cursor.Position);
            }
        }

        private void TableLayoutPalletControl(int rollOnPallet, int rollNumber, string palletNumber)
        {
            if (rollOnPallet <= 25)
            {
                int count = 0;

                tabLayoutPallet.Controls.Clear();
                tabLayoutPallet.RowStyles.Clear();
                tabLayoutPallet.ColumnStyles.Clear();

                //Definir FILAS Y COLUMNAS
                switch (rollOnPallet)
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
                    case 11:
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
                            IEnumerable<string> columnWeigthsNetKg = dgSacks.Rows.Cast<DataGridViewRow>().Where(fila => fila.Cells["R_Pallet"].Value.ToString().Equals(palletNumber))
                                                                    .Select(fila => fila.Cells["R_Net"].Value.ToString());

                            string[] weigthRoll = columnWeigthsNetKg.ToArray();

                            if (float.Parse(weigthRoll[count]) == float.Parse(lblStdSack.Text))
                            {
                                picRoll.Image = Resources.sack_green;
                            }
                            else if (float.Parse(weigthRoll[count]) > float.Parse(lblStdSack.Text))
                            {
                                picRoll.Image = Resources.sack_yellow;
                            }
                            else if (float.Parse(weigthRoll[count]) < float.Parse(lblStdSack.Text))
                            {
                                picRoll.Image = Resources.sack_red;
                            }

                            AppController.ToolTip(picRoll, weigthRoll[count].ToString() + " kg");
                        }
                        else
                        {
                            picRoll.Image = Resources.sack;
                        }

                        picRoll.BackColor = Color.Transparent;
                        picRoll.SizeMode = PictureBoxSizeMode.Zoom;
                        picRoll.Dock = DockStyle.Fill;

                        count++;

                        if (count <= rollOnPallet)
                        {
                            tabLayoutPallet.Controls.Add(picRoll, col, row);

                            if (rollOnPallet == 3 && count == 3)
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
                label.ITEMDESCRIPTION = string.IsNullOrEmpty(lblItemDescription.Text) ? " " : lblItemDescription.Text;label.EQU = string.IsNullOrEmpty(lblResourceCode.Text) ? " " : lblResourceCode.Text;
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
                label.LEGALENTITY = string.IsNullOrEmpty(_tradingPartnerName) ? "NE" : _tradingPartnerName;
                label.PURCHASEORDER = string.IsNullOrEmpty(_CustomerPONumber) ? " " : _CustomerPONumber;
                label.PONUM = string.IsNullOrEmpty(_CustomerPONumber) ? " " : _CustomerPONumber.Contains("TPRM") ? _CustomerPONumber.Replace("TPRM", "") : _CustomerPONumber;

                Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);
            }
        }
        #endregion

        /*------------------------------- CONTROLS -----------------------------------*/
        #region Controls
        private void ClearAll()
        {
            //Weight Section
            lblTare.Text = string.Empty;
            lblBag.Text = string.Empty;
            lblWeight.Text = string.Empty;

            btnGetWeight.Enabled = false;
            btnGetWeight.Text = "TARAR";
            btnGetWeight.BackColor = Color.Red;
            btnGetWeight.ForeColor = Color.White;

            //Shift Section
            timerShift.Stop();

            //WorkOrder Section
            lblWOStatus.ForeColor = Color.DarkGray;
            TipStatusWO.SetToolTip(lblWOStatus, "Status orden");

            //cmbWorkOrders.Items.Clear();
            _workOrderId = 0;
            _workOrderNumber = string.Empty;
            lblPrimaryProductQuantity.Text = string.Empty;
            lblCompletedQuantity.Text = string.Empty;
            lblUoM.Text = "--";
            progressBarWO.Value = 0;
            lblAdvance.Text = "0%";
            lblPlannedStartDate.Text = string.Empty;
            lblPlannedCompletionDate.Text = string.Empty;
            lblItemNumber.Text = string.Empty;
            lblItemDescription.Text = string.Empty;

            //AKA Section
            _akaCustomer = "DEFAULT";
            _CustomerPONumber = string.Empty;
            _tradingPartnerName = string.Empty;
            strWithThickness = string.Empty; //Ancho y grosor

            //Pallet Anim Section
            _completedHistory = false;
            tabLayoutPallet.BackgroundImage = Resources.pallet_empty;
            TableLayoutPalletControl(0, 0, lblSackNumber.Text);

            //Weigth Section
            lblStdSack.Text = string.Empty;
            lblPalletTotal.Text = string.Empty;

            lblTare.Text = string.Empty;
            lblSackNumber.Text = string.Empty;

            //DataGrid Rolls Section
            _sackCount = 0;
            dgSacks.Rows.Clear();
            dgSacks.Refresh();
            _labelName = string.Empty;

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
        #endregion

        /*--------------------------------- APEX -------------------------------------*/
        #region APEX
        private async void CreateRollApex()
        {
            if (!_apexCreated)
            {
                dynamic jsonRoll = JObject.Parse(Payloads.weightRolls);
                jsonRoll.DateMark = DateService.EpochTime();
                jsonRoll.OrganizationId = Int64.Parse(Constants.Plant3Id);
                jsonRoll.WorkOrderId = _workOrderId;
                jsonRoll.WorkOrder = _workOrderNumber;
                jsonRoll.ItemNumber = lblItemNumber.Text;
                jsonRoll.Pallet = lblSackNumber.Text;
                jsonRoll.Roll = _sackCount.ToString();
                jsonRoll.Tare = lblTare.Text;
                jsonRoll.Core = lblBag.Text;
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

                jsonRoll.OrganizationId = Int64.Parse(Constants.Plant3Id); ;
                jsonRoll.WorkOrder = _workOrderNumber;
                jsonRoll.Roll = roll;
                jsonRoll.Net = net;

                _lastApexUpdate = JsonConvert.SerializeObject(jsonRoll, Formatting.Indented);
                _apexUpdated = true;
            }

            if (AppController.CheckInternetConnection())
            {
                Task<string> putWeightRoll = APIService.PutApexAsync(String.Format(EndPoints.WeightRolls, _workOrderNumber, roll, Constants.Plant3Id), _lastApexUpdate);
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

        private void cmbResources_DropDown(object sender, EventArgs e)
        {
            cmbResources.Items.Clear();
            pbWaitResources.Visible = true;

            dynamic items = machines["items"];
            pbWaitResources.Visible = false;
            foreach (var item in items)
            {
                cmbResources.Items.Add(item.ResourceName.ToString());
            }
        }

        private async void cmbResources_SelectedIndexChanged(object sender, EventArgs e)
        {
            timerShift.Stop();

            lblResourceCode.Text = string.Empty;
            lblWorkCenterName.Text = string.Empty;
            lblShift.Text = string.Empty;

            int index = cmbResources.SelectedIndex;

            dynamic resource = machines["items"][index];

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

        private void lblTare_TextChanged(object sender, EventArgs e)
        {
            tabLayoutPallet.BackgroundImage = Resources.pallet_filled;
            TableLayoutPalletControl(1, 0, 1.ToString());

            if (!string.IsNullOrEmpty(lblTare.Text) && !string.IsNullOrEmpty(lblBag.Text))
            {
                btnGetWeight.Enabled = true;
            }
            else
            {
                btnGetWeight.Enabled = false;
            }
        }

        private void lblBag_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lblTare.Text) && !string.IsNullOrEmpty(lblBag.Text))
            {
                btnGetWeight.Enabled = true;
            }
            else
            {
                btnGetWeight.Enabled = false;
            }
        }

        private void txtScannerInput_LostFocus(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtScannerInput.Text))
            {
                if(txtScannerInput.Text.Contains("T"))
                {
                    lblTare.Text = txtScannerInput.Text.Replace("T", "");
                    txtScannerInput.Text =string.Empty; 
                    txtScannerInput.Focus();
                }
                else if(txtScannerInput.Text.Contains("S"))
                {
                    lblBag.Text = txtScannerInput.Text.Replace("S", "");
                    txtScannerInput.Text = string.Empty;
                    txtScannerInput.Focus();
                }
                else
                {
                    NotifierController.Warning($"Código {txtScannerInput.Text} invalido");
                    txtScannerInput.Text = string.Empty;
                    txtScannerInput.Focus();
                }
            }
        }
    }
}

