using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xmryDownloader
{
    internal class Utils
    {
        private static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static long GetUnixTime(DateTime? targetTime = null)
        {
            targetTime = (targetTime ?? DateTime.Now).ToUniversalTime();

            TimeSpan elapsedTime = targetTime.Value - UNIX_EPOCH;

            return (long)elapsedTime.TotalSeconds;
        }
    }
}
