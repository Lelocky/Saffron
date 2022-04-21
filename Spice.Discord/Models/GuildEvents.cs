using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.DiscordClient.Models
{
    public class GuildEvents : Cacheable
    {
        public IEnumerable<GuildEvent> Events { get; set; }

        public class GuildEvent
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            [JsonProperty("scheduled_start_time")]
            public DateTimeOffset ScheduledStartTime { get; set; }
            [JsonProperty("scheduled_end_time")]
            public DateTimeOffset ScheduledEndTime { get; set; }
            [JsonProperty("privacy_level")]
            public int PrivacyLevel { get; set; }
            public int Status { get; set; }
            public Creator Creator { get; set; }
        }

        public class Creator
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public string Avatar { get; set; }
        }
    }
}
