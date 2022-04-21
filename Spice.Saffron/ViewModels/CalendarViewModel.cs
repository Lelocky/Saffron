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
        public CalendarItemViewModel(string name, DateTimeOffset occurAt, string description)
        {
            Name = name;
            OccurAt = occurAt;
            Description = description;
        }
        public string Name { get; set; }
        public DateTimeOffset OccurAt { get; set; }
        public string Description { get; set; }
    }

    public enum CalendarType
    {
        Birthday,
        Events
    }
}
