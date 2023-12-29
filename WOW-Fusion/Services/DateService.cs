using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WOW_Fusion.Services
{
    internal class DateService
    {
        public static string Now()
        {
            //DateTime today = DateTime.UtcNow.Date;
            //DateTime today = DateTime.Today;
            DateTimeOffset today = DateTimeOffset.Now;
            return today.ToString("dd-MM-yyyy");
        }

        public static string Today()
        {
            DateTimeOffset today = DateTimeOffset.Now;
            return today.ToString("dd-MM-yyyy HH:mm:ss");
        }

        public static string LocalDate(string dateISO8601)
        {
            DateTimeOffset dateTimeOffset, localTimeOffset;
            dateTimeOffset = DateTimeOffset.Parse(dateISO8601);
            localTimeOffset = dateTimeOffset.ToLocalTime();
            return localTimeOffset.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}
