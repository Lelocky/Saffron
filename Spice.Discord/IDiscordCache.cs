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
        Task SetMemberAsync(string guildId, string userId, Member member);
        Task<GuildRoles> GetGuildRolesAsync(string guildId);
        Task SetRolesAsync(string guildId, GuildRoles guildRoles);
        Task RemoveMemberFromCacheAsync(string guildId, string userId);
        Task RemoveGuildRolesFromCacheAsync(string guildId);
        Task<GuildEvents> GetGuildEventsAsync(string guildId);
        Task SetGuildEventsAsync(string guildId, GuildEvents guildEvents);
        Task RemoveGuildEventsFromCacheAsync(string guildId);
    }
}
