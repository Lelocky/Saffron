using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Spice.Saffron.Services;
using System.Security.Claims;

namespace Spice.Saffron.Factories
{
	public class AdditionalUserClaimsPrincipalFactory: UserClaimsPrincipalFactory<IdentityUser>
	{
		private readonly IDiscordClaimsService _claimsService;

		public AdditionalUserClaimsPrincipalFactory(
			UserManager<IdentityUser> userManager,
			IOptions<IdentityOptions> optionsAccessor,
			IDiscordClaimsService claimsService)
			: base(userManager, optionsAccessor)
		{ 
			_claimsService = claimsService;
		}

		public async override Task<ClaimsPrincipal> CreateAsync(IdentityUser user)
		{			
			var userId = user.UserName;
			var customClaims = await _claimsService.GetDiscordRolesAsClaimsAsync(userId);

			var principal = await base.CreateAsync(user);
            
			if (principal.Identity == null)
            {
				throw new Exception("Principal Identity is null");
			}

			var identity = (ClaimsIdentity)principal.Identity;

			if (identity == null)
			{
				throw new Exception("Identity is null");
			}

			identity.AddClaims(customClaims);

			return principal;
		}
	}
}
