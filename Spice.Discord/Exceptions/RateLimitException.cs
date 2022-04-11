using System;

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
