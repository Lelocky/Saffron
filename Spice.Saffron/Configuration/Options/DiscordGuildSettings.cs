namespace Spice.Saffron.Configuration.Options
{
    public class DiscordGuildSettings
    {
        public string Id { get; set; }
        public string BirthdayAnnouncementChannelId { get; set; }
        public string BirthdayAnnouncementCronExpression { get; set; }
        public bool BirthdayAnnouncementEnabled { get; set; }
    }
}
