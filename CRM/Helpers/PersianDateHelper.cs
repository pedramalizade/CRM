using System.Globalization;
using System;

namespace CRM.Helpers
{
    public static class PersianDateHelper
    {
        public static string ToPersianDate(this DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();

            // بررسی محدوده مجاز تاریخ
            DateTime minSupportedDate = new DateTime(622, 3, 22, 0, 0, 0, DateTimeKind.Unspecified);
            DateTime maxSupportedDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Unspecified);

            if (date < minSupportedDate || date > maxSupportedDate)
            {
                return null; // یا خطا پرتاب کنید
            }

            try
            {
                return $"{pc.GetYear(date)}/{pc.GetMonth(date):D2}/{pc.GetDayOfMonth(date):D2}";
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        public static DateTime? ToGregorianDate(string persianDate)
        {
            if (string.IsNullOrEmpty(persianDate))
                return null;

            var parts = persianDate.Split('/');
            if (parts.Length != 3)
                return null;

            if (!int.TryParse(parts[0], out int year) ||
                !int.TryParse(parts[1], out int month) ||
                !int.TryParse(parts[2], out int day))
                return null;

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
                return null;
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