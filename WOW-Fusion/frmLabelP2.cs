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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace WOW_Fusion
{
    public partial class frmLabelP2 : Form
    {
        Random rnd = new Random();

        PopController pop;

        //Pesos params
        private float _tareWeight = 0.0f;
        private float _weightFromWeighing = 0.0f;
        private float _previousWeight = 0.0f;
        private float _lbs = 2.205f;

        private int _rowSelected = 0;

        //Rollos pallet control
        private int _rollCount = 0;
        private int _palletCount = 0;

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
            ConsoleController console = new ConsoleController(txtBoxConsole);
            Console.SetOut(console);

            Console.WriteLine($"{DateService.Today()} -> Radwag {Settings.Default.WeighingIP}");
            Console.WriteLine($"{DateService.Today()} -> Zebra {Settings.Default.PrinterIP}");

            btnGetWeight.Text = "TARA";

            TableLayoutPalletControl();
            /* 
          for(int i = 0; i < 5; i++)
          {
              string[] fill = new string[] {(i+1).ToString(), rnd.Next(10,50).ToString(), rnd.Next(10, 50).ToString(), rnd.Next(10, 50).ToString() , rnd.Next(10, 50).ToString() };
              dgWeights.Rows.Add(fill);
          }

       IEnumerable<string> columnValues = dgWeights.Rows.Cast<DataGridViewRow>().Where(row => row.Cells[1].Value != null).Select(row => row.Cells[1].Value.ToString());

          // Convert the IEnumerable to an array
          string[] resultArray = columnValues.ToArray();
          resultArray.ToList().ForEach(i => MessageBox.Show(i.ToString()));*/
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

                        if ((int)resource["WorkOrderOperationResourceInstance"]["count"] >= 1)
                        //if ((int)resource["ResourceInstance"]["count"] >= 1)
                        {
                            //dynamic instance = resource["ResourceInstance"]["items"][0]; // Objeto INSTANCIA
                            dynamic instance = resource["WorkOrderOperationResourceInstance"]["items"][0]; // Objeto INSTANCIA
                            lblEquipmentInstanceCode.Text = instance["EquipmentInstanceCode"].ToString();
                            lblEquipmentInstanceName.Text = instance["EquipmentInstanceName"].ToString();

                            //Fill label
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
                        _tareWeight = float.Parse(requestTareWeight);
                        lblTare.Text = float.Parse(requestTareWeight).ToString("F2");
                        txtBoxWeight.Text = float.Parse(requestTareWeight).ToString("F2");
                        btnGetWeight.Text = "OBTENER";
                        _rollCount = 0;
                        _palletCount += 1;

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

                    if(_weightFromWeighing < _previousWeight)
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
                        //Llenar campos de pallet (NET siempre sera el peso acomuladod de la bascula)
                        lblPalletNetKg.Text = _weightFromWeighing.ToString("F2");
                        lblPalletGrossKg.Text = (_weightFromWeighing + _tareWeight).ToString("F2");

                        lblPalletNetLbs.Text = (_weightFromWeighing * _lbs).ToString("F2");
                        lblPalletGrossLbs.Text = ((_weightFromWeighing +_tareWeight) * _lbs).ToString("F2");

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

        private void AddRoll(float rollNetKg)
        {
            _rollCount++;
            float rollNetLbs = rollNetKg * _lbs;

            //Calcular pero bruto de cada rollo (con tara)
            float rollGrossKg = rollNetKg + _tareWeight;
            float rollGrossLbs = rollGrossKg * _lbs;

            //Agregar pesos a datagrid
            string[] row = new string[] { _rollCount.ToString(), rollNetKg.ToString("F2"),rollGrossKg.ToString("F2"),
                                                                        rollNetLbs.ToString("F2"), rollGrossLbs.ToString("F2") };
            dgWeights.Rows.Add(row);


            //Reserver peso neto acomulado para sacar peso de rollo
            _previousWeight = _weightFromWeighing;

            //Llenar campos de pallet (SUMA)
            float palletNetSum = dgWeights.Rows.Cast<DataGridViewRow>().Sum(t => float.Parse(t.Cells[1].Value.ToString()));

            if (dgWeights.RowCount == 1)
            {
                DataGridViewButtonColumn btnColumnPrint = new DataGridViewButtonColumn();
                {
                    btnColumnPrint.HeaderText = "Imprimir";
                    btnColumnPrint.Name = "btnPrintLabel";
                    btnColumnPrint.FlatStyle = FlatStyle.Flat;
                    btnColumnPrint.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    btnColumnPrint.UseColumnTextForButtonValue = true;
                }
                dgWeights.Columns.Add(btnColumnPrint);
            }

            FillLabelRoll(row);
        }

        private void TableLayoutPalletControl()
        {

            tabLayoutPallet.RowCount = 1;
            tabLayoutPallet.ColumnCount = 2;
            


            /*tabLayoutPallet.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tabLayoutPallet.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            //tabLayoutPallet.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            //tabLayoutPallet.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tabLayoutPallet.Controls.Add(picRoll, 0, 0);
            tabLayoutPallet.Controls.Add(picRoll2, 1, 0);*/

            //List<List<Control>> contrlList = new List<List<Control>>();
            for (int row = 0; row < tabLayoutPallet.RowCount; row++)
            {
                
                for (int col = 0; col < tabLayoutPallet.ColumnCount; col++)
                {
                    PictureBox picRoll = new PictureBox();
                    picRoll.Image = Resources.roll_icon;
                    picRoll.BackColor = Color.Transparent;
                    picRoll.SizeMode = PictureBoxSizeMode.Zoom;
                    picRoll.Dock = DockStyle.Fill;

                    tabLayoutPallet.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                    tabLayoutPallet.Controls.Add(picRoll, col, row);
                }
            }
        }

        private void dgWeight_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            //Columna a colocar icono
            if(e.ColumnIndex == 5)
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

        private async void dgWeights_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {
                int rollForPrint = int.Parse(dgWeights.CurrentRow.Cells[0].Value.ToString());
                
                string[] row = new string[dgWeights.CurrentRow.Cells.Count - 1];
                for(int i=0; i < dgWeights.CurrentRow.Cells.Count -1; i++)
                {
                    row[i] = dgWeights.CurrentRow.Cells[i].Value.ToString();
                }
                FillLabelRoll(row);
                await LabelService.PrintP2(rollForPrint, "ROLL");
            }
        }

        private void dgWeights_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow row in dgWeights.Rows)
            {
                float rollNetKg = float.Parse(row.Cells[2].Value.ToString());
                if (rollNetKg > Settings.Default.RollMax)
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                    //row.DefaultCellStyle.ForeColor = Color.White;
                }
                else if(rollNetKg < Settings.Default.RollMin)
                {
                    row.DefaultCellStyle.BackColor = Color.Yellow;
                    //row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else
                {
                    continue;
                }
            }
        }

        private void dgWeights_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex == dgWeights.Rows.Count - 1)
            {
                dgWeights.Rows[e.RowIndex].Selected = true;
                _rowSelected = e.RowIndex;
                dgWeights.CurrentCell = dgWeights.Rows[e.RowIndex].Cells[1];
                MenuShipDeleteWeight.Show(dgWeights, e.Location);
                MenuShipDeleteWeight.Show(Cursor.Position);
            }
        }
       private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!dgWeights.Rows[_rowSelected].IsNewRow)
            {
                //Restar peso eliminado en PESO PREVIO peara evitar inconsistencias
                _previousWeight -= float.Parse(dgWeights.CurrentRow.Cells[1].Value.ToString());
                dgWeights.Rows.RemoveAt(_rowSelected);
                lblPalletNetKg.Text = _previousWeight.ToString("F2");
                //Restar 1 a la cantidad de rollos
                _rollCount -= 1;
            }
        }

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
                label.ROLLNUMBER = "R" + weights[0].PadLeft(4, '0');
                label.WNETKG = weights[1];
                label.WGROSSKG = weights[2];
                label.WNETLBS = weights[3];
                label.WGROSSLBS = weights[4];

                Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);

                if (_rollCount == 1)
                {
                    picLabelRoll.Image = Image.FromStream(await LabelService.CreateFromApexAsync(lblLabelDesignRoll.Text, 2));
                }
                else
                {
                    picLabelRoll.Image = Image.FromStream(LabelService.UpdateLabelLabelary(_rollCount));
                }
            }
        }

        private async void FillLabelPallet(string weights)
        {
            if (!string.IsNullOrEmpty(lblEquipmentInstanceName.Text))
            {
                dynamic label = JObject.Parse(Constants.LabelJson);

                label.EQU = lblEquipmentInstanceCode.Text;
                label.PALLETNUMBER = "P" + _palletCount.ToString().PadLeft(4, '0');
                label.WEIGHTS = weights;
                Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);

                picLabelPallet.Image = Image.FromStream(await LabelService.CreateFromApexAsync(lblLabelDesignRoll.Text, 3));
            }
        }

        private async void btnPrintPallet_Click(object sender, EventArgs e)
        {
            await LabelService.PrintP2(_palletCount, "PALLET");
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmSettingsP2 frmSettingsP2 = new frmSettingsP2();
            frmSettingsP2.StartPosition = FormStartPosition.CenterParent;
            frmSettingsP2.FormClosed += FrmSettingsP2Closed;
            frmSettingsP2.ShowDialog();
        }

        private void FrmSettingsP2Closed(object sender, FormClosedEventArgs e)
        {
            this.Refresh();
            InitializeFusionData();
        }

        private void checkBoxPallet_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBoxPallet.Checked)
            {
                btnGetWeight.Enabled = false;
                btnPrintPallet.Enabled = true;

                IEnumerable<string> columnWeigthsNetKg = dgWeights.Rows.Cast<DataGridViewRow>()
                                              .Where(row => row.Cells[1].Value != null)
                                              .Select(row => row.Cells[1].Value.ToString());

                string[] weigthsArray = columnWeigthsNetKg.ToArray();
                string strWeights = "";
                for (int i = 0; i < weigthsArray.Length; i++)
                {
                    strWeights += $"{i + 1}-{weigthsArray[i]},";
                }

                //weigthsArray.ToList().ForEach(i => );
                FillLabelPallet(strWeights.TrimEnd(','));
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}
