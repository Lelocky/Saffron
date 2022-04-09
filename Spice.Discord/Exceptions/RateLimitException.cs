using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.DiscordClient.Exceptions
{
    internal class RateLimitException : Exception
    {
        public RateLimitException(DateTimeOffset limitedUntil)
        {
            LimitedUntil = limitedUntil;
        }

        public DateTimeOffset LimitedUntil { get; private set; }
    }
}
