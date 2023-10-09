using ChatApp.Demos;

public class Main : IMain
{
    private readonly ILogger<Main> _logger;
    private readonly IEnumerable<IDemo> demos;

    private readonly IHostApplicationLifetime _applicationLifetime;

    public Main(
        IEnumerable<IDemo> demos,
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration,
        ILogger<Main> logger)
    {
        this.demos = demos ?? throw new ArgumentNullException(nameof(demos));

        _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IConfiguration Configuration { get; set; }

    public async Task<int> RunAsync()
    {
        _logger.LogInformation("Main executed");

        foreach (var demo in demos)
        {
            await demo.RunAsync(_applicationLifetime.ApplicationStopping);
        }
        // use this token for stopping the services
        _applicationLifetime.ApplicationStopping.ThrowIfCancellationRequested();

        return 0;
    }
}
