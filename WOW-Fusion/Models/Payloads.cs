using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOW_Fusion.Models
{
    internal class Payloads
    {

        //*********************************** Payloads para APEX ***********************************
        public static string weightPallet = @"{
                                                ""DateMark"":""0000000000"",
                                                ""OrganizationId"": ""300000002650049"",
                                                ""WorkOrderNumber"": ""WO-PL2-00000"",
                                                ""ItemNumber"": ""XXXX"",
                                                ""PalletNumber"": 0,
                                                ""PalletRolls"": 0,
                                                ""Tare"": 0.0,
                                                ""Weight"": 0.0,
                                                ""Shift"": ""XX""
                                            }";

        public static string weightRoll = @"{
                                                ""DateMark"":""0000000000"",
                                                ""PalletId"": ""0000000000"",
                                                ""RollNumber"": 0,
                                                ""Weight"": 0.0
                                            }";
    }
}
