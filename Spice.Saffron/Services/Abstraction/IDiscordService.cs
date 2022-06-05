using Spice.Saffron.ViewModels;
using System.Security.Claims;

namespace Spice.Saffron.Services
{   public interface IDiscordService
    {
        Task<List<Claim>> GetDiscordRolesAsClaimsAsync(string userId);
        Task<ServerRolesViewModel> GetServerRolesAsync();
        Task<ChannelMessagesViewModel> GetChannelMessagesAsync(string channelId);
    }
}
