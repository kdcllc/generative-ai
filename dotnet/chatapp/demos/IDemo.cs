namespace ChatApp.Demos;

public interface IDemo
{
    Task RunAsync(CancellationToken cancellationToken);
}
