using MD.PersianDateTime.Core;
using System;
using System.Globalization;

namespace SobhanJuice.Utilities
{
    public static class DateConvertor
    {
        public static PersianDateTime ToShamsi(this DateTime dt)
        {
            PersianCalendar pc = new PersianCalendar();
            PersianDateTime datetimePersianDateTime = new PersianDateTime(pc.GetYear(dt), pc.GetMonth(dt), pc.GetDayOfMonth(dt), pc.GetHour(dt), pc.GetMinute(dt), pc.GetSecond(dt));

            return datetimePersianDateTime;
        }
        public static DateTime ChangeTime(this DateTime dateTime, int hours, int minutes, int seconds)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                hours,
                minutes,
                seconds,
                dateTime.Kind);
        }
    }
}
