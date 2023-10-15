using System.ComponentModel.DataAnnotations;

namespace ChatApp.Options;

/// <summary>
/// Represents the options for OpenAI API or Azure OpenAI Api.
/// </summary>
public class OpenAiOptions
{
    /// <summary>
    /// Gets or sets the API key for OpenAI.
    /// </summary>
    [Required]
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the endpoint URL for OpenAI.
    /// </summary>
    //[Url]
    [Required]
    public Uri Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the deployment ID for OpenAI.
    /// </summary>
    [Required]
    [RegularExpression("^[a-zA-Z0-9-]+$")]
    public string ModelId { get; set; } = string.Empty;


    /// <summary>
    /// Add Embeddings Id that are deployed within the services.
    /// </summary>
    public string EmbeddingsId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the OpenAI API is hosted on Azure.
    /// </summary>
    public bool IsAzure { get; set; }
}