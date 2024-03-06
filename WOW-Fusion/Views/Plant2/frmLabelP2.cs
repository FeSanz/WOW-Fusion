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

namespace WOW_Fusion
{
    public partial class frmLabelP2 : Form
    {
        Random rnd = new Random();
        //Timer timerShift = new Timer();

        PopController pop;

        //Pesos params
        private float _tareWeight = 0.0f;
        private float _weightFromWeighing = 0.0f;
        private float _previousWeight = 0.0f;
        private float _lbs = 2.205f;

        private int _rowSelected = 0;

        //Rollos pallet control
        private int _rollCount = 0;
        private int _rollByPallet = 0;
        private int _palletCount = 0;
        private bool _isPalletStart = false;

        //JObjets response
        private JObject machines = null;
        private dynamic shifts = null;

        private string _resourceId = string.Empty;

        //Scheduling
        List<WorkOrderShedule> schedule;

        #region Start
        public frmLabelP2()
        {
            InitializeComponent();
            InitializeFusionData();
        }

        private async void frmLabelP2_Load(object sender, EventArgs e)
        {
            pop = new PopController();

            ConsoleController console = new ConsoleController(txtBoxConsole);
            Console.SetOut(console);

            AppController.ToolTip(btnSettings, "Configuración");
            AppController.ToolTip(pbRed, "Peso debajo del estándar");
            AppController.ToolTip(pbYellow, "Peso encima del estándar");

            btnGetWeight.Text = "TARA";

            btnGetWeight.Enabled = true;

            /************************************************************/

            //WorkOrder Section
            cmbWorkOrders.Items.Clear();
            cmbWorkOrders.Items.Add("WO-PL2-1062");
            cmbWorkOrders.SelectedIndex = 0;
            lblPrimaryProductQuantity.Text = "600";
            lblUoM.Text = "kg";
            lblPlannedStartDate.Text = "2024-03-05T16:56:36.590Z";
            lblPlannedCompletionDate.Text = "2024-03-05T16:57:12.409Z";
            lblResourceCode.Text = "MF-LAM 01";
            lblResourceName.Text = "MF-LAMINADORA L0";
            lblEquipmentInstanceCode.Text = "LAM 01";
            lblEquipmentInstanceName.Text = "LINEA DE LAMINACION L01";
            lblItemNumber.Text = "PCR23-0620WSKSLT";
            lblItemDescription.Text = "PET CRIST 23milsx24.4in WS TSK R0430C6EL";
            lblItemDescriptionEnglish.Text = "PET CRIST 23milsx24.4in WS TSK R0430C6EL";

            //Lote
            checkBoxLotControl.Checked = true;

            //Organizacion
            lblWorkCenterName.Text = "LAMINADORA L01";
            lblWorkAreaName.Text = "LAMINADORA 1";
            lblShift.Text = "FD";
            lblOrganizationCode.Text = "OI_PL2";
            lblLocationCode.Text = "PLANTA 2";

            //Pesaje
            lblStdRoll.Text = "50";
            lblWeightUOMRoll.Text = "kg";
            lblWeightUOMPallet.Text = "kg";
            lblStdPallet.Text = "150";
            lblContainerType.Text = "Pallet";
            int rollsOnPallet = int.Parse(lblStdPallet.Text) / int.Parse(lblStdRoll.Text);
            lblRollOnPallet.Text = rollsOnPallet.ToString();

            int palletTotal = (int)Math.Ceiling(float.Parse(lblPrimaryProductQuantity.Text) / (float.Parse(lblStdRoll.Text) * float.Parse(lblRollOnPallet.Text)));
            lblPalletTotal.Text = palletTotal.ToString();

            lblLabelDesign.Text = "XILAM";
            dynamic aka = await LabelService.LabelInfo(lblLabelDesign.Text); // XILAM  lblLabelDesign.Text
            lblAkaItem.Text = (aka.AkaItemNumber.ToString() == "null") ? string.Empty : aka.AkaItemNumber.ToString();
            lblAkaDescription.Text = (aka.AkaItemDescription.ToString() == "null") ? string.Empty : aka.AkaItemDescription.ToString();
            lblLegalEntitie.Text = (aka.AkaLegalEntity.ToString() == "null") ? string.Empty : aka.AkaLegalEntity.ToString();

            /**********************************************************/
        }

