using ChatApp.Demos;
using ChatApp.Options;
using ChatApp.Services;

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

        // add scoped instance of the kernel service
        services.AddSingleton<KernelService>();
        services.AddSingleton<MemoryService>();

        // add this for IHttpFactory.
        services.AddHttpClient();

        // Register the OpenAiOptions with the service collection
        services.AddOptions<OpenAiOptions>()
            .Bind(hostBuilder.Configuration.GetSection("OpenAiOptions"))
            .ValidateDataAnnotations();

        services.Configure<MemoryOptions>(hostBuilder.Configuration.GetSection("MemoryOptions"));


        // Register the IDemo implementations with the service collection
        services.AddTransient<IDemo, BasicDemo>();
        services.AddTransient<IDemo, DateTimeDemo>();
        services.AddTransient<IDemo, MemoryDemo>();
    }

}
