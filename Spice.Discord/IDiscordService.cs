using Spice.DiscordClient.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Spice.DiscordClient
{
    public interface IDiscordService
    {
        Task<List<string>> GetUserRolesAsync(string guildId, string userId);
        Task<Member> GetMemberAsync(string guildId, string userId);
        Task RemoveMemberFromCacheAsync(string guildId, string userId);
        Task<GuidRoles> GetGuildRolesAsync(string guildId);
        Task RemoveGuildRolesFromCacheAsync(string guildId);
    }
}
