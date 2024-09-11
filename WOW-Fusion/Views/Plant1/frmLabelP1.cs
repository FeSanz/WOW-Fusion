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
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using Google.Api.Gax;

namespace WOW_Fusion
{
    public partial class frmLabelP1 : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        private const uint SC_CLOSE = 0xF060;
        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint MF_GRAYED = 0x00000001;
        private const uint MF_ENABLED = 0x00000000;
        private const uint MF_DISABLED = 0x00000002;

        PopController pop;

        //JObjets response
        private JObject machines = null;
        private JObject workCenters = null;

        private int workCenterSelected = -1;
        private string workCenterIdSelected = string.Empty;
        private List<string> workCentersNew = new List<string>();

        private Int64 workOrderId = 0;
        private string itemId = string.Empty;

        private string _akaCustomer = "DEFAULT";
        private int lastPagePrinted = -1;

        private string _tradingPartnerName = string.Empty;
        private string _CustomerPONumber = string.Empty;

        private static frmLabelP1 instance;
        private bool _isPrint = false;

        public frmLabelP1()
        {
            InitializeComponent();
            instance = this;
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
            lblEnvironment.Text = Settings.Default.FusionUrl.Contains("-test") ? "TEST" : "PROD";
            //Obtener datos de Organizacion
            dynamic org = await CommonService.OneItem(String.Format(EndPoints.InventoryOrganizations, Constants.Plant1Id));

            if (org == null)
            {
                NotifierController.Error("Sin organización, la aplicación no funcionará");
                return;
            }
            else
            {
                Constants.BusinessUnitId = /*org.ManagementBusinessUnitId.ToString()*/"300000003202139";
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
            //List<dynamic> ordersPosition = new List<dynamic>();
            foreach (var item in items)
            {
                if (item.WorkCenterId.ToString() != "300000003225523" && item.WorkCenterId.ToString() != "300000003225522")
                {
                    //ordersPosition.Add(new List<dynamic> { item.WorkCenterId.ToString(), item.WorkCenterName.ToString() });
                    cmbWorkCenters.Items.Add(item.WorkCenterName.ToString());
                }
            }
        }

        private void SelectedIndexChangedWorkCenters(object sender, EventArgs e)
        {
            if (cmbWorkCenters.SelectedIndex != -1)
            {

                if (workCenters == null) { return; }

                //workCenterSelected = cmbWorkCenters.SelectedIndex;

                dynamic reviewWC = workCenters["items"];
                for (int i=0; i<= (int)workCenters["count"]; i++)
                {
                    if (reviewWC[i].WorkCenterName.ToString() == cmbWorkCenters.Text)
                    {
                        workCenterSelected = i;
                        break;
                    }
                }
                dynamic ct = workCenters["items"][workCenterSelected]; //Objeto CENTROS DE TRABAJO

                workCenterIdSelected = ct.WorkCenterId.ToString();
                lblWorkAreaName.Text = ct.WorkAreaName.ToString();

                CleanAll();

                cmbWorkOrders.Enabled = true;
            }
        }

        private async void DropDownOpenWorkOrders(object sender, EventArgs e)
        {
            cmbWorkOrders.Items.Clear();
            picBoxWaitWO.Visible = true;

            List<string> workOrderNumbers = workCenterIdSelected.Equals("300000003822431") ? //EXTRUSION
                await CommonService.WOByCenterAndOperation(Constants.Plant1Id, workCenterIdSelected, "50") : 
                await CommonService.WOProcessByWorkCenter(Constants.Plant1Id, workCenterIdSelected);
            //-------------------------- Ordenar --------------------
            List<int> workOrderNumberSort = new List<int>();
            foreach (string order in workOrderNumbers)
            {
                if (int.TryParse(order, out int orderNumber)) { workOrderNumberSort.Add(orderNumber); }
            }
            workOrderNumberSort.Sort();
            //-------------------------- Ordenado -------------------
            picBoxWaitWO.Visible = false;

            if (workOrderNumbers == null) return;

            List<string> ordersPrinted = FileController.ContentFile(Constants.OrdersPrintedP1);
            foreach (int order in workOrderNumberSort)
            {
                //Reimpresión activada
                if (checkBoxReprint.Checked)
                {
                    if (FileController.IsOrderPrinted(ordersPrinted, order.ToString()))
                    {
                        cmbWorkOrders.Items.Add(order.ToString());
                    }
                }
                else
                {
                    cmbWorkOrders.Items.Add(order.ToString());
                }
            }
            /*List<string> ordersPrinted = FileController.ContentFile(Constants.OrdersPrintedP1);
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
            }*/
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
            cmbWorkCenters.Enabled = false; 
            cmbWorkOrders.Enabled = false;
            pop.Show(this);
            try
            {
                //♥ Consultar WORKORDER ♥
                Task<string> tskWorkOrdersData = APIService.GetRequestAsync(String.Format(EndPoints.WOProcessDetailP1, workOrder, Constants.Plant1Id));
                string response = await tskWorkOrdersData;

                if (string.IsNullOrEmpty(response)) 
                {
                    pop.Close();
                    cmbWorkCenters.Enabled = true;
                    cmbWorkOrders.Enabled = true;
                    return; 
                }
                
                JObject objWorkOrder = JObject.Parse(response);
                if ((int)objWorkOrder["count"] == 0)
                {
                    pop.Close();
                    NotifierController.Warning("Datos de orden no encotrada");
                    cmbWorkCenters.Enabled = true;
                    cmbWorkOrders.Enabled = true;
                    return;
                }
                

                dynamic wo = objWorkOrder["items"][0]; //Objeto WORKORDER
                workOrderId = wo.WorkOrderId;

                lblOutputQuantity.Text = wo.PrimaryProductQuantity.ToString();
                lblUoM.Text = wo.PrimaryProductUOMCode.ToString();
                itemId = wo.PrimaryProductId.ToString();
                lblItemNumber.Text = wo.ItemNumber.ToString();
                lblItemDescription.Text = wo.Description.ToString();

                List<string> endPoints = new List<string>
                {
                    String.Format(BatchPoints.ItemP1, lblItemNumber.Text, Constants.Plant1Id),
                    String.Format(BatchPoints.GtinP1, lblItemNumber.Text)
                };

                Task<string> batchTsk = APIService.PostBatchRequestAsync(Batchs.BatchPayload(endPoints));
                string batchResponse = await batchTsk;
                if (!string.IsNullOrEmpty(batchResponse))
                {
                    JObject obj = JObject.Parse(batchResponse);

                    if ((int)obj["parts"][0]["payload"]["count"] > 0)
                    {
                        dynamic itemsV2 = obj["parts"][0]["payload"]["items"][0];
                        if (itemsV2 != null)
                        {
                            lblItemDescriptionEnglish.Text = string.IsNullOrEmpty(itemsV2.LongDescription.ToString()) ?
                                                             TranslateService.Translate(lblItemDescription.Text) : itemsV2.LongDescription.ToString();
                        }
                        else
                        {
                            lblItemDescriptionEnglish.Text = TranslateService.Translate(lblItemDescription.Text);
                        }
                    }
                    else
                    {
                        lblItemDescriptionEnglish.Text = TranslateService.Translate(lblItemDescription.Text);
                        Console.WriteLine($"Descripción larga no encontrada [{DateService.Today()}]", Color.Red);
                    }

                    if((int)obj["parts"][1]["payload"]["count"] > 0)
                    {
                        if ((int)obj["parts"][1]["payload"]["count"] == 1)
                        {
                            dynamic GtinRelationships = obj["parts"][1]["payload"]["items"][0];

                            if (GtinRelationships != null)
                            {
                                lblGTIN.Text = string.IsNullOrEmpty(GtinRelationships.GTIN.ToString()) ? string.Empty : GtinRelationships.GTIN.ToString();
                            }
                            else
                            {
                                lblGTIN.Text = string.Empty;
                            }
                        }
                        else
                        {
                            for(int i = 0; i < (int)obj["parts"][1]["payload"]["count"]; i++)
                            {
                                dynamic GtinRelationships = obj["parts"][1]["payload"]["items"][i];
                                if (GtinRelationships.RegistryId.ToString() == "GCA00DL")
                                {
                                    lblGTIN.Text = string.IsNullOrEmpty(GtinRelationships.GTIN.ToString()) ? string.Empty : GtinRelationships.GTIN.ToString() + " ";
                                }
                            }
                        }
                    }
                    else
                    {
                        lblGTIN.Text = string.Empty;
                        NotifierController.Warning("GTNI [UPC] no encontrado");
                    }
                }
                else
                {
                    Console.WriteLine($"Sin respuesta al obtener descripción larga y GTIN [{DateService.Today()}]", Color.Red);
                }

                /*if (!string.IsNullOrEmpty(lblItemDescription.Text))
                {
                    //♥ Consultar ITEM ♥
                    dynamic itemsV2 = await CommonService.OneItem(String.Format(EndPoints.ItemP1, lblItemNumber.Text, Constants.Plant1Id));

                    if (itemsV2 != null)
                    {
                        lblItemDescriptionEnglish.Text = string.IsNullOrEmpty(itemsV2.LongDescription.ToString()) ? 
                                                         TranslateService.Translate(lblItemDescription.Text) : itemsV2.LongDescription.ToString();
                    }
                    else
                    {
                        lblItemDescriptionEnglish.Text = TranslateService.Translate(lblItemDescription.Text);
                    }
                }
                else
                {
                    lblItemDescriptionEnglish.Text = string.Empty;
                }*/

                lblPlannedStartDate.Text = wo.PlannedStartDate.ToString();
                lblPlannedCompletionDate.Text = wo.PlannedCompletionDate.ToString();

                //Porcentaje adicional
                float additionalLabels = (Settings.Default.Aditional * int.Parse(lblOutputQuantity.Text)) / 100;
                lblAditional.Text = $"Total (+{Convert.ToInt32(Math.Round(additionalLabels))}):";
                lbLabelQuantity.Text = (float.Parse(lblOutputQuantity.Text) + additionalLabels).ToString();

                //Obtener última página de etiqueta impresa
                Task<string> tskLastPage = APIService.GetApexAsync(String.Format(EndPoints.LabelsPrinted, cmbWorkOrders.Text));
                string responseLP = await tskLastPage;

                if (!string.IsNullOrEmpty(responseLP))
                {
                    txtStartPage.Enabled = false;
                    dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(responseLP);
                    
                    lastPagePrinted = string.IsNullOrEmpty(responsePayload.LastPage.ToString()) ? 0 : int.Parse(responsePayload.LastPage.ToString());
                    if(lbLabelQuantity.Text.Equals(responsePayload.LastPage.ToString()))
                    {
                        txtStartPage.Text = responsePayload.LastPage.ToString();
                        if(!checkBoxReprint.Checked)
                            NotifierController.Warning("Se han impreso todas las etiquetas de esta orden");
                    }
                    else
                    {
                        txtStartPage.Text = checkBoxReprint.Checked ? responsePayload.LastPage.ToString() : (lastPagePrinted + 1).ToString();
                    }  
                }
                else
                {
                    txtStartPage.Text = "1";
                    txtStartPage.Enabled = true;
                }

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
                    _akaCustomer = "DEFAULT";
                }
                else
                {
                    lblAkaOrder.Text = flexPV;

                    //♥ Consultar OM ♥
                    dynamic om = await CommonService.OneItem(String.Format(EndPoints.SalesOrders, lblAkaOrder.Text, Constants.BusinessUnitId));
                    if (om != null)
                    {
                        lblAkaCustomer.Text = om.BuyingPartyNumber.ToString();//BuyingPartyNumber BuyingPartyName
                        _CustomerPONumber = om.CustomerPONumber.ToString();
                        _akaCustomer = lblAkaCustomer.Text;

                        //♥ Consultar TradingPartnerItemRelationships ♥
                        dynamic aka = await CommonService.OneItem(String.Format(EndPoints.TradingPartnerItemRelationships, lblItemNumber.Text, lblAkaCustomer.Text));
                        if (aka != null)
                        {
                            lblAkaItem.Text = aka.TradingPartnerItemNumber.ToString();
                            lblAkaDescription.Text = aka.TradingPartnerItemDescription.ToString();
                            _tradingPartnerName = aka.TradingPartnerName.ToString();
                        }
                        else
                        {
                            lblAkaItem.Text = string.Empty;
                            lblAkaDescription.Text = string.Empty;
                            _tradingPartnerName = string.Empty;
                            Console.WriteLine($"Orden sin datos AKA [{DateService.Today()}]", Color.Black);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Pedido de venta no encontrado [{DateService.Today()}]", Color.Red);
                    }
                }

                //Consultar template etiqueta en APEX 
                dynamic labelApex = await LabelService.LabelInfo(Constants.Plant1Id, _akaCustomer, lblItemNumber.Text);
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

                txtTotalPrint.Enabled = string.IsNullOrEmpty(lblOutputQuantity.Text) ? false : true;

                //Validar activación de boton de impresion
                if (!string.IsNullOrEmpty(cmbWorkOrders.Text) && !string.IsNullOrEmpty(lblResourceName.Text) && !string.IsNullOrEmpty(lblLabelName.Text))
                {
                    if (checkBoxReprint.Checked) //REIMPRIMIR
                    {
                        txtTotalPrint.Enabled = false; // Simpre bloquear en reimpresion
                        if (lastPagePrinted > 0)
                        {
                            txtBoxStart.Enabled = true;
                            txtBoxEnd.Enabled = true;
                            btnPrint.Enabled = true;
                        }
                        else
                        {
                            txtBoxStart.Enabled = false;
                            txtBoxEnd.Enabled = false;
                            btnPrint.Enabled = false;
                        }
                    }
                    else //IMPRIMIR
                    {
                        txtBoxStart.Enabled = false;
                        txtBoxEnd.Enabled = false; //Siempre bloquerar en impresion

                        if(lastPagePrinted < int.Parse(lbLabelQuantity.Text))
                        {
                            txtTotalPrint.Enabled = true;
                            btnPrint.Enabled = true;
                        }
                        else
                        {
                            txtTotalPrint.Enabled = false;
                            btnPrint.Enabled = false;
                        }
                    }
                }
                else
                {
                    txtTotalPrint.Enabled = false;
                    txtBoxStart.Enabled = false;
                    txtBoxEnd.Enabled = false;

                    btnPrint.Enabled = false;
                }

                
            }
            catch (Exception ex)
            {
                pop.Close();
                cmbWorkCenters.Enabled = true;
                cmbWorkOrders.Enabled = true;
                MessageBox.Show("Error. " + ex.Message, "Error [WorkOrderSelected]", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            pop.Close();
            cmbWorkCenters.Enabled = true;
            cmbWorkOrders.Enabled = true;
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
            lblGTIN.Text = string.Empty;
            //Resource Section
            lblResourceCode.Text = string.Empty;
            lblResourceName.Text = string.Empty;
            //Planned Dates
            lblPlannedStartDate.Text = string.Empty;
            lblPlannedCompletionDate.Text = string.Empty;
            //AKA Section
            _akaCustomer = "DEFAULT";
            lblAkaOrder.Text = string.Empty;
            lblAkaItem.Text = string.Empty;
            lblAkaCustomer.Text = string.Empty;
            lblAkaDescription.Text = string.Empty;
            _CustomerPONumber = string.Empty;
            _tradingPartnerName = string.Empty;
            //Reprint Section
            //-------------------groupBoxReprint.Visible = false;
            txtBoxStart.Text = string.Empty;
            txtBoxStart.BackColor = Color.White;
            txtBoxEnd.Text = string.Empty;
            txtBoxEnd.BackColor = Color.White;
            lblStatus.Text = string.Empty;
            //Label Preview Section
            lblLabelName.Text = string.Empty;
            lbLabelQuantity.Text = string.Empty;
            lblAditional.Text = "Total (+0):";
            txtStartPage.Text = string.Empty;
            txtStartPage.BackColor = Color.White;
            txtStartPage.Enabled = false;
            txtTotalPrint.Text = string.Empty;
            txtTotalPrint.BackColor = Color.White;
            txtTotalPrint.Enabled = false;
            picLabel.Image = null;
            btnPrint.Enabled = false;
            lblStatusPrint.Text = string.Empty;
        }

        private async void btnPrint_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbWorkOrders.Text))
            {
                NotifierController.Warning("Seleccione orden de trabajo");
            }
            else
            {
                // * Reimpresión activada *
                if (checkBoxReprint.Checked)
                {
                    if (!string.IsNullOrEmpty(txtBoxStart.Text) && !string.IsNullOrEmpty(txtBoxEnd.Text))
                    {
                        if (int.TryParse(txtBoxStart.Text, out _) && int.TryParse(txtBoxEnd.Text, out _))
                        {
                            if (int.Parse(txtBoxStart.Text) > 0 && int.Parse(txtBoxEnd.Text) <= int.Parse(txtStartPage.Text))
                            {
                                if (int.Parse(txtBoxEnd.Text) >= int.Parse(txtBoxStart.Text))
                                {
                                    cmbWorkCenters.Enabled = false;
                                    cmbWorkOrders.Enabled = false;
                                    btnReprint.Enabled = false;
                                    btnSettings.Enabled = false;
                                    btnCloseReprint.Enabled = false;
                                    DisableCloseButton();

                                    pbWaitPrint.Visible = true;
                                    lblStatusPrint.Font = new Font(Label.DefaultFont, FontStyle.Bold);
                                    lblStatusPrint.Text = "Imprimiendo...";
                                    btnPrint.Enabled = false;
                                    _isPrint = true;

                                    txtBoxStart.Enabled = false;
                                    txtBoxEnd.Enabled = false;

                                    if (await LabelService.PrintP1(int.Parse(txtBoxStart.Text), int.Parse(txtBoxEnd.Text)))
                                    {
                                        //RegisterPrintRecordsApex("reprint");
                                        CheckInternetToRegister("record");
                                        groupBoxReprint.Visible = false;
                                        CleanAll();

                                        checkBoxReprint.Checked = false;
                                        btnPrint.Text = "IMPRIMIR";
                                    }
                                    else
                                    {
                                        txtBoxStart.Enabled = true;
                                        txtBoxEnd.Enabled = true;
                                        btnPrint.Enabled = true;

                                        lblStatus.Text = "Error de impresión";
                                    }

                                    cmbWorkCenters.Enabled = true;
                                    cmbWorkOrders.Enabled = true;
                                    btnReprint.Enabled = true;
                                    btnSettings.Enabled = true;
                                    btnCloseReprint.Enabled = true;
                                    EnableCloseButton();

                                    pbWaitPrint.Visible = false;
                                    lblStatusPrint.Font = new Font(Label.DefaultFont, FontStyle.Regular);
                                    lblStatusPrint.Text = string.Empty;
                                    //btnPrint.Enabled = true;
                                    _isPrint = false;
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
                else // * Impresion normal *
                {
                    if (!string.IsNullOrEmpty(txtStartPage.Text) && !string.IsNullOrEmpty(txtTotalPrint.Text))
                    {
                        if (int.TryParse(txtStartPage.Text, out _) && int.TryParse(txtTotalPrint.Text, out _))
                        {
                            if (int.Parse(txtStartPage.Text) > 0 && int.Parse(txtTotalPrint.Text) <= int.Parse(lbLabelQuantity.Text))
                            {
                                if (int.Parse(txtTotalPrint.Text) >= int.Parse(txtStartPage.Text))
                                {
                                    cmbWorkCenters.Enabled = false;
                                    cmbWorkOrders.Enabled = false;
                                    txtTotalPrint.Enabled = false;
                                    btnReprint.Enabled = false;
                                    btnSettings.Enabled = false;
                                    DisableCloseButton();

                                    pbWaitPrint.Visible = true;
                                    lblStatusPrint.Font = new Font(Label.DefaultFont, FontStyle.Bold);
                                    lblStatusPrint.Text = "Imprimiendo...";
                                    btnPrint.Enabled = false;
                                    _isPrint = true;

                                    if (await LabelService.PrintP1(int.Parse(txtStartPage.Text), int.Parse(txtTotalPrint.Text)))
                                    {
                                        await CheckAndRegister();
                                    }
                                    else
                                    {
                                        if (Constants.LastPrint > 0)
                                        {
                                            NotifierController.Error($"Error al imprimir, última etiqueta {Constants.LastPrint}");
                                            txtTotalPrint.Text = Constants.LastPrint.ToString();

                                            await CheckAndRegister();
                                        }
                                        else
                                        {
                                            btnPrint.Enabled = true;
                                            NotifierController.Error("Error al imprimir");
                                        }
                                    }

                                    
                                    cmbWorkCenters.Enabled = true;
                                    cmbWorkOrders.Enabled = true;
                                    txtTotalPrint.Enabled = false;
                                    btnReprint.Enabled = true;
                                    btnSettings.Enabled = true;
                                    EnableCloseButton();

                                    pbWaitPrint.Visible = false;
                                    lblStatusPrint.Font = new Font(Label.DefaultFont, FontStyle.Regular);
                                    lblStatusPrint.Text = string.Empty;
                                    //btnPrint.Enabled = true;
                                    _isPrint = false;

                                    Constants.LastPrint = 0;
                                }
                                else
                                {
                                    lblStatusPrint.Text = "Cantidad final no puede ser menor a la inicial";
                                }
                            }
                            else
                            {
                                lblStatusPrint.Text = "Cantidades fuera del rango permitido";
                            }
                        }
                        else
                        {
                            lblStatusPrint.Text = "Ingrese únicamente números enteros";
                        }
                    }
                    else
                    {
                        lblStatusPrint.Text = "Llene todos los campos";
                    }
                }
                
            }
        }

        private async Task CheckAndRegister()
        {
            List<string> ordersPrinted = FileController.ContentFile(Constants.OrdersPrintedP1);

            if (!FileController.IsOrderPrinted(ordersPrinted, cmbWorkOrders.Text))
            {
                //Guardar orden en archivo y DB
                await FileController.Write(cmbWorkOrders.SelectedItem.ToString(), Constants.OrdersPrintedP1);
                CheckInternetToRegister("new");
            }
            {
                CheckInternetToRegister("update");
            }

            await Task.Delay(500);
            CheckInternetToRegister("record");

            CleanAll();
        }

        private void CheckInternetToRegister(string method)
        {
            if (AppController.CheckInternetConnection())
            {
                switch(method)
                {
                    case "new":
                        RegisterPrintApex();
                        break;
                    case "update":
                        UpdatePrintApex();
                        break;
                    case "record":
                        RegisterPrintRecordsApex(btnPrint.Text);
                        break;
                   default:
                        break;
                }    
            }
            else
            {
                NotifierController.Warning("Sin conexión a internet");
                DialogResult result = MessageBox.Show("Verificar la conexión internet", "Sin Internet", MessageBoxButtons.OK, MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    CheckInternetToRegister(method);
                }
                else
                {
                    CheckInternetToRegister(method);
                }
            }
        }

        public static void SetLabelStatusPrint(string text)
        {
            if (instance != null && instance.lblStatusPrint != null)
            {
                instance.lblStatusPrint.Text = text;
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

                label.AKAITEM = string.IsNullOrEmpty(lblAkaItem.Text) ? "NE" : lblAkaItem.Text;
                label.AKADESCRIPTION = string.IsNullOrEmpty(lblAkaDescription.Text) ? "NE" : lblAkaDescription.Text;
                label.LEGALENTITY = string.IsNullOrEmpty(_tradingPartnerName) ? "NE" : _tradingPartnerName;
                label.PURCHASEORDER = string.IsNullOrEmpty(_CustomerPONumber) ? " " : _CustomerPONumber;
                label.PONUM = string.IsNullOrEmpty(_CustomerPONumber) ? " "  : _CustomerPONumber;

                //label.UPCA = string.IsNullOrEmpty(itemId) ? $"{Constants.UPCPrefix}0000" : $"{Constants.UPCPrefix}{itemId.Substring(itemId.Length - 4)}";
                label.UPCA = string.IsNullOrEmpty(lblGTIN.Text) ? "00000000000000" : lblGTIN.Text;
                label.EQU = string.IsNullOrEmpty(lblResourceName.Text) ? " ": lblResourceName.Text;
                label.DATE = DateService.Now();
                label.BOX = "1";

                Constants.LabelJson = JsonConvert.SerializeObject(label, Formatting.Indented);

                picLabel.Image = Image.FromStream(await LabelService.UpdateLabelLabelary(int.Parse(txtStartPage.Text), "BOX"));
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmSettingsLogP1 frmSettingsLogP1 = new frmSettingsLogP1();
            frmSettingsLogP1.StartPosition = FormStartPosition.CenterParent;
            frmSettingsLogP1.ShowDialog();
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
                if(int.Parse(txtBoxStart.Text) > int.Parse(txtStartPage.Text))
                {
                    txtBoxStart.BackColor = Color.LightSalmon;
                    lblStatus.Text = $"Valor no puede ser mayor a la cantidad de etiquetas impresas ({txtStartPage.Text})";
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
                if (int.Parse(txtBoxEnd.Text) > int.Parse(txtStartPage.Text))
                {
                    txtBoxEnd.BackColor = Color.LightSalmon;
                    lblStatus.Text = $"Valor no puede ser mayor a la cantidad de etiquetas impresas ({txtStartPage.Text})";
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

        #region APEX
        private async void RegisterPrintApex()
        {
            dynamic jsonPrint = JObject.Parse(Payloads.labelsPrinted);
            jsonPrint.DateMark = DateService.EpochTime();
            jsonPrint.WorkOrder = cmbWorkOrders.Text;
            jsonPrint.UserId = Constants.UserId;
            jsonPrint.LastPage = int.Parse(txtTotalPrint.Text);
            jsonPrint.LimitPrint = int.Parse(lbLabelQuantity.Text);

            string jsonSerialized = JsonConvert.SerializeObject(jsonPrint, Formatting.Indented);

            Task<string> postPrint = APIService.PostApexAsync(String.Format(EndPoints.LabelsPrinted, cmbWorkOrders.Text), jsonSerialized);
            string response = await postPrint;

            if (!string.IsNullOrEmpty(response))
            {
                dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(response);
                Console.WriteLine($"{responsePayload.Message} [{DateService.Today()}]", Color.Green);
            }
            else
            {
                Console.WriteLine($"Sin respuesta al registrar impresión [{DateService.Today()}]", Color.Red);
            }
        }

        private async void UpdatePrintApex()
        {
            dynamic jsonPrint = JObject.Parse(Payloads.labelsPrintedUpdate);
            jsonPrint.UserId = Constants.UserId;
            jsonPrint.LastPage = int.Parse(txtTotalPrint.Text);

            string jsonSerialized = JsonConvert.SerializeObject(jsonPrint, Formatting.Indented);

            Task<string> putPrint = APIService.PutApexAsync(String.Format(EndPoints.LabelsPrinted, cmbWorkOrders.Text), jsonSerialized);
            string response = await putPrint;

            if (!string.IsNullOrEmpty(response))
            {
                dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(response);
                Console.WriteLine($"{responsePayload.Message} [{DateService.Today()}]", Color.Green);
            }
            else
            {
                Console.WriteLine($"Sin respuesta al actualizar impresión [{DateService.Today()}]", Color.Red);
            }
        }

        private async void RegisterPrintRecordsApex(string operation)
        {
            dynamic jsonPR = JObject.Parse(Payloads.labelsRecords);

            jsonPR.DateMark = DateService.EpochTime();
            jsonPR.WorkOrder = cmbWorkOrders.Text;
            jsonPR.UserId = Constants.UserId;
            jsonPR.StartPage = operation.Equals("IMPRIMIR") ? int.Parse(txtStartPage.Text) : int.Parse(txtBoxStart.Text);
            jsonPR.EndPage = operation.Equals("IMPRIMIR") ? int.Parse(txtTotalPrint.Text) : int.Parse(txtBoxEnd.Text);
            jsonPR.Operation = operation.Equals("IMPRIMIR") ? "print" : "reprint";

            string jsonSerialized = JsonConvert.SerializeObject(jsonPR, Formatting.Indented);

            Task<string> postPR = APIService.PostApexAsync(EndPoints.LabelsRecords, jsonSerialized);
            string response = await postPR;

            if (!string.IsNullOrEmpty(response))
            {
                dynamic responsePayload = JsonConvert.DeserializeObject<dynamic>(response);
                Console.WriteLine($"{responsePayload.Message} [{DateService.Today()}]", Color.Green);
            }
            else
            {
                Console.WriteLine($"Sin respuesta al registrar impresión [{DateService.Today()}]", Color.Red);
            }
        }
        #endregion

        private void btnCloseReprint_Click(object sender, EventArgs e)
        {
            CleanAll();
            txtBoxStart.Enabled = false;
            txtBoxEnd.Enabled = false;

            checkBoxReprint.Checked = false;
            groupBoxReprint.Visible = false;
            btnPrint.Text = "IMPRIMIR";
        }

        private void frmLabelP1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (!string.IsNullOrEmpty(cmbWorkOrders.Text))
                {
                    DialogResult result = MessageBox.Show("¿Seguro que desea cerra la aplicación?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.No)
                    {
                        e.Cancel = true;
                    }
                    else if (result == DialogResult.Yes)
                    {
                        if (_isPrint)
                        {
                            e.Cancel = true;
                            NotifierController.Warning("No se permite cerrar la aplicación durante una impresión");
                        }
                        else
                        {
                            e.Cancel = false;
                        }
                    }
                }
            }
        }

        private void frmLabelP1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void txtTotalPrint_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtTotalPrint.Text, out _))
            {
                if (int.Parse(txtTotalPrint.Text) > int.Parse(lbLabelQuantity.Text))
                {
                    txtTotalPrint.BackColor = Color.LightSalmon;
                    lblStatusPrint.Text = "Valor no puede ser mayor a la cantidad permitida";
                }
                else
                {
                    txtTotalPrint.BackColor = Color.White;
                    lblStatusPrint.Text = string.Empty;
                }
            }
            else
            {
                txtTotalPrint.BackColor = Color.LightSalmon;
                lblStatusPrint.Text = "Ingrese únicamente números enteros";
            }
        }

        private void txtStartPage_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtStartPage.Text, out _))
            {
                if (int.Parse(txtStartPage.Text) > int.Parse(lbLabelQuantity.Text))
                {
                    txtStartPage.BackColor = Color.LightSalmon;
                    lblStatusPrint.Text = "Valor no puede ser mayor a la cantidad permitida";
                }
                else
                {
                    txtStartPage.BackColor = Color.White;
                    lblStatusPrint.Text = string.Empty;
                }
            }
            else
            {
                txtStartPage.BackColor = Color.LightSalmon;
                lblStatusPrint.Text = "Ingrese únicamente números enteros";
            }
        }

        private void frmLabelP1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnPrint.PerformClick();
            }
        }

        private void DisableCloseButton()
        {
            IntPtr hMenu = GetSystemMenu(this.Handle, false);
            EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED | MF_DISABLED);
        }

        private void EnableCloseButton()
        {
            IntPtr hMenu = GetSystemMenu(this.Handle, false);
            EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_ENABLED);
        }
    }
}
