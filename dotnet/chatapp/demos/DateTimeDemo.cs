using ChatApp.Services;
using Microsoft.SemanticKernel;


namespace ChatApp.Demos;

/// <summary>
/// What is the current time in Israel?
/// What is time in Jerusalem Capital of Israel?
/// </summary>
public class DateTimeDemo : IDemo
{
    private readonly IKernel _kernel;
    private readonly ILogger<DateTimeDemo> _logger;

    public string Name => nameof(DateTimeDemo);

    public DateTimeDemo(
        KernelService kernelService,
        ILogger<DateTimeDemo> logger)
    {
        _kernel = kernelService.GetKernel()!;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DateTimeDemo is running");

        // Q&A loop
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Console.WriteLine("Question: ");

            var question = Console.ReadLine();
            if (string.IsNullOrEmpty(question) || question.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            // register native function with the kernel
            _kernel.RegisterCustomFunction(SKFunction.FromNativeFunction(
                // the delegate
                () => $"{DateTime.UtcNow:r}",
                // plugin name
                "DateTime",
                // native function name
                "Now",
                // description of the native plug in
                "Gets the current date and time"
            ));

            var prompt = _kernel.CreateSemanticFunction(
                """ 
                The current date and time is {{datetime.now}}.
                {{ $input}}

                The date and time is {{datetime.now}}.
                {{ $input}}
                """
            );

            var response = await prompt.InvokeAsync(question, _kernel, functions: _kernel.Functions);

            Console.WriteLine("Answer: {0}", response.GetValue<string>());
        }
    }
}