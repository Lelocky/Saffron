using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Spice.DiscordClient.Models;
using System;
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

        public async Task SetMemberAsync(string guildId, string userId, string json)
        {
            if (string.IsNullOrWhiteSpace(guildId)) { throw new ArgumentNullException(nameof(guildId)); }
            if (string.IsNullOrWhiteSpace(guildId)) { throw new ArgumentNullException(nameof(guildId)); }
            if (string.IsNullOrWhiteSpace(json)) { throw new ArgumentNullException(nameof(json)); }

            try
            {
                var cachingKey = GetMemberCacheKey(guildId, userId);
                await _cache.SetStringAsync(cachingKey, json, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(20) });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unknown error while setting member in cache.");
                throw;
            }
        }

        private string GetMemberCacheKey(string guildId, string userId)
        {
            return $"{guildId}-{userId}";
        }
    }
}