        private async void InitializeFusionData()
        {
            /*
            //Obtener datos de Organizacion
            dynamic org = await CommonService.OneItem(String.Format(EndPoints.InventoryOrganizations, Constants.Plant2Id));
            //Obtener datos de centro de trabajo
            dynamic wc = await CommonService.OneItem(String.Format(EndPoints.WorkCentersById, Constants.Plant2Id, Settings.Default.WorkCenterP2));

            if (org == null || wc == null)
            {
                NotifierController.Error("Sin organización o centro de trabajo, la aplicación no funcionará");
                return;
            }
            else
            {
                lblOrganizationCode.Text = org.OrganizationCode.ToString();
                lblLocationCode.Text = org.LocationCode.ToString();
                lblWorkCenterName.Text = wc.WorkCenterName.ToString();
                lblWorkAreaName.Text = wc.WorkAreaName.ToString();
            }

            machines = await CommonService.ProductionResourcesMachines(String.Format(EndPoints.ProductionResourcesP2, Constants.Plant2Id)); //Obtener Objeto RECURSOS MAQUINAS 

            ProductionScheduling(this, EventArgs.Empty);*/
        }

        private async void ProductionScheduling(object sender, EventArgs e)
        {
            picBoxWaitWO.Visible = true;
            schedule = await CommonService.WOProcessSchedule(Constants.Plant2Id, Settings.Default.WorkCenterP2); //Obtener OT Schedule

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

            if(string.IsNullOrEmpty(cmbWorkOrders.Text) && lblMode.Text.Equals("Auto."))
            {
                timerSchedule.Tick += new EventHandler(ProductionScheduling);
                timerSchedule.Start();
            }
            else
            {
                picBoxWaitWO.Visible = false;
                timerSchedule.Stop();   
            }
        }

        private void CheckShift(object sender, EventArgs e)
        {
            if(shifts != null && !string.IsNullOrEmpty(_resourceId))
            {
                lblShift.Text = (shifts == null) ? string.Empty : DateService.CurrentShift(shifts, _resourceId);
            }
        }
        #endregion

        #region WorkOrders
        private async void DropDownWorkOrders(object sender, EventArgs e)
        {
            cmbWorkOrders.Items.Clear();
            picBoxWaitWO.Visible = true;

            List<string> workOrderNumbers = await CommonService.WOProcessByWorkCenter(Constants.Plant2Id, Settings.Default.WorkCenterP2); //Obtener OT
            picBoxWaitWO.Visible = false;

            if (workOrderNumbers == null) return;

            foreach (var item in workOrderNumbers)
            {
                cmbWorkOrders.Items.Add(item.ToString());
            }
        }

        private void SelectedIndexChangedWorkOrders(object sender, EventArgs e)
        {
            //WorkOrderUIFill(cmbWorkOrders.SelectedItem.ToString());
        }

