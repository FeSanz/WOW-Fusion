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
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Controllers;
using WOW_Fusion.Models;
using WOW_Fusion.Properties;
using WOW_Fusion.Services;

namespace WOW_Fusion
{
    public partial class frmLabelP2 : Form
    {
        Random rnd = new Random();
        System.Windows.Forms.Timer timerShift = new System.Windows.Forms.Timer();

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

        #region Start
        public frmLabelP2()
        {
            InitializeComponent();
            InitializeFusionData();
        }

        private void frmLabelP2_Load(object sender, EventArgs e)
        {
            pop = new PopController();

            ConsoleController console = new ConsoleController(txtBoxConsole);
            Console.SetOut(console);

            AppController.ToolTip(btnSettings, "Configuración");
            AppController.ToolTip(btnAddPallet, "Agregar Pallet");
            Console.WriteLine($"{DateService.Today()} -> {0}");

            btnGetWeight.Text = "TARA";
        }

        private async void InitializeFusionData()
        {
            //Obtener datos de Organizacion
            dynamic org = await CommonService.OneItem(String.Format(EndPoints.InventoryOrganizations, Constants.Plant2Id));
            //Obtener datos de centro de trabajo
            dynamic wc = await CommonService.OneItem(String.Format(EndPoints.WorkCentersById, Constants.Plant2Id, Settings.Default.WorkCenterP2));

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

            List<string> workOrderNumbers = await CommonService.WODiscreteByWorkCenter(Constants.Plant2Id, Settings.Default.WorkCenterP2); //Obtener OT
            picBoxWaitWO.Visible = false;

            if (workOrderNumbers == null) return;

            foreach (var item in workOrderNumbers)
            {
                cmbWorkOrders.Items.Add(item.ToString());
            }
        }

