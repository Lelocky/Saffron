using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.DiscordClient.Exceptions
{
    public class GuildEventsNotFoundException : Exception
    {
        public GuildEventsNotFoundException(string guildId)
        {
            GuildId = guildId;
        }

        public string GuildId { get; set; }
    }
}