        private async void WorkOrderUIFill(string workOrder)
        {
            timerShift.Stop();
            try
            {
                Task<string> tskWorkOrdersData = APIService.GetRequestAsync(String.Format(EndPoints.WOProcessDetail, workOrder));
                string response = await tskWorkOrdersData;
                if (string.IsNullOrEmpty(response)) { return; }

                JObject objWorkOrder = JObject.Parse(response);
                if ((int)objWorkOrder["count"] == 0)
                {
                    NotifierController.Warning("Datos de orden no encotrada");
                    return;
                }
                dynamic wo = objWorkOrder["items"][0]; //Objeto WORKORDER

                dynamic itemsV2 = await CommonService.OneItem(String.Format(EndPoints.Item, wo.ItemNumber.ToString(), Constants.Plant2Id));
                if(itemsV2 != null)
                {
                    lblStdRoll.Text = itemsV2.UnitWeightQuantity.ToString();
                    lblWeightUOMRoll.Text = itemsV2.WeightUOMValue.ToString();
                    lblWeightUOMPallet.Text = lblWeightUOMRoll.Text;
                    lblStdPallet.Text = itemsV2.MaximumLoadWeight.ToString();
                    lblContainerType.Text = itemsV2.ContainerTypeValue.ToString();
                    int rollsOnPallet = int.Parse(itemsV2.MaximumLoadWeight.ToString()) / int.Parse(itemsV2.UnitWeightQuantity.ToString());
                    lblRollOnPallet.Text = rollsOnPallet.ToString();
                    //float stdPalletWeight = float.Parse(itemsV2.UnitWeightQuantity.ToString()) * float.Parse(lblRollOnPallet.Text);
                    //lblStdPallet.Text = stdPalletWeight.ToString();
                    checkBoxLotControl.Checked = itemsV2.LotControlValue.ToString() == "Full lot control" ? true : false;
                }
                else
                {
                    NotifierController.Warning("Datos de item no encontrados");
                }

                //lblPlannedQuantity.Text = wo.PlannedStartQuantity.ToString();
                lblPrimaryProductQuantity.Text = wo.PrimaryProductQuantity.ToString();
                lblCompletedQuantity.Text = wo.CompletedQuantity.ToString();
                lblUoM.Text = wo.UOMCode.ToString();

                int palletTotal = (int)Math.Ceiling(float.Parse(lblPrimaryProductQuantity.Text) / (float.Parse(lblStdRoll.Text) * float.Parse(lblRollOnPallet.Text)));
                lblPalletTotal.Text = palletTotal.ToString();

                lblItemNumber.Text = wo.ItemNumber.ToString();
                lblItemDescription.Text = wo.Description.ToString();
                lblItemDescriptionEnglish.Text = TranslateService.Translate(lblItemDescription.Text.ToString());
                lblPlannedStartDate.Text = wo.PlannedStartDate.ToString();
                lblPlannedCompletionDate.Text = wo.PlannedCompletionDate.ToString();

                //int countResources = (int)wo.WorkOrderResource.count;
                int countResources = (int)wo.ProcessWorkOrderResource.count;
                if (countResources >= 1)
                {
                    int indexMachine = -1;

                    for (int i = 0; i < countResources; i++)
                    {
                        for (int j = 0; j < (int)machines["count"]; j++)
                        {
                            //string resourceOrder = wo.WorkOrderResource.items[i].ResourceId.ToString();
                            string resourceOrder = wo.ProcessWorkOrderResource.items[i].ResourceId.ToString();
                            string resourceMachines = machines["items"][j]["ResourceId"].ToString();

                            if (resourceOrder.Equals(resourceMachines))
                            {
                                indexMachine = i;
                            }
                        }
                    }
                    if (indexMachine >= 0)
                    {
                        //dynamic resource = wo.WorkOrderResource.items[indexMachine]; //Objeto RESURSO
                        dynamic resource = wo.ProcessWorkOrderResource.items[indexMachine]; //Objeto RESURSO
                        _resourceId = resource.ResourceId.ToString();
                        lblResourceCode.Text = resource.ResourceCode.ToString();
                        lblResourceName.Text = resource.ResourceName.ToString();

                        //Consulta de turno
                        shifts = await CommonService.OneItem(String.Format(EndPoints.ShiftByWorkCenter, Settings.Default.WorkCenterP2));
                        lblShift.Text = (shifts == null) ? string.Empty : DateService.CurrentShift(shifts, _resourceId);

                        //Consultar datos AKA
                        dynamic flexPO = wo.ProcessWorkOrderDFF.items[0];
                        lblAkaPO.Text = flexPO.pedidoDeVenta.ToString();

                        lblLabelDesign.Text = string.IsNullOrEmpty(lblAkaPO.Text) ? "XILAM" : "-------------"; //XILAM LAMTIN03

                        dynamic aka = await LabelService.LabelInfo(lblLabelDesign.Text); // XILAM  lblLabelDesign.Text
                        lblAkaItem.Text = (aka.AkaItemNumber.ToString() == "null") ? string.Empty : aka.AkaItemNumber.ToString();
                        lblAkaDescription.Text = (aka.AkaItemDescription.ToString() == "null") ? string.Empty : aka.AkaItemDescription.ToString();
                        lblLegalEntitie.Text = (aka.AkaLegalEntity.ToString() == "null") ? string.Empty : aka.AkaLegalEntity.ToString();

                        btnGetWeight.Enabled = true;
                        timerShift.Tick += new EventHandler(CheckShift);
                        timerShift.Start();


                        //if ((int)resource.WorkOrderOperationResourceInstance.count >= 1)
                        if ((int)resource.ResourceInstance.count >= 1)
                        {
                            //dynamic instance = resource.WorkOrderOperationResourceInstance.items[0]; // Objeto INSTANCIA
                            dynamic instance = resource.ResourceInstance.items[0]; // Objeto INSTANCIA
                            lblEquipmentInstanceCode.Text = instance.EquipmentInstanceCode.ToString();
                            lblEquipmentInstanceName.Text = instance.EquipmentInstanceName.ToString();
                        }
                        else
                        {
                            NotifierController.Warning("Datos de instancias no encontrados");
                        }
                    }
                    else
                    {
                        NotifierController.Warning("Datos de recurso no encontrados");
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
                            lblTare.Text = float.Parse(requestTareWeight).ToString("F2");
                            txtBoxWeight.Text = float.Parse(requestTareWeight).ToString("F2");
                            btnGetWeight.Text = "OBTENER";
                            btnReloadTare.Visible = true;
                            //_rollCount = 0;
                            _rollByPallet = 0;
                            _palletCount += 1;
                            _previousWeight = 0;

                            tabLayoutPallet.BackgroundImage = Resources.pallet_filled;

                            lblPalletNumber.Text = _palletCount.ToString();
                            cmbWorkOrders.Enabled = false; // Deshabilitar combo de Ordenes

                            //Registrar pallet en DB APEX
                            lblPalletId.Text = DateService.EpochTime();
                            //+++++++++++++++++++++++++++++++CreatePalletApex(_palletCount, 0.0f);
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
                    //MessageBox.Show(responseTare, "Báscula", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        if (_weightFromWeighing < _previousWeight)
                        {
                            pop.Close();
                            MessageBox.Show("Se detecto menor peso al obtenido anteriormente, " +
                                            "verifique el producto colocado debido a que esto puede provocar inconsistencias, " +
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

                            dgRolls.Rows.Add(row);

                            //Reserver peso neto acomulado para sacar peso de rollo
                            _previousWeight = _weightFromWeighing;

                            FillLabelRoll(row);
                        }
                    }
                    else
                    {
                        NotifierController.Warning($"Peso invalido [{_weightFromWeighing.ToString("F2")} kg]");
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
                    ClearAll();
                    if (lblMode.Text.Equals("Auto."))
                    {
                        lblMode.Text = "Manual";
                        cmbWorkOrders.Items.Clear();
                        cmbWorkOrders.Enabled = true;
                        picBoxWaitWO.Visible = false;
                    }
                    else
                    {
                        lblMode.Text = "Auto.";
                        cmbWorkOrders.Items.Clear();
                        cmbWorkOrders.Enabled = false;
                        ProductionScheduling(this, EventArgs.Empty);
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }
            else
            {
                ClearAll();
                if (lblMode.Text.Equals("Auto."))
                {
                    lblMode.Text = "Manual";
                    cmbWorkOrders.Items.Clear();
                    cmbWorkOrders.Enabled = true;
                    picBoxWaitWO.Visible = false;
                }
                else
                {
                    lblMode.Text = "Auto.";
                    cmbWorkOrders.Items.Clear();
                    cmbWorkOrders.Enabled = false;
                    ProductionScheduling(this, EventArgs.Empty);
                }
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmSettingsP2 frmSettingsP2 = new frmSettingsP2();
            frmSettingsP2.StartPosition = FormStartPosition.CenterParent;
            frmSettingsP2.FormClosed += FrmSettingsP2Closed;
            frmSettingsP2.ShowDialog();
        }

        private void btnEndProcess_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region DataGrid Rollos
        private void TableLayoutPalletControl(int rollOnPallet, int rollNumber)
        {
            lblRollCount.Text = $"+ {rollOnPallet}";

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
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
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
            }
        }

        //Click derecho sobre fila
        private void dgWeights_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
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

            //Combinar celdas (Quitar valor repetido)
            if (e.RowIndex == 0)
                return;
            if (IsTheSameCellValue(e.ColumnIndex, e.RowIndex))
            {
                e.Value = "";
                e.FormattingApplied = true;
            }
        }

        private async void dgRolls_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            _rollByPallet++;
            _isPalletStart = true;
            TableLayoutPalletControl(int.Parse(lblRollOnPallet.Text), _rollByPallet);

            //Llenar campos de pallet (SUMA)
            float palletNetSum = dgRolls.Rows.Cast<DataGridViewRow>().Sum(t => float.Parse(t.Cells["R_NetKg"].Value.ToString()));
            lblCompletedQuantity.Text = palletNetSum.ToString();

            await LabelService.PrintP2(_rollCount, "ROLL"); //Imprimir rollo

            if (_rollByPallet == int.Parse(lblRollOnPallet.Text))
            {
                AddPallet();
            }
            
            //+++++++++++++++++++++++++++CreateRollApex(_rollCount, float.Parse(dgRolls.Rows[e.RowIndex].Cells["R_NetKg"].Value.ToString()));
        }

        //Eliminar ultima fila de la lista de pesos
        private void dgWeights_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex == dgRolls.Rows.Count - 1)
            {
                dgRolls.Rows[e.RowIndex].Selected = true;
                _rowSelected = e.RowIndex;
                dgRolls.CurrentCell = dgRolls.Rows[e.RowIndex].Cells["R_NetKg"];
                MenuShipDeleteWeight.Show(dgRolls, e.Location);
                MenuShipDeleteWeight.Show(Cursor.Position);
            }
        }

        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
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
        #endregion

