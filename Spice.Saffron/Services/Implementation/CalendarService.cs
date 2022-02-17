using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spice.Saffron.Data;
using Spice.Saffron.ViewModels;

namespace Spice.Saffron.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CalendarService> _logger;

        public CalendarService(UserManager<ApplicationUser> userManager, ILogger<CalendarService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<CalendarViewModel> GetBirthdays()
        {
            var calendarViewModel = new CalendarViewModel(CalendarType.Birthday);

            try
            {
                var users = await _userManager.Users.ToListAsync();
                if (users != null)
                {
                    foreach (var user in users)
                    {
                        if (user.DateOfBirth != null)
                        {
                            calendarViewModel.Items.Add(new CalendarItemViewModel(user.Nickname, (DateTime)user.DateOfBirth));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting users birthdays");
            }

            return calendarViewModel;
        }
    }
}
