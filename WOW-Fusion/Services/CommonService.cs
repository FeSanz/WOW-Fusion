using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOW_Fusion.Controllers;
using WOW_Fusion.Models;

namespace WOW_Fusion.Services
{
    internal class CommonService
    {
        public static async Task<dynamic> OneItem(string endpoint)
        {
            try
            {
                Task<string> tskItem = APIService.GetRequestAsync(endpoint);
                string response = await tskItem;
                if (string.IsNullOrEmpty(response))
                {
                    return null;
                }
                else
                {
                    JObject objResponse = JObject.Parse(response);

                    if ((int)objResponse["count"] == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return objResponse["items"][0];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error [OneItem]", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static async Task<JObject> ProductionResourcesMachines(string endpoint)
        {
            try
            {
                Task<string> tskresourcesMachines = APIService.GetRequestAsync(endpoint);
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

        public static async Task<List<string>> WODiscreteByWorkCenter(string organizationId, string workCenterId)
        {
            try
            {
                Task<string> tskWorkOrders = APIService.GetRequestAsync(String.Format(EndPoints.WODiscreteList, organizationId, workCenterId));
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
                return null;
            }
        }

        public static async Task<List<WorkOrderShedule>> WODiscreteSchedule(string organizationId, string workCenterId)
        {
            try
            {
                DateTimeOffset now = DateTimeOffset.Now;
                Task<string> tskWorkOrders = APIService.GetRequestAsync(String.Format(EndPoints.WODiscreteList, organizationId, workCenterId, now.ToString("yyyy-MM-dd HH:mm:ss")));
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
                        List<dynamic> orders = new List<dynamic>(workOrders["items"]);

                        List<WorkOrderShedule> schedule = new List<WorkOrderShedule>();

                        foreach (var item in orders)
                        {
                            schedule.Add(new WorkOrderShedule 
                            { 
                                WorkOrderNumber = item.WorkOrderNumber.ToString(), 
                                PlannedStartDate = DateTime.Parse(item.PlannedStartDate.ToString()),
                                PlannedCompletionDate = DateTime.Parse(item.PlannedCompletionDate.ToString())
                            });
                        }
                        schedule.Sort((a, b) => a.PlannedStartDate.CompareTo(b.PlannedStartDate));

                        foreach (var item in schedule)
                        {
                            var i = Array.IndexOf(schedule.ToArray(), item);
                            Console.WriteLine((i+1) + ". " + item.WorkOrderNumber.ToString() +" -> "+ item.PlannedStartDate.ToString() + " - " + item.PlannedCompletionDate.ToString());
                        }

                        return schedule;
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
                return null;
            }
        }

        public static async Task<List<string>> WOProcessByWorkCenter(string organizationId, string workCenterId)
        {
            try
            {
                Task<string> tskWorkOrders = APIService.GetRequestAsync(String.Format(EndPoints.WOProcessList, organizationId, workCenterId));
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
                return null;
            }
        }

        public static async Task<JObject> LabelTamplate(string label)
        {
            try
            {
                Task<string> tskLabels = APIService.GetApexAsync(String.Format(EndPoints.LabelTamplate, label));
                string response = await tskLabels;

                return JObject.Parse(response);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message, "Error [Label]", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

    }
}
