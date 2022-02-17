using System.Security.Claims;

namespace Spice.Saffron.Services
{   public interface IDiscordClaimsService
    {
        Task<List<Claim>> GetDiscordRolesAsClaimsAsync(string userId);
    }
}
