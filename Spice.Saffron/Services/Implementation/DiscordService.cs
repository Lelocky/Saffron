using AutoMapper;
using Microsoft.Extensions.Options;
using Spice.DiscordClient;
using Spice.DiscordClient.Exceptions;
using Spice.Saffron.Configuration.Options;
using Spice.Saffron.ViewModels;
using System.Security.Claims;

namespace Spice.Saffron.Services
{
    public class DiscordService : IDiscordService
    {
        private readonly DiscordClient.IDiscordService _discordService;
        private readonly ILogger<DiscordService> _logger;
        private readonly DiscordGuildSettings _guildSettings;
        private readonly IMapper _mapper;

        public DiscordService(DiscordClient.IDiscordService discordService, ILogger<DiscordService> logger, IOptions<DiscordGuildSettings> options, IMapper mapper)
        {
            _discordService = discordService;
            _logger = logger;
            _guildSettings = options.Value;
            _mapper = mapper;
        }

        public async Task<List<Claim>> GetDiscordRolesAsClaimsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) { throw new ArgumentNullException(nameof(userId)); }

            try
            {
                var userRoles = await _discordService.GetUserRolesAsync(_guildSettings.Id, userId);

                if (userRoles == null)
                {
                    return new List<Claim>();
                }

                var claims = new List<Claim>();
                var serverRoles = await _discordService.GetGuildRolesAsync(_guildSettings.Id);

                foreach (var role in userRoles)
                {
                    var foundServerRole = serverRoles.Roles.FirstOrDefault(x => x.Id.Equals(role));

                    if (foundServerRole != null)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, foundServerRole.Name));
                    };
                }

                return claims;
            }
            catch (GuildRolesNotFoundException ex)
            {
                _logger.LogError(ex, $"Could not find guild roles for guild {ex.GuildId}");
                return new List<Claim>();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unknown error while getting Discord roles.");
                throw;
            }
        }

        public async Task<ServerRolesViewModel> GetServerRolesAsync()
        {
            try
            {
                var serverRolesViewModel = new ServerRolesViewModel();

                var serverRoles = await _discordService.GetGuildRolesAsync(_guildSettings.Id);
                if (serverRoles != null)
                {
                    //Should probably use something nice like automapper or atleast a mapper class. Too lazy now!
                    serverRolesViewModel.RetrievedFromCache = serverRoles.RetrievedFromCache;
                    serverRolesViewModel.CachedAt = serverRoles.CachedAt;

                    foreach (var role in serverRoles.Roles)
                    {
                        serverRolesViewModel.Roles.Add(new RolesViewModel
                        {
                            Color = role.Color,
                            Hoist = role.Hoist,
                            Icon = role.Icon,
                            Id = role.Id,
                            Managed = role.Managed,
                            Mentionable = role.Mentionable,
                            Name = role.Name,
                            Permissions = role.Permissions,
                            Postition = role.Postition,
                            UnicodeEmoji = role.UnicodeEmoji
                        });
                    }
                }

                return serverRolesViewModel;
            }
            catch (GuildRolesNotFoundException ex)
            {
                _logger.LogError(ex, $"Could not find guild roles for guild {ex.GuildId}");
                return null;

            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unknown error while getting Discord roles.");
                throw;
            }
        }

        public async Task<ChannelMessagesViewModel> GetChannelMessagesAsync(string channelId)
        {
            try
            {
                var channelMessages = await _discordService.GetChannelMessagesAsync(channelId);
                if (channelMessages != null)
                {
                    var viewModel = _mapper.Map<ChannelMessagesViewModel>(channelMessages);
                    return viewModel;
                }

                return null;
                
            }
            catch (ChannelNotFoundException ex)
            {
                _logger.LogError(ex, $"Could not find channelMessages for channelId {channelId}");
                return null;

            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unknown error while getting channelMessages.");
                throw;
            }
        }
    }
}
