namespace Spice.API.V1.ViewModels
{
    public class User
    {
        public Guid Id { get; set; }
        public string DiscordId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
