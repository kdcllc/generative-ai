
using ChatApp.Options;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace ChatApp.Demos;

/// <summary>
/// Executes questions against the OpenAI model.
/// if asked: "what is current time?" it does not know the answer.
/// </summary> 
public class BasicDemo : IDemo
{
    private readonly ILogger<BasicDemo> _logger;
    private readonly OpenAiOptions _openApiOptions;

    public BasicDemo(
        IOptions<OpenAiOptions> openApiOptions,
        ILogger<BasicDemo> logger)
    {
        _logger = logger;
        _openApiOptions = openApiOptions.Value;
    }

    //<inheritdoc/>
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("BasicDemo is running");
        _logger.LogInformation($"OpenApiOptions: {_openApiOptions.DeploymentId}");

        // Initialize the kernel
        IKernel kernel = Kernel.Builder
            .WithAzureChatCompletionService(
                _openApiOptions.DeploymentId,
                _openApiOptions.Endpoint.ToString(),
                _openApiOptions.Key)
            .Build();

        // Q&A loop
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Q: ");
            var question = Console.ReadLine();
            if (string.IsNullOrEmpty(question) || question.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
            var answer = await kernel.InvokeSemanticFunctionAsync(question!, maxTokens: 2000);
            _logger.LogInformation("A: {0}",answer);
        }
    }
}
