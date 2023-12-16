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

        //Pesos params
        private float _tareWeight = 0.0f;
        private float _weightFromWeighing = 0.0f;
        private float _previousWeight = 0.0f;

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

                            //Fill label
                            btnGetWeight.Enabled = true;
                            cmbWorkOrders.Enabled = false;

                            lblLabelDesignRoll.Text = "XILAM";
                            lblLabelDesignPallet.Text = "PALLET";

                            FillLabelRoll();
                        } 
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
                        _tareWeight = float.Parse(requestTareWeight);
                        lblPalletTare.Text = float.Parse(requestTareWeight).ToString("F2");
                        txtBoxWeight.Text = float.Parse(requestTareWeight).ToString("F2");
                        btnGetWeight.Text = "OBTENER";
                        _rollCount = 0;
                        _palletCount += 1;

                        lblPalletNumber.Text = _palletCount.ToString();
                    }
                    else
                    {
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
                //Obtiene peso acomulado (sin tara)
                string responseWeighing = RadwagController.SocketWeighing("S");

                if (responseWeighing == "EX")
                {
                    pop.Close();
                    NotifierController.Warning("Tiempo de espera agotado, vuelva a  intentar");
                }
                else
                {
                    pop.Close();

                    //La bascula solo acomula el peso neto (SIN TARA)
                    _weightFromWeighing = float.Parse(responseWeighing);

                    //Llenar campos de pallet (NET siempre sera el peso acomuladod de la bascula)
                    lblPalletNet.Text = _weightFromWeighing.ToString("F2");
                    lblPalletGross.Text = (_weightFromWeighing + _tareWeight).ToString("F2");

                    //Calcular pero neto de cada rollo (SIN TARA)
                    float rollNetKg = _weightFromWeighing - _previousWeight;
                    txtBoxWeight.Text = rollNetKg.ToString("F2");

                    if (rollNetKg > Settings.Default.RollMax) {
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
                    else if(rollNetKg < Settings.Default.RollMin)
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
            pop.Close();
        }

        private void AddRoll(float rollNetKg)
        {
            _rollCount++;
            float rollNetLbs = rollNetKg * 2.205f;

            //Calcular pero bruto de cada rollo (con tara)
            float rollGrossKg = rollNetKg + _tareWeight;
            float rollGrossLbs = rollGrossKg * 2.205f;

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

        private void dgWeights_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {
                DataGridViewRow row = dgWeights.Rows[e.RowIndex];
                MessageBox.Show(dgWeights.SelectedCells[0].Value.ToString());
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
                dgWeights.Rows.RemoveAt(_rowSelected);
                //Restar peso eliminado en PESO PREVIO peara evitar inconsistencias
                _previousWeight -= float.Parse(dgWeights.SelectedCells[1].Value.ToString());
                lblPalletNet.Text = _previousWeight.ToString("F2");
                //Restar 1 a la cantidad de rollos
                _rollCount -= 1;
            }
        }

        private async void FillLabelRoll()
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
                label.ROLLNUMBER = "1".PadLeft(4, '0');
                label.PALLETNUMBER = "1".PadLeft(4, '0');
                label.WNETKG = lblPalletNet.Text;
                label.WNETLBS = lblPalletNet.Text;
                label.WGROSSKG = lblPalletGross.Text;
                label.WGROSSLBS = lblPalletGross.Text;

                Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);

                picLabel.Image = Image.FromStream(await LabelService.CreateFromApexAsync(lblLabelDesignRoll.Text, 2));
            }
        }

        private void btnPrintPallet_Click(object sender, EventArgs e)
        {

        }
    }
}
