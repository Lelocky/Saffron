namespace Spice.Saffron.ViewModels
{
    public class ServerRolesViewModel
    {
        public ServerRolesViewModel()
        {
            Roles = new List<RolesViewModel>();
        }

        public List<RolesViewModel> Roles { get; set; }
        public bool RetrievedFromCache { get; internal set; }
        public DateTimeOffset CachedAt { get; internal set; }
    }

    public class RolesViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Permissions { get; set; }
        public int Postition { get; set; }
        public long Color { get; set; }
        public bool Hoist { get; set; }
        public bool Managed { get; set; }
        public bool Mentionable { get; set; }
        public string Icon { get; set; }
        public string UnicodeEmoji { get; set; }
    }
}
