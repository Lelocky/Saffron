using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Spice.Saffron.Data;
using Spice.Saffron.Extensions;
using Spice.Saffron.Services;
using System.Security.Claims;

namespace Spice.Saffron.Factories
{
	public class AdditionalUserClaimsPrincipalFactory: UserClaimsPrincipalFactory<ApplicationUser>
	{
		private readonly IDiscordService _claimsService;
		private readonly UserManager<ApplicationUser> _userManager;

		public AdditionalUserClaimsPrincipalFactory(
			UserManager<ApplicationUser> userManager,
			IOptions<IdentityOptions> optionsAccessor,
			IDiscordService claimsService)
			: base(userManager, optionsAccessor)
		{ 
			_claimsService = claimsService;
			_userManager = userManager;
		}

		public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
		{			
			var userId = user.UserName;
			var dbUser = await _userManager.FindByNameAsync(userId);
			var roleClaims = await _claimsService.GetDiscordRolesAsClaimsAsync(userId);

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

			identity.AddClaims(roleClaims);

			identity.AddClaim(new Claim(SaffronClaimTypes.Nickname, dbUser.Nickname));

			if (dbUser.ProfileImage != null)
            {
				identity.AddClaim(new Claim(SaffronClaimTypes.ProfileImage, dbUser.ProfileImage));
			}
			
			return principal;
		}
	}
}
