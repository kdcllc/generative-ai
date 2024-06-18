using ChatApp.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI;

namespace ChatApp.Demos;

/// <summary>
/// Executes questions against the (Azure) OpenAI model.
/// - if asked: "what is current time in Israel?" it does not know the answer.
/// - doesn't have the context for the next prediction.
/// - the chat is stateless
/// </summary> 
public class BasicDemo : IDemo
{
    private readonly ILogger<BasicDemo> _logger;
    private readonly IKernel _kernel;

    public string Name => nameof(BasicDemo);

    public BasicDemo(
        KernelService kernelService,
        ILogger<BasicDemo> logger)
    {
        _kernel = kernelService.GetKernel()!;
        _logger = logger;
    }

    //<inheritdoc/>
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("BasicDemo is running");

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

            var options = new AIRequestSettings
            {
                ExtensionData = new Dictionary<string, object>{
                    {"temperature", 0.9},
                    {"max_tokens", 2000},
                    {"top_p", 1.0},
                    {"frequency_penalty", 0.0},
                    {"presence_penalty", 0.0},
                    {"stop", "\n"}
                }
            };

            var answer = await _kernel.InvokeSemanticFunctionAsync(question!, requestSettings: options);
            Console.WriteLine("Answer: {0}", answer.GetValue<string>());
        }
    }
}
