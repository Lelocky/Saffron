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
        Task<GuildRoles> GetGuildRolesAsync(string guildId);
        Task RemoveGuildRolesFromCacheAsync(string guildId);
        Task<GuildEvents> GetGuildEventsAsync(string guildId);
        Task RemoveGuildEventsFromCacheAsync(string guildId);
        Task<ChannelMessages> GetChannelMessagesAsync(string channelId);
        Task RemoveChannelMessagesFromCacheAsync(string channelId);
        Task PostMessagesAsync(string channelId, CreateMessage message);
    }
}
