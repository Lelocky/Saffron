using Spice.Saffron.ViewModels;

namespace Spice.Saffron.Services
{
    public interface ICalendarService
    {
        Task<CalendarViewModel> GetBirthdays();
        Task<CalendarViewModel> GetEvents();
        Task<CalendarViewModel> GetThisWeeksBirthDaysAsync();
    }
}
