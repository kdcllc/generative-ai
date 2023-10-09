using ChatApp.Options;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace ChatApp.Demos;

public class KernelService
{
    private readonly OpenAiOptions _options;
    private KernelBuilder? _builder;

    public KernelService(IOptions<OpenAiOptions> options)
    {
        _options = options.Value;
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

        if (_options.IsAzure)
        {
            _builder.WithAzureChatCompletionService(
                _options.DeploymentId,
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