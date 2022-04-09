using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http.Headers
{
    internal static class HttpResponseHeadersExtensions
    {
        public static string GetRateLimitBucket(this HttpResponseHeaders headers)
        {
            return headers.TryGetValues("x-ratelimit-bucket", out var values) ? values.FirstOrDefault() : null;
        }

        public static int GetRateLimitLimit(this HttpResponseHeaders headers)
        {
            var value = headers.TryGetValues("x-ratelimit-limit", out var values) ? values.FirstOrDefault() : null;
            return int.TryParse(value, out var intValue) ? intValue : -1;
        }

        public static int GetRateLimitRemaining(this HttpResponseHeaders headers)
        {
            var value = headers.TryGetValues("x-ratelimit-remaining", out var values) ? values.FirstOrDefault() : null;
            return int.TryParse(value, out var intValue) ? intValue : -1;
        }

        public static DateTimeOffset GetRateLimitReset(this HttpResponseHeaders headers)
        {
            var value = headers.TryGetValues("x-ratelimit-reset", out var values) ? values.FirstOrDefault() : null;
            return DateTimeOffset.FromUnixTimeSeconds(long.TryParse(value, out var longValue) ? longValue : 0);
        }

        public static double GetRateLimitResetAfter(this HttpResponseHeaders headers)
        {
            var value = headers.TryGetValues("x-ratelimit-reset-after", out var values) ? values.FirstOrDefault() : null;
            return double.TryParse(value, out var doubleValue) ? doubleValue : -1;
        }
    }
}
