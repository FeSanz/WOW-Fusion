using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tulpep.NotificationWindow;
using WOW_Fusion.Controllers;
using WOW_Fusion.Models;
using WOW_Fusion.Services;
using WOW_Fusion.Views.Plant1;
using WOW_Fusion.Properties;

namespace WOW_Fusion
{
    public partial class frmLabelP1 : Form
    {
        PopController pop;

        //JObjets response
        private JObject machines = null;
        private JObject workCenters = null;

        private string workCenterId = string.Empty;
        private string itemId = string.Empty;

        //Variables impresion
        public static int startPage = 0;
        public static int endPage = 0;
        public static string workOrder = string.Empty;
        public static int totalQuantity = 0;

        public frmLabelP1()
        {
            InitializeComponent();
            InitializeFusionData();
        }

        private void frmLabelP1_Load(object sender, EventArgs e)
        {
            pop = new PopController();
            AppController.ToolTip(btnSettings, "Configuración");
            AppController.ToolTip(btnReprint, "Reimprimir");

            ConsoleController console = new ConsoleController(rtbLog);
            Console.SetOut(console);
        }

        private async void InitializeFusionData()
        {
            //Obtener datos de Organizacion
            dynamic org = await CommonService.OneItem(String.Format(EndPoints.InventoryOrganizations, Constants.Plant1Id));

            if (org == null)
            {
                NotifierController.Error("Sin organización, la aplicación no funcionará");
                return;
            }
            else
            {
                lblOrganizationCode.Text = org.OrganizationCode.ToString();
                lblOrganizationName.Text = org.OrganizationName.ToString();
                lblLocationCode.Text = org.LocationCode.ToString();
                machines = await CommonService.ProductionResourcesMachines(String.Format(EndPoints.ProductionResourcesP1, Constants.Plant1Id)); //Obtener Objeto RECURSOS MAQUINAS
            }
        }

        private async void DropDownOpenWorkCenters(object sender, EventArgs e)
        {
            cmbWorkCenters.Items.Clear();
            picBoxWaitWC.Visible = true;

            workCenters = await CommonService.WorkCenters(Constants.Plant1Id); //Obtener Objeto CENTROS DE TRABAJO
            picBoxWaitWC.Visible = false;

            if (workCenters == null) return;

            dynamic items = workCenters["items"];

            foreach (var item in items)
            {
                cmbWorkCenters.Items.Add(item["WorkCenterName"].ToString());
            }
        }

        private void SelectedIndexChangedWorkCenters(object sender, EventArgs e)
        {
            int index = cmbWorkCenters.SelectedIndex;

            if(workCenters == null) { return; }

            dynamic ct = workCenters["items"][index]; //Objeto CENTROS DE TRABAJO

            lblWorkAreaName.Text = ct.WorkAreaName.ToString();
            workCenterId = ct.WorkCenterId.ToString();
            
            cmbWorkOrders.Items.Clear();
            lblItemNumber.Text = string.Empty;
            lblItemDescription.Text = string.Empty;
            lblOutputQuantity.Text = string.Empty;
            lblUoM.Text = string.Empty;
            lblResourceName.Text= string.Empty;
            lblResourceCode.Text= string.Empty;
            lblPlannedStartDate.Text = string.Empty;
            lblPlannedCompletionDate.Text = string.Empty;

            cmbWorkOrders.Enabled = true;
        }

        private async void DropDownOpenWorkOrders(object sender, EventArgs e)
        {
            cmbWorkOrders.Items.Clear();
            picBoxWaitWO.Visible = true;

            List<string> workOrderNumbers = await CommonService.WOProcessByWorkCenter(Constants.Plant1Id, workCenterId); //Obtener datos de Organizacion
            picBoxWaitWO.Visible = false;

            if (workOrderNumbers == null) return;

            List<string> ordersPrinted = FileController.ContentFile(Constants.PathPrintedLables);

            foreach(string order in workOrderNumbers)
            {
                if(!FileController.IsOrderPrinted(ordersPrinted, order))
                {
                    cmbWorkOrders.Items.Add(order);
                }
            }
        }

        private void SelectedIndexChangedWorkOrders(object sender, EventArgs e)
        {
            if (cmbWorkOrders.SelectedItem != null)
            {
                WorkOrderUIFill(cmbWorkOrders.SelectedItem.ToString());
            }
            else
            {
                NotifierController.Warning("No se ha seleccionado ningún elemento");
            }
        }

