
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using ChatApp.Options;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using Microsoft.SemanticKernel.Text;

namespace ChatApp.Services;

/// <summary>
/// This service requires to install:
/// dotnet add package Microsoft.SemanticKernel.Plugins.Memory --prerelease
/// </summary>
public class InMemoryService
{
    private readonly IKernel? _kernel;
    private readonly IHttpClientFactory _clientFactory;
    private readonly OpenAiOptions _options;
    private ISemanticTextMemory _memory;

    public InMemoryService(
        KernelService kernel,
        IHttpClientFactory clientFactory,
        IOptions<OpenAiOptions> options)
    {
        _kernel = kernel.GetKernel();
        _options = options.Value;
        _clientFactory = clientFactory;

        CreateMemoryStore();
    }

    public ISemanticTextMemory Memory => _memory;

    public async Task AddContextFromUrlAsync(string collectionName, string url, CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateClient();
        var response = await client.GetStringAsync(url, cancellationToken);

        var lines = TextChunker.SplitPlainTextLines(
            WebUtility.HtmlDecode(Regex.Replace(response, @"<[^>]+>|&nbsp;", "")),
            128);

        List<string> paragraphs = TextChunker.SplitPlainTextParagraphs(
                                    lines,
                                    1024);

        for (int i = 0; i < paragraphs.Count; i++)
        {
            await _memory.SaveInformationAsync(collectionName, paragraphs[i], $"paragraph{i}", cancellationToken: cancellationToken);
        }
    }

    public async Task<string> SearchContextAsync(
        string collectionName,
        string prompt,
        CancellationToken cancellationToken)
    {
        StringBuilder builder = new();
        await foreach (var result in _memory.SearchAsync(collectionName, prompt, limit: 3, cancellationToken: cancellationToken))
        {
            builder.AppendLine(result.Metadata.Text);
        }

        return builder.ToString();
    }

    private void CreateMemoryStore()
    {
        if (_memory == null)
        {
            var builder = new MemoryBuilder()
                .WithLoggerFactory(_kernel!.LoggerFactory)
                .WithMemoryStore(new VolatileMemoryStore());

            if (_options.IsAzure)
            {
                builder.WithAzureTextEmbeddingGenerationService(_options.EmbeddingsId, _options.Endpoint.ToString(), _options.Key);
            }
            else
            {
                builder.WithOpenAITextEmbeddingGenerationService(_options.EmbeddingsId, _options.Key);
            }
            _memory = builder.Build();
        }
    }
}
