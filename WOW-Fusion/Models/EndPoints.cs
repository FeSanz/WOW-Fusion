using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOW_Fusion.Properties;

namespace WOW_Fusion.Models
{
    internal static class EndPoints
    {
        public static string InventoryOrganizations = Settings.Default.FusionUrl + "/inventoryOrganizations?" + Constants.ParamsGet +
                                                    "&fields=OrganizationId,OrganizationCode,OrganizationName,LocationCode,ManagementBusinessUnitName" +
                                                    "&q=OrganizationId={0}";

        public static string WorkCenters = Settings.Default.FusionUrl + "/workCenters?" + Constants.ParamsGet +
                                                    "&fields=WorkCenterId,WorkCenterName,WorkAreaId,WorkAreaName" +
                                                    "&q=OrganizationId={0}";

        public static string WorkCentersById = Settings.Default.FusionUrl + "/workCenters?" + Constants.ParamsGet +
                                                    "&fields=WorkCenterName,WorkCenterDescription,WorkAreaId,WorkAreaName" +
                                                    "&q=OrganizationId={0} and WorkCenterId={1}";

        public static string ShiftByWorkCenter = Settings.Default.FusionUrl + "/workCenters?" + Constants.ParamsGet +
                                                    "&expand=WorkCenterResource.WorkCenterResourceShift" +
                                                    "&fields=WorkCenterName;" +
                                                    "WorkCenterResource:ResourceId;" +
                                                    "WorkCenterResource.WorkCenterResourceShift:ShiftName,StartTime,Duration" +
                                                    "&q=WorkCenterId={0}";

        //Endpoints Manufactura Discreta
        public static string WODiscreteList = Settings.Default.FusionUrl + "/workOrders?" + Constants.ParamsGet +
                                                    "&fields=WorkOrderNumber" +
                                                    "&q=OrganizationId={0} and WorkOrderStatusCode='ORA_RELEASED' and WorkOrderActiveOperation.WorkCenterId={1}";

        public static string WODiscreteDetail = Settings.Default.FusionUrl + "/workOrders?" + Constants.ParamsGet +
                                                    "&expand=WorkOrderResource.WorkOrderOperationResourceInstance" +
                                                    "&fields=WorkOrderId,ItemNumber,Description,UOMCode,PlannedStartQuantity,CompletedQuantity,PlannedStartDate,PlannedCompletionDate;" +
                                                    "WorkOrderResource:ResourceId,ResourceCode,ResourceDescription;" +
                                                    "WorkOrderResource.WorkOrderOperationResourceInstance:" +
                                                    "EquipmentInstanceId,EquipmentInstanceCode,EquipmentInstanceName" +
                                                    "&q=WorkOrderNumber='{0}'";

        //Endpoints Manufactura x Procesos
        public static string WOProcessList = Settings.Default.FusionUrl + "/processWorkOrders?" + Constants.ParamsGet +
                                                    "&fields=WorkOrderNumber" +
                                                    "&q=OrganizationId={0} and WorkOrderStatusCode='ORA_RELEASED' and WorkOrderActiveOperation.WorkCenterId={1}";

        public static string WOProcessDetail = Settings.Default.FusionUrl + "/processWorkOrders?" + Constants.ParamsGet +
                                                    "&expand=ProcessWorkOrderResource.WorkOrderOperationResourceInstance" +
                                                    "&fields=WorkOrderId,ItemNumber,Description,UOMCode,PrimaryProductQuantity,BatchQuantity,PlannedStartDate,PlannedCompletionDate;" +
                                                    "ProcessWorkOrderResource:ResourceId,ResourceCode,ResourceDescription;" +
                                                    "ProcessWorkOrderResource.ResourceInstance:" +
                                                    "EquipmentInstanceId,EquipmentInstanceCode,EquipmentInstanceName" +
                                                    "&q=WorkOrderNumber='{0}'";

        //Endpoints APEX
        public static string LabelTamplate = Settings.Default.ApexUrl + "/getEntity/{0}";

        //Endpoints Planta 1
        public static string WorkOrdersItemList = Settings.Default.FusionUrl + "/workOrders?" + Constants.ParamsGet +
                                            "&fields=WorkOrderNumber,ItemNumber" +
                                            "&q=OrganizationId={0} and WorkOrderStatusCode='ORA_RELEASED' and WorkOrderActiveOperation.WorkCenterId={1}";

        public static string ProductionResourcesP1 = Settings.Default.FusionUrl + "/productionResources?" + Constants.ParamsGet +
                                                        "&fields=ResourceId" +
                                                        "&q=OrganizationId={0} and ResourceType='EQUIPMENT' and ResourceClassCode='EQU'";
        //Endpoints Planta 2
        public static string ProductionResourcesP2 = Settings.Default.FusionUrl + "/productionResources?" + Constants.ParamsGet +
                                                   "&fields=ResourceId" +
                                                   "&q=OrganizationId={0} and ResourceType='EQUIPMENT' and ResourceCode like 'MF-LAM%' or ResourceCode like 'MF-C01%'";

        
    }
}