        private async void SelectedIndexChangedWorkOrders(object sender, EventArgs e)
        {
            timerShift.Stop();
            try
            {
                Task<string> tskWorkOrdersData = APIService.GetRequestAsync(String.Format(EndPoints.WODiscreteDetail, cmbWorkOrders.SelectedItem.ToString()));
                string response = await tskWorkOrdersData;
                if (string.IsNullOrEmpty(response)) { return; }

                JObject objWorkOrder = JObject.Parse(response);
                if ((int)objWorkOrder["count"] == 0)
                {
                    NotifierController.Warning("Datos de orden no encotrada");
                    return;
                }
                dynamic wo = objWorkOrder["items"][0]; //Objeto WORKORDER

                lblPlannedQuantity.Text = wo["PlannedStartQuantity"].ToString();
                //lblPlannedQuantity.Text = wo["BatchQuantity"].ToString();
                lblCompletedQuantity.Text = wo["CompletedQuantity"].ToString();
                lblUoM.Text = wo["UOMCode"].ToString();

                lblItemNumber.Text = wo["ItemNumber"].ToString();
                lblItemDescription.Text = wo["Description"].ToString();
                lblItemDescriptionEnglish.Text = TranslateService.Translate(wo["Description"].ToString());
                lblPlannedStartDate.Text = wo["PlannedStartDate"].ToString();
                lblPlannedCompletionDate.Text = wo["PlannedCompletionDate"].ToString();

                int countResources = (int)wo["WorkOrderResource"]["count"];
                //int countResources = (int)wo["ProcessWorkOrderResource"]["count"];
                if (countResources >= 1)
                {
                    int indexMachine = -1;

                    for (int i = 0; i < countResources; i++)
                    {
                        for (int j = 0; j < (int)machines["count"]; j++)
                        {
                            string resourceOrder = wo["WorkOrderResource"]["items"][i]["ResourceId"].ToString();
                            //string resourceOrder = wo["ProcessWorkOrderResource"]["items"][i]["ResourceId"].ToString();
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
                        //dynamic resource = wo["ProcessWorkOrderResource"]["items"][indexMachine]; //Objeto RESURSO
                        lblResourceCode.Text = resource["ResourceCode"].ToString();
                        lblResourceDescription.Text = resource["ResourceDescription"].ToString();
                        _resourceId = resource["ResourceId"].ToString();

                        if ((int)resource["WorkOrderOperationResourceInstance"]["count"] >= 1)
                        //if ((int)resource["ResourceInstance"]["count"] >= 1)
                        {
                            //dynamic instance = resource["ResourceInstance"]["items"][0]; // Objeto INSTANCIA
                            dynamic instance = resource["WorkOrderOperationResourceInstance"]["items"][0]; // Objeto INSTANCIA
                            lblEquipmentInstanceCode.Text = instance["EquipmentInstanceCode"].ToString();
                            lblEquipmentInstanceName.Text = instance["EquipmentInstanceName"].ToString();

                            shifts = await CommonService.OneItem(String.Format(EndPoints.ShiftByWorkCenter, Settings.Default.WorkCenterP2));
                            lblShift.Text = (shifts == null) ? string.Empty : DateService.CurrentShift(shifts, _resourceId);

                            timerShift.Interval = 5000;
                            timerShift.Tick += new EventHandler(CheckShift);
                            timerShift.Start();

                            btnGetWeight.Enabled = true;

                            lblLabelDesignRoll.Text = "XILAM";
                            lblLabelDesignPallet.Text = "PALLET";
                        }
                        else
                        {
                            NotifierController.Warning("Datos de máquina no encontrados");
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
        private void btnGetWeight_Click(object sender, EventArgs e)
        {
            txtBoxWeight.Text = "";
            pop.Show(this);
            if (string.IsNullOrEmpty(lblTare.Text))
            {
                //Solicitar peso tara
                string responseTare = RadwagController.SocketWeighing("T");
                if (responseTare.Equals("OK"))
                {
                    string requestTareWeight = RadwagController.SocketWeighing("OT");
                    if (!requestTareWeight.Equals("EX"))
                    {
                        //TARAR
                        _tareWeight = float.Parse(requestTareWeight);
                        lblTare.Text = float.Parse(requestTareWeight).ToString("F2");
                        txtBoxWeight.Text = float.Parse(requestTareWeight).ToString("F2");
                        btnGetWeight.Text = "OBTENER";
                        //_rollCount = 0;
                        _rollByPallet = 0;
                        _palletCount += 1;
                        _previousWeight = 0;

                        tabLayoutPallet.BackgroundImage = Resources.pallet_icon;

                        lblPalletNumber.Text = _palletCount.ToString();
                        cmbWorkOrders.Enabled = false; // Deshabilitar combo de Ordenes
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
                string responseWeighing = RadwagController.SocketWeighing("S");

                if (responseWeighing == "EX")
                {
                    NotifierController.Warning("Tiempo de espera agotado, vuelva a  intentar");
                }
                else
                {
                    //La bascula solo acomula el peso neto (SIN TARA)
                    _weightFromWeighing = float.Parse(responseWeighing);

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

                        if (rollNetKg > Settings.Default.RollMax)
                        {
                            DialogResult dialogResult = MessageBox.Show($"Peso fuera del rango ({txtBoxWeight.Text} kg) ¿Desea continuar?", "Fuera del rango", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                AddRoll(rollNetKg);
                            }
                            else if (dialogResult == DialogResult.No)
                            {
                                txtBoxWeight.Text = string.Empty;
                            }
                        }
                        else if (rollNetKg < Settings.Default.RollMin)
                        {
                            DialogResult dialogResult = MessageBox.Show($"Peso debajo del rango ({txtBoxWeight.Text} kg) ¿Desea continuar?", "Debajo del rango", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                AddRoll(rollNetKg);
                            }
                            else if (dialogResult == DialogResult.No)
                            {
                                txtBoxWeight.Text = string.Empty;
                            }
                        }
                        else
                        {
                            AddRoll(rollNetKg);
                        }
                    }
                }
            }
            pop.Close();
        }
        private void btnAddPallet_Click(object sender, EventArgs e)
        {
            if (_isPalletStart)
            {
                DialogResult dialogResult = MessageBox.Show($"¿Desea agregar nuevo pallet?", "Agregar pallet", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string[] rowPallet = new string[] { _palletCount.ToString(), _tareWeight.ToString(), lblPalletNetKg.Text,lblPalletGrossKg.Text,
                                                                        lblPalletNetLbs.Text, lblPalletGrossLbs.Text};
                    dgPallets.Rows.Add(rowPallet);

                    ClearForPalletNew();

                    if (dgPallets.RowCount == 1)
                    {
                        DataGridViewButtonColumn btnColumnPrint = new DataGridViewButtonColumn();
                        {
                            btnColumnPrint.HeaderText = "Imprimir";
                            btnColumnPrint.Name = "btnPrintLabelPallet";
                            btnColumnPrint.FlatStyle = FlatStyle.Flat;
                            btnColumnPrint.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            btnColumnPrint.UseColumnTextForButtonValue = true;
                        }
                        dgPallets.Columns.Add(btnColumnPrint);
                    }
                    //_palletCount++;
                }
                else if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }
            else
            {
                NotifierController.Warning("Aún no cuenta con datos de pesaje del nuevo pallet");
            }
        }

        private void ClearForPalletNew()
        {
            _rollByPallet = 0;
            _isPalletStart = false;
            tabLayoutPallet.BackgroundImage = Resources.pallet_empty_icon;
            TableLayoutPalletControl(_rollByPallet);
            btnGetWeight.Text = "TARA";
            
            lblPalletNetKg.Text = string.Empty;
            lblPalletGrossKg.Text = string.Empty;
            lblPalletNetLbs.Text = string.Empty;
            lblPalletGrossLbs.Text = string.Empty;
            lblTare.Text = string.Empty;
        }
        private void btnEndOrder_Click(object sender, EventArgs e)
        {

        }
        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmSettingsP2 frmSettingsP2 = new frmSettingsP2();
            frmSettingsP2.StartPosition = FormStartPosition.CenterParent;
            frmSettingsP2.FormClosed += FrmSettingsP2Closed;
            frmSettingsP2.ShowDialog();
        }
        #endregion

        #region DataGrid Rollos
        private void AddRoll(float rollNetKg)
        {
            _rollCount++;
            _rollByPallet++;
            float rollNetLbs = rollNetKg * _lbs;

            //Calcular pero bruto de cada rollo (con tara)
            float rollGrossKg = rollNetKg + _tareWeight;
            float rollGrossLbs = rollGrossKg * _lbs;

            //Agregar pesos a datagrid
            string[] row = new string[] { _palletCount.ToString(), _rollCount.ToString(), rollNetKg.ToString("F2"),rollGrossKg.ToString("F2"),
                                                                        rollNetLbs.ToString("F2"), rollGrossLbs.ToString("F2") };

            dgRolls.Rows.Add(row);
            TableLayoutPalletControl(_rollByPallet);

            //Reserver peso neto acomulado para sacar peso de rollo
            _previousWeight = _weightFromWeighing;

            //Llenar campos de pallet (SUMA)
            float palletNetSum = dgRolls.Rows.Cast<DataGridViewRow>().Sum(t => float.Parse(t.Cells["R_NetKg"].Value.ToString()));
            lblCompletedQuantity.Text = palletNetSum.ToString();

            if (dgRolls.RowCount == 1)
            {
                DataGridViewButtonColumn btnColumnPrint = new DataGridViewButtonColumn();
                {
                    btnColumnPrint.HeaderText = "Imprimir";
                    btnColumnPrint.Name = "btnPrintLabel";
                    btnColumnPrint.FlatStyle = FlatStyle.Flat;
                    btnColumnPrint.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    btnColumnPrint.UseColumnTextForButtonValue = true;
                }
                dgRolls.Columns.Add(btnColumnPrint);
            }
            _isPalletStart = true;
            FillLabelRoll(row);
        }

        private void TableLayoutPalletControl(int rollNumber)
        {
            lblRollCount.Text = $"+ {rollNumber}";

            if (rollNumber <= 12)
            {
                lblRollCount.Visible = false;
                int count = 0;

                tabLayoutPallet.Controls.Clear();
                tabLayoutPallet.RowStyles.Clear();
                tabLayoutPallet.ColumnStyles.Clear();

                tabLayoutPallet.ColumnCount = rollNumber > 4 ? 4 : rollNumber;

                if (rollNumber >= 9)
                {
                    tabLayoutPallet.RowCount = 3;
                }
                else if (rollNumber >= 5 && rollNumber <= 8)
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
                        picRoll.Image = Resources.roll_icon;
                        picRoll.BackColor = Color.Transparent;
                        picRoll.SizeMode = PictureBoxSizeMode.Zoom;
                        picRoll.Dock = DockStyle.Fill;

                        count++;

                        if (count <= rollNumber)
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

        //Agregar botón para imprimir etiqueta
        private void dgWeight_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            //Columna a colocar icono
            if (e.ColumnIndex == 6)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Resources.printer_01.Width / 20;
                var h = Resources.printer_01.Height / 20;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Resources.printer_01, new Rectangle(x, y, w, h));
                e.Handled = true;
            }

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

        //Imprimir etiqueta (Click sobre fila)
        private async void dgWeights_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 6)
            {
                int rollForPrint = int.Parse(dgRolls.CurrentRow.Cells["R_Roll"].Value.ToString());

                string[] row = new string[dgRolls.CurrentRow.Cells.Count - 1];
                for (int i = 0; i < dgRolls.CurrentRow.Cells.Count - 1; i++)
                {
                    row[i] = dgRolls.CurrentRow.Cells[i].Value.ToString();
                }
                FillLabelRoll(row);
                await LabelService.PrintP2(rollForPrint, "ROLL");
            }
        }

        //Cambio de color de filas (Max-Min)
        private void dgWeights_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //Combinar celdas (Quitar valor repetido)
            if (e.RowIndex == 0)
                return;
            if (IsTheSameCellValue(e.ColumnIndex, e.RowIndex))
            {
                e.Value = "";
                e.FormattingApplied = true;
            }

            //Cambiar color
            foreach (DataGridViewRow row in dgRolls.Rows)
            {
                float rollNetKg = float.Parse(row.Cells["R_NetKg"].Value.ToString());
                if (rollNetKg > Settings.Default.RollMax)
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                }
                else if (rollNetKg < Settings.Default.RollMin)
                {
                    row.DefaultCellStyle.BackColor = Color.Yellow;
                }
                else
                {
                    continue;
                }
            }
        }

        private void dgRolls_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            //dgRolls.Rows[e.RowIndex].Selected = true;
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
        private string WeightsPallet(int palletSelected)
        {
            IEnumerable<string> columnWeigthsNetKg = dgRolls.Rows.Cast<DataGridViewRow>().Where(row => row.Cells["R_Pallet"].Value.ToString().Equals(palletSelected.ToString()))
                                                                                            .Select(row => row.Cells["R_NetKg"].Value.ToString());

            string[] weigthsArray = columnWeigthsNetKg.ToArray();
            string strWeights = "";

            //weigthsArray.ToList().ForEach(i => );
            for (int i = 0; i < weigthsArray.Length; i++)
            {
                strWeights += $"{i + 1}-{weigthsArray[i]},";
            }

            return strWeights.TrimEnd(',');
        }

        private void dgPallets_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            int palletAdded = int.Parse(dgPallets.Rows[e.RowIndex].Cells["P_Pallet"].Value.ToString());
            string rollWeights = WeightsPallet(palletAdded);

            string[] palletWeight = new string[6];
            for (int i = 0; i < 6; i++)
            {
                palletWeight[i] = dgPallets.Rows[e.RowIndex].Cells[i].Value.ToString();
            }

            FillLabelPallet(palletWeight, rollWeights);
        }
        //Agregar boton para imprimir etiqueta
        private void dgWeightsPallets_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            //Columna a colocar icono
            if (e.ColumnIndex == 6)
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

        //Imprimir etiqueta pallet (Click sobre fila)
        private async void dgWeightsPallets_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 6)
            {
                int palletForPrint = int.Parse(dgPallets.CurrentRow.Cells["P_Pallet"].Value.ToString());

                string[] row = new string[dgPallets.CurrentRow.Cells.Count - 1];
                for (int i = 0; i < dgPallets.CurrentRow.Cells.Count - 1; i++)
                {
                    row[i] = dgPallets.CurrentRow.Cells[i].Value.ToString();
                }
                //FillLabelPallet();
                await LabelService.PrintP2(palletForPrint, "PALLET");
            }
        }
        #endregion

