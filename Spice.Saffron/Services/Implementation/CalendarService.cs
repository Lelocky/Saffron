using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Spice.Saffron.Configuration.Options;
using Spice.Saffron.Data;
using Spice.Saffron.ViewModels;
using System.Globalization;

namespace Spice.Saffron.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CalendarService> _logger;
        private readonly DiscordClient.IDiscordService _discordService;
        private readonly DiscordGuildSettings _guildSettings;

        public CalendarService(UserManager<ApplicationUser> userManager, ILogger<CalendarService> logger, DiscordClient.IDiscordService discordService, IOptions<DiscordGuildSettings> options)
        {
            _userManager = userManager;
            _logger = logger;
            _discordService = discordService;
            _guildSettings = options.Value;
        }

        public async Task<CalendarViewModel> GetBirthdays()
        {
            var calendarViewModel = new CalendarViewModel(CalendarType.Birthday);

            try
            {
                var birthdayItems = await _userManager.Users
                    .Where(user => user.DateOfBirth != null)
                    .Select(user => new CalendarItemViewModel(
                        string.IsNullOrWhiteSpace(user.IngameName) ? user.Nickname : user.IngameName,
                        (DateTimeOffset)user.DateOfBirth,
                        "Birthday"))
                    .ToListAsync();

                calendarViewModel.Items.AddRange(birthdayItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting users birthdays");
            }

            return calendarViewModel;
        }

        public async Task<CalendarViewModel> GetThisWeeksBirthDaysAsync()
        {
            var calendarViewModel = new CalendarViewModel(CalendarType.Birthday);

            try
            {
                // Load only users with birthdays from database
                var users = await _userManager.Users
                    .Where(x => x.DateOfBirth.HasValue)
                    .Select(u => new { u.DateOfBirth, u.Nickname, u.IngameName })
                    .ToListAsync();

                // Filter for this week's birthdays in memory (cannot be translated to SQL efficiently)
                var currentWeek = DateTime.Now.GetIso8601WeekOfYear();
                var userbirthDaysThisWeek = users
                    .Where(x => x.DateOfBirth.Value.ChangeToCurrentYear().GetIso8601WeekOfYear().Equals(currentWeek))
                    .ToList();

                foreach (var user in userbirthDaysThisWeek)
                {
                    calendarViewModel.Items.Add(new CalendarItemViewModel(
                        string.IsNullOrWhiteSpace(user.IngameName) ? user.Nickname : user.IngameName,
                        (DateTimeOffset)user.DateOfBirth,
                        "Birthday"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting users birthdays");
            }

            return calendarViewModel;
        }

        public async Task<CalendarViewModel> GetEvents()
        {
            var calendarViewModel = new CalendarViewModel(CalendarType.Events);

            try
            {
                var guildEvents = await _discordService.GetGuildEventsAsync(_guildSettings.Id);
                if (guildEvents != null && guildEvents.Events != null)
                {
                    foreach (var guildEvent in guildEvents.Events)
                    {
                        calendarViewModel.Items.Add(new CalendarItemViewModel(guildEvent.Name, guildEvent.ScheduledStartTime, guildEvent.Description));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting events");
            }

            return calendarViewModel;
        }
    }
}
