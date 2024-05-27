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

namespace WOW_Fusion.Views.Plant3
{
    public partial class frmLabelP3 : Form
    {
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
        private int _sackCount = 0;
        private int _sackByPallet = 1;
        private bool _isPalletStart = false;
        private bool _endWeight = false;

        //JObjets response
        private dynamic shifts = null;

        //Scheduling
        List<WorkOrderShedule> ordersForSchedule;
        private bool _startOrder = false;

        #region Start
        public frmLabelP3()
        {
            InitializeComponent();
            InitializeFusionData();
        }

        private void frmLabelP3_Load(object sender, EventArgs e)
        {
            pop = new PopController();

            richTextConsole.Clear();
            Console.SetOut(new ConsoleController(richTextConsole));

            AppController.ToolTip(btnSettings, "Configuración");
            AppController.ToolTip(pbRed, "Peso debajo del estándar");
            AppController.ToolTip(pbYellow, "Peso encima del estándar");

            TipStatusWO.SetToolTip(lblWOStatus, "Status orden"); ;

            btnGetWeight.Text = "TARAR";
            btnGetWeight.Enabled = false;
        }

        public async void InitializeFusionData()
        {
            timerShift.Stop();

            List<string> endPoints = new List<string>
            {
                String.Format(BatchPoints.Organizations, Constants.Plant3Id),
                String.Format(BatchPoints.ResourceById, Settings.Default.ResourceId3),
                String.Format(BatchPoints.WorkCenterByResourceId, Settings.Default.ResourceId3),
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
                    lblShift.Text = (shifts == null) ? string.Empty : DateService.CurrentShift(shifts, Settings.Default.ResourceId3);
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
            ordersForSchedule = await CommonService.WOProcessSchedule(Constants.Plant3Id, Settings.Default.ResourceId3); //Obtener OT Schedule

            if (ordersForSchedule.Count > 0)
            {
                List<WorkOrderShedule> schedule = CommonService.OrderByPriority(ordersForSchedule, "PlannedStartDate");

                List<string> ordersPrinted = FileController.ContentFile(Constants.OrdersPrintedP2);

                for (int i = 0; i < schedule.Count; i++)
                {
                    if (FileController.IsOrderPrinted(ordersPrinted, schedule[i].WorkOrderNumber))
                    {
                        //Quitar ordenes ya impresas
                        schedule.RemoveAt(i);
                        i--;
                    }
                }

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
                        cmbWorkOrders.Items.Clear();
                        cmbWorkOrders.Items.Add(wo.WorkOrderNumber.ToString());
                        cmbWorkOrders.Text = wo.WorkOrderNumber.ToString();
                        break;
                    }
                }
            }
            else
            {
                NotifierController.Warning("Sin ordenes de trabajo");
            }

            lblWOStatus.Visible = true;

