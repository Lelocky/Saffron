using Microsoft.AspNetCore.Identity;

namespace Spice.Saffron.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string Nickname { get; set; }
        public string ProfileImage { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
