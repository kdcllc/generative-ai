
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using ChatApp.Options;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.Connectors.Memory.Sqlite;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using Microsoft.SemanticKernel.Text;

namespace ChatApp.Services;

/// <summary>
/// This service requires to install:
/// dotnet add package Microsoft.SemanticKernel.Plugins.Memory --prerelease
/// </summary>
public class MemoryService
{
    private readonly IKernel? _kernel;
    private readonly IHttpClientFactory _clientFactory;
    private readonly MemoryOptions _memoryOptions;
    private readonly ILogger<MemoryService> _logger;
    private readonly OpenAiOptions _aiOptions;
    private ISemanticTextMemory _memory;

    public MemoryService(
        KernelService kernel,
        IHttpClientFactory clientFactory,
        IOptions<OpenAiOptions> aiOptions,
        IOptions<MemoryOptions> memoryOptions,
        IHostApplicationLifetime applicationLifetime,
        ILogger<MemoryService> logger)
    {
        _kernel = kernel.GetKernel();
        _aiOptions = aiOptions.Value;
        _clientFactory = clientFactory;
        _memoryOptions = memoryOptions.Value;
        _logger = logger;

        CreateMemoryStore(applicationLifetime.ApplicationStopping).GetAwaiter().GetResult();
    }

    public ISemanticTextMemory Memory => _memory;

    public async Task<string> AddContextFromUrlAsync(
        string url,
        CancellationToken cancellationToken)
    {
        var collectionName = Utils.HashUrl(url);

        var stored = await _memory.GetCollectionsAsync(cancellationToken);
        if (stored.Contains(collectionName))
        {
            _logger.LogInformation("Found stored url {url}", url);
            return collectionName;
        }

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

        return collectionName;
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

    private async Task CreateMemoryStore(CancellationToken cancellationToken)
    {
        if (_memory == null)
        {
            var builder = new MemoryBuilder()
                .WithLoggerFactory(_kernel!.LoggerFactory);

            if (_memoryOptions.IsInMemory)
            {
                builder.WithMemoryStore(new VolatileMemoryStore());
            }
            else if (!string.IsNullOrEmpty(_memoryOptions.Sqlite))
            {
                // dotnet add package Microsoft.SemanticKernel.Connectors.Memory.Sqlite --prerelease
                builder.WithMemoryStore(await SqliteMemoryStore.ConnectAsync(_memoryOptions.Sqlite!, cancellationToken));
            }

            if (_aiOptions.IsAzure)
            {
                builder.WithAzureTextEmbeddingGenerationService(_aiOptions.EmbeddingsId, _aiOptions.Endpoint.ToString(), _aiOptions.Key);
            }
            else
            {
                builder.WithOpenAITextEmbeddingGenerationService(_aiOptions.EmbeddingsId, _aiOptions.Key);
            }
            _memory = builder.Build();
        }
    }
}
