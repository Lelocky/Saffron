using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.DiscordClient.Exceptions
{
    public class ChannelNotFoundException : Exception
    {
        public ChannelNotFoundException(string channelId)
        {
            ChannelId = channelId;
        }

        public string ChannelId { get; set; }
    }
}
