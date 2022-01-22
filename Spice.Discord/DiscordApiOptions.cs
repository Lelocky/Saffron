using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.DiscordClient
{
    public class DiscordApiOptions
    {
        public string? BotToken { get; set; }
        public string BaseUrl { get; set; } = "https://discord.com/api/";
        public int Version { get; set; } = 9;

        internal Uri BaseUrlWithVersion => new Uri($"{BaseUrl}v{Version}/");
        
    }
}