        #region DataGrid Pallets
        private void AddPallet()
        {
            if (_isPalletStart)
            {
                string[] rowPallet = new string[] { _palletCount.ToString(), _tareWeight.ToString(), lblPalletNetKg.Text,
                                                    lblPalletGrossKg.Text, lblPalletNetLbs.Text, lblPalletGrossLbs.Text };
                dgPallets.Rows.Add(rowPallet);

                //Limpiar datos para pallet nuevo
                _rollByPallet = 0;
                _isPalletStart = false;
                tabLayoutPallet.BackgroundImage = Resources.pallet_empty;
                TableLayoutPalletControl(0, _rollByPallet);
                btnGetWeight.Text = "TARA";

                lblPalletNetKg.Text = string.Empty;
                lblPalletGrossKg.Text = string.Empty;
                lblPalletNetLbs.Text = string.Empty;
                lblPalletGrossLbs.Text = string.Empty;
                lblTare.Text = string.Empty;
            }
            else
            {
                NotifierController.Warning("Aún no cuenta con datos de pesaje del nuevo palet");
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
            int palletAdded = int.Parse(dgPallets.Rows[e.RowIndex].Cells["P_Pallet"].Value.ToString());
            string rollWeights = WeightsPallet(palletAdded);

            string[] palletWeight = new string[6];
            for (int i = 0; i < 6; i++)
            {
                palletWeight[i] = dgPallets.Rows[e.RowIndex].Cells[i].Value.ToString();
            }

            FillLabelPallet(palletWeight, rollWeights);
            await LabelService.PrintP2(palletAdded, "PALLET");

            //TERMINA PROCESO DE PESAJE PARA LA ORDEN SELECCIONADA
            if (palletAdded == int.Parse(lblPalletTotal.Text))
            {
                ClearAll();
            }
        }

        //Click derecho sobre fila
        private void dgWeightsPallets_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
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
        private void FillLabelRoll(string[] weights)
        {
            if (!string.IsNullOrEmpty(lblResourceCode.Text))
            {
                dynamic label = JObject.Parse(Constants.LabelJson);

                label.WORKORDER = cmbWorkOrders.Text.Substring(7);
                label.ITEMNUMBER = lblItemNumber.Text;
                label.ITEMDESCRIPTION = lblItemDescription.Text;
                label.ENGLISHDESCRIPTION = lblItemDescriptionEnglish.Text;
                label.EQU = lblResourceCode.Text;
                label.DATE = DateService.Now();
                label.SHIFT = lblShift.Text;
                label.ROLL = "R" + weights[1].PadLeft(4, '0');
                label.LOTNUMBER = "";
                label.WNETKG = weights[2];
                label.WGROSSKG = weights[3];
                label.WNETLBS = weights[4];
                label.WGROSSLBS = weights[5];
                label.WIDTHTHICKNESS = "";

                //AKA Info
                label.AKAITEM = lblAkaItem.Text;
                label.AKADESCRIPTION = lblAkaDescription.Text;
                label.LEGALENTITY = lblLegalEntitie.Text;
                label.PURCHASEORDER = lblAkaPO.Text;
                label.ADDRESS = "";
                label.EMAIL = "";

                Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);
                picLabelRoll.Image = Image.FromStream(LabelService.UpdateLabelLabelary(_rollCount, "ROLL"));
            }
        }

