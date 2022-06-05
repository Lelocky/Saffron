using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.DiscordClient.Models
{
    public abstract class Cacheable
    {
        public bool RetrievedFromCache { get; internal set; } = false;
        public DateTimeOffset CachedAt { get; internal set; }
        internal abstract string GetKey(string keyPartial);
    }
}
