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
                    throw new RolesNotFoundException(guildId, userId);
                }

                //But we can cache the member to use it later
                _ = _cache.SetMemberAsync(guildId, userId, result);

                return JsonConvert.DeserializeObject<Member>(result, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                }).Roles;
            }
            catch (RolesNotFoundException)
            {
                throw;
            }
            catch(RateLimitException)
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

                _ = _cache.SetMemberAsync(guildId, userId, result);
                return JsonConvert.DeserializeObject<Member>(result, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                });
            }
            catch (RolesNotFoundException)
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
    }
}
