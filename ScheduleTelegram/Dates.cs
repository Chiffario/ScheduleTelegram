using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace ScheduleTelegram
{
    public class Dates
    {
        static string NextSchoolDay;
        public static string GetNeededDate()
        // Функция, определяющая следующий рабочий день
        {
            DateTime today = DateTime.Now;
            DateTime tomorrow;
            if (today.DayOfWeek == DayOfWeek.Saturday)
            {
                DateTime nextday = today.AddDays(2);
                tomorrow = nextday;
            }
            else
            {
                DateTime nextday = today.AddDays(1);
                tomorrow = nextday;
            }
            return NextSchoolDay = (string)tomorrow.ToString("dd.MM.yy");
        }
    }
}
