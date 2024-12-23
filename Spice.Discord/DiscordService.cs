﻿using Microsoft.Extensions.Logging;
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
using static Spice.DiscordClient.Models.ChannelMessages;
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

                await _cache.Set(member, $"{guildId}-{userId}");

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
                var cached = await _cache.Get<Member>($"{guildId}-{userId}");
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

                await _cache.Set(member, $"{guildId}-{userId}");

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
                var cached = await _cache.Get<GuildRoles>(guildId);
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

                await _cache.Set(guildRoles, guildId);

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
                await _cache.Remove<Member>($"{guildId}-{userId}");
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
                await _cache.Remove<GuildRoles>(guildId);
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

        private async Task<string> PostAsync(string requestUri, object objectToPost)
        {
            if (string.IsNullOrEmpty(requestUri)) { throw new ArgumentNullException(nameof(requestUri)); }
            if (objectToPost == null) { throw new ArgumentNullException(nameof(objectToPost)); }

            var json = JsonConvert.SerializeObject(objectToPost, Formatting.Indented);
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var httpResult = await _httpClient.PostAsync(requestUri, byteContent);

            string jsonResult = null;
            if (httpResult.IsSuccessStatusCode)
            {
                jsonResult = await httpResult.Content.ReadAsStringAsync();
            }

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
                var cached = await _cache.Get<GuildEvents>(guildId);
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

                await _cache.Set(guildEvents, guildId);

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
                await _cache.Remove<GuildEvents>(guildId);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unknown error while removing events from cache.");
                throw;
            }
        }

        public async Task<ChannelMessages> GetChannelMessagesAsync(string channelId)
        {
            var isRatedLimitedCall = true;

            if (string.IsNullOrWhiteSpace(channelId)) { throw new ArgumentNullException(nameof(channelId)); }

            try
            {
                var cached = await _cache.Get<ChannelMessages>(channelId);
                if (cached != null)
                {
                    return cached;
                }

                var result = await GetAsync($"channels/{channelId}/messages", isRatedLimitedCall);
                if (result == null)
                {
                    throw new ChannelNotFoundException(channelId);
                }

                var messages = JsonConvert.DeserializeObject<IEnumerable<ChannelMessage>>(result, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                });

                var channelMessages = new ChannelMessages { Messages = messages };

                await _cache.Set(channelMessages, channelId);

                return channelMessages;
            }
            catch (ChannelNotFoundException)
            {
                throw;
            }
            catch (RateLimitException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while getting messages.");
                throw;
            }
        }

        public async Task PostMessagesAsync(string channelId, CreateMessage message)
        {
            if (string.IsNullOrWhiteSpace(channelId)) { throw new ArgumentNullException(nameof(channelId)); }

            try
            {
                var result = await PostAsync($"channels/{channelId}/messages", message);
                if (result == null)
                {
                    throw new Exception(); //Make exception
                }
            }
            catch (RateLimitException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while posting messages.");
                throw;
            }
        }

        public async Task RemoveChannelMessagesFromCacheAsync(string channelId)
        {
            if (string.IsNullOrWhiteSpace(channelId)) { throw new ArgumentNullException(nameof(channelId)); }

            try
            {
                await _cache.Remove<ChannelMessages>(channelId);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unknown error while removing channelMessages from cache.");
                throw;
            }
        }
    }
}
