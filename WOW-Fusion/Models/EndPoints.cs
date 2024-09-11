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
                                                    "&fields=OrganizationId,OrganizationCode,OrganizationName,LocationCode,ManagementBusinessUnitId,ManagementBusinessUnitName" +
                                                    "&q=OrganizationId={0}";

        public static string AuthFusion = "{0}/inventoryOrganizations?" + Constants.ParamsGet +
                                                    "&fields=OrganizationId,OrganizationCode,OrganizationName,LocationCode,ManagementBusinessUnitId,ManagementBusinessUnitName" +
                                                    "&q=OrganizationId={1}";

        public static string ResourcesTypeMachine = Settings.Default.FusionUrl + "/productionResources?" + Constants.ParamsGet +
                                                  "&fields=ResourceId,ResourceCode,ResourceName" +
                                                  "&q=OrganizationId={0} and ResourceType='EQUIPMENT' and ResourceCode like '%MF-%'";

        public static string ResourceById = Settings.Default.FusionUrl + "/productionResources?" + Constants.ParamsGet +
                                                    "&fields=ResourceCode,ResourceName" +
                                                    "&q=ResourceId={0}";

        public static string WorkCenters = Settings.Default.FusionUrl + "/workCenters?" + Constants.ParamsGet +
                                                    "&fields=WorkCenterId,WorkCenterName,WorkAreaId,WorkAreaName" +
                                                    "&q=OrganizationId={0}";

        public static string WorkCentersById = Settings.Default.FusionUrl + "/workCenters?" + Constants.ParamsGet +
                                                    "&fields=WorkCenterName,WorkCenterDescription,WorkAreaId,WorkAreaName" +
                                                    "&q=OrganizationId={0} and WorkCenterId={1}";

        public static string WorkCenterByResourceId = Settings.Default.FusionUrl + "/workCenters?" + Constants.ParamsGet +
                                                    "&fields=WorkCenterId,WorkCenterName" +
                                                    "&finder=WorkCentersByResourceId;ResourceId={0}";

        public static string ShiftByWorkCenter = Settings.Default.FusionUrl + "/workCenters?" + Constants.ParamsGet +
                                                    "&fields=WorkCenterName;" +
                                                    "WorkCenterResource:ResourceId;" +
                                                    "WorkCenterResource.WorkCenterResourceShift:ShiftName,StartTime,Duration" +
                                                    "&q=WorkCenterId={0}";

        public static string SalesOrders = Settings.Default.FusionUrl + "/salesOrdersForOrderHub?" + Constants.ParamsGet +
                                                    "&fields=BuyingPartyId,BuyingPartyName,BuyingPartyNumber,CustomerPONumber" +
                                                    "&q=OrderNumber='{0}' and BusinessUnitId={1}";

        //Endpoints Manufactura Discreta
        public static string WODiscreteList = Settings.Default.FusionUrl + "/workOrders?" + Constants.ParamsGet +
                                                    "&fields=WorkOrderNumber,PlannedStartDate,PlannedCompletionDate" +
                                                    "&q=OrganizationId={0} and WorkOrderStatusCode='ORA_RELEASED' and WorkOrderOperation.WorkCenterId={1} and PlannedCompletionDate>='{2}'";

        public static string WODiscreteDetail = Settings.Default.FusionUrl + "/workOrders?" + Constants.ParamsGet +
                                                    "&fields=WorkOrderId,ItemNumber,Description,UOMCode,PlannedStartQuantity,CompletedQuantity,PlannedStartDate,PlannedCompletionDate;" +
                                                    "WorkOrderResource:ResourceId,ResourceCode,ResourceName;" +
                                                    "WorkOrderResource.WorkOrderOperationResourceInstance:" +
                                                    "EquipmentInstanceCode,EquipmentInstanceName" +
                                                    "&q=WorkOrderNumber='{0}'";

        //Endpoints Manufactura x Procesos
        public static string WOProcessList = Settings.Default.FusionUrl + "/processWorkOrders?" + Constants.ParamsGet +
                                                    "&fields=WorkOrderNumber,PlannedStartDate,PlannedCompletionDate" +
                                                    "&q=OrganizationId={0} and WorkOrderStatusCode='ORA_RELEASED' and Operation.WorkCenterId={1}";

        public static string WOProcesstByOperation = Settings.Default.FusionUrl + "/processWorkOrders?" + Constants.ParamsGet +
                                                    "&fields=WorkOrderNumber,PlannedStartDate,PlannedCompletionDate" +
                                                    "&q=OrganizationId={0} and WorkOrderStatusCode='ORA_RELEASED' and Operation.WorkCenterId={1} and Operation.OperationSequenceNumber={2}";

        public static string WOListByResource = Settings.Default.FusionUrl + "/processWorkOrders?" + Constants.ParamsGet +
                                                    "&fields=WorkOrderNumber,PlannedStartDate,PlannedCompletionDate" +
                                                    "&q=WorkOrderStatusCode='ORA_RELEASED' and OrganizationId={0} and ProcessWorkOrderResource.ResourceId={1}";

        public static string WOByMachine = Settings.Default.FusionUrl + "/processWorkOrders?" + Constants.ParamsGet +
                                                    "&fields=WorkOrderNumber,PlannedStartDate,PlannedCompletionDate" +
                                                    "&q=OrganizationId={0} and WorkOrderStatusCode='ORA_RELEASED' and ProcessWorkOrderResource.ResourceId={1}";

        public static string WOProcessData = Settings.Default.FusionUrl + "/processWorkOrders?" + Constants.ParamsGet +
                                                    "&fields=WorkOrderId,ItemNumber,Description,UOMCode,PrimaryProductQuantity,CompletedQuantity,PlannedStartDate,PlannedCompletionDate;" +
                                                    "ProcessWorkOrderDFF:pedidoDeVenta" +
                                                    "&q=WorkOrderNumber='{0}' and OrganizationId={1}";

        public static string WOProcessDataP3 = Settings.Default.FusionUrl + "/processWorkOrders?" + Constants.ParamsGet +
                                                    "&fields=WorkOrderId,ItemNumber,Description,UOMCode,PrimaryProductQuantity,CompletedQuantity,PlannedStartDate,PlannedCompletionDate" +
                                                    "&q=WorkOrderNumber='{0}' and OrganizationId={1}";

        public static string WOProcessDetailP1 = Settings.Default.FusionUrl + "/processWorkOrders?" + Constants.ParamsGet +
                                                    "&fields=WorkOrderId,PrimaryProductId,ItemNumber,Description,PrimaryProductQuantity,CompletedQuantity,PrimaryProductUOMCode,PlannedStartDate,PlannedCompletionDate;" +
                                                    "ProcessWorkOrderResource:ResourceId,ResourceCode,ResourceName;" +
                                                    "ProcessWorkOrderResource.ResourceInstance:" +
                                                    "EquipmentInstanceCode,EquipmentInstanceName;" +
                                                    "ProcessWorkOrderDFF:pedidoDeVenta;" +
                                                    "ProcessWorkOrderOutput:OperationSequenceNumber,OperationName" +
                                                    "&q=WorkOrderNumber='{0}' and OrganizationId={1}";

        //Enpoints AKA
        public static string TradingPartnerItemRelationships = Settings.Default.FusionUrl + "/tradingPartnerItemRelationships?" + Constants.ParamsGet +
                                                            "&fields=TradingPartnerType,TradingPartnerName,TradingPartnerItemNumber,TradingPartnerItemDescription,RelationshipDescription" +
                                                            "&q=Item='{0}' and RegistryId='{1}'";

        public static string TradingPartnerItems = Settings.Default.FusionUrl + "/tradingPartnerItemRelationships?" + Constants.ParamsGet +
                                                    "&fields=TradingPartnerItemNumber,TradingPartnerItemDescription" +
                                                    "&q={0}";

        //Endpoints APEX
        public static string LabelTamplate = Settings.Default.ApexUrl + "/labels/{0}/{1}/{2}";
        public static string WeightRolls = Settings.Default.ApexUrl + "/weightRolls";
        public static string RollsOrder = Settings.Default.ApexUrl + "/rollsOrder/{0}/{1}";
        public static string WeightSacks = Settings.Default.ApexUrl + "/weightSacks";
        public static string SacksOrder = Settings.Default.ApexUrl + "/sacksOrder/{0}/{1}";
        public static string LabelsPrinted = Settings.Default.ApexUrl + "/labelsPrinted/{0}";
        public static string LabelsRecords = Settings.Default.ApexUrl + "/labelsRecords";
        public static string Auth = Settings.Default.ApexUrl + "/labelUsers/{0}";

        //Endpoints Planta 1
        public static string WorkOrdersItemList = Settings.Default.FusionUrl + "/workOrders?" + Constants.ParamsGet +
                                            "&fields=WorkOrderNumber,ItemNumber" +
                                            "&q=OrganizationId={0} and WorkOrderStatusCode='ORA_CLOSED' and WorkOrderOperation.WorkCenterId={1}";

        public static string ProductionResourcesP1 = Settings.Default.FusionUrl + "/productionResources?" + Constants.ParamsGet +
                                                        "&fields=ResourceId" +
                                                        "&q=OrganizationId={0} and ResourceType='EQUIPMENT' and ResourceClassCode='EQU'";

        public static string ItemP1 = Settings.Default.FusionUrl + "/itemsV2?" + Constants.ParamsGet +
                                    "&fields=ItemId,LongDescription" +
                                    "&q=ItemNumber='{0}' and OrganizationId={1}";

        //Endpoints Planta 2
        public static string ProductionResourcesP2 = Settings.Default.FusionUrl + "/productionResources?" + Constants.ParamsGet +
                                                   "&fields=ResourceId" +
                                                   "&q=OrganizationId={0} and ResourceType='EQUIPMENT' and ResourceCode like 'MF-LAM%' or ResourceCode like 'MF-C01%'";

        public static string ItemP2 = Settings.Default.FusionUrl + "/itemsV2?" + Constants.ParamsGet +
                                    "&fields=ItemId,LongDescription,UnitWeightQuantity,WeightUOMValue,WeightUOMValue,MaximumLoadWeight,ContainerTypeValue,LotControlValue" +
                                    "&q=ItemNumber='{0}' and OrganizationId={1}";

        //EndPoints Planta 3
        public static string PurchaseOrdersList = Settings.Default.FusionUrl + "/purchaseOrders?" + Constants.ParamsGet +
                                                "&fields=OrderNumber" +
                                                "&q=ProcurementBUId={0} and StatusCode='OPEN'";

        public static string PurchaseOrder = Settings.Default.FusionUrl + "/purchaseOrders?" + Constants.ParamsGet +
                                                "&fields=OrderNumber,POHeaderId,SoldToLegalEntity,Buyer,BuyerId,Supplier,SupplierSite,ProcurementBU,BillToLocation;" +
                                                "lines:POLineId,LineNumber,StatusCode,LineType,ItemId,Item,Description,UOMCode,UOM,Quantity" +
                                                "&q=OrderNumber='{0}'";

        public static string ProductionResourcesP3 = Settings.Default.FusionUrl + "/productionResources?" + Constants.ParamsGet +
                                                        "&fields=ResourceId,ResourceCode,ResourceName" +
                                                        "&q=OrganizationId={0} and (ResourceId=300000003969051 or ResourceId=300000003969052)";
    }
}
