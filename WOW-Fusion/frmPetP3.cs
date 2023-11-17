using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WOW_Fusion
{
    public partial class frmPetP3 : Form
    {
        int TurnoPesaje = 0;

        int MockBruto = 50;
        int MockTara = 5;
        public frmPetP3()
        {
            InitializeComponent();
        }

        private void btnGetWeight_Click(object sender, EventArgs e)
        {
            if (TurnoPesaje == 0)
            {
                txtBoxWeight.Text = MockTara.ToString();
                lblTara.Text = MockTara.ToString();
                TurnoPesaje++;
            }
            else if (TurnoPesaje > 0)
            {
                lblFechaEntrada.Text = DateTime.Now.ToString();
                txtBoxWeight.Text = MockBruto.ToString();
                lblNeto.Text = (Convert.ToInt32(lblTara.Text) + 50).ToString();
                lblTara.Text = MockTara.ToString();
                lblBruto.Text = MockBruto.ToString();
                TurnoPesaje = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtBoxWeight.Text = 0.ToString();
            lblNeto.Text = 0.ToString();
            lblTara.Text = 0.ToString();
            lblBruto.Text = 0.ToString();
            TurnoPesaje = 0;
        }

        public async Task<List<string>> ParseOrders()
        {
            try
            {
                string response = await StaticApiService.GetRequestAsync("/fscmRestApi/resources/11.13.18.05/purchaseOrdersForReceiving");
                if (response != null)
                {
                    List<string> ordersList = new List<string>();
                    var data = JArray.Parse(response);
                    foreach (var item in data)
                    {
                        // Aquí extraes la información específica que necesitas de cada orden
                        string orderInfo = item["OrderField"].ToString(); // Reemplaza "OrderField" con el campo correspondiente
                        ordersList.Add(orderInfo);
                    }
                    return ordersList;

                }
                return new List<string> { "Item1", "Item2", "Item3", "Item4" };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return new List<string> { "Item1", "Item2", "Item3", "Item4" };
            }
        }

        private void Pa(object sender, EventArgs e)
        {
            ParseOrders();
        }
    }
}
