using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Controllers;
using WOW_Fusion.Models;
using WOW_Fusion.Properties;
using WOW_Fusion.Services;

namespace WOW_Fusion
{
    public partial class frmPetP3 : Form
    {
        private string ManagementBusinessUnitId = string.Empty;
        public frmPetP3()
        {
            InitializeComponent();
            InitializeFusionData();
        }

        private async void InitializeFusionData()
        {
            //Obtener datos de Organizacion
            dynamic org = await CommonService.OneItem(String.Format(EndPoints.InventoryOrganizations, Constants.Plant3Id));

            if (org == null)
            {
                NotifierController.Error("Sin organización, la aplicación no podrá funcionar");
                return;
            }
            else
            {
                lblManagementBUName.Text = org["ManagementBusinessUnitName"].ToString();
                lblOrganizationCode.Text = org["OrganizationCode"].ToString();
                lblLocationCode.Text = org["LocationCode"].ToString();
                
                ManagementBusinessUnitId = org["ManagementBusinessUnitId"].ToString();
            }
        }

        private void frmPetP3_Load(object sender, EventArgs e)
        {

        }

        private async void cmbPO_DropDown(object sender, EventArgs e)
        {
            cmbPO.Items.Clear();
            picBoxWaitPO.Visible = true;

            List<string> PONumbers = await Plant3Service.POForReceiving(ManagementBusinessUnitId); //Obtener OC
            picBoxWaitPO.Visible = false;

            if (PONumbers == null) return;

            foreach (var item in PONumbers)
            {
                cmbPO.Items.Add(item.ToString());
            }
        }

        private async void cmbPO_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Task<string> tskPurchaseOrdersData = APIService.GetRequestAsync(String.Format(EndPoints.PurchaseOrder, cmbPO.SelectedItem.ToString()));
                string response = await tskPurchaseOrdersData;
                if (string.IsNullOrEmpty(response)) { return; }

                JObject objPurchaseOrder = JObject.Parse(response);
                if ((int)objPurchaseOrder["count"] == 0)
                {
                    NotifierController.Warning("Datos de orden no encotrada");
                    return;
                }
                dynamic po = objPurchaseOrder["items"][0]; //Objeto WORKORDER

                lblProcurementBU.Text = po["ProcurementBU"].ToString();
                lblSupplier.Text = po["Supplier"].ToString();
                lblSupplierSite.Text = po["SupplierSite"].ToString();
                lblSoldToLegalEntity.Text = po["SoldToLegalEntity"].ToString();

                dynamic poLines = objPurchaseOrder["items"][0]["lines"]["items"]; //Objeto LINEAS
                int countLines = (int)objPurchaseOrder["items"][0]["lines"]["count"];
                int nullItemNumber = 0;

                dgPO.Rows.Clear();
                dgPO.Refresh();



                for (int i = 0; i < countLines; i++)
                {
                    if (string.IsNullOrEmpty(poLines[i]["Item"].ToString()))
                    {
                        nullItemNumber++;
                    }
                    else
                    {
                        if (poLines[i]["LineType"].ToString() != "Servicios")
                        {
                            dgPO.Rows.Add(poLines[i]["LineNumber"], poLines[i]["Quantity"], poLines[i]["UOMCode"], poLines[i]["Item"], poLines[i]["Description"]);
                        }
                    }
                }

                if (nullItemNumber > 0) NotifierController.Warning("Prudutos no inventariables no se muestran en la lista");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error[PO Selected]", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }  
        
        private void btnGetWeight_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

    }
}
