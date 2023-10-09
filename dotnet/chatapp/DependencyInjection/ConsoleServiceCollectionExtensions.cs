using ChatApp.Demos;
using ChatApp.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConsoleServiceCollectionExtensions
{
    /// <summary>
    /// Configures the services for the console application.
    /// </summary>
    /// <param name="hostBuilder">The host builder context.</param>
    /// <param name="services">The service collection.</param>
    public static void ConfigureServices(
        HostBuilderContext hostBuilder,
        IServiceCollection services)
    {
        services.AddScoped<IMain, Main>();

        // Register the OpenAiOptions with the service collection
        services.AddOptions<OpenAiOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                options.Key = configuration["OpenAiOptions:Key"];
                options.Endpoint = new Uri(configuration["OpenAiOptions:Endpoint"]);
                options.DeploymentId = configuration["OpenAiOptions:DeploymentId"];
            });
            //.ValidateDataAnnotations();

        // Register the IDemo implementations with the service collection
        services.AddTransient<IDemo, BasicDemo>();
    }

}
