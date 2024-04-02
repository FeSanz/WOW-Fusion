using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOW_Fusion.Models
{
    internal class WorkOrderShedule
    {

        public string WorkOrderNumber { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedCompletionDate { get; set; }

        /*public WorkOrderShedule(string WorkOrder, DateTime StartDate, DateTime CompletionDat)
        {
            this.WorkOrderNumber = WorkOrder;
            this.PlannedStartDate = StartDate;
            this.PlannedCompletionDate = CompletionDat;
        }*/

    }
}
