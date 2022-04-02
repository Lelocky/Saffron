using Microsoft.Extensions.Options;
using Spice.DiscordClient;
using Spice.Saffron.Configuration.Options;
using System.Security.Claims;

namespace Spice.Saffron.Services
{
    public class DiscordClaimsService : IDiscordClaimsService
    {
        private readonly IDiscordService _discordService;
        private readonly ILogger<DiscordClaimsService> _logger;
        private readonly DiscordGuildSettings _guildSettings;

        public DiscordClaimsService(IDiscordService discordService, ILogger<DiscordClaimsService> logger, IOptions<DiscordGuildSettings> options)
        {
            _discordService = discordService;
            _logger = logger;
            _guildSettings = options.Value;
        }

        public async Task<List<Claim>> GetDiscordRolesAsClaimsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) { throw new ArgumentNullException(nameof(userId)); }

            var userRoles = await _discordService.GetUserRoles(_guildSettings.Id, userId);

            if (userRoles == null)
            {
                return new List<Claim>();
            }

            var claims = new List<Claim>();
            var spiceRoles = GetSpiceRoles();

            foreach (var role in userRoles)
            {
                var foundSpiceRole = spiceRoles.Find(x => x.Id.Equals(role));

                if (foundSpiceRole != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, foundSpiceRole.Name));
                };
            }

            return claims;
        }

        private List<SpiceRole> GetSpiceRoles()
        {
            //This is fucking ugly!! It should be collected and stored somewhere.
            var roles = new List<SpiceRole>
            {
                new SpiceRole{ Id = "456089936148561920", Name = "Leader" },
                new SpiceRole{ Id = "456090312713175052", Name = "Officer" },
                new SpiceRole{ Id = "456101564533309442", Name = "Member" },
                new SpiceRole{ Id = "456101628349513739", Name = "Coconut" },
                new SpiceRole{ Id = "457899357740728330", Name = "Company Friend" },
                new SpiceRole{ Id = "456089498581729290", Name = "Founder" },
                new SpiceRole{ Id = "556133503259901953", Name = "Coconut Farmer" }
            };

            return roles;
        }

        private class SpiceRole
        {
            public string? Id{ get; set; }
            public string? Name { get; set; }
        }
    }
}