        #region Labels Fill
        private async void FillLabelRoll(string[] weights)
        {
            if (!string.IsNullOrEmpty(lblEquipmentInstanceName.Text))
            {
                dynamic label = JObject.Parse(Constants.LabelJson);

                label.WORKORDER = cmbWorkOrders.Text;
                label.ITEMNUMBER = lblItemNumber.Text;
                label.ITEMDESCRIPTION = lblItemDescription.Text;
                label.ENGLISHDESCRIPTION = lblItemDescriptionEnglish.Text;
                label.EQU = lblEquipmentInstanceCode.Text;
                label.DATE = DateService.Now();
                label.SHIFT = lblShift.Text;
                label.ROLLNUMBER = "R" + weights[1].PadLeft(4, '0');
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

                if (_rollCount == 1)
                {
                    picLabelRoll.Image = Image.FromStream(await LabelService.CreateFromApexAsync(lblLabelDesignRoll.Text, 2));
                }
                else
                {
                    picLabelRoll.Image = Image.FromStream(LabelService.UpdateLabelLabelary(_rollCount, "ROLL"));
                }
            }
        }

        private void FillLabelPallet(string[] palletWeight, string rollWeights)
        {
            if (!string.IsNullOrEmpty(lblEquipmentInstanceName.Text))
            {
                dynamic label = JObject.Parse(Constants.LabelJson);

                label.PALLETNUMBER = "P" + _palletCount.ToString().PadLeft(4, '0');
                label.WNETKG = palletWeight[2];
                label.WGROSSKG = palletWeight[3];
                label.WNETLBS = palletWeight[4];
                label.WGROSSLBS = palletWeight[5];
                label.WEIGHTS = rollWeights;
                Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);

                //picLabelPallet.Image = Image.FromStream(await LabelService.CreateFromApexAsync(lblLabelDesignRoll.Text, 3));
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
        #endregion
    }
}
