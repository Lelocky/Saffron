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

        public async Task<T> Get<T>(string key) where T : Cacheable
        {
            if (string.IsNullOrWhiteSpace(key)) { throw new ArgumentNullException(nameof(key)); }

            try
            {
                var cachable = (T)Activator.CreateInstance(typeof(T));
                var cacheKey = cachable.GetKey(key);

                var cached = await _cache.GetStringAsync(cacheKey);
                if (cached != null)
                {
                    var result = JsonConvert.DeserializeObject<T>(cached, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Formatting.Indented
                    });

                    result.RetrievedFromCache = true;
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unknown error while getting from cache");
            }

            return null;
        }

        public async Task Set(Cacheable cacheable, string key)
        {
            if (cacheable == null) { throw new ArgumentNullException(nameof(cacheable)); }
            if (string.IsNullOrWhiteSpace(key)) { throw new ArgumentNullException(nameof(key)); }

            try
            {
                cacheable.CachedAt = DateTimeOffset.Now;
                var cacheKey = cacheable.GetKey(key);
                await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(cacheable), new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(20) });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unknown error while setting cache.");
                throw;
            }
        }
        public async Task Remove<T>(string key) where T: Cacheable
        {
            if (string.IsNullOrWhiteSpace(key)) { throw new ArgumentNullException(nameof(key)); }

            try
            {
                var cachable = (T)Activator.CreateInstance(typeof(T));
                var cacheKey = cachable.GetKey(key);
                await _cache.RemoveAsync(cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unknown error while removing from cache");
                throw;
            }
        }
    }
}
