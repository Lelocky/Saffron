using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.DiscordClient.Models
{
    public class GuidRoles : Cacheable
    {
        public IEnumerable<Role> Roles { get; set;}
    }

    public class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Permissions { get; set; }
        public int Postition { get; set; }
        public long Color { get; set; }
        public bool Hoist { get; set; }
        public bool Managed { get; set; }
        public bool Mentionable { get; set; }
        public string Icon { get; set; }
        [JsonProperty("unicode_emoji")]
        public string UnicodeEmoji { get; set; }
    }
}
