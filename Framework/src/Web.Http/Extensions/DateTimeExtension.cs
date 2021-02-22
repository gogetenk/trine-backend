using System;

namespace Sogetrel.Sinapse.Framework.Web.Http.Extensions
{
    /// <summary>
    /// Extensions for System.DateTime.
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// Convert a DateTime to an url string.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToUrlString(this DateTime dateTime)
        {
            return dateTime.ToString("s");
        }
    }
}
