namespace Spice.Saffron.Services
{
    public interface IBirthdayAnnouncementService
    {
        bool IsEnabled { get; }
        string ChannelId { get; }
        string CronExpression { get; }

        Task AnnounceAsync(CancellationToken cancellationToken = default);
        void SetAnnouncementPostingStatus(bool enabled);
    }
}
