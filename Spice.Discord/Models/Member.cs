using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.DiscordClient.Models
{
    public class Member
    {
        public List<string> Roles { get; set; }
        public User User { get; set; }
        public string Nick { get; set; }
        public string Avatar { get; set; }
        [JsonProperty("premium_since")]
        public DateTimeOffset PremiumSince { get; set; }
        [JsonProperty("joined_at")]
        public DateTimeOffset JoinedAt { get;set; }
        [JsonProperty("is_pending")]
        public bool IsPending { get; set; }
        public bool Pending { get; set; }
        [JsonProperty("communication_disabled_until")]
        public DateTimeOffset CommunicationDisabledUntil { get; set; }
        public string Flags { get; set; }
        public bool Mute { get; set; }
        public bool Deaf { get; set; }


    }
}
