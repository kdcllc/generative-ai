namespace ChatApp.Demos;

public interface IDemo
{
    string Name { get; }

    Task RunAsync(CancellationToken cancellationToken);
}
