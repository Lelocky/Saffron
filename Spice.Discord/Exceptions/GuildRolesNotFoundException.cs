using System;

namespace Spice.DiscordClient.Exceptions
{
    public class GuildRolesNotFoundException : Exception
    {
        public GuildRolesNotFoundException(string guildId)
        {
            GuildId = guildId;
        }

        public string GuildId { get; set; }
    }
}
