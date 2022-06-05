using Spice.DiscordClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.DiscordClient
{
    internal interface IDiscordCache
    {
        Task<T> Get<T>(string key) where T : Cacheable;
        Task Set(Cacheable cacheable, string key);
        Task Remove<T>(string key) where T : Cacheable;
    }
}
