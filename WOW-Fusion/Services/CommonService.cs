using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Controllers;
using WOW_Fusion.Models;

namespace WOW_Fusion.Services
{
    internal class CommonService
    {
        public static async Task<List<string>> Organization(string organizationId)
        {
            try
            {
                Task<string> tskOrganizationData = APIService.GetRequestAsync(String.Format(EndPoints.InventoryOrganizations, organizationId));
                string response = await tskOrganizationData;

                if (string.IsNullOrEmpty(response))
                {
                    AppController.Exit("Sin organización, la aplicación se cerrará");
                    return null;
                }

                JObject organization = JObject.Parse(response);
                List<string> organizationData = new List<string>();
                
                if ((int)organization["count"] > 0)
                {
                    dynamic items = organization["items"][0];

                    organizationData.Add(items["OrganizationCode"].ToString());
                    organizationData.Add(items["OrganizationName"].ToString());
                    organizationData.Add(items["LocationCode"].ToString());
                    organizationData.Add(items["ManagementBusinessUnitName"].ToString());
                    return organizationData;
                }
                else
                {
                    AppController.Exit("Sin organización, la aplicación se cerrará");
                    return null;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error [Organization]", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static async Task<JObject> ProductionResourcesMachines(string organizationId)
        {
            try
            {
                Task<string> tskresourcesMachines = APIService.GetRequestAsync(String.Format(EndPoints.ProductionResourcesP1, organizationId));
                string response = await tskresourcesMachines;

                if (string.IsNullOrEmpty(response))
                {
                    AppController.Exit("Sin recursos de producción, la aplicacion se cerrará");
                    return null;
                }

                JObject productionResoucesMachines = JObject.Parse(response);

                if ((int)productionResoucesMachines["count"] == 0)
                {
                    AppController.Exit("Sin recursos de producción, la aplicacion se cerrará");
                    return null;
                }
                else
                {
                    return productionResoucesMachines;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error [ProductionResources]", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static async Task<JObject> WorkCenters(string organizationId)
        {
            try
            {
                Task<string> tskWorkCenters = APIService.GetRequestAsync(String.Format(EndPoints.WorkCenters, organizationId));
                string response = await tskWorkCenters;
                if (string.IsNullOrEmpty(response))
                {
                    return null;
                }
                else
                {
                    JObject workCenters = JObject.Parse(response);

                    if ((int)workCenters["count"] == 0)
                    {
                        NotifierController.Warning("Sin centros de trabajo");
                        return null;
                    }
                    else
                    {
                        return workCenters;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error [WorkCenters]", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static async Task<List<string>> WorkOrdersByWorkCenter(string organizationId, string workCenterId)
        {
            try
            {
                Task<string> tskWorkOrders = APIService.GetRequestAsync(String.Format(EndPoints.WorkCenters, organizationId));
                string response = await tskWorkOrders;
                if (string.IsNullOrEmpty(response))
                {
                    return null;
                }
                else
                {
                    JObject workOrders = JObject.Parse(response);

                    if ((int)workOrders["count"] >= 0)
                    {
                        List<string> workOrderNumbers = new List<string>();
                        dynamic items = workOrders["items"];

                        foreach (var item in items)
                        {
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
                MessageBox.Show("Error. " + ex.Message, "Error [WorkOrdersList]", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null; ;
            }
        }
    }
}
