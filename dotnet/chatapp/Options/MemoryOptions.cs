namespace ChatApp.Options;

public class MemoryOptions
{
    public bool IsInMemory { get; set; }

    public string? Sqlite { get; set; }

    public string? QdrantUrl { get; set; }
}