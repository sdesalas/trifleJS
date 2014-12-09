using System;
using System.Collections.Generic;
using System.Text;

namespace TrifleJS
{
    public static class Extensions
    {
        /// <summary>
        /// Returns a unix timestamp
        /// </summary>
        /// <returns></returns>
        public static int ToUnixTimestamp(this DateTime date) {
            TimeSpan span = (date - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
            return (int)span.TotalSeconds;
        }
    }
}
