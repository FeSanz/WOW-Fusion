using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOW_Fusion.Properties;

namespace WOW_Fusion.Models
{
    internal class EndPointsInventory
    {
        //EndPoints Planta 3
        public static string PurchaseOrdersList = Settings.Default.FusionUrl + "/purchaseOrders?" + Constants.ParamsGet +
                                                "&fields=OrderNumber" +
                                                "&q=ProcurementBUId={0} and StatusCode='OPEN'";

        public static string PurchaseOrder = Settings.Default.FusionUrl + "/purchaseOrders?" + Constants.ParamsGet +
                                                "&fields=OrderNumber,POHeaderId,SoldToLegalEntity,Buyer,BuyerId,Supplier,SupplierSite,ProcurementBU,BillToLocation;" +
                                                "lines:POLineId,LineNumber,StatusCode,LineType,ItemId,Item,Description,UOMCode,UOM,Quantity" +
                                                "&q=OrderNumber='{0}'";
    }
}
