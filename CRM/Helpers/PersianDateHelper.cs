using System.Globalization;
using System;

namespace CRM.Helpers
{
    public static class PersianDateHelper
    {
        public static string ToPersianDate(this DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            return $"{pc.GetYear(date)}/{pc.GetMonth(date):D2}/{pc.GetDayOfMonth(date):D2}";
        }

        public static DateTime? ToGregorianDate(string persianDate)
        {
            if (string.IsNullOrEmpty(persianDate))
                return null; // به جای خطا، null برمی‌گردونیم

            var parts = persianDate.Split('/');
            if (parts.Length != 3)
                return null; // به جای خطا، null برمی‌گردونیم

            if (!int.TryParse(parts[0], out int year) ||
                !int.TryParse(parts[1], out int month) ||
                !int.TryParse(parts[2], out int day))
                return null; // اگه تبدیل به عدد ممکن نباشه

            // چک کردن محدوده معتبر
            if (year < 1300 || year > 1500 || month < 1 || month > 12 || day < 1 || day > 31)
                return null;

            try
            {
                PersianCalendar pc = new PersianCalendar();
                return pc.ToDateTime(year, month, day, 0, 0, 0, 0);
            }
            catch
            {
                return null; // اگه تاریخ نامعتبر باشه
            }
        }

        public static bool IsValidPersianDate(string persianDate)
        {
            if (string.IsNullOrEmpty(persianDate))
                return false;

            var parts = persianDate.Split('/');
            if (parts.Length != 3)
                return false;

            if (!int.TryParse(parts[0], out int year) ||
                !int.TryParse(parts[1], out int month) ||
                !int.TryParse(parts[2], out int day))
                return false;

            if (year < 1300 || year > 1500 || month < 1 || month > 12 || day < 1 || day > 31)
                return false;

            try
            {
                PersianCalendar pc = new PersianCalendar();
                pc.ToDateTime(year, month, day, 0, 0, 0, 0);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
