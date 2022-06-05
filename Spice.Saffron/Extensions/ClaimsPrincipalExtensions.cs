using Spice.Saffron.Extensions;

namespace System.Security.Claims
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetNickname(this ClaimsPrincipal principal)
        {
            if (principal.HasClaim(c => c.Type == SaffronClaimTypes.Nickname))
            {
                return principal.FindFirst(SaffronClaimTypes.Nickname).Value;
            }

            return null;
        }

        public static string GetProfileImage(this ClaimsPrincipal principal)
        {
            if (principal.HasClaim(c => c.Type == SaffronClaimTypes.ProfileImage))
            {
                return principal.FindFirst(SaffronClaimTypes.ProfileImage).Value;
            }

            return null;
        }
    }
}
