namespace Spice.Saffron.ViewModels
{
    public class CalendarViewModel
    {
        public CalendarViewModel(CalendarType type)
        {
            Type = type;
            Items = new List<CalendarItemViewModel>();
        }

        public CalendarType Type { get; set; }
        public List<CalendarItemViewModel> Items { get; set; }
    }

    public class CalendarItemViewModel
    {
        public CalendarItemViewModel(string name, DateTime occurAt)
        {
            Name = name;
            OccurAt = occurAt;
        }
        public string Name { get; set; }
        public DateTime OccurAt { get; set; }
    }

    public enum CalendarType
    {
        Birthday,
        Events
    }
}
