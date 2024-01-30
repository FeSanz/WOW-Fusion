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
        public static readonly string ParamsGet = "limit=500&totalResults=true&onlyData=true";

        //Endpoint Etiquetas
        public static string LaberalyUrl = "http://api.labelary.com/v1/printers/8dpmm/labels/4x2/0/ --data-urlencode {0}";

        public static string PathPrintedLables = @"C:\Fusion\Labels.txt";

        public static string UPCPrefix = "7506383";

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
                                                ""ADDRESS"": """",
                                                ""EMAIL"": """",
                                                ""UPCA"": """",
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
