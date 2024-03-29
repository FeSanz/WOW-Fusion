﻿using System;
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

        public static string weightPalletUpdate = @"{
                                                        ""Tare"": 0.0,
                                                        ""Weight"": 0.0
                                                    }";

        public static string weightRollUpdate = @"{
                                                        ""Pallet"": 0,
                                                        ""Weight"": 0.0
                                                    }";

        //************************************* Payloads Planta 3 ************************************
        public static string receivingReceiptRequests = @"{
                                                        ""ReceiptSourceCode"": ""VENDOR"",
                                                        ""OrganizationCode"": ""CUECONOMA"",
                                                        ""VendorName"": ""El Maharaja de La Riviera SA de CV"",
                                                        ""VendorSiteCode"": ""PRINCIPAL"",
                                                        ""BusinessUnit"": ""SCAN"",
                                                        ""EmployeeId"": ""300000002504391"",
                                                        ""lines"": []
                                                    }";

        public static string receiptLines = @"{
                                        ""ReceiptSourceCode"": ""VENDOR"",
                                        ""SourceDocumentCode"": ""PO"",
                                        ""TransactionType"": ""RECEIVE"",
                                        ""AutoTransactCode"": ""RECEIVE"",
                                        ""OrganizationCode"": ""CUECONOMA"",
                                        ""DocumentNumber"": ""PCTTH23990000101"",
                                        ""DocumentLineNumber"": 1,
                                        ""ItemNumber"": ""01-014-000407"",
                                        ""ItemId"": ""100000004137173"",
                                        ""Quantity"": 1,
                                        ""UnitOfMeasure"": ""KILOGRAMO"",
                                        ""SoldtoLegalEntity"": ""Travel Ten SA de CV""
                                    }";
    }
}
