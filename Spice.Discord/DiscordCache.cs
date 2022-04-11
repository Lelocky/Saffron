using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Spice.DiscordClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.DiscordClient
{
    internal class DiscordCache : IDiscordCache
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<DiscordCache> _logger;

        public DiscordCache(IDistributedCache cache, ILogger<DiscordCache> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<Member> GetMemberAsync(string guildId, string userId)
        {
            if (string.IsNullOrWhiteSpace(guildId)) { throw new ArgumentNullException(nameof(guildId)); }
            if (string.IsNullOrWhiteSpace(userId)) { throw new ArgumentNullException(nameof(userId)); }

            try
            {
                var cachedMember = await _cache.GetStringAsync(GetMemberCacheKey(guildId, userId));
                if (cachedMember != null)
                {
                    var member = JsonConvert.DeserializeObject<Member>(cachedMember, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Formatting.Indented
                    });

                    member.RetrievedFromCache = true;

                    return member;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unknown error while getting member from cache.");
                throw;
            }
        }

        public async Task<GuidRoles> GetGuildRolesAsync(string guildId)
        {
            if (string.IsNullOrWhiteSpace(guildId)) { throw new ArgumentNullException(nameof(guildId)); }
            
            try
            {
                var cachedRoles = await _cache.GetStringAsync(GetRolesCacheKey(guildId));
                if (cachedRoles != null)
                {
                    var roles = JsonConvert.DeserializeObject<GuidRoles>(cachedRoles, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Formatting.Indented
                    });

                    roles.RetrievedFromCache = true;
                    return roles;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unknown error while getting roles from cache.");
                throw;
            }
        }

        public async Task SetMemberAsync(string guildId, string userId, Member member)
        {
            if (string.IsNullOrWhiteSpace(guildId)) { throw new ArgumentNullException(nameof(guildId)); }
            if (string.IsNullOrWhiteSpace(userId)) { throw new ArgumentNullException(nameof(userId)); }
            if (member == null) { throw new ArgumentNullException(nameof(member)); }

            try
            {
                var cachingKey = GetMemberCacheKey(guildId, userId);

                member.CachedAt = DateTimeOffset.Now;

                await _cache.SetStringAsync(cachingKey, JsonConvert.SerializeObject(member), new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(20) });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unknown error while setting member in cache.");
                throw;
            }
        }

        public async Task SetRolesAsync(string guildId, GuidRoles guildRoles)
        {
            if (string.IsNullOrWhiteSpace(guildId)) { throw new ArgumentNullException(nameof(guildId)); }
            if (guildRoles == null) { throw new ArgumentNullException(nameof(guildRoles)); }

            try
            {
                var cachingKey = GetRolesCacheKey(guildId);

                guildRoles.CachedAt = DateTimeOffset.Now;

                await _cache.SetStringAsync(cachingKey, JsonConvert.SerializeObject(guildRoles), new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(20) });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unknown error while setting member in cache.");
                throw;
            }
        }

        public async Task RemoveMemberFromCacheAsync(string guildId, string userId)
        {
            if (string.IsNullOrWhiteSpace(guildId)) { throw new ArgumentNullException(nameof(guildId)); }
            if (string.IsNullOrWhiteSpace(userId)) { throw new ArgumentNullException(nameof(userId)); }

            try
            {
                await _cache.RemoveAsync(GetMemberCacheKey(guildId, userId));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unknown error while removing member from cache.");
                throw;
            }
        }

        public async Task RemoveGuildRolesFromCacheAsync(string guildId)
        {
            if (string.IsNullOrWhiteSpace(guildId)) { throw new ArgumentNullException(nameof(guildId)); }

            try
            {
                await _cache.RemoveAsync(GetRolesCacheKey(guildId));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unknown error while removing roles from cache.");
                throw;
            }
        }

        private string GetMemberCacheKey(string guildId, string userId)
        {
            return $"MEMBER-{guildId}-{userId}";
        }

        private string GetRolesCacheKey(string guildId)
        {
            return $"GUILDROLES-{guildId}";
        }

       
    }
}
