using System;

namespace Spice.DiscordClient.Exceptions
{
    public class MemberNotFoundException : Exception
    {
        public MemberNotFoundException(string guildId, string userId)
        {
            GuildId = guildId;
            UserId = userId;
        }

        public string GuildId { get; set; }
        public string UserId { get; set; }
    }
}
