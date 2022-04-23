using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Spice.DiscordClient.Exceptions;
using Spice.DiscordClient.Models;
using Spice.DiscordClient.Models.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Spice.DiscordClient.Models.GuildEvents;

namespace Spice.DiscordClient
{
    internal class DiscordService : IDiscordService
    {
        private readonly HttpClient _httpClient;
        private readonly DiscordApiOptions _options;
        private readonly ILogger<DiscordService> _logger;
        private readonly IDiscordCache _cache;
        private RateLimitInformation _rateLimitInformation;

        public DiscordService(HttpClient httpClient, IOptions<DiscordApiOptions> options, ILogger<DiscordService> logger, IDiscordCache cache)
        {
            _options = options.Value;
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = _options.BaseUrlWithVersion;
            _cache = cache;
            _rateLimitInformation = new RateLimitInformation();

            _httpClient.DefaultRequestHeaders.Add(
                "authorization", $"Bot {_options.BotToken}");
        }

        public async Task<List<string>> GetUserRolesAsync(string guildId, string userId)
        {
            var isRatedLimitedCall = true;

            if (string.IsNullOrWhiteSpace(guildId)) { throw new ArgumentNullException(nameof(guildId)); }
            if (string.IsNullOrWhiteSpace(userId)) { throw new ArgumentNullException(nameof(userId)); }

            try
            {
                //No caching because this is used for authentication
                var result = await GetAsync($"guilds/{guildId}/members/{userId}", isRatedLimitedCall);
                if (result == null)
                {
                    throw new MemberNotFoundException(guildId, userId);
                }

                //But we can cache the member to use it later
                var member = JsonConvert.DeserializeObject<Member>(result, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                });

                await _cache.SetMemberAsync(guildId, userId, member);

                return member.Roles;
            }
            catch (MemberNotFoundException)
            {
                throw;
            }
            catch (RateLimitException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while getting user roles.");
                throw;
            }
        }

        public async Task<Member> GetMemberAsync(string guildId, string userId)
        {
            var isRatedLimitedCall = true;

            if (string.IsNullOrWhiteSpace(guildId)) { throw new ArgumentNullException(nameof(guildId)); }
            if (string.IsNullOrWhiteSpace(userId)) { throw new ArgumentNullException(nameof(userId)); }

            try
            {
                var cached = await _cache.GetMemberAsync(guildId, userId);
                if (cached != null)
                {
                    return cached;
                }

                var result = await GetAsync($"guilds/{guildId}/members/{userId}", isRatedLimitedCall);
                if (result == null)
                {
                    throw new MemberNotFoundException(guildId, userId);
                }

                var member = JsonConvert.DeserializeObject<Member>(result, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                });

                await _cache.SetMemberAsync(guildId, userId, member);

                return member;
            }
            catch (MemberNotFoundException)
            {
                throw;
            }
            catch (RateLimitException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while getting user roles.");
                throw;
            }
        }

        public async Task<GuildRoles> GetGuildRolesAsync(string guildId)
        {
            var isRatedLimitedCall = false;

            if (string.IsNullOrWhiteSpace(guildId)) { throw new ArgumentNullException(nameof(guildId)); }

            try
            {
                var cached = await _cache.GetGuildRolesAsync(guildId);
                if (cached != null)
                {
                    return cached;
                }

                var result = await GetAsync($"guilds/{guildId}/roles", isRatedLimitedCall);
                if (result == null)
                {
                    throw new GuildRolesNotFoundException(guildId);
                }

                var roles = JsonConvert.DeserializeObject<IEnumerable<Role>>(result, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                });

                var guildRoles = new GuildRoles { Roles = roles };

                await _cache.SetRolesAsync(guildId, guildRoles);

                return guildRoles;
            }
            catch (GuildRolesNotFoundException)
            {
                throw;
            }
            catch (RateLimitException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while getting user roles.");
                throw;
            }
        }

        public async Task RemoveMemberFromCacheAsync(string guildId, string userId)
        {
            if (string.IsNullOrWhiteSpace(guildId)) { throw new ArgumentNullException(nameof(guildId)); }
            if (string.IsNullOrWhiteSpace(userId)) { throw new ArgumentNullException(nameof(userId)); }

            try
            {
                await _cache.RemoveMemberFromCacheAsync(guildId, userId);
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
                await _cache.RemoveGuildRolesFromCacheAsync(guildId);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unknown error while removing roles from cache.");
                throw;
            }
        }

        //Should be moved to it's own classs
        private async Task<string> GetAsync(string requestUri, bool isRateLimitedCall)
        {
            if (string.IsNullOrEmpty(requestUri)) { throw new ArgumentNullException(nameof(requestUri)); }

            var httpResult = await _httpClient.GetAsync(requestUri);

            string jsonResult = null;
            if (httpResult.IsSuccessStatusCode)
            {
                jsonResult = await httpResult.Content.ReadAsStringAsync();
            }

            UpdateRateLimitInformation(httpResult.Headers);

            return jsonResult;
        }

        private void UpdateRateLimitInformation(HttpResponseHeaders headers)
        {
            _rateLimitInformation.Bucket = headers.GetRateLimitBucket();
            _rateLimitInformation.Limit = headers.GetRateLimitLimit();
            _rateLimitInformation.ResetAfter = headers.GetRateLimitResetAfter();
            _rateLimitInformation.Reset = headers.GetRateLimitReset();
        }

        public async Task<GuildEvents> GetGuildEventsAsync(string guildId)
        {
            var isRatedLimitedCall = true;

            if (string.IsNullOrWhiteSpace(guildId)) { throw new ArgumentNullException(nameof(guildId)); }

            try
            {
                var cached = await _cache.GetGuildEventsAsync(guildId);
                if (cached != null)
                {
                    return cached;
                }

                var result = await GetAsync($"guilds/{guildId}/scheduled-events", isRatedLimitedCall);
                if (result == null)
                {
                    throw new GuildEventsNotFoundException(guildId);
                }

                var events = JsonConvert.DeserializeObject<IEnumerable<GuildEvent>>(result, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                });

                var guildEvents = new GuildEvents { Events = events };

                await _cache.SetGuildEventsAsync(guildId, guildEvents);

                return guildEvents;
            }
            catch (GuildRolesNotFoundException)
            {
                throw;
            }
            catch (RateLimitException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while getting events.");
                throw;
            }
        }

        public async Task RemoveGuildEventsFromCacheAsync(string guildId)
        {
            if (string.IsNullOrWhiteSpace(guildId)) { throw new ArgumentNullException(nameof(guildId)); }

            try
            {
                await _cache.RemoveGuildEventsFromCacheAsync(guildId);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unknown error while removing events from cache.");
                throw;
            }
        }
    }
}
