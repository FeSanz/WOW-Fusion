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

        //Direccion báscula TCP/IP
        public static string RadwagIp = "192.168.12.3";
        public static int RadwagPort = 4001;

        //Dirección impresora TCP/IP
        public static string PrinterIp = "127.0.0.1";
        public static int PrinterPort = 9100;

        //Credenciales autenticación
        public static readonly string FusionUrl = "https://iapxqy-test.fa.ocs.oraclecloud.com/fscmRestApi/resources/11.13.18.05";
        public static readonly string AuthenticationUser = "felipe.antonio@i-condor.com";
        public static readonly string AuthenticationPassword = "CondorXR112";
        public static readonly string Credentials = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(AuthenticationUser + ":" + AuthenticationPassword));

        //Endpoints fusion
        public static readonly string ObtenerTodasLasOrdenes = "/PurchaseOrdersForReceiving";
        public static readonly string ParamsGet = "limit=500&totalResults=true&onlyData=true";

        //Rutas Etiquetas
        public static string PathLabelsP1 = @"D:\Visual Studio Projects\WOW-Fusion\WOW-Fusion\Resources\Labels\LP1";
        public static string PathLabelsPalletP2 = @"D:\Visual Studio Projects\WOW-Fusion\WOW-Fusion\Resources\Labels\LP2\Pallet";
        public static string PathLabelsRollP2 = @"D:\Visual Studio Projects\WOW-Fusion\WOW-Fusion\Resources\Labels\LP2\Roll";
        public static string LaberalyUrl = "http://api.labelary.com/v1/printers/12dpmm/labels/4x2/0/ --data-urlencode {0}";

        public const string organizationId = "300000002650049";

        //ID Centro de trabajo
        public static string WorkCenterIdP2 = "300000002654222";  //1. 300000003523428    2. 300000003523430      3. 300000002654222

        //Etiquetas KEYS
        public static string LabelJson = @"{
                                                ""WORKORDER"": """",
                                                ""ITEMNUMBER"": """",
                                                ""ITEMDESCRIPTION"": """",
                                                ""DESCRIPTIONENGLISH"": """",
                                                ""EQU"": """",
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
                                                ""BUSINESSUNIT"": """",
                                                ""PURCHASEORDER"": """",
                                                ""ADDRESS"": """"
                                          }";
    }
}
