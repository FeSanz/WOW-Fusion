﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOW_Fusion.Properties;

namespace WOW_Fusion.Models
{
    internal class BatchPoints
    {
        public static string Organizations = "/inventoryOrganizations?" + Constants.ParamsGet +
                                             "&fields=OrganizationCode,OrganizationName,LocationCode,ManagementBusinessUnitId,ManagementBusinessUnitName" +
                                             "&q=OrganizationId={0}";

        public static string ResourceById = "/productionResources?" + Constants.ParamsGet +
                                            "&fields=ResourceCode,ResourceName" +
                                            "&q=ResourceId={0}";

        public static string WorkCenterByResourceId = "/workCenters?" + Constants.ParamsGet +
                                                      "&fields=WorkCenterId,WorkCenterName" +
                                                      "&finder=WorkCentersByResourceId;ResourceId={0}";
        public static string ShiftByWorkCenter = "/workCenters?" + Constants.ParamsGet +
                                                    "&fields=WorkCenterName;" +
                                                    "WorkCenterResource:ResourceId;" +
                                                    "WorkCenterResource.WorkCenterResourceShift:ShiftName,StartTime,Duration" +
                                                    "&q=WorkCenterId={0}";
    }
}
