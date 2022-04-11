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
        Task<GuidRoles> GetGuildRolesAsync(string guildId);
        Task SetRolesAsync(string guildId, GuidRoles guildRoles);
        Task RemoveMemberFromCacheAsync(string guildId, string userId);
        Task RemoveGuildRolesFromCacheAsync(string guildId);
    }
}
