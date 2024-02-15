using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOW_Fusion.Models
{
    internal class P3Payloads
    {
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