        private void FillLabelPallet(string[] palletWeight, string rollWeights)
        {
            if (!string.IsNullOrEmpty(lblResourceCode.Text))
            {
                dynamic label = JObject.Parse(Constants.LabelJson);

                label.PALET = "P" + _palletCount.ToString().PadLeft(4, '0');
                label.WNETKG = palletWeight[2];
                label.WGROSSKG = palletWeight[3];
                label.WNETLBS = palletWeight[4];
                label.WGROSSLBS = palletWeight[5];
                label.WEIGHTS = rollWeights;

                Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);
                picLabelPallet.Image = Image.FromStream(LabelService.UpdateLabelLabelary(_palletCount, "PALLET"));
            }
        }
        #endregion

        #region Controls
        private void FrmSettingsP2Closed(object sender, FormClosedEventArgs e)
        {
            Refresh();
            InitializeFusionData();
        }

        private void txtBoxConsole_TextChanged(object sender, EventArgs e)
        {
            txtBoxConsole.SelectionStart = txtBoxConsole.Text.Length;
            txtBoxConsole.ScrollToCaret();
        }

        private void ClearAll()
        {
            //Weight Section
            txtBoxWeight.Text = string.Empty;
            btnGetWeight.Enabled = false;
            btnGetWeight.Text = "TARA";
            timerShift.Stop();
            lblShift.Text = string.Empty;

            //WorkOrder Section
            cmbWorkOrders.Items.Clear();
            cmbWorkOrders.Enabled = true;
            lblPrimaryProductQuantity.Text = string.Empty;
            lblCompletedQuantity.Text = string.Empty;
            lblUoM.Text = "--";
            lblPlannedStartDate.Text = string.Empty;
            lblPlannedCompletionDate.Text = string.Empty;
            lblResourceCode.Text = string.Empty;
            lblResourceName.Text = string.Empty;
            lblEquipmentInstanceCode.Text = string.Empty;
            lblEquipmentInstanceName.Text = string.Empty;
            lblItemNumber.Text = string.Empty;
            lblItemDescription.Text = string.Empty;
            lblItemDescriptionEnglish.Text = string.Empty;

            //Lote
            checkBoxLotControl.Checked = false;

            //AKA Section
            lblAkaPO.Text = string.Empty;
            lblLegalEntitie.Text = string.Empty;
            lblAkaItem.Text = string.Empty;
            lblAkaDescription.Text = string.Empty;

            //Pallet Anim Section
            _rollByPallet = 0;
            _isPalletStart = false;
            tabLayoutPallet.BackgroundImage = Resources.pallet_empty;
            TableLayoutPalletControl(0, 0);

            //Pesaje area
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
            lblLabelDesign.Text = string.Empty;
            picLabelRoll.Image = null;

            //DataGrid Pallets Section
            _palletCount = 0;
            dgPallets.Rows.Clear();
            dgPallets.Refresh();
            picLabelPallet.Image = null;
        }
        #endregion

