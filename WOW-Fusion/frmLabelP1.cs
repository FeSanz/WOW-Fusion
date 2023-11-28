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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

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
            
            lblAdditional.Text = trackBarPercentageAdd.Value.ToString();
        }

        private async void InitializeFusionData()
        {
            List<string> orgInfo = await CommonService.Organization(Constants.Plant1Id); //Obtener datos de Organizacion

            if (orgInfo == null) return;

            lblOrganizationCode.Text = orgInfo[0];
            lblOrganizationName.Text = orgInfo[1];
            lblLocationCode.Text = orgInfo[2];
            machines = await CommonService.ProductionResourcesMachines(Constants.Plant1Id); //Obtener Objeto RECURSOS MAQUINAS
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
            lblResourceDescription.Text= string.Empty;
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
                    Task<string> tskWorkOrdersData = APIService.GetRequestAsync(String.Format(EndPoints.WorkOrderDetail, cmbWorkOrders.SelectedItem.ToString()));
                    string response = await tskWorkOrdersData;
                    if (string.IsNullOrEmpty(response)) { return; }

                    JObject objWorkOrder = JObject.Parse(response);
                    dynamic wo = objWorkOrder["items"][0]; //Objeto WORKORDER


                    lblPlannedQuantity.Text = wo["PlannedStartQuantity"].ToString();
                    lblUoM.Text = wo["UOMCode"].ToString();

                    lblItemNumber.Text = wo["ItemNumber"].ToString();
                    lblItemDescription.Text = wo["Description"].ToString();
                    lblPlannedStartDate.Text = wo["PlannedStartDate"].ToString();
                    lblPlannedCompletionDate.Text = wo["PlannedCompletionDate"].ToString();

                    trackBarPercentageAdd.Enabled = string.IsNullOrEmpty(lblPlannedQuantity.Text) ? false : true;
                    lblStartPage.Text = string.IsNullOrEmpty(lblPlannedQuantity.Text) ? "" : "1";
                    lblEndPage.Text = lblPlannedQuantity.Text;

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
                            }
                            else
                            {
                                NotifierController.Warning("Datos de instancia de máquina no encontrados");
                            }

                            cmbDesignLabels.Enabled = true;
                            btnPrint.Enabled = true;
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
                }
                catch (Exception ex)
                {
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
            lblPlannedQuantity.Text = "";
            lblUoM.Text = "--";
            lblItemNumber.Text = string.Empty;
            lblResourceDescription.Text = string.Empty;
            lblResourceCode.Text = string.Empty;
            lblResourceDescription.Text = string.Empty;
            lblEquipmentInstanceCode.Text = string.Empty;
            lblEquipmentInstanceName.Text= string.Empty;
        }

        private void trackBarPercentageAdd_Scroll(object sender, EventArgs e)
        {
            lblAdditional.Text = trackBarPercentageAdd.Value.ToString() + "";
            if (!string.IsNullOrEmpty(lblPlannedQuantity.Text))
            {
                //float additionalQuantity = int.Parse(lblPlannedQuantity.Text) + ((trackBarPercentageAdd.Value * int.Parse(lblPlannedQuantity.Text)) / 100);
                //lblEndPage.Text = Convert.ToInt32(Math.Round(additionalQuantity)).ToString();
                int totalPrint = int.Parse(lblPlannedQuantity.Text) + trackBarPercentageAdd.Value;
                lblEndPage.Text = totalPrint.ToString();
            }
        }

        private async void btnPrint_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbWorkOrders.Text))
            {
                NotifierController.Warning("Seleccione orden de trabajo");
            }
            else if(string.IsNullOrEmpty(cmbDesignLabels.Text))
            {
                NotifierController.Warning("Seleccione diseño de etiqueta");
            }
            else
            {
                await LabelService.Print(int.Parse(lblEndPage.Text));
            }
        }

        private void cmbDesignLabels_DropDown(object sender, EventArgs e)
        {
            picBoxWaitLD.Visible = true;
            cmbDesignLabels.Items.Clear();
            cmbDesignLabels.Items.AddRange(LabelService.FilesDesign());
            picBoxWaitLD.Visible = false;
        }

        private void cmbDesignLabels_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbDesignLabels.SelectedItem != null)
            {
                LabelService.labelDictionary.Clear();
                LabelService.labelDictionary.Add("WORKORDER", cmbWorkOrders.Text);
                LabelService.labelDictionary.Add("ITEMNUMBER", lblItemNumber.Text);
                LabelService.labelDictionary.Add("ITEMDESCRIPTION", lblItemDescription.Text);
                LabelService.labelDictionary.Add("DESCRIPTIONENGLISH", TranslateService.Translate(lblItemDescription.Text));
                LabelService.labelDictionary.Add("EQU", lblEquipmentInstanceCode.Text);
                LabelService.labelDictionary.Add("DATE", DateService.Now());
                LabelService.labelDictionary.Add("BOXNUMBER", "1".PadLeft(5, '0'));

                picLabel.Image = Image.FromStream(LabelService.CreateFromFile(cmbDesignLabels.SelectedItem.ToString()));
            }
        }

    }
}
