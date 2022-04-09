using Spice.DiscordClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.DiscordClient
{
    internal interface IDiscordCache
    {
        Task<Member> GetMemberAsync(string guildId, string userId);
        Task SetMemberAsync(string guildId, string userId, string json);
    }
}