        #region APEX
        private async void CreatePalletApex(int pallet, float weight)
        {
            dynamic jsonPallet = JObject.Parse(Payloads.weightPallet);
            
            jsonPallet.DateMark = lblPalletId.Text;
            jsonPallet.OrganizationId = Constants.Plant2Id;
            jsonPallet.WorkOrderNumber = cmbWorkOrders.Text;
            jsonPallet.ItemNumber = lblItemNumber.Text;
            jsonPallet.PalletNumber = pallet;
            jsonPallet.PalletRolls = int.Parse(lblRollOnPallet.Text);
            jsonPallet.Tare = float.Parse(lblTare.Text);
            jsonPallet.Weight = weight;
            jsonPallet.Shift = lblShift.Text;

            string jsonSerialized = JsonConvert.SerializeObject(jsonPallet, Formatting.Indented);

            Task<string> postWeightPallet = APIService.PostApexAsync(EndPoints.WeightPallets, jsonSerialized);
            string response = await postWeightPallet;

            if (!string.IsNullOrEmpty(response))
            {
                dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(response);
                Console.WriteLine($"{DateService.Today()} > {responsePayload.Message}.  Pallet [{lblPalletId.Text}]");
            }
            else
            {
                Console.Write($"{DateService.Today()} > IoT Pallet[APEX]. Sin respuesta");
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

            Task<string> postWeightRoll = APIService.PostApexAsync(EndPoints.WeightRolls, jsonSerialized);
            string response = await postWeightRoll;

            if (!string.IsNullOrEmpty(response))
            {
                dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(response);
                Console.WriteLine($"{DateService.Today()} > {responsePayload.Message}. Rollo [{rollId}]");
            }
            else
            {
                Console.Write($"{DateService.Today()} > IoT Rollo[APEX]. Sin respuesta");
            }
        }
        #endregion
    }
}
