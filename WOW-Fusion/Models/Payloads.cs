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
                                                ""OrganizationId"": 0,
                                                ""WorkOrderNumber"": ""0000"",
                                                ""ItemNumber"": ""XXXX"",
                                                ""PalletNumber"": 0,
                                                ""ContentRolls"": 0,
                                                ""Tare"": 0.0,
                                                ""Weight"": 0.0,
                                                ""Shift"": ""XX""
                                            }";

        public static string weightPalletUpdate = @"{
                                                        ""Tare"": 0.0,
                                                        ""Weight"": 0.0
                                                    }";


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
