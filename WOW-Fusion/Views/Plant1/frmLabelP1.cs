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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace WOW_Fusion
{
    public partial class frmLabelP1 : Form
    {
        PopController pop;

        //JObjets response
        private JObject machines = null;
        private JObject workCenters = null;

        private int workCenterSelected = -1;
        private string itemId = string.Empty;

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
                cmbWorkCenters.Items.Add(item.WorkCenterName.ToString());
            }
        }

        private void cmbWorkCenters_DropDownClosed(object sender, EventArgs e)
        {
            if (cmbWorkCenters.SelectedIndex == -1 && workCenterSelected != -1)
            {
                /*cmbWorkCenters.Items.Clear();
                cmbWorkCenters.Items.Add(workCenters["items"][workCenterSelected]["WorkCenterName"].ToString());
                //workCenterUnselected = true;
                cmbWorkCenters.SelectedIndex = 0;  */  
            }
        }

        private void SelectedIndexChangedWorkCenters(object sender, EventArgs e)
        {
            if (cmbWorkCenters.SelectedIndex != -1)
            {

                if (workCenters == null) { return; }

                workCenterSelected = cmbWorkCenters.SelectedIndex;

                dynamic ct = workCenters["items"][workCenterSelected]; //Objeto CENTROS DE TRABAJO

                lblWorkAreaName.Text = ct.WorkAreaName.ToString();
                //workCenterId = ct.WorkCenterId.ToString();

                cmbWorkOrders.Items.Clear();
                lblItemNumber.Text = string.Empty;
                lblItemDescription.Text = string.Empty;
                lblOutputQuantity.Text = string.Empty;
                lblUoM.Text = string.Empty;
                lblResourceName.Text = string.Empty;
                lblResourceCode.Text = string.Empty;
                lblPlannedStartDate.Text = string.Empty;
                lblPlannedCompletionDate.Text = string.Empty;

                cmbWorkOrders.Enabled = true;
            }
        }

        private async void DropDownOpenWorkOrders(object sender, EventArgs e)
        {
            cmbWorkOrders.Items.Clear();
            picBoxWaitWO.Visible = true;

            List<string> workOrderNumbers = await CommonService.WOProcessByWorkCenter(Constants.Plant1Id, workCenters["items"][workCenterSelected]["WorkCenterId"].ToString()); //Obtener datos de la orden
            picBoxWaitWO.Visible = false;

            if (workOrderNumbers == null) return;

            List<string> ordersPrinted = FileController.ContentFile(Constants.PathPrintedLables);

            foreach (string order in workOrderNumbers)
            {
                //Reimpresión activada
                if (checkBoxReprint.Checked)
                {
                    //Ordenes IGUAL a las ya impresas y aún en RELEASED
                    if (FileController.IsOrderPrinted(ordersPrinted, order))
                    {
                        cmbWorkOrders.Items.Add(order);
                    }
                }
                else
                {
                    //Ordenes DIFERENTES a las ya impresas y en RELEASED
                    if (!FileController.IsOrderPrinted(ordersPrinted, order))
                    {
                        cmbWorkOrders.Items.Add(order);
                    }
                }
            }
        }

        private void SelectedIndexChangedWorkOrders(object sender, EventArgs e)
        {
            if (cmbWorkOrders.SelectedIndex != -1)
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

                    lblStartPage.Text = string.IsNullOrEmpty(lblOutputQuantity.Text) ? 0.ToString() : 1.ToString();

                    float additionalLabels = (Settings.Default.Aditional * int.Parse(lblOutputQuantity.Text)) / 100;
                    lblAditional.Text = $"(+{Convert.ToInt32(Math.Round(additionalLabels))})";
                    lblTotalPrint.Text = (float.Parse(lblOutputQuantity.Text) + additionalLabels).ToString();
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

                        string akaCustomer = "STANDARD";
                        dynamic aka = await LabelService.LabelInfo(Constants.Plant2Id, akaCustomer); //Obtener template de etiqueta APEX
                        lblLabelName.Text = aka.LabelName.ToString();

                        FillLabel();

                        btnPrint.Enabled = true;

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
            lblOperationSequenceNumber.Text = string.Empty;
            lblOperationName.Text = string.Empty;
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
                //Reimpresión activada
                if (checkBoxReprint.Checked)
                {
                    if (!string.IsNullOrEmpty(txtBoxStart.Text) && !string.IsNullOrEmpty(txtBoxEnd.Text))
                    {
                        if (int.TryParse(txtBoxStart.Text, out _) && int.TryParse(txtBoxEnd.Text, out _))
                        {
                            if (int.Parse(txtBoxStart.Text) > 0 && int.Parse(txtBoxEnd.Text) <= int.Parse(lblTotalPrint.Text))
                            {
                                if (int.Parse(txtBoxEnd.Text) >= int.Parse(txtBoxStart.Text))
                                {
                                    Constants.pop = "Imprimiendo...";
                                    pop.Show(this);
                                    if (await LabelService.PrintP1(int.Parse(txtBoxStart.Text), int.Parse(txtBoxEnd.Text)))
                                    {
                                        lblStatus.Text = string.Empty;
                                        txtBoxStart.Text = string.Empty;
                                        txtBoxEnd.Text = string.Empty;
                                        checkBoxReprint.Checked = false;
                                        groupBoxReprint.Visible = false;
                                        cmbWorkOrders.Items.Clear();
                                        CleanUIWorkOrders();
                                    }
                                    else
                                    {
                                        lblStatus.Text = "Error de impresión";
                                    }
                                    pop.Close();
                                }
                                else
                                {
                                    lblStatus.Text = "Cantidad final no puede ser menor a la inicial";
                                }
                            }
                            else
                            {
                                lblStatus.Text = "Cantidades fuera del rango permitido";
                            }
                        }
                        else
                        {
                            lblStatus.Text = "Ingrese únicamente números enteros";
                        }
                    }
                    else
                    {
                        lblStatus.Text = "Llene todos los campos";
                    }
                }
                else
                {
                    Constants.pop = "Imprimiendo...";
                    pop.Show(this);
                    if (await LabelService.PrintP1(int.Parse(lblStartPage.Text), int.Parse(lblTotalPrint.Text)))
                    {
                        //Guardar orden impresa
                        await FileController.Write(cmbWorkOrders.SelectedItem.ToString(), Constants.PathPrintedLables);
                        cmbWorkOrders.Items.Clear();
                        CleanUIWorkOrders();
                        pop.Close();
                    }
                    else
                    {
                        pop.Close();
                        NotifierController.Error("Error al imprimir");
                    }
                }
                
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

        private void btnReprint_Click(object sender, EventArgs e)
        {
            /*frmReprint frmReprint = new frmReprint();
            frmReprint.StartPosition = FormStartPosition.CenterParent;
            frmReprint.FormClosed += FrmReprintClosed;
            frmReprint.ShowDialog();*/

            DialogResult dialogResult = MessageBox.Show($"¿Desea reimprimir alguna etiqueta?", "Reimpresión", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                NotifierController.Success("Indique etiquetas a imprimir");
                checkBoxReprint.Checked = true;
                groupBoxReprint.Visible = true;
                txtBoxStart.Focus();
            }
            else if (dialogResult == DialogResult.No)
            {
                return;
            }
        }

        private void txtBoxStart_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtBoxStart.Text, out _))
            {
                if(int.Parse(txtBoxStart.Text) > int.Parse(lblTotalPrint.Text))
                {
                    txtBoxStart.BackColor = Color.LightSalmon;
                    lblStatus.Text = "Valor no puede ser mayor a la cantidad permitida";
                }
                else
                {
                    txtBoxStart.BackColor = Color.White;
                    lblStatus.Text = string.Empty;
                }
            }
            else
            {
                txtBoxStart.BackColor = Color.LightSalmon;
                lblStatus.Text = "Ingrese únicamente números enteros";
            }
        }

        private void txtBoxEnd_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtBoxEnd.Text, out _))
            {
                if (int.Parse(txtBoxEnd.Text) > int.Parse(lblTotalPrint.Text))
                {
                    txtBoxEnd.BackColor = Color.LightSalmon;
                    lblStatus.Text = "Valor no puede ser mayor a la cantidad permitida";
                }
                else
                {
                    txtBoxEnd.BackColor = Color.White;
                    lblStatus.Text = string.Empty;
                }
            }
            else
            {
                txtBoxEnd.BackColor = Color.LightSalmon;
                lblStatus.Text = "Ingrese únicamente números enteros";
            }
        }

        private void cmbWorkOrders_TextChanged(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(cmbWorkOrders.Text)) 
            {
                btnReprint.Enabled = true;
            }
            else
            {
                btnReprint.Enabled = false;
            }
        }
    }
}
