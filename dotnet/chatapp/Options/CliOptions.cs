// description: options for the CLI

using CommandLine;

namespace ChatApp.Demos;

public class CliOptions 
{
    [Option('n', "name", Required = false, HelpText = nameof(BasicDemo))]
    public string? Name { get; set; }
}