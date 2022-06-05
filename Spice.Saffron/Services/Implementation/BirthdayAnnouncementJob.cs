namespace Spice.Saffron.Services.Implementation
{
    public class BirthdayAnnouncementJob : CronJobService
    {
        private readonly ILogger<BirthdayAnnouncementJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        public BirthdayAnnouncementJob(IScheduleConfig<BirthdayAnnouncementJob> config, ILogger<BirthdayAnnouncementJob> logger, IServiceProvider serviceProvider)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BirthdayAnnouncementJob starts.");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Posting announcement");
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                IBirthdayAnnouncementService birthdayAnnouncementService =
                    scope.ServiceProvider.GetRequiredService<IBirthdayAnnouncementService>();

                await birthdayAnnouncementService.AnnounceAsync(cancellationToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ScheduleJob is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
