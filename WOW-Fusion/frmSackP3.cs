using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WOW_Fusion
{
    public partial class frmSackP3 : Form
    {
        public frmSackP3()
        {
            InitializeComponent();
        }

        private void dgWeights_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string json = null; 
            StaticApiService.PostRequest(Constants.FusionUrl+Constants.ObtenerTodasLasOrdenes+"&size=5;proveedor=", json);
        }

        private async void cmbWorkOrders_DropDown(object sender, EventArgs e)
        {
            if (cmbWorkOrders != null)
            {
                cmbWorkOrders.Items.Clear();
                List<string> workOrdersList = new List<string> { "Seleccione orden" };
                workOrdersList.AddRange(await StaticApiService.RequestWorkOrdersList(Constants.organizationId));
                foreach (var item in workOrdersList)
                {
                    cmbWorkOrders.Items.Add(item);
                }
            }
        }

        private void cmbWorkOrders_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
