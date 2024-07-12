using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOW_Fusion.Models
{
    internal class Payloads
    {
        //*********************************** Impresion de etiquetas ***********************************
        public static string labelsPrinted = @"{
                                                    ""DateMark"":""0000000000"",
                                                    ""WorkOrder"": ""0"",
                                                    ""UserId"": 0,
                                                    ""LastPage"": 0,
                                                    ""LimitPrint"": 0
                                               }";

        public static string labelsPrintedUpdate = @"{
                                                    ""UserId"": 0,
                                                    ""LastPage"": 0
                                               }";

        public static string labelsRecords = @"{
                                                    ""DateMark"":""0000000000"",
                                                    ""WorkOrder"": ""0"",
                                                    ""UserId"": 0,
                                                    ""StartPage"": 0,
                                                    ""EndPage"": 0,
                                                    ""Operation"": """"
                                                 }";

        //************************************** Pesaje rollos *****************************************
        public static string weightRolls = @"{
                                                ""DateMark"":""0000000000"",
                                                ""OrganizationId"": 0,
                                                ""WorkOrderId"": 0,
                                                ""WorkOrder"": ""000"",
                                                ""ItemNumber"": ""X"",
                                                ""Pallet"": 0,
                                                ""Roll"": 0,
                                                ""Tare"": 0.0,
                                                ""Core"": 0.0,
                                                ""Net"": 0.0,
                                                ""Shift"": ""XX""
                                            }";


        public static string weightRollUpdate = @"{
                                                    ""OrganizationId"": 0,
                                                    ""WorkOrder"": ""000"",
                                                    ""Roll"": 0,
                                                    ""Net"": 0.0
                                                }";

        //************************************** Pesaje sacos *****************************************
        public static string weightSack = @"{
                                                ""DateMark"":""0000000000"",
                                                ""OrganizationId"": 0,
                                                ""WorkOrderId"": 0,
                                                ""WorkOrder"": ""000"",
                                                ""ItemNumber"": ""X"",
                                                ""Sack"": 0,
                                                ""Tare"": 0.0,
                                                ""Bag"": 0.0,
                                                ""Net"": 0.0,
                                                ""Shift"": ""XX""
                                            }";


        public static string weightSackUpdate = @"{
                                                    ""OrganizationId"": 0,
                                                    ""WorkOrder"": ""000"",
                                                    ""Bag"": 0,
                                                    ""Net"": 0.0
                                                }";
    }
}
