using ChatApp.Demos;
using Microsoft.Extensions.Options;

public class Main : IMain
{
    private readonly ILogger<Main> _logger;
    private readonly CliOptions _cliOptions;

    private readonly IEnumerable<IDemo> demos;

    private readonly IHostApplicationLifetime _applicationLifetime;

    public Main(
        IOptions<CliOptions> cliOptions,
        IEnumerable<IDemo> demos,
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration,
        ILogger<Main> logger)
    {
        _cliOptions = cliOptions.Value;

        this.demos = demos ?? throw new ArgumentNullException(nameof(demos));

        _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IConfiguration Configuration { get; set; }

    public async Task<int> RunAsync()
    {

        var demo = demos.FirstOrDefault(d => d.Name == _cliOptions.Name);

        if (demo is not null)
        {
            _logger.LogInformation("Demo: {Name}", _cliOptions.Name ?? "BasicDemo");

            await demo.RunAsync(_applicationLifetime.ApplicationStopping);
        }
        else
        {
            _logger.LogWarning("No demo with name: {Name}", _cliOptions.Name);
        }

        // use this token for stopping the services
        _applicationLifetime.ApplicationStopping.ThrowIfCancellationRequested();

        return 0;
    }
}