        private async void WorkOrderUIFill(string workOrder)
        {
            CleanUIWorkOrders();
            try
            {
                pop.Show(this);
                Task<string> tskWorkOrdersData = APIService.GetRequestAsync(String.Format(EndPoints.WOProcessDetailP1, workOrder));
                string response = await tskWorkOrdersData;
                if (string.IsNullOrEmpty(response)) { return; }

                JObject objWorkOrder = JObject.Parse(response);
                if ((int)objWorkOrder["count"] == 0)
                {
                    NotifierController.Warning("Datos de orden no encotrada");
                    return;
                }

                dynamic wo = objWorkOrder["items"][0]; //Objeto WORKORDER

                /*dynamic itemsV2 = await CommonService.OneItem(String.Format(EndPoints.Item, wo.ItemNumber.ToString(), Constants.Plant1Id));
                if (itemsV2 != null)
                {
                    itemId = itemsV2.ItemId.ToString();
                }
                else
                {
                    itemId = string.Empty;
                    NotifierController.Warning("Datos de item no encontrados");
                }*/

                int countOutput = (int)wo.ProcessWorkOrderOutput.count;
                if (countOutput == 2)
                {
                    int indexOutput = -1;
                    for (int i = 0; i < countOutput; i++)
                    {
                        if ((int)wo.ProcessWorkOrderOutput.items[i].OperationSequenceNumber == 20)//Siempre buscar en SECUENCIA 20
                        {
                            indexOutput = i;
                            break;
                        }
                    }
                    dynamic output = wo.ProcessWorkOrderOutput.items[indexOutput];

                    lblOperationSequenceNumber.Text = output.OperationSequenceNumber;
                    lblOperationName.Text = output.OperationName;

                    lblOutputQuantity.Text = output.OutputQuantity.ToString();
                    lblUoM.Text = output.UOMCode.ToString();
                    itemId = output.InventoryItemId.ToString();
                    lblItemNumber.Text = output.ItemNumber.ToString();
                    lblItemDescription.Text = output.ItemDescription.ToString();
                    lblItemDescriptionEnglish.Text = TranslateService.Translate(lblItemDescription.Text);
                    lblPlannedStartDate.Text = wo.PlannedStartDate.ToString();
                    lblPlannedCompletionDate.Text = wo.PlannedCompletionDate.ToString();

                    startPage = string.IsNullOrEmpty(lblOutputQuantity.Text) ? 0 : 1;
                    lblStartPage.Text = startPage.ToString();

                    float additionalLabels = (Settings.Default.Aditional * int.Parse(lblOutputQuantity.Text)) / 100;
                    lblAditional.Text = $"(+{Convert.ToInt32(Math.Round(additionalLabels))})";
                    totalQuantity = (int)(float.Parse(lblOutputQuantity.Text) + additionalLabels);
                    endPage = totalQuantity;
                    lblTotalPrint.Text = endPage.ToString();
                    lbLabelQuantity.Text = lblOutputQuantity.Text;
                }
                else
                {
                    NotifierController.Warning("Secuencias definidas incorrectamente");
                }

                /*
                    lblPrimaryProductQuantity.Text = wo.PrimaryProductQuantity.ToString();
                    lblUoM.Text = wo.UOMCode.ToString();
                    lblItemNumber.Text = wo.ItemNumber.ToString();
                    lblItemDescription.Text = wo.Description.ToString();
                    lblItemDescriptionEnglish.Text = TranslateService.Translate(lblItemDescription.Text);
                    lblPlannedStartDate.Text = wo.PlannedStartDate.ToString();
                    lblPlannedCompletionDate.Text = wo.PlannedCompletionDate.ToString();

                    startPage = string.IsNullOrEmpty(lblPrimaryProductQuantity.Text) ? 0 : 1;
                    lblStartPage.Text = startPage.ToString();

                    float additionalLabels = (Properties.Settings.Default.Aditional * int.Parse(lblPrimaryProductQuantity.Text)) / 100;
                    lblAditional.Text = $"(+{Convert.ToInt32(Math.Round(additionalLabels))})";
                    totalQuantity = (int)(float.Parse(lblPrimaryProductQuantity.Text) + additionalLabels);
                    endPage = totalQuantity;
                    lblTotalPrint.Text = endPage.ToString();
                    lbLabelQuantity.Text = lblPrimaryProductQuantity.Text; 
                 */

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
                        lblResourceCode.Text = resource.ResourceCode.ToString();
                        lblResourceName.Text = resource.ResourceName.ToString();

                        lblLabelDesign.Text = "XIPRD";

                        dynamic aka = await LabelService.LabelInfo(lblLabelDesign.Text);
                        FillLabel();

                        btnPrint.Enabled = true;
                        btnReprint.Enabled = true;

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
                            NotifierController.Warning("Datos de instancia de máquina no encontrados");
                        }
                    }
                    else
                    {
                        NotifierController.Warning("Datos de recurso máquina no encontrados");
                    }
                }
                else
                {
                    NotifierController.Warning("Orden sin recursos");
                }
                pop.Close();
            }
            catch (Exception ex)
            {
                pop.Close();
                MessageBox.Show("Error. " + ex.Message, "Error [WorkOrderSelected]", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CleanUIWorkOrders()
        {
            lblItemNumber.Text = string.Empty;
            lblOutputQuantity.Text = string.Empty;
            lblUoM.Text = "--";
            lblItemDescription.Text = string.Empty;
            lblItemDescriptionEnglish.Text = string.Empty;
            lblResourceCode.Text = string.Empty;
            lblResourceName.Text = string.Empty;
            lblEquipmentInstanceCode.Text = string.Empty;
            lblEquipmentInstanceName.Text= string.Empty;
            lblPlannedStartDate.Text = string.Empty;
            lblPlannedCompletionDate.Text = string.Empty;

            btnPrint.Enabled = false;
            btnReprint.Enabled = false;
        }

        private async void btnPrint_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbWorkOrders.Text))
            {
                NotifierController.Warning("Seleccione orden de trabajo");
            }
            else
            {
                await LabelService.PrintP1(int.Parse(lblStartPage.Text), int.Parse(lblTotalPrint.Text));
                //Guardar orden impresa
                await FileController.Write(cmbWorkOrders.SelectedItem.ToString(), Constants.PathPrintedLables);
            }
        }

        private void FillLabel()
        {
            if (!string.IsNullOrEmpty(lblItemNumber.Text))
            {
                dynamic label = JObject.Parse(Constants.LabelJson);

                label.ITEMNUMBER = string.IsNullOrEmpty(lblItemNumber.Text) ? " " : lblItemNumber.Text;
                label.ITEMDESCRIPTION = string.IsNullOrEmpty(lblItemDescription.Text) ? " " : lblItemDescription.Text;
                label.ENGLISHDESCRIPTION = string.IsNullOrEmpty(lblItemDescriptionEnglish.Text) ? " " : lblItemDescriptionEnglish.Text;
                label.WORKORDER = string.IsNullOrEmpty(cmbWorkOrders.Text) ? " " : cmbWorkOrders.Text.Substring(7);
                label.UPCA = string.IsNullOrEmpty(itemId) ? $"{Constants.UPCPrefix}0000" : $"{Constants.UPCPrefix}{itemId.Substring(itemId.Length - 4)}";
                label.EQU = string.IsNullOrEmpty(lblResourceCode.Text) ? " ": lblResourceCode.Text;
                label.DATE = DateService.Now();
                label.BOX = "1";

                Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);

                picLabel.Image = Image.FromStream(LabelService.UpdateLabelLabelary(1, "BOX"));
                btnPrint.Enabled = true;
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmSettingsP1 frmSettingsP1 = new frmSettingsP1();
            frmSettingsP1.StartPosition = FormStartPosition.CenterParent;
            frmSettingsP1.ShowDialog();
        }

        private void FrmReprintClosed(object sender, FormClosedEventArgs e)
        {
            Refresh();
            lblStartPage.Text = startPage.ToString();
            lblTotalPrint.Text = endPage.ToString();
        }

        private void btnReprint_Click(object sender, EventArgs e)
        {
            frmReprint frmReprint = new frmReprint();
            frmReprint.StartPosition = FormStartPosition.CenterParent;
            frmReprint.FormClosed += FrmReprintClosed;
            frmReprint.ShowDialog();
        }
    }
}
