using ChatApp.Options;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace ChatApp.Services;

public class KernelService
{
    private readonly OpenAiOptions _options;
    private readonly ILoggerFactory _loggerFactory;
    private KernelBuilder? _builder;

    public KernelService(
        IOptions<OpenAiOptions> options,
        ILoggerFactory loggerFactory)
    {
        _options = options.Value;
        _loggerFactory = loggerFactory;
    }

    public IKernel? GetKernel()
    {
        if (_builder == null)
        {
            CreateKernel();
        }

        return _builder?.Build();
    }

    private void CreateKernel()
    {
        _builder = new KernelBuilder();

        // this logging will display information like Prompt tokens: 75. Completion tokens: 43. Total tokens: 118.
        _builder.WithLoggerFactory(_loggerFactory);

        if (_options.IsAzure)
        {
            _builder.WithAzureChatCompletionService(
                _options.ModelId,
                _options.Endpoint.ToString(),
                _options.Key);
        }
        else
        {
            _builder.WithOpenAIChatCompletionService(
                _options.Endpoint.ToString(),
                _options.Key);
        }
    }
}