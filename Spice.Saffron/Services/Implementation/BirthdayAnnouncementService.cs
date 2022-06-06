using Microsoft.Extensions.Options;
using Spice.Saffron.Configuration.Options;

namespace Spice.Saffron.Services
{
    public class BirthdayAnnouncementService : IBirthdayAnnouncementService
    {
        private readonly ILogger<BirthdayAnnouncementService> _logger;
        private readonly DiscordGuildSettings _guildSettings;
        private readonly DiscordClient.IDiscordService _discordService;
        private readonly ICalendarService _calendarService;

        public BirthdayAnnouncementService(ILogger<BirthdayAnnouncementService> logger, DiscordClient.IDiscordService discordService, IOptions<DiscordGuildSettings> guildSettings, ICalendarService calendarService)
        {
            _logger = logger;
            _discordService = discordService;
            _guildSettings = guildSettings.Value;
            _calendarService = calendarService;
        }

        public bool IsEnabled => _guildSettings.BirthdayAnnouncementEnabled;
        public string ChannelId => _guildSettings.BirthdayAnnouncementChannelId;
        public string CronExpression => _guildSettings.BirthdayAnnouncementCronExpression;

        public async Task AnnounceAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!IsEnabled)
            {
                _logger.LogInformation("Skipping announcement since it's functionality is disabled.");
                return;
            }

            try
            {
                var birthDays = await _calendarService.GetThisWeeksBirthDaysAsync();
                var fields = new List<DiscordClient.Models.CreateMessage.Field>();

                if (birthDays.Items.Any())
                {
                    foreach (var item in birthDays.Items)
                    {
                        fields.Add(new DiscordClient.Models.CreateMessage.Field
                        {
                            Name = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(item.OccurAt.DateTime.ChangeToCurrentYear().DayOfWeek),
                            Value = item.Name
                        });

                    }
                }
                else
                {
                    fields.Add(new DiscordClient.Models.CreateMessage.Field
                    {
                        Name = "No birthdays",
                        Value = "There are no known birthdays this week."
                    });
                }

                await _discordService.PostMessagesAsync(_guildSettings.BirthdayAnnouncementChannelId, new DiscordClient.Models.CreateMessage
                {
                    ChannelId = _guildSettings.BirthdayAnnouncementChannelId,
                    Content = "",
                    Tts = false,
                    Embeds = new List<DiscordClient.Models.CreateMessage.Embed>
                    {
                        new DiscordClient.Models.CreateMessage.Embed
                        {
                            Type = "rich",
                            Title = "Birthdays this week",
                            Description = "",
                            Timestamp = DateTime.UtcNow,
                            Thumbnail = new DiscordClient.Models.CreateMessage.Thumbnail
                            {
                                Url = "https://spice-fc.com/images/birthday-cake.png",
                                Height = 0,
                                Width = 0
                            },
                            Fields = fields,
                            Footer = new DiscordClient.Models.CreateMessage.Footer
                            {
                                Text = "Saffron",
                                IconUrl = "https://spice-fc.com/images/header-logo.png"
                            },
                            Url = "http://spice-fc.com"
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unknown error while announcing birthdays");
            }
        }
        public void SetAnnouncementPostingStatus(bool enabled)
        {
            //_announcementEnabled = enabled;
        }
    }
}
