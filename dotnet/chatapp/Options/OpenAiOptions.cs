using System.ComponentModel.DataAnnotations;

namespace ChatApp.Options;

public class OpenAiOptions
{
    [Required]
    public string Key { get; set; } = string.Empty;

    [Url]
    [Required]
    public Uri Endpoint { get; set; }

    [Required]
    [RegularExpression("^[a-zA-Z0-9-]+$")]
    public string DeploymentId { get; set; } = string.Empty;
}