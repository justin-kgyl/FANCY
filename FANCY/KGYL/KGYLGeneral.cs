using System.Globalization;
using System.Threading;

namespace KGYL.GENERAL
{
    public static class KGYLGeneral
    {
        public static string Right(this string value, int length)
        {
            return value.Substring(value.Length - length);
        }

        public static string ShowDollarAmount(this decimal value)
        {
            var numberFormat = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            numberFormat.CurrencyNegativePattern = 1;
            return value.ToString("C", numberFormat);
            //return value.ToString("C", new CultureInfo("en-US"));
        }
    }
}