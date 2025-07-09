using System;
using System.Globalization;

namespace OfficeNet.Domain.Contracts
{

    public static class DateHelper
    {
        public static DateTime ParseDate(string dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr))
                return DateTime.MinValue;


            return DateTime.ParseExact(dateStr, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        }
    }
}
