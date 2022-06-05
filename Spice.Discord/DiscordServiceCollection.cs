using Microsoft.Extensions.DependencyInjection;
using Spice.DiscordClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spice.DiscordClient.DependencyInjection
{
    public static class DiscordServiceCollection
    {
        public static IServiceCollection AddDiscordService(this IServiceCollection services, Action<DiscordApiOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddHttpClient<IDiscordService, DiscordService>();
            services.AddSingleton<IDiscordCache, DiscordCache>();
            services.AddDistributedMemoryCache();
            return services;
        }
    }
}
