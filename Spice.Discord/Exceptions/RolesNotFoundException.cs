using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.DiscordClient.Exceptions
{
    internal class RolesNotFoundException : Exception
    {
        public RolesNotFoundException(string guildId, string userId)
        {
            GuildId = guildId;
            UserId = userId;
        }

        public string? GuildId { get; set; }
        public string? UserId { get; set; }
    }
}
