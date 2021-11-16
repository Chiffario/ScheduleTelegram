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
        public static string GetNeededDate(string commandText)
        // Функция, определяющая следующий рабочий день
        {
            DateTime today = DateTime.Now;
            DateTime tomorrow;
            switch (commandText)
            {
                case "/today":
                    return NextSchoolDay = (string)today.ToString("dd.MM.yy");

                case "/tomorrow":
                    if (today.DayOfWeek == DayOfWeek.Saturday)
                    {
                        tomorrow = today.AddDays(2);
                    }
                    else
                    {
                        tomorrow = today.AddDays(1);
                    }
                    return NextSchoolDay = (string)tomorrow.ToString("dd.MM.yy");
                default:
                    return NextSchoolDay = (string)today.ToString("dd.MM.yy");
            }
            
        }
    }
}