            if (lblMode.Text.Equals("Auto."))
            {
                timerSchedule.Tick += new EventHandler(ProductionScheduling);
                timerSchedule.Start();
            }
            else
            {
                timerSchedule.Stop();
            }
        }

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
            if (shifts != null && !string.IsNullOrEmpty(Settings.Default.ResourceId3))
            {
                lblShift.Text = (shifts == null) ? string.Empty : DateService.CurrentShift(shifts, Settings.Default.ResourceId3);
            }
        }
        #endregion

        #region WorkOrders
        private async void DropDownWorkOrders(object sender, EventArgs e)
        {
            cmbWorkOrders.Items.Clear();
            picBoxWaitWO.Visible = true;

            List<string> workOrderNumbers = await CommonService.WOProcessByResource(Constants.Plant3Id, Settings.Default.ResourceId3); //Obtener OT
            picBoxWaitWO.Visible = false;

            if (workOrderNumbers == null) return;

            List<string> ordersPrinted = FileController.ContentFile(Constants.OrdersPrintedP2);

            foreach (string order in workOrderNumbers)
            {
                //Ordenes sin imprimir
                if (!FileController.IsOrderPrinted(ordersPrinted, order))
                {
                    cmbWorkOrders.Items.Add(order);
                }
            }

            /*foreach (var item in workOrderNumbers)
            {
                cmbWorkOrders.Items.Add(item.ToString());
            }*/
        }

        private void SelectedIndexChangedWorkOrders(object sender, EventArgs e)
        {
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
                Task<string> tskWorkOrdersData = APIService.GetRequestAsync(String.Format(EndPoints.WOProcessDataP3, workOrder, Constants.Plant3Id));
                string response = await tskWorkOrdersData;

                if (string.IsNullOrEmpty(response))
                {
                    pop.Close();
                    cmbWorkOrders.Enabled = true;
                    return;
                }

                JObject objWorkOrder = JObject.Parse(response);
                if ((int)objWorkOrder["count"] == 0)
                {
                    pop.Close();
                    NotifierController.Warning("Datos de orden no encotrada");
                    cmbWorkOrders.Enabled = true;
                    return;
                }
                dynamic wo = objWorkOrder["items"][0]; //Objeto WORKORDER
                lblPrimaryProductQuantity.Text = wo.PrimaryProductQuantity.ToString();
                lblCompletedQuantity.Text = wo.CompletedQuantity.ToString();
                if (!string.IsNullOrEmpty(lblCompletedQuantity.Text))
                {
                    pop.Close();
                    NotifierController.Warning("Orden con cantidad completa registrada, verifique antes de pesar");
                }
                lblUoM.Text = wo.UOMCode.ToString();

                lblItemNumber.Text = wo.ItemNumber.ToString();
                lblItemDescription.Text = wo.Description.ToString();

                lblPlannedStartDate.Text = wo.PlannedStartDate.ToString();
                lblPlannedCompletionDate.Text = wo.PlannedCompletionDate.ToString();

                //Verificar Status de la programacion de la orden
                CheckStatusScheduleOrder(DateTime.Parse(wo.PlannedStartDate.ToString()), DateTime.Parse(wo.PlannedCompletionDate.ToString()));

                //♥ Consultar template etiqueta en APEX  ♥
                dynamic labelApex = await LabelService.LabelInfo(Constants.Plant3Id, _akaCustomer, lblItemNumber.Text);
                if (labelApex.LabelName.ToString().Equals("null"))
                {
                    pop.Close();
                    MessageBox.Show("Etiqueta no encontrada", "Verificar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    gbLabelSack.Text = $"Etiqueta [{labelApex.LabelName.ToString()}]";
                }

                //Validar activacion de boton de pesaje
                if (!string.IsNullOrEmpty(cmbWorkOrders.Text) && !string.IsNullOrEmpty(lblResourceName.Text) && !string.IsNullOrEmpty(gbLabelSack.Text))
                {
                    btnGetWeight.Enabled = true;
                }
                else
                {
                    btnGetWeight.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                pop.Close();
                cmbWorkOrders.Enabled = true;
                MessageBox.Show("Error. " + ex.Message, "Error al seleccionar orden", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            pop.Close();
            cmbWorkOrders.Enabled = true;
        }

        #endregion

        #region Buttons Actions
        private async void btnGetWeight_Click(object sender, EventArgs e)
        {
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
                        //TARAR
                        _tareWeight = float.Parse(requestTareWeight);
                        if (_tareWeight > 0)
                        {
                            timerSchedule.Stop();
                            lblTare.Text = float.Parse(requestTareWeight).ToString("F2");
                            txtBoxWeight.Text = float.Parse(requestTareWeight).ToString("F2");
                            btnGetWeight.Text = "PESAR";
                            btnGetWeight.BackColor = Color.LimeGreen;
                            btnReloadTare.Visible = true;
                            _sackCount += 1;
                            _previousWeight = 0;


                            tabLayoutPallet.BackgroundImage = Resources.pallet_filled;
                            TableLayoutPalletControl(1, 0);

                            lblSackNumber.Text =_sackCount.ToString();
                            cmbWorkOrders.Enabled = false; // Deshabilitar combo de Ordenes

                            //Registrar pallet en DB APEX
                            lblPalletId.Text = DateService.EpochTime();
                            ApexService.CreatePallet(Constants.Plant3Id, lblPalletId.Text, _sackCount, cmbWorkOrders.Text, lblItemNumber.Text,
                                                     float.Parse(lblTare.Text), 0.0f, lblShift.Text, _sackByPallet);
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
                    string res = responseTare.Equals("EX") ? "Error de comunicación a basculas" : responseTare;
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
                    _weightFromWeighing = float.Parse(responseWeighing);

                    if (_weightFromWeighing > 0)
                    {
                        if (_weightFromWeighing == _previousWeight)
                        {
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

                            //Calcular peso neto de cada rollo (SIN TARA)
                            float sackNetKg = _weightFromWeighing - _previousWeight;
                            txtBoxWeight.Text = sackNetKg.ToString("F2");

                            //Calcular pero bruto de cada rollo (con tara)
                            float sackGrossKg = sackNetKg + _tareWeight;
                            btnReloadTare.Visible = false;

                            //Agregar pesos a datagrid
                            string[] row = new string[] {_sackCount.ToString(), _tareWeight.ToString("F2"), sackNetKg.ToString("F2"), sackGrossKg.ToString("F2")};


                            dgSacks.Rows.Add(row);

                            //Reserver peso neto acomulado para sacar peso de rollo
                            _previousWeight = _weightFromWeighing;

                            FillLabelSack(row);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Peso invalido [{_weightFromWeighing.ToString("F2")} kg]", "Báscula", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
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
            frmLoginP3 frmLogin = new frmLoginP3();
            frmLogin.StartPosition = FormStartPosition.CenterParent;
            frmLogin.ShowDialog();
        }

        private void btnEndProcess_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Seguro que desea terminar de pesar la orden?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _endWeight = true;
                //AddPallet();
            }
        }
        #endregion

        #region DataGrid Supersacos
        private void TableLayoutPalletControl(int sackOnPallet, int sackNumber)
        {
            if (sackOnPallet <= 12)
            {
                int count = 0;

                tabLayoutPallet.Controls.Clear();
                tabLayoutPallet.RowStyles.Clear();
                tabLayoutPallet.ColumnStyles.Clear();

                tabLayoutPallet.ColumnCount = sackOnPallet > 4 ? 4 : sackOnPallet;

                if (sackOnPallet >= 9)
                {
                    tabLayoutPallet.RowCount = 3;
                }
                else if (sackOnPallet >= 5 && sackOnPallet <= 8)
                {
                    tabLayoutPallet.RowCount = 2;
                }
                else
                {
                    tabLayoutPallet.RowCount = 1;
                }

                int cells = tabLayoutPallet.RowCount * tabLayoutPallet.ColumnCount;

                for (int row = 0; row < tabLayoutPallet.RowCount; row++)
                {
                    tabLayoutPallet.RowStyles.Add(new RowStyle(SizeType.Percent, 1));
                    for (int col = 0; col < tabLayoutPallet.ColumnCount; col++)
                    {
                        PictureBox picSack = new PictureBox();
                        if (count < sackNumber)
                        {
                            IEnumerable<string> columnWeigthsNetKg = dgSacks.Rows.Cast<DataGridViewRow>()
                                                                    .Where(fila => fila.Cells["S_Sack"].Value.ToString().Equals(lblSackNumber.Text))
                                                                    .Select(fila => fila.Cells["S_Net"].Value.ToString());

                            string[] weigthRoll = columnWeigthsNetKg.ToArray();

                            if (float.Parse(weigthRoll[count]) == Settings.Default.WeightStdP3)
                            {
                                picSack.Image = Resources.sack;
                            }
                            else if (float.Parse(weigthRoll[count]) > Settings.Default.WeightStdP3)
                            {
                                picSack.Image = Resources.sack_yellow;
                            }
                            else if (float.Parse(weigthRoll[count]) < Settings.Default.WeightStdP3)
                            {
                                picSack.Image = Resources.sack_red;
                            }
                            else
                            {
                                continue;
                            }

                            AppController.ToolTip(picSack, weigthRoll[count].ToString() + " kg");
                        }
                        else
                        {
                            picSack.Image = Resources.sack_empty;
                        }

                        picSack.BackColor = Color.Transparent;
                        picSack.SizeMode = PictureBoxSizeMode.Zoom;
                        picSack.Dock = DockStyle.Fill;



                        count++;

                        if (count <= sackOnPallet)
                        {
                            tabLayoutPallet.Controls.Add(picSack, col, row);
                        }
                        tabLayoutPallet.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1));
                    }
                }
            }
        }

        bool IsTheSameCellValue(int column, int row)
        {
            DataGridViewCell cell1 = dgSacks[column, row];
            DataGridViewCell cell2 = dgSacks[column, row - 1];
            if (cell1.Value == null || cell2.Value == null)
            {
                return false;
            }
            return cell1.Value.ToString() == cell2.Value.ToString();
        }

        //Cambio de color de filas (Max-Min)
        private void dgWeights_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //Cambiar color
            foreach (DataGridViewRow row in dgSacks.Rows)
            {
                float rollNetKg = float.Parse(row.Cells["S_Net"].Value.ToString());
                float _stdRollWeight = float.Parse("50.00"); ///cambiar++++++++++++++++++
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
            _isPalletStart = true;
            if (lblMode.Text.Equals("Auto.")) { timerSchedule.Stop(); }
            TableLayoutPalletControl(1, _sackByPallet);

            //Llenar campos de pallet (SUMA)
            float palletNetSum = dgSacks.Rows.Cast<DataGridViewRow>().Sum(t => float.Parse(t.Cells["S_Net"].Value.ToString()));
            lblCompletedQuantity.Text = palletNetSum.ToString();
            CalculateAdvace(palletNetSum);

            await LabelService.PrintP2(_sackCount, "SACK"); //Imprimir etiqueta

            ApexService.CreateWeightItem(_sackCount, float.Parse(dgSacks.Rows[e.RowIndex].Cells["S_Net"].Value.ToString()), lblPalletId.Text, Constants.Plant3Id);
            ApexService.UpdatePallet(_sackCount, float.Parse(dgSacks.Rows[e.RowIndex].Cells["S_Tare"].Value.ToString()), 
                                     float.Parse(dgSacks.Rows[e.RowIndex].Cells["S_Net"].Value.ToString()), cmbWorkOrders.Text);

            //Activar boton para terminar orden
            btnEndProcess.Visible = _sackCount > 0 ? true : false;

            //Limpiar para NUEVO pesaje
            _isPalletStart = false;
            tabLayoutPallet.BackgroundImage = Resources.pallet_empty;
            TableLayoutPalletControl(0, _sackByPallet);
            btnGetWeight.Text = "TARAR";
            btnGetWeight.BackColor = Color.DarkOrange;

            lblPalletNetKg.Text = string.Empty;
            lblPalletGrossKg.Text = string.Empty;
            lblTare.Text = string.Empty;
        }

        //Eliminar ultima fila de la lista de pesos
        private void dgWeights_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
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

        private void deleteMenuItem_Click(object sender, EventArgs e)
        {
            if (!dgSacks.Rows[_rowSelected].IsNewRow)
            {
                //Restar peso eliminado en PESO PREVIO peara evitar inconsistencias
                _previousWeight -= float.Parse(dgSacks.CurrentRow.Cells["S_Net"].Value.ToString());
                dgSacks.Rows.RemoveAt(_rowSelected);
                lblPalletNetKg.Text = _previousWeight.ToString("F2");
                //Restar 1 a la cantidad de rollos
                _sackCount -= 1;
            }
        }

        private async void ReWeightMenuItem_Click(object sender, EventArgs e)
        {
            if (!dgSacks.Rows[_rowSelected].IsNewRow)
            {
                txtBoxWeight.Text = string.Empty;
                pop.Show(this);

                //Restar peso a recalcular en PESO PREVIO peara evitar inconsistencias
                _previousWeight -= float.Parse(dgSacks.CurrentRow.Cells["S_Net"].Value.ToString());

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

                            //Calcular pero neto de cada rollo (SIN TARA)
                            float rollNetKg = _weightFromWeighing - _previousWeight;
                            txtBoxWeight.Text = rollNetKg.ToString("F2");

                            float rollNetLbs = rollNetKg * _lbs;
                            btnReloadTare.Visible = false;

                            //Calcular pero bruto de cada rollo (con tara)
                            float rollGrossKg = rollNetKg + _tareWeight;
                            float rollGrossLbs = rollGrossKg * _lbs;


                            //Agregar pesos a datagrid
                            string[] rowRoll = new string[] { _sackCount.ToString(), _tareWeight.ToString("F2"), rollNetKg.ToString("F2"),rollGrossKg.ToString("F2")};

                            //Actualizar rollo
                            dgSacks.Rows[_rowSelected].SetValues(rowRoll);

                            float palletNetSum = dgSacks.Rows.Cast<DataGridViewRow>().Sum(t => float.Parse(t.Cells["S_Net"].Value.ToString()));
                            lblCompletedQuantity.Text = palletNetSum.ToString();
                            CalculateAdvace(palletNetSum);

                            TableLayoutPalletControl(1, _sackByPallet);
                            FillLabelSack(rowRoll);

                            await LabelService.PrintP2(_sackCount, "SACK"); //Imprimir etiqueta
                            ApexService.UpdateWeightItem(_sackCount, _sackCount, rollNetKg, cmbWorkOrders.Text, Constants.Plant2Id);

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

        #region Labels Fill
        private async void FillLabelSack(string[] weights)
        {
            if (!string.IsNullOrEmpty(lblResourceCode.Text))
            {
                dynamic label = JObject.Parse(Constants.LabelJson);

                label.WORKORDER = string.IsNullOrEmpty(cmbWorkOrders.Text) ? " " : cmbWorkOrders.Text/*.Substring(7)*/;
                label.ITEMNUMBER = string.IsNullOrEmpty(lblItemNumber.Text) ? " " : lblItemNumber.Text;
                label.ITEMDESCRIPTION = string.IsNullOrEmpty(lblItemDescription.Text) ? " " : lblItemDescription.Text;
                label.EQU = string.IsNullOrEmpty(lblResourceCode.Text) ? " " : lblResourceCode.Text;
                label.DATE = DateService.Now();
                label.SHIFT = string.IsNullOrEmpty(lblShift.Text) ? " " : lblShift.Text;
                label.SACK = string.IsNullOrEmpty(weights[1]) ? " " :  weights[1].PadLeft(4, '0');
                label.WNETKG = string.IsNullOrEmpty(weights[2]) ? " " : weights[2];
                label.WGROSSKG = string.IsNullOrEmpty(weights[3]) ? " " : weights[3];

                Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);
                picLabelSack.Image = Image.FromStream(await LabelService.UpdateLabelLabelary(_sackCount, "SACK"));
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

            //Pallet Anim Section
            _isPalletStart = false;
            tabLayoutPallet.BackgroundImage = Resources.pallet_empty;
            TableLayoutPalletControl(0, 0);

            //Weigth Section
            lblPalletId.Text = "*************";

            lblPalletNetKg.Text = string.Empty;
            lblPalletGrossKg.Text = string.Empty;
            lblTare.Text = string.Empty;
            lblSackNumber.Text = string.Empty;

            //DataGrid Rolls Section
            _sackCount = 0;
            dgSacks.Rows.Clear();
            dgSacks.Refresh();
            picLabelSack.Image = null;

            _endWeight = false;
            btnEndProcess.Visible = false;

            //Orden inicio a pesar
            _startOrder = false;

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
                }
            }
        }
        #endregion

        private void flowLayoutMain_SizeChanged(object sender, EventArgs e)
        {
            /*if (flowLayoutMain.Size.Height > 700)
            {
                int boxes = groupBoxProd.Size.Height + groupBoxAka.Size.Height + groupBoxWeight.Size.Height;
                int total = (flowLayoutMain.Size.Height - boxes) - 20;
                int divide = total / 3;
                groupBoxProd.Height = groupBoxProd.Size.Height + divide;
                groupBoxAka.Height = groupBoxAka.Size.Height + divide;
                groupBoxWeight.Height = groupBoxWeight.Size.Height + divide;
            }

            flowLayoutMain.HorizontalScroll.Maximum = 0;
            flowLayoutMain.HorizontalScroll.Enabled = false;*/
        }
    }
}
