using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.DiscordClient.Models.Internal
{
    internal class RateLimitInformation
    {
        public int Limit { get; set; } = -1;
        public int Remaining { get; set; } = -1;
        public DateTimeOffset Reset { get; set; }
        public double ResetAfter { get; set; }
        public string Bucket { get; set; }
    }
}
