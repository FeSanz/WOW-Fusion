using System;
using System.Drawing;
using System.Text;

namespace WOW_Fusion
{
    internal static class Constants
    {
        //ID Organizaciones
        public const string Plant1Id = "300000003173662";
        public const string Plant2Id = "300000003173674";
        public const string Plant3Id = "300000003173686";

        public static string BusinessUnitId = "000000000000000";
        public static string UserName = "user.name";
        public static int UserId = 0;

        public static bool loginMode = true;

        //frmLoading
        public static string pop = "Procesando...";
        public static string Exception = "Error de excepción";
        public static int LastPrint = 0;

        //Endpoints fusion
        public static readonly string ParamsGet = "limit=500&totalResults=true&onlyData=true";

        //Endpoint Etiquetas
        public static string LaberalyUrl = "http://api.labelary.com/v1/printers/8dpmm/labels/4x2/0/ --data-urlencode \"{0}\"";

        public static string OrdersPrintedP1 = @"C:\Fusion\P1Labels.txt";
        public static string OrdersReprintedP1 = @"C:\Fusion\P1Reprinted.txt";
        public static string OrdersPrintedP2 = @"C:\Fusion\P2Labels.txt";

        public static string UPCPrefix = "75063839";

        //Etiquetas KEYS
        public static string LabelJson = @"{
                                                ""WORKORDER"": """",
                                                ""ITEMNUMBER"": """",
                                                ""ITEMDESCRIPTION"": """",
                                                ""ENGLISHDESCRIPTION"": """",
                                                ""EQU"": """",
                                                ""DATE"": """",
                                                ""SHIFT"": """",
                                                ""BOX"": """",
                                                ""ROLL"": """",
                                                ""SACK"": """",
                                                ""PALET"": """",
                                                ""LOTNUMBER"": """",
                                                ""WNETKG"": """",
                                                ""WGROSSKG"": """",
                                                ""WNETLBS"":"""",
                                                ""WGROSSLBS"": """",
                                                ""WEIGHTS"": """",
                                                ""WIDTHTHICKNESS"": """",
                                                ""AKAITEM"": """",
                                                ""AKADESCRIPTION"": """",
                                                ""LEGALENTITY"": """",
                                                ""PURCHASEORDER"": """",
                                                ""PONUM"": """",
                                                ""ADDRESS"": """",
                                                ""EMAIL"": """",
                                                ""UPCA"": """"
                                          }";
    }
}
