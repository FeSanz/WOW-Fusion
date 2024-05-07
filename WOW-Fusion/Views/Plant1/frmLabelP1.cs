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
using WOW_Fusion.Views.Plant2;

namespace WOW_Fusion
{
    public partial class frmLabelP1 : Form
    {
        PopController pop;

        //JObjets response
        private JObject machines = null;
        private JObject workCenters = null;

        private int workCenterSelected = -1;
        private int workOrderId = 0;
        private string itemId = string.Empty;

        private string _akaCustomer = "STANDARD";


        public frmLabelP1()
        {
            InitializeComponent();
            InitializeFusionData();
        }

        private void frmLabelP1_Load(object sender, EventArgs e)
        {
            lblUserName.Text = Constants.UserName;

            pop = new PopController();
            AppController.ToolTip(btnSettings, "Configuración");
            AppController.ToolTip(btnReprint, "Reimprimir");
            AppController.ToolTip(btnCloseReprint, "Cerrar reimpresión");
            
            rtbLog.Clear();
            ConsoleController console = new ConsoleController(rtbLog);
            Console.SetOut(console);
            Console.WriteLine($"Bienvenido {Constants.UserName}", Color.Black);
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
                Constants.BusinessUnitId = org.ManagementBusinessUnitId.ToString();
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
                if (item.WorkCenterName.ToString() != "SERVICIOS" && item.WorkCenterName.ToString() != "INSTALACIONES" &&
                    item.WorkCenterName.ToString() != "DECORADO")
                {
                    cmbWorkCenters.Items.Add(item.WorkCenterName.ToString());
                }
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

                CleanAll();

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

            List<string> ordersPrinted = FileController.ContentFile(Constants.OrdersPrintedP1);
            List<string> ordersReprinted = FileController.ContentFile(Constants.OrdersReprintedP1);
            foreach (string order in workOrderNumbers)
            {
                //Reimpresión activada
                if (checkBoxReprint.Checked)
                {
                    //Ordenes impresas
                    if (FileController.IsOrderPrinted(ordersPrinted, order))
                    {
                        if(!FileController.IsOrderPrinted(ordersReprinted, order))
                        {
                            cmbWorkOrders.Items.Add(order);
                        }
                    }
                }
                else
                {
                    //Ordenes pendientes por imprimir
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
            pop.Show(this);
            try
            {
                //♥ Consultar WORKORDER ♥
                Task<string> tskWorkOrdersData = APIService.GetRequestAsync(String.Format(EndPoints.WOProcessDetailP1, workOrder, Constants.Plant1Id));
                string response = await tskWorkOrdersData;
                if (string.IsNullOrEmpty(response)) { pop.Close(); return; }

                JObject objWorkOrder = JObject.Parse(response);
                if ((int)objWorkOrder["count"] == 0)
                {
                    pop.Close();
                    NotifierController.Warning("Datos de orden no encotrada");
                    return;
                }

                dynamic wo = objWorkOrder["items"][0]; //Objeto WORKORDER
                workOrderId = wo.WorkOrderId;


                lblOutputQuantity.Text = wo.PrimaryProductQuantity.ToString();
                lblUoM.Text = wo.PrimaryProductUOMCode.ToString();
                itemId = wo.PrimaryProductId.ToString();
                lblItemNumber.Text = wo.ItemNumber.ToString();
                lblItemDescription.Text = wo.Description.ToString();
                lblItemDescriptionEnglish.Text = TranslateService.Translate(lblItemDescription.Text);
                lblPlannedStartDate.Text = wo.PlannedStartDate.ToString();
                lblPlannedCompletionDate.Text = wo.PlannedCompletionDate.ToString();

                lblStartPage.Text = string.IsNullOrEmpty(lblOutputQuantity.Text) ? 0.ToString() : 1.ToString();

                //Porcentaje adicional
                float additionalLabels = (Settings.Default.Aditional * int.Parse(lblOutputQuantity.Text)) / 100;
                lblAditional.Text = $"(+{Convert.ToInt32(Math.Round(additionalLabels))})";
                lblTotalPrint.Text = (float.Parse(lblOutputQuantity.Text) + additionalLabels).ToString();
                lbLabelQuantity.Text = lblOutputQuantity.Text;

                //Obtener datos de máquina
                int countResources = (int)wo.ProcessWorkOrderResource.count;
                if (countResources >= 1)
                {
                    int indexMachine = -1;
                    //Buscar maquina entre recursos de producción
                    for (int i = 0; i < countResources; i++)
                    {
                        for (int j = 0; j < (int)machines["count"]; j++)
                        {
                            string resourceOrder = wo.ProcessWorkOrderResource.items[i].ResourceId.ToString();
                            string resourceMachines = machines["items"][j]["ResourceId"].ToString();
                            if (resourceOrder.Equals(resourceMachines))
                            {
                                indexMachine = i;
                            }
                        }
                    }
                    //obtener index de maquina encontrada
                    if (indexMachine >= 0)
                    {
                        dynamic resource = wo.ProcessWorkOrderResource.items[indexMachine]; //Objeto RESURSO
                        string resourceCode = resource.ResourceCode.ToString();
                        lblResourceCode.Text = resourceCode.Replace("-EMP", "");
                        string resourceName = resource.ResourceName.ToString();
                        lblResourceName.Text = resourceName.Replace("-EMP", "");
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

                //Flex orden de venta
                string flexPV = wo.ProcessWorkOrderDFF.items[0].pedidoDeVenta.ToString();
                if (string.IsNullOrEmpty(flexPV))
                {
                    lblAkaOrder.Text = "NA";
                    _akaCustomer = "STANDARD";
                }
                else
                {
                    lblAkaOrder.Text = flexPV;

                    //♥ Consultar OM & AKA ♥
                    AkaData();
                }
                TemplateLabel();
            }
            catch (Exception ex)
            {
                pop.Close();
                MessageBox.Show("Error. " + ex.Message, "Error [WorkOrderSelected]", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            pop.Close();
        }

        //Obtener datos OM & AKA
        private async void AkaData()
        {
            //♥ Consultar OM ♥
            dynamic om = await CommonService.OneItem(String.Format(EndPoints.SalesOrders, lblAkaOrder.Text, Constants.BusinessUnitId));
            if (om != null)
            {
                lblAkaCustomer.Text = om.BuyingPartyName.ToString();//BuyingPartyNumber
                _akaCustomer = lblAkaCustomer.Text;

                //♥ Consultar TradingPartnerItemRelationships ♥
                dynamic aka = await CommonService.OneItem(String.Format(EndPoints.TradingPartnerItemRelationships, lblItemNumber.Text, lblAkaCustomer.Text));
                if (aka != null)
                {
                    lblAkaItem.Text = aka.TradingPartnerItemNumber.ToString();
                    lblAkaDescription.Text = aka.RelationshipDescription.ToString();
                }
                else
                {
                    Console.WriteLine($"Orden sin datos AKA [{DateService.Today()}]", Color.Black);
                }
            }
            else
            {
                Console.WriteLine($"Pedido de venta no encontrado [{DateService.Today()}]", Color.Red);
            }
        }

        //Template etiqueta y validar boton de impresión
        private async void TemplateLabel()
        {
            //Consultar template etiqueta en APEX 
            dynamic labelApex = await LabelService.LabelInfo(Constants.Plant1Id, _akaCustomer);
            if (labelApex.LabelName.ToString().Equals("null"))
            {
                pop.Close();
                MessageBox.Show("Etiqueta de cliente/producto no encontrada", "Verificar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                lblLabelName.Text = labelApex.LabelName.ToString();
                FillLabel();
            }

            //Validar activacion de boton de pesaje
            if (!string.IsNullOrEmpty(cmbWorkOrders.Text) && !string.IsNullOrEmpty(lblResourceName.Text) && !string.IsNullOrEmpty(lblLabelName.Text))
            {
                if (lblAkaOrder.Text.Equals("NA") && _akaCustomer.Equals("STANDARD"))
                {
                    btnPrint.Enabled = true;
                }
                else
                {
                    btnPrint.Enabled = string.IsNullOrEmpty(lblAkaItem.Text) ? false : true;
                }
            }
            else
            {
                btnPrint.Enabled = false;
            }
        }

        private void CleanAll()
        {
            //WOrkOrder Section
            cmbWorkOrders.Items.Clear();
            //Item Section
            lblItemNumber.Text = string.Empty;
            lblOutputQuantity.Text = string.Empty;
            lblUoM.Text = "--";
            lblItemDescription.Text = string.Empty;
            lblItemDescriptionEnglish.Text = string.Empty;
            //Resource Section
            lblResourceCode.Text = string.Empty;
            lblResourceName.Text = string.Empty;
            //Planned Dates
            lblPlannedStartDate.Text = string.Empty;
            lblPlannedCompletionDate.Text = string.Empty;
            //AKA Section
            lblAkaOrder.Text = string.Empty;
            lblAkaItem.Text = string.Empty;
            lblAkaCustomer.Text = string.Empty;
            lblAkaDescription.Text = string.Empty;
            //Reprint Section
            groupBoxReprint.Visible = false;
            txtBoxStart.Text = string.Empty;
            txtBoxEnd.Text = string.Empty;
            lblStatus.Text = string.Empty;
            //Label Preview Section
            lblLabelName.Text = string.Empty;
            lbLabelQuantity.Text = string.Empty;
            lblAditional.Text = "(+0)";
            lblStartPage.Text = string.Empty;
            lblTotalPrint.Text = string.Empty;
            picLabel.Image = null;
            btnPrint.Enabled = false;
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
                                        ReprintApex();
                                        CleanAll();

                                        checkBoxReprint.Checked = false;
                                        btnPrint.Text = "IMPRIMIR";
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
                        await FileController.Write(cmbWorkOrders.SelectedItem.ToString(), Constants.OrdersPrintedP1);
                        RegisterPrintApex();
                        CleanAll();
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

        private async void FillLabel()
        {
            if (!string.IsNullOrEmpty(lblItemNumber.Text))
            {
                dynamic label = JObject.Parse(Constants.LabelJson);

                label.ITEMNUMBER = string.IsNullOrEmpty(lblItemNumber.Text) ? " " : lblItemNumber.Text;
                label.ITEMDESCRIPTION = string.IsNullOrEmpty(lblItemDescription.Text) ? " " : lblItemDescription.Text;
                label.ENGLISHDESCRIPTION = string.IsNullOrEmpty(lblItemDescriptionEnglish.Text) ? " " : lblItemDescriptionEnglish.Text;
                label.WORKORDER = string.IsNullOrEmpty(cmbWorkOrders.Text) ? " " : cmbWorkOrders.Text/*.Substring(7)*/;
                label.UPCA = string.IsNullOrEmpty(itemId) ? $"{Constants.UPCPrefix}0000" : $"{Constants.UPCPrefix}{itemId.Substring(itemId.Length - 4)}";
                label.EQU = string.IsNullOrEmpty(lblResourceName.Text) ? " ": lblResourceName.Text;
                label.DATE = DateService.Now();
                label.BOX = "1";

                Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);

                picLabel.Image = Image.FromStream(await LabelService.UpdateLabelLabelary(1, "BOX"));
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
            DialogResult dialogResult = MessageBox.Show($"¿Desea reimprimir alguna etiqueta?", "Reimpresión", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                NotifierController.Success("Seleccione la orden a imprimir");

                CleanAll();
                checkBoxReprint.Checked = true;
                groupBoxReprint.Visible = true;
                btnPrint.Text = "REIMPRIMIR";
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
        }

        #region APEX
        private async void RegisterPrintApex()
        {
            dynamic jsonPrint = JObject.Parse(Payloads.printHistory);

            jsonPrint.DateMark = DateService.EpochTime();
            jsonPrint.WorkOrderId = workOrderId;
            jsonPrint.UserId = Constants.UserId;
            jsonPrint.PrintCount = int.Parse(lblTotalPrint.Text);

            string jsonSerialized = JsonConvert.SerializeObject(jsonPrint, Formatting.Indented);

            Task<string> postPrint = APIService.PostApexAsync(EndPoints.PrintHistory, jsonSerialized);
            string response = await postPrint;

            if (!string.IsNullOrEmpty(response))
            {
                dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(response);
                Console.WriteLine($"{responsePayload.Message} [{DateService.Today()}]", Color.Green);
            }
            else
            {
                Console.WriteLine($"Sin respuesta al impresión [{DateService.Today()}]", Color.Red);
            }
        }

        private async void ReprintApex()
        {
            //Guardar orden reimpresa
            await FileController.Write(cmbWorkOrders.Text.ToString(), Constants.OrdersReprintedP1);

            dynamic jsonReprint = JObject.Parse(Payloads.reprint);

            jsonReprint.WorkOrderId = workOrderId;
            jsonReprint.UserId = Constants.UserId;
            jsonReprint.ReprintCount = txtBoxStart.Text + "-" + txtBoxEnd.Text;

            string jsonSerialized = JsonConvert.SerializeObject(jsonReprint, Formatting.Indented);

            Task<string> putReprint = APIService.PutApexAsync(EndPoints.PrintHistory, jsonSerialized);
            string response = await putReprint;

            if (!string.IsNullOrEmpty(response))
            {
                dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(response);
                Console.WriteLine($"{responsePayload.Message} [{DateService.Today()}]", Color.Green);
            }
            else
            {
                Console.WriteLine($"Sin respuesta al actualizar orden reimpresa [{DateService.Today()}]", Color.Red);
            }
        }
        #endregion

        private void btnCloseReprint_Click(object sender, EventArgs e)
        {
            CleanAll();
            checkBoxReprint.Checked = false;
            groupBoxReprint.Visible = false;
            btnPrint.Text = "IMPRIMIR";
        }
    }
}
