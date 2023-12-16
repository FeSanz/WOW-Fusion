using System;
using System.Text;

namespace WOW_Fusion
{
    internal static class Constants
    {
        //ID Organizaciones
        public const string Plant1Id = "300000002650034";
        public const string Plant2Id = "300000002650049";
        public const string Plant3Id = "300000002650061";

        //Endpoints fusion
        public static readonly string ObtenerTodasLasOrdenes = "/PurchaseOrdersForReceiving";
        public static readonly string ParamsGet = "limit=500&totalResults=true&onlyData=true";

        //Endpoint Etiquetas
        public static string LaberalyUrl = "http://api.labelary.com/v1/printers/12dpmm/labels/4x2/0/ --data-urlencode {0}";

        public const string organizationId = "300000002650049";

        //ID Centro de trabajo
        public static string WorkCenterIdP2 = "300000002654222";  //1. 300000003523428    2. 300000003523430      3. 300000002654222

        //Etiquetas KEYS
        public static string LabelJson = @"{
                                                ""WORKORDER"": """",
                                                ""ITEMNUMBER"": """",
                                                ""ITEMDESCRIPTION"": """",
                                                ""ENGLISHDESCRIPTION"": """",
                                                ""EQU"": """",
                                                ""DATE"": """",
                                                ""BOXNUMBER"": """",
                                                ""PARTNUMBER"": """",
                                                ""LOTNUMBER"": """",
                                                ""WNETKG"": """",
                                                ""WNETLBS"":"""",
                                                ""WGROSSKG"": """",
                                                ""WGROSSLBS"": """",
                                                ""WEIGHTS"": """",
                                                ""ROLLNUMBER"": """",
                                                ""PALLETNUMBER"": """",
                                                ""WIDTHTHICKNESS"": """",
                                                ""LEGALENTITY"": """",
                                                ""PURCHASEORDER"": """",
                                                ""AKAITEM"": """",
                                                ""AKADESCRIPTION"": """",
                                                ""SHIFT"": """",
                                                ""ADDRESS"": """",
                                                ""EMAIL"": """",
                                                ""ATTRIBUTTE01"": """",
                                                ""ATTRIBUTTE03"": """",
                                                ""ATTRIBUTTE04"": """",
                                                ""ATTRIBUTTE05"": """",
                                                ""ATTRIBUTTE06"": """",
                                                ""ATTRIBUTTE07"": """",
                                                ""ATTRIBUTTE08"": """",
                                                ""ATTRIBUTTE09"": """",
                                                ""ATTRIBUTTE10"": """"
                                          }";
    }
}
