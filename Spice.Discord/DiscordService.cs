using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Spice.DiscordClient.Exceptions;
using Spice.DiscordClient.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Spice.DiscordClient
{
    internal class DiscordService : IDiscordService
    {
        private readonly HttpClient _httpClient;
        private readonly DiscordApiOptions _options;
        private readonly ILogger<DiscordService> _logger;

        public DiscordService(HttpClient httpClient, IOptions<DiscordApiOptions> options, ILogger<DiscordService> logger)
        {
            _options = options.Value;
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = _options.BaseUrlWithVersion;

            _httpClient.DefaultRequestHeaders.Add(
                "authorization", $"Bot {_options.BotToken}");
        }

        public async Task<List<string>> GetUserRoles(string guildId, string userId)
        {
            if (string.IsNullOrWhiteSpace(guildId)) { throw new ArgumentNullException(nameof(guildId)); }
            if (string.IsNullOrWhiteSpace(userId)) { throw new ArgumentNullException(nameof(userId)); }

            try
            {
                var result = await _httpClient.GetFromJsonAsync<Member>($"guilds/{guildId}/members/{userId}");
                if (result == null)
                {
                    throw new RolesNotFoundException(guildId, userId);
                }

                return result.Roles;
            }
            catch (RolesNotFoundException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while getting user roles.");
                throw;
            }
        }

        //public async Task<IEnumerable<GitHubBranch>?> GetAspNetCoreDocsBranchesAsync() =>
        //    await _httpClient.GetFromJsonAsync<IEnumerable<GitHubBranch>>(
        //        "repos/dotnet/AspNetCore.Docs/branches");
    }
}
