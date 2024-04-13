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
    internal class Plant1Service
    {
        public static async Task<List<string>> WorkOrdersListItemEval(string organizationId, string workCenterId)
        {
            try
            {
                Task<string> tskWorkOrdersList = APIService.GetRequestAsync(String.Format(EndPoints.WorkOrdersItemList, organizationId, workCenterId));
                string response = await tskWorkOrdersList;
                if (string.IsNullOrEmpty(response))
                {
                    return null;
                }
                else
                {
                    JObject workOrders = JObject.Parse(response);

                    if ((int)workOrders["count"] >= 1)
                    {
                        dynamic items = workOrders["items"];
                        List<string> workOrderNumbers = new List<string>();

                        foreach (var item in items)
                        {
                            /*string itemNumber = item["ItemNumber"].ToString();
                            if (itemNumber.Substring(itemNumber.Length - 2).Equals("01"))//Terminacion 01 producto empaquetado (CAJAS)
                            {
                                workOrderNumbers.Add(item["WorkOrderNumber"].ToString());
                            }*/
                            workOrderNumbers.Add(item["WorkOrderNumber"].ToString());
                        }
                        return workOrderNumbers;
                    }
                    else
                    {
                        NotifierController.Warning("Sin ordenes de trabajo");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "[Error] Lista de ordenes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

    }
}
