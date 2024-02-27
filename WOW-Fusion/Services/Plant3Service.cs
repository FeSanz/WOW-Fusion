using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Controllers;
using WOW_Fusion.Models;

namespace WOW_Fusion.Services
{
    internal class Plant3Service
    {
        public static async Task<List<string>> POForReceiving(string businessUnitId)
        {
            try
            {
                Task<string> tskPurchaseOrders = APIService.GetRequestAsync(String.Format(EndPointsInventory.PurchaseOrdersList, businessUnitId));
                string response = await tskPurchaseOrders;
                if (string.IsNullOrEmpty(response))
                {
                    return null;
                }
                else
                {
                    JObject purchaseOrders = JObject.Parse(response);

                    if ((int)purchaseOrders["count"] >= 0)
                    {
                        List<string> purchaseOrderNumbers = new List<string>();
                        dynamic items = purchaseOrders["items"];

                        foreach (var item in items)
                        {
                            purchaseOrderNumbers.Add(item["OrderNumber"].ToString());
                        }
                        return purchaseOrderNumbers;
                    }
                    else
                    {
                        NotifierController.Warning("Sin ordenes de compra");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error [POList]", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
