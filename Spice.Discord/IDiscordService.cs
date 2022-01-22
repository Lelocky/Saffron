using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Spice.DiscordClient
{
    public interface IDiscordService
    {
        Task<List<string>> GetUserRoles(string guildId, string userId);
    }
}
