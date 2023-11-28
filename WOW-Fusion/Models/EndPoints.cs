using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOW_Fusion.Models
{
    internal static class EndPoints
    {
        public static string InventoryOrganizations = Constants.FusionUrl + "/inventoryOrganizations?" + Constants.ParamsGet +
                                                    "&fields=OrganizationId,OrganizationCode,OrganizationName,LocationCode,ManagementBusinessUnitName" +
                                                    "&q=OrganizationId={0}";

        public static string WorkCenters = Constants.FusionUrl + "/workCenters?" + Constants.ParamsGet +
                                                    "&fields=WorkCenterId,WorkCenterName,WorkAreaId,WorkAreaName" +
                                                    "&q=OrganizationId={0}";
        public static string WorkOrdersList = Constants.FusionUrl + "/workOrders?" + Constants.ParamsGet +
                                                    "&fields=WorkOrderNumber" +
                                                    "&q=OrganizationId={0} and WorkOrderStatusCode='ORA_RELEASED' and WorkOrderActiveOperation.WorkCenterId={1}";

        public static string WorkOrderDetail = Constants.FusionUrl + "/workOrders?" + Constants.ParamsGet +
                                                    "&expand=WorkOrderResource.WorkOrderOperationResourceInstance" +
                                                    "&fields=WorkOrderId,ItemNumber,Description,UOMCode,PlannedStartQuantity,PlannedStartDate,PlannedCompletionDate;" +
                                                    "WorkOrderResource:ResourceId,ResourceCode,ResourceDescription;" +
                                                    "WorkOrderResource.WorkOrderOperationResourceInstance:" +
                                                    "EquipmentInstanceId,EquipmentInstanceCode,EquipmentInstanceName" +
                                                    "&q=WorkOrderNumber='{0}'";

        //Endpoints Planta 1
        public static string WorkOrdersItemList = Constants.FusionUrl + "/workOrders?" + Constants.ParamsGet +
                                            "&fields=WorkOrderNumber,ItemNumber" +
                                            "&q=OrganizationId={0} and WorkOrderStatusCode='ORA_RELEASED' and WorkOrderActiveOperation.WorkCenterId={1}";

        public static string ProductionResourcesP1 = Constants.FusionUrl + "/productionResources?" + Constants.ParamsGet +
                                                        "&fields=ResourceId" +
                                                        "&q=OrganizationId={0} and ResourceType='EQUIPMENT' and ResourceClassCode='EQU'";
        //Endpoints Planta 2
        public static string ProductionResourcesP2 = Constants.FusionUrl + "/productionResources?" + Constants.ParamsGet +
                                                   "&fields=ResourceId" +
                                                   "&q=OrganizationId={0} and ResourceType='EQUIPMENT' and ResourceCode like 'MF-LAM%' or ResourceCode like 'MF-C01%'";

        
    }
}
