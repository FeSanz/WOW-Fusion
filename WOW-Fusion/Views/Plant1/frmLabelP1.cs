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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tulpep.NotificationWindow;
using WOW_Fusion.Controllers;
using WOW_Fusion.Models;
using WOW_Fusion.Services;

namespace WOW_Fusion
{
    public partial class frmLabelP1 : Form
    {
        PopController pop;

        //JObjets response
        private JObject machines = null;
        private JObject workCenters = null;

        private string workCenterId = string.Empty;

        public frmLabelP1()
        {
            InitializeComponent();
            InitializeFusionData();
        }

        private void frmLabelP1_Load(object sender, EventArgs e)
        {
            pop = new PopController();
            AppController.ToolTip(btnSettings, "Configuración");

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
                lblOrganizationCode.Text = org["OrganizationCode"].ToString();
                lblOrganizationName.Text = org["OrganizationName"].ToString();
                lblLocationCode.Text = org["LocationCode"].ToString();
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

            lblWorkAreaName.Text = ct["WorkAreaName"].ToString();
            workCenterId = ct["WorkCenterId"].ToString();
            
            cmbWorkOrders.Items.Clear();
            lblItemNumber.Text = string.Empty;
            lblItemDescription.Text = string.Empty;
            lblPlannedQuantity.Text = string.Empty;
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

            List<string> workOrderNumbers = await Plant1Service.WorkOrdersListItemEval(Constants.Plant1Id, workCenterId); //Obtener datos de Organizacion
            picBoxWaitWO.Visible = false;

            if (workOrderNumbers == null) return;

            foreach (var item in workOrderNumbers)
            {
                cmbWorkOrders.Items.Add(item.ToString());
            }
        }

        private async void SelectedIndexChangedWorkOrders(object sender, EventArgs e)
        {
            if (cmbWorkOrders.SelectedItem != null)
            {
                CleanUIWorkOrders();
                try
                {
                    pop.Show(this);
                    Task<string> tskWorkOrdersData = APIService.GetRequestAsync(String.Format(EndPoints.WODiscreteDetail, cmbWorkOrders.SelectedItem.ToString()));
                    string response = await tskWorkOrdersData;
                    if (string.IsNullOrEmpty(response)) { return; }

                    JObject objWorkOrder = JObject.Parse(response);
                    dynamic wo = objWorkOrder["items"][0]; //Objeto WORKORDER


                    lblPlannedQuantity.Text = wo["PlannedStartQuantity"].ToString();
                    lblUoM.Text = wo["UOMCode"].ToString();

                    lblItemNumber.Text = wo["ItemNumber"].ToString();
                    lblItemDescription.Text = wo["Description"].ToString();
                    lblItemDescriptionEnglish.Text = TranslateService.Translate(lblItemDescription.Text);
                    lblPlannedStartDate.Text = wo["PlannedStartDate"].ToString();
                    lblPlannedCompletionDate.Text = wo["PlannedCompletionDate"].ToString();
                
                    lblStartPage.Text = string.IsNullOrEmpty(lblPlannedQuantity.Text) ? "" : "1";
                    float additionalLabels = (Properties.Settings.Default.Aditional * int.Parse(lblPlannedQuantity.Text)) / 100;
                    lblAditional.Text = $"(+{Convert.ToInt32(Math.Round(additionalLabels))})";
                    lblTotalPrint.Text = (float.Parse(lblPlannedQuantity.Text) + additionalLabels).ToString();
                    lbLabelQuantity.Text = lblPlannedQuantity.Text;
                    
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
                            lblResourceName.Text = resource["ResourceName"].ToString();

                            lblLabelDesign.Text = "XIPRD";

                            dynamic aka = await LabelService.LabelInfo(lblLabelDesign.Text);
                            FillLabel();

                            if ((int)resource["WorkOrderOperationResourceInstance"]["count"] >= 1)
                            {
                                dynamic instance = resource["WorkOrderOperationResourceInstance"]["items"][0]; // Objeto INSTANCIA
                                lblEquipmentInstanceCode.Text = instance["EquipmentInstanceCode"].ToString();
                                lblEquipmentInstanceName.Text = instance["EquipmentInstanceName"].ToString();
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
            else
            {
                NotifierController.Warning("No se ha seleccionado ningún elemento");
            }
        }

        private void CleanUIWorkOrders()
        {
            lblItemNumber.Text = string.Empty;
            lblPlannedQuantity.Text = string.Empty;
            lblUoM.Text = "--";
            lblItemDescription.Text = string.Empty;
            lblItemDescriptionEnglish.Text = string.Empty;
            lblResourceCode.Text = string.Empty;
            lblResourceName.Text = string.Empty;
            lblEquipmentInstanceCode.Text = string.Empty;
            lblEquipmentInstanceName.Text= string.Empty;
            lblPlannedStartDate.Text = string.Empty;
            lblPlannedCompletionDate.Text = string.Empty;
        }

        private async void btnPrint_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbWorkOrders.Text))
            {
                NotifierController.Warning("Seleccione orden de trabajo");
            }
            else
            {
                await LabelService.PrintP1(int.Parse(lblTotalPrint.Text));
            }
        }

        /*private void cmbDesignLabels_DropDown(object sender, EventArgs e)
        {
            picBoxWaitLD.Visible = true;
            cmbDesignLabels.Items.Clear();
            cmbDesignLabels.Items.AddRange(LabelService.FilesDesign(Constants.PathLabelsP1));
            picBoxWaitLD.Visible = false;
        }*/

        /*private void cmbDesignLabels_SelectedValueChanged(object sender, EventArgs e)
        {
            

            if (cmbDesignLabels.SelectedItem != null)
            {                
                dynamic label = JObject.Parse(Constants.LabelJson);

                label.WORKORDER = cmbWorkOrders.Text;
                label.ITEMNUMBER = lblItemNumber.Text;
                label.ITEMDESCRIPTION = lblItemDescription.Text;
                label.DESCRIPTIONENGLISH = TranslateService.Translate(lblItemDescription.Text);
                label.EQU = lblEquipmentInstanceCode.Text;
                label.DATE = DateService.Now();
                label.BOXNUMBER = "1".PadLeft(5, '0');

                Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);

                picLabel.Image = Image.FromStream(LabelService.CreateFromFile(cmbDesignLabels.SelectedItem.ToString(), 1));
            }
        }*/

        private void FillLabel()
        {
            if (!string.IsNullOrEmpty(lblItemNumber.Text))
            {
                dynamic label = JObject.Parse(Constants.LabelJson);

                label.WORKORDER = string.IsNullOrEmpty(cmbWorkOrders.Text) ? " " : cmbWorkOrders.Text;
                label.ITEMNUMBER = string.IsNullOrEmpty(lblItemNumber.Text) ? " " : lblItemNumber.Text;
                label.ITEMDESCRIPTION = string.IsNullOrEmpty(lblItemDescription.Text) ? " " : lblItemDescription.Text;
                label.DESCRIPTIONENGLISH = string.IsNullOrEmpty(lblItemDescriptionEnglish.Text) ? " " : lblItemDescriptionEnglish.Text;
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
    }
}
